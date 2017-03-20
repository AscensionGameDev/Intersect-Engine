
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intersect_Library;
using Intersect_Library.GameObjects;
using Intersect_Library.GameObjects.Events;
using Intersect_Library.Localization;
using Intersect_Server.Classes.Core;
using Intersect_Server.Classes.General;
using Intersect_Server.Classes.Items;
using Intersect_Server.Classes.Maps;
using Intersect_Server.Classes.Networking;
using Intersect_Server.Classes.Spells;
namespace Intersect_Server.Classes.Entities
{

    public class Player : Entity
    {
        //5 minute timeout before someone can send a trade/party request after it has been declined
        public const long RequestDeclineTimeout = 300000;

        public long MyId = -1;
        public bool InGame;
        public Client MyClient;
        private bool _sentMap;
        public int StatPoints;
        public int Class = 0;
        public int Gender = 0;
        public int Level = 1;
        public int Experience;
        public int[] Equipment = new int[Options.EquipmentSlots.Count];
        public Dictionary<int, bool> Switches = new Dictionary<int, bool>();
        public Dictionary<int, int> Variables = new Dictionary<int, int>();
        public Dictionary<int, QuestProgressStruct> Quests = new Dictionary<int, QuestProgressStruct>();
        public List<EventInstance> MyEvents = new List<EventInstance>();
        public HotbarInstance[] Hotbar = new HotbarInstance[Options.MaxHotbar];
        public ItemInstance[] Bank = new ItemInstance[Options.MaxBankSlots];
        public List<int> QuestOffers = new List<int>();

        //Temporary Values
        private object EventLock = new object();
        //Event Spawned Npcs
        public List<Npc> SpawnedNpcs = new List<Npc>();
        public bool InBank;
        public int InShop = -1;
        public int InCraft = -1;
        public int InBag = -1;
        public int CraftIndex = -1;
        public long CraftTimer = 0;
        public long SaveTimer = Environment.TickCount;
        public List<Player> Party = new List<Player>();
        public int Trading = -1;
        public ItemInstance[] Trade = new ItemInstance[Options.MaxInvItems];
        public bool TradeAccepted = false;
        public Player TradeRequester = null;
        public Player PartyRequester = null;
        public Dictionary<Player, long> TradeRequests = new Dictionary<Player, long>();
        public Dictionary<Player, long> PartyRequests = new Dictionary<Player, long>();
        public int LastMapEntered = -1;

        public bool IsValidPlayer
        {
            get
            {
                return (MyClient != null && MyClient.Entity == this);
            }
        }

        //Init
        public Player(int index, Client newClient) : base(index)
        {
            MyClient = newClient;
            for (int i = 0; i < Options.MaxPlayerSkills; i++)
            {
                Spells.Add(new SpellInstance());
            }
            for (int i = 0; i < Options.MaxInvItems; i++)
            {
                Inventory.Add(new ItemInstance(-1, 0, -1));
            }
            for (int i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                Equipment[i] = -1;
            }
            for (int i = 0; i < Options.MaxHotbar; i++)
            {
                Hotbar[i] = new HotbarInstance();
            }
            for (int I = 0; I < (int)Stats.StatCount; I++)
            {
                Stat[I] = new EntityStat(0, I, this);
            }
        }

        //Update
        public override void Update()
        {
            if (!InGame || CurrentMap == -1) { return; }

            if (SaveTimer + 120000 < Environment.TickCount)
            {
                Task.Run(() => Database.SaveCharacter(this, false));
                SaveTimer = Environment.TickCount;
            }

            if (InCraft > -1 && CraftIndex > -1)
            {
                BenchBase b = BenchBase.GetCraft(InCraft);
                if (CraftTimer + b.Crafts[CraftIndex].Time < Environment.TickCount)
                {
                    CraftItem(CraftIndex);
                }
            }

            base.Update();

            //If we have a move route then let's process it....
            if (MoveRoute != null && MoveTimer < Globals.System.GetTimeMs())
            {
                //Check to see if the event instance is still active for us... if not then let's remove this route
                for (var i = 0; i < MyEvents.Count; i++)
                {
                    var evt = MyEvents[i];
                    if (MyEvents[i] != null && MyEvents[i].PageInstance == MoveRouteSetter)
                    {
                        if (MoveRoute.ActionIndex < MoveRoute.Actions.Count)
                        {
                            ProcessMoveRoute(MyClient);
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
                    else if (i == MyEvents.Count -1)
                    {
                        MoveRoute = null;
                        MoveRouteSetter = null;
                        PacketSender.SendMoveRouteToggle(MyClient, false);
                    }
                }
            }

            //If we switched maps, lets update the maps
            if (LastMapEntered != CurrentMap)
            {
                if (MapInstance.GetMap(LastMapEntered) != null)
                {
                    MapInstance.GetMap(LastMapEntered).RemoveEntity(this);
                }
                if (CurrentMap > -1)
                {
                    if (!MapInstance.GetObjects().ContainsKey(CurrentMap))
                    {
                        WarpToSpawn();
                    }
                    else
                    {
                        MapInstance.GetMap(CurrentMap).PlayerEnteredMap(this);
                    }
                }
            }

            var currentMap = MapInstance.GetMap(CurrentMap);
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
                        map = MapInstance.GetMap(currentMap.SurroundingMaps[i]);
                    }
                    if (map == null) continue;
                    lock (map.GetMapLock())
                    {
                        //Check to see if we can spawn events, if already spawned.. update them.
                        lock (EventLock)
                        {
                            foreach (var mapEvent in map.Events.Values)
                            {
                                //Look for event
                                var foundEvent = EventExists(map.MyMapNum, mapEvent.SpawnX, mapEvent.SpawnY);
                                if (foundEvent == -1)
                                {
                                    var tmpEvent = new EventInstance(MyEvents.Count, MyClient, mapEvent, map.MyMapNum)
                                    {
                                        IsGlobal = mapEvent.IsGlobal == 1,
                                        MapNum = map.MyMapNum,
                                        SpawnX = mapEvent.SpawnX,
                                        SpawnY = mapEvent.SpawnY
                                    };
                                    MyEvents.Add(tmpEvent);
                                }
                                else
                                {
                                    MyEvents[foundEvent].Update();
                                }
                            }
                        }
                    }
                }
            }
            //Check to see if we can spawn events, if already spawned.. update them.
            lock (EventLock)
            {
                for (var i = 0; i < MyEvents.Count; i++)
                {
                    var evt = MyEvents[i];
                    if (MyEvents[i] == null) continue;
                    var eventFound = false;
                    if (evt.MapNum == -1)
                    {
                        evt.Update();
                        if (evt.CallStack.Count > 0)
                        {
                            eventFound = true;
                        }
                    }
                    if (evt.MapNum != CurrentMap)
                    {
                        foreach (var t in MapInstance.GetMap(CurrentMap).SurroundingMaps)
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
                    PacketSender.SendEntityLeaveTo(MyClient, i, (int)EntityTypes.Event, evt.MapNum);
                    MyEvents[i] = null;
                }
            }
        }

        //Sending Data
        public override byte[] Data()
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteBytes(base.Data());
            bf.WriteInteger(Level);
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
            var cls = ClassBase.GetClass(Class);
            if (cls != null)
            {
                Warp(cls.SpawnMap, cls.SpawnX, cls.SpawnY, cls.SpawnDir);
            }
            else
            {
                Warp(0, 0, 0, 0);
            }
            PacketSender.SendEntityDataToProximity(this);
        }
        public override void Die(bool dropitems = false, Entity killer = null)
        {
            //Flag death to the client
            PacketSender.SendPlayerDeath(this);

            //Event trigger
            for (var i = 0; i < MyEvents.Count; i++)
            {
                if (MyEvents[i] != null)
                {
                    MyEvents[i].PlayerHasDied = true;
                }
            }

            base.Die(dropitems, killer);
            Reset();
            Respawn();
        }

        //Vitals
        public void RestoreVital(Vitals vital)
        {
            Vital[(int)vital] = MaxVital[(int)vital];
            PacketSender.SendEntityVitals(this);
        }
        public void AddVital(Vitals vital, int amount)
        {
            Vital[(int)vital] += amount;
            if (Vital[(int)vital] < 0) Vital[(int)vital] = 0;
            if (Vital[(int)vital] > MaxVital[(int)vital]) Vital[(int)vital] = MaxVital[(int)vital];
            PacketSender.SendEntityVitals(this);
        }
        public override void ProcessRegen()
        {
            var myclass = ClassBase.GetClass(Class);
            var vitalAdded = false;
            if (myclass != null)
            {
                foreach (Vitals vital in Enum.GetValues(typeof(Vitals)))
                {
                    if ((int)vital < (int)Vitals.VitalCount && Vital[(int)vital] != MaxVital[(int)vital])
                    {
                        AddVital(vital, (int)((float)MaxVital[(int)vital] * (myclass.VitalRegen[(int)vital] / 100f)));
                        vitalAdded = true;
                    }
                }
            }
            if (vitalAdded)
            {
                PacketSender.SendEntityVitals(this);
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
            if (Level < Options.MaxLevel)
            {
                for (int i = 0; i < levels; i++)
                {
                    SetLevel(Level + 1, resetExperience);
                    //Let's pull up class - leveling info
                    var myclass = ClassBase.GetClass(Class);
                    if (myclass != null)
                    {
                        foreach (Vitals vital in Enum.GetValues(typeof(Vitals)))
                        {
                            if ((int)vital < (int)Vitals.VitalCount)
                            {
                                var maxVital = MaxVital[(int)vital];
                                if (myclass.IncreasePercentage == 1)
                                {
                                    maxVital = (int)(MaxVital[(int)vital] * (1f + ((float)myclass.VitalIncrease[(int)vital] / 100f)));
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
                                    newStat = (int)(Stat[(int)stat].Stat * (1f + ((float)myclass.StatIncrease[(int)stat] / 100f)));
                                }
                                else
                                {
                                    newStat = Stat[(int)stat].Stat + myclass.StatIncrease[(int)stat];
                                }
                                var statDiff = newStat - Stat[(int)stat].Stat;
                                AddStat(stat, statDiff);
                            }
                        }
                    }
                    StatPoints += myclass.PointIncrease;
                }
            }

            PacketSender.SendPlayerMsg(MyClient, Strings.Get("player", "levelup", Level), Color.Cyan, MyName);
            PacketSender.SendActionMsg(this, Strings.Get("combat", "levelup"), new Color(255, 0, 255, 0));
            if (StatPoints > 0)
            {
                PacketSender.SendPlayerMsg(MyClient, Strings.Get("player", "statpoints", StatPoints), Color.Cyan, MyName);
            }
            PacketSender.SendExperience(MyClient);
            PacketSender.SendPointsTo(MyClient);
            PacketSender.SendEntityDataToProximity(this);

            //Search for login activated events and run them
            foreach (var evt in EventBase.GetObjects())
            {
                StartCommonEvent(evt.Value, (int)EventPage.CommonEventTriggers.LevelUp);
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
            int levelCount = 0;
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
            var myclass = ClassBase.GetClass(Class);
            if (myclass != null)
            {
                return (int)(myclass.BaseExp * Math.Pow(1 + (myclass.ExpIncrease / 100f) / 1, Level));
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
                    for (int i = 0; i < Party.Count; i++)
                    {
                        Party[i].GiveExperience(((Npc)en).MyBase.Experience / Party.Count);
                    }
                }
                else
                {
                    GiveExperience(((Npc)en).MyBase.Experience);
                }

                //If any quests demand that this Npc be killed then let's handle it
                var npc = (Npc)en;
                for (int i = 0; i < Quests.Keys.Count; i++)
                {
                    var questId = Quests.Keys.ToArray()[i];
                    var quest = QuestBase.GetQuest(questId);
                    if (quest != null)
                    {
                        if (Quests[questId].task > -1)
                        {
                            //Assume this quest is in progress. See if we can find the task in the quest
                            var questTask = quest.FindTask(Quests[questId].task);
                            if (questTask != null)
                            {
                                if (questTask.Objective == 2 && questTask.Data1 == npc.MyBase.GetId()) //kill npcs
                                {
                                    var questProg = Quests[questId];
                                    questProg.taskProgress++;
                                    if (questProg.taskProgress >= questTask.Data2)
                                    {
                                        CompleteQuestTask(questId, Quests[questId].task);
                                    }
                                    else
                                    {
                                        Quests[questId] = questProg;
                                        PacketSender.SendQuestProgress(this, quest.GetId());
                                        PacketSender.SendPlayerMsg(MyClient, Strings.Get("quests", "npctask", quest.Name, questProg.taskProgress, questTask.Data2, NpcBase.GetName(questTask.Data1)));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public override void TryAttack(Entity enemy)
        {
            if (CastTime >= Globals.System.GetTimeMs())
            {
                PacketSender.SendPlayerMsg(MyClient, Strings.Get("combat", "channelingnoattack"));
                return;
            }

            if (!IsOneBlockAway(enemy)) return;
            if (!isFacingTarget(enemy)) return;

            ItemBase weapon = null;
            if (((Player)Globals.Entities[MyIndex]).Equipment[Options.WeaponIndex] >= 0)
            {
                weapon = ItemBase.GetItem(Inventory[((Player)Globals.Entities[MyIndex]).Equipment[Options.WeaponIndex]].ItemNum);
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
                        PacketSender.SendPlayerMsg(MyClient, Strings.Get("combat", "toolrequired", Options.ToolTypes[resource.Tool]));
                        return;
                    }
                }
            }

            var deadAnimations = new List<KeyValuePair<int, int>>();
            var aliveAnimations = new List<KeyValuePair<int, int>>();
            if (weapon != null)
            {
                var attackAnim = AnimationBase.GetAnim(weapon.AttackAnimation);
                if (attackAnim != null)
                {
                    deadAnimations.Add(new KeyValuePair<int, int>(weapon.AttackAnimation, Dir));
                    aliveAnimations.Add(new KeyValuePair<int, int>(weapon.AttackAnimation, Dir));
                }
                base.TryAttack(enemy, weapon.Damage == 0 ? 1 : weapon.Damage, (DamageType)weapon.DamageType, (Stats)weapon.ScalingStat,
                    weapon.Scaling, weapon.CritChance, Options.CritMultiplier, deadAnimations, aliveAnimations);
            }
            else
            {
                var classBase = ClassBase.GetClass(Class);
                if (classBase != null)
                {
                    var attackAnim = AnimationBase.GetAnim(classBase.AttackAnimation);
                    if (attackAnim != null)
                    {
                        deadAnimations.Add(new KeyValuePair<int, int>(classBase.AttackAnimation, Dir));
                        aliveAnimations.Add(new KeyValuePair<int, int>(classBase.AttackAnimation, Dir));
                    }
                    base.TryAttack(enemy, classBase.Damage == 0 ? 1 : classBase.Damage, (DamageType)classBase.DamageType, (Stats)classBase.ScalingStat,
                    classBase.Scaling, classBase.CritChance, Options.CritMultiplier, deadAnimations, aliveAnimations);
                }
                else
                {
                    base.TryAttack(enemy, 1, (DamageType)DamageType.Physical, Stats.Attack,
                    100, 10, Options.CritMultiplier, deadAnimations, aliveAnimations);
                }
            }
            PacketSender.SendEntityAttack(this, (int)EntityTypes.GlobalEntity, CurrentMap, CalculateAttackTime());
        }
        public override bool CanAttack(Entity en, SpellBase spell)
        {
            //Check if the attacker is stunned or blinded.
            for (var n = 0; n < Status.Count; n++)
            {
                if (Status[n].Type == (int)StatusTypes.Stun)
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
        public override void Warp(int newMap, int newX, int newY)
        {
            Warp(newMap, newX, newY, 1);
        }
        public override void Warp(int newMap, int newX, int newY, int newDir)
        {
            var map = MapInstance.GetMap(newMap);
            if (map == null)
            {
                WarpToSpawn();
                return;
            }
            CurrentX = newX;
            CurrentY = newY;
            for (int i = 0; i < MyEvents.Count; i++)
            {
                if (MyEvents[i] != null && MyEvents[i].MapNum != -1 && MyEvents[i].MapNum != newMap)
                {
                    MyEvents[i] = null;
                }
            }
            if (newMap != CurrentMap || _sentMap == false)
            {
                var oldMap = MapInstance.GetMap(CurrentMap);
                if (oldMap != null)
                {
                    oldMap.RemoveEntity(this);
                }
                PacketSender.SendEntityLeave(MyIndex, (int)EntityTypes.Player, CurrentMap);
                CurrentMap = newMap;
                map.PlayerEnteredMap(this);
                PacketSender.SendEntityDataToProximity(this);
                PacketSender.SendEntityPositionToAll(this);
                PacketSender.SendMapGrid(MyClient, map.MapGrid);
                var surroundingMaps = map.GetSurroundingMaps(true);
                foreach (var surrMap in surroundingMaps)
                {
                    PacketSender.SendMap(MyClient, surrMap.MyMapNum);
                }
                _sentMap = true;
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
            int map = -1, x = 0, y = 0;
            var cls = ClassBase.GetClass(Class);
            if (cls != null)
            {
                if (MapInstance.GetObjects().ContainsKey(cls.SpawnMap))
                {
                    map = cls.SpawnMap;
                }
                x = cls.SpawnX;
                y = cls.SpawnY;
            }
            if (map == -1)
            {
                var mapenum = MapInstance.GetObjects().GetEnumerator();
                mapenum.MoveNext();
                map = mapenum.Current.Value.MyMapNum;
            }
            Warp(map, x, y);
        }

        //Inventory
        public bool CanGiveItem(ItemInstance item)
        {
            var itemBase = ItemBase.GetItem(item.ItemNum);
            if (itemBase != null)
            {
                if (itemBase.IsStackable())
                {
                    for (int i = 0; i < Options.MaxInvItems; i++)
                    {
                        if (Inventory[i].ItemNum == item.ItemNum)
                        {
                            return true;
                        }
                    }
                }

                //Either a non stacking item, or we couldn't find the item already existing in the players inventory
                for (int i = 0; i < Options.MaxInvItems; i++)
                {
                    if (Inventory[i].ItemNum == -1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool TryGiveItem(ItemInstance item, bool SendUpdate = true)
        {
            var itemBase = ItemBase.GetItem(item.ItemNum);
            if (itemBase != null)
            {
                if (itemBase.IsStackable())
                {
                    for (int i = 0; i < Options.MaxInvItems; i++)
                    {
                        if (Inventory[i].ItemNum == item.ItemNum)
                        {
                            Inventory[i].ItemVal += item.ItemVal;
                            if (SendUpdate)
                            {
                                PacketSender.SendInventoryItemUpdate(MyClient, i);
                            }
                            UpdateGatherItemQuests(item.ItemNum);
                            return true;
                        }
                    }
                }

                //Either a non stacking item, or we couldn't find the item already existing in the players inventory
                for (int i = 0; i < Options.MaxInvItems; i++)
                {
                    if (Inventory[i].ItemNum == -1)
                    {
                        Inventory[i] = item.Clone();
                        if (SendUpdate)
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
            ItemInstance tmpInstance = Inventory[item2].Clone();
            Inventory[item2] = Inventory[item1].Clone();
            Inventory[item1] = tmpInstance.Clone();
            PacketSender.SendInventoryItemUpdate(MyClient, item1);
            PacketSender.SendInventoryItemUpdate(MyClient, item2);
            EquipmentProcessItemSwap(item1, item2);
            HotbarProcessItemSwap(item1, item2);
        }
        public void DropItems(int slot, int amount)
        {
            var itemBase = ItemBase.GetItem(Inventory[slot].ItemNum);
            if (itemBase != null)
            {
                if (itemBase.Bound > 0)
                {
                    PacketSender.SendPlayerMsg(MyClient, Strings.Get("items", "bound"), Color.Red);
                    return;
                }
                if (itemBase.IsStackable())
                {
                    if (amount >= Inventory[slot].ItemVal)
                    {
                        amount = Inventory[slot].ItemVal;
                    }
                    MapInstance.GetMap(CurrentMap).SpawnItem(CurrentX, CurrentY, Inventory[slot], amount);
                    if (amount == Inventory[slot].ItemVal)
                    {
                        Inventory[slot] = new ItemInstance(-1, 0, -1);
                        EquipmentProcessItemLoss(slot);
                    }
                    else
                    {
                        Inventory[slot].ItemVal -= amount;
                    }
                }
                else
                {
                    MapInstance.GetMap(CurrentMap).SpawnItem(CurrentX, CurrentY, Inventory[slot], 1);
                    Inventory[slot] = new ItemInstance(-1, 0, -1);
                    EquipmentProcessItemLoss(slot);
                }
                UpdateGatherItemQuests(itemBase.GetId());
                PacketSender.SendInventoryItemUpdate(MyClient, slot);
            }
        }
        public void UseItem(int slot)
        {
            bool equipped = false;
            var itemInstance = Inventory[slot];
            var itemBase = ItemBase.GetItem(itemInstance.ItemNum);
            if (itemBase != null)
            {
                //Check if the user is silenced or stunned
                for (var n = 0; n < Status.Count; n++)
                {
                    if (Status[n].Type == (int)StatusTypes.Stun)
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
                        break;
                    case (int)ItemTypes.Consumable:
                        string s = "";
                        if (itemBase.Data2 > 0) { s = Strings.Get("combat", "addsymbol"); }

                        switch (itemBase.Data1)
                        {
                            case 0: //Health
                                AddVital(Vitals.Health, itemBase.Data2);
                                if (s == Strings.Get("combat", "addsymbol"))
                                {
                                    PacketSender.SendActionMsg(this, s + itemBase.Data2.ToString(), Color.Green);
                                }
                                else
                                {
                                    PacketSender.SendActionMsg(this, s + itemBase.Data2.ToString(), Color.Red);
                                    if (Vital[(int)Vitals.Health] <= 0) //Add a death handler for poison.
                                    {
                                        Die();
                                    }
                                }
                                break;
                            case 1: //Mana
                                AddVital(Vitals.Mana, itemBase.Data2);
                                PacketSender.SendActionMsg(this, s + itemBase.Data2.ToString(), Color.Blue);
                                break;
                            case 2: //Exp
                                GiveExperience(itemBase.Data2);
                                PacketSender.SendActionMsg(this, s + itemBase.Data2.ToString(), Color.White);
                                break;
                            default:
                                break;
                        }
                        TakeItem(slot, 1);
                        break;
                    case (int)ItemTypes.Equipment:
                        for (int i = 0; i < Options.EquipmentSlots.Count; i++)
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
                                        if (ItemBase.GetItem(Inventory[Equipment[Options.WeaponIndex]].ItemNum) != null &&
                                            Convert.ToBoolean(ItemBase.GetItem(Inventory[Equipment[Options.WeaponIndex]].ItemNum).Data4))
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
                        break;
                    case (int)ItemTypes.Spell:
                        if (itemBase.Data1 > -1)
                        {
                            if (TryTeachSpell(new SpellInstance(itemBase.Data1)))
                            {
                                TakeItem(slot, 1);
                            }
                        }
                        break;
                    case (int)ItemTypes.Event:
                        var evt = EventBase.GetEvent(itemBase.Data1);
                        if (evt != null)
                        {
                            if (StartCommonEvent(evt))
                            {
                                TakeItem(slot, 1);
                            }
                        }
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
                        break;
                }
            }

        }
        public bool TakeItem(int slot, int amount)
        {
            bool returnVal = false;
            if (slot < 0) { return false; }
            var itemBase = ItemBase.GetItem(Inventory[slot].ItemNum);
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
                UpdateGatherItemQuests(itemBase.GetId());
            }
            return returnVal;
        }
        public int FindItem(int itemNum, int itemVal = 1)
        {
            for (int i = 0; i < Options.MaxInvItems; i++)
            {
                if (Inventory[i].ItemNum == itemNum && Inventory[i].ItemVal >= itemVal)
                {
                    return i;
                }
            }
            return -1;
        }
        public int CountItemInstances(int itemNum)
        {
            int count = 0;
            for (int i = 0; i < Options.MaxInvItems; i++)
            {
                if (Inventory[i].ItemNum == itemNum)
                {
                    if (Inventory[i].ItemVal < 1)
                    {
                        count += 1;
                    }
                    else
                    {
                        count += Inventory[i].ItemVal;
                    }
                }
            }
            return count;
        }
        public override int GetWeaponDamage()
        {
            if (Equipment[Options.WeaponIndex] > -1 && Equipment[Options.WeaponIndex] < Options.MaxInvItems)
            {
                if (Inventory[Equipment[Options.WeaponIndex]].ItemNum > -1)
                {
                    var item = ItemBase.GetItem(Inventory[Equipment[Options.WeaponIndex]].ItemNum);
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
            bool canSellItem = true;
            int rewardItemNum = -1;
            int rewardItemVal = 0;
            int sellItemNum = Inventory[slot].ItemNum;
            if (InShop == -1) return;
            ShopBase shop = ShopBase.GetShop(InShop);
            if (shop != null)
            {
                var itemBase = ItemBase.GetItem(Inventory[slot].ItemNum);
                if (itemBase != null)
                {
                    if (itemBase.Bound > 0)
                    {
                        PacketSender.SendPlayerMsg(MyClient, Strings.Get("shops", "bound"), Color.Red);
                        return;
                    }

                    //Check if this is a bag with items.. if so don't allow sale
                    if (itemBase.ItemType == (int) ItemTypes.Bag)
                    {
                        if (Inventory[slot].BagId > -1)
                        {
                            if (!Database.BagEmpty(Inventory[slot].BagId))
                            {
                                PacketSender.SendPlayerMsg(MyClient, Strings.Get("bags", "onlysellempty"), Color.Red);
                                return;
                            }
                        }
                    }

                    for (int i = 0; i < shop.BuyingItems.Count; i++)
                    {
                        if (shop.BuyingItems[i].ItemNum == sellItemNum)
                        {
                            if (!shop.BuyingWhitelist)
                            {
                                PacketSender.SendPlayerMsg(MyClient, Strings.Get("shops", "doesnotaccept"), Color.Red);
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
                            PacketSender.SendPlayerMsg(MyClient, Strings.Get("shops", "doesnotaccept"), Color.Red);
                            return;
                        }
                        else
                        {
                            rewardItemNum = shop.DefaultCurrency;
                            rewardItemVal = itemBase.Price;
                        }
                    }
                    
                     if (itemBase.IsStackable())
                    {
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
                            if (!CanGiveItem(new ItemInstance(rewardItemNum, rewardItemVal, -1))) canSellItem = false;
                            Inventory[slot].ItemVal -= amount;

                        }
                    }
                    else
                    {
                        Inventory[slot] = new ItemInstance(-1, 0, -1);
                        EquipmentProcessItemLoss(slot);
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
            bool canSellItem = true;
            int buyItemNum = -1;
            int buyItemAmt = 1;
            if (InShop == -1) return;
            ShopBase shop = ShopBase.GetShop(InShop);
            if (shop != null)
            {
                if (slot >= 0 && slot < shop.SellingItems.Count)
                {
                    var itemBase = ItemBase.GetItem(shop.SellingItems[slot].ItemNum);
                    if (itemBase != null)
                    {
                        buyItemNum = shop.SellingItems[slot].ItemNum;
                        if (itemBase.IsStackable())
                        {
                            buyItemAmt = Math.Max(1, amount);
                        }
                        if (shop.SellingItems[slot].CostItemVal == 0 || FindItem(shop.SellingItems[slot].CostItemNum, shop.SellingItems[slot].CostItemVal * buyItemAmt) > -1)
                        {
                            if (CanGiveItem(new ItemInstance(buyItemNum, buyItemAmt, -1)))
                            {
                                if (shop.SellingItems[slot].CostItemVal > 0)
                                {
                                    TakeItem(
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
                                    TakeItem(
                                        FindItem(shop.SellingItems[slot].CostItemNum,
                                            shop.SellingItems[slot].CostItemVal * buyItemAmt),
                                        shop.SellingItems[slot].CostItemVal * buyItemAmt);
                                    TryGiveItem(new ItemInstance(buyItemNum, buyItemAmt, -1), true);
                                }
                                else
                                {
                                    PacketSender.SendPlayerMsg(MyClient, Strings.Get("shops", "inventoryfull"),
                                        Color.Red, MyName);
                                }
                            }
                        }
                        else
                        {
                            PacketSender.SendPlayerMsg(MyClient, Strings.Get("shops", "cantafford"),
                                Color.Red, MyName);
                        }
                    }
                }
            }
        }

        //Crafting
        public bool OpenCraftingBench(int index)
        {
            if (IsBusy()) return false;
            InCraft = index;
            PacketSender.SendOpenCraftingBench(MyClient, index);
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
                //Check the player actually has the items
                foreach (CraftIngredient c in BenchBase.GetCraft(InCraft).Crafts[index].Ingredients)
                {
                    int n = FindItem(c.Item);
                    int x = 0;
                    if (n > -1)
                    {
                        x = Inventory[n].ItemVal;
                        if (x == 0) { x = 1; }
                    }
                    if (x < c.Quantity)
                    {
                        return;
                    }
                }

                //Take the items
                foreach (CraftIngredient c in BenchBase.GetCraft(InCraft).Crafts[index].Ingredients)
                {
                    int n = FindItem(c.Item);
                    if (n > -1)
                    {
                        TakeItem(n, c.Quantity);
                    }
                }

                //Give them the craft
                if (TryGiveItem(new ItemInstance(BenchBase.GetCraft(InCraft).Crafts[index].Item, 1, -1)))
                {
                    PacketSender.SendPlayerMsg(MyClient, Strings.Get("crafting", "crafted", ItemBase.GetName(BenchBase.GetCraft(InCraft).Crafts[index].Item)), Color.Green);
                }
                else
                {
                    Inventory = invbackup;
                    PacketSender.SendInventory(MyClient);
                    PacketSender.SendPlayerMsg(MyClient,
                        "You do not have enough inventory space to craft " + ItemBase.GetName(BenchBase.GetCraft(InCraft).Crafts[index].Item) +
                        "!", Color.Red);
                }
                CraftIndex = -1;
            }
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
            var itemBase = ItemBase.GetItem(Inventory[slot].ItemNum);
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
                        for (int i = 0; i < Options.MaxBankSlots; i++)
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
                    for (int i = 0; i < Options.MaxBankSlots; i++)
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
                    PacketSender.SendPlayerMsg(MyClient, Strings.Get("banks", "banknospace"), Color.Red);
                }
                else
                {
                    PacketSender.SendPlayerMsg(MyClient, Strings.Get("banks", "depositinvalid"), Color.Red);
                }
            }
        }
        public void WithdrawItem(int slot, int amount)
        {
            if (!InBank) return;
            var itemBase = ItemBase.GetItem(Bank[slot].ItemNum);
            var inventorySlot = -1;
            if (itemBase != null)
            {
                if (Bank[slot] != null && Bank[slot].ItemNum > -1)
                {
                    if (itemBase.IsStackable())
                    {
                        if (amount >= Bank[slot].ItemVal)
                        {
                            amount = Bank[slot].ItemVal;
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
                        for (int i = 0; i < Options.MaxInvItems; i++)
                        {
                            if (Inventory[i] != null && Inventory[i].ItemNum == Bank[slot].ItemNum)
                            {
                                inventorySlot = i;
                                break;
                            }
                        }
                    }

                    if (inventorySlot < 0)
                    {
                        /* Find a free slot if we don't have one already */
                        for (int j = 0; j < Options.MaxInvItems; j++)
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
                        PacketSender.SendPlayerMsg(MyClient, Strings.Get("banks", "inventorynospace"), Color.Red);
                        return; //Panda forgot this :P
                    }

                    /* Move the items to the inventory */
                    amount = Math.Min(amount, int.MaxValue - Inventory[inventorySlot].ItemVal);

                    if (Inventory[inventorySlot] == null || Inventory[inventorySlot].ItemNum == -1 || Inventory[inventorySlot].ItemVal < 0)
                    {
                        Inventory[inventorySlot] = new ItemInstance(Bank[slot].ItemNum, 0, -1);
                    }

                    Inventory[inventorySlot].ItemVal += amount;
                    if (amount >= Bank[slot].ItemVal)
                    {
                        Bank[slot] = null;
                    }
                    else
                    {
                        Bank[slot].ItemVal -= amount;
                    }

                    PacketSender.SendInventoryItemUpdate(MyClient, inventorySlot);
                    PacketSender.SendBankUpdate(MyClient, slot);
                }
                else
                {
                    PacketSender.SendPlayerMsg(MyClient, Strings.Get("banks", "withdrawinvalid"), Color.Red);
                }
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
            PacketSender.SendOpenBag(MyClient, bagItem.BagInstance.Slots,bagItem.BagInstance);
            return true;
        }
        public bool HasBag(int bagId)
        {
            for (int i = 0; i < Inventory.Count; i++)
            {
                if (Inventory[i] != null && Inventory[i].BagId == bagId) return true;
            }
            return false;
        }
        public BagInstance GetBag()
        {
            if (InBag > -1)
            {
                for (int i = 0; i < Inventory.Count; i++)
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
            var itemBase = ItemBase.GetItem(Inventory[slot].ItemNum);
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
                        PacketSender.SendPlayerMsg(MyClient, Strings.Get("bags", "baginself"), Color.Red);
                        return;
                    }


                    //Find a spot in the bag for it!
                    if (itemBase.IsStackable())
                    {
                        for (int i = 0; i < bag.Slots; i++)
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
                    for (int i = 0; i < bag.Slots; i++)
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
                    PacketSender.SendPlayerMsg(MyClient, Strings.Get("bags", "bagnospace"), Color.Red);
                }
                else
                {
                    PacketSender.SendPlayerMsg(MyClient, Strings.Get("bags", "depositinvalid"), Color.Red);
                }
            }
        }
        public void RetreiveBagItem(int slot, int amount)
        {
            if (InBag < 0 || !HasBag(InBag)) return;
            var bag = GetBag();
            var itemBase = ItemBase.GetItem(bag.Items[slot].ItemNum);
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
                        for (int i = 0; i < Options.MaxInvItems; i++)
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
                        for (int j = 0; j < Options.MaxInvItems; j++)
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
                        PacketSender.SendPlayerMsg(MyClient, Strings.Get("bags", "inventorynospace"), Color.Red);
                        return; //Panda forgot this :P
                    }

                    /* Move the items to the inventory */
                    amount = Math.Min(amount, int.MaxValue - Inventory[inventorySlot].ItemVal);

                    if (Inventory[inventorySlot] == null || Inventory[inventorySlot].ItemNum == -1 || Inventory[inventorySlot].ItemVal < 0)
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
                    PacketSender.SendPlayerMsg(MyClient, Strings.Get("bags", "withdrawinvalid"), Color.Red);
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

        //Trading
        public void InviteToTrade(Player fromPlayer)
        {
            if (fromPlayer.TradeRequests.ContainsKey(this))
            {
                fromPlayer.TradeRequests.Remove(this);
            }
            if (TradeRequests.ContainsKey(fromPlayer) && TradeRequests[fromPlayer] > Globals.System.GetTimeMs())
            {
                PacketSender.SendPlayerMsg(fromPlayer.MyClient, Strings.Get("trading", "alreadydenied"), Color.Red);
            }
            else
            {
                if (TradeRequester == null && PartyRequester == null)
                {
                    TradeRequester = fromPlayer;
                    PacketSender.SendTradeRequest(MyClient, fromPlayer);
                }
                else
                {
                    PacketSender.SendPlayerMsg(fromPlayer.MyClient, Strings.Get("trading", "busy", MyName), Color.Red);
                }
            }
        }
        public void OfferItem(int slot, int amount)
        {
            if (Trading < 0) return;
            var itemBase = ItemBase.GetItem(Inventory[slot].ItemNum);
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

                    //Check if this is a bag with items.. if so don't allow sale
                    if (itemBase.ItemType == (int)ItemTypes.Bag)
                    {
                        if (Inventory[slot].BagId > -1)
                        {
                            if (!Database.BagEmpty(Inventory[slot].BagId))
                            {
                                PacketSender.SendPlayerMsg(MyClient, Strings.Get("bags", "onlytradeempty"), Color.Red);
                                return;
                            }
                        }
                    }

                    //Find a spot in the trade for it!
                    if (itemBase.IsStackable())
                    {
                        for (int i = 0; i < Options.MaxInvItems; i++)
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
                    for (int i = 0; i < Options.MaxInvItems; i++)
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
                    PacketSender.SendPlayerMsg(MyClient, Strings.Get("trading", "tradenosapce"), Color.Red);
                }
                else
                {
                    PacketSender.SendPlayerMsg(MyClient, Strings.Get("trading", "offerinvalid"), Color.Red);
                }
            }
        }
        public void RevokeItem(int slot, int amount)
        {
            if (Trading < 0) return;

            ItemBase itemBase = ItemBase.GetItem(Trade[slot].ItemNum);
            if (itemBase == null)
            {
                return;
            }

            if (Trade[slot] == null || Trade[slot].ItemNum < 0)
            {
                PacketSender.SendPlayerMsg(MyClient, Strings.Get("trading", "revokeinvalid"), Color.Red);
                return;
            }

            int inventorySlot = -1;
            bool stackable = itemBase.IsStackable();
            if (stackable)
            {
                /* Find an existing stack */
                for (int i = 0; i < Options.MaxInvItems; i++)
                {
                    if (this.Inventory[i] != null && this.Inventory[i].ItemNum == this.Trade[slot].ItemNum)
                    {
                        inventorySlot = i;
                        break;
                    }
                }
            }

            if (inventorySlot < 0)
            {
                /* Find a free slot if we don't have one already */
                for (int j = 0; j < Options.MaxInvItems; j++)
                {
                    if (this.Inventory[j] == null || this.Inventory[j].ItemNum == -1)
                    {
                        inventorySlot = j;
                        break;
                    }
                }
            }

            /* If we don't have a slot send an error. */
            if (inventorySlot < 0)
            {
                PacketSender.SendPlayerMsg(MyClient, Strings.Get("trading", "inventorynosapce"), Color.Red);
            }

            /* Move the items to the inventory */
            amount = Math.Min(amount, int.MaxValue - this.Inventory[inventorySlot].ItemVal);

            if (this.Inventory[inventorySlot] == null || this.Inventory[inventorySlot].ItemNum == -1 || this.Inventory[inventorySlot].ItemVal < 0)
            {
                this.Inventory[inventorySlot] = new ItemInstance(Trade[slot].ItemNum, 0, Trade[slot].BagId);
            }

            this.Inventory[inventorySlot].ItemVal += amount;
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

            for (int i = 0; i < Options.MaxInvItems; i++)
            {
                if (Trade[i].ItemNum > 0)
                {
                    if (!TryGiveItem(Trade[i]))
                    {
                        MapInstance.GetMap(CurrentMap).SpawnItem(CurrentX, CurrentY, Trade[i], Trade[i].ItemVal);
                        PacketSender.SendPlayerMsg(MyClient, Strings.Get("trading", "itemsdropped"), Color.Red);
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
            PacketSender.SendPlayerMsg(MyClient, Strings.Get("trading", "declined"), Color.Red);
            PacketSender.SendPlayerMsg(((Player)Globals.Entities[Trading]).MyClient, Strings.Get("trading", "declined"), Color.Red);
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
                PacketSender.SendPlayerMsg(fromPlayer.MyClient, Strings.Get("parties", "alreadydenied"), Color.Red);
            }
            else
            {
                if (TradeRequester == null && PartyRequester == null)
                {
                    PartyRequester = fromPlayer;
                    PacketSender.SendPartyInvite(MyClient, fromPlayer);
                }
                else
                {
                    PacketSender.SendPlayerMsg(fromPlayer.MyClient, Strings.Get("parties", "busy", MyName), Color.Red);
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
                    PacketSender.SendPlayerMsg(MyClient, Strings.Get("parties", "leaderinvonly"), Color.Red);
                    return;
                }

                //Check for member being already in the party, if so cancel
                for (int i = 0; i < Party.Count; i++)
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
                for (int i = 0; i < Party.Count; i++)
                {
                    Party[i].Party = Party;
                    PacketSender.SendParty(Party[i].MyClient);
                    PacketSender.SendPlayerMsg(Party[i].MyClient, Strings.Get("parties", "joined", target.MyName), Color.Green);
                }
            }
            else
            {
                PacketSender.SendPlayerMsg(MyClient, Strings.Get("parties", "limitreached"), Color.Red);
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
                    PacketSender.SendPlayerMsg(oldMember.MyClient, Strings.Get("parties", "kicked"), Color.Red);
                    Party.RemoveAt(target);

                    if (Party.Count > 1) //Need atleast 2 party members to function
                    {
                        //Update all members of the party with the new list
                        for (int i = 0; i < Party.Count; i++)
                        {
                            Party[i].Party = Party;
                            PacketSender.SendParty(Party[i].MyClient);
                            PacketSender.SendPlayerMsg(Party[i].MyClient, Strings.Get("parties", "memberkicked", oldMember.MyName), Color.Red);
                        }
                    }
                    else if (Party.Count > 0) //Check if anyone is left on their own
                    {
                        Player remainder = Party[0];
                        remainder.Party.Clear();
                        PacketSender.SendParty(remainder.MyClient);
                        PacketSender.SendPlayerMsg(remainder.MyClient, Strings.Get("parties", "disbanded"), Color.Red);
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
                    for (int i = 0; i < Party.Count; i++)
                    {
                        Party[i].Party = Party;
                        PacketSender.SendParty(Party[i].MyClient);
                        PacketSender.SendPlayerMsg(Party[i].MyClient, Strings.Get("parties", "memberleft", oldMember.MyName), Color.Red);
                    }
                }
                else if (Party.Count > 0) //Check if anyone is left on their own
                {
                    Player remainder = Party[0];
                    remainder.Party.Clear();
                    PacketSender.SendParty(remainder.MyClient);
                    PacketSender.SendPlayerMsg(remainder.MyClient, Strings.Get("parties", "disbanded"), Color.Red);
                }
            }
            Party.Clear();
            PacketSender.SendParty(MyClient);
            PacketSender.SendPlayerMsg(MyClient, Strings.Get("parties", "left"), Color.Red);
        }
        public bool InParty(Player member)
        {
            for (int i = 0; i < Party.Count; i++)
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

                for (int i = 0; i < Options.MaxInvItems; i++)
                {
                    Trade[i] = new ItemInstance();
                    target.Trade[i] = new ItemInstance();
                }

                //Send the trade confirmation to both players
                PacketSender.StartTrade(target.MyClient, MyIndex);
                PacketSender.StartTrade(MyClient, target.MyIndex);
            }
        }

        //Spells
        public bool TryTeachSpell(SpellInstance spell, bool SendUpdate = true)
        {
            if (KnowsSpell(spell.SpellNum)) { return false; }
            for (int i = 0; i < Options.MaxPlayerSkills; i++)
            {
                if (Spells[i].SpellNum <= 0)
                {
                    Spells[i] = spell.Clone();
                    if (SendUpdate)
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
            for (int i = 0; i < Options.MaxPlayerSkills; i++)
            {
                if (Spells[i].SpellNum == spellnum) { return true; }
            }
            return false;
        }
        public int FindSpell(int spellNum)
        {
            for (int i = 0; i < Options.MaxPlayerSkills; i++)
            {
                if (Spells[i].SpellNum == spellNum) { return i; }
            }
            return -1;
        }
        public void SwapSpells(int spell1, int spell2)
        {
            SpellInstance tmpInstance = Spells[spell2].Clone();
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
        public void UseSpell(int spellSlot, int target)
        {
            int spellNum = Spells[spellSlot].SpellNum;
            Target = target;
            if (SpellBase.Get(spellNum) != null)
            {
                var spell = SpellBase.GetSpell(spellNum);

                if (!EventInstance.MeetsConditionLists(spell.CastingReqs, this, null))
                {
                    PacketSender.SendPlayerMsg(MyClient, Strings.Get("combat", "dynamicreq"));
                    return;
                }

                //Check if the caster is silenced or stunned
                for (var n = 0; n < Status.Count; n++)
                {
                    if (Status[n].Type == (int)StatusTypes.Silence)
                    {
                        PacketSender.SendPlayerMsg(MyClient, Strings.Get("combat", "silenced"));
                        return;
                    }
                    if (Status[n].Type == (int)StatusTypes.Stun)
                    {
                        PacketSender.SendPlayerMsg(MyClient, Strings.Get("combat", "stunned"));
                        return;
                    }
                }

                //Check if the caster has the right ammunition if a projectile
                if (spell.SpellType == (int)SpellTargetTypes.Projectile && spell.Projectile > -1)
                {
                    var projectileBase = ProjectileBase.GetProjectile(spell.Projectile);
                    if (projectileBase.Ammo > -1)
                    {
                        if (FindItem(projectileBase.Ammo, projectileBase.AmmoRequired) == -1)
                        {
                            PacketSender.SendPlayerMsg(MyClient, Strings.Get("items", "notenough", ItemBase.GetName(projectileBase.Ammo)), Color.Red);
                            return;
                        }
                    }
                }

                if (target == -1 && ((spell.SpellType == (int)SpellTypes.CombatSpell && spell.TargetType == (int)SpellTargetTypes.Single) || spell.SpellType == (int)SpellTypes.WarpTo))
                {
                    PacketSender.SendActionMsg(this, Strings.Get("combat", "notarget"), new Color(255, 255, 0, 0));
                    return;
                }

                //Check for range of a single target spell
                if (spell.TargetType == (int)SpellTargetTypes.Single && Globals.Entities[Target] != this)
                {
                    if (!InRangeOf(Globals.Entities[Target], spell.CastRange))
                    {
                        PacketSender.SendActionMsg(this, Strings.Get("combat", "targetoutsiderange"), new Color(255, 255, 0, 0));
                        return;
                    }
                }

                if (spell.VitalCost[(int)Vitals.Mana] <= Vital[(int)Vitals.Mana])
                {
                    if (spell.VitalCost[(int)Vitals.Health] <= Vital[(int)Vitals.Health])
                    {
                        if (Spells[spellSlot].SpellCD < Globals.System.GetTimeMs())
                        {
                            if (CastTime < Globals.System.GetTimeMs())
                            {
                                Vital[(int)Vitals.Mana] = Vital[(int)Vitals.Mana] - spell.VitalCost[(int)Vitals.Mana];
                                Vital[(int)Vitals.Health] = Vital[(int)Vitals.Health] - spell.VitalCost[(int)Vitals.Health];
                                CastTime = Globals.System.GetTimeMs() + (spell.CastDuration * 100);
                                SpellCastSlot = spellSlot;

                                //Check if the caster has the right ammunition if a projectile
                                if (spell.SpellType == (int)SpellTargetTypes.Projectile && spell.Projectile > -1)
                                {
                                    var projectileBase = ProjectileBase.GetProjectile(spell.Projectile);
                                    if (projectileBase.Ammo > -1)
                                    {
                                        TakeItem(FindItem(projectileBase.Ammo, projectileBase.AmmoRequired), projectileBase.AmmoRequired);
                                    }
                                }

                                if (spell.CastAnimation > -1)
                                {
                                    PacketSender.SendAnimationToProximity(spell.CastAnimation, 1, MyIndex, CurrentMap, 0, 0, Dir); //Target Type 1 will be global entity
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
        public override void CastSpell(int SpellNum, int SpellSlot = -1)
        {
            var spellBase = SpellBase.GetSpell(SpellNum);
            if (spellBase != null)
            {
                if (spellBase.SpellType == (int)SpellTypes.Event)
                {
                    var evt = EventBase.GetEvent(spellBase.Data1);
                    if (evt != null)
                    {
                        StartCommonEvent(evt);
                    }
                }
                else
                {
                    base.CastSpell(SpellNum, SpellSlot);
                }
            }
        }

        //Equipment
        public void UnequipItem(int slot)
        {
            Equipment[slot] = -1;
            PacketSender.SendPlayerEquipmentToProximity(this);
        }
        public void EquipmentProcessItemSwap(int item1, int item2)
        {
            for (int i = 0; i < Options.EquipmentSlots.Count; i++)
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
            for (int i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                if (Equipment[i] == slot)
                    Equipment[i] = -1;
            }
            PacketSender.SendPlayerEquipmentToProximity(this);
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
            for (int i = 0; i < Options.MaxHotbar; i++)
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
            for (int i = 0; i < Options.MaxHotbar; i++)
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
            if (Quests.ContainsKey(quest.GetId()))
            {
                if (Quests[quest.GetId()].task != -1 && quest.GetTaskIndex(Quests[quest.GetId()].task) != -1)
                {
                    return false;
                }
                if (Quests[quest.GetId()].completed == 1 && quest.Repeatable == 0)
                {
                    return false;
                }
            }
            //So the quest isn't started or we can repeat it.. let's make sure that we meet requirements.
            if (!EventInstance.MeetsConditionLists(quest.Requirements, this, null)) return false;
            if (quest.Tasks.Count == 0)
            {
                return false;
            }
            return true;
        }
        public bool QuestCompleted(QuestBase quest)
        {
            if (Quests.ContainsKey(quest.GetId()))
            {
                if (Quests[quest.GetId()].completed == 1)
                {
                    return true;
                }
            }
            return false;
        }
        public bool QuestInProgress(QuestBase quest, QuestProgress progress, int taskId)
        {
            if (Quests.ContainsKey(quest.GetId()))
            {
                if (Quests[quest.GetId()].task != -1 && quest.GetTaskIndex(Quests[quest.GetId()].task) != -1)
                {
                    switch (progress)
                    {
                        case QuestProgress.OnAnyTask:
                            return true;
                        case QuestProgress.BeforeTask:
                            if (quest.GetTaskIndex(taskId) != -1)
                            {
                                return quest.GetTaskIndex(taskId) > quest.GetTaskIndex(Quests[quest.GetId()].task);
                            }
                            break;
                        case QuestProgress.OnTask:
                            if (quest.GetTaskIndex(taskId) != -1)
                            {
                                return quest.GetTaskIndex(taskId) == quest.GetTaskIndex(Quests[quest.GetId()].task);
                            }
                            break;
                        case QuestProgress.AfterTask:
                            if (quest.GetTaskIndex(taskId) != -1)
                            {
                                return quest.GetTaskIndex(taskId) < quest.GetTaskIndex(Quests[quest.GetId()].task);
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
                QuestOffers.Add(quest.GetId());
                PacketSender.SendQuestOffer(this, quest.GetId());
            }
        }
        public void StartQuest(QuestBase quest)
        {
            if (CanStartQuest(quest))
            {
                if (Quests.ContainsKey(quest.GetId()))
                {
                    var questProgress = Quests[quest.GetId()];
                    questProgress.task = quest.Tasks[0].Id;
                    questProgress.taskProgress = 0;
                    Quests[quest.GetId()] = questProgress;
                }
                else
                {
                    var questProgress = new QuestProgressStruct();
                    questProgress.task = quest.Tasks[0].Id;
                    questProgress.taskProgress = 0;
                    Quests.Add(quest.GetId(), questProgress);
                }
                if (quest.Tasks[0].Objective == 1) //Gather Items
                {
                    UpdateGatherItemQuests(quest.Tasks[0].Data1);
                }
                StartCommonEvent(quest.StartEvent);
                PacketSender.SendPlayerMsg(MyClient, Strings.Get("quests", "started", quest.Name), Color.Cyan);
                PacketSender.SendQuestProgress(this, quest.GetId());
            }
        }
        public void AcceptQuest(int questId)
        {
            if (QuestOffers.Contains(questId))
            {
                QuestOffers.Remove(questId);
                var quest = QuestBase.GetQuest(questId);
                if (quest != null)
                {
                    StartQuest(quest);
                    lock (EventLock)
                    {
                        for (int i = 0; i < MyEvents.Count; i++)
                        {
                            if (MyEvents[i] != null)
                            {
                                if (MyEvents[i].CallStack.Count <= 0) return;
                                if (MyEvents[i].CallStack.Peek().WaitingForResponse != CommandInstance.EventResponse.Quest) return;
                                if (MyEvents[i].CallStack.Peek().ResponseIndex == questId)
                                {
                                    MyEvents[i].CallStack.Peek().WaitingForResponse = CommandInstance.EventResponse.None;
                                    //Run success branch
                                    var tmpStack = new CommandInstance(MyEvents[i].CallStack.Peek().Page)
                                    {
                                        CommandIndex = 0,
                                        ListIndex = MyEvents[i].CallStack.Peek().Page.CommandLists[MyEvents[i].CallStack.Peek().ListIndex].Commands[MyEvents[i].CallStack.Peek().CommandIndex].Ints[4]
                                    };
                                    MyEvents[i].CallStack.Peek().CommandIndex++;
                                    MyEvents[i].CallStack.Push(tmpStack);
                                }
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
                QuestOffers.Remove(questId);
                PacketSender.SendPlayerMsg(MyClient, Strings.Get("quests", "declined", QuestBase.GetName(questId)), Color.Red);
                lock (EventLock)
                {
                    for (int i = 0; i < MyEvents.Count; i++)
                    {
                        if (MyEvents[i] != null)
                        {
                            if (MyEvents[i].CallStack.Count <= 0) return;
                            if (MyEvents[i].CallStack.Peek().WaitingForResponse != CommandInstance.EventResponse.Quest) return;
                            if (MyEvents[i].CallStack.Peek().ResponseIndex == questId)
                            {
                                MyEvents[i].CallStack.Peek().WaitingForResponse = CommandInstance.EventResponse.None;
                                //Run failure branch
                                var tmpStack = new CommandInstance(MyEvents[i].CallStack.Peek().Page)
                                {
                                    CommandIndex = 0,
                                    ListIndex = MyEvents[i].CallStack.Peek().Page.CommandLists[MyEvents[i].CallStack.Peek().ListIndex].Commands[MyEvents[i].CallStack.Peek().CommandIndex].Ints[5]
                                };
                                MyEvents[i].CallStack.Peek().CommandIndex++;
                                MyEvents[i].CallStack.Push(tmpStack);
                            }
                        }
                    }
                }
            }
        }
        public void CancelQuest(int questId)
        {
            var quest = QuestBase.GetQuest(questId);
            if (quest != null)
            {
                if (QuestInProgress(quest, QuestProgress.OnAnyTask, -1))
                {
                    //Cancel the quest somehow...
                    if (quest.Quitable == 1)
                    {
                        var questProgress = Quests[questId];
                        questProgress.task = -1;
                        questProgress.taskProgress = -1;
                        Quests[questId] = questProgress;
                        PacketSender.SendPlayerMsg(MyClient, Strings.Get("quests", "abandoned", QuestBase.GetName(questId)), Color.Red);
                        PacketSender.SendQuestProgress(this, questId);
                    }
                }
            }
        }
        public void CompleteQuestTask(int questId, int taskId)
        {
            var quest = QuestBase.GetQuest(questId);
            if (quest != null)
            {
                if (Quests.ContainsKey(questId))
                {
                    var questProgress = Quests[questId];
                    if (Quests[questId].task == taskId)
                    {
                        //Let's Advance this task or complete the quest
                        for (int i = 0; i < quest.Tasks.Count; i++)
                        {
                            if (quest.Tasks[i].Id == taskId)
                            {
                                PacketSender.SendPlayerMsg(MyClient, Strings.Get("quests", "taskcompleted"));
                                if (i == quest.Tasks.Count - 1)
                                {
                                    //Complete Quest
                                    questProgress.completed = 1;
                                    questProgress.task = -1;
                                    questProgress.taskProgress = -1;
                                    Quests[questId] = questProgress;
                                    StartCommonEvent(quest.Tasks[i].CompletionEvent);
                                    StartCommonEvent(quest.EndEvent);
                                    PacketSender.SendPlayerMsg(MyClient, Strings.Get("quests", "completed", quest.Name), Color.Green);
                                }
                                else
                                {
                                    //Advance Task
                                    questProgress.task = quest.Tasks[i + 1].Id;
                                    questProgress.taskProgress = 0;
                                    Quests[questId] = questProgress;
                                    StartCommonEvent(quest.Tasks[i].CompletionEvent);
                                    if (quest.Tasks[i + 1].Objective == 1) //Gather Items
                                    {
                                        UpdateGatherItemQuests(quest.Tasks[i + 1].Data1);
                                    }
                                    PacketSender.SendPlayerMsg(MyClient, Strings.Get("quests", "updated", quest.Name), Color.Cyan);
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
            var item = ItemBase.GetItem(itemNum);
            if (item != null)
            {
                for (int i = 0; i < Quests.Keys.Count; i++)
                {
                    var questId = Quests.Keys.ToArray()[i];
                    var quest = QuestBase.GetQuest(questId);
                    if (quest != null)
                    {
                        if (Quests[questId].task > -1)
                        {
                            //Assume this quest is in progress. See if we can find the task in the quest
                            var questTask = quest.FindTask(Quests[questId].task);
                            if (questTask != null)
                            {
                                if (questTask.Objective == 1 && questTask.Data1 == item.GetId()) //gather items
                                {
                                    var questProg = Quests[questId];
                                    if (questProg.taskProgress != CountItemInstances(item.GetId()))
                                    {
                                        questProg.taskProgress = CountItemInstances(item.GetId());
                                        if (questProg.taskProgress >= questTask.Data2)
                                        {
                                            CompleteQuestTask(questId, Quests[questId].task);
                                        }
                                        else
                                        {
                                            Quests[questId] = questProg;
                                            PacketSender.SendQuestProgress(this, quest.GetId());
                                            PacketSender.SendPlayerMsg(MyClient, Strings.Get("quests", "itemtask", quest.Name, questProg.taskProgress, questTask.Data2, ItemBase.GetName(questTask.Data1)));
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
        private int EventExists(int map, int x, int y)
        {
            for (var i = 0; i < MyEvents.Count; i++)
            {
                if (MyEvents[i] == null) continue;
                if (map == MyEvents[i].MapNum && x == MyEvents[i].SpawnX && y == MyEvents[i].SpawnY)
                {
                    return i;
                }
            }
            return -1;
        }

        public EventPageInstance EventAt(int map, int x, int y, int z)
        {
            foreach (var evt in MyEvents)
            {
                if (evt != null && evt.PageInstance != null)
                {
                    if (evt.PageInstance.CurrentMap == map && evt.PageInstance.CurrentX == x && evt.PageInstance.CurrentY == y && evt.PageInstance.CurrentZ == z)
                    {
                        return evt.PageInstance;
                    }
                }
            }
            return null;
        }

        public void TryActivateEvent(int mapNum, int eventIndex)
        {
            for (int i = 0; i < MyEvents.Count; i++)
            {
                if (MyEvents[i] != null)
                {
                    if (MyEvents[i].MapNum == mapNum && MyEvents[i].BaseEvent.MyIndex == eventIndex)
                    {
                        if (MyEvents[i].PageInstance == null) return;
                        if (MyEvents[i].PageInstance.Trigger != 0) return;
                        if (!IsEventOneBlockAway(i)) return;
                        if (MyEvents[i].CallStack.Count != 0) return;
                        var newStack = new CommandInstance(MyEvents[i].PageInstance.MyPage) { CommandIndex = 0, ListIndex = 0 };
                        MyEvents[i].CallStack.Push(newStack);
                        if (!MyEvents[i].IsGlobal)
                        {
                            MyEvents[i].PageInstance.TurnTowardsPlayer();
                        }
                        else
                        {
                            //Turn the global event opposite of the player
                            switch (Dir)
                            {
                                case 0:
                                    MyEvents[i].PageInstance.GlobalClone.ChangeDir(1);
                                    break;
                                case 1:
                                    MyEvents[i].PageInstance.GlobalClone.ChangeDir(0);
                                    break;
                                case 2:
                                    MyEvents[i].PageInstance.GlobalClone.ChangeDir(3);
                                    break;
                                case 3:
                                    MyEvents[i].PageInstance.GlobalClone.ChangeDir(2);
                                    break;
                            }
                        }
                    }
                }
            }
        }

        public void RespondToEvent(int mapNum, int eventIndex, int responseId)
        {
            lock (EventLock)
            {
                for (int i = 0; i < MyEvents.Count; i++)
                {
                    if (MyEvents[i] != null && MyEvents[i].MapNum == mapNum && MyEvents[i].BaseEvent.MyIndex == eventIndex)
                    {
                        if (MyEvents[i].CallStack.Count <= 0) return;
                        if (MyEvents[i].CallStack.Peek().WaitingForResponse != CommandInstance.EventResponse.Dialogue) return;
                        if (MyEvents[i].CallStack.Peek().ResponseIndex == 0)
                        {
                            MyEvents[i].CallStack.Peek().WaitingForResponse = CommandInstance.EventResponse.None;
                        }
                        else
                        {
                            var tmpStack = new CommandInstance(MyEvents[i].CallStack.Peek().Page);
                            tmpStack.CommandIndex = 0;
                            tmpStack.ListIndex = MyEvents[i].CallStack.Peek().Page.CommandLists[MyEvents[i].CallStack.Peek().ListIndex].Commands[MyEvents[i].CallStack.Peek().CommandIndex].Ints[responseId - 1];
                            MyEvents[i].CallStack.Peek().CommandIndex++;
                            MyEvents[i].CallStack.Peek().WaitingForResponse = CommandInstance.EventResponse.None;
                            MyEvents[i].CallStack.Push(tmpStack);
                        }
                        return;
                    }
                }
            }
        }

        static bool IsEventOneBlockAway(int eventIndex)
        {
            return true;
        }

        public int FindEvent(EventPageInstance en)
        {
            int id = -1;
            lock (EventLock)
            {
                for (int i = 0; i < MyEvents.Count; i++)
                {
                    var evt = MyEvents[i];
                    if (evt == null) { continue; }
                    if (evt.PageInstance == null) { continue; }

                    if (evt.PageInstance == en || evt.PageInstance.GlobalClone == en)
                    {
                        id = i;
                        return id;
                    }
                }
            }
            return id;
        }

        public EventInstance GetEventFromPageInstance(EventPageInstance instance)
        {
            var evt = FindEvent(instance);
            if (evt > -1 && evt < MyEvents.Count)
            {
                return MyEvents[evt];
            }
            else
            {
                return null;
            }
        }

        public void SendEvents()
        {
            for (int i = 0; i < MyEvents.Count; i++)
            {
                if (MyEvents[i] != null && MyEvents[i].PageInstance != null)
                {
                    MyEvents[i].PageInstance.SendToClient();
                }
            }
        }

        public bool StartCommonEvent(EventBase evt, int trigger = -1)
        {
            lock (EventLock)
            {
                for (int i = 0; i < MyEvents.Count; i++)
                {
                    if (MyEvents[i] != null && MyEvents[i].BaseEvent == evt) return false;
                }
                var tmpEvent = new EventInstance(MyEvents.Count, MyClient, evt, -1)
                {
                    MapNum = -1,
                    SpawnX = -1,
                    SpawnY = -1
                };
                MyEvents.Add(tmpEvent);
                tmpEvent.Update();
                if (tmpEvent.PageInstance != null && (trigger == -1 || tmpEvent.PageInstance.MyPage.Trigger == trigger))
                {
                    var newStack = new CommandInstance(tmpEvent.PageInstance.MyPage) { CommandIndex = 0, ListIndex = 0 };
                    tmpEvent.CallStack.Push(newStack);
                }
                else
                {
                    MyEvents.RemoveAt(MyEvents.Count - 1);
                }
                return true;
            }
        }

        public override int CanMove(int moveDir)
        {
            //If crafting or locked by event return blocked 
            if (InCraft > -1 && CraftIndex > -1)
            {
                return -5;
            }
            for (int i = 0; i < MyEvents.Count; i++)
            {
                if (MyEvents[i] != null)
                {
                    if (MyEvents[i].HoldingPlayer) return -5;
                }
            }
            return base.CanMove(moveDir);
        }

        public override void Move(int moveDir, Client client, bool DontUpdate = false, bool correction = false)
        {
            int index = MyIndex;
            int oldMap = CurrentMap;
            client = MyClient;
            base.Move(moveDir, client, DontUpdate, correction);
            // Check for a warp, if so warp the player.
            var attribute = MapInstance.GetMap(Globals.Entities[index].CurrentMap).Attributes[Globals.Entities[index].CurrentX, Globals.Entities[index].CurrentY];
            if (attribute != null && attribute.value == (int)MapAttributes.Warp)
            {
                Globals.Entities[index].Warp(attribute.data1, attribute.data2, attribute.data3, Globals.Entities[index].Dir);
            }

            //Check for slide tiles
            if (attribute != null && attribute.value == (int)MapAttributes.Slide)
            {
                if (attribute.data1 > 0)
                {
                    Globals.Entities[index].Dir = attribute.data1 - 1;
                } //If sets direction, set it.
                var dash = new DashInstance(this, 1, base.Dir);
            }

            for (int i = 0; i < MyEvents.Count; i++)
            {
                if (MyEvents[i] != null)
                {
                    if (MyEvents[i].MapNum == CurrentMap)
                    {
                        if (MyEvents[i].PageInstance != null)
                        {
                            if (MyEvents[i].PageInstance.CurrentMap == CurrentMap && MyEvents[i].PageInstance.CurrentX == CurrentX && MyEvents[i].PageInstance.CurrentY == CurrentY && MyEvents[i].PageInstance.CurrentZ == CurrentZ)
                            {
                                if (MyEvents[i].PageInstance.Trigger != 1) return;
                                if (MyEvents[i].CallStack.Count != 0) return;
                                var newStack = new CommandInstance(MyEvents[i].PageInstance.MyPage)
                                {
                                    CommandIndex = 0,
                                    ListIndex = 0
                                };
                                MyEvents[i].CallStack.Push(newStack);
                            }
                        }
                    }
                }
            }
        }
    }

    public class HotbarInstance
    {
        public int Type = -1;
        public int Slot = -1;
    }
}

