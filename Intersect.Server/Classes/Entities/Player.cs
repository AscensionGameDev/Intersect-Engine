using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Crafting;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Events.Commands;
using Intersect.GameObjects.Maps;
using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData.Players;
using Intersect.Server.EventProcessing;
using Intersect.Server.General;
using Intersect.Server.Localization;
using Intersect.Server.Maps;
using Intersect.Server.Networking;
using Intersect.Utilities;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Switch = Intersect.Server.Database.PlayerData.Players.Switch;

namespace Intersect.Server.Entities
{
    using LegacyDatabase = LegacyDatabase;

    public partial class Player : EntityInstance
    {
        //Online Players List
        [NotNull] private static readonly Dictionary<Guid, Player> OnlinePlayers = new Dictionary<Guid, Player>();
        public static Player FindOnline(Guid id) => OnlinePlayers.ContainsKey(id) ? OnlinePlayers[id] : null;
        public static Player FindOnline(string charName) => OnlinePlayers.Values.FirstOrDefault(s => s.Name.ToLower().Trim() == charName.ToLower().Trim());
        public static int OnlineCount => OnlinePlayers.Count;

        //Name, X, Y, Dir, Etc all in the base Entity Class
        public Guid ClassId { get; set; }
        public Gender Gender { get; set; }
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
        [NotNull]
        public virtual List<BankSlot> Bank { get; set; } = new List<BankSlot>();

        //Friends
        [NotNull]
        public virtual List<Friend> Friends { get; set; } = new List<Friend>();

        //HotBar
        [NotNull]
        public virtual List<HotbarSlot> Hotbar { get; set; } = new List<HotbarSlot>();

        //Quests
        [NotNull]
        public virtual List<Quest> Quests { get; set; } = new List<Quest>();

        //Switches
        [NotNull]
        public virtual List<Switch> Switches { get; set; } = new List<Switch>();

        //Variables
        [NotNull]
        public virtual List<Variable> Variables { get; set; } = new List<Variable>();

        public bool ValidateLists()
        {
            var changes = false;

            changes |= SlotHelper.ValidateSlots(Spells, Options.MaxPlayerSkills);
            changes |= SlotHelper.ValidateSlots(Items, Options.MaxInvItems);
            changes |= SlotHelper.ValidateSlots(Bank, Options.MaxBankSlots);

            if (Hotbar.Count < Options.MaxHotbar)
            {
                Hotbar.Sort((a, b) => a?.Slot - b?.Slot ?? 0);
                for (var i = Hotbar.Count; i < Options.MaxHotbar; i++)
                {
                    Hotbar.Add(new HotbarSlot(i));
                }
                changes = true;
            }


            return changes;
        }


        //5 minute timeout before someone can send a trade/party request after it has been declined
        [NotMapped] public const long REQUEST_DECLINE_TIMEOUT = 300000;  //TODO: Server option this bitch. JC is a lazy fuck

        //TODO: Clean all of this stuff up
        #region Temporary Values

        [NotMapped] public bool InGame;
        [NotMapped] public Guid LastMapEntered = Guid.Empty;

        [JsonIgnore, NotMapped] public Client MyClient;

        [JsonIgnore, NotMapped] private bool mSentMap;
        [JsonIgnore, NotMapped] private int mCommonEventLaunches = 0;
        [JsonIgnore, NotMapped] private object mEventLock = new object();
        [JsonIgnore, NotMapped] public ConcurrentDictionary<Guid, EventInstance> EventLookup = new ConcurrentDictionary<Guid, EventInstance>();

        #endregion

        #region Event Spawned Npcs

        [JsonIgnore] [NotMapped] public List<Npc> SpawnedNpcs = new List<Npc>();

        #endregion

        #region Chat

        [JsonIgnore] [NotMapped] public Player ChatTarget = null;

        #endregion

        #region Trading

        [JsonProperty(nameof(Trading))] private Guid JsonTradingId => Trading.Counterparty?.Id ?? Guid.Empty;

        [JsonIgnore, NotMapped] public Trading Trading;

        #endregion

        #region Quests

        [NotMapped] public List<Guid> QuestOffers = new List<Guid>();

        #endregion

        #region Crafting

        [NotMapped] public Guid CraftingTableId = Guid.Empty;
        [NotMapped] public Guid CraftId = Guid.Empty;
        [NotMapped] public long CraftTimer = 0;

        #endregion

        #region Parties

        [JsonProperty(nameof(Party))] private List<Guid> JsonPartyIds => Party.Select(partyMember => partyMember?.Id ?? Guid.Empty).ToList();
        [JsonProperty(nameof(PartyRequester))] private Guid JsonPartyRequesterId => PartyRequester?.Id ?? Guid.Empty;
        [JsonProperty(nameof(PartyRequests))] private Dictionary<Guid, long> JsonPartyRequests => PartyRequests.ToDictionary(pair => pair.Key?.Id ?? Guid.Empty, pair => pair.Value);

        [JsonIgnore, NotMapped] public List<Player> Party = new List<Player>();
        [JsonIgnore, NotMapped] public Player PartyRequester;
        [JsonIgnore, NotMapped] public Dictionary<Player, long> PartyRequests = new Dictionary<Player, long>();

        #endregion

        #region Friends

        [JsonProperty(nameof(FriendRequester))] private Guid JsonFriendRequesterId => FriendRequester?.Id ?? Guid.Empty;
        [JsonProperty(nameof(FriendRequests))] private Dictionary<Guid, long> JsonFriendRequests => FriendRequests.ToDictionary(pair => pair.Key?.Id ?? Guid.Empty, pair => pair.Value);

        [JsonIgnore, NotMapped] public Player FriendRequester;
        [JsonIgnore, NotMapped] public Dictionary<Player, long> FriendRequests = new Dictionary<Player, long>();

        #endregion

        #region Bag/Shops/etc

        [JsonProperty(nameof(InBag))] private bool JsonInBag => InBag != null;
        [JsonProperty(nameof(InShop))] private bool JsonInShop => InShop != null;

        [JsonIgnore, NotMapped] public Bag InBag;
        [JsonIgnore, NotMapped] public ShopBase InShop;

        [NotMapped] public bool InBank;

        #endregion

        #region Item Cooldowns

        [NotMapped] public Dictionary<Guid, long> ItemCooldowns = new Dictionary<Guid, long>();

        #endregion

        [JsonIgnore, ] public bool IsValidPlayer => !IsDisposed && MyClient?.Entity == this;

        [NotMapped]
        public long ExperienceToNextLevel => GetExperienceToNextLevel(Level);

        private long GetExperienceToNextLevel(int level)
        {
            if (level >= Options.MaxLevel) return -1;
            var classBase = ClassBase.Get(ClassId);
            return classBase?.ExperienceToNextLevel(level) ?? ClassBase.DEFAULT_BASE_EXPERIENCE;
        }

        public Player() { }

        public void SetOnline()
        {
            IsDisposed = false;
            mSentMap = false;
            if (OnlinePlayers.TryGetValue(Id, out var player))
            {
                if (player != this)
                {
                    throw new InvalidOperationException($@"A player with the id {Id} is already listed as online.");
                }
            }
            OnlinePlayers.Add(Id, this);
        }

        public override void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            base.Dispose();

            if (OnlinePlayers?.ContainsKey(Id) ?? false)
            {
                OnlinePlayers.Remove(Id);
            }
        }

        public void Logout()
        {
            var map = MapInstance.Get(MapId);
            map?.RemoveEntity(this);

            //Update parties
            LeaveParty();

            //Update trade
            CancelTrade();

            mSentMap = false;
            ChatTarget = null;

            //Clear all event spawned NPC's
            var entities = SpawnedNpcs.ToArray();
            foreach (var t in entities)
            {
                if (t == null || t.GetType() != typeof(Npc)) continue;
                if (t.Despawnable) t.Die(0);
            }
            SpawnedNpcs.Clear();

            lock (mEventLock)
            {
                EventLookup.Clear();
            }

            InGame = false;
            mSentMap = false;
            mCommonEventLaunches = 0;
            LastMapEntered = Guid.Empty;
            ChatTarget = null;
            QuestOffers.Clear();
            CraftingTableId = Guid.Empty;
            CraftId = Guid.Empty;
            CraftTimer = 0;
            PartyRequester = null;
            PartyRequests.Clear();
            FriendRequester = null;
            FriendRequests.Clear();
            InBag = null;
            InBank = false;
            InShop = null;
            ItemCooldowns.Clear();

            PacketSender.SendEntityLeave(Id, (int)EntityTypes.Player, MapId);
        }

        ~Player()
        {
            if (OnlinePlayers.ContainsKey(Id)) OnlinePlayers.Remove(Id);
        }

        //Update
        public override void Update(long timeMs)
        {
            if (!InGame || MapId == Guid.Empty)
            {
                return;
            }

            if (CraftingTableId != Guid.Empty && CraftId != Guid.Empty)
            {
                var b = CraftingTableBase.Get(CraftingTableId);
                if (b.Crafts.Contains(CraftId))
                {
                    if (CraftTimer + CraftBase.Get(CraftId).Time < timeMs)
                    {
                        CraftItem(CraftId);
                    }
                    else
                    {
                        if (!CheckCrafting(CraftId))
                        {
                            CraftId = Guid.Empty;
                        }
                    }
                }
                else
                {
                    CraftId = Guid.Empty;
                }
            }

            base.Update(timeMs);

            //Check for autorun common events and run them
            foreach (EventBase evt in EventBase.Lookup.Values)
            {
                if (evt != null && evt.CommonEvent)
                {
                    StartCommonEvent(evt, CommonEventTrigger.Autorun);
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
            if (LastMapEntered != MapId)
            {
                if (MapInstance.Get(LastMapEntered) != null)
                {
                    MapInstance.Get(LastMapEntered).RemoveEntity(this);
                }
                if (MapId != Guid.Empty)
                {
                    if (!MapInstance.Lookup.Keys.Contains(MapId))
                    {
                        WarpToSpawn();
                    }
                    else
                    {
                        MapInstance.Get(MapId).PlayerEnteredMap(this);
                    }
                }
            }

            var currentMap = MapInstance.Get(MapId);
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
                        map = MapInstance.Get(currentMap.SurroundingMaps[i]);
                    }
                    if (map == null) continue;
                    lock (map.GetMapLock())
                    {
                        //Check to see if we can spawn events, if already spawned.. update them.
                        lock (mEventLock)
                        {
                            foreach (var evtId in map.EventIds.ToArray())
                            {
                                var mapEvent = EventBase.Get(evtId);
                                if (mapEvent != null)
                                {
                                    //Look for event
                                    var foundEvent = EventExists(map.Id, mapEvent.SpawnX, mapEvent.SpawnY);
                                    if (foundEvent == null)
                                    {
                                        var tmpEvent = new EventInstance(Guid.NewGuid(), map.Id, MyClient, mapEvent)
                                        {
                                            Global = mapEvent.Global,
                                            MapId = map.Id,
                                            SpawnX = mapEvent.SpawnX,
                                            SpawnY = mapEvent.SpawnY
                                        };
                                        EventLookup.AddOrUpdate(tmpEvent.Id, tmpEvent, (key, oldValue) => tmpEvent);
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
            }
            //Check to see if we can spawn events, if already spawned.. update them.
            lock (mEventLock)
            {
                foreach (var evt in EventLookup.Values)
                {
                    if (evt == null) continue;
                    var eventFound = false;
                    if (evt.MapId == Guid.Empty)
                    {
                        evt.Update(timeMs);
                        if (evt.CallStack.Count > 0)
                        {
                            eventFound = true;
                        }
                    }
                    if (evt.MapId != MapId)
                    {
                        foreach (var t in MapInstance.Get(MapId).SurroundingMaps)
                        {
                            if (t == evt.MapId)
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
                    PacketSender.SendEntityLeaveTo(MyClient, evt.BaseEvent.Id, (int)EntityTypes.Event, evt.MapId);
                    EventLookup.TryRemove(evt.Id, out EventInstance z);
                }
            }
        }

        public void RemoveEvent(Guid id)
        {
            EventInstance outInstance;
            EventLookup.TryRemove(id, out outInstance);
            PacketSender.SendEntityLeaveTo(MyClient,id, (int)EntityTypes.Event, Id);
        }

        //Sending Data
        public override byte[] Data()
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(base.Data());
            bf.WriteInteger((int)Gender);
            bf.WriteGuid(ClassId);
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

            var cls = ClassBase.Get(ClassId);
            if (cls != null)
            {
                Warp(cls.SpawnMapId, cls.SpawnX, cls.SpawnY, cls.SpawnDir);
            }
            else
            {
                Warp(Guid.Empty, 0, 0, 0);
            }
            PacketSender.SendEntityDataToProximity(this);
            //Search death common event trigger
            foreach (EventBase evt in EventBase.Lookup.Values)
            {
                if (evt != null)
                {
                    StartCommonEvent(evt, CommonEventTrigger.OnRespawn);
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

            var playerClass = ClassBase.Get(ClassId);
            if (playerClass?.VitalRegen == null) return;

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

		public override int GetMaxVital(int vital)
		{
			ClassBase myclass = ClassBase.Get(this.ClassId);
            var classVital = 20;
            if (myclass != null)
            {
                if (myclass.IncreasePercentage)
                {
                    classVital = (int)(myclass.BaseVital[vital] * Math.Pow(1 + ((double)myclass.VitalIncrease[vital] / 100), Level - 1));
                }
                else
                {
                    classVital = myclass.BaseVital[vital] + (myclass.VitalIncrease[vital] * (Level - 1));
                }
            }

            //Loop through equipment and see if any items grant vital buffs
            for (var i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                if (Equipment[i] >= 0 && Equipment[i] < Options.MaxInvItems)
                {
                    if (Items[Equipment[i]].ItemId != Guid.Empty)
                    {
                        var item = ItemBase.Get(Items[Equipment[i]].ItemId);
                        if (item != null)
                        {
                            classVital += item.VitalsGiven[vital];
                        }
                    }
                }
            }

            //Must have at least 1 hp and no less than 0 mp
            if (vital == (int)Vitals.Health)
            {
                classVital = Math.Max(classVital, 1);
            }
            else if (vital == (int)Vitals.Mana)
            {
                classVital = Math.Max(classVital, 0);
            }
            return classVital;
		}
		public override int GetMaxVital(Vitals vital)
		{
			return GetMaxVital((int)vital);
		}
        public void FixVitals()
        {
            //If add/remove equipment then our vitals might exceed the new max.. this should fix those cases.
            SetVital(Vitals.Health, GetVital(Vitals.Health));
            SetVital(Vitals.Mana, GetVital(Vitals.Mana));
        }

		//Leveling
		public void SetLevel(int level, bool resetExperience = false)
        {
            if (level < 1) return;
            Level = Math.Min(Options.MaxLevel, level);
            if (resetExperience) Exp = 0;
            RecalculateStatsAndPoints();
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
                    var myclass = ClassBase.Get(ClassId);
                    if (myclass != null)
                    {
                        foreach (var spell in myclass.Spells)
                        {
                            if (spell.Level == Level)
                            {
                                var spellInstance = new Spell(spell.Id);
                                if (TryTeachSpell(spellInstance, true))
                                {
                                    spellMsgs.Add(Strings.Player.spelltaughtlevelup.ToString(SpellBase.GetName(spellInstance.SpellId)));
                                }
                            }
                        }
                    }
                }
            }

            PacketSender.SendPlayerMsg(MyClient, Strings.Player.levelup.ToString(Level), CustomColors.LevelUp, Name);
            PacketSender.SendActionMsg(this, Strings.Combat.levelup, CustomColors.LevelUp);
            foreach (var msg in spellMsgs)
            {
                PacketSender.SendPlayerMsg(MyClient, msg, CustomColors.Info, Name);
            }
            if (StatPoints > 0)
            {
                PacketSender.SendPlayerMsg(MyClient, Strings.Player.statpoints.ToString(StatPoints),
                    CustomColors.StatPoints, Name);
            }
            RecalculateStatsAndPoints();
            PacketSender.SendExperience(MyClient);
            PacketSender.SendPointsTo(MyClient);
            PacketSender.SendEntityDataToProximity(this);

            //Search for level up activated events and run them
            foreach (EventBase evt in EventBase.Lookup.Values)
            {
                if (evt != null)
                {
                    StartCommonEvent(evt, CommonEventTrigger.LevelUp);
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
            while (Exp >= GetExperienceToNextLevel(Level + levelCount) && GetExperienceToNextLevel(Level + levelCount) > 0)
            {
                Exp -= GetExperienceToNextLevel(Level + levelCount);
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
                var npcBase = ((Npc)en).Base;
                var playerEvent = npcBase.OnDeathEvent;
                var partyEvent = npcBase.OnDeathPartyEvent;

                if (Party.Count > 0) //If in party, split the exp.
                {
                    for (var i = 0; i < Party.Count; i++)
                    {
                        //TODO: Only share experience with party members on the 9 surrounding maps....
                        Party[i].GiveExperience(npcBase.Experience / Party.Count);
                        Party[i].UpdateQuestKillTasks(en);
                        if (partyEvent != null)
                        {
                            if (!(playerEvent != null && Party[i] == this))
                            {
                                Party[i].StartCommonEvent(partyEvent);
                            }
                        }
                    }
                }
                else
                {
                    GiveExperience(npcBase.Experience);
                    UpdateQuestKillTasks(en);
                }

                if (playerEvent != null) StartCommonEvent(playerEvent);
            }
            else if (en.GetType() == typeof(Resource))
            {
                var resourceBase = ((Resource)en).Base;
                if (resourceBase.Event != null)
                {
                    StartCommonEvent(resourceBase.Event);
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
                var quest = QuestBase.Get(questId);
                if (quest != null)
                {
                    if (questProgress.TaskId != Guid.Empty)
                    {
                        //Assume this quest is in progress. See if we can find the task in the quest
                        var questTask = quest.FindTask(questProgress.TaskId);
                        if (questTask != null)
                        {
                            if (questTask.Objective == QuestObjective.KillNpcs && questTask.TargetId == npc.Base.Id)
                            {
                                questProgress.TaskProgress++;
                                if (questProgress.TaskProgress >= questTask.Quantity)
                                {
                                    CompleteQuestTask(questId, questProgress.TaskId);
                                }
                                else
                                {
                                    PacketSender.SendQuestProgress(this, quest.Id);
                                    PacketSender.SendPlayerMsg(MyClient,
                                        Strings.Quests.npctask.ToString(quest.Name, questProgress.TaskProgress,
                                            questTask.Quantity, NpcBase.GetName(questTask.TargetId)));
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void TryAttack(EntityInstance enemy, ProjectileBase projectile, SpellBase parentSpell, ItemBase parentItem, int projectileDir)
        {
            if (!CanAttack(enemy, parentSpell)) return;

            //If Entity is resource, check for the correct tool and make sure its not a spell cast.
            if (enemy.GetType() == typeof(Resource))
            {
                if (((Resource)enemy).IsDead) return;
                // Check that a resource is actually required.
                var resource = ((Resource)enemy).Base;
                //Check Dynamic Requirements
                if (!Conditions.MeetsConditionLists(resource.HarvestingRequirements, this, null))
                {
                    PacketSender.SendPlayerMsg(MyClient, Strings.Combat.resourcereqs);
                    return;
                }
                if (resource.Tool > -1 && resource.Tool < Options.ToolTypes.Count)
                {
                    if (parentItem == null || resource.Tool != parentItem.Tool)
                    {
                        PacketSender.SendPlayerMsg(MyClient, Strings.Combat.toolrequired.ToString(Options.ToolTypes[resource.Tool]));
                        return;
                    }
                }
            }
            base.TryAttack(enemy, projectile, parentSpell, parentItem, projectileDir);
        }

        public override void TryAttack(EntityInstance enemy)
        {
            if (CastTime >= Globals.Timing.TimeMs)
            {
                PacketSender.SendPlayerMsg(MyClient, Strings.Combat.channelingnoattack);
                return;
            }

            if (!IsOneBlockAway(enemy)) return;
            if (!IsFacingTarget(enemy)) return;
            if (!CanAttack(enemy, null)) return;
            if (enemy.GetType() == typeof(EventPageInstance)) return;

            ItemBase weapon = null;
            if (Options.WeaponIndex > -1 && Options.WeaponIndex < Equipment.Length && Equipment[Options.WeaponIndex] >= 0)
            {
                weapon = ItemBase.Get(Items[Equipment[Options.WeaponIndex]].ItemId);
            }

            //If Entity is resource, check for the correct tool and make sure its not a spell cast.
            if (enemy.GetType() == typeof(Resource))
            {
                if (((Resource)enemy).IsDead) return;
                // Check that a resource is actually required.
                var resource = ((Resource)enemy).Base;
                //Check Dynamic Requirements
                if (!Conditions.MeetsConditionLists(resource.HarvestingRequirements, this, null))
                {
                    PacketSender.SendPlayerMsg(MyClient, Strings.Combat.resourcereqs);
                    return;
                }
                if (resource.Tool > -1 && resource.Tool < Options.ToolTypes.Count)
                {
                    if (weapon == null || resource.Tool != weapon.Tool)
                    {
                        PacketSender.SendPlayerMsg(MyClient,
                            Strings.Combat.toolrequired.ToString(Options.ToolTypes[resource.Tool]));
                        return;
                    }
                }
            }

            if (weapon != null)
            {
                base.TryAttack(enemy, weapon.Damage, (DamageType)weapon.DamageType,
                    (Stats)weapon.ScalingStat,
                    weapon.Scaling, weapon.CritChance, weapon.CritMultiplier, null, null, weapon);
            }
            else
            {
                var classBase = ClassBase.Get(ClassId);
                if (classBase != null)
                {
                    base.TryAttack(enemy, classBase.Damage,
                        (DamageType)classBase.DamageType, (Stats)classBase.ScalingStat,
                        classBase.Scaling, classBase.CritChance, classBase.CritMultiplier);
                }
                else
                {
                    base.TryAttack(enemy, 1, (DamageType)DamageType.Physical, Stats.Attack,
                        100, 10, 1.5);
                }
            }
            PacketSender.SendEntityAttack(this, (int)EntityTypes.GlobalEntity, MapId, CalculateAttackTime());
        }

        public override bool CanAttack(EntityInstance en, SpellBase spell)
        {
            if (!base.CanAttack(en, spell)) return false;
            if (en.GetType() == typeof(EventPageInstance)) return false;
            //Check if the attacker is stunned or blinded.
            var statuses = Statuses.Values.ToArray();
            foreach (var status in statuses)
            {
                if (status.Type == StatusTypes.Stun || status.Type == StatusTypes.Sleep)
                {
                    return false;
                }
            }
            if (en.GetType() == typeof(Player))
            {
                if (spell != null && spell.Combat.Friendly)
                {
                    return false;
                }
            }
            else if (en.GetType() == typeof(Resource))
            {
                if (spell != null) return false;
            }
            else if (en.GetType() == typeof(Npc))
            {
                var npc = ((Npc)en);
                var friendly = spell != null && spell.Combat.Friendly;
                if (!friendly && npc.CanPlayerAttack(this))
                {
                    return true;
                }
                else if (friendly && npc.IsFriend(this))
                {
                    return true;
                }
                return false;
            }
            return true;
        }

        public override void NotifySwarm(EntityInstance attacker)
        {
            var mapEntities = MapInstance.Get(MapId).GetEntities(true);
            foreach (var en in mapEntities)
            {
                if (en.GetType() == typeof(Npc))
                {
                    var npc = (Npc)en;
                    if (npc.Target == null && npc.IsFriend(this))
                    {
                        if (InRangeOf(npc, npc.Base.SightRange))
                        {
                            npc.AssignTarget(attacker);
                        }
                    }
                }
            }
        }

        public override int CalculateAttackTime()
        {
            ItemBase weapon = null;
            var attackTime = base.CalculateAttackTime();
            if (Options.WeaponIndex > -1 && Options.WeaponIndex < Equipment.Length && Equipment[Options.WeaponIndex] >= 0)
            {
                weapon = ItemBase.Get(Items[Equipment[Options.WeaponIndex]].ItemId);
            }

            if (weapon != null)
            {
                if (weapon.AttackSpeedModifier == 1) // Static
                {
                    attackTime = weapon.AttackSpeedValue;
                }
                else if (weapon.AttackSpeedModifier == 2) //Percentage
                {
                    attackTime = (int)(attackTime * (100f / weapon.AttackSpeedValue));
                }
            }
            return attackTime - 60; //subtracting 60 to account for a moderate ping to the server so some attacks dont get cancelled.
        }

        public override int GetStatBuffs(Stats statType)
        {
            var s = 0;
            //Add up player equipment values
            for (var i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                if (Equipment[i] >= 0 && Equipment[i] < Options.MaxInvItems)
                {
                    if (Items[Equipment[i]].ItemId != Guid.Empty)
                    {
                        var item = ItemBase.Get(Items[Equipment[i]].ItemId);
                        if (item != null)
                        {
                            s += Items[Equipment[i]].StatBuffs[(int)statType] + item.StatsGiven[(int)statType];
                        }
                    }
                }
            }
            return s;
        }

        public void RecalculateStatsAndPoints()
        {
            ClassBase playerClass = ClassBase.Get(ClassId);

            if (playerClass != null)
            {
                for (int i = 0; i < (int) Stats.StatCount; i++)
                {
                    var s = playerClass.BaseStat[i];
                    //Add class stat scaling
                    if (playerClass.IncreasePercentage) //% increase per level
                    {
                        s = (int)(s * Math.Pow(1 + ((double)playerClass.StatIncrease[i] / 100), Level - 1));
                    }
                    else //Static value increase per level
                    {
                        s += playerClass.StatIncrease[i] * (Level - 1);
                    }

                    BaseStats[i] = s;
                }

                //Handle Changes in Points
                var currentPoints = StatPoints + StatPointAllocations.Sum();
                var expectedPoints = playerClass.BasePoints + playerClass.PointIncrease * (Level - 1);
                if (expectedPoints > currentPoints)
                {
                    StatPoints += (expectedPoints - currentPoints);
                }
                else if (expectedPoints < currentPoints)
                {
                    var removePoints = currentPoints - expectedPoints;
                    StatPoints -= removePoints;
                    if (StatPoints < 0)
                    {
                        removePoints = Math.Abs(StatPoints);
                        StatPoints = 0;
                    }

                    var i = 0;
                    while (removePoints > 0 && StatPointAllocations.Sum() > 0)
                    {
                        if (StatPointAllocations[i] > 0)
                        {
                            StatPointAllocations[i]--;
                            removePoints--;
                        }
                        i++;
                        if (i >= (int) Stats.StatCount) i = 0;
                    }
                }
            }
        }

        //Warping
        public override void Warp(Guid newMapId, int newX, int newY, bool adminWarp = false)
        {
            Warp(newMapId, newX, newY, 1, adminWarp, 0, false);
        }

        public override void Warp(Guid newMapId, int newX, int newY, int newDir, bool adminWarp = false, int zOverride = 0, bool mapSave = false)
        {
            var map = MapInstance.Get(newMapId);
            if (map == null)
            {
                WarpToSpawn();
                return;
            }
            X = newX;
            Y = newY;
            Z = zOverride;
            Dir = newDir;
            var newSurroundingMaps = map.GetSurroundingMapIds(true);
            foreach (var evt in EventLookup.Values.ToArray())
            {
                if (evt.MapId != Guid.Empty && (!newSurroundingMaps.Contains(evt.MapId) || mapSave))
                {
                    EventLookup.TryRemove(evt.Id, out EventInstance z);
                }
            }
            if (newMapId != MapId || mSentMap == false)
            {
                var oldMap = MapInstance.Get(MapId);
                if (oldMap != null)
                {
                    oldMap.RemoveEntity(this);
                }
                PacketSender.SendEntityLeave(base.Id, (int)EntityTypes.Player, MapId);
                MapId = newMapId;
                map.PlayerEnteredMap(this);
                PacketSender.SendEntityDataToProximity(this);
                PacketSender.SendEntityPositionToAll(this);

                //If map grid changed then send the new map grid
                if (!adminWarp && (oldMap == null || !oldMap.SurroundingMaps.Contains(newMapId)))
                {
                    PacketSender.SendMapGrid(MyClient, map.MapGrid, true);
                }

                var surroundingMaps = map.GetSurroundingMaps(true);
                foreach (var surrMap in surroundingMaps)
                {
                    PacketSender.SendMap(MyClient, surrMap.Id);
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
            Guid mapId = Guid.Empty;
            int x = 0, y = 0, dir = 0;
            var cls = ClassBase.Get(ClassId);
            if (cls != null)
            {
                if (MapInstance.Lookup.Keys.Contains(cls.SpawnMapId))
                {
                    mapId = cls.SpawnMapId;
                }
                x = cls.SpawnX;
                y = cls.SpawnY;
                dir = cls.SpawnDir;
            }
            if (mapId == Guid.Empty)
            {
                using (var mapenum = MapInstance.Lookup.GetEnumerator())
                {
                    mapenum.MoveNext();
                    mapId = mapenum.Current.Value.Id;
                }
            }
            Warp(mapId, x, y, dir);
        }

        //Inventory
        public bool CanGiveItem(Item item)
        {
            var itemBase = ItemBase.Get(item.ItemId);
            if (itemBase != null)
            {
                if (itemBase.IsStackable())
                {
                    for (var i = 0; i < Options.MaxInvItems; i++)
                    {
                        if (Items[i].ItemId == item.ItemId)
                        {
                            return true;
                        }
                    }
                }

                //Either a non stacking item, or we couldn't find the item already existing in the players inventory
                for (var i = 0; i < Options.MaxInvItems; i++)
                {
                    if (Items[i].ItemId == Guid.Empty)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool TryGiveItem(Item item, bool sendUpdate = true)
        {
            var itemBase = ItemBase.Get(item.ItemId);
            if (itemBase != null)
            {
                if (itemBase.IsStackable())
                {
                    for (var i = 0; i < Options.MaxInvItems; i++)
                    {
                        if (Items[i].ItemId == item.ItemId)
                        {
                            Items[i].Quantity += item.Quantity;
                            if (sendUpdate)
                            {
                                PacketSender.SendInventoryItemUpdate(MyClient, i);
                            }
                            UpdateGatherItemQuests(item.ItemId);
                            return true;
                        }
                    }
                }

                //Either a non stacking item, or we couldn't find the item already existing in the players inventory
                for (var i = 0; i < Options.MaxInvItems; i++)
                {
                    if (Items[i].ItemId == Guid.Empty)
                    {
                        Items[i].Set(item);
                        if (sendUpdate)
                        {
                            PacketSender.SendInventoryItemUpdate(MyClient, i);
                        }
                        UpdateGatherItemQuests(item.ItemId);
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
        }

        public void DropItems(int slot, int amount)
        {
            var itemBase = ItemBase.Get(Items[slot].ItemId);
            if (itemBase != null)
            {
                if (itemBase.Bound)
                {
                    PacketSender.SendPlayerMsg(MyClient, Strings.Items.bound, CustomColors.ItemBound);
                    return;
                }

                if (itemBase.ItemType == ItemTypes.Bag)
                {
                    var bag = LegacyDatabase.GetBag(Items[slot]);
                    if (bag != null && !LegacyDatabase.BagEmpty(bag))
                    {
                        PacketSender.SendPlayerMsg(MyClient, Strings.Bags.dropnotempty, CustomColors.Error);
                        return;
                    }
                }

                for (var i = 0; i < Options.EquipmentSlots.Count; i++)
                {
                    if (Equipment[i] == slot)
                    {
                        PacketSender.SendPlayerMsg(MyClient, Strings.Items.equipped, CustomColors.ItemBound);
                        return;
                    }
                }

                if (amount >= Items[slot].Quantity)
                {
                    amount = Items[slot].Quantity;
                }
                if (itemBase.IsStackable())
                {
                    MapInstance.Get(MapId)
                        .SpawnItem(X, Y, Items[slot], amount);
                }
                else
                {
                    for (var i = 0; i < amount; i++)
                    {
                        MapInstance.Get(MapId)
                            .SpawnItem(X, Y, Items[slot], 1);
                    }
                }
                if (amount == Items[slot].Quantity)
                {
                    Items[slot].Set(Item.None);
                    EquipmentProcessItemLoss(slot);
                }
                else
                {
                    Items[slot].Quantity -= amount;
                }
                UpdateGatherItemQuests(itemBase.Id);
                PacketSender.SendInventoryItemUpdate(MyClient, slot);
            }
        }

        public void UseItem(int slot)
        {
            var equipped = false;
            var Item = Items[slot];
            var itemBase = ItemBase.Get(Item.ItemId);
            if (itemBase != null)
            {
                //Check if the user is silenced or stunned
                var statuses = Statuses.Values.ToArray();
                foreach (var status in statuses)
                {
                    if (status.Type == StatusTypes.Stun)
                    {
                        PacketSender.SendPlayerMsg(MyClient, Strings.Items.stunned);
                        return;
                    }
                    if (status.Type == StatusTypes.Sleep)
                    {
                        PacketSender.SendPlayerMsg(MyClient, Strings.Items.sleep);
                        return;
                    }
                }

                // Unequip items even if you do not meet the requirements. 
                // (Need this for silly devs who give people items and then later add restrictions...)
                if (itemBase.ItemType == ItemTypes.Equipment)
                {
                    for (var i = 0; i < Options.EquipmentSlots.Count; i++)
                    {
                        if (Equipment[i] == slot)
                        {
                            Equipment[i] = -1;
                            FixVitals();
                            PacketSender.SendPlayerEquipmentToProximity(this);
                            PacketSender.SendEntityStats(this);
                            return;
                        }
                    }
                }

                if (!Conditions.MeetsConditionLists(itemBase.UsageRequirements, this, null))
                {
                    PacketSender.SendPlayerMsg(MyClient, Strings.Items.dynamicreq);
                    return;
                }

                if (ItemCooldowns.ContainsKey(itemBase.Id) && ItemCooldowns[itemBase.Id] > Globals.Timing.TimeMs)
                {
                    //Cooldown warning!
                    PacketSender.SendPlayerMsg(MyClient, Strings.Items.cooldown);
                    return;
                }

                switch (itemBase.ItemType)
                {
                    case ItemTypes.None:
                    case ItemTypes.Currency:
                        PacketSender.SendPlayerMsg(MyClient, Strings.Items.cannotuse);
                        return;
                    case ItemTypes.Consumable:
                        var negative = itemBase.Consumable.Value < 0;
                        var symbol = negative ? Strings.Combat.removesymbol : Strings.Combat.addsymbol;
                        var number = $"{symbol}{itemBase.Consumable.Value}";
                        var color = CustomColors.Heal;
                        var die = false;

                        switch (itemBase.Consumable.Type)
                        {
                            case ConsumableType.Health:
                                AddVital(Vitals.Health, itemBase.Consumable.Value);
                                if (negative)
                                {
                                    color = CustomColors.PhysicalDamage;
                                    //Add a death handler for poison.
                                    die = !HasVital(Vitals.Health);
                                }
                                break;

                            case ConsumableType.Mana:
                                AddVital(Vitals.Mana, itemBase.Consumable.Value);
                                color = CustomColors.AddMana;
                                break;

                            case ConsumableType.Experience:
                                GiveExperience(itemBase.Consumable.Value);
                                color = CustomColors.Experience;
                                break;

                            default:
                                throw new IndexOutOfRangeException();
                        }

                        PacketSender.SendActionMsg(this, number, color);

                        if (die)
                        {
                            Die();
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
                            if (itemBase.EquipmentSlot == Options.WeaponIndex)
                            {
                                if (Options.WeaponIndex > -1)
                                {
                                    //If we are equipping a 2hand weapon, remove the shield
                                    Equipment[Options.WeaponIndex] = slot;
                                    if (itemBase.TwoHanded)
                                    {
                                        if (Options.ShieldIndex > -1 && Options.ShieldIndex < Equipment.Length)
                                        {
                                            Equipment[Options.ShieldIndex] = -1;
                                        }
                                    }
                                }
                            }
                            else if (itemBase.EquipmentSlot == Options.ShieldIndex)
                            {
                                if (Options.ShieldIndex > -1)
                                {
                                    if (Options.WeaponIndex > -1 && Equipment[Options.WeaponIndex] > -1)
                                    {
                                        //If we have a 2-hand weapon, remove it to equip this new shield
                                        var item = ItemBase.Get(Items[Equipment[Options.WeaponIndex]].ItemId);
                                        if (item != null && item.TwoHanded)
                                        {
                                            Equipment[Options.WeaponIndex] = -1;
                                        }
                                    }
                                    Equipment[Options.ShieldIndex] = slot;
                                }
                            }
                            else
                            {
                                Equipment[itemBase.EquipmentSlot] = slot;
                            }
                        }
                        FixVitals();
                        PacketSender.SendPlayerEquipmentToProximity(this);
                        PacketSender.SendEntityStats(this);
                        if (equipped) return;
                        break;
                    case ItemTypes.Spell:
                        if (itemBase.SpellId == Guid.Empty) return;

                        if (itemBase.QuickCast)
                        {
                            CastSpell(itemBase.SpellId);
                        }
                        else
                        {
                            if (!TryTeachSpell(new Spell(itemBase.SpellId))) return;
                        }

                        if (itemBase.DestroySpell)
                        {
                            TakeItemsBySlot(slot, 1);
                        }
                        break;
                    case ItemTypes.Event:
                        var evt = EventBase.Get(itemBase.EventId);
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
                    PacketSender.SendAnimationToProximity(itemBase.Animation.Id, 1, base.Id, MapId, 0, 0, Dir); //Target Type 1 will be global entity
                }
                if (itemBase.Cooldown > 0)
                {
                    decimal cooldownReduction = (1 - ((decimal)((Player)this).GetCooldownReduction() / 100));
                    if (ItemCooldowns.ContainsKey(itemBase.Id))
                    {
                        ItemCooldowns[itemBase.Id] = Globals.Timing.TimeMs + (long)(itemBase.Cooldown * cooldownReduction);
                    }
                    else
                    {
                        ItemCooldowns.Add(itemBase.Id, Globals.Timing.TimeMs + (long)(itemBase.Cooldown * cooldownReduction));
                    }
                    PacketSender.SendItemCooldown(MyClient, itemBase.Id);
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
            var itemBase = ItemBase.Get(Items[slot].ItemId);
            if (itemBase != null)
            {
                if (itemBase.IsStackable())
                {
                    if (amount > Items[slot].Quantity)
                    {
                        amount = Items[slot].Quantity;
                    }
                    else
                    {
                        if (amount == Items[slot].Quantity)
                        {
                            Items[slot].Set(Item.None);
                            EquipmentProcessItemLoss(slot);
                            returnVal = true;
                        }
                        else
                        {
                            Items[slot].Quantity -= amount;
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
                UpdateGatherItemQuests(itemBase.Id);
            }
            return returnVal;
        }

        public bool TakeItemsById(Guid itemId, int amount)
        {
            if (CountItems(itemId) < amount)
                return false;

            if (Items == null)
                return false;

            var invbackup = Items.Select(item => item?.Clone()).ToList();

            for (var i = 0; i < Options.MaxInvItems; i++)
            {
                var item = Items[i];
                if (item?.ItemId != itemId)
                    continue;

				//Remove equipment
	            for (var x = 0; x < Options.EquipmentSlots.Count; x++)
	            {
		            if (Equipment[x] == item.Slot)
		            {
			            Equipment[x] = -1;
			            PacketSender.SendPlayerEquipmentToProximity(this);
			            break;
		            }
	            }

				if (item.Quantity <= 1)
                {
                    amount -= 1;
                    Items[i].Set(Item.None);
                    PacketSender.SendInventoryItemUpdate(MyClient, i);
                    if (amount == 0)
                        return true;
                }
                else
                {
                    if (amount >= item.Quantity)
                    {
                        amount -= item.Quantity;
                        Items[i].Set(Item.None);
                        PacketSender.SendInventoryItemUpdate(MyClient, i);
                        if (amount == 0)
                            return true;
                    }
                    else
                    {
                        item.Quantity -= amount;
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

        public int FindItem(Guid itemId, int itemVal = 1)
        {
            if (Items == null)
                return -1;

            for (var i = 0; i < Options.MaxInvItems; i++)
            {
                var item = Items[i];
                if (item?.ItemId != itemId)
                    continue;

                if (item.Quantity >= itemVal)
                    return i;
            }

            return -1;
        }

        public int CountItems(Guid itemId)
        {
            if (Items == null)
                return -1;

            var count = 0;
            for (var i = 0; i < Options.MaxInvItems; i++)
            {
                var item = Items[i];
                if (item?.ItemId != itemId)
                    continue;

                count += Math.Max(1, item.Quantity);
            }

            return count;
        }

        public override int GetWeaponDamage()
        {
            if (Equipment[Options.WeaponIndex] > -1 && Equipment[Options.WeaponIndex] < Options.MaxInvItems)
            {
                if (Items[Equipment[Options.WeaponIndex]].ItemId != Guid.Empty)
                {
                    var item = ItemBase.Get(Items[Equipment[Options.WeaponIndex]].ItemId);
                    if (item != null)
                    {
                        return item.Damage;
                    }
                }
            }
            return 0;
        }

        public decimal GetCooldownReduction()
        {
            int cooldown = 0;

            for (var i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                if (Equipment[i] > -1)
                {
                    if (Items[Equipment[i]].ItemId != Guid.Empty)
                    {
                        var item = ItemBase.Get(Items[Equipment[i]].ItemId);
                        if (item != null)
                        {
                            //Check for cooldown reduction
                            if (item.Effect.Type == EffectType.CooldownReduction)
                            {
                                cooldown += item.Effect.Percentage;
                            }
                        }
                    }
                }
            }

            return cooldown;
        }

        public decimal GetLifeSteal()
        {
            int lifesteal = 0;

            for (var i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                if (Equipment[i] > -1)
                {
                    if (Items[Equipment[i]].ItemId != Guid.Empty)
                    {
                        var item = ItemBase.Get(Items[Equipment[i]].ItemId);
                        if (item != null)
                        {
                            //Check for cooldown reduction
                            if (item.Effect.Type == EffectType.Lifesteal)
                            {
                                lifesteal += item.Effect.Percentage;
                            }
                        }
                    }
                }
            }

            return lifesteal;
        }

        //Shop
        public bool OpenShop(ShopBase shop)
        {
            if (IsBusy()) return false;
            InShop = shop;
            PacketSender.SendOpenShop(MyClient, shop);
            return true;
        }

        public void CloseShop()
        {
            if (InShop != null)
            {
                InShop = null;
                PacketSender.SendCloseShop(MyClient);
            }
        }

        public void SellItem(int slot, int amount)
        {
            var canSellItem = true;
            Guid rewardItemId = Guid.Empty;
            var rewardItemVal = 0;
            var sellItemNum = Items[slot].ItemId;
            var shop = InShop;
            if (shop != null)
            {
                var itemBase = ItemBase.Get(Items[slot].ItemId);
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
                        if (shop.BuyingItems[i].ItemId == sellItemNum)
                        {
                            if (!shop.BuyingWhitelist)
                            {
                                PacketSender.SendPlayerMsg(MyClient, Strings.Shops.doesnotaccept,
                                    CustomColors.Error);
                                return;
                            }
                            else
                            {
                                rewardItemId = shop.BuyingItems[i].CostItemId;
                                rewardItemVal = shop.BuyingItems[i].CostItemQuantity;
                                break;
                            }
                        }
                    }
                    if (rewardItemId == Guid.Empty)
                    {
                        if (shop.BuyingWhitelist)
                        {
                            PacketSender.SendPlayerMsg(MyClient, Strings.Shops.doesnotaccept,
                                CustomColors.Error);
                            return;
                        }
                        else
                        {
                            rewardItemId = shop.DefaultCurrencyId;
                            rewardItemVal = itemBase.Price;
                        }
                    }

                    if (amount >= Items[slot].Quantity)
                    {
                        amount = Items[slot].Quantity;
                    }
                    if (amount == Items[slot].Quantity)
                    {
                        //Definitely can get reward.
                        Items[slot].Set(Item.None);
                        EquipmentProcessItemLoss(slot);
                    }
                    else
                    {
                        //check if can get reward
                        if (!CanGiveItem(new Item(rewardItemId, rewardItemVal)))
                        {
                            canSellItem = false;
                        }
                        else
                        {
                            Items[slot].Quantity -= amount;
                        }
                    }
                    if (canSellItem)
                    {
                        TryGiveItem(new Item(rewardItemId, rewardItemVal * amount), true);
                    }
                    PacketSender.SendInventoryItemUpdate(MyClient, slot);
                }
            }
        }

        public void BuyItem(int slot, int amount)
        {
            var canSellItem = true;
            var buyItemNum = Guid.Empty;
            var buyItemAmt = 1;
            var shop = InShop;
            if (shop != null)
            {
                if (slot >= 0 && slot < shop.SellingItems.Count)
                {
                    var itemBase = ItemBase.Get(shop.SellingItems[slot].ItemId);
                    if (itemBase != null)
                    {
                        buyItemNum = shop.SellingItems[slot].ItemId;
                        if (itemBase.IsStackable())
                        {
                            buyItemAmt = Math.Max(1, amount);
                        }
                        if (shop.SellingItems[slot].CostItemQuantity == 0 ||
                            FindItem(shop.SellingItems[slot].CostItemId,
                                shop.SellingItems[slot].CostItemQuantity * buyItemAmt) > -1)
                        {
                            if (CanGiveItem(new Item(buyItemNum, buyItemAmt)))
                            {
                                if (shop.SellingItems[slot].CostItemQuantity > 0)
                                {
                                    TakeItemsBySlot(
                                        FindItem(shop.SellingItems[slot].CostItemId,
                                            shop.SellingItems[slot].CostItemQuantity * buyItemAmt),
                                        shop.SellingItems[slot].CostItemQuantity * buyItemAmt);
                                }
                                TryGiveItem(new Item(buyItemNum, buyItemAmt), true);
                            }
                            else
                            {
                                if (shop.SellingItems[slot].CostItemQuantity * buyItemAmt ==
                                    Items[
                                        FindItem(shop.SellingItems[slot].CostItemId,
                                            shop.SellingItems[slot].CostItemQuantity * buyItemAmt)].Quantity)
                                {
                                    TakeItemsBySlot(
                                        FindItem(shop.SellingItems[slot].CostItemId,
                                            shop.SellingItems[slot].CostItemQuantity * buyItemAmt),
                                        shop.SellingItems[slot].CostItemQuantity * buyItemAmt);
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
                CraftingTableId = table.Id;
                PacketSender.SendOpenCraftingTable(MyClient, table);
            }
            return true;
        }

        public void CloseCraftingTable()
        {
            if (CraftingTableId != Guid.Empty && CraftId == Guid.Empty)
            {
                CraftingTableId = Guid.Empty;
                PacketSender.SendCloseCraftingTable(MyClient);
            }
        }

        //Craft a new item
        public void CraftItem(Guid id)
        {
            if (CraftingTableId != Guid.Empty)
            {
                var invbackup = new List<Item>();
                foreach (var item in Items)
                {
                    invbackup.Add(item.Clone());
                }

                //Quickly Look through the inventory and create a catalog of what items we have, and how many
                var itemdict = new Dictionary<Guid, int>();
                foreach (var item in Items)
                {
                    if (item != null)
                    {
                        if (itemdict.ContainsKey(item.ItemId))
                        {
                            itemdict[item.ItemId] += item.Quantity;
                        }
                        else
                        {
                            itemdict.Add(item.ItemId, item.Quantity);
                        }
                    }
                }

                //Check the player actually has the items
                foreach (var c in CraftBase.Get(id).Ingredients)
                {
                    if (itemdict.ContainsKey(c.ItemId))
                    {
                        if (itemdict[c.ItemId] >= c.Quantity)
                        {
                            itemdict[c.ItemId] -= c.Quantity;
                        }
                        else
                        {
                            CraftId = Guid.Empty;
                            return;
                        }
                    }
                    else
                    {
                        CraftId = Guid.Empty;
                        return;
                    }
                }

                //Take the items
                foreach (var c in CraftBase.Get(id).Ingredients)
                {
                    if (!TakeItemsById(c.ItemId, c.Quantity))
                    {
                        for (int i = 0; i < invbackup.Count; i++)
                        {
                            Items[i].Set(invbackup[i]);
                        }
                        PacketSender.SendInventory(MyClient);
                        CraftId = Guid.Empty;
                        return;
                    }
                }

                //Give them the craft
                if (TryGiveItem(new Item(CraftBase.Get(id).ItemId, 1)))
                {
                    PacketSender.SendPlayerMsg(MyClient,
                        Strings.Crafting.crafted.ToString(
                            ItemBase.GetName(CraftBase.Get(id).ItemId)),
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
                            ItemBase.GetName(CraftBase.Get(id).ItemId)),
                        CustomColors.Error);
                }
                CraftId = Guid.Empty;
            }
        }

        public bool CheckCrafting(Guid id)
        {
            //See if we have lost the items needed for our current craft, if so end the crafting session
            //Quickly Look through the inventory and create a catalog of what items we have, and how many
            var itemdict = new Dictionary<Guid, int>();
            foreach (var item in Items)
            {
                if (item != null)
                {
                    if (itemdict.ContainsKey(item.ItemId))
                    {
                        itemdict[item.ItemId] += item.Quantity;
                    }
                    else
                    {
                        itemdict.Add(item.ItemId, item.Quantity);
                    }
                }
            }

            //Check the player actually has the items
            foreach (var c in CraftBase.Get(id).Ingredients)
            {
                if (itemdict.ContainsKey(c.ItemId))
                {
                    if (itemdict[c.ItemId] >= c.Quantity)
                    {
                        itemdict[c.ItemId] -= c.Quantity;
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
            return InShop != null || InBank || CraftingTableId != Guid.Empty || Trading.Counterparty != null;
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
            var itemBase = ItemBase.Get(Items[slot].ItemId);
            if (itemBase != null)
            {
                if (Items[slot].ItemId != Guid.Empty)
                {
                    if (itemBase.IsStackable())
                    {
                        if (amount >= Items[slot].Quantity)
                        {
                            amount = Items[slot].Quantity;
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
                            if (Bank[i] != null && Bank[i].ItemId == Items[slot].ItemId)
                            {
                                amount = Math.Min(amount, int.MaxValue - Bank[i].Quantity);
                                Bank[i].Quantity += amount;
                                //Remove Items from inventory send updates
                                if (amount >= Items[slot].Quantity)
                                {
                                    Items[slot].Set(Item.None);
                                    EquipmentProcessItemLoss(slot);
                                }
                                else
                                {
                                    Items[slot].Quantity -= amount;
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
                        if (Bank[i] == null || Bank[i].ItemId == Guid.Empty)
                        {
                            Bank[i].Set(Items[slot]);
                            Bank[i].Quantity = amount;
                            //Remove Items from inventory send updates
                            if (amount >= Items[slot].Quantity)
                            {
                                Items[slot].Set(Item.None);
                                EquipmentProcessItemLoss(slot);
                            }
                            else
                            {
                                Items[slot].Quantity -= amount;
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

            var itemBase = ItemBase.Get(bankSlotItem.ItemId);
            var inventorySlot = -1;
            if (itemBase == null) return;
            if (bankSlotItem.ItemId != Guid.Empty)
            {
                if (itemBase.IsStackable())
                {
                    if (amount >= bankSlotItem.Quantity)
                    {
                        amount = bankSlotItem.Quantity;
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
                        if (inventorySlotItem.ItemId != bankSlotItem.ItemId) continue;
                        inventorySlot = i;
                        break;
                    }
                }

                if (inventorySlot < 0)
                {
                    /* Find a free slot if we don't have one already */
                    for (var j = 0; j < Options.MaxInvItems; j++)
                    {
                        if (Items[j] != null && Items[j].ItemId != Guid.Empty) continue;
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
                amount = Math.Min(amount, int.MaxValue - Items[inventorySlot].Quantity);

                if (Items[inventorySlot] == null || Items[inventorySlot].ItemId == Guid.Empty ||
                    Items[inventorySlot].Quantity < 0)
                {
                    Items[inventorySlot].Set(bankSlotItem);
                    Items[inventorySlot].Quantity = 0;
                }

                Items[inventorySlot].Quantity += amount;
                if (amount >= bankSlotItem.Quantity)
                {
                    Bank[slot].Set(Item.None);
                }
                else
                {
                    bankSlotItem.Quantity -= amount;
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
                Bank[item2].Set(Item.None);
            }
            if (tmpInstance != null)
            {
                Bank[item1].Set(tmpInstance);
            }
            else
            {
                Bank[item1].Set(Item.None);
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
                if (bagItem.Bag == null) //Bag doesnt exist, create it!
                {
                    //Create the Bag
                    var slotCount = itemBase.SlotCount;
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
            var itemBase = ItemBase.Get(Items[slot].ItemId);
            var bag = GetBag();
            if (itemBase != null && bag != null)
            {
                if (Items[slot].ItemId != Guid.Empty)
                {
                    if (itemBase.IsStackable())
                    {
                        if (amount >= Items[slot].Quantity)
                        {
                            amount = Items[slot].Quantity;
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

                    if (itemBase.ItemType == ItemTypes.Bag)
                    {
                        PacketSender.SendPlayerMsg(MyClient,Strings.Bags.baginbag, CustomColors.Error);
                        return;
                    }

                    //Find a spot in the bag for it!
                    if (itemBase.IsStackable())
                    {
                        for (var i = 0; i < bag.SlotCount; i++)
                        {
                            if (bag.Slots[i] != null && bag.Slots[i].ItemId == Items[slot].ItemId)
                            {
                                amount = Math.Min(amount, int.MaxValue - bag.Slots[i].Quantity);
                                bag.Slots[i].Quantity += amount;
                                //Remove Items from inventory send updates
                                if (amount >= Items[slot].Quantity)
                                {
                                    Items[slot].Set(Item.None);
                                    EquipmentProcessItemLoss(slot);
                                }
                                else
                                {
                                    Items[slot].Quantity -= amount;
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
                        if (bag.Slots[i] == null || bag.Slots[i].ItemId == Guid.Empty)
                        {
                            bag.Slots[i].Set(Items[slot]);
                            bag.Slots[i].Quantity = amount;
                            //Remove Items from inventory send updates
                            if (amount >= Items[slot].Quantity)
                            {
                                Items[slot].Set(Item.None);
                                EquipmentProcessItemLoss(slot);
                            }
                            else
                            {
                                Items[slot].Quantity -= amount;
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
            var itemBase = ItemBase.Get(bag.Slots[slot].ItemId);
            var inventorySlot = -1;
            if (itemBase != null)
            {
                if (bag.Slots[slot] != null && bag.Slots[slot].ItemId != Guid.Empty)
                {
                    if (itemBase.IsStackable())
                    {
                        if (amount >= bag.Slots[slot].Quantity)
                        {
                            amount = bag.Slots[slot].Quantity;
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
                            if (Items[i] != null && Items[i].ItemId == bag.Slots[slot].ItemId)
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
                            if (Items[j] == null || Items[j].ItemId == Guid.Empty)
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
                    amount = Math.Min(amount, int.MaxValue - Items[inventorySlot].Quantity);

                    if (Items[inventorySlot] == null || Items[inventorySlot].ItemId == Guid.Empty ||
                        Items[inventorySlot].Quantity < 0)
                    {
                        Items[inventorySlot].Set(bag.Slots[slot]);
                        Items[inventorySlot].Quantity = 0;
                    }

                    Items[inventorySlot].Quantity += amount;
                    if (amount >= bag.Slots[slot].Quantity)
                    {
                        bag.Slots[slot].Set(Item.None);
                    }
                    else
                    {
                        bag.Slots[slot].Quantity -= amount;
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
                bag.Slots[item2].Set(Item.None);
            }
            if (tmpInstance != null)
            {
                bag.Slots[item1].Set(tmpInstance);
            }
            else
            {
                bag.Slots[item1].Set(Item.None);
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
            if (!FriendRequests.ContainsKey(fromPlayer) || !(FriendRequests[fromPlayer] > Globals.Timing.TimeMs))
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
                    PacketSender.SendPlayerMsg(fromPlayer.MyClient, Strings.Friends.busy.ToString(Name),
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

        public void RemoveFriend(Player character)
        {
            var friend = Friends.FirstOrDefault(f => f.Target == character);
            if (friend != null) Friends.Remove(friend);
        }

        //Trading
        public void InviteToTrade(Player fromPlayer)
        {
			if (Trading.Requests == null) Trading = new Trading(this);
			if (fromPlayer.Trading.Requests == null) fromPlayer.Trading = new Trading(fromPlayer);

			if (fromPlayer.Trading.Requests.ContainsKey(this))
            {
                fromPlayer.Trading.Requests.Remove(this);
            }
            if (Trading.Requests.ContainsKey(fromPlayer) && Trading.Requests[fromPlayer] > Globals.Timing.TimeMs)
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
                    PacketSender.SendPlayerMsg(fromPlayer.MyClient, Strings.Trading.busy.ToString(Name),
                        CustomColors.Error);
                }
            }
        }

        public void OfferItem(int slot, int amount)
        {
            if (Trading.Counterparty == null) return;
            var itemBase = ItemBase.Get(Items[slot].ItemId);
            if (itemBase != null)
            {
                if (Items[slot].ItemId != Guid.Empty)
                {
                    if (itemBase.IsStackable())
                    {
                        if (amount >= Items[slot].Quantity)
                        {
                            amount = Items[slot].Quantity;
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
                            if (Trading.Offer[i] != null && Trading.Offer[i].ItemId == Items[slot].ItemId)
                            {
                                amount = Math.Min(amount, int.MaxValue - Trading.Offer[i].Quantity);
                                Trading.Offer[i].Quantity += amount;
                                //Remove Items from inventory send updates
                                if (amount >= Items[slot].Quantity)
                                {
                                    Items[slot].Set(Item.None);
                                    EquipmentProcessItemLoss(slot);
                                }
                                else
                                {
                                    Items[slot].Quantity -= amount;
                                }
                                PacketSender.SendInventoryItemUpdate(MyClient, slot);
                                PacketSender.SendTradeUpdate(MyClient, this, i);
                                PacketSender.SendTradeUpdate(Trading.Counterparty?.MyClient, this, i);
                                return;
                            }
                        }
                    }

                    //Either a non stacking item, or we couldn't find the item already existing in the players inventory
                    for (var i = 0; i < Options.MaxInvItems; i++)
                    {
                        if (Trading.Offer[i] == null || Trading.Offer[i].ItemId == Guid.Empty)
                        {
                            Trading.Offer[i] = Items[slot].Clone();
                            Trading.Offer[i].Quantity = amount;
                            //Remove Items from inventory send updates
                            if (amount >= Items[slot].Quantity)
                            {
                                Items[slot].Set(Item.None);
                                EquipmentProcessItemLoss(slot);
                            }
                            else
                            {
                                Items[slot].Quantity -= amount;
                            }
                            PacketSender.SendInventoryItemUpdate(MyClient, slot);
                            PacketSender.SendTradeUpdate(MyClient, this, i);
                            PacketSender.SendTradeUpdate(Trading.Counterparty?.MyClient, this, i);
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

            var itemBase = ItemBase.Get(Trading.Offer[slot].ItemId);
            if (itemBase == null)
            {
                return;
            }

            if (Trading.Offer[slot] == null || Trading.Offer[slot].ItemId == Guid.Empty)
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
                    if (Items[i] != null && Items[i].ItemId == Trading.Offer[slot].ItemId)
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
                    if (Items[j] == null || Items[j].ItemId == Guid.Empty)
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

            if (amount > Trading.Offer[slot].Quantity)
                amount = Trading.Offer[slot].Quantity;

            /* Move the items to the inventory */
            amount = Math.Min(amount, int.MaxValue - Items[inventorySlot].Quantity);

            if (Items[inventorySlot] == null || Items[inventorySlot].ItemId == Guid.Empty || Items[inventorySlot].Quantity < 0)
            {
                Items[inventorySlot].Set(Trading.Offer[slot]);
                Items[inventorySlot].Quantity = amount;
            }
            else
            {
                Items[inventorySlot].Quantity += amount;
            }

            if (amount >= Trading.Offer[slot].Quantity)
            {
                Trading.Offer[slot] = null;
            }
            else
            {
                Trading.Offer[slot].Quantity -= amount;
            }

            PacketSender.SendInventoryItemUpdate(MyClient, inventorySlot);
            PacketSender.SendTradeUpdate(MyClient, this, slot);
            PacketSender.SendTradeUpdate(Trading.Counterparty?.MyClient, this, slot);
        }

        public void ReturnTradeItems()
        {
            if (Trading.Counterparty == null)
            {
                return;
            }

            foreach (var offer in Trading.Offer)
            {
                if (offer == null || offer.ItemId == Guid.Empty)
                {
                    continue;
                }

                if (!TryGiveItem(new Item(offer)))
                {
                    MapInstance.Get(MapId)?.SpawnItem(X, Y, offer, offer.Quantity);
                    PacketSender.SendPlayerMsg(MyClient, Strings.Trading.itemsdropped, CustomColors.Error);
                }

                offer.ItemId = Guid.Empty;
                offer.Quantity = 0;
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
            if (PartyRequests.ContainsKey(fromPlayer) && PartyRequests[fromPlayer] > Globals.Timing.TimeMs)
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
                    PacketSender.SendPlayerMsg(fromPlayer.MyClient, Strings.Parties.busy.ToString(Name),
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
                    PacketSender.SendPlayerMsg(Party[i].MyClient, Strings.Parties.joined.ToString(target.Name),
                        CustomColors.Accepted);
                }
            }
            else
            {
                PacketSender.SendPlayerMsg(MyClient, Strings.Parties.limitreached, CustomColors.Error);
            }
        }

        public void KickParty(Guid target)
        {
            if (Party.Count > 0 && Party[0] == this)
            {
                if (target != Guid.Empty)
                {
                    var oldMember = Party.Where(p => p.Id == target).FirstOrDefault();
                    if (oldMember != null)
                    {
                        oldMember.Party = new List<Player>();
                        PacketSender.SendParty(oldMember.MyClient);
                        PacketSender.SendPlayerMsg(oldMember.MyClient, Strings.Parties.kicked, CustomColors.Error);
                        Party.Remove(oldMember);

                        if (Party.Count > 1) //Need atleast 2 party members to function
                        {
                            //Update all members of the party with the new list
                            for (var i = 0; i < Party.Count; i++)
                            {
                                Party[i].Party = Party;
                                PacketSender.SendParty(Party[i].MyClient);
                                PacketSender.SendPlayerMsg(Party[i].MyClient, Strings.Parties.memberkicked.ToString(oldMember.Name), CustomColors.Error);
                            }
                        }
                        else if (Party.Count > 0) //Check if anyone is left on their own
                        {
                            var remainder = Party[0];
                            remainder.Party.Clear();
                            PacketSender.SendParty(remainder.MyClient);
                            PacketSender.SendPlayerMsg(remainder.MyClient, Strings.Parties.disbanded, CustomColors.Error);
                        }
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
                            Strings.Parties.memberleft.ToString(oldMember.Name), CustomColors.Error);
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
                PacketSender.SendPlayerMsg(MyClient, Strings.Parties.left, CustomColors.Error);
            }
            Party.Clear();
            PacketSender.SendParty(MyClient);
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
            PacketSender.StartTrade(target.MyClient, this);
            PacketSender.StartTrade(MyClient, target);
        }

        public byte[] PartyData()
        {
            var bf = new ByteBuffer();
            bf.WriteGuid(base.Id);
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
            if (SpellBase.Get(spell.SpellId) == null) return false;
            for (var i = 0; i < Options.MaxPlayerSkills; i++)
            {
                if (Spells[i].SpellId == Guid.Empty)
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

        public bool KnowsSpell(Guid spellId)
        {
            for (var i = 0; i < Options.MaxPlayerSkills; i++)
            {
                if (Spells[i].SpellId == spellId)
                {
                    return true;
                }
            }
            return false;
        }

        public int FindSpell(Guid spellId)
        {
            for (var i = 0; i < Options.MaxPlayerSkills; i++)
            {
                if (Spells[i].SpellId == spellId)
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
        }

        public void ForgetSpell(int spellSlot)
        {
            if (!SpellBase.Get(Spells[spellSlot].SpellId).Bound)
            {
                Spells[spellSlot].Set(Spell.None);
                PacketSender.SendPlayerSpellUpdate(MyClient, spellSlot);
            }
            else
            {
                PacketSender.SendPlayerMsg(MyClient, Strings.Combat.tryforgetboundspell);
            }
        }

        public void UseSpell(int spellSlot, EntityInstance target)
        {
            var spellNum = Spells[spellSlot].SpellId;
            Target = target;
            if (SpellBase.Get(spellNum) != null)
            {
                var spell = SpellBase.Get(spellNum);

                if (!Conditions.MeetsConditionLists(spell.CastingRequirements, this, null))
                {
                    PacketSender.SendPlayerMsg(MyClient, Strings.Combat.dynamicreq);
                    return;
                }

				//Check if the caster is silenced or stunned. Clense casts break the rule.
				if (spell.Combat.Effect != StatusTypes.Cleanse)
				{
					var statuses = Statuses.Values.ToArray();
					foreach (var status in statuses)
					{
						if (status.Type == StatusTypes.Silence)
						{
							PacketSender.SendPlayerMsg(MyClient, Strings.Combat.silenced);
							return;
						}
						if (status.Type == StatusTypes.Stun)
						{
							PacketSender.SendPlayerMsg(MyClient, Strings.Combat.stunned);
							return;
						}
                        if (status.Type == StatusTypes.Sleep)
                        {
                            PacketSender.SendPlayerMsg(MyClient, Strings.Combat.sleep);
                            return;
                        }
					}
				}

                //Check if the caster has the right ammunition if a projectile
                if (spell.SpellType == SpellTypes.CombatSpell && spell.Combat.TargetType == SpellTargetTypes.Projectile && spell.Combat.ProjectileId != Guid.Empty)
                {
                    var projectileBase = spell.Combat.Projectile;
                    if (projectileBase == null) return;
                    if (projectileBase.AmmoItemId != Guid.Empty)
                    {
                        if (FindItem(projectileBase.AmmoItemId, projectileBase.AmmoRequired) == -1)
                        {
                            PacketSender.SendPlayerMsg(MyClient,
                                Strings.Items.notenough.ToString(ItemBase.GetName(projectileBase.AmmoItemId)),
                                CustomColors.Error);
                            return;
                        }
                    }
                }

                if (target == null && ((spell.SpellType == SpellTypes.CombatSpell && spell.Combat.TargetType == SpellTargetTypes.Single) || spell.SpellType == SpellTypes.WarpTo))
                {
                    PacketSender.SendActionMsg(this, Strings.Combat.notarget, CustomColors.NoTarget);
                    return;
                }

                //Check for range of a single target spell
                if (spell.SpellType == (int)SpellTypes.CombatSpell && spell.Combat.TargetType == SpellTargetTypes.Single && Target != this)
                {
                    if (!InRangeOf(Target, spell.Combat.CastRange))
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
                        if (Spells[spellSlot].SpellCd < Globals.Timing.RealTimeMs)
                        {
                            if (CastTime < Globals.Timing.TimeMs)
                            {
                                CastTime = Globals.Timing.TimeMs + spell.CastDuration;
                                SubVital(Vitals.Mana, spell.VitalCost[(int)Vitals.Mana]);
                                SubVital(Vitals.Health, spell.VitalCost[(int)Vitals.Health]);
                                SpellCastSlot = spellSlot;
                                CastTarget = Target;

                                //Check if the caster has the right ammunition if a projectile
                                if (spell.SpellType == SpellTypes.CombatSpell && spell.Combat.TargetType == SpellTargetTypes.Projectile && spell.Combat.ProjectileId != Guid.Empty)
                                {
                                    var projectileBase = spell.Combat.Projectile;
                                    if (projectileBase != null && projectileBase.AmmoItemId != Guid.Empty)
                                    {
                                        TakeItemsById(projectileBase.AmmoItemId, projectileBase.AmmoRequired);
                                    }
                                }

                                if (spell.CastAnimationId != Guid.Empty)
                                {
                                    PacketSender.SendAnimationToProximity(spell.CastAnimationId, 1, base.Id, MapId, 0, 0, Dir); //Target Type 1 will be global entity
                                }

                                PacketSender.SendEntityVitals(this);

                                //Check if cast should be instance
                                if (Globals.Timing.TimeMs >= CastTime)
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

        public override void CastSpell(Guid spellId, int spellSlot = -1)
        {
            var spellBase = SpellBase.Get(spellId);
            if (spellBase != null)
            {
                if (spellBase.SpellType == SpellTypes.Event)
                {
                    var evt = spellBase.Event;
                    if (evt != null)
                    {
                        StartCommonEvent(evt);
                    }
                    base.CastSpell(spellId, spellSlot); //To get cooldown :P
                }
                else
                {
                    base.CastSpell(spellId, spellSlot);
                }
            }
        }

        //Equipment
        public void UnequipItem(int slot)
        {
            Equipment[slot] = -1;
            FixVitals();
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
            FixVitals();
            PacketSender.SendPlayerEquipmentToProximity(this);
            PacketSender.SendEntityStats(this);
        }

        public void EquipmentProcessItemLoss(int slot)
        {
            for (var i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                if (Equipment[i] == slot)
                    Equipment[i] = -1;
            }
            FixVitals();
            PacketSender.SendPlayerEquipmentToProximity(this);
            PacketSender.SendEntityStats(this);
        }

        //Stats
        public void UpgradeStat(int statIndex)
        {
            if (Stat[statIndex].Stat + StatPointAllocations[statIndex] < Options.MaxStatValue && StatPoints > 0)
            {
                StatPointAllocations[statIndex]++;
                StatPoints--;
                PacketSender.SendEntityStats(this);
                PacketSender.SendPointsTo(MyClient);
            }
        }

        //HotbarSlot
        public void HotbarChange(int index, int type, int slot)
        {
            Hotbar[index].ItemOrSpellId = Guid.Empty;
            Hotbar[index].BagId = Guid.Empty;
            Hotbar[index].PreferredStatBuffs = new int[(int) Stats.StatCount];
            if (type == 0) //Item
            {
                var item = Items[slot];
                if (item != null)
                {
                    Hotbar[index].ItemOrSpellId = item.ItemId;
                    Hotbar[index].BagId = item.BagId ?? Guid.Empty;
                    Hotbar[index].PreferredStatBuffs = item.StatBuffs;
                }
            }
            else if (type == 1) //Spell
            {
                var spell = Spells[slot];
                if (spell != null)
                {
                    Hotbar[index].ItemOrSpellId = spell.SpellId;
                }
            }
        }
        public void HotbarSwap(int index, int swapIndex)
        {
            var itemId = Hotbar[index].ItemOrSpellId;
            var bagId = Hotbar[index].BagId;
            var stats = Hotbar[index].PreferredStatBuffs;

            Hotbar[index].ItemOrSpellId = Hotbar[swapIndex].ItemOrSpellId;
            Hotbar[index].BagId = Hotbar[swapIndex].BagId;
            Hotbar[index].PreferredStatBuffs = Hotbar[swapIndex].PreferredStatBuffs;

            Hotbar[swapIndex].ItemOrSpellId = itemId;
            Hotbar[swapIndex].BagId = bagId;
            Hotbar[swapIndex].PreferredStatBuffs = stats;
        }

        //Quests
        public bool CanStartQuest(QuestBase quest)
        {
            //Check and see if the quest is already in progress, or if it has already been completed and cannot be repeated.
            var questProgress = FindQuest(quest.Id);
            if (questProgress != null)
            {
                if (questProgress.TaskId != Guid.Empty && quest.GetTaskIndex(questProgress.TaskId) != -1)
                {
                    return false;
                }
                if (questProgress.Completed && !quest.Repeatable)
                {
                    return false;
                }
            }
            //So the quest isn't started or we can repeat it.. let's make sure that we meet requirements.
            if (!Conditions.MeetsConditionLists(quest.Requirements, this, null, true, quest)) return false;
            if (quest.Tasks.Count == 0)
            {
                return false;
            }
            return true;
        }

        public bool QuestCompleted(QuestBase quest)
        {
            var questProgress = FindQuest(quest.Id);
            if (questProgress != null)
            {
                if (questProgress.Completed)
                {
                    return true;
                }
            }
            return false;
        }

        public bool QuestInProgress(QuestBase quest, QuestProgress progress, Guid taskId)
        {
            var questProgress = FindQuest(quest.Id);
            if (questProgress != null)
            {
                if (questProgress.TaskId != Guid.Empty && quest.GetTaskIndex(questProgress.TaskId) != -1)
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
                QuestOffers.Add(quest.Id);
                PacketSender.SendQuestOffer(this, quest.Id);
            }
        }

        public Quest FindQuest(Guid questId)
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
                var questProgress = FindQuest(quest.Id);
                if (questProgress != null)
                {
                    questProgress.TaskId = quest.Tasks[0].Id;
                    questProgress.TaskProgress = 0;
                }
                else
                {
                    questProgress = new Quest(quest.Id)
                    {
                        TaskId = quest.Tasks[0].Id,
                        TaskProgress = 0
                    };
                    Quests.Add(questProgress);
                }
                if (quest.Tasks[0].Objective == QuestObjective.GatherItems) //Gather Items
                {
                    UpdateGatherItemQuests(quest.Tasks[0].TargetId);
                }
                StartCommonEvent(EventBase.Get(quest.StartEventId));
                PacketSender.SendPlayerMsg(MyClient, Strings.Quests.started.ToString(quest.Name),
                    CustomColors.QuestStarted);
                PacketSender.SendQuestProgress(this, quest.Id);
            }
        }

        public void AcceptQuest(Guid questId)
        {
            if (QuestOffers.Contains(questId))
            {
                lock (mEventLock)
                {
                    QuestOffers.Remove(questId);
                    var quest = QuestBase.Get(questId);
                    if (quest != null)
                    {
                        StartQuest(quest);
                        foreach (var evt in EventLookup.Values)
                        {
                            if (evt.CallStack.Count <= 0) continue;
                            var stackInfo = evt.CallStack.Peek();
                            if (stackInfo.WaitingForResponse != CommandInstance.EventResponse.Quest) continue;
                            if (((StartQuestCommand)stackInfo.WaitingOnCommand).QuestId == questId)
                            {
                                var tmpStack = new CommandInstance(stackInfo.Page, stackInfo.BranchIds[0]);
                                evt.CallStack.Peek().WaitingForResponse = CommandInstance.EventResponse.None;
                                evt.CallStack.Push(tmpStack);
                            }
                        }
                    }
                }
            }
        }

        public void DeclineQuest(Guid questId)
        {
            if (QuestOffers.Contains(questId))
            {
                lock (mEventLock)
                {
                    QuestOffers.Remove(questId);
                    PacketSender.SendPlayerMsg(MyClient, Strings.Quests.declined.ToString(QuestBase.GetName(questId)),
                        CustomColors.QuestDeclined);
                    foreach (var evt in EventLookup.Values)
                    {
                        if (evt.CallStack.Count <= 0) continue;
                        var stackInfo = evt.CallStack.Peek();
                        if (stackInfo.WaitingForResponse != CommandInstance.EventResponse.Quest) continue;
                        if (((StartQuestCommand)stackInfo.WaitingOnCommand).QuestId == questId)
                        {
                            //Run failure branch
                            var tmpStack = new CommandInstance(stackInfo.Page, stackInfo.BranchIds[1]);
                            stackInfo.WaitingForResponse = CommandInstance.EventResponse.None;
                            evt.CallStack.Push(tmpStack);
                        }
                    }
                }
            }
        }

        public void CancelQuest(Guid questId)
        {
            var quest = QuestBase.Get(questId);
            if (quest != null)
            {
                if (QuestInProgress(quest, QuestProgress.OnAnyTask, Guid.Empty))
                {
                    //Cancel the quest somehow...
                    if (quest.Quitable)
                    {
                        var questProgress = FindQuest(quest.Id);
                        questProgress.TaskId = Guid.Empty;
                        questProgress.TaskProgress = -1;
                        PacketSender.SendPlayerMsg(MyClient,
                            Strings.Quests.abandoned.ToString(QuestBase.GetName(questId)), Color.Red);
                        PacketSender.SendQuestProgress(this, questId);
                    }
                }
            }
        }

        public void CompleteQuestTask(Guid questId, Guid taskId)
        {
            var quest = QuestBase.Get(questId);
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
                                    questProgress.Completed = true;
                                    questProgress.TaskId = Guid.Empty;
                                    questProgress.TaskProgress = -1;
                                    if (quest.Tasks[i].CompletionEvent != null)
                                    {
                                        StartCommonEvent(quest.Tasks[i].CompletionEvent);
                                    }
                                    StartCommonEvent(EventBase.Get(quest.EndEventId));
                                    PacketSender.SendPlayerMsg(MyClient, Strings.Quests.completed.ToString(quest.Name),
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
                                    if (quest.Tasks[i + 1].Objective == QuestObjective.GatherItems)
                                    {
                                        UpdateGatherItemQuests(quest.Tasks[i + 1].TargetId);
                                    }
                                    PacketSender.SendPlayerMsg(MyClient, Strings.Quests.updated.ToString(quest.Name),
                                        CustomColors.TaskUpdated);
                                }
                            }
                        }
                    }
                    PacketSender.SendQuestProgress(this, questId);
                }
            }
        }

        public void CompleteQuest(Guid questId, bool skipCompletionEvent)
        {
            var quest = QuestBase.Get(questId);
            if (quest != null)
            {
                var questProgress = FindQuest(questId);
                if (questProgress != null)
                {
                    //Complete Quest
                    questProgress.Completed = true;
                    questProgress.TaskId = Guid.Empty;
                    questProgress.TaskProgress = -1;
                    if (!skipCompletionEvent)
                    {
                        StartCommonEvent(EventBase.Get(quest.EndEventId));
                        PacketSender.SendPlayerMsg(MyClient, Strings.Quests.completed.ToString(quest.Name), Color.Green);
                    }
                }
            }
        }

        private void UpdateGatherItemQuests(Guid itemId)
        {
            //If any quests demand that this item be gathered then let's handle it
            var item = ItemBase.Get(itemId);
            if (item != null)
            {
                foreach (var questProgress in Quests)
                {
                    var questId = questProgress.QuestId;
                    var quest = QuestBase.Get(questId);
                    if (quest != null)
                    {
                        if (questProgress.TaskId != Guid.Empty)
                        {
                            //Assume this quest is in progress. See if we can find the task in the quest
                            var questTask = quest.FindTask(questProgress.TaskId);
                            if (questTask?.Objective == QuestObjective.GatherItems && questTask.TargetId == item.Id)
                            {
                                if (questProgress.TaskProgress != CountItems(item.Id))
                                {
                                    questProgress.TaskProgress = CountItems(item.Id);
                                    if (questProgress.TaskProgress >= questTask.Quantity)
                                    {
                                        CompleteQuestTask(questId, questProgress.TaskId);
                                    }
                                    else
                                    {
                                        PacketSender.SendQuestProgress(this, quest.Id);
                                        PacketSender.SendPlayerMsg(MyClient, Strings.Quests.itemtask.ToString(quest.Name, questProgress.TaskProgress, questTask.Quantity, ItemBase.GetName(questTask.TargetId)));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        //Switches and Variables
        private Switch GetSwitch(Guid id)
        {
            foreach (var s in Switches)
            {
                if (s.SwitchId == id) return s;
            }
            return null;
        }
        public bool GetSwitchValue(Guid id)
        {
            var s = GetSwitch(id);
            if (s == null) return false;
            return s.Value;
        }
        public void SetSwitchValue(Guid id, bool value)
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
        private Variable GetVariable(Guid id)
        {
            foreach (var v in Variables)
            {
                if (v.VariableId == id) return v;
            }
            return null;
        }
        public long GetVariableValue(Guid id)
        {
            var v = GetVariable(id);
            if (v == null) return 0;
            return v.Value;
        }
        public void SetVariableValue(Guid id, long value)
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
        public EventInstance EventExists(Guid mapId, int x, int y)
        {
            foreach (var evt in EventLookup.Values)
            {
                if (evt.MapId == mapId && evt.BaseEvent.SpawnX == x && evt.BaseEvent.SpawnY == y)
                {
                    return evt;
                }
            }
            return null;
        }

        public EventPageInstance EventAt(Guid mapId, int x, int y, int z)
        {
            foreach (var evt in EventLookup.Values)
            {
                if (evt != null && evt.PageInstance != null)
                {
                    if (evt.PageInstance.MapId == mapId && evt.PageInstance.X == x &&
                        evt.PageInstance.Y == y && evt.PageInstance.Z == z)
                    {
                        return evt.PageInstance;
                    }
                }
            }
            return null;
        }

        public void TryActivateEvent(Guid eventId)
        {
            foreach (var evt in EventLookup.Values)
            {
                if (evt.PageInstance != null && evt.PageInstance.Id == eventId)
                {
                    if (evt.PageInstance.Trigger != EventTrigger.ActionButton) return;
                    if (!IsEventOneBlockAway(evt)) return;
                    if (evt.CallStack.Count != 0) return;
                    var newStack = new CommandInstance(evt.PageInstance.MyPage);
                    evt.CallStack.Push(newStack);
                    if (!evt.Global)
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

        public void RespondToEvent(Guid eventId, int responseId)
        {
            lock (mEventLock)
            {
                foreach (var evt in EventLookup.Values)
                {
                    if (evt.PageInstance != null && evt.PageInstance.Id == eventId)
                    {
                        if (evt.CallStack.Count <= 0) return;
                        var stackInfo = evt.CallStack.Peek();
                        if (stackInfo.WaitingForResponse != CommandInstance.EventResponse.Dialogue) return;
                        stackInfo.WaitingForResponse = CommandInstance.EventResponse.None;
                        if (stackInfo.WaitingOnCommand != null && stackInfo.WaitingOnCommand.Type == EventCommandType.ShowOptions)
                        {
                            var tmpStack = new CommandInstance(stackInfo.Page, stackInfo.BranchIds[responseId - 1]);
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

        public bool StartCommonEvent(EventBase baseEvent, CommonEventTrigger trigger = CommonEventTrigger.None, string command = "", string param = "")
        {
            if (baseEvent == null) return false;
            if (!baseEvent.CommonEvent && baseEvent.MapId != Guid.Empty) return false;
            lock (mEventLock)
            {
                foreach (var evt in EventLookup.Values)
                {
                    if (evt.BaseEvent == baseEvent) return false;
                }
                mCommonEventLaunches++;
                var commonEventLaunch = mCommonEventLaunches;
                //Use Fake Ids for Common Events Since they are not tied to maps and such
                var evtId = Guid.NewGuid();
                var mapId = Guid.Empty;
                var tmpEvent = new EventInstance(evtId, Guid.Empty, MyClient, baseEvent)
                {
                    MapId = mapId,
                    SpawnX = -1,
                    SpawnY = -1
                };
                EventLookup.AddOrUpdate(evtId, tmpEvent, (key, oldValue) => tmpEvent);
                //Try to Spawn a PageInstance.. if we can
                for (var i = baseEvent.Pages.Count - 1; i >= 0; i--)
                {
                    if ((trigger == CommonEventTrigger.None || baseEvent.Pages[i].CommonTrigger == trigger) && Conditions.CanSpawnPage(baseEvent.Pages[i], this, null))
                    {
                        tmpEvent.PageInstance = new EventPageInstance(baseEvent, baseEvent.Pages[i], mapId, tmpEvent, MyClient);
                        tmpEvent.PageIndex = i;
                        //Check for /command trigger
                        if (trigger == CommonEventTrigger.SlashCommand)
                        {
                            if (command.ToLower() == tmpEvent.PageInstance.MyPage.TriggerCommand.ToLower())
                            {
                                var newStack = new CommandInstance(tmpEvent.PageInstance.MyPage);
                                tmpEvent.PageInstance.Param = param;
                                tmpEvent.CallStack.Push(newStack);
                                return true;
                            }
                        }
                        else
                        {
                            var newStack = new CommandInstance(tmpEvent.PageInstance.MyPage);
                            tmpEvent.CallStack.Push(newStack);
                            return true;
                        }
                        break;
                    }
                }
                EventLookup.TryRemove(evtId, out EventInstance z);
                return false;
            }
        }

        public override int CanMove(int moveDir)
        {
            //If crafting or locked by event return blocked 
            if (CraftingTableId != Guid.Empty && CraftId != Guid.Empty)
            {
                return -5;
            }
            foreach (var evt in EventLookup.Values)
            {
                if (evt.HoldingPlayer) return -5;
            }
            return base.CanMove(moveDir);
        }

        protected override int IsTileWalkable(MapInstance map, int x, int y, int z)
        {
            if (base.IsTileWalkable(map, x, y, z) == -1)
            {
                foreach (var evt in EventLookup.Values)
                {
                    if (evt.PageInstance != null)
                    {
                        var instance = evt.PageInstance;
                        if (instance.GlobalClone != null) instance = instance.GlobalClone;
                        if (instance.Map == map && instance.X == x && instance.Y == y && instance.Z == z && !instance.Passable)
                        {
                            return (int) EntityTypes.Event;
                        }
                    }
                }
            }
            return -1;
        }


        public override void Move(int moveDir, Client client, bool dontUpdate = false, bool correction = false)
        {
            var oldMap = MapId;
            client = MyClient;
            base.Move(moveDir, client, dontUpdate, correction);
            // Check for a warp, if so warp the player.
            var attribute = MapInstance.Get(MapId).Attributes[X, Y];
            if (attribute != null && attribute.Type == MapAttributes.Warp)
            {
                var warpAtt = (MapWarpAttribute) attribute;
                if (warpAtt.Direction == WarpDirection.Retain)
                {
                    Warp(warpAtt.MapId, warpAtt.X, warpAtt.Y, Dir);
                }
                else
                {
                    Warp(warpAtt.MapId, warpAtt.X, warpAtt.Y, (int)warpAtt.Direction - 1);
                }
            }

            foreach (var evt in EventLookup.Values)
            {
                if (evt.MapId == MapId)
                {
                    if (evt.PageInstance != null && evt.PageInstance.MapId == MapId)
                    {
                        var x = evt.PageInstance.GlobalClone?.X ?? evt.PageInstance.X;
                        var y = evt.PageInstance.GlobalClone?.Y ?? evt.PageInstance.Y;
                        var z = evt.PageInstance.GlobalClone?.Z ?? evt.PageInstance.Z;
                        if (x == X && y == Y &&  z == Z)
                        {
                            if (evt.PageInstance.Trigger != EventTrigger.PlayerTouch) return;
                            if (evt.CallStack.Count != 0) return;
                            var newStack = new CommandInstance(evt.PageInstance.MyPage);
                            evt.CallStack.Push(newStack);
                        }
                    }
                }
            }
        }
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
