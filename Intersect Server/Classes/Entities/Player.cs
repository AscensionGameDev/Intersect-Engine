using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.Localization;
using Intersect.Server.Classes.General;
using Intersect.Server.Classes.Items;
using Intersect.Server.Classes.Maps;
using Intersect.Server.Classes.Networking;
using Intersect.Server.Classes.Spells;

namespace Intersect.Server.Classes.Entities
{
    using Database = Intersect.Server.Classes.Core.Database;

    public class Player : Entity
    {
        //5 minute timeout before someone can send a trade/party request after it has been declined
        public const long REQUEST_DECLINE_TIMEOUT = 300000;

        private bool mSentMap;
        public ItemInstance[] Bank = new ItemInstance[Options.MaxBankSlots];
        public Player ChatTarget = null;
        public int Class = 0;
        public int CraftIndex = -1;
        public long CraftTimer = 0;
        public int[] Equipment = new int[Options.EquipmentSlots.Count];

        //Temporary Values
        private object mEventLock = new object();

        public ConcurrentDictionary<Tuple<int, int, int>, EventInstance> EventLookup = new ConcurrentDictionary<Tuple<int, int, int>, EventInstance>();
        private int mEventCounter = 0;
        private int mCommonEventLaunches = 0;
        public int Experience;
        public Player FriendRequester;
        public Dictionary<Player, long> FriendRequests = new Dictionary<Player, long>();
        public Dictionary<int, string> Friends = new Dictionary<int, string>();
        public int Gender = 0;
        public HotbarInstance[] Hotbar = new HotbarInstance[Options.MaxHotbar];
        public int InBag = -1;
        public bool InBank;
        public int InCraft = -1;
        public bool InGame;
        public int InShop = -1;
        public int LastMapEntered = -1;
        public Client MyClient;

        public long MyId = -1;
        public List<Player> Party = new List<Player>();
        public Player PartyRequester;
        public Dictionary<Player, long> PartyRequests = new Dictionary<Player, long>();
        public List<int> QuestOffers = new List<int>();
        public Dictionary<int, QuestProgressStruct> Quests = new Dictionary<int, QuestProgressStruct>();

        public long SaveTimer = Environment.TickCount;

        //Event Spawned Npcs
        public List<Npc> SpawnedNpcs = new List<Npc>();

        public int StatPoints;
        public Dictionary<int, bool> Switches = new Dictionary<int, bool>();
        public ItemInstance[] Trade = new ItemInstance[Options.MaxInvItems];
        public bool TradeAccepted;
        public Player TradeRequester;
        public Dictionary<Player, long> TradeRequests = new Dictionary<Player, long>();
        public int Trading = -1;
        public Dictionary<int, int> Variables = new Dictionary<int, int>();

        //Init
        public Player(int index, Client newClient) : base(index)
        {
            MyClient = newClient;
            for (var i = 0; i < Options.MaxPlayerSkills; i++)
            {
                Spells.Add(new SpellInstance());
            }
            for (var i = 0; i < Options.MaxInvItems; i++)
            {
                Inventory.Add(new ItemInstance(-1, 0, -1));
            }
            for (var i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                Equipment[i] = -1;
            }
            for (var i = 0; i < Options.MaxHotbar; i++)
            {
                Hotbar[i] = new HotbarInstance();
            }
            for (var I = 0; I < (int)Stats.StatCount; I++)
            {
                Stat[I] = new EntityStat(0, I, this);
            }
        }

        public bool IsValidPlayer
        {
            get
            {
                if (IsDisposed)
                {
                    return false;
                }

                return (MyClient != null && MyClient.Entity == this);
            }
        }

        //Update
        public override void Update(long timeMs)
        {
            if (!InGame || CurrentMap == -1)
            {
                return;
            }

            if (SaveTimer + 120000 < timeMs)
            {
                Task.Run(() => Database.SaveCharacter(this, false));
                SaveTimer = timeMs;
            }

            if (InCraft > -1 && CraftIndex > -1)
            {
                var b = BenchBase.Lookup.Get<BenchBase>(InCraft);
                if (CraftTimer + b.Crafts[CraftIndex].Time < timeMs)
                {
                    CraftItem(CraftIndex);
                }
                else
                {
                    if (!CheckCrafting(CraftIndex))
                    {
                        CraftIndex = -1;
                    }
                }
            }

            base.Update(timeMs);

            //Check for autorun common events and run them
            foreach (EventBase evt in EventBase.Lookup.IndexValues)
            {
                if (evt != null)
                {
                    StartCommonEvent(evt, (int)EventPage.CommonEventTriggers.Autorun);
                }
            }

            //If we have a move route then let's process it....
            if (MoveRoute != null && MoveTimer < timeMs)
            {
                //Check to see if the event instance is still active for us... if not then let's remove this route
                var foundEvent = false;
                foreach (var evt in EventLookup.Values)
                {
                    if (evt.PageInstance == MoveRouteSetter)
                    {
                        foundEvent = true;
                        if (MoveRoute.ActionIndex < MoveRoute.Actions.Count)
                        {
                            ProcessMoveRoute(MyClient, timeMs);
                        }
                        else
                        {
                            if (MoveRoute.Complete && !MoveRoute.RepeatRoute)
                            {
                                MoveRoute = null;
                                MoveRouteSetter = null;
                                PacketSender.SendMoveRouteToggle(MyClient, false);
                            }
                        }
                        break;
                    }
                }
                if (!foundEvent)
                {
                    MoveRoute = null;
                    MoveRouteSetter = null;
                    PacketSender.SendMoveRouteToggle(MyClient, false);
                }
            }

            //If we switched maps, lets update the maps
            if (LastMapEntered != CurrentMap)
            {
                if (MapInstance.Lookup.Get<MapInstance>(LastMapEntered) != null)
                {
                    MapInstance.Lookup.Get<MapInstance>(LastMapEntered).RemoveEntity(this);
                }
                if (CurrentMap > -1)
                {
                    if (!MapInstance.Lookup.IndexKeys.Contains(CurrentMap))
                    {
                        WarpToSpawn();
                    }
                    else
                    {
                        MapInstance.Lookup.Get<MapInstance>(CurrentMap).PlayerEnteredMap(this);
                    }
                }
            }

            var currentMap = MapInstance.Lookup.Get<MapInstance>(CurrentMap);
            if (currentMap != null)
            {
                for (var i = 0; i < currentMap.SurroundingMaps.Count + 1; i++)
                {
                    MapInstance map = null;
                    if (i == currentMap.SurroundingMaps.Count)
                    {
                        map = currentMap;
                    }
                    else
                    {
                        map = MapInstance.Lookup.Get<MapInstance>(currentMap.SurroundingMaps[i]);
                    }
                    if (map == null) continue;
                    lock (map.GetMapLock())
                    {
                        //Check to see if we can spawn events, if already spawned.. update them.
                        lock (mEventLock)
                        {
                            foreach (var mapEvent in map.Events.Values)
                            {
                                //Look for event
                                var foundEvent = EventExists(map.Index, mapEvent.SpawnX, mapEvent.SpawnY);
                                if (foundEvent == null)
                                {
                                    var tmpEvent = new EventInstance(mEventCounter++, MyClient, mapEvent, map.Index)
                                    {
                                        IsGlobal = mapEvent.IsGlobal == 1,
                                        MapNum = map.Index,
                                        SpawnX = mapEvent.SpawnX,
                                        SpawnY = mapEvent.SpawnY
                                    };
                                    EventLookup.AddOrUpdate(new Tuple<int, int, int>(map.Index, mapEvent.SpawnX, mapEvent.SpawnY), tmpEvent, (key, oldValue) => tmpEvent);
                                }
                                else
                                {
                                    foundEvent.Update(timeMs);
                                }
                            }
                        }
                    }
                }
            }
            //Check to see if we can spawn events, if already spawned.. update them.
            lock (mEventLock)
            {
                foreach (var evt in EventLookup.Values)
                {
                    if (evt == null) continue;
                    var eventFound = false;
                    if (evt.MapNum < 0)
                    {
                        evt.Update(timeMs);
                        if (evt.CallStack.Count > 0)
                        {
                            eventFound = true;
                        }
                    }
                    if (evt.MapNum != CurrentMap)
                    {
                        foreach (var t in MapInstance.Lookup.Get<MapInstance>(CurrentMap).SurroundingMaps)
                        {
                            if (t == evt.MapNum)
                            {
                                eventFound = true;
                            }
                        }
                    }
                    else
                    {
                        eventFound = true;
                    }
                    if (eventFound) continue;
                    PacketSender.SendEntityLeaveTo(MyClient, evt.MyIndex, (int)EntityTypes.Event, evt.MapNum);
                    EventLookup.TryRemove(new Tuple<int, int, int>(evt.MapNum, evt.MapNum < 0 ? -1 : evt.BaseEvent.SpawnX, evt.MapNum < 0 ? -1 : evt.BaseEvent.SpawnY), out EventInstance z);
                }
            }
        }

        //Sending Data
        public override byte[] Data()
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(base.Data());
            bf.WriteInteger(Gender);
            bf.WriteInteger(Class);
            return bf.ToArray();
        }

        public override EntityTypes GetEntityType()
        {
            return EntityTypes.Player;
        }

        //Spawning/Dying
        private void Respawn()
        {
            //Remove any damage over time effects
            DoT.Clear();

            var cls = ClassBase.Lookup.Get<ClassBase>(Class);
            if (cls != null)
            {
                Warp(cls.SpawnMap, cls.SpawnX, cls.SpawnY, cls.SpawnDir);
            }
            else
            {
                Warp(0, 0, 0, 0);
            }
            PacketSender.SendEntityDataToProximity(this);
            //Search death common event trigger
            foreach (EventBase evt in EventBase.Lookup.IndexValues)
            {
                if (evt != null)
                {
                    StartCommonEvent(evt, (int)EventPage.CommonEventTriggers.OnRespawn);
                }
            }
        }

        public override void Die(int dropitems = 0, Entity killer = null)
        {
            //Flag death to the client
            PacketSender.SendPlayerDeath(this);

            //Event trigger
            foreach (var evt in EventLookup.Values)
            {
                evt.PlayerHasDied = true;
            }

            base.Die(dropitems, killer);
            Reset();
            Respawn();
            PacketSender.SendInventory(MyClient);
        }

        public override void ProcessRegen()
        {
            Debug.Assert(ClassBase.Lookup != null, "ClassBase.Lookup != null");

            var playerClass = ClassBase.Lookup.Get<ClassBase>(Class);
            if (playerClass?.VitalRegen == null) return; ;

            foreach (Vitals vital in Enum.GetValues(typeof(Vitals)))
            {
                Debug.Assert(Vital != null, "Vital != null");
                Debug.Assert(MaxVital != null, "MaxVital != null");

                if (vital >= Vitals.VitalCount) continue;

                var vitalId = (int)vital;
                var vitalValue = Vital[vitalId];
                var maxVitalValue = MaxVital[vitalId];
                if (vitalValue >= maxVitalValue) continue;

                var vitalRegenRate = playerClass.VitalRegen[vitalId] / 100f;
                var regenValue = (int)Math.Max(1, maxVitalValue * vitalRegenRate) * Math.Abs(Math.Sign(vitalRegenRate));
                AddVital(vital, regenValue);
            }
        }

        //Leveling
        public void SetLevel(int level, bool resetExperience = false)
        {
            if (level > 0)
            {
                Level = Math.Min(Options.MaxLevel, level);
                if (resetExperience) Experience = 0;
                PacketSender.SendEntityDataToProximity(this);
                PacketSender.SendExperience(MyClient);
            }
        }

        public void LevelUp(bool resetExperience = true, int levels = 1)
        {
            var spellMsgs = new List<string>();
            if (Level < Options.MaxLevel)
            {
                for (var i = 0; i < levels; i++)
                {
                    SetLevel(Level + 1, resetExperience);
                    //Let's pull up class - leveling info
                    var myclass = ClassBase.Lookup.Get<ClassBase>(Class);
                    if (myclass != null)
                    {
                        foreach (Vitals vital in Enum.GetValues(typeof(Vitals)))
                        {
                            if ((int)vital < (int)Vitals.VitalCount)
                            {
                                var maxVital = MaxVital[(int)vital];
                                if (myclass.IncreasePercentage == 1)
                                {
                                    maxVital =
                                        (int)
                                        (MaxVital[(int)vital] *
                                         (1f + ((float)myclass.VitalIncrease[(int)vital] / 100f)));
                                }
                                else
                                {
                                    maxVital = MaxVital[(int)vital] + myclass.VitalIncrease[(int)vital];
                                }
                                var vitalDiff = maxVital - MaxVital[(int)vital];
                                MaxVital[(int)vital] = maxVital;
                                AddVital(vital, vitalDiff);
                            }
                        }

                        foreach (Stats stat in Enum.GetValues(typeof(Stats)))
                        {
                            if ((int)stat < (int)Stats.StatCount)
                            {
                                var newStat = Stat[(int)stat].Stat;
                                if (myclass.IncreasePercentage == 1)
                                {
                                    newStat =
                                        (int)
                                        (Stat[(int)stat].Stat *
                                         (1f + ((float)myclass.StatIncrease[(int)stat] / 100f)));
                                }
                                else
                                {
                                    newStat = Stat[(int)stat].Stat + myclass.StatIncrease[(int)stat];
                                }
                                var statDiff = newStat - Stat[(int)stat].Stat;
                                AddStat(stat, statDiff);
                            }
                        }

                        foreach (var spell in myclass.Spells)
                        {
                            if (spell.Level == Level)
                            {
                                var spellInstance = new SpellInstance();
                                spellInstance.SpellNum = spell.SpellNum;
                                if (TryTeachSpell(spellInstance, true))
                                {
                                    spellMsgs.Add(Strings.Get("player","spelltaughtlevelup",SpellBase.GetName(spellInstance.SpellNum)));
                                }
                            }
                        }
                    }
                    StatPoints += myclass.PointIncrease;
                }
            }

            PacketSender.SendPlayerMsg(MyClient, Strings.Get("player", "levelup", Level), CustomColors.LevelUp, MyName);
            PacketSender.SendActionMsg(this, Strings.Get("combat", "levelup"), CustomColors.LevelUp);
            foreach (var msg in spellMsgs)
            {
                PacketSender.SendPlayerMsg(MyClient, msg, CustomColors.Info, MyName);
            }
            if (StatPoints > 0)
            {
                PacketSender.SendPlayerMsg(MyClient, Strings.Get("player", "statpoints", StatPoints),
                    CustomColors.StatPoints, MyName);
            }
            PacketSender.SendExperience(MyClient);
            PacketSender.SendPointsTo(MyClient);
            PacketSender.SendEntityDataToProximity(this);

            //Search for login activated events and run them
            foreach (EventBase evt in EventBase.Lookup.IndexValues)
            {
                if (evt != null)
                {
                    StartCommonEvent(evt, (int)EventPage.CommonEventTriggers.LevelUp);
                }
            }
        }

        public void GiveExperience(int amount)
        {
            Experience += amount;
            if (!CheckLevelUp())
            {
                PacketSender.SendExperience(MyClient);
            }
        }

        private bool CheckLevelUp()
        {
            var levelCount = 0;
            while (Experience >= GetExperienceToNextLevel() && GetExperienceToNextLevel() > 0)
            {
                Experience -= GetExperienceToNextLevel();
                levelCount++;
            }
            if (levelCount > 0)
            {
                LevelUp(false, levelCount);
                return true;
            }
            return false;
        }

        public int GetExperienceToNextLevel()
        {
            if (Level >= Options.MaxLevel) return -1;
            var myclass = ClassBase.Lookup.Get<ClassBase>(Class);
            if (myclass != null)
            {
                return (int)(myclass.BaseExp * Math.Pow(1 + (myclass.ExpIncrease / 100f) / 1, Level - 1));
            }
            return 1000;
        }

        //Combat
        public override void KilledEntity(Entity en)
        {
            if (en.GetType() == typeof(Npc))
            {
                if (Party.Count > 0) //If in party, split the exp.
                {
                    for (var i = 0; i < Party.Count; i++)
                    {
                        Party[i].GiveExperience(((Npc)en).MyBase.Experience / Party.Count);
						Party[i].UpdateQuestKillTasks(en);
					}
                }
                else
                {
                    GiveExperience(((Npc)en).MyBase.Experience);
					UpdateQuestKillTasks(en);
				}
            }
        }

		public void UpdateQuestKillTasks(Entity en)
		{
			//If any quests demand that this Npc be killed then let's handle it
			var npc = (Npc)en;
			for (var i = 0; i < Quests.Keys.Count; i++)
			{
				var questId = Quests.Keys.ToArray()[i];
				var quest = QuestBase.Lookup.Get<QuestBase>(questId);
				if (quest != null)
				{
					if (Quests[questId].Task > -1)
					{
						//Assume this quest is in progress. See if we can find the task in the quest
						var questTask = quest.FindTask(Quests[questId].Task);
						if (questTask != null)
						{
							if (questTask.Objective == 2 && questTask.Data1 == npc.MyBase.Index) //kill npcs
							{
								var questProg = Quests[questId];
								questProg.TaskProgress++;
								if (questProg.TaskProgress >= questTask.Data2)
								{
									CompleteQuestTask(questId, Quests[questId].Task);
								}
								else
								{
									Quests[questId] = questProg;
									PacketSender.SendQuestProgress(this, quest.Index);
									PacketSender.SendPlayerMsg(MyClient,
										Strings.Get("quests", "npctask", quest.Name, questProg.TaskProgress,
											questTask.Data2, NpcBase.GetName(questTask.Data1)));
								}
							}
						}
					}
				}
			}
		}

        public override void TryAttack(Entity enemy, ProjectileBase projectile, SpellBase parentSpell, ItemBase parentItem, int projectileDir)
        {
            //If Entity is resource, check for the correct tool and make sure its not a spell cast.
            if (enemy.GetType() == typeof(Resource))
            {
                if (((Resource)enemy).IsDead) return;
                // Check that a resource is actually required.
                var resource = ((Resource)enemy).MyBase;
                //Check Dynamic Requirements
                if (!EventInstance.MeetsConditionLists(resource.HarvestingReqs, this, null))
                {
                    PacketSender.SendPlayerMsg(MyClient, Strings.Get("combat", "resourcereqs"));
                    return;
                }
                if (resource.Tool > -1 && resource.Tool < Options.ToolTypes.Count)
                {
                    if (parentItem == null || resource.Tool != parentItem.Tool)
                    {
                        PacketSender.SendPlayerMsg(MyClient,
                            Strings.Get("combat", "toolrequired", Options.ToolTypes[resource.Tool]));
                        return;
                    }
                }
            }
            base.TryAttack(enemy, projectile, parentSpell, parentItem, projectileDir);
        }

        public override void TryAttack(Entity enemy)
        {
            if (CastTime >= Globals.System.GetTimeMs())
            {
                PacketSender.SendPlayerMsg(MyClient, Strings.Get("combat", "channelingnoattack"));
                return;
            }

            if (!IsOneBlockAway(enemy)) return;
            if (!IsFacingTarget(enemy)) return;
            if (enemy.GetType() == typeof(Npc) && ((Npc)enemy).MyBase.Behavior == (int)NpcBehavior.Friendly) return;
            if (enemy.GetType() == typeof(EventPageInstance)) return;

            ItemBase weapon = null;
            if (((Player)Globals.Entities[MyIndex]).Equipment[Options.WeaponIndex] >= 0)
            {
                weapon =
                    ItemBase.Lookup.Get<ItemBase>(
                        Inventory[((Player)Globals.Entities[MyIndex]).Equipment[Options.WeaponIndex]].ItemNum);
            }

            //If Entity is resource, check for the correct tool and make sure its not a spell cast.
            if (enemy.GetType() == typeof(Resource))
            {
                if (((Resource)enemy).IsDead) return;
                // Check that a resource is actually required.
                var resource = ((Resource)enemy).MyBase;
                //Check Dynamic Requirements
                if (!EventInstance.MeetsConditionLists(resource.HarvestingReqs, this, null))
                {
                    PacketSender.SendPlayerMsg(MyClient, Strings.Get("combat", "resourcereqs"));
                    return;
                }
                if (resource.Tool > -1 && resource.Tool < Options.ToolTypes.Count)
                {
                    if (weapon == null || resource.Tool != weapon.Tool)
                    {
                        PacketSender.SendPlayerMsg(MyClient,
                            Strings.Get("combat", "toolrequired", Options.ToolTypes[resource.Tool]));
                        return;
                    }
                }
            }

            if (weapon != null)
            {
                base.TryAttack(enemy, weapon.Damage, (DamageType)weapon.DamageType,
                    (Stats)weapon.ScalingStat,
                    weapon.Scaling, weapon.CritChance, Options.CritMultiplier);
            }
            else
            {
                var classBase = ClassBase.Lookup.Get<ClassBase>(Class);
                if (classBase != null)
                {
                    base.TryAttack(enemy, classBase.Damage,
                        (DamageType)classBase.DamageType, (Stats)classBase.ScalingStat,
                        classBase.Scaling, classBase.CritChance, Options.CritMultiplier);
                }
                else
                {
                    base.TryAttack(enemy, 1, (DamageType)DamageType.Physical, Stats.Attack,
                        100, 10, Options.CritMultiplier);
                }
            }
            PacketSender.SendEntityAttack(this, (int)EntityTypes.GlobalEntity, CurrentMap, CalculateAttackTime());
        }

        public override bool CanAttack(Entity en, SpellBase spell)
        {
            if (!base.CanAttack(en,spell)) return false;
            if (en.GetType() == typeof(Npc) && ((Npc)en).MyBase.Behavior == (int)NpcBehavior.Friendly) return false;
            if (en.GetType() == typeof(EventPageInstance)) return false;
            //Check if the attacker is stunned or blinded.
            var statuses = Statuses.Values.ToArray();
            foreach (var status in statuses)
            {
                if (status.Type == (int)StatusTypes.Stun)
                {
                    return false;
                }
            }
            if (en.GetType() == typeof(Player))
            {
                if (spell != null && spell.Friendly == 1)
                {
                    return true;
                }
                return false;
            }
            else if (en.GetType() == typeof(Resource))
            {
                if (spell != null) return false;
            }
            return true;
        }

        //Warping
        public override void Warp(int newMap, int newX, int newY, bool adminWarp = false)
        {
            Warp(newMap, newX, newY, 1, adminWarp,0,false);
        }

        public override void Warp(int newMap, int newX, int newY, int newDir, bool adminWarp = false, int zOverride = 0, bool mapSave = false)
        {
            var map = MapInstance.Lookup.Get<MapInstance>(newMap);
            if (map == null)
            {
                WarpToSpawn();
                return;
            }
            CurrentX = newX;
            CurrentY = newY;
            CurrentZ = zOverride;
            Dir = newDir;
            foreach (var evt in EventLookup.Values)
            {
                if (evt.MapNum > -1 && (evt.MapNum != newMap || mapSave))
                {
                    EventLookup.TryRemove(new Tuple<int, int, int>(evt.MapNum, evt.BaseEvent.SpawnX, evt.BaseEvent.SpawnY), out EventInstance z);
                }
            }
            if (newMap != CurrentMap || mSentMap == false)
            {
                var oldMap = MapInstance.Lookup.Get<MapInstance>(CurrentMap);
                if (oldMap != null)
                {
                    oldMap.RemoveEntity(this);
                }
                PacketSender.SendEntityLeave(MyIndex, (int)EntityTypes.Player, CurrentMap);
                CurrentMap = newMap;
                map.PlayerEnteredMap(this);
                PacketSender.SendEntityDataToProximity(this);
                PacketSender.SendEntityPositionToAll(this);

				//If map grid changed then send the new map grid
				if (!adminWarp && (oldMap == null || !oldMap.SurroundingMaps.Contains(newMap)))
                {
                    PacketSender.SendMapGrid(MyClient, map.MapGrid, true);
                }

                var surroundingMaps = map.GetSurroundingMaps(true);
                foreach (var surrMap in surroundingMaps)
                {
                    PacketSender.SendMap(MyClient, surrMap.Index);
                }
                mSentMap = true;
            }
            else
            {
                PacketSender.SendEntityPositionToAll(this);
                PacketSender.SendEntityVitals(this);
                PacketSender.SendEntityStats(this);
            }
        }

        public void WarpToSpawn(bool sendWarp = false)
        {
            int map = -1, x = 0, y = 0, dir = 0;
            var cls = ClassBase.Lookup.Get<ClassBase>(Class);
            if (cls != null)
            {
                if (MapInstance.Lookup.IndexKeys.Contains(cls.SpawnMap))
                {
                    map = cls.SpawnMap;
                }
                x = cls.SpawnX;
                y = cls.SpawnY;
                dir = cls.SpawnDir;
            }
            if (map == -1)
            {
                using (var mapenum = MapInstance.Lookup.GetEnumerator())
                {
                    mapenum.MoveNext();
                    map = mapenum.Current.Value.Index;
                }
            }
            Warp(map, x, y,dir);
        }

        //Inventory
        public bool CanGiveItem(ItemInstance item)
        {
            var itemBase = ItemBase.Lookup.Get<ItemBase>(item.ItemNum);
            if (itemBase != null)
            {
                if (itemBase.IsStackable())
                {
                    for (var i = 0; i < Options.MaxInvItems; i++)
                    {
                        if (Inventory[i].ItemNum == item.ItemNum)
                        {
                            return true;
                        }
                    }
                }

                //Either a non stacking item, or we couldn't find the item already existing in the players inventory
                for (var i = 0; i < Options.MaxInvItems; i++)
                {
                    if (Inventory[i].ItemNum == -1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool TryGiveItem(ItemInstance item, bool sendUpdate = true)
        {
            var itemBase = ItemBase.Lookup.Get<ItemBase>(item.ItemNum);
            if (itemBase != null)
            {
                if (itemBase.IsStackable())
                {
                    for (var i = 0; i < Options.MaxInvItems; i++)
                    {
                        if (Inventory[i].ItemNum == item.ItemNum)
                        {
                            Inventory[i].ItemVal += item.ItemVal;
                            if (sendUpdate)
                            {
                                PacketSender.SendInventoryItemUpdate(MyClient, i);
                            }
                            UpdateGatherItemQuests(item.ItemNum);
                            return true;
                        }
                    }
                }

                //Either a non stacking item, or we couldn't find the item already existing in the players inventory
                for (var i = 0; i < Options.MaxInvItems; i++)
                {
                    if (Inventory[i].ItemNum == -1)
                    {
                        Inventory[i] = item.Clone();
                        if (sendUpdate)
                        {
                            PacketSender.SendInventoryItemUpdate(MyClient, i);
                        }
                        UpdateGatherItemQuests(item.ItemNum);
                        return true;
                    }
                }
            }
            return false;
        }

        public void SwapItems(int item1, int item2)
        {
            var tmpInstance = Inventory[item2].Clone();
            Inventory[item2] = Inventory[item1].Clone();
            Inventory[item1] = tmpInstance.Clone();
            PacketSender.SendInventoryItemUpdate(MyClient, item1);
            PacketSender.SendInventoryItemUpdate(MyClient, item2);
            EquipmentProcessItemSwap(item1, item2);
            HotbarProcessItemSwap(item1, item2);
        }

        public void DropItems(int slot, int amount)
        {
            var itemBase = ItemBase.Lookup.Get<ItemBase>(Inventory[slot].ItemNum);
            if (itemBase != null)
            {
                if (itemBase.Bound > 0)
                {
                    PacketSender.SendPlayerMsg(MyClient, Strings.Get("items", "bound"), CustomColors.ItemBound);
                    return;
                }
                for (var i = 0; i < Options.EquipmentSlots.Count; i++)
                {
                    if (Equipment[i] == slot)
                    {
                        PacketSender.SendPlayerMsg(MyClient, Strings.Get("items", "equipped"), CustomColors.ItemBound);
                        return;
                    }
                }

                if (amount >= Inventory[slot].ItemVal)
                {
                    amount = Inventory[slot].ItemVal;
                }
                if (itemBase.IsStackable())
                {
                    MapInstance.Lookup.Get<MapInstance>(CurrentMap)
                        .SpawnItem(CurrentX, CurrentY, Inventory[slot], amount);
                }
                else
                {
                    for (var i = 0; i < amount; i++)
                    {
                        MapInstance.Lookup.Get<MapInstance>(CurrentMap)
                            .SpawnItem(CurrentX, CurrentY, Inventory[slot], 1);
                    }
                }
                if (amount == Inventory[slot].ItemVal)
                {
                    Inventory[slot] = new ItemInstance(-1, 0, -1);
                    EquipmentProcessItemLoss(slot);
                }
                else
                {
                    Inventory[slot].ItemVal -= amount;
                }
                UpdateGatherItemQuests(itemBase.Index);
                PacketSender.SendInventoryItemUpdate(MyClient, slot);
            }
        }

        public void UseItem(int slot)
        {
            var equipped = false;
            var itemInstance = Inventory[slot];
            var itemBase = ItemBase.Lookup.Get<ItemBase>(itemInstance.ItemNum);
            if (itemBase != null)
            {
                //Check if the user is silenced or stunned
                var statuses = Statuses.Values.ToArray();
                foreach (var status in statuses)
                {
                    if (status.Type == (int)StatusTypes.Stun)
                    {
                        PacketSender.SendPlayerMsg(MyClient, Strings.Get("items", "stunned"));
                        return;
                    }
                }

                if (!EventInstance.MeetsConditionLists(itemBase.UseReqs, this, null))
                {
                    PacketSender.SendPlayerMsg(MyClient, Strings.Get("items", "dynamicreq"));
                    return;
                }

                switch (itemBase.ItemType)
                {
                    case (int)ItemTypes.None:
                    case (int)ItemTypes.Currency:
                        PacketSender.SendPlayerMsg(MyClient, Strings.Get("items", "cannotuse"));
                        return;
                    case (int)ItemTypes.Consumable:
                        var s = "";
                        if (itemBase.Data2 > 0)
                        {
                            s = Strings.Get("combat", "addsymbol");
                        }

                        switch (itemBase.Data1)
                        {
                            case 0: //Health
                                AddVital(Vitals.Health, itemBase.Data2);
                                if (s == Strings.Get("combat", "addsymbol"))
                                {
                                    PacketSender.SendActionMsg(this, s + itemBase.Data2, CustomColors.Heal);
                                }
                                else
                                {
                                    PacketSender.SendActionMsg(this, s + itemBase.Data2, CustomColors.PhysicalDamage);
                                    if (Vital[(int)Vitals.Health] <= 0) //Add a death handler for poison.
                                    {
                                        Die();
                                    }
                                }
                                break;
                            case 1: //Mana
                                AddVital(Vitals.Mana, itemBase.Data2);
                                PacketSender.SendActionMsg(this, s + itemBase.Data2, CustomColors.AddMana);
                                break;
                            case 2: //Exp
                                GiveExperience(itemBase.Data2);
                                PacketSender.SendActionMsg(this, s + itemBase.Data2, CustomColors.Experience);
                                break;
                            default:
                                return;
                        }
                        TakeItemsBySlot(slot, 1);
                        break;
                    case (int)ItemTypes.Equipment:
                        for (var i = 0; i < Options.EquipmentSlots.Count; i++)
                        {
                            if (Equipment[i] == slot)
                            {
                                Equipment[i] = -1;
                                equipped = true;
                            }
                        }
                        if (!equipped)
                        {
                            if (itemBase.Data1 == Options.WeaponIndex)
                            {
                                if (Options.WeaponIndex > -1)
                                {
                                    //If we are equipping a 2hand weapon, remove the shield
                                    if (Convert.ToBoolean(itemBase.Data4))
                                    {
                                        Equipment[Options.ShieldIndex] = -1;
                                    }
                                    Equipment[Options.WeaponIndex] = slot;
                                }
                            }
                            else if (itemBase.Data1 == Options.ShieldIndex)
                            {
                                if (Options.ShieldIndex > -1)
                                {
                                    if (Equipment[Options.WeaponIndex] > -1)
                                    {
                                        //If we have a 2-hand weapon, remove it to equip this new shield
                                        if (ItemBase.Lookup.Get<ItemBase>(Inventory[Equipment[Options.WeaponIndex]]
                                                .ItemNum) != null &&
                                            Convert.ToBoolean(
                                                ItemBase.Lookup
                                                    .Get<ItemBase>(Inventory[Equipment[Options.WeaponIndex]].ItemNum)
                                                    .Data4))
                                        {
                                            Equipment[Options.WeaponIndex] = -1;
                                        }
                                    }
                                    Equipment[Options.ShieldIndex] = slot;
                                }
                            }
                            else
                            {
                                Equipment[itemBase.Data1] = slot;
                            }
                        }
                        PacketSender.SendPlayerEquipmentToProximity(this);
                        PacketSender.SendEntityStats(this);
                        if (equipped) return;
                        break;
                    case (int)ItemTypes.Spell:
                        if (itemBase.Data1 <= -1) return;
                        if (!TryTeachSpell(new SpellInstance(itemBase.Data1))) return;
                        TakeItemsBySlot(slot, 1);
                        break;
                    case (int)ItemTypes.Event:
                        var evt = EventBase.Lookup.Get<EventBase>(itemBase.Data1);
                        if (evt == null) return;
                        if (!StartCommonEvent(evt)) return;
                        TakeItemsBySlot(slot, 1);
                        break;
                    case (int)ItemTypes.Bag:
                        //Bags will never, ever, be stackable. Going to use the value property for the bag id in the database.
                        if (itemInstance.BagId == -1)
                        {
                            //Create the Bag
                            var slotCount = itemBase.Data1;
                            if (slotCount < 1) slotCount = 1;
                            itemInstance.BagId = Database.CreateBag(slotCount);
                        }
                        //Send the bag to the player (this will make it appear on screen)
                        OpenBag(itemInstance);
                        break;
                    default:
                        PacketSender.SendPlayerMsg(MyClient, Strings.Get("items", "notimplemented"));
                        return;
                }
                if (itemBase.Animation > -1)
                {
                    PacketSender.SendAnimationToProximity(itemBase.Animation, 1, MyIndex, CurrentMap,
                        0,
                        0, Dir); //Target Type 1 will be global entity
                }
            }
        }

        public bool TakeItemsBySlot(int slot, int amount)
        {
            var returnVal = false;
            if (slot < 0)
            {
                return false;
            }
            var itemBase = ItemBase.Lookup.Get<ItemBase>(Inventory[slot].ItemNum);
            if (itemBase != null)
            {
                if (itemBase.IsStackable())
                {
                    if (amount > Inventory[slot].ItemVal)
                    {
                        amount = Inventory[slot].ItemVal;
                    }
                    else
                    {
                        if (amount == Inventory[slot].ItemVal)
                        {
                            Inventory[slot] = new ItemInstance(-1, 0, -1);
                            EquipmentProcessItemLoss(slot);
                            returnVal = true;
                        }
                        else
                        {
                            Inventory[slot].ItemVal -= amount;
                            returnVal = true;
                        }
                    }
                }
                else
                {
                    Inventory[slot] = new ItemInstance(-1, 0, -1);
                    EquipmentProcessItemLoss(slot);
                    returnVal = true;
                }
                PacketSender.SendInventoryItemUpdate(MyClient, slot);
            }
            if (returnVal)
            {
                UpdateGatherItemQuests(itemBase.Index);
            }
            return returnVal;
        }

        public bool TakeItemsByNum(int itemNum, int amount)
        {
            if (CountItemInstances(itemNum) < amount)
                return false;

            if (Inventory == null)
                return false;

            var invbackup = Inventory.Select(item => item?.Clone()).ToList();

            for (var i = 0; i < Options.MaxInvItems; i++)
            {
                var item = Inventory[i];
                if (item?.ItemNum != itemNum)
                    continue;

                if (item.ItemVal <= 1)
                {
                    amount -= 1;
                    Inventory[i] = new ItemInstance();
                    PacketSender.SendInventoryItemUpdate(MyClient, i);
                    if (amount == 0)
                        return true;
                }
                else
                {
                    if (amount >= item.ItemVal)
                    {
                        amount -= item.ItemVal;
                        Inventory[i] = new ItemInstance();
                        PacketSender.SendInventoryItemUpdate(MyClient, i);
                        if (amount == 0)
                            return true;
                    }
                    else
                    {
                        item.ItemVal -= amount;
                        PacketSender.SendInventoryItemUpdate(MyClient, i);
                        return true;
                    }
                }
            }

            Inventory = invbackup;
            PacketSender.SendInventory(MyClient);
            return false;
        }

        public int FindItem(int itemNum, int itemVal = 1)
        {
            if (Inventory == null)
                return -1;

            for (var i = 0; i < Options.MaxInvItems; i++)
            {
                var item = Inventory[i];
                if (item?.ItemNum != itemNum)
                    continue;

                if (item.ItemVal >= itemVal)
                    return i;
            }

            return -1;
        }

        public int CountItemInstances(int itemNum)
        {
            if (Inventory == null)
                return -1;

            var count = 0;
            for (var i = 0; i < Options.MaxInvItems; i++)
            {
                var item = Inventory[i];
                if (item?.ItemNum != itemNum)
                    continue;

                count += Math.Max(1, item.ItemVal);
            }

            return count;
        }

        public override int GetWeaponDamage()
        {
            if (Equipment[Options.WeaponIndex] > -1 && Equipment[Options.WeaponIndex] < Options.MaxInvItems)
            {
                if (Inventory[Equipment[Options.WeaponIndex]].ItemNum > -1)
                {
                    var item = ItemBase.Lookup.Get<ItemBase>(Inventory[Equipment[Options.WeaponIndex]].ItemNum);
                    if (item != null)
                    {
                        return item.Damage;
                    }
                }
            }
            return 0;
        }

        //Shop
        public bool OpenShop(int shopNum)
        {
            if (IsBusy()) return false;
            InShop = shopNum;
            PacketSender.SendOpenShop(MyClient, shopNum);
            return true;
        }

        public void CloseShop()
        {
            if (InShop > -1)
            {
                InShop = -1;
                PacketSender.SendCloseShop(MyClient);
            }
        }

        public void SellItem(int slot, int amount)
        {
            var canSellItem = true;
            var rewardItemNum = -1;
            var rewardItemVal = 0;
            var sellItemNum = Inventory[slot].ItemNum;
            if (InShop == -1) return;
            var shop = ShopBase.Lookup.Get<ShopBase>(InShop);
            if (shop != null)
            {
                var itemBase = ItemBase.Lookup.Get<ItemBase>(Inventory[slot].ItemNum);
                if (itemBase != null)
                {
                    if (itemBase.Bound > 0)
                    {
                        PacketSender.SendPlayerMsg(MyClient, Strings.Get("shops", "bound"), CustomColors.ItemBound);
                        return;
                    }

                    //Check if this is a bag with items.. if so don't allow sale
                    if (itemBase.ItemType == (int)ItemTypes.Bag)
                    {
                        if (Inventory[slot].BagId > -1)
                        {
                            if (!Database.BagEmpty(Inventory[slot].BagId))
                            {
                                PacketSender.SendPlayerMsg(MyClient, Strings.Get("bags", "onlysellempty"),
                                    CustomColors.Error);
                                return;
                            }
                        }
                    }

                    for (var i = 0; i < shop.BuyingItems.Count; i++)
                    {
                        if (shop.BuyingItems[i].ItemNum == sellItemNum)
                        {
                            if (!shop.BuyingWhitelist)
                            {
                                PacketSender.SendPlayerMsg(MyClient, Strings.Get("shops", "doesnotaccept"),
                                    CustomColors.Error);
                                return;
                            }
                            else
                            {
                                rewardItemNum = shop.BuyingItems[i].CostItemNum;
                                rewardItemVal = shop.BuyingItems[i].CostItemVal;
                                break;
                            }
                        }
                    }
                    if (rewardItemNum == -1)
                    {
                        if (shop.BuyingWhitelist)
                        {
                            PacketSender.SendPlayerMsg(MyClient, Strings.Get("shops", "doesnotaccept"),
                                CustomColors.Error);
                            return;
                        }
                        else
                        {
                            rewardItemNum = shop.DefaultCurrency;
                            rewardItemVal = itemBase.Price;
                        }
                    }

                    if (amount >= Inventory[slot].ItemVal)
                    {
                        amount = Inventory[slot].ItemVal;
                    }
                    if (amount == Inventory[slot].ItemVal)
                    {
                        //Definitely can get reward.
                        Inventory[slot] = new ItemInstance(-1, 0, -1);
                        EquipmentProcessItemLoss(slot);
                    }
                    else
                    {
                        //check if can get reward
                        if (!CanGiveItem(new ItemInstance(rewardItemNum, rewardItemVal, -1)))
                        {
                            canSellItem = false;
                        }
                        else
                        {
                            Inventory[slot].ItemVal -= amount;
                        }
                    }
                    if (canSellItem)
                    {
                        TryGiveItem(new ItemInstance(rewardItemNum, rewardItemVal * amount, -1), true);
                    }
                    PacketSender.SendInventoryItemUpdate(MyClient, slot);
                }
            }
        }

        public void BuyItem(int slot, int amount)
        {
            var canSellItem = true;
            var buyItemNum = -1;
            var buyItemAmt = 1;
            if (InShop == -1) return;
            var shop = ShopBase.Lookup.Get<ShopBase>(InShop);
            if (shop != null)
            {
                if (slot >= 0 && slot < shop.SellingItems.Count)
                {
                    var itemBase = ItemBase.Lookup.Get<ItemBase>(shop.SellingItems[slot].ItemNum);
                    if (itemBase != null)
                    {
                        buyItemNum = shop.SellingItems[slot].ItemNum;
                        if (itemBase.IsStackable())
                        {
                            buyItemAmt = Math.Max(1, amount);
                        }
                        if (shop.SellingItems[slot].CostItemVal == 0 ||
                            FindItem(shop.SellingItems[slot].CostItemNum,
                                shop.SellingItems[slot].CostItemVal * buyItemAmt) > -1)
                        {
                            if (CanGiveItem(new ItemInstance(buyItemNum, buyItemAmt, -1)))
                            {
                                if (shop.SellingItems[slot].CostItemVal > 0)
                                {
                                    TakeItemsBySlot(
                                        FindItem(shop.SellingItems[slot].CostItemNum,
                                            shop.SellingItems[slot].CostItemVal * buyItemAmt),
                                        shop.SellingItems[slot].CostItemVal * buyItemAmt);
                                }
                                TryGiveItem(new ItemInstance(buyItemNum, buyItemAmt, -1), true);
                            }
                            else
                            {
                                if (shop.SellingItems[slot].CostItemVal * buyItemAmt ==
                                    Inventory[
                                        FindItem(shop.SellingItems[slot].CostItemNum,
                                            shop.SellingItems[slot].CostItemVal * buyItemAmt)].ItemVal)
                                {
                                    TakeItemsBySlot(
                                        FindItem(shop.SellingItems[slot].CostItemNum,
                                            shop.SellingItems[slot].CostItemVal * buyItemAmt),
                                        shop.SellingItems[slot].CostItemVal * buyItemAmt);
                                    TryGiveItem(new ItemInstance(buyItemNum, buyItemAmt, -1), true);
                                }
                                else
                                {
                                    PacketSender.SendPlayerMsg(MyClient, Strings.Get("shops", "inventoryfull"),
                                        CustomColors.Error, MyName);
                                }
                            }
                        }
                        else
                        {
                            PacketSender.SendPlayerMsg(MyClient, Strings.Get("shops", "cantafford"),
                                CustomColors.Error, MyName);
                        }
                    }
                }
            }
        }

        //Crafting
        public bool OpenCraftingBench(BenchBase bench)
        {
            if (IsBusy()) return false;
            if (bench != null)
            {
                InCraft = bench.Index;
                PacketSender.SendOpenCraftingBench(MyClient, bench);
            }
            return true;
        }

        public void CloseCraftingBench()
        {
            if (InCraft > -1 && CraftIndex == -1)
            {
                InCraft = -1;
                PacketSender.SendCloseCraftingBench(MyClient);
            }
        }

        //Craft a new item
        public void CraftItem(int index)
        {
            if (InCraft > -1)
            {
                var invbackup = new List<ItemInstance>();
                foreach (var item in Inventory)
                {
                    invbackup.Add(item.Clone());
                }

                //Quickly Look through the inventory and create a catalog of what items we have, and how many
                var itemdict = new Dictionary<int, int>();
                foreach (var item in Inventory)
                {
                    if (item != null)
                    {
                        if (itemdict.ContainsKey(item.ItemNum))
                        {
                            itemdict[item.ItemNum] += item.ItemVal;
                        }
                        else
                        {
                            itemdict.Add(item.ItemNum, item.ItemVal);
                        }
                    }
                }

                //Check the player actually has the items
                foreach (var c in BenchBase.Lookup.Get<BenchBase>(InCraft).Crafts[index].Ingredients)
                {
                    if (itemdict.ContainsKey(c.Item))
                    {
                        if (itemdict[c.Item] >= c.Quantity)
                        {
                            itemdict[c.Item] -= c.Quantity;
                        }
                        else
                        {
                            CraftIndex = -1;
                            return;
                        }
                    }
                    else
                    {
                        CraftIndex = -1;
                        return;
                    }
                }

                //Take the items
                foreach (var c in BenchBase.Lookup.Get<BenchBase>(InCraft).Crafts[index].Ingredients)
                {
                    if (!TakeItemsByNum(c.Item, c.Quantity))
                    {
                        Inventory = invbackup;
                        PacketSender.SendInventory(MyClient);
                        CraftIndex = -1;
                        return;
                    }
                }

                //Give them the craft
                if (TryGiveItem(new ItemInstance(BenchBase.Lookup.Get<BenchBase>(InCraft).Crafts[index].Item, 1, -1)))
                {
                    PacketSender.SendPlayerMsg(MyClient,
                        Strings.Get("crafting", "crafted",
                            ItemBase.GetName(BenchBase.Lookup.Get<BenchBase>(InCraft).Crafts[index].Item)),
                        CustomColors.Crafted);
                }
                else
                {
                    Inventory = invbackup;
                    PacketSender.SendInventory(MyClient);
                    PacketSender.SendPlayerMsg(MyClient,
                        Strings.Get("crafting", "nospace",
                            ItemBase.GetName(BenchBase.Lookup.Get<BenchBase>(InCraft).Crafts[index].Item)),
                        CustomColors.Error);
                }
                CraftIndex = -1;
            }
        }

        public bool CheckCrafting(int index)
        {
            //See if we have lost the items needed for our current craft, if so end the crafting session
            //Quickly Look through the inventory and create a catalog of what items we have, and how many
            var itemdict = new Dictionary<int, int>();
            foreach (var item in Inventory)
            {
                if (item != null)
                {
                    if (itemdict.ContainsKey(item.ItemNum))
                    {
                        itemdict[item.ItemNum] += item.ItemVal;
                    }
                    else
                    {
                        itemdict.Add(item.ItemNum, item.ItemVal);
                    }
                }
            }

            //Check the player actually has the items
            foreach (var c in BenchBase.Lookup.Get<BenchBase>(InCraft).Crafts[index].Ingredients)
            {
                if (itemdict.ContainsKey(c.Item))
                {
                    if (itemdict[c.Item] >= c.Quantity)
                    {
                        itemdict[c.Item] -= c.Quantity;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        //Business
        public bool IsBusy()
        {
            return InShop > 1 || InBank || InCraft > -1 || Trading > -1;
        }

        //Bank
        public bool OpenBank()
        {
            if (IsBusy()) return false;
            InBank = true;
            PacketSender.SendOpenBank(MyClient);
            return true;
        }

        public void CloseBank()
        {
            if (InBank)
            {
                InBank = false;
                PacketSender.SendCloseBank(MyClient);
            }
        }

        public void DepositItem(int slot, int amount)
        {
            if (!InBank) return;
            var itemBase = ItemBase.Lookup.Get<ItemBase>(Inventory[slot].ItemNum);
            if (itemBase != null)
            {
                if (Inventory[slot].ItemNum > -1)
                {
                    if (itemBase.IsStackable())
                    {
                        if (amount >= Inventory[slot].ItemVal)
                        {
                            amount = Inventory[slot].ItemVal;
                        }
                    }
                    else
                    {
                        amount = 1;
                    }
                    //Find a spot in the bank for it!
                    if (itemBase.IsStackable())
                    {
                        for (var i = 0; i < Options.MaxBankSlots; i++)
                        {
                            if (Bank[i] != null && Bank[i].ItemNum == Inventory[slot].ItemNum)
                            {
                                amount = Math.Min(amount, int.MaxValue - Bank[i].ItemVal);
                                Bank[i].ItemVal += amount;
                                //Remove Items from inventory send updates
                                if (amount >= Inventory[slot].ItemVal)
                                {
                                    Inventory[slot] = new ItemInstance(-1, 0, -1);
                                    EquipmentProcessItemLoss(slot);
                                }
                                else
                                {
                                    Inventory[slot].ItemVal -= amount;
                                }
                                PacketSender.SendInventoryItemUpdate(MyClient, slot);
                                PacketSender.SendBankUpdate(MyClient, i);
                                return;
                            }
                        }
                    }

                    //Either a non stacking item, or we couldn't find the item already existing in the players inventory
                    for (var i = 0; i < Options.MaxBankSlots; i++)
                    {
                        if (Bank[i] == null || Bank[i].ItemNum == -1)
                        {
                            Bank[i] = new ItemInstance(-1, 0, -1);
                            Bank[i] = Inventory[slot].Clone();
                            Bank[i].ItemVal = amount;
                            //Remove Items from inventory send updates
                            if (amount >= Inventory[slot].ItemVal)
                            {
                                Inventory[slot] = new ItemInstance(-1, 0, -1);
                                EquipmentProcessItemLoss(slot);
                            }
                            else
                            {
                                Inventory[slot].ItemVal -= amount;
                            }
                            PacketSender.SendInventoryItemUpdate(MyClient, slot);
                            PacketSender.SendBankUpdate(MyClient, i);
                            return;
                        }
                    }
                    PacketSender.SendPlayerMsg(MyClient, Strings.Get("banks", "banknospace"), CustomColors.Error);
                }
                else
                {
                    PacketSender.SendPlayerMsg(MyClient, Strings.Get("banks", "depositinvalid"), CustomColors.Error);
                }
            }
        }

        public void WithdrawItem(int slot, int amount)
        {
            if (!InBank) return;

            Debug.Assert(ItemBase.Lookup != null, "ItemBase.Lookup != null");
            Debug.Assert(Bank != null, "Bank != null");
            Debug.Assert(Inventory != null, "Inventory != null");

            var bankSlotItem = Bank[slot];
            if (bankSlotItem == null) return;

            var itemBase = ItemBase.Lookup.Get<ItemBase>(bankSlotItem.ItemNum);
            var inventorySlot = -1;
            if (itemBase == null) return;
            if (bankSlotItem.ItemNum > -1)
            {
                if (itemBase.IsStackable())
                {
                    if (amount >= bankSlotItem.ItemVal)
                    {
                        amount = bankSlotItem.ItemVal;
                    }
                }
                else
                {
                    amount = 1;
                }

                //Find a spot in the inventory for it!
                if (itemBase.IsStackable())
                {
                    /* Find an existing stack */
                    for (var i = 0; i < Options.MaxInvItems; i++)
                    {
                        var inventorySlotItem = Inventory[i];
                        if (inventorySlotItem == null) continue;
                        if (inventorySlotItem.ItemNum != bankSlotItem.ItemNum) continue;
                        inventorySlot = i;
                        break;
                    }
                }

                if (inventorySlot < 0)
                {
                    /* Find a free slot if we don't have one already */
                    for (var j = 0; j < Options.MaxInvItems; j++)
                    {
                        if (Inventory[j] != null && Inventory[j].ItemNum != -1) continue;
                        inventorySlot = j;
                        break;
                    }
                }

                /* If we don't have a slot send an error. */
                if (inventorySlot < 0)
                {
                    PacketSender.SendPlayerMsg(MyClient, Strings.Get("banks", "inventorynospace"),
                        CustomColors.Error);
                    return; //Panda forgot this :P
                }

                /* Move the items to the inventory */
                Debug.Assert(Inventory[inventorySlot] != null, "Inventory[inventorySlot] != null");
                amount = Math.Min(amount, int.MaxValue - Inventory[inventorySlot].ItemVal);

                if (Inventory[inventorySlot] == null || Inventory[inventorySlot].ItemNum == -1 ||
                    Inventory[inventorySlot].ItemVal < 0)
                {
                    Inventory[inventorySlot] = bankSlotItem.Clone();
                    Inventory[inventorySlot].ItemVal = 0;
                }

                Inventory[inventorySlot].ItemVal += amount;
                if (amount >= bankSlotItem.ItemVal)
                {
                    Bank[slot] = null;
                }
                else
                {
                    bankSlotItem.ItemVal -= amount;
                }

                PacketSender.SendInventoryItemUpdate(MyClient, inventorySlot);
                PacketSender.SendBankUpdate(MyClient, slot);
            }
            else
            {
                PacketSender.SendPlayerMsg(MyClient, Strings.Get("banks", "withdrawinvalid"), CustomColors.Error);
            }
        }

        public void SwapBankItems(int item1, int item2)
        {
            ItemInstance tmpInstance = null;
            if (Bank[item2] != null) tmpInstance = Bank[item2].Clone();
            if (Bank[item1] != null)
            {
                Bank[item2] = Bank[item1].Clone();
            }
            else
            {
                Bank[item2] = null;
            }
            if (tmpInstance != null)
            {
                Bank[item1] = tmpInstance.Clone();
            }
            else
            {
                Bank[item1] = null;
            }
            PacketSender.SendBankUpdate(MyClient, item1);
            PacketSender.SendBankUpdate(MyClient, item2);
        }

        //Bag
        public bool OpenBag(ItemInstance bagItem)
        {
            if (IsBusy() || !HasBag(bagItem.BagId)) return false;
            if (bagItem.BagInstance == null) Database.LoadBag(bagItem);
            InBag = bagItem.BagId;
            PacketSender.SendOpenBag(MyClient, bagItem.BagInstance.Slots, bagItem.BagInstance);
            return true;
        }

        public bool HasBag(int bagId)
        {
            for (var i = 0; i < Inventory.Count; i++)
            {
                if (Inventory[i] != null && Inventory[i].BagId == bagId) return true;
            }
            return false;
        }

        public BagInstance GetBag()
        {
            if (InBag > -1)
            {
                for (var i = 0; i < Inventory.Count; i++)
                {
                    if (Inventory[i] != null && Inventory[i].BagId == InBag) return Inventory[i].BagInstance;
                }
            }
            return null;
        }

        public void CloseBag()
        {
            if (InBag > -1)
            {
                InBag = -1;
                PacketSender.SendCloseBag(MyClient);
            }
        }

        public void StoreBagItem(int slot, int amount)
        {
            if (InBag < 0 || !HasBag(InBag)) return;
            var itemBase = ItemBase.Lookup.Get<ItemBase>(Inventory[slot].ItemNum);
            var bag = GetBag();
            if (itemBase != null && bag != null)
            {
                if (Inventory[slot].ItemNum > -1)
                {
                    if (itemBase.IsStackable())
                    {
                        if (amount >= Inventory[slot].ItemVal)
                        {
                            amount = Inventory[slot].ItemVal;
                        }
                    }
                    else
                    {
                        amount = 1;
                    }

                    //Make Sure we are not Storing a Bag inside of itself
                    if (Inventory[slot].BagId == InBag)
                    {
                        PacketSender.SendPlayerMsg(MyClient, Strings.Get("bags", "baginself"), CustomColors.Error);
                        return;
                    }

                    //Find a spot in the bag for it!
                    if (itemBase.IsStackable())
                    {
                        for (var i = 0; i < bag.Slots; i++)
                        {
                            if (bag.Items[i] != null && bag.Items[i].ItemNum == Inventory[slot].ItemNum)
                            {
                                amount = Math.Min(amount, int.MaxValue - bag.Items[i].ItemVal);
                                bag.Items[i].ItemVal += amount;
                                //Remove Items from inventory send updates
                                if (amount >= Inventory[slot].ItemVal)
                                {
                                    Inventory[slot] = new ItemInstance(-1, 0, -1);
                                    EquipmentProcessItemLoss(slot);
                                }
                                else
                                {
                                    Inventory[slot].ItemVal -= amount;
                                }
                                Database.SaveBagItem(InBag, i, bag.Items[i]);
                                PacketSender.SendInventoryItemUpdate(MyClient, slot);
                                PacketSender.SendBagUpdate(MyClient, i, bag.Items[i]);
                                return;
                            }
                        }
                    }

                    //Either a non stacking item, or we couldn't find the item already existing in the players inventory
                    for (var i = 0; i < bag.Slots; i++)
                    {
                        if (bag.Items[i] == null || bag.Items[i].ItemNum == -1)
                        {
                            bag.Items[i] = new ItemInstance(-1, 0, -1);
                            bag.Items[i] = Inventory[slot].Clone();
                            bag.Items[i].ItemVal = amount;
                            //Remove Items from inventory send updates
                            if (amount >= Inventory[slot].ItemVal)
                            {
                                Inventory[slot] = new ItemInstance(-1, 0, -1);
                                EquipmentProcessItemLoss(slot);
                            }
                            else
                            {
                                Inventory[slot].ItemVal -= amount;
                            }
                            Database.SaveBagItem(InBag, i, bag.Items[i]);
                            PacketSender.SendInventoryItemUpdate(MyClient, slot);
                            PacketSender.SendBagUpdate(MyClient, i, bag.Items[i]);
                            return;
                        }
                    }
                    PacketSender.SendPlayerMsg(MyClient, Strings.Get("bags", "bagnospace"), CustomColors.Error);
                }
                else
                {
                    PacketSender.SendPlayerMsg(MyClient, Strings.Get("bags", "depositinvalid"), CustomColors.Error);
                }
            }
        }

        public void RetreiveBagItem(int slot, int amount)
        {
            if (InBag < 0 || !HasBag(InBag)) return;
            var bag = GetBag();
            if (bag == null || slot > bag.Items.Length || bag.Items[slot] == null) return;
            var itemBase = ItemBase.Lookup.Get<ItemBase>(bag.Items[slot].ItemNum);
            var inventorySlot = -1;
            if (itemBase != null)
            {
                if (bag.Items[slot] != null && bag.Items[slot].ItemNum > -1)
                {
                    if (itemBase.IsStackable())
                    {
                        if (amount >= bag.Items[slot].ItemVal)
                        {
                            amount = bag.Items[slot].ItemVal;
                        }
                    }
                    else
                    {
                        amount = 1;
                    }
                    //Find a spot in the inventory for it!
                    if (itemBase.IsStackable())
                    {
                        /* Find an existing stack */
                        for (var i = 0; i < Options.MaxInvItems; i++)
                        {
                            if (Inventory[i] != null && Inventory[i].ItemNum == bag.Items[slot].ItemNum)
                            {
                                inventorySlot = i;
                                break;
                            }
                        }
                    }

                    if (inventorySlot < 0)
                    {
                        /* Find a free slot if we don't have one already */
                        for (var j = 0; j < Options.MaxInvItems; j++)
                        {
                            if (Inventory[j] == null || Inventory[j].ItemNum == -1)
                            {
                                inventorySlot = j;
                                break;
                            }
                        }
                    }

                    /* If we don't have a slot send an error. */
                    if (inventorySlot < 0)
                    {
                        PacketSender.SendPlayerMsg(MyClient, Strings.Get("bags", "inventorynospace"),
                            CustomColors.Error);
                        return; //Panda forgot this :P
                    }

                    /* Move the items to the inventory */
                    amount = Math.Min(amount, int.MaxValue - Inventory[inventorySlot].ItemVal);

                    if (Inventory[inventorySlot] == null || Inventory[inventorySlot].ItemNum == -1 ||
                        Inventory[inventorySlot].ItemVal < 0)
                    {
                        Inventory[inventorySlot] = new ItemInstance(bag.Items[slot].ItemNum, 0, -1);
                    }

                    Inventory[inventorySlot].ItemVal += amount;
                    if (amount >= bag.Items[slot].ItemVal)
                    {
                        bag.Items[slot] = null;
                    }
                    else
                    {
                        bag.Items[slot].ItemVal -= amount;
                    }
                    Database.SaveBagItem(InBag, slot, bag.Items[slot]);

                    PacketSender.SendInventoryItemUpdate(MyClient, inventorySlot);
                    PacketSender.SendBagUpdate(MyClient, slot, bag.Items[slot]);
                }
                else
                {
                    PacketSender.SendPlayerMsg(MyClient, Strings.Get("bags", "withdrawinvalid"), CustomColors.Error);
                }
            }
        }

        public void SwapBagItems(int item1, int item2)
        {
            if (InBag < 0 || !HasBag(InBag)) return;
            var bag = GetBag();
            ItemInstance tmpInstance = null;
            if (bag.Items[item2] != null) tmpInstance = bag.Items[item2].Clone();
            if (bag.Items[item1] != null)
            {
                bag.Items[item2] = bag.Items[item1].Clone();
            }
            else
            {
                bag.Items[item2] = null;
            }
            if (tmpInstance != null)
            {
                bag.Items[item1] = tmpInstance.Clone();
            }
            else
            {
                bag.Items[item1] = null;
            }
            Database.SaveBagItem(InBag, item1, bag.Items[item1]);
            Database.SaveBagItem(InBag, item2, bag.Items[item2]);
            PacketSender.SendBagUpdate(MyClient, item1, bag.Items[item1]);
            PacketSender.SendBagUpdate(MyClient, item2, bag.Items[item2]);
        }

        //Friends
        public void FriendRequest(Player fromPlayer)
        {
            if (fromPlayer.FriendRequests.ContainsKey(this))
            {
                fromPlayer.FriendRequests.Remove(this);
            }
            if (!FriendRequests.ContainsKey(fromPlayer) || !(FriendRequests[fromPlayer] > Globals.System.GetTimeMs()))
            {
                if (TradeRequester == null && PartyRequester == null && FriendRequester == null)
                {
                    FriendRequester = fromPlayer;
                    PacketSender.SendFriendRequest(MyClient, fromPlayer);
                    PacketSender.SendPlayerMsg(fromPlayer.MyClient, Strings.Get("friends", "sent"),
                        CustomColors.RequestSent);
                }
                else
                {
                    PacketSender.SendPlayerMsg(fromPlayer.MyClient, Strings.Get("friends", "busy", MyName),
                        CustomColors.Error);
                }
            }
        }

        //Trading
        public void InviteToTrade(Player fromPlayer)
        {
            if (fromPlayer.TradeRequests.ContainsKey(this))
            {
                fromPlayer.TradeRequests.Remove(this);
            }
            if (TradeRequests.ContainsKey(fromPlayer) && TradeRequests[fromPlayer] > Globals.System.GetTimeMs())
            {
                PacketSender.SendPlayerMsg(fromPlayer.MyClient, Strings.Get("trading", "alreadydenied"),
                    CustomColors.Error);
            }
            else
            {
                if (TradeRequester == null && PartyRequester == null && FriendRequester == null)
                {
                    TradeRequester = fromPlayer;
                    PacketSender.SendTradeRequest(MyClient, fromPlayer);
                }
                else
                {
                    PacketSender.SendPlayerMsg(fromPlayer.MyClient, Strings.Get("trading", "busy", MyName),
                        CustomColors.Error);
                }
            }
        }

        public void OfferItem(int slot, int amount)
        {
            if (Trading < 0) return;
            var itemBase = ItemBase.Lookup.Get<ItemBase>(Inventory[slot].ItemNum);
            if (itemBase != null)
            {
                if (Inventory[slot].ItemNum > -1)
                {
                    if (itemBase.Bound > 0)
                    {
                        PacketSender.SendPlayerMsg(MyClient, Strings.Get("trading", "bound"), CustomColors.ItemBound);
                        return;
                    }

                    if (itemBase.IsStackable())
                    {
                        if (amount >= Inventory[slot].ItemVal)
                        {
                            amount = Inventory[slot].ItemVal;
                        }
                    }
                    else
                    {
                        amount = 1;
                    }

                    //Check if this is a bag with items.. if so don't allow sale
                    if (itemBase.ItemType == (int)ItemTypes.Bag)
                    {
                        if (Inventory[slot].BagId > -1)
                        {
                            if (!Database.BagEmpty(Inventory[slot].BagId))
                            {
                                PacketSender.SendPlayerMsg(MyClient, Strings.Get("bags", "onlytradeempty"),
                                    CustomColors.Error);
                                return;
                            }
                        }
                    }

                    //Find a spot in the trade for it!
                    if (itemBase.IsStackable())
                    {
                        for (var i = 0; i < Options.MaxInvItems; i++)
                        {
                            if (Trade[i] != null && Trade[i].ItemNum == Inventory[slot].ItemNum)
                            {
                                amount = Math.Min(amount, int.MaxValue - Trade[i].ItemVal);
                                Trade[i].ItemVal += amount;
                                //Remove Items from inventory send updates
                                if (amount >= Inventory[slot].ItemVal)
                                {
                                    Inventory[slot] = new ItemInstance(-1, 0, -1);
                                    EquipmentProcessItemLoss(slot);
                                }
                                else
                                {
                                    Inventory[slot].ItemVal -= amount;
                                }
                                PacketSender.SendInventoryItemUpdate(MyClient, slot);
                                PacketSender.SendTradeUpdate(MyClient, MyIndex, i);
                                PacketSender.SendTradeUpdate(((Player)Globals.Entities[Trading]).MyClient, MyIndex, i);
                                return;
                            }
                        }
                    }

                    //Either a non stacking item, or we couldn't find the item already existing in the players inventory
                    for (var i = 0; i < Options.MaxInvItems; i++)
                    {
                        if (Trade[i] == null || Trade[i].ItemNum == -1)
                        {
                            Trade[i] = new ItemInstance(-1, 0, -1);
                            Trade[i] = Inventory[slot].Clone();
                            Trade[i].ItemVal = amount;
                            //Remove Items from inventory send updates
                            if (amount >= Inventory[slot].ItemVal)
                            {
                                Inventory[slot] = new ItemInstance(-1, 0, -1);
                                EquipmentProcessItemLoss(slot);
                            }
                            else
                            {
                                Inventory[slot].ItemVal -= amount;
                            }
                            PacketSender.SendInventoryItemUpdate(MyClient, slot);
                            PacketSender.SendTradeUpdate(MyClient, MyIndex, i);
                            PacketSender.SendTradeUpdate(((Player)Globals.Entities[Trading]).MyClient, MyIndex, i);
                            return;
                        }
                    }
                    PacketSender.SendPlayerMsg(MyClient, Strings.Get("trading", "tradenosapce"), CustomColors.Error);
                }
                else
                {
                    PacketSender.SendPlayerMsg(MyClient, Strings.Get("trading", "offerinvalid"), CustomColors.Error);
                }
            }
        }

        public void RevokeItem(int slot, int amount)
        {
            if (Trading < 0) return;

            var itemBase = ItemBase.Lookup.Get<ItemBase>(Trade[slot].ItemNum);
            if (itemBase == null)
            {
                return;
            }

            if (Trade[slot] == null || Trade[slot].ItemNum < 0)
            {
                PacketSender.SendPlayerMsg(MyClient, Strings.Get("trading", "revokeinvalid"), CustomColors.Error);
                return;
            }

            var inventorySlot = -1;
            var stackable = itemBase.IsStackable();
            if (stackable)
            {
                /* Find an existing stack */
                for (var i = 0; i < Options.MaxInvItems; i++)
                {
                    if (Inventory[i] != null && Inventory[i].ItemNum == Trade[slot].ItemNum)
                    {
                        inventorySlot = i;
                        break;
                    }
                }
            }

            if (inventorySlot < 0)
            {
                /* Find a free slot if we don't have one already */
                for (var j = 0; j < Options.MaxInvItems; j++)
                {
                    if (Inventory[j] == null || Inventory[j].ItemNum == -1)
                    {
                        inventorySlot = j;
                        break;
                    }
                }
            }

            /* If we don't have a slot send an error. */
            if (inventorySlot < 0)
            {
                PacketSender.SendPlayerMsg(MyClient, Strings.Get("trading", "inventorynosapce"), CustomColors.Error);
            }

            /* Move the items to the inventory */
            amount = Math.Min(amount, int.MaxValue - Inventory[inventorySlot].ItemVal);

            if (Inventory[inventorySlot] == null || Inventory[inventorySlot].ItemNum == -1 ||
                Inventory[inventorySlot].ItemVal < 0)
            {
                Inventory[inventorySlot] = new ItemInstance(Trade[slot].ItemNum, 0, Trade[slot].BagId);
            }

            Inventory[inventorySlot].ItemVal += amount;
            if (amount >= Trade[slot].ItemVal)
            {
                Trade[slot] = null;
            }
            else
            {
                Trade[slot].ItemVal -= amount;
            }

            PacketSender.SendInventoryItemUpdate(MyClient, inventorySlot);
            PacketSender.SendTradeUpdate(MyClient, MyIndex, slot);
            PacketSender.SendTradeUpdate(((Player)Globals.Entities[Trading]).MyClient, MyIndex, slot);
        }

        public void ReturnTradeItems()
        {
            if (Trading < 0) return;

            for (var i = 0; i < Options.MaxInvItems; i++)
            {
                if (Trade[i] != null && Trade[i].ItemNum > 0)
                {
                    if (!TryGiveItem(Trade[i]))
                    {
                        MapInstance.Lookup.Get<MapInstance>(CurrentMap)
                            .SpawnItem(CurrentX, CurrentY, Trade[i], Trade[i].ItemVal);
                        PacketSender.SendPlayerMsg(MyClient, Strings.Get("trading", "itemsdropped"),
                            CustomColors.Error);
                    }
                    Trade[i].ItemNum = 0;
                    Trade[i].ItemVal = 0;
                }
            }
            PacketSender.SendInventory(MyClient);
        }

        public void CancelTrade()
        {
            if (Trading < 0) return;
            ReturnTradeItems();
            ((Player)Globals.Entities[Trading]).ReturnTradeItems();
            PacketSender.SendPlayerMsg(MyClient, Strings.Get("trading", "declined"), CustomColors.Error);
            PacketSender.SendPlayerMsg(((Player)Globals.Entities[Trading]).MyClient,
                Strings.Get("trading", "declined"),
                CustomColors.Error);
            PacketSender.SendTradeClose(((Player)Globals.Entities[Trading]).MyClient);
            PacketSender.SendTradeClose(MyClient);
            ((Player)Globals.Entities[Trading]).Trading = -1;
            Trading = -1;
        }

        //Parties
        public void InviteToParty(Player fromPlayer)
        {
            if (fromPlayer.PartyRequests.ContainsKey(this))
            {
                fromPlayer.PartyRequests.Remove(this);
            }
            if (PartyRequests.ContainsKey(fromPlayer) && PartyRequests[fromPlayer] > Globals.System.GetTimeMs())
            {
                PacketSender.SendPlayerMsg(fromPlayer.MyClient, Strings.Get("parties", "alreadydenied"),
                    CustomColors.Error);
            }
            else
            {
                if (TradeRequester == null && PartyRequester == null && FriendRequester == null)
                {
                    PartyRequester = fromPlayer;
                    PacketSender.SendPartyInvite(MyClient, fromPlayer);
                }
                else
                {
                    PacketSender.SendPlayerMsg(fromPlayer.MyClient, Strings.Get("parties", "busy", MyName),
                        CustomColors.Error);
                }
            }
        }

        public void AddParty(Player target)
        {
            //If a new party, make yourself the leader
            if (Party.Count == 0)
            {
                Party.Add(this);
            }
            else
            {
                if (Party[0] != this)
                {
                    PacketSender.SendPlayerMsg(MyClient, Strings.Get("parties", "leaderinvonly"), CustomColors.Error);
                    return;
                }

                //Check for member being already in the party, if so cancel
                for (var i = 0; i < Party.Count; i++)
                {
                    if (Party[i] == target)
                    {
                        return;
                    }
                }
            }

            if (Party.Count < 4)
            {
                Party.Add(target);

                //Update all members of the party with the new list
                for (var i = 0; i < Party.Count; i++)
                {
                    Party[i].Party = Party;
                    PacketSender.SendParty(Party[i].MyClient);
                    PacketSender.SendPlayerMsg(Party[i].MyClient, Strings.Get("parties", "joined", target.MyName),
                        CustomColors.Accepted);
                }
            }
            else
            {
                PacketSender.SendPlayerMsg(MyClient, Strings.Get("parties", "limitreached"), CustomColors.Error);
            }
        }

        public void KickParty(int target)
        {
            if (Party.Count > 0 && Party[0] == this)
            {
                if (target > 0 && target < Party.Count)
                {
                    var oldMember = Party[target];
                    oldMember.Party = new List<Player>();
                    PacketSender.SendParty(oldMember.MyClient);
                    PacketSender.SendPlayerMsg(oldMember.MyClient, Strings.Get("parties", "kicked"),
                        CustomColors.Error);
                    Party.RemoveAt(target);

                    if (Party.Count > 1) //Need atleast 2 party members to function
                    {
                        //Update all members of the party with the new list
                        for (var i = 0; i < Party.Count; i++)
                        {
                            Party[i].Party = Party;
                            PacketSender.SendParty(Party[i].MyClient);
                            PacketSender.SendPlayerMsg(Party[i].MyClient,
                                Strings.Get("parties", "memberkicked", oldMember.MyName), CustomColors.Error);
                        }
                    }
                    else if (Party.Count > 0) //Check if anyone is left on their own
                    {
                        var remainder = Party[0];
                        remainder.Party.Clear();
                        PacketSender.SendParty(remainder.MyClient);
                        PacketSender.SendPlayerMsg(remainder.MyClient, Strings.Get("parties", "disbanded"),
                            CustomColors.Error);
                    }
                }
            }
        }

        public void LeaveParty()
        { 
            if (Party.Count > 0 && Party.Contains(this))
            {
                var oldMember = this;
                Party.Remove(this);

                if (Party.Count > 1) //Need atleast 2 party members to function
                {
                    //Update all members of the party with the new list
                    for (var i = 0; i < Party.Count; i++)
                    {
                        Party[i].Party = Party;
                        PacketSender.SendParty(Party[i].MyClient);
                        PacketSender.SendPlayerMsg(Party[i].MyClient,
                            Strings.Get("parties", "memberleft", oldMember.MyName), CustomColors.Error);
                    }
                }
                else if (Party.Count > 0) //Check if anyone is left on their own
                {
                    var remainder = Party[0];
                    remainder.Party.Clear();
                    PacketSender.SendParty(remainder.MyClient);
                    PacketSender.SendPlayerMsg(remainder.MyClient, Strings.Get("parties", "disbanded"),
                        CustomColors.Error);
                }
            }
            Party.Clear();
            PacketSender.SendParty(MyClient);
            PacketSender.SendPlayerMsg(MyClient, Strings.Get("parties", "left"), CustomColors.Error);
        }

        public bool InParty(Player member)
        {
            for (var i = 0; i < Party.Count; i++)
            {
                if (member == Party[i])
                {
                    return true;
                }
            }
            return false;
        }

        public void StartTrade(Player target)
        {
            if (target.Trading == -1)
            {
                // Set the status of both players to be in a trade
                Trading = target.MyIndex;
                target.Trading = MyIndex;
                TradeAccepted = false;
                target.TradeAccepted = false;
                Trade = new ItemInstance[Options.MaxInvItems];
                target.Trade = new ItemInstance[Options.MaxInvItems];

                for (var i = 0; i < Options.MaxInvItems; i++)
                {
                    Trade[i] = new ItemInstance();
                    target.Trade[i] = new ItemInstance();
                }

                //Send the trade confirmation to both players
                PacketSender.StartTrade(target.MyClient, MyIndex);
                PacketSender.StartTrade(MyClient, target.MyIndex);
            }
        }

        public byte[] PartyData()
        {
            var bf = new ByteBuffer();
            bf.WriteInteger(MyIndex);
            bf.WriteString(MyName);
            for (int i = 0; i < (int)Vitals.VitalCount; i++)
            {
                bf.WriteInteger(Vital[i]);
            }
            for (int i = 0; i < (int)Vitals.VitalCount; i++)
            {
                bf.WriteInteger(MaxVital[i]);
            }
            return bf.ToArray();
        }

        //Spells
        public bool TryTeachSpell(SpellInstance spell, bool sendUpdate = true)
        {
            if (KnowsSpell(spell.SpellNum))
            {
                return false;
            }
            if (SpellBase.Lookup.Get<SpellBase>(spell.SpellNum) == null) return false;
            for (var i = 0; i < Options.MaxPlayerSkills; i++)
            {
                if (Spells[i].SpellNum <= 0)
                {
                    Spells[i] = spell.Clone();
                    if (sendUpdate)
                    {
                        PacketSender.SendPlayerSpellUpdate(MyClient, i);
                    }
                    return true;
                }
            }
            return false;
        }

        public bool KnowsSpell(int spellnum)
        {
            for (var i = 0; i < Options.MaxPlayerSkills; i++)
            {
                if (Spells[i].SpellNum == spellnum)
                {
                    return true;
                }
            }
            return false;
        }

        public int FindSpell(int spellNum)
        {
            for (var i = 0; i < Options.MaxPlayerSkills; i++)
            {
                if (Spells[i].SpellNum == spellNum)
                {
                    return i;
                }
            }
            return -1;
        }

        public void SwapSpells(int spell1, int spell2)
        {
            var tmpInstance = Spells[spell2].Clone();
            Spells[spell2] = Spells[spell1].Clone();
            Spells[spell1] = tmpInstance.Clone();
            PacketSender.SendPlayerSpellUpdate(MyClient, spell1);
            PacketSender.SendPlayerSpellUpdate(MyClient, spell2);
            HotbarProcessSpellSwap(spell1, spell2);
        }

        public void ForgetSpell(int spellSlot)
        {
            Spells[spellSlot] = new SpellInstance();
            PacketSender.SendPlayerSpellUpdate(MyClient, spellSlot);
        }

        public void UseSpell(int spellSlot, Entity target)
        {
            var spellNum = Spells[spellSlot].SpellNum;
            Target = target;
            if (SpellBase.Lookup.Get<SpellBase>(spellNum) != null)
            {
                var spell = SpellBase.Lookup.Get<SpellBase>(spellNum);

                if (!EventInstance.MeetsConditionLists(spell.CastingReqs, this, null))
                {
                    PacketSender.SendPlayerMsg(MyClient, Strings.Get("combat", "dynamicreq"));
                    return;
                }

                //Check if the caster is silenced or stunned
                var statuses = Statuses.Values.ToArray();
                foreach (var status in statuses)
                {
                    if (status.Type == (int)StatusTypes.Silence)
                    {
                        PacketSender.SendPlayerMsg(MyClient, Strings.Get("combat", "silenced"));
                        return;
                    }
                    if (status.Type == (int)StatusTypes.Stun)
                    {
                        PacketSender.SendPlayerMsg(MyClient, Strings.Get("combat", "stunned"));
                        return;
                    }
                }

                //Check if the caster has the right ammunition if a projectile
                if (spell.SpellType == (int)SpellTypes.CombatSpell &&
                    spell.TargetType == (int)SpellTargetTypes.Projectile && spell.Projectile > -1)
                {
                    var projectileBase = ProjectileBase.Lookup.Get<ProjectileBase>(spell.Projectile);
                    if (projectileBase == null) return;
                    if (projectileBase.Ammo > -1)
                    {
                        if (FindItem(projectileBase.Ammo, projectileBase.AmmoRequired) == -1)
                        {
                            PacketSender.SendPlayerMsg(MyClient,
                                Strings.Get("items", "notenough", ItemBase.GetName(projectileBase.Ammo)),
                                CustomColors.Error);
                            return;
                        }
                    }
                }

                if (target == null &&
                    ((spell.SpellType == (int)SpellTypes.CombatSpell &&
                      spell.TargetType == (int)SpellTargetTypes.Single) || spell.SpellType == (int)SpellTypes.WarpTo))
                {
                    PacketSender.SendActionMsg(this, Strings.Get("combat", "notarget"), CustomColors.NoTarget);
                    return;
                }

                //Check for range of a single target spell
                if (spell.SpellType == (int)SpellTypes.CombatSpell &&
                    spell.TargetType == (int)SpellTargetTypes.Single && Target != this)
                {
                    if (!InRangeOf(Target, spell.CastRange))
                    {
                        PacketSender.SendActionMsg(this, Strings.Get("combat", "targetoutsiderange"),
                            CustomColors.NoTarget);
                        return;
                    }
                }

                if (spell.VitalCost[(int)Vitals.Mana] <= Vital[(int)Vitals.Mana])
                {
                    if (spell.VitalCost[(int)Vitals.Health] <= Vital[(int)Vitals.Health])
                    {
                        if (Spells[spellSlot].SpellCd < Globals.System.GetTimeMs())
                        {
                            if (CastTime < Globals.System.GetTimeMs())
                            {
                                Vital[(int)Vitals.Mana] =
                                    Vital[(int)Vitals.Mana] - spell.VitalCost[(int)Vitals.Mana];
                                Vital[(int)Vitals.Health] = Vital[(int)Vitals.Health] -
                                                             spell.VitalCost[(int)Vitals.Health];
                                CastTime = Globals.System.GetTimeMs() + (spell.CastDuration * 100);
                                SpellCastSlot = spellSlot;
                                CastTarget = Target;

                                //Check if the caster has the right ammunition if a projectile
                                if (spell.SpellType == (int)SpellTypes.CombatSpell &&
                                    spell.TargetType == (int)SpellTargetTypes.Projectile && spell.Projectile > -1)
                                {
                                    var projectileBase = ProjectileBase.Lookup.Get<ProjectileBase>(spell.Projectile);
                                    if (projectileBase != null && projectileBase.Ammo > -1)
                                    {
                                        TakeItemsByNum(FindItem(projectileBase.Ammo, projectileBase.AmmoRequired),
                                            projectileBase.AmmoRequired);
                                    }
                                }

                                if (spell.CastAnimation > -1)
                                {
                                    PacketSender.SendAnimationToProximity(spell.CastAnimation, 1, MyIndex, CurrentMap,
                                        0,
                                        0, Dir); //Target Type 1 will be global entity
                                }

                                PacketSender.SendEntityVitals(this);
                                PacketSender.SendEntityVitals(this);
                                PacketSender.SendEntityCastTime(this, spellNum);
                            }
                            else
                            {
                                PacketSender.SendPlayerMsg(MyClient, Strings.Get("combat", "channeling"));
                            }
                        }
                        else
                        {
                            PacketSender.SendPlayerMsg(MyClient, Strings.Get("combat", "cooldown"));
                        }
                    }
                    else
                    {
                        PacketSender.SendPlayerMsg(MyClient, Strings.Get("combat", "lowhealth"));
                    }
                }
                else
                {
                    PacketSender.SendPlayerMsg(MyClient, Strings.Get("combat", "lowmana"));
                }
            }
        }

        public override void CastSpell(int spellNum, int spellSlot = -1)
        {
            var spellBase = SpellBase.Lookup.Get<SpellBase>(spellNum);
            if (spellBase != null)
            {
                if (spellBase.SpellType == (int)SpellTypes.Event)
                {
                    var evt = EventBase.Lookup.Get<EventBase>(spellBase.Data1);
                    if (evt != null)
                    {
                        StartCommonEvent(evt);
                    }
                    base.CastSpell(spellNum, spellSlot); //To get cooldown :P
                }
                else
                {
                    base.CastSpell(spellNum, spellSlot);
                }
            }
        }

        //Equipment
        public void UnequipItem(int slot)
        {
            Equipment[slot] = -1;
            PacketSender.SendPlayerEquipmentToProximity(this);
            PacketSender.SendEntityStats(this);
        }

        public void EquipmentProcessItemSwap(int item1, int item2)
        {
            for (var i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                if (Equipment[i] == item1)
                    Equipment[i] = item2;
                else if (Equipment[i] == item2)
                    Equipment[i] = item1;
            }
            PacketSender.SendPlayerEquipmentToProximity(this);
        }

        public void EquipmentProcessItemLoss(int slot)
        {
            for (var i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                if (Equipment[i] == slot)
                    Equipment[i] = -1;
            }
            PacketSender.SendPlayerEquipmentToProximity(this);
            PacketSender.SendEntityStats(this);
        }

        //Stats
        public void UpgradeStat(int statIndex)
        {
            if (Stat[statIndex].Stat < Options.MaxStatValue && StatPoints > 0)
            {
                Stat[statIndex].Stat++;
                StatPoints--;
                PacketSender.SendEntityStats(this);
                PacketSender.SendPointsTo(MyClient);
            }
        }

        public void AddStat(Stats stat, int amount)
        {
            Stat[(int)stat].Stat += amount;
            if (Stat[(int)stat].Stat < 0) Stat[(int)stat].Stat = 0;
            if (Stat[(int)stat].Stat > Options.MaxStatValue) Stat[(int)stat].Stat = Options.MaxStatValue;
        }

        //Hotbar
        public void HotbarChange(int index, int type, int slot)
        {
            Hotbar[index].Type = type;
            Hotbar[index].Slot = slot;
        }

        public void HotbarProcessItemSwap(int item1, int item2)
        {
            for (var i = 0; i < Options.MaxHotbar; i++)
            {
                if (Hotbar[i].Type == 0 && Hotbar[i].Slot == item1)
                    Hotbar[i].Slot = item2;
                else if (Hotbar[i].Type == 0 && Hotbar[i].Slot == item2)
                    Hotbar[i].Slot = item1;
            }
            PacketSender.SendHotbarSlots(MyClient);
        }

        public void HotbarProcessSpellSwap(int spell1, int spell2)
        {
            for (var i = 0; i < Options.MaxHotbar; i++)
            {
                if (Hotbar[i].Type == 1 && Hotbar[i].Slot == spell1)
                    Hotbar[i].Slot = spell2;
                else if (Hotbar[i].Type == 1 && Hotbar[i].Slot == spell2)
                    Hotbar[i].Slot = spell1;
            }
            PacketSender.SendHotbarSlots(MyClient);
        }

        //Quests
        public bool CanStartQuest(QuestBase quest)
        {
            //Check and see if the quest is already in progress, or if it has already been completed and cannot be repeated.
            if (Quests.ContainsKey(quest.Index))
            {
                if (Quests[quest.Index].Task != -1 && quest.GetTaskIndex(Quests[quest.Index].Task) != -1)
                {
                    return false;
                }
                if (Quests[quest.Index].Completed == 1 && quest.Repeatable == 0)
                {
                    return false;
                }
            }
            //So the quest isn't started or we can repeat it.. let's make sure that we meet requirements.
            if (!EventInstance.MeetsConditionLists(quest.Requirements, this, null, true, quest)) return false;
            if (quest.Tasks.Count == 0)
            {
                return false;
            }
            return true;
        }

        public bool QuestCompleted(QuestBase quest)
        {
            if (Quests.ContainsKey(quest.Index))
            {
                if (Quests[quest.Index].Completed == 1)
                {
                    return true;
                }
            }
            return false;
        }

        public bool QuestInProgress(QuestBase quest, QuestProgress progress, int taskId)
        {
            if (Quests.ContainsKey(quest.Index))
            {
                if (Quests[quest.Index].Task != -1 && quest.GetTaskIndex(Quests[quest.Index].Task) != -1)
                {
                    switch (progress)
                    {
                        case QuestProgress.OnAnyTask:
                            return true;
                        case QuestProgress.BeforeTask:
                            if (quest.GetTaskIndex(taskId) != -1)
                            {
                                return quest.GetTaskIndex(taskId) > quest.GetTaskIndex(Quests[quest.Index].Task);
                            }
                            break;
                        case QuestProgress.OnTask:
                            if (quest.GetTaskIndex(taskId) != -1)
                            {
                                return quest.GetTaskIndex(taskId) == quest.GetTaskIndex(Quests[quest.Index].Task);
                            }
                            break;
                        case QuestProgress.AfterTask:
                            if (quest.GetTaskIndex(taskId) != -1)
                            {
                                return quest.GetTaskIndex(taskId) < quest.GetTaskIndex(Quests[quest.Index].Task);
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(progress), progress, null);
                    }
                }
            }
            return false;
        }

        public void OfferQuest(QuestBase quest)
        {
            if (CanStartQuest(quest))
            {
                QuestOffers.Add(quest.Index);
                PacketSender.SendQuestOffer(this, quest.Index);
            }
        }

        public void StartQuest(QuestBase quest)
        {
            if (CanStartQuest(quest))
            {
                if (Quests.ContainsKey(quest.Index))
                {
                    var questProgress = Quests[quest.Index];
                    questProgress.Task = quest.Tasks[0].Id;
                    questProgress.TaskProgress = 0;
                    Quests[quest.Index] = questProgress;
                }
                else
                {
                    var questProgress = new QuestProgressStruct()
                    {
                        Task = quest.Tasks[0].Id,
                        TaskProgress = 0
                    };
                    Quests.Add(quest.Index, questProgress);
                }
                if (quest.Tasks[0].Objective == 1) //Gather Items
                {
                    UpdateGatherItemQuests(quest.Tasks[0].Data1);
                }
                if (quest.StartEvent != null)
                {
                    StartCommonEvent(quest.StartEvent);
                }
                PacketSender.SendPlayerMsg(MyClient, Strings.Get("quests", "started", quest.Name),
                    CustomColors.QuestStarted);
                PacketSender.SendQuestProgress(this, quest.Index);
            }
        }

        public void AcceptQuest(int questId)
        {
            if (QuestOffers.Contains(questId))
            {
                lock (mEventLock)
                {
                    QuestOffers.Remove(questId);
                    var quest = QuestBase.Lookup.Get<QuestBase>(questId);
                    if (quest != null)
                    {
                        StartQuest(quest);
                        foreach (var evt in EventLookup.Values)
                        {
                            if (evt.CallStack.Count <= 0) continue;
                            if (evt.CallStack.Peek().WaitingForResponse !=
                                CommandInstance.EventResponse.Quest) continue;
                            if (evt.CallStack.Peek().ResponseIndex == questId)
                            {
                                //Run success branch
                                var tmpStack = new CommandInstance(evt.CallStack.Peek().Page)
                                {
                                    CommandIndex = 0,
                                    ListIndex =
                                        evt.CallStack.Peek().Page.CommandLists[
                                            evt.CallStack.Peek().ListIndex].Commands[
                                            evt.CallStack.Peek().CommandIndex].Ints[4]
                                };
                                evt.CallStack.Peek().CommandIndex++;
                                evt.CallStack.Peek().WaitingForResponse =
                                    CommandInstance.EventResponse.None;
                                evt.CallStack.Push(tmpStack);
                            }
                        }
                    }
                }
            }
        }

        public void DeclineQuest(int questId)
        {
            if (QuestOffers.Contains(questId))
            {
                lock (mEventLock)
                {
                    QuestOffers.Remove(questId);
                    PacketSender.SendPlayerMsg(MyClient, Strings.Get("quests", "declined", QuestBase.GetName(questId)),
                        CustomColors.QuestDeclined);
                    foreach (var evt in EventLookup.Values)
                    {
                        if (evt.CallStack.Count <= 0) continue;
                        if (evt.CallStack.Peek().WaitingForResponse != CommandInstance.EventResponse.Quest)
                            continue;
                        if (evt.CallStack.Peek().ResponseIndex == questId)
                        {
                            //Run failure branch
                            var tmpStack = new CommandInstance(evt.CallStack.Peek().Page)
                            {
                                CommandIndex = 0,
                                ListIndex =
                                    evt.CallStack.Peek().Page.CommandLists[
                                        evt.CallStack.Peek().ListIndex].Commands[
                                        evt.CallStack.Peek().CommandIndex].Ints[5]
                            };
                            evt.CallStack.Peek().CommandIndex++;
                            evt.CallStack.Peek().WaitingForResponse =
                                CommandInstance.EventResponse.None;
                            evt.CallStack.Push(tmpStack);
                        }
                    }
                }
            }
        }

        public void CancelQuest(int questId)
        {
            var quest = QuestBase.Lookup.Get<QuestBase>(questId);
            if (quest != null)
            {
                if (QuestInProgress(quest, QuestProgress.OnAnyTask, -1))
                {
                    //Cancel the quest somehow...
                    if (quest.Quitable == 1)
                    {
                        var questProgress = Quests[questId];
                        questProgress.Task = -1;
                        questProgress.TaskProgress = -1;
                        Quests[questId] = questProgress;
                        PacketSender.SendPlayerMsg(MyClient,
                            Strings.Get("quests", "abandoned", QuestBase.GetName(questId)), Color.Red);
                        PacketSender.SendQuestProgress(this, questId);
                    }
                }
            }
        }

        public void CompleteQuestTask(int questId, int taskId)
        {
            var quest = QuestBase.Lookup.Get<QuestBase>(questId);
            if (quest != null)
            {
                if (Quests.ContainsKey(questId))
                {
                    var questProgress = Quests[questId];
                    if (Quests[questId].Task == taskId)
                    {
                        //Let's Advance this task or complete the quest
                        for (var i = 0; i < quest.Tasks.Count; i++)
                        {
                            if (quest.Tasks[i].Id == taskId)
                            {
                                PacketSender.SendPlayerMsg(MyClient, Strings.Get("quests", "taskcompleted"));
                                if (i == quest.Tasks.Count - 1)
                                {
                                    //Complete Quest
                                    questProgress.Completed = 1;
                                    questProgress.Task = -1;
                                    questProgress.TaskProgress = -1;
                                    Quests[questId] = questProgress;
                                    if (quest.Tasks[i].CompletionEvent != null)
                                    {
                                        StartCommonEvent(quest.Tasks[i].CompletionEvent);
                                    }
                                    if (quest.EndEvent != null)
                                    {
                                        StartCommonEvent(quest.EndEvent);
                                    }
                                    PacketSender.SendPlayerMsg(MyClient, Strings.Get("quests", "completed", quest.Name),
                                        Color.Green);
                                }
                                else
                                {
                                    //Advance Task
                                    questProgress.Task = quest.Tasks[i + 1].Id;
                                    questProgress.TaskProgress = 0;
                                    Quests[questId] = questProgress;
                                    if (quest.Tasks[i].CompletionEvent != null)
                                    {
                                        StartCommonEvent(quest.Tasks[i].CompletionEvent);
                                    }
                                    if (quest.Tasks[i + 1].Objective == 1) //Gather Items
                                    {
                                        UpdateGatherItemQuests(quest.Tasks[i + 1].Data1);
                                    }
                                    PacketSender.SendPlayerMsg(MyClient, Strings.Get("quests", "updated", quest.Name),
                                        CustomColors.TaskUpdated);
                                }
                            }
                        }
                    }
                    PacketSender.SendQuestProgress(this, questId);
                }
            }
        }

        private void UpdateGatherItemQuests(int itemNum)
        {
            //If any quests demand that this item be gathered then let's handle it
            var item = ItemBase.Lookup.Get<ItemBase>(itemNum);
            if (item != null)
            {
                for (var i = 0; i < Quests.Keys.Count; i++)
                {
                    var questId = Quests.Keys.ToArray()[i];
                    var quest = QuestBase.Lookup.Get<QuestBase>(questId);
                    if (quest != null)
                    {
                        if (Quests[questId].Task > -1)
                        {
                            //Assume this quest is in progress. See if we can find the task in the quest
                            var questTask = quest.FindTask(Quests[questId].Task);
                            if (questTask != null)
                            {
                                if (questTask.Objective == 1 && questTask.Data1 == item.Index) //gather items
                                {
                                    var questProg = Quests[questId];
                                    if (questProg.TaskProgress != CountItemInstances(item.Index))
                                    {
                                        questProg.TaskProgress = CountItemInstances(item.Index);
                                        if (questProg.TaskProgress >= questTask.Data2)
                                        {
                                            CompleteQuestTask(questId, Quests[questId].Task);
                                        }
                                        else
                                        {
                                            Quests[questId] = questProg;
                                            PacketSender.SendQuestProgress(this, quest.Index);
                                            PacketSender.SendPlayerMsg(MyClient,
                                                Strings.Get("quests", "itemtask", quest.Name, questProg.TaskProgress,
                                                    questTask.Data2, ItemBase.GetName(questTask.Data1)));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        //Event Processing Methods
        public EventInstance EventExists(int map, int x, int y)
        {
            EventInstance instance = null;
            if (EventLookup.TryGetValue(new Tuple<int, int, int>(map, x, y), out instance)) return instance;
            return instance;
        }

        public EventPageInstance EventAt(int map, int x, int y, int z)
        {
            foreach (var evt in EventLookup.Values)
            {
                if (evt != null && evt.PageInstance != null)
                {
                    if (evt.PageInstance.CurrentMap == map && evt.PageInstance.CurrentX == x &&
                        evt.PageInstance.CurrentY == y && evt.PageInstance.CurrentZ == z)
                    {
                        return evt.PageInstance;
                    }
                }
            }
            return null;
        }

        public void TryActivateEvent(int mapNum, int eventIndex)
        {
            foreach (var evt in EventLookup.Values)
            {
                if (evt.MapNum == mapNum && evt.BaseEvent.Index == eventIndex)
                {
                    if (evt.PageInstance == null) return;
                    if (evt.PageInstance.Trigger != 0) return;
                    if (!IsEventOneBlockAway(evt)) return;
                    if (evt.CallStack.Count != 0) return;
                    var newStack = new CommandInstance(evt.PageInstance.MyPage)
                    {
                        CommandIndex = 0,
                        ListIndex = 0
                    };
                    evt.CallStack.Push(newStack);
                    if (!evt.IsGlobal)
                    {
                        evt.PageInstance.TurnTowardsPlayer();
                    }
                    else
                    {
                        //Turn the global event opposite of the player
                        switch (Dir)
                        {
                            case 0:
                                evt.PageInstance.GlobalClone.ChangeDir(1);
                                break;
                            case 1:
                                evt.PageInstance.GlobalClone.ChangeDir(0);
                                break;
                            case 2:
                                evt.PageInstance.GlobalClone.ChangeDir(3);
                                break;
                            case 3:
                                evt.PageInstance.GlobalClone.ChangeDir(2);
                                break;
                        }
                    }
                }
            }
        }

        public void RespondToEvent(int mapNum, int eventIndex, int responseId)
        {
            lock (mEventLock)
            {
                foreach (var evt in EventLookup.Values)
                {
                    if (evt.MapNum == mapNum && evt.BaseEvent.Index == eventIndex)
                    {
                        if (evt.CallStack.Count <= 0) return;
                        if (evt.CallStack.Peek().WaitingForResponse != CommandInstance.EventResponse.Dialogue)
                            return;
                        if (evt.CallStack.Peek().ResponseIndex == 0)
                        {
                            evt.CallStack.Peek().WaitingForResponse = CommandInstance.EventResponse.None;
                        }
                        else
                        {
                            var tmpStack = new CommandInstance(evt.CallStack.Peek().Page)
                            {
                                CommandIndex = 0,
                                ListIndex =
                                    evt.CallStack.Peek().Page.CommandLists[
                                        evt.CallStack.Peek().ListIndex].Commands[
                                        evt.CallStack.Peek().CommandIndex].Ints[responseId - 1]
                            };
                            evt.CallStack.Peek().CommandIndex++;
                            evt.CallStack.Peek().WaitingForResponse = CommandInstance.EventResponse.None;
                            evt.CallStack.Push(tmpStack);
                        }
                        return;
                    }
                }
            }
        }

        static bool IsEventOneBlockAway(EventInstance evt)
        {
            //todo this
            return true;
        }

        public EventInstance FindEvent(EventPageInstance en)
        {
            lock (mEventLock)
            {
                foreach (var evt in EventLookup.Values)
                {
                    if (evt.PageInstance == null)
                    {
                        continue;
                    }

                    if (evt.PageInstance == en || evt.PageInstance.GlobalClone == en)
                    {
                        return evt;
                    }
                }
            }
            return null;
        }

        public void SendEvents()
        {
            foreach (var evt in EventLookup.Values)
            {
                if (evt.PageInstance != null)
                {
                    evt.PageInstance.SendToClient();
                }
            }
        }

        public bool StartCommonEvent(EventBase baseEvent, int trigger = -1, string command = "", string param = "")
        {
            lock (mEventLock)
            {
                foreach (var evt in EventLookup.Values)
                {
                    if (evt.BaseEvent == baseEvent) return false;
                }
                mCommonEventLaunches++;
                var commonEventLaunch = mCommonEventLaunches;
                var tmpEvent = new EventInstance(mEventCounter++, MyClient, baseEvent, -1)
                {
                    MapNum = -1 * commonEventLaunch,
                    SpawnX = -1,
                    SpawnY = -1
                };
                EventLookup.AddOrUpdate(new Tuple<int, int, int>(-1 * commonEventLaunch, -1, -1), tmpEvent, (key, oldValue) => tmpEvent);
                //Try to Spawn a PageInstance.. if we can
                for (var i = baseEvent.MyPages.Count - 1; i >= 0; i--)
                {
                    if ((trigger == -1 || baseEvent.MyPages[i].Trigger == trigger) && tmpEvent.CanSpawnPage(i, baseEvent))
                    {
                        tmpEvent.PageInstance = new EventPageInstance(baseEvent, baseEvent.MyPages[i], baseEvent.Index, -1 * commonEventLaunch, tmpEvent, MyClient);
                        tmpEvent.PageIndex = i;
                        //Check for /command trigger
                        if (trigger == (int)EventPage.CommonEventTriggers.Command)
                        {
                            if (command.ToLower() == tmpEvent.PageInstance.MyPage.TriggerCommand.ToLower())
                            {
                                var newStack =
                                    new CommandInstance(tmpEvent.PageInstance.MyPage) { CommandIndex = 0, ListIndex = 0 };
                                tmpEvent.PageInstance.Param = param;
                                tmpEvent.CallStack.Push(newStack);
                                return true;
                            }
                        }
                        else
                        {
                            var newStack =
                                new CommandInstance(tmpEvent.PageInstance.MyPage) { CommandIndex = 0, ListIndex = 0 };
                            tmpEvent.CallStack.Push(newStack);
                            return true;
                        }
                        break;
                    }
                }
                EventLookup.TryRemove(new Tuple<int, int, int>(-1 * commonEventLaunch, -1, -1), out EventInstance z);
                return false;
            }
        }

        public override int CanMove(int moveDir)
        {
            //If crafting or locked by event return blocked 
            if (InCraft > -1 && CraftIndex > -1)
            {
                return -5;
            }
            foreach (var evt in EventLookup.Values)
            {
                if (evt.HoldingPlayer) return -5;
			}
            //TODO Check if any events are blocking us
            return base.CanMove(moveDir);
        }

        public override void Move(int moveDir, Client client, bool dontUpdate = false, bool correction = false)
        {
            var index = MyIndex;
            var oldMap = CurrentMap;
            client = MyClient;
            base.Move(moveDir, client, dontUpdate, correction);
            // Check for a warp, if so warp the player.
            var attribute =
                MapInstance.Lookup.Get<MapInstance>(Globals.Entities[index].CurrentMap).Attributes[
                    Globals.Entities[index].CurrentX, Globals.Entities[index].CurrentY];
            if (attribute != null && attribute.Value == (int)MapAttributes.Warp)
            {
                if (Convert.ToInt32(attribute.Data4) == -1)
                {
                    Globals.Entities[index].Warp(attribute.Data1, attribute.Data2, attribute.Data3,
                        Globals.Entities[index].Dir);
                }
                else
                {
                    Globals.Entities[index].Warp(attribute.Data1, attribute.Data2, attribute.Data3,
                        Convert.ToInt32(attribute.Data4));
                }
            }

            foreach (var evt in EventLookup.Values)
            {
                if (evt.MapNum == CurrentMap)
                {
                    if (evt.PageInstance != null)
                    {
                        if (evt.PageInstance.CurrentMap == CurrentMap &&
                            evt.PageInstance.CurrentX == CurrentX &&
                            evt.PageInstance.CurrentY == CurrentY &&
                            evt.PageInstance.CurrentZ == CurrentZ)
                        {
                            if (evt.PageInstance.Trigger != 1) return;
                            if (evt.CallStack.Count != 0) return;
                            var newStack = new CommandInstance(evt.PageInstance.MyPage)
                            {
                                CommandIndex = 0,
                                ListIndex = 0
                            };
                            evt.CallStack.Push(newStack);
                        }
                    }
                }
            }
        }
    }

    public class HotbarInstance
    {
        public int Slot = -1;
        public int Type = -1;
    }
}
