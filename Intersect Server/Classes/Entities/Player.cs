using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Crafting;
using Intersect.GameObjects.Events;
using Intersect.Server.Classes.Localization;
using Intersect.Server.Classes.Core;
using Intersect.Server.Classes.Database;
using Intersect.Server.Classes.Database.PlayerData;
using Intersect.Server.Classes.Database.PlayerData.Characters;
using Intersect.Server.Classes.General;

using Intersect.Server.Classes.Maps;
using Intersect.Server.Classes.Networking;
using Intersect.Server.Classes.Spells;
using Intersect.Utilities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Switch = Intersect.Server.Classes.Database.PlayerData.Characters.Switch;

namespace Intersect.Server.Classes.Entities
{
    using LegacyDatabase = Intersect.Server.Classes.Core.LegacyDatabase;

    [Table("Characters")]
    public class Player : EntityInstance
    {
        //Account
        public virtual User Account { get; private set; }

        //Character Info
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; private set; }

        //Name, X, Y, Dir, Etc all in the base Entity Class
        public Guid Class { get; set; }
        public int ClassIndex { get; set; }
        public int Gender { get; set; }
        public long Exp { get; set; }

        public int StatPoints { get; set; }

        [Column("Equipment")]
        public string EquipmentJson
        {
            get => DatabaseUtils.SaveIntArray(Equipment, Options.EquipmentSlots.Count);
            set => Equipment = DatabaseUtils.LoadIntArray(value, Options.EquipmentSlots.Count);
        }
        [NotMapped]
        public int[] Equipment { get; set; } = new int[Options.EquipmentSlots.Count];


        public DateTime? LastOnline { get; set; }

        //Bank
        public virtual List<BankSlot> Bank { get; set; } = new List<BankSlot>();

        //Friends
        public virtual List<Friend> Friends { get; set; } = new List<Friend>();

        //HotBar
        public virtual List<HotbarSlot> Hotbar { get; set; } = new List<HotbarSlot>();

        //Quests
        public virtual List<Quest> Quests { get; set; } = new List<Quest>();

        //Switches
        public virtual List<Switch> Switches { get; set; } = new List<Switch>();

        //Variables
        public virtual List<Variable> Variables { get; set; } = new List<Variable>();

        public void FixLists()
        {
            if (Spells.Count < Options.MaxPlayerSkills)
            {
                for (int i = Spells.Count; i < Options.MaxPlayerSkills; i++)
                {
                    Spells.Add(new SpellSlot(i));
                }
            }
            if (Items.Count < Options.MaxInvItems)
            {
                for (int i = Items.Count; i < Options.MaxInvItems; i++)
                {
                    Items.Add(new InventorySlot(i));
                }
            }
            if (Bank.Count < Options.MaxBankSlots)
            {
                for (int i = Bank.Count; i < Options.MaxBankSlots; i++)
                {
                    Bank.Add(new BankSlot(i));
                }
            }
            if (Hotbar.Count < Options.MaxHotbar)
            {
                for (var i = Hotbar.Count; i < Options.MaxHotbar; i++)
                {
                    Hotbar.Add(new HotbarSlot(i));
                }
            }
        }


        //5 minute timeout before someone can send a trade/party request after it has been declined
        [NotMapped] public const long REQUEST_DECLINE_TIMEOUT = 300000;  //TODO: Server option this bitch. JC is a lazy fuck


        //TODO: Clean all of this stuff up
        [NotMapped] private bool mSentMap;
        [NotMapped] public Player ChatTarget = null;
        [NotMapped] public int CraftIndex = -1;
        [NotMapped] public long CraftTimer = 0;
        //Temporary Values
        [NotMapped] private object mEventLock = new object();
        [NotMapped] public ConcurrentDictionary<Tuple<int, int, int>, EventInstance> EventLookup = new ConcurrentDictionary<Tuple<int, int, int>, EventInstance>();
        [NotMapped] private int mEventCounter = 0;
        [NotMapped] private int mCommonEventLaunches = 0;
        [NotMapped] public Player FriendRequester;
        [NotMapped] public Dictionary<Player, long> FriendRequests = new Dictionary<Player, long>();
        [NotMapped] public Bag InBag;
        [NotMapped] public bool InBank;
        [NotMapped] public int InCraft = -1;
        [NotMapped] public bool InGame;
        [NotMapped] public int InShop = -1;
        [NotMapped] public int LastMapEntered = -1;
        [NotMapped] public Client MyClient;
        [NotMapped] public long MyId = -1;
        [NotMapped] public List<Player> Party = new List<Player>();
        [NotMapped] public Player PartyRequester;
        [NotMapped] public Dictionary<Player, long> PartyRequests = new Dictionary<Player, long>();
        [NotMapped] public List<int> QuestOffers = new List<int>();
        //Event Spawned Npcs
        [NotMapped] public List<Npc> SpawnedNpcs = new List<Npc>();
        [NotMapped] public Trading Trading;

        [NotMapped]
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

        [NotMapped]
        public long ExperienceToNextLevel
        {
            get
            {
                if (Level >= Options.MaxLevel) return -1;
                var classBase = ClassBase.Lookup.Get<ClassBase>(ClassIndex);
                return classBase?.ExperienceToNextLevel(Level) ?? ClassBase.DEFAULT_BASE_EXPERIENCE;
            }
        }

        public Player() : this(-1, null)
        {
            
        }

        //Init
        public Player(int index, Client newClient) : base(index)
        {
            Trading = new Trading(this);
            MyClient = newClient;
            for (var I = 0; I < (int)Stats.StatCount; I++)
            {
                Stat[I] = new EntityStat(I, this,this);
            }
        }

        //Update
        public override void Update(long timeMs)
        {
            if (!InGame || MapIndex == -1)
            {
                return;
            }

            if (InCraft > -1 && CraftIndex > -1)
            {
                var b = CraftingTableBase.Lookup.Get<CraftingTableBase>(InCraft);
                if (CraftTimer + CraftBase.Lookup.Get<CraftBase>(b.Crafts[CraftIndex]).Time < timeMs)
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
            if (LastMapEntered != MapIndex)
            {
                if (MapInstance.Lookup.Get<MapInstance>(LastMapEntered) != null)
                {
                    MapInstance.Lookup.Get<MapInstance>(LastMapEntered).RemoveEntity(this);
                }
                if (MapIndex > -1)
                {
                    if (!MapInstance.Lookup.IndexKeys.Contains(MapIndex))
                    {
                        WarpToSpawn();
                    }
                    else
                    {
                        MapInstance.Lookup.Get<MapInstance>(MapIndex).PlayerEnteredMap(this);
                    }
                }
            }

            var currentMap = MapInstance.Lookup.Get<MapInstance>(MapIndex);
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
                    if (evt.MapNum != MapIndex)
                    {
                        foreach (var t in MapInstance.Lookup.Get<MapInstance>(MapIndex).SurroundingMaps)
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
                    PacketSender.SendEntityLeaveTo(MyClient, evt.MapIndex, (int)EntityTypes.Event, evt.MapNum);
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
            bf.WriteInteger(ClassIndex);
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

            var cls = ClassBase.Lookup.Get<ClassBase>(ClassIndex);
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

        public override void Die(int dropitems = 0, EntityInstance killer = null)
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

            var playerClass = ClassBase.Lookup.Get<ClassBase>(ClassIndex);
            if (playerClass?.VitalRegen == null) return; ;

            foreach (Vitals vital in Enum.GetValues(typeof(Vitals)))
            {
                if (vital >= Vitals.VitalCount) continue;

                var vitalId = (int)vital;
                var vitalValue = GetVital(vital);
                var maxVitalValue = GetMaxVital(vital);
                if (vitalValue >= maxVitalValue) continue;

                var vitalRegenRate = playerClass.VitalRegen[vitalId] / 100f;
                var regenValue = (int)Math.Max(1, maxVitalValue * vitalRegenRate) * Math.Abs(Math.Sign(vitalRegenRate));
                AddVital(vital, regenValue);
            }
        }

        //Leveling
        public void SetLevel(int level, bool resetExperience = false)
        {
            if (level < 1) return;
            Level = Math.Min(Options.MaxLevel, level);
            if (resetExperience) Exp = 0;
            PacketSender.SendEntityDataToProximity(this);
            PacketSender.SendExperience(MyClient);
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
                    var myclass = ClassBase.Lookup.Get<ClassBase>(ClassIndex);
                    if (myclass != null)
                    {
                        foreach (Vitals vital in Enum.GetValues(typeof(Vitals)))
                        {
                            if ((int)vital < (int)Vitals.VitalCount)
                            {
                                var maxVital = GetMaxVital(vital);
                                if (myclass.IncreasePercentage == 1)
                                {
                                    maxVital =  (int) (GetMaxVital(vital) * (1f + ((float)myclass.VitalIncrease[(int)vital] / 100f)));
                                }
                                else
                                {
                                    maxVital = GetMaxVital(vital) + myclass.VitalIncrease[(int)vital];
                                }
                                var vitalDiff = maxVital - GetMaxVital(vital);
                                SetMaxVital(vital, maxVital);
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
                                var spellInstance = new Spell(spell.Spell);
                                if (TryTeachSpell(spellInstance, true))
                                {
                                    spellMsgs.Add(Strings.Player.spelltaughtlevelup.ToString(SpellBase.GetName(spellInstance.SpellId)));
                                }
                            }
                        }
                    }
                    StatPoints += myclass.PointIncrease;
                }
            }

            PacketSender.SendPlayerMsg(MyClient, Strings.Player.levelup.ToString( Level), CustomColors.LevelUp, Name);
            PacketSender.SendActionMsg(this, Strings.Combat.levelup, CustomColors.LevelUp);
            foreach (var msg in spellMsgs)
            {
                PacketSender.SendPlayerMsg(MyClient, msg, CustomColors.Info, Name);
            }
            if (StatPoints > 0)
            {
                PacketSender.SendPlayerMsg(MyClient, Strings.Player.statpoints.ToString( StatPoints),
                    CustomColors.StatPoints, Name);
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

        public void GiveExperience(long amount)
        {
            Exp += amount;
            if (Exp < 0) Exp = 0;
            if (!CheckLevelUp())
            {
                PacketSender.SendExperience(MyClient);
            }
        }

        private bool CheckLevelUp()
        {
            var levelCount = 0;
            while (Exp >= ExperienceToNextLevel && ExperienceToNextLevel > 0)
            {
                Exp -= ExperienceToNextLevel;
                levelCount++;
            }
            if (levelCount <= 0) return false;
            LevelUp(false, levelCount);
            return true;
        }

        //Combat
        public override void KilledEntity(EntityInstance en)
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

        public void UpdateQuestKillTasks(EntityInstance en)
        { 
                //If any quests demand that this Npc be killed then let's handle it
                var npc = (Npc)en;
                foreach (var questProgress in Quests)
                {
                    var questId = questProgress.QuestId;
                    var quest = QuestBase.Lookup.Get<QuestBase>(questId);
                    if (quest != null)
                    {
                        if (questProgress.TaskId > -1)
                        {
                            //Assume this quest is in progress. See if we can find the task in the quest
                            var questTask = quest.FindTask(questProgress.TaskId);
                            if (questTask != null)
                            {
                                var questProg = Quests[questId];
                                questProg.TaskProgress++;
                                if (questProg.TaskProgress >= questTask.Data2)
                                {
                                    questProgress.TaskProgress++;
                                    if (questProgress.TaskProgress >= questTask.Data2)
                                    {
                                        CompleteQuestTask(questId, questProgress.TaskId);
                                    }
                                    else
                                    {
                                        PacketSender.SendQuestProgress(this, quest.Index);
                                        PacketSender.SendPlayerMsg(MyClient,
                                            Strings.Quests.npctask.ToString( quest.Name, questProgress.TaskProgress,
                                                questTask.Data2, NpcBase.GetName(questTask.Data1)));
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void TryAttack(EntityInstance enemy, ProjectileBase projectile, SpellBase parentSpell, ItemBase parentItem, int projectileDir)
        {
            //If Entity is resource, check for the correct tool and make sure its not a spell cast.
            if (enemy.GetType() == typeof(Resource))
            {
                if (((Resource)enemy).IsDead) return;
                // Check that a resource is actually required.
                var resource = ((Resource)enemy).MyBase;
                //Check Dynamic Requirements
                if (!EventInstance.MeetsConditionLists(resource.HarvestingRequirements, this, null))
                {
                    PacketSender.SendPlayerMsg(MyClient, Strings.Combat.resourcereqs);
                    return;
                }
                if (resource.Tool > -1 && resource.Tool < Options.ToolTypes.Count)
                {
                    if (parentItem == null || resource.Tool != parentItem.Tool)
                    {
                        PacketSender.SendPlayerMsg(MyClient, Strings.Combat.toolrequired.ToString( Options.ToolTypes[resource.Tool]));
                        return;
                    }
                }
            }
            base.TryAttack(enemy, projectile, parentSpell, parentItem, projectileDir);
        }

        public override void TryAttack(EntityInstance enemy)
        {
            if (CastTime >= Globals.System.GetTimeMs())
            {
                PacketSender.SendPlayerMsg(MyClient, Strings.Combat.channelingnoattack);
                return;
            }

            if (!IsOneBlockAway(enemy)) return;
            if (!IsFacingTarget(enemy)) return;
            if (enemy.GetType() == typeof(Npc) && ((Npc)enemy).MyBase.Behavior == (int)NpcBehavior.Friendly) return;
            if (enemy.GetType() == typeof(EventPageInstance)) return;

            ItemBase weapon = null;
            if (Options.WeaponIndex < Equipment.Length && Equipment[Options.WeaponIndex] >= 0)
            {
                weapon = ItemBase.Lookup.Get<ItemBase>( Items[((Player)Globals.Entities[MyIndex]).Equipment[Options.WeaponIndex]].ItemNum);
            }

            //If Entity is resource, check for the correct tool and make sure its not a spell cast.
            if (enemy.GetType() == typeof(Resource))
            {
                if (((Resource)enemy).IsDead) return;
                // Check that a resource is actually required.
                var resource = ((Resource)enemy).MyBase;
                //Check Dynamic Requirements
                if (!EventInstance.MeetsConditionLists(resource.HarvestingRequirements, this, null))
                {
                    PacketSender.SendPlayerMsg(MyClient, Strings.Combat.resourcereqs);
                    return;
                }
                if (resource.Tool > -1 && resource.Tool < Options.ToolTypes.Count)
                {
                    if (weapon == null || resource.Tool != weapon.Tool)
                    {
                        PacketSender.SendPlayerMsg(MyClient,
                            Strings.Combat.toolrequired.ToString( Options.ToolTypes[resource.Tool]));
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
                var classBase = ClassBase.Lookup.Get<ClassBase>(ClassIndex);
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
            PacketSender.SendEntityAttack(this, (int)EntityTypes.GlobalEntity, MapIndex, CalculateAttackTime());
        }

        public override bool CanAttack(EntityInstance en, SpellBase spell)
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
            X = newX;
            Y = newY;
            Z = zOverride;
            Dir = newDir;
            foreach (var evt in EventLookup.Values)
            {
                if (evt.MapNum > -1 && (evt.MapNum != newMap || mapSave))
                {
                    EventLookup.TryRemove(new Tuple<int, int, int>(evt.MapNum, evt.BaseEvent.SpawnX, evt.BaseEvent.SpawnY), out EventInstance z);
                }
            }
            if (newMap != MapIndex || mSentMap == false)
            {
                var oldMap = MapInstance.Lookup.Get<MapInstance>(MapIndex);
                if (oldMap != null)
                {
                    oldMap.RemoveEntity(this);
                }
                PacketSender.SendEntityLeave(MyIndex, (int)EntityTypes.Player, MapIndex);
                MapIndex = newMap;
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
            var cls = ClassBase.Lookup.Get<ClassBase>(ClassIndex);
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
        public bool CanGiveItem(Item item)
        {
            var itemBase = ItemBase.Lookup.Get<ItemBase>(item.ItemNum);
            if (itemBase != null)
            {
                if (itemBase.IsStackable())
                {
                    for (var i = 0; i < Options.MaxInvItems; i++)
                    {
                        if (Items[i].ItemNum == item.ItemNum)
                        {
                            return true;
                        }
                    }
                }

                //Either a non stacking item, or we couldn't find the item already existing in the players inventory
                for (var i = 0; i < Options.MaxInvItems; i++)
                {
                    if (Items[i].ItemNum == -1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool TryGiveItem(Item item, bool sendUpdate = true)
        {
            var itemBase = ItemBase.Lookup.Get<ItemBase>(item.ItemNum);
            if (itemBase != null)
            {
                if (itemBase.IsStackable())
                {
                    for (var i = 0; i < Options.MaxInvItems; i++)
                    {
                        if (Items[i].ItemNum == item.ItemNum)
                        {
                            Items[i].ItemVal += item.ItemVal;
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
                    if (Items[i].ItemNum == -1)
                    {
                        Items[i].Set(item);
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
            var tmpInstance = Items[item2].Clone();
            Items[item2].Set(Items[item1]);
            Items[item1].Set(tmpInstance);
            PacketSender.SendInventoryItemUpdate(MyClient, item1);
            PacketSender.SendInventoryItemUpdate(MyClient, item2);
            EquipmentProcessItemSwap(item1, item2);
            HotbarProcessItemSwap(item1, item2);
        }

        public void DropItems(int slot, int amount)
        {
            var itemBase = ItemBase.Lookup.Get<ItemBase>(Items[slot].ItemNum);
            if (itemBase != null)
            {
                if (itemBase.Bound)
                {
                    PacketSender.SendPlayerMsg(MyClient, Strings.Items.bound, CustomColors.ItemBound);
                    return;
                }
                for (var i = 0; i < Options.EquipmentSlots.Count; i++)
                {
                    if (Equipment[i] == slot)
                    {
                        PacketSender.SendPlayerMsg(MyClient, Strings.Items.equipped, CustomColors.ItemBound);
                        return;
                    }
                }

                if (amount >= Items[slot].ItemVal)
                {
                    amount = Items[slot].ItemVal;
                }
                if (itemBase.IsStackable())
                {
                    MapInstance.Lookup.Get<MapInstance>(MapIndex)
                        .SpawnItem(X, Y, Items[slot], amount);
                }
                else
                {
                    for (var i = 0; i < amount; i++)
                    {
                        MapInstance.Lookup.Get<MapInstance>(MapIndex)
                            .SpawnItem(X, Y, Items[slot], 1);
                    }
                }
                if (amount == Items[slot].ItemVal)
                {
                    Items[slot].Set(Item.None);
                    EquipmentProcessItemLoss(slot);
                }
                else
                {
                    Items[slot].ItemVal -= amount;
                }
                UpdateGatherItemQuests(itemBase.Index);
                PacketSender.SendInventoryItemUpdate(MyClient, slot);
            }
        }

        public void UseItem(int slot)
        {
            var equipped = false;
            var Item = Items[slot];
            var itemBase = ItemBase.Lookup.Get<ItemBase>(Item.ItemNum);
            if (itemBase != null)
            {
                //Check if the user is silenced or stunned
                var statuses = Statuses.Values.ToArray();
                foreach (var status in statuses)
                {
                    if (status.Type == (int)StatusTypes.Stun)
                    {
                        PacketSender.SendPlayerMsg(MyClient, Strings.Items.stunned);
                        return;
                    }
                }

				// Unequip items even if you do not meet the requirements. 
				// (Need this for silly devs who give people items and then later add restrictions...)
				if (itemBase.ItemType == ItemTypes.Equipment)
                for (var i = 0; i < Options.EquipmentSlots.Count; i++)
				{
					if (Equipment[i] == slot)
					{
						Equipment[i] = -1;
						PacketSender.SendPlayerEquipmentToProximity(this);
						PacketSender.SendEntityStats(this);
						return;
					}
				}

				if (!EventInstance.MeetsConditionLists(itemBase.UsageRequirements, this, null))
                {
                    PacketSender.SendPlayerMsg(MyClient, Strings.Items.dynamicreq);
                    return;
                }

                switch (itemBase.ItemType)
                {
                    case ItemTypes.None:
                    case ItemTypes.Currency:
                        PacketSender.SendPlayerMsg(MyClient, Strings.Items.cannotuse);
                        return;
                    case ItemTypes.Consumable:
                        var s = Strings.Combat.removesymbol;
                        if (itemBase.Data2 > 0)
                        {
                            s = Strings.Combat.addsymbol;
                        }

                        switch (itemBase.ConsumableType)
                        {
                            case ConsumableType.Health:
                                AddVital(Vitals.Health, itemBase.Data2);
                                if (s == Strings.Combat.addsymbol)
                                {
                                    PacketSender.SendActionMsg(this, s + itemBase.Data2, CustomColors.Heal);
                                }
                                else
                                {
                                    PacketSender.SendActionMsg(this, s + itemBase.Data2, CustomColors.PhysicalDamage);
                                    if (!HasVital(Vitals.Health)) //Add a death handler for poison.
                                    {
                                        Die();
                                    }
                                }
                                break;
                            case ConsumableType.Mana:
                                AddVital(Vitals.Mana, itemBase.Data2);
                                PacketSender.SendActionMsg(this, s + itemBase.Data2, CustomColors.AddMana);
                                break;

                            case ConsumableType.Experience:
                                GiveExperience(itemBase.Data2);
                                PacketSender.SendActionMsg(this, s + itemBase.Data2, CustomColors.Experience);
                                break;

                            default:
                                return;
                        }
                        TakeItemsBySlot(slot, 1);
                        break;
                    case ItemTypes.Equipment:
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
                                        if (ItemBase.Lookup.Get<ItemBase>(Items[Equipment[Options.WeaponIndex]]
                                                .ItemNum) != null &&
                                            Convert.ToBoolean(
                                                ItemBase.Lookup
                                                    .Get<ItemBase>(Items[Equipment[Options.WeaponIndex]].ItemNum)
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
                    case ItemTypes.Spell:
                        if (itemBase.Data1 <= -1) return;
                        if (!TryTeachSpell(new Spell(itemBase.Data1))) return;
                        TakeItemsBySlot(slot, 1);
                        break;
                    case ItemTypes.Event:
                        var evt = EventBase.Lookup.Get<EventBase>(itemBase.Data1);
                        if (evt == null) return;
                        if (!StartCommonEvent(evt)) return;
                        TakeItemsBySlot(slot, 1);
                        break;
                    case ItemTypes.Bag:
                        OpenBag(Item, itemBase);
                        break;
                    default:
                        PacketSender.SendPlayerMsg(MyClient, Strings.Items.notimplemented);
                        return;
                }
                if (itemBase.Animation != null)
                {
                    PacketSender.SendAnimationToProximity(itemBase.Animation.Index, 1, MyIndex, MapIndex,0,0, Dir); //Target Type 1 will be global entity
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
            var itemBase = ItemBase.Lookup.Get<ItemBase>(Items[slot].ItemNum);
            if (itemBase != null)
            {
                if (itemBase.IsStackable())
                {
                    if (amount > Items[slot].ItemVal)
                    {
                        amount = Items[slot].ItemVal;
                    }
                    else
                    {
                        if (amount == Items[slot].ItemVal)
                        {
                            Items[slot].Set(Item.None);
                            EquipmentProcessItemLoss(slot);
                            returnVal = true;
                        }
                        else
                        {
                            Items[slot].ItemVal -= amount;
                            returnVal = true;
                        }
                    }
                }
                else
                {
                    Items[slot].Set(Item.None);
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
            if (CountItems(itemNum) < amount)
                return false;

            if (Items == null)
                return false;

            var invbackup = Items.Select(item => item?.Clone()).ToList();

            for (var i = 0; i < Options.MaxInvItems; i++)
            {
                var item = Items[i];
                if (item?.ItemNum != itemNum)
                    continue;

                if (item.ItemVal <= 1)
                {
                    amount -= 1;
                    Items[i].Set(Item.None);
                    PacketSender.SendInventoryItemUpdate(MyClient, i);
                    if (amount == 0)
                        return true;
                }
                else
                {
                    if (amount >= item.ItemVal)
                    {
                        amount -= item.ItemVal;
                        Items[i].Set(Item.None);
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
            //Restore Backup
            for (int i = 0; i < invbackup.Count; i++)
            {
                Items[i].Set(invbackup[i]);
            }
            PacketSender.SendInventory(MyClient);
            return false;
        }

        public int FindItem(int itemNum, int itemVal = 1)
        {
            if (Items == null)
                return -1;

            for (var i = 0; i < Options.MaxInvItems; i++)
            {
                var item = Items[i];
                if (item?.ItemNum != itemNum)
                    continue;

                if (item.ItemVal >= itemVal)
                    return i;
            }

            return -1;
        }

        public int CountItems(int itemNum)
        {
            if (Items == null)
                return -1;

            var count = 0;
            for (var i = 0; i < Options.MaxInvItems; i++)
            {
                var item = Items[i];
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
                if (Items[Equipment[Options.WeaponIndex]].ItemNum > -1)
                {
                    var item = ItemBase.Lookup.Get<ItemBase>(Items[Equipment[Options.WeaponIndex]].ItemNum);
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
            var sellItemNum = Items[slot].ItemNum;
            if (InShop == -1) return;
            var shop = ShopBase.Lookup.Get<ShopBase>(InShop);
            if (shop != null)
            {
                var itemBase = ItemBase.Lookup.Get<ItemBase>(Items[slot].ItemNum);
                if (itemBase != null)
                {
                    if (itemBase.Bound)
                    {
                        PacketSender.SendPlayerMsg(MyClient, Strings.Shops.bound, CustomColors.ItemBound);
                        return;
                    }

                    //Check if this is a bag with items.. if so don't allow sale
                    if (itemBase.ItemType == ItemTypes.Bag)
                    {
                        if (Items[slot].Bag == null) Items[slot].Bag = LegacyDatabase.GetBag(Items[slot]);
                        if (Items[slot].Bag != null)
                        {
                            if (!LegacyDatabase.BagEmpty(Items[slot].Bag))
                            {
                                PacketSender.SendPlayerMsg(MyClient, Strings.Bags.onlysellempty,
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
                                PacketSender.SendPlayerMsg(MyClient, Strings.Shops.doesnotaccept,
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
                            PacketSender.SendPlayerMsg(MyClient, Strings.Shops.doesnotaccept,
                                CustomColors.Error);
                            return;
                        }
                        else
                        {
                            rewardItemNum = shop.DefaultCurrency;
                            rewardItemVal = itemBase.Price;
                        }
                    }

                    if (amount >= Items[slot].ItemVal)
                    {
                        amount = Items[slot].ItemVal;
                    }
                    if (amount == Items[slot].ItemVal)
                    {
                        //Definitely can get reward.
                        Items[slot].Set(Item.None);
                        EquipmentProcessItemLoss(slot);
                    }
                    else
                    {
                        //check if can get reward
                        if (!CanGiveItem(new Item(rewardItemNum, rewardItemVal)))
                        {
                            canSellItem = false;
                        }
                        else
                        {
                            Items[slot].ItemVal -= amount;
                        }
                    }
                    if (canSellItem)
                    {
                        TryGiveItem(new Item(rewardItemNum, rewardItemVal * amount), true);
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
                            if (CanGiveItem(new Item(buyItemNum, buyItemAmt)))
                            {
                                if (shop.SellingItems[slot].CostItemVal > 0)
                                {
                                    TakeItemsBySlot(
                                        FindItem(shop.SellingItems[slot].CostItemNum,
                                            shop.SellingItems[slot].CostItemVal * buyItemAmt),
                                        shop.SellingItems[slot].CostItemVal * buyItemAmt);
                                }
                                TryGiveItem(new Item(buyItemNum, buyItemAmt), true);
                            }
                            else
                            {
                                if (shop.SellingItems[slot].CostItemVal * buyItemAmt ==
                                    Items[
                                        FindItem(shop.SellingItems[slot].CostItemNum,
                                            shop.SellingItems[slot].CostItemVal * buyItemAmt)].ItemVal)
                                {
                                    TakeItemsBySlot(
                                        FindItem(shop.SellingItems[slot].CostItemNum,
                                            shop.SellingItems[slot].CostItemVal * buyItemAmt),
                                        shop.SellingItems[slot].CostItemVal * buyItemAmt);
                                    TryGiveItem(new Item(buyItemNum, buyItemAmt), true);
                                }
                                else
                                {
                                    PacketSender.SendPlayerMsg(MyClient, Strings.Shops.inventoryfull,
                                        CustomColors.Error, Name);
                                }
                            }
                        }
                        else
                        {
                            PacketSender.SendPlayerMsg(MyClient, Strings.Shops.cantafford,
                                CustomColors.Error, Name);
                        }
                    }
                }
            }
        }

        //Crafting
        public bool OpenCraftingTable(CraftingTableBase table)
        {
            if (IsBusy()) return false;
            if (table != null)
            {
                InCraft = table.Index;
                PacketSender.SendOpenCraftingTable(MyClient, table);
            }
            return true;
        }

        public void CloseCraftingTable()
        {
            if (InCraft > -1 && CraftIndex == -1)
            {
                InCraft = -1;
                PacketSender.SendCloseCraftingTable(MyClient);
            }
        }

        //Craft a new item
        public void CraftItem(int index)
        {
            if (InCraft > -1)
            {
                var invbackup = new List<Item>();
                foreach (var item in Items)
                {
                    invbackup.Add(item.Clone());
                }

                //Quickly Look through the inventory and create a catalog of what items we have, and how many
                var itemdict = new Dictionary<int, int>();
                foreach (var item in Items)
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
                foreach (var c in CraftBase.Lookup.Get<CraftBase>(CraftingTableBase.Lookup.Get<CraftingTableBase>(InCraft).Crafts[index]).Ingredients)
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
                foreach (var c in CraftBase.Lookup.Get<CraftBase>(CraftingTableBase.Lookup.Get<CraftingTableBase>(InCraft).Crafts[index]).Ingredients)
                {
                    if (!TakeItemsByNum(c.Item, c.Quantity))
                    {
                        for (int i = 0; i < invbackup.Count; i++)
                        {
                            Items[i].Set(invbackup[i]);
                        }
                        PacketSender.SendInventory(MyClient);
                        CraftIndex = -1;
                        return;
                    }
                }

                //Give them the craft
                if (TryGiveItem(new Item(CraftBase.Lookup.Get<CraftBase>(CraftingTableBase.Lookup.Get<CraftingTableBase>(InCraft).Crafts[index]).Item, 1)))
                {
                    PacketSender.SendPlayerMsg(MyClient,
                        Strings.Crafting.crafted.ToString(
                            ItemBase.GetName(CraftBase.Lookup.Get<CraftBase>(CraftingTableBase.Lookup.Get<CraftingTableBase>(InCraft).Crafts[index]).Item)),
                        CustomColors.Crafted);
                }
                else
                {
                    for (int i = 0; i < invbackup.Count; i++)
                    {
                        Items[i].Set(invbackup[i]);
                    }
                    PacketSender.SendInventory(MyClient);
                    PacketSender.SendPlayerMsg(MyClient,
                        Strings.Crafting.nospace.ToString(
                            ItemBase.GetName(CraftBase.Lookup.Get<CraftBase>(CraftingTableBase.Lookup.Get<CraftingTableBase>(InCraft).Crafts[index]).Item)),
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
            foreach (var item in Items)
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
            foreach (var c in CraftBase.Lookup.Get<CraftBase>(CraftingTableBase.Lookup.Get<CraftingTableBase>(InCraft).Crafts[index]).Ingredients)
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
            return InShop > 1 || InBank || InCraft > -1 || Trading.Counterparty != null;
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
            var itemBase = ItemBase.Lookup.Get<ItemBase>(Items[slot].ItemNum);
            if (itemBase != null)
            {
                if (Items[slot].ItemNum > -1)
                {
                    if (itemBase.IsStackable())
                    {
                        if (amount >= Items[slot].ItemVal)
                        {
                            amount = Items[slot].ItemVal;
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
                            if (Bank[i] != null && Bank[i].ItemNum == Items[slot].ItemNum)
                            {
                                amount = Math.Min(amount, int.MaxValue - Bank[i].ItemVal);
                                Bank[i].ItemVal += amount;
                                //Remove Items from inventory send updates
                                if (amount >= Items[slot].ItemVal)
                                {
                                    Items[slot].Set(Item.None);
                                    EquipmentProcessItemLoss(slot);
                                }
                                else
                                {
                                    Items[slot].ItemVal -= amount;
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
                            Bank[i].Set(Items[slot]);
                            Bank[i].ItemVal = amount;
                            //Remove Items from inventory send updates
                            if (amount >= Items[slot].ItemVal)
                            {
                                Items[slot].Set(Item.None);
                                EquipmentProcessItemLoss(slot);
                            }
                            else
                            {
                                Items[slot].ItemVal -= amount;
                            }
                            PacketSender.SendInventoryItemUpdate(MyClient, slot);
                            PacketSender.SendBankUpdate(MyClient, i);
                            return;
                        }
                    }
                    PacketSender.SendPlayerMsg(MyClient, Strings.Banks.banknospace, CustomColors.Error);
                }
                else
                {
                    PacketSender.SendPlayerMsg(MyClient, Strings.Banks.depositinvalid, CustomColors.Error);
                }
            }
        }

        public void WithdrawItem(int slot, int amount)
        {
            if (!InBank) return;

            Debug.Assert(ItemBase.Lookup != null, "ItemBase.Lookup != null");
            Debug.Assert(Bank != null, "Bank != null");
            Debug.Assert(Items != null, "Inventory != null");

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
                        var inventorySlotItem = Items[i];
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
                        if (Items[j] != null && Items[j].ItemNum != -1) continue;
                        inventorySlot = j;
                        break;
                    }
                }

                    /* If we don't have a slot send an error. */
                    if (inventorySlot < 0)
                    {
                        PacketSender.SendPlayerMsg(MyClient, Strings.Banks.inventorynospace,
                            CustomColors.Error);
                        return; //Panda forgot this :P
                    }

                /* Move the items to the inventory */
                Debug.Assert(Items[inventorySlot] != null, "Inventory[inventorySlot] != null");
                amount = Math.Min(amount, int.MaxValue - Items[inventorySlot].ItemVal);

                if (Items[inventorySlot] == null || Items[inventorySlot].ItemNum == -1 ||
                    Items[inventorySlot].ItemVal < 0)
                {
                    Items[inventorySlot].Set(bankSlotItem);
                    Items[inventorySlot].ItemVal = 0;
                }

                Items[inventorySlot].ItemVal += amount;
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
                    PacketSender.SendPlayerMsg(MyClient, Strings.Banks.withdrawinvalid, CustomColors.Error);
                }
            }
        
        public void SwapBankItems(int item1, int item2)
        {
            Item tmpInstance = null;
            if (Bank[item2] != null) tmpInstance = Bank[item2].Clone();
            if (Bank[item1] != null)
            {
                Bank[item2].Set(Bank[item1]);
            }
            else
            {
                Bank[item2] = null;
            }
            if (tmpInstance != null)
            {
                Bank[item1].Set(tmpInstance);
            }
            else
            {
                Bank[item1] = null;
            }
            PacketSender.SendBankUpdate(MyClient, item1);
            PacketSender.SendBankUpdate(MyClient, item2);
        }

        //Bag
        public bool OpenBag(Item bagItem, ItemBase itemBase)
        {
            if (IsBusy()) return false;
            //Bags will never, ever, be stackable. Going to use the value property for the bag id in the Database.
            if (bagItem.Bag == null)
            {
                bagItem.Bag = LegacyDatabase.GetBag(bagItem);
                if (bagItem.Bag == null) //Bag doesnt exist, creatre it!
                {
                    //Create the Bag
                    var slotCount = itemBase.Data1;
                    if (slotCount < 1) slotCount = 1;
                    bagItem.Bag = new Bag(slotCount);
                }
                bagItem.Bag.Slots = bagItem.Bag.Slots.OrderBy(p => p.Slot).ToList();
            }
            //Send the bag to the player (this will make it appear on screen)
            InBag = bagItem.Bag;
            PacketSender.SendOpenBag(MyClient, bagItem.Bag.SlotCount, bagItem.Bag);
            return true;
        }

        public bool HasBag(Bag bag)
        {
            for (var i = 0; i < Items.Count; i++)
            {
                if (Items[i] != null && Items[i].Bag == bag) return true;
            }
            return false;
        }

        public Bag GetBag()
        {
            if (InBag != null)
            {
                return InBag;
            }
            return null;
        }

        public void CloseBag()
        {
            if (InBag != null)
            {
                InBag = null;
                PacketSender.SendCloseBag(MyClient);
            }
        }

        public void StoreBagItem(int slot, int amount)
        {
            if (InBag == null || !HasBag(InBag)) return;
            var itemBase = ItemBase.Lookup.Get<ItemBase>(Items[slot].ItemNum);
            var bag = GetBag();
            if (itemBase != null && bag != null)
            {
                if (Items[slot].ItemNum > -1)
                {
                    if (itemBase.IsStackable())
                    {
                        if (amount >= Items[slot].ItemVal)
                        {
                            amount = Items[slot].ItemVal;
                        }
                    }
                    else
                    {
                        amount = 1;
                    }

                    //Make Sure we are not Storing a Bag inside of itself
                    if (Items[slot].Bag == InBag)
                    {
                        PacketSender.SendPlayerMsg(MyClient, Strings.Bags.baginself, CustomColors.Error);
                        return;
                    }

                    //Find a spot in the bag for it!
                    if (itemBase.IsStackable())
                    {
                        for (var i = 0; i < bag.SlotCount; i++)
                        {
                            if (bag.Slots[i] != null && bag.Slots[i].ItemNum == Items[slot].ItemNum)
                            {
                                amount = Math.Min(amount, int.MaxValue - bag.Slots[i].ItemVal);
                                bag.Slots[i].ItemVal += amount;
                                //Remove Items from inventory send updates
                                if (amount >= Items[slot].ItemVal)
                                {
                                    Items[slot].Set(Item.None);
                                    EquipmentProcessItemLoss(slot);
                                }
                                else
                                {
                                    Items[slot].ItemVal -= amount;
                                }
                                //LegacyDatabase.SaveBagItem(InBag, i, bag.Items[i]);
                                PacketSender.SendInventoryItemUpdate(MyClient, slot);
                                PacketSender.SendBagUpdate(MyClient, i, bag.Slots[i]);
                                return;
                            }
                        }
                    }

                    //Either a non stacking item, or we couldn't find the item already existing in the players inventory
                    for (var i = 0; i < bag.SlotCount; i++)
                    {
                        if (bag.Slots[i].ItemNum == -1)
                        {
                            bag.Slots[i].Set(Items[slot]);
                            bag.Slots[i].ItemVal = amount;
                            //Remove Items from inventory send updates
                            if (amount >= Items[slot].ItemVal)
                            {
                                Items[slot].Set(Item.None);
                                EquipmentProcessItemLoss(slot);
                            }
                            else
                            {
                                Items[slot].ItemVal -= amount;
                            }
                            //LegacyDatabase.SaveBagItem(InBag, i, bag.Items[i]);
                            PacketSender.SendInventoryItemUpdate(MyClient, slot);
                            PacketSender.SendBagUpdate(MyClient, i, bag.Slots[i]);
                            return;
                        }
                    }
                    PacketSender.SendPlayerMsg(MyClient, Strings.Bags.bagnospace, CustomColors.Error);
                }
                else
                {
                    PacketSender.SendPlayerMsg(MyClient, Strings.Bags.depositinvalid, CustomColors.Error);
                }
            }
        }

        public void RetreiveBagItem(int slot, int amount)
        {
            if (InBag == null || !HasBag(InBag)) return;
            var bag = GetBag();
            if (bag == null || slot > bag.Slots.Count || bag.Slots[slot] == null) return;
            var itemBase = ItemBase.Lookup.Get<ItemBase>(bag.Slots[slot].ItemNum);
            var inventorySlot = -1;
            if (itemBase != null)
            {
                if (bag.Slots[slot] != null && bag.Slots[slot].ItemNum > -1)
                {
                    if (itemBase.IsStackable())
                    {
                        if (amount >= bag.Slots[slot].ItemVal)
                        {
                            amount = bag.Slots[slot].ItemVal;
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
                            if (Items[i] != null && Items[i].ItemNum == bag.Slots[slot].ItemNum)
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
                            if (Items[j] == null || Items[j].ItemNum == -1)
                            {
                                inventorySlot = j;
                                break;
                            }
                        }
                    }

                    /* If we don't have a slot send an error. */
                    if (inventorySlot < 0)
                    {
                        PacketSender.SendPlayerMsg(MyClient, Strings.Bags.inventorynospace,
                            CustomColors.Error);
                        return; //Panda forgot this :P
                    }

                    /* Move the items to the inventory */
                    amount = Math.Min(amount, int.MaxValue - Items[inventorySlot].ItemVal);

                    if (Items[inventorySlot] == null || Items[inventorySlot].ItemNum == -1 ||
                        Items[inventorySlot].ItemVal < 0)
                    {
                        Items[inventorySlot].Set(bag.Slots[slot]);
                        Items[inventorySlot].ItemVal = 0;
                    }

                    Items[inventorySlot].ItemVal += amount;
                    if (amount >= bag.Slots[slot].ItemVal)
                    {
                        bag.Slots[slot] = null;
                    }
                    else
                    {
                        bag.Slots[slot].ItemVal -= amount;
                    }
                    //LegacyDatabase.SaveBagItem(InBag, slot, bag.Items[slot]);

                    PacketSender.SendInventoryItemUpdate(MyClient, inventorySlot);
                    PacketSender.SendBagUpdate(MyClient, slot, bag.Slots[slot]);
                }
                else
                {
                    PacketSender.SendPlayerMsg(MyClient, Strings.Bags.withdrawinvalid, CustomColors.Error);
                }
            }
        }

        public void SwapBagItems(int item1, int item2)
        {
            if (InBag != null || !HasBag(InBag)) return;
            var bag = GetBag();
            Item tmpInstance = null;
            if (bag.Slots[item2] != null) tmpInstance = bag.Slots[item2].Clone();
            if (bag.Slots[item1] != null)
            {
                bag.Slots[item2].Set(bag.Slots[item1]);
            }
            else
            {
                bag.Slots[item2] = null;
            }
            if (tmpInstance != null)
            {
                bag.Slots[item1].Set(tmpInstance);
            }
            else
            {
                bag.Slots[item1] = null;
            }
            PacketSender.SendBagUpdate(MyClient, item1, bag.Slots[item1]);
            PacketSender.SendBagUpdate(MyClient, item2, bag.Slots[item2]);
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
                if (Trading.Requester == null && PartyRequester == null && FriendRequester == null)
                {
                    FriendRequester = fromPlayer;
                    PacketSender.SendFriendRequest(MyClient, fromPlayer);
                    PacketSender.SendPlayerMsg(fromPlayer.MyClient, Strings.Friends.sent,
                        CustomColors.RequestSent);
                }
                else
                {
                    PacketSender.SendPlayerMsg(fromPlayer.MyClient, Strings.Friends.busy.ToString( Name),
                        CustomColors.Error);
                }
            }
        }

        public bool HasFriend(Player character)
        {
            foreach (var friend in Friends)
            {
                if (friend.Target == character) return true;
            }
            return false;
        }

        public void AddFriend(Player character)
        {
            var friend = new Friend(this, character);
            Friends.Add(friend);
        }

        //Trading
        public void InviteToTrade(Player fromPlayer)
        {
            if (fromPlayer.Trading.Requests.ContainsKey(this))
            {
                fromPlayer.Trading.Requests.Remove(this);
            }
            if (Trading.Requests.ContainsKey(fromPlayer) && Trading.Requests[fromPlayer] > Globals.System.GetTimeMs())
            {
                PacketSender.SendPlayerMsg(fromPlayer.MyClient, Strings.Trading.alreadydenied,
                    CustomColors.Error);
            }
            else
            {
                if (Trading.Requester == null && PartyRequester == null && FriendRequester == null)
                {
                    Trading.Requester = fromPlayer;
                    PacketSender.SendTradeRequest(MyClient, fromPlayer);
                }
                else
                {
                    PacketSender.SendPlayerMsg(fromPlayer.MyClient, Strings.Trading.busy.ToString( Name),
                        CustomColors.Error);
                }
            }
        }

        public void OfferItem(int slot, int amount)
        {
            if (Trading.Counterparty == null) return;
            var itemBase = ItemBase.Lookup.Get<ItemBase>(Items[slot].ItemNum);
            if (itemBase != null)
            {
                if (Items[slot].ItemNum > -1)
                {
                    if (itemBase.IsStackable())
                    {
                        if (amount >= Items[slot].ItemVal)
                        {
                            amount = Items[slot].ItemVal;
                        }
                    }
                    else
                    {
                        amount = 1;
                    }

                    //Check if this is a bag with items.. if so don't allow sale
                    if (itemBase.ItemType == ItemTypes.Bag)
                    {
                        if (Items[slot].Bag == null) Items[slot].Bag = LegacyDatabase.GetBag(Items[slot]);
                        if (Items[slot].Bag != null)
                        {
                            if (!LegacyDatabase.BagEmpty(Items[slot].Bag))
                            {
                                PacketSender.SendPlayerMsg(MyClient, Strings.Bags.onlytradeempty,
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
                            if (Trading.Offer[i] != null && Trading.Offer[i].ItemNum == Items[slot].ItemNum)
                            {
                                amount = Math.Min(amount, int.MaxValue - Trading.Offer[i].ItemVal);
                                Trading.Offer[i].ItemVal += amount;
                                //Remove Items from inventory send updates
                                if (amount >= Items[slot].ItemVal)
                                {
                                    Items[slot].Set(Item.None);
                                    EquipmentProcessItemLoss(slot);
                                }
                                else
                                {
                                    Items[slot].ItemVal -= amount;
                                }
                                PacketSender.SendInventoryItemUpdate(MyClient, slot);
                                PacketSender.SendTradeUpdate(MyClient, MyIndex, i);
                                PacketSender.SendTradeUpdate(Trading.Counterparty?.MyClient, MyIndex, i);
                                return;
                            }
                        }
                    }

                    //Either a non stacking item, or we couldn't find the item already existing in the players inventory
                    for (var i = 0; i < Options.MaxInvItems; i++)
                    {
                        if (Trading.Offer[i] == null || Trading.Offer[i].ItemNum == -1)
                        {
                            Trading.Offer[i] = Items[slot].Clone();
                            Trading.Offer[i].ItemVal = amount;
                            //Remove Items from inventory send updates
                            if (amount >= Items[slot].ItemVal)
                            {
                                Items[slot].Set(Item.None);
                                EquipmentProcessItemLoss(slot);
                            }
                            else
                            {
                                Items[slot].ItemVal -= amount;
                            }
                            PacketSender.SendInventoryItemUpdate(MyClient, slot);
                            PacketSender.SendTradeUpdate(MyClient, MyIndex, i);
                            PacketSender.SendTradeUpdate(Trading.Counterparty?.MyClient, MyIndex, i);
                            return;
                        }
                    }
                    PacketSender.SendPlayerMsg(MyClient, Strings.Trading.tradenospace, CustomColors.Error);
                }
                else
                {
                    PacketSender.SendPlayerMsg(MyClient, Strings.Trading.offerinvalid, CustomColors.Error);
                }
            }
        }

        public void RevokeItem(int slot, int amount)
        {
            if (Trading.Counterparty == null) return;

            var itemBase = ItemBase.Lookup.Get<ItemBase>(Trading.Offer[slot].ItemNum);
            if (itemBase == null)
            {
                return;
            }

            if (Trading.Offer[slot] == null || Trading.Offer[slot].ItemNum < 0)
            {
                PacketSender.SendPlayerMsg(MyClient, Strings.Trading.revokeinvalid, CustomColors.Error);
                return;
            }

            var inventorySlot = -1;
            var stackable = itemBase.IsStackable();
            if (stackable)
            {
                /* Find an existing stack */
                for (var i = 0; i < Options.MaxInvItems; i++)
                {
                    if (Items[i] != null && Items[i].ItemNum == Trading.Offer[slot].ItemNum)
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
                    if (Items[j] == null || Items[j].ItemNum == -1)
                    {
                        inventorySlot = j;
                        break;
                    }
                }
            }

            /* If we don't have a slot send an error. */
            if (inventorySlot < 0)
            {
                PacketSender.SendPlayerMsg(MyClient, Strings.Trading.inventorynospace, CustomColors.Error);
            }

            /* Move the items to the inventory */
            amount = Math.Min(amount, int.MaxValue - Items[inventorySlot].ItemVal);

            if (Items[inventorySlot] == null || Items[inventorySlot].ItemNum == -1 ||
                Items[inventorySlot].ItemVal < 0)
            {
                Items[inventorySlot].Set(Trading.Offer[slot]);
            }

            Items[inventorySlot].ItemVal += amount;
            if (amount >= Trading.Offer[slot].ItemVal)
            {
                Trading.Offer[slot] = null;
            }
            else
            {
                Trading.Offer[slot].ItemVal -= amount;
            }

            PacketSender.SendInventoryItemUpdate(MyClient, inventorySlot);
            PacketSender.SendTradeUpdate(MyClient, MyIndex, slot);
            PacketSender.SendTradeUpdate(Trading.Counterparty?.MyClient, MyIndex, slot);
        }

        public void ReturnTradeItems()
        {
            if (Trading.Counterparty == null) return;

            for (var i = 0; i < Options.MaxInvItems; i++)
            {
                if (Trading.Offer[i].ItemNum > 0)
                {
                    if (!TryGiveItem(new Item(Trading.Offer[i])))
                    {
                        MapInstance.Lookup.Get<MapInstance>(MapIndex)
                            .SpawnItem(X, Y, Trading.Offer[i], Trading.Offer[i].ItemVal);
                        PacketSender.SendPlayerMsg(MyClient, Strings.Trading.itemsdropped,
                            CustomColors.Error);
                    }
                    Trading.Offer[i].ItemNum = 0;
                    Trading.Offer[i].ItemVal = 0;
                }
            }
            PacketSender.SendInventory(MyClient);
        }

        public void CancelTrade()
        {
            if (Trading.Counterparty == null) return;
            Trading.Counterparty.ReturnTradeItems();
            PacketSender.SendPlayerMsg(Trading.Counterparty.MyClient, Strings.Trading.declined, CustomColors.Error);
            PacketSender.SendTradeClose(Trading.Counterparty.MyClient);
            Trading.Counterparty.Trading.Counterparty = null;

            ReturnTradeItems();
            PacketSender.SendPlayerMsg(MyClient, Strings.Trading.declined, CustomColors.Error);
            PacketSender.SendTradeClose(MyClient);
            Trading.Counterparty = null;
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
                PacketSender.SendPlayerMsg(fromPlayer.MyClient, Strings.Parties.alreadydenied,
                    CustomColors.Error);
            }
            else
            {
                if (Trading.Requester == null && PartyRequester == null && FriendRequester == null)
                {
                    PartyRequester = fromPlayer;
                    PacketSender.SendPartyInvite(MyClient, fromPlayer);
                }
                else
                {
                    PacketSender.SendPlayerMsg(fromPlayer.MyClient, Strings.Parties.busy.ToString( Name),
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
                    PacketSender.SendPlayerMsg(MyClient, Strings.Parties.leaderinvonly, CustomColors.Error);
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
                    PacketSender.SendPlayerMsg(Party[i].MyClient, Strings.Parties.joined.ToString( target.Name),
                        CustomColors.Accepted);
                }
            }
            else
            {
                PacketSender.SendPlayerMsg(MyClient, Strings.Parties.limitreached, CustomColors.Error);
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
                    PacketSender.SendPlayerMsg(oldMember.MyClient, Strings.Parties.kicked,
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
                                Strings.Parties.memberkicked.ToString( oldMember.Name), CustomColors.Error);
                        }
                    }
                    else if (Party.Count > 0) //Check if anyone is left on their own
                    {
                        var remainder = Party[0];
                        remainder.Party.Clear();
                        PacketSender.SendParty(remainder.MyClient);
                        PacketSender.SendPlayerMsg(remainder.MyClient, Strings.Parties.disbanded,
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
                            Strings.Parties.memberleft.ToString( oldMember.Name), CustomColors.Error);
                    }
                }
                else if (Party.Count > 0) //Check if anyone is left on their own
                {
                    var remainder = Party[0];
                    remainder.Party.Clear();
                    PacketSender.SendParty(remainder.MyClient);
                    PacketSender.SendPlayerMsg(remainder.MyClient, Strings.Parties.disbanded,
                        CustomColors.Error);
                }
            }
            Party.Clear();
            PacketSender.SendParty(MyClient);
            PacketSender.SendPlayerMsg(MyClient, Strings.Parties.left, CustomColors.Error);
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
            if (target?.Trading.Counterparty != null) return;
            // Set the status of both players to be in a trade
            Trading.Counterparty = target;
            target.Trading.Counterparty = this;
            Trading.Accepted = false;
            target.Trading.Accepted = false;
            Trading.Offer = new Item[Options.MaxInvItems];
            target.Trading.Offer = new Item[Options.MaxInvItems];

            for (var i = 0; i < Options.MaxInvItems; i++)
            {
                Trading.Offer[i] = new Item();
                target.Trading.Offer[i] = new Item();
            }

            //Send the trade confirmation to both players
            PacketSender.StartTrade(target.MyClient, MyIndex);
            PacketSender.StartTrade(MyClient, target.MyIndex);
        }

        public byte[] PartyData()
        {
            var bf = new ByteBuffer();
            bf.WriteInteger(MyIndex);
            bf.WriteString(Name);
            for (int i = 0; i < (int)Vitals.VitalCount; i++)
            {
                bf.WriteInteger(GetVital(i));
            }
            for (int i = 0; i < (int)Vitals.VitalCount; i++)
            {
                bf.WriteInteger(GetMaxVital(i));
            }
            return bf.ToArray();
        }

        //Spells
        public bool TryTeachSpell(Spell spell, bool sendUpdate = true)
        {
            if (KnowsSpell(spell.SpellId))
            {
                return false;
            }
            if (SpellBase.Lookup.Get<SpellBase>(spell.SpellId) == null) return false;
            for (var i = 0; i < Options.MaxPlayerSkills; i++)
            {
                if (Spells[i].SpellId <= 0)
                {
                    Spells[i].Set(spell);
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
                if (Spells[i].SpellId == spellnum)
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
                if (Spells[i].SpellId == spellNum)
                {
                    return i;
                }
            }
            return -1;
        }

        public void SwapSpells(int spell1, int spell2)
        {
            var tmpInstance = Spells[spell2].Clone();
            Spells[spell2].Set(Spells[spell1]);
            Spells[spell1].Set(tmpInstance);
            PacketSender.SendPlayerSpellUpdate(MyClient, spell1);
            PacketSender.SendPlayerSpellUpdate(MyClient, spell2);
            HotbarProcessSpellSwap(spell1, spell2);
        }

        public void ForgetSpell(int spellSlot)
        {
            Spells[spellSlot].Set(Spell.None);
            PacketSender.SendPlayerSpellUpdate(MyClient, spellSlot);
        }

        public void UseSpell(int spellSlot, EntityInstance target)
        {
            var spellNum = Spells[spellSlot].SpellId;
            Target = target;
            if (SpellBase.Lookup.Get<SpellBase>(spellNum) != null)
            {
                var spell = SpellBase.Lookup.Get<SpellBase>(spellNum);

                if (!EventInstance.MeetsConditionLists(spell.CastingReqs, this, null))
                {
                    PacketSender.SendPlayerMsg(MyClient, Strings.Combat.dynamicreq);
                    return;
                }

                //Check if the caster is silenced or stunned
                var statuses = Statuses.Values.ToArray();
                foreach (var status in statuses)
                {
                    if (status.Type == (int)StatusTypes.Silence)
                    {
                        PacketSender.SendPlayerMsg(MyClient, Strings.Combat.silenced);
                        return;
                    }
                    if (status.Type == (int)StatusTypes.Stun)
                    {
                        PacketSender.SendPlayerMsg(MyClient, Strings.Combat.stunned);
                        return;
                    }
                }

                //Check if the caster has the right ammunition if a projectile
                if (spell.SpellType == (int)SpellTypes.CombatSpell &&
                    spell.TargetType == (int)SpellTargetTypes.Projectile && spell.ProjectileId > -1)
                {
                    var projectileBase = spell.Projectile;
                    if (projectileBase == null) return;
                    if (projectileBase.AmmoItemId > -1)
                    {
                        if (FindItem(projectileBase.AmmoItemId, projectileBase.AmmoRequired) == -1)
                        {
                            PacketSender.SendPlayerMsg(MyClient,
                                Strings.Items.notenough.ToString( ItemBase.GetName(projectileBase.AmmoItemId)),
                                CustomColors.Error);
                            return;
                        }
                    }
                }

                if (target == null &&
                    ((spell.SpellType == (int)SpellTypes.CombatSpell &&
                      spell.TargetType == (int)SpellTargetTypes.Single) || spell.SpellType == (int)SpellTypes.WarpTo))
                {
                    PacketSender.SendActionMsg(this, Strings.Combat.notarget, CustomColors.NoTarget);
                    return;
                }

                //Check for range of a single target spell
                if (spell.SpellType == (int)SpellTypes.CombatSpell &&
                    spell.TargetType == (int)SpellTargetTypes.Single && Target != this)
                {
                    if (!InRangeOf(Target, spell.CastRange))
                    {
                        PacketSender.SendActionMsg(this, Strings.Combat.targetoutsiderange,
                            CustomColors.NoTarget);
                        return;
                    }
                }

                if (spell.VitalCost[(int)Vitals.Mana] <= GetVital(Vitals.Mana))
                {
                    if (spell.VitalCost[(int)Vitals.Health] <= GetVital(Vitals.Health))
                    {
                        if (Spells[spellSlot].SpellCd < Globals.System.GetTimeMs())
                        {
                            if (CastTime < Globals.System.GetTimeMs())
                            {
                                CastTime = Globals.System.GetTimeMs() + spell.CastDuration;
                                SubVital(Vitals.Mana, spell.VitalCost[(int) Vitals.Mana]);
                                SubVital(Vitals.Health, spell.VitalCost[(int) Vitals.Health]);
                                SpellCastSlot = spellSlot;
                                CastTarget = Target;

                                //Check if the caster has the right ammunition if a projectile
                                if (spell.SpellType == (int)SpellTypes.CombatSpell &&
                                    spell.TargetType == (int)SpellTargetTypes.Projectile && spell.ProjectileId > -1)
                                {
                                    var projectileBase = spell.Projectile;
                                    if (projectileBase != null && projectileBase.AmmoItemId > -1)
                                    {
                                        TakeItemsByNum(FindItem(projectileBase.AmmoItemId, projectileBase.AmmoRequired),
                                            projectileBase.AmmoRequired);
                                    }
                                }

                                if (spell.CastAnimationId > -1)
                                {
                                    PacketSender.SendAnimationToProximity(spell.CastAnimationId, 1, MyIndex, MapIndex,
                                        0,
                                        0, Dir); //Target Type 1 will be global entity
                                }

                                PacketSender.SendEntityVitals(this);

                                //Check if cast should be instance
                                if (Globals.System.GetTimeMs() >= CastTime)
                                {
                                    //Cast now!
                                    CastTime = 0;
                                    CastSpell(Spells[SpellCastSlot].SpellId, SpellCastSlot);
                                    CastTarget = null;
                                }
                                else
                                {
                                    //Tell the client we are channeling the spell
                                    PacketSender.SendEntityCastTime(this, spellNum);
                                }
                            }
                            else
                            {
                                PacketSender.SendPlayerMsg(MyClient, Strings.Combat.channeling);
                            }
                        }
                        else
                        {
                            PacketSender.SendPlayerMsg(MyClient, Strings.Combat.cooldown);
                        }
                    }
                    else
                    {
                        PacketSender.SendPlayerMsg(MyClient, Strings.Combat.lowhealth);
                    }
                }
                else
                {
                    PacketSender.SendPlayerMsg(MyClient, Strings.Combat.lowmana);
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

        //HotbarSlot
        public void HotbarChange(int index, int type, int slot)
        {
            Hotbar[index].Type = type;
            Hotbar[index].ItemSlot = slot;
        }

        public void HotbarProcessItemSwap(int item1, int item2)
        {
            for (var i = 0; i < Options.MaxHotbar; i++)
            {
                if (Hotbar[i].Type == 0 && Hotbar[i].Slot == item1)
                    Hotbar[i].ItemSlot = item2;
                else if (Hotbar[i].Type == 0 && Hotbar[i].Slot == item2)
                    Hotbar[i].ItemSlot = item1;
            }
            PacketSender.SendHotbarSlots(MyClient);
        }

        public void HotbarProcessSpellSwap(int spell1, int spell2)
        {
            for (var i = 0; i < Options.MaxHotbar; i++)
            {
                if (Hotbar[i].Type == 1 && Hotbar[i].Slot == spell1)
                    Hotbar[i].ItemSlot = spell2;
                else if (Hotbar[i].Type == 1 && Hotbar[i].Slot == spell2)
                    Hotbar[i].ItemSlot = spell1;
            }
            PacketSender.SendHotbarSlots(MyClient);
        }

        //Quests
        public bool CanStartQuest(QuestBase quest)
        {
            //Check and see if the quest is already in progress, or if it has already been completed and cannot be repeated.
            var questProgress = FindQuest(quest.Index);
            if (questProgress != null)
            {
                if (questProgress.TaskId != -1 && quest.GetTaskIndex(questProgress.TaskId) != -1)
                {
                    return false;
                }
                if (questProgress.Completed == 1 && quest.Repeatable == 0)
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
            var questProgress = FindQuest(quest.Index);
            if (questProgress != null)
            {
                if (questProgress.Completed == 1)
                {
                    return true;
                }
            }
            return false;
        }

        public bool QuestInProgress(QuestBase quest, QuestProgress progress, int taskId)
        {
            var questProgress = FindQuest(quest.Index);
            if (questProgress != null)
            {
                if (questProgress.TaskId != -1 && quest.GetTaskIndex(questProgress.TaskId) != -1)
                {
                    switch (progress)
                    {
                        case QuestProgress.OnAnyTask:
                            return true;
                        case QuestProgress.BeforeTask:
                            if (quest.GetTaskIndex(taskId) != -1)
                            {
                                return quest.GetTaskIndex(taskId) > quest.GetTaskIndex(questProgress.TaskId);
                            }
                            break;
                        case QuestProgress.OnTask:
                            if (quest.GetTaskIndex(taskId) != -1)
                            {
                                return quest.GetTaskIndex(taskId) == quest.GetTaskIndex(questProgress.TaskId);
                            }
                            break;
                        case QuestProgress.AfterTask:
                            if (quest.GetTaskIndex(taskId) != -1)
                            {
                                return quest.GetTaskIndex(taskId) < quest.GetTaskIndex(questProgress.TaskId);
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

        public Quest FindQuest(int questId)
        {
            foreach (var quest in Quests)
            {
                if (quest.QuestId == questId) return quest;
            }
            return null;
        }

        public void StartQuest(QuestBase quest)
        {
            if (CanStartQuest(quest))
            {
                var questProgress = FindQuest(quest.Index);
                if (questProgress != null)
                {
                    questProgress.TaskId = quest.Tasks[0].Id;
                    questProgress.TaskProgress = 0;
                }
                else
                {
                    questProgress = new Quest(quest.Index)
                    {
                        TaskId = quest.Tasks[0].Id,
                        TaskProgress = 0
                    };
                    Quests.Add(questProgress);
                }
                if (quest.Tasks[0].Objective == 1) //Gather Items
                {
                    UpdateGatherItemQuests(quest.Tasks[0].Data1);
                }
                if (quest.StartEvent != null)
                {
                    StartCommonEvent(quest.StartEvent);
                }
                PacketSender.SendPlayerMsg(MyClient, Strings.Quests.started.ToString( quest.Name),
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
                    PacketSender.SendPlayerMsg(MyClient, Strings.Quests.declined.ToString( QuestBase.GetName(questId)),
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
                        var questProgress = FindQuest(quest.Index);
                        questProgress.TaskId = -1;
                        questProgress.TaskProgress = -1;
                        PacketSender.SendPlayerMsg(MyClient,
                            Strings.Quests.abandoned.ToString( QuestBase.GetName(questId)), Color.Red);
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
                var questProgress = FindQuest(questId);
                if (questProgress != null)
                {
                    if (questProgress.TaskId == taskId)
                    {
                        //Let's Advance this task or complete the quest
                        for (var i = 0; i < quest.Tasks.Count; i++)
                        {
                            if (quest.Tasks[i].Id == taskId)
                            {
                                PacketSender.SendPlayerMsg(MyClient, Strings.Quests.taskcompleted);
                                if (i == quest.Tasks.Count - 1)
                                {
                                    //Complete Quest
                                    questProgress.Completed = 1;
                                    questProgress.TaskId = -1;
                                    questProgress.TaskProgress = -1;
                                    if (quest.Tasks[i].CompletionEvent != null)
                                    {
                                        StartCommonEvent(quest.Tasks[i].CompletionEvent);
                                    }
                                    if (quest.EndEvent != null)
                                    {
                                        StartCommonEvent(quest.EndEvent);
                                    }
                                    PacketSender.SendPlayerMsg(MyClient, Strings.Quests.completed.ToString( quest.Name),
                                        Color.Green);
                                }
                                else
                                {
                                    //Advance Task
                                    questProgress.TaskId = quest.Tasks[i + 1].Id;
                                    questProgress.TaskProgress = 0;
                                    if (quest.Tasks[i].CompletionEvent != null)
                                    {
                                        StartCommonEvent(quest.Tasks[i].CompletionEvent);
                                    }
                                    if (quest.Tasks[i + 1].Objective == 1) //Gather Items
                                    {
                                        UpdateGatherItemQuests(quest.Tasks[i + 1].Data1);
                                    }
                                    PacketSender.SendPlayerMsg(MyClient, Strings.Quests.updated.ToString( quest.Name),
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
                foreach (var questProgress in Quests)
                {
                    var questId = questProgress.QuestId;
                    var quest = QuestBase.Lookup.Get<QuestBase>(questId);
                    if (quest != null)
                    {
                        if (questProgress.TaskId > -1)
                        {
                            //Assume this quest is in progress. See if we can find the task in the quest
                            var questTask = quest.FindTask(questProgress.TaskId);
                            if (questTask != null)
                            {
                                if (questTask.Objective == 1 && questTask.Data1 == item.Index) //gather items
                                {
                                    if (questProgress.TaskProgress != CountItems(item.Index))
                                    {
                                        questProgress.TaskProgress = CountItems(item.Index);
                                        if (questProgress.TaskProgress >= questTask.Data2)
                                        {
                                            CompleteQuestTask(questId, questProgress.TaskId);
                                        }
                                        else
                                        {
                                            PacketSender.SendQuestProgress(this, quest.Index);
                                            PacketSender.SendPlayerMsg(MyClient,
                                                Strings.Quests.itemtask.ToString( quest.Name, questProgress.TaskProgress,
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

        //Switches and Variables
        private Switch GetSwitch(int id)
        {
            foreach (var s in Switches)
            {
                if (s.SwitchId == id) return s;
            }
            return null;
        }
        public bool GetSwitchValue(int id)
        {
            var s = GetSwitch(id);
            if (s == null) return false;
            return s.Value;
        }
        public void SetSwitchValue(int id, bool value)
        {
            var s = GetSwitch(id);
            if (s != null)
            {
                s.Value = value;
            }
            else
            {
                s = new Switch(id)
                {
                    Value = value
                };
                Switches.Add(s);
            }
        }
        private Variable GetVariable(int id)
        {
            foreach (var v in Variables)
            {
                if (v.VariableId == id) return v;
            }
            return null;
        }
        public int GetVariableValue(int id)
        {
            var v = GetVariable(id);
            if (v == null) return 0;
            return v.Value;
        }
        public void SetVariableValue(int id, int value)
        {
            var v = GetVariable(id);
            if (v != null)
            {
                v.Value = value;
            }
            else
            {
                v = new Variable(id)
                {
                    Value = value
                };
                Variables.Add(v);
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
                    if (evt.PageInstance.MapIndex == map && evt.PageInstance.X == x &&
                        evt.PageInstance.Y == y && evt.PageInstance.Z == z)
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
                if (evt.MapNum == mapNum && evt.BaseEvent.MapIndex == eventIndex)
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
                    if (evt.MapNum == mapNum && evt.BaseEvent.MapIndex == eventIndex)
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
                        tmpEvent.PageInstance = new EventPageInstance(baseEvent, baseEvent.MyPages[i], baseEvent.MapIndex, -1 * commonEventLaunch, tmpEvent, MyClient);
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
            var oldMap = MapIndex;
            client = MyClient;
            base.Move(moveDir, client, dontUpdate, correction);
            // Check for a warp, if so warp the player.
            var attribute =
                MapInstance.Lookup.Get<MapInstance>(Globals.Entities[index].MapIndex).Attributes[
                    Globals.Entities[index].X, Globals.Entities[index].Y];
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
                if (evt.MapNum == MapIndex)
                {
                    if (evt.PageInstance != null)
                    {
                        if (evt.PageInstance.MapIndex == MapIndex &&
                            evt.PageInstance.X == X &&
                            evt.PageInstance.Y == Y &&
                            evt.PageInstance.Z == Z)
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

    public struct Trading : IDisposable
    {
        [NotNull]
        private readonly Player mPlayer;

        public bool Actively => Counterparty != null;

        [CanBeNull]
        public Player Counterparty;

        public bool Accepted;

        [NotNull]
        public Item[] Offer;

        public Player Requester;

        [NotNull]
        public Dictionary<Player, long> Requests;

        public Trading([NotNull] Player player)
        {
            mPlayer = player;

            Accepted = false;
            Counterparty = null;
            Offer = new Item[Options.MaxInvItems];
            Requester = null;
            Requests = new Dictionary<Player, long>();
        }

        public void Dispose()
        {
            Offer = new Item[0];
            Requester = null;
            Requests.Clear();
        }
    }
}
