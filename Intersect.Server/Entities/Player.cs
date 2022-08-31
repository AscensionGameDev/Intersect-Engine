using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Threading;

using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Crafting;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Events.Commands;
using Intersect.GameObjects.Maps;
using Intersect.GameObjects.Switches_and_Variables;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Network.Packets.Server;
using Intersect.Server.Database;
using Intersect.Server.Database.Logging.Entities;
using Intersect.Server.Database.PlayerData.Players;
using Intersect.Server.Database.PlayerData.Security;
using Intersect.Server.Entities.Combat;
using Intersect.Server.Entities.Events;
using Intersect.Server.Localization;
using Intersect.Server.Maps;
using Intersect.Server.Networking;
using Intersect.Utilities;

using Newtonsoft.Json;

namespace Intersect.Server.Entities
{

    public partial class Player : Entity
    {
        [NotMapped, JsonIgnore]
        public Guid PreviousMapInstanceId = Guid.Empty;

        //Online Players List
        private static readonly ConcurrentDictionary<Guid, Player> OnlinePlayers = new ConcurrentDictionary<Guid, Player>();

        public static Player[] OnlineList { get; private set; } = new Player[0];

        [NotMapped]
        public bool Online => OnlinePlayers.ContainsKey(Id);

        #region Chat

        [JsonIgnore] [NotMapped] public Player ChatTarget = null;

        #endregion

        [NotMapped, JsonIgnore] public long LastChatTime = -1;

        #region Quests

        [NotMapped, JsonIgnore] public List<Guid> QuestOffers = new List<Guid>();

        #endregion

        #region Event Spawned Npcs

        [JsonIgnore] [NotMapped] public List<Npc> SpawnedNpcs = new List<Npc>();

        #endregion

        public static int OnlineCount => OnlinePlayers.Count;

        [JsonProperty("MaxVitals"), NotMapped]
        public new int[] MaxVitals => GetMaxVitals();

        //Name, X, Y, Dir, Etc all in the base Entity Class
        public Guid ClassId { get; set; }

        [NotMapped]
        public string ClassName => ClassBase.GetName(ClassId);

        public Gender Gender { get; set; }

        public long Exp { get; set; }

        public int StatPoints { get; set; }

        [Column("Equipment"), JsonIgnore]
        public string EquipmentJson
        {
            get => DatabaseUtils.SaveIntArray(Equipment, Options.EquipmentSlots.Count);
            set => Equipment = DatabaseUtils.LoadIntArray(value, Options.EquipmentSlots.Count);
        }

        [NotMapped]
        public int[] Equipment { get; set; } = new int[Options.EquipmentSlots.Count];

        public DateTime? LastOnline { get; set; }

        public DateTime? CreationDate { get; set; } = DateTime.UtcNow;

        private ulong mLoadedPlaytime { get; set; } = 0;

        public ulong PlayTimeSeconds
        {
            get
            {
                return mLoadedPlaytime + (ulong)(LoginTime != null ? (DateTime.UtcNow - (DateTime)LoginTime) : TimeSpan.Zero).TotalSeconds;
            }

            set
            {
                mLoadedPlaytime = value;
            }
        }

        [NotMapped]
        public TimeSpan OnlineTime => LoginTime != null ? DateTime.UtcNow - (DateTime)LoginTime : TimeSpan.Zero;

        [NotMapped]
        public DateTime? LoginTime { get; set; }

        //Bank
        [JsonIgnore]
        public virtual List<BankSlot> Bank { get; set; } = new List<BankSlot>();

        //Friends -- Not used outside of EF
        [JsonIgnore]
        public virtual List<Friend> Friends { get; set; } = new List<Friend>();

        //Local Friends
        [NotMapped, JsonProperty("Friends")]
        public virtual Dictionary<Guid, string> CachedFriends { get; set; } = new Dictionary<Guid, string>();

        //HotBar
        [JsonIgnore]
        public virtual List<HotbarSlot> Hotbar { get; set; } = new List<HotbarSlot>();

        //Quests
        [JsonIgnore]
        public virtual List<Quest> Quests { get; set; } = new List<Quest>();

        //Variables
        [JsonIgnore]
        public virtual List<PlayerVariable> Variables { get; set; } = new List<PlayerVariable>();

        [JsonIgnore, NotMapped]
        public bool IsValidPlayer => !IsDisposed && Client?.Entity == this;

        [NotMapped]
        public long ExperienceToNextLevel => GetExperienceToNextLevel(Level);

        [NotMapped, JsonIgnore]
        public long ClientAttackTimer { get; set; }

        [NotMapped, JsonIgnore]
        public long ClientMoveTimer { get; set; }

        private long mAutorunCommonEventTimer { get; set; }

        [NotMapped, JsonIgnore]
        public int CommonAutorunEvents { get; private set; }

        [NotMapped, JsonIgnore]
        public int MapAutorunEvents { get; private set; }

        /// <summary>
        /// References the in-memory copy of the guild for this player, reference this instead of the Guild property below.
        /// </summary>
        [NotMapped] [JsonIgnore] public Guild Guild { get; set; }

        /// <summary>
        /// This field is used for EF database fields only and should never be assigned to or used, instead the guild instance will be assigned to CachedGuild above
        /// </summary>
        [JsonIgnore] public Guild DbGuild { get; set; }

        [NotMapped]
        [JsonIgnore]
        public Tuple<Player, Guild> GuildInvite { get; set; }

        public int GuildRank { get; set; }

        public DateTime GuildJoinDate { get; set; }

        /// <summary>
        /// Used to determine whether the player is operating in the guild bank vs player bank
        /// </summary>
        [NotMapped] public bool GuildBank;

        // Instancing
        public MapInstanceType InstanceType { get; set; } = MapInstanceType.Overworld;

        [NotMapped, JsonIgnore] public MapInstanceType PreviousMapInstanceType { get; set; } = MapInstanceType.Overworld;

        public Guid PersonalMapInstanceId { get; set; } = Guid.Empty;

        /// <summary>
        /// This instance Id is shared amongst members of a party. Party members will use the shared ID of the party leader.
        /// </summary>
        public Guid SharedMapInstanceId { get; set; } = Guid.Empty;

        /* This bundle of columns exists so that we have a "non-instanced" location to reference in case we need
         * to kick someone out of an instance for any reason */
        [Column("LastOverworldMapId")]
        [JsonProperty]
        public Guid LastOverworldMapId { get; set; }
        [NotMapped]
        [JsonIgnore]
        public MapBase LastOverworldMap
        {
            get => MapBase.Get(LastOverworldMapId);
            set => LastOverworldMapId = value?.Id ?? Guid.Empty;
        }
        public int LastOverworldX { get; set; }
        public int LastOverworldY { get; set; }

        // For respawning in shared instances (configurable option)
        [Column("SharedInstanceRespawnId")]
        [JsonProperty]
        public Guid SharedInstanceRespawnId { get; set; }
        [NotMapped]
        [JsonIgnore]
        public MapBase SharedInstanceRespawn
        {
            get => MapBase.Get(SharedInstanceRespawnId);
            set => SharedInstanceRespawnId = value?.Id ?? Guid.Empty;
        }
        public int SharedInstanceRespawnX { get; set; }
        public int SharedInstanceRespawnY { get; set; }
        public int SharedInstanceRespawnDir { get; set; }

        [NotMapped, JsonIgnore]
        public int InstanceLives { get; set; }

        public static Player FindOnline(Guid id)
        {
            return OnlinePlayers.ContainsKey(id) ? OnlinePlayers[id] : null;
        }

        public static Player FindOnline(string charName)
        {
            return OnlinePlayers.Values.FirstOrDefault(s => s.Name.ToLower().Trim() == charName.ToLower().Trim());
        }

        public bool ValidateLists()
        {
            var changes = false;

            changes |= SlotHelper.ValidateSlots(Spells, Options.MaxPlayerSkills);
            changes |= SlotHelper.ValidateSlots(Items, Options.MaxInvItems);
            changes |= SlotHelper.ValidateSlots(Bank, Options.Instance.PlayerOpts.InitialBankslots);

            if (Hotbar.Count < Options.Instance.PlayerOpts.HotbarSlotCount)
            {
                Hotbar.Sort((a, b) => a?.Slot - b?.Slot ?? 0);
                for (var i = Hotbar.Count; i < Options.Instance.PlayerOpts.HotbarSlotCount; i++)
                {
                    Hotbar.Add(new HotbarSlot(i));
                }

                changes = true;
            }

            return changes;
        }

        private long GetExperienceToNextLevel(int level)
        {
            if (level >= Options.MaxLevel)
            {
                return -1;
            }

            var classBase = ClassBase.Get(ClassId);

            return classBase?.ExperienceToNextLevel(level) ?? ClassBase.DEFAULT_BASE_EXPERIENCE;
        }

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

            if (LoginTime == null)
            {
                LoginTime = DateTime.UtcNow;
            }

            if (User != null && User.LoginTime == null)
            {
                User.LoginTime = DateTime.UtcNow;
            }

            LoadFriends();
            LoadGuild();

            //Upon Sign In Remove Any Items/Spells that have been deleted
            foreach (var itm in Items)
            {
                if (itm.ItemId != Guid.Empty && ItemBase.Get(itm.ItemId) == null)
                {
                    itm.Set(Item.None);
                }
            }

            foreach (var itm in Bank)
            {
                if (itm.ItemId != Guid.Empty && ItemBase.Get(itm.ItemId) == null)
                {
                    itm.Set(Item.None);
                }
            }

            foreach (var spl in Spells)
            {
                if (spl.SpellId != Guid.Empty && SpellBase.Get(spl.SpellId) == null)
                {
                    spl.Set(Spell.None);
                }
            }

            OnlinePlayers[Id] = this;
            OnlineList = OnlinePlayers.Values.ToArray();

            //Send guild list update to all members when coming online
            Guild?.UpdateMemberList();

            // If we are configured to do so, send a notification of us logging in to all online friends.
            if (Options.Player.EnableFriendLoginNotifications)
            {
                foreach (var friend in CachedFriends)
                {
                    var onlineFriend = Player.FindOnline(friend.Key);
                    if (onlineFriend != null)
                    {
                        PacketSender.SendChatMsg(onlineFriend, Strings.Friends.LoggedIn.ToString(Name), ChatMessageType.Friend, CustomColors.Alerts.Info, Name);
                    }
                }
            }
        }

        public void SendPacket(IPacket packet, TransmissionMode mode = TransmissionMode.All)
        {
            Client?.Send(packet, mode);
        }

        public override void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            base.Dispose();
        }

        public void TryLogout(bool force = false, bool softLogout = false)
        {
            LastOnline = DateTime.Now;
            Client = default;

            if (LoginTime != null)
            {
                PlayTimeSeconds += (ulong)(DateTime.UtcNow - (DateTime)LoginTime).TotalSeconds;
                LoginTime = null;
            }

            if (CombatTimer < Timing.Global.Milliseconds || force)
            {
                Logout(softLogout);
            }
        }

        private void Logout(bool softLogout = false)
        {
            if (MapController.TryGetInstanceFromMap(MapId, MapInstanceId, out var instance))
            {
                instance.RemoveEntity(this);
            }

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
                if (t == null || t.GetType() != typeof(Npc))
                {
                    continue;
                }

                if (t.Despawnable)
                {
                    lock (t.EntityLock)
                    {
                        t.Die();
                    }
                }
            }

            SpawnedNpcs.Clear();

            lock (mEventLock)
            {
                EventLookup.Clear();
                EventBaseIdLookup.Clear();
                GlobalPageInstanceLookup.Clear();
                EventTileLookup.Clear();
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
            BankInterface?.Dispose();
            BankInterface = null;
            InShop = null;

            //Clear cooldowns that have expired
            var keys = SpellCooldowns.Keys.ToArray();
            foreach (var key in keys)
            {
                if (SpellCooldowns.TryGetValue(key, out var time) && time < Timing.Global.MillisecondsUtc)
                {
                    SpellCooldowns.TryRemove(key, out _);
                }
            }

            keys = ItemCooldowns.Keys.ToArray();
            foreach (var key in keys)
            {
                if (ItemCooldowns.TryGetValue(key, out var time) && time < Timing.Global.MillisecondsUtc)
                {
                    ItemCooldowns.TryRemove(key, out _);
                }
            }

            PacketSender.SendEntityLeave(this);

            if (!string.IsNullOrWhiteSpace(Strings.Player.left.ToString()))
            {
                PacketSender.SendGlobalMsg(Strings.Player.left.ToString(Name, Options.Instance.GameName));
            }

            //Remvoe this player from the online list
            if (OnlinePlayers?.ContainsKey(Id) ?? false)
            {
                OnlinePlayers.TryRemove(Id, out Player me);
                OnlineList = OnlinePlayers.Values.ToArray();
            }

            //Send guild update to all members when logging out
            Guild?.UpdateMemberList();
            Guild = null;
            GuildBank = false;

            //If our client has disconnected or logged out but we have kept the user logged in due to being in combat then we should try to logout the user now
            if (Client == null)
            {
                User?.TryLogout(softLogout);
            }

            DbInterface.Pool.QueueWorkItem(CompleteLogout);
        }

        public void CompleteLogout()
        {
            User?.Save();

            Dispose();
        }

        //Update
        public override void Update(long timeMs)
        {
            if (!InGame || MapId == Guid.Empty)
            {
                return;
            }

            var lockObtained = false;
            try
            {
                Monitor.TryEnter(EntityLock, ref lockObtained);
                if (lockObtained)
                {
                    if (Client == null) //Client logged out
                    {
                        if (CombatTimer < Timing.Global.Milliseconds)
                        {
                            Logout();

                            return;
                        }
                    }
                    else
                    {
                        if (SaveTimer < Timing.Global.Milliseconds)
                        {
                            var user = User;
                            if (user != null)
                            {
                                DbInterface.Pool.QueueWorkItem(user.Save, false);
                            }
                            SaveTimer = Timing.Global.Milliseconds + Options.Instance.Processing.PlayerSaveInterval;
                        }
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

                    if (mAutorunCommonEventTimer < Timing.Global.Milliseconds)
                    {
                        var autorunEvents = 0;
                        //Check for autorun common events and run them
                        foreach (var obj in EventBase.Lookup)
                        {
                            var evt = obj.Value as EventBase;
                            if (evt != null && evt.CommonEvent)
                            {
                                if (Options.Instance.Metrics.Enable)
                                {
                                    autorunEvents += evt.Pages.Count(p => p.CommonTrigger == CommonEventTrigger.Autorun);
                                }
                                StartCommonEvent(evt, CommonEventTrigger.Autorun);
                            }
                        }

                        mAutorunCommonEventTimer = Timing.Global.Milliseconds + Options.Instance.Processing.CommonEventAutorunStartInterval;
                        CommonAutorunEvents = autorunEvents;
                    }

                    //If we have a move route then let's process it....
                    if (MoveRoute != null && MoveTimer < timeMs)
                    {
                        //Check to see if the event instance is still active for us... if not then let's remove this route
                        var foundEvent = false;
                        foreach (var evt in EventLookup)
                        {
                            if (evt.Value.PageInstance == MoveRouteSetter)
                            {
                                foundEvent = true;
                                if (MoveRoute.ActionIndex < MoveRoute.Actions.Count)
                                {
                                    ProcessMoveRoute(this, timeMs);
                                }
                                else
                                {
                                    if (MoveRoute.Complete && !MoveRoute.RepeatRoute)
                                    {
                                        MoveRoute = null;
                                        MoveRouteSetter = null;
                                        PacketSender.SendMoveRouteToggle(this, false);
                                    }
                                }

                                break;
                            }
                        }

                        if (!foundEvent)
                        {
                            MoveRoute = null;
                            MoveRouteSetter = null;
                            PacketSender.SendMoveRouteToggle(this, false);
                        }
                    }

                    //If we switched maps, lets update the maps
                    if (LastMapEntered != MapId)
                    {
                        if (MapController.TryGetInstanceFromMap(LastMapEntered, MapInstanceId, out var oldMapInstance))
                        {
                            oldMapInstance.RemoveEntity(this);
                        }

                        if (MapId != Guid.Empty)
                        {
                            if (!MapController.Lookup.Keys.Contains(MapId))
                            {
                                WarpToSpawn();
                            }
                            else
                            {
                                if (MapController.TryGetInstanceFromMap(MapId, MapInstanceId, out var newMapInstance))
                                {
                                    newMapInstance.PlayerEnteredMap(this);
                                }
                            }
                        }
                    }

                    var map = MapController.Get(MapId);
                    foreach (var surrMap in map.GetSurroundingMaps(true))
                    {
                        if (surrMap == null)
                        {
                            continue;
                        }

                        MapInstance mapInstance;
                        // If the map does not yet have a MapInstance matching this player's instanceId, create one.
                        lock (EntityLock)
                        {
                            if (!surrMap.TryGetInstance(MapInstanceId, out mapInstance))
                            {
                                surrMap.TryCreateInstance(MapInstanceId, out mapInstance);
                            }
                        }

                        //Check to see if we can spawn events, if already spawned.. update them.
                        lock (mEventLock)
                        {
                            var autorunEvents = 0;
                            foreach (var mapEvent in mapInstance.EventsCache)
                            {
                                if (mapEvent != null)
                                {
                                    //Look for event
                                    var loc = new MapTileLoc(surrMap.Id, mapEvent.SpawnX, mapEvent.SpawnY);
                                    var foundEvent = EventExists(loc);
                                    if (foundEvent == null)
                                    {
                                        var tmpEvent = new Event(Guid.NewGuid(), surrMap, this, mapEvent)
                                        {
                                            Global = mapEvent.Global,
                                            MapId = surrMap.Id,
                                            SpawnX = mapEvent.SpawnX,
                                            SpawnY = mapEvent.SpawnY
                                        };

                                        EventLookup.AddOrUpdate(tmpEvent.Id, tmpEvent, (key, oldValue) => tmpEvent);
                                        EventBaseIdLookup.AddOrUpdate(mapEvent.Id, tmpEvent, (key, oldvalue) => tmpEvent);
                                        //var newTileLookup = new Dictionary<MapTileLoc, Event>(EventTileLookup);
                                        ////If we get a collision here we need to rethink the MapTileLoc struct..
                                        ////We want a fast lookup through this dictionary and this is hopefully a solution over using a slow Tuple.
                                        //newTileLookup.Add(loc, tmpEvent);
                                        //EventTileLookup = newTileLookup;

                                        EventTileLookup.AddOrUpdate(loc, tmpEvent, (key, oldvalue) => tmpEvent);
                                    }
                                    else
                                    {
                                        foundEvent.Update(timeMs, foundEvent.MapController);
                                    }
                                    if (Options.Instance.Metrics.Enable)
                                    {
                                        autorunEvents += mapEvent.Pages.Count(p => p.Trigger == EventTrigger.Autorun);
                                    }
                                }
                            }
                            MapAutorunEvents = autorunEvents;
                        }
                    }

                    //Check to see if we can spawn events, if already spawned.. update them.
                    lock (mEventLock)
                    {
                        foreach (var evt in EventLookup)
                        {
                            if (evt.Value == null)
                            {
                                continue;
                            }

                            var eventFound = false;
                            var eventMap = map;

                            if (evt.Value.MapId != Guid.Empty)
                            {
                                if (evt.Value.MapId != MapId)
                                {
                                    eventMap = evt.Value.MapController;
                                    eventFound = map.SurroundingMapIds.Contains(eventMap.Id);
                                }
                                else
                                {
                                    eventFound = true;
                                }
                            }


                            if (evt.Value.MapId == Guid.Empty)
                            {
                                evt.Value.Update(timeMs, eventMap);
                                if (evt.Value.CallStack.Count > 0)
                                {
                                    eventFound = true;
                                }
                            }


                            if (eventFound)
                            {
                                continue;
                            }

                            RemoveEvent(evt.Value.Id);
                        }
                    }
                }
            }
            finally
            {
                if (lockObtained)
                {
                    Monitor.Exit(EntityLock);
                }
            }
        }

        public void RemoveEvent(Guid id, bool sendLeave = true)
        {
            Event outInstance;
            EventLookup.TryRemove(id, out outInstance);
            if (outInstance != null)
            {
                EventBaseIdLookup.TryRemove(outInstance.BaseEvent.Id, out Event evt);
            }
            if (outInstance != null && outInstance.MapId != Guid.Empty)
            {
                //var newTileLookup = new Dictionary<MapTileLoc, Event>(EventTileLookup);
                //newTileLookup.Remove(new MapTileLoc(outInstance.MapId, outInstance.SpawnX, outInstance.SpawnY));
                //EventTileLookup = newTileLookup;
                EventTileLookup.TryRemove(new MapTileLoc(outInstance.MapId, outInstance.SpawnX, outInstance.SpawnY), out Event val);
            }
            if (outInstance?.PageInstance?.GlobalClone != null)
            {
                GlobalPageInstanceLookup.TryRemove(outInstance.PageInstance.GlobalClone, out Event val);
            }
            if (sendLeave && outInstance != null && outInstance.MapId != Guid.Empty)
            {
                PacketSender.SendEntityLeaveTo(this, outInstance);
            }
        }

        //Sending Data
        public override EntityPacket EntityPacket(EntityPacket packet = null, Player forPlayer = null)
        {
            if (packet == null)
            {
                packet = new PlayerEntityPacket();
            }

            packet = base.EntityPacket(packet, forPlayer);

            var pkt = (PlayerEntityPacket) packet;
            pkt.Gender = Gender;
            pkt.ClassId = ClassId;
            pkt.Stats = GetStatValues();

            if (Power.IsAdmin)
            {
                pkt.AccessLevel = (int) Access.Admin;
            }
            else if (Power.IsModerator)
            {
                pkt.AccessLevel = (int) Access.Moderator;
            }
            else
            {
                pkt.AccessLevel = 0;
            }

            if (CombatTimer > Timing.Global.Milliseconds)
            {
                pkt.CombatTimeRemaining = CombatTimer - Timing.Global.Milliseconds;
            }

            if (forPlayer != null && this is Player thisPlayer)
            {
                ((PlayerEntityPacket) packet).Equipment =
                    PacketSender.GenerateEquipmentPacket(forPlayer, thisPlayer);
            }

            pkt.Guild = Guild?.Name;
            pkt.GuildRank = GuildRank;

            return pkt;
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
            CachedDots = new DoT[0];
            Statuses.Clear();
            CachedStatuses = new Status[0];

            CombatTimer = 0;

            var cls = ClassBase.Get(ClassId);
            if (cls != null)
            {
                WarpToSpawn();
            }
            else
            {
                Warp(Guid.Empty, 0, 0, 0);
            }

            PacketSender.SendEntityDataToProximity(this);

            //Search death common event trigger
            StartCommonEventsWithTrigger(CommonEventTrigger.OnRespawn);
        }

        public override void Die(bool dropItems = true, Entity killer = null)
        {
            CastTime = 0;
            CastTarget = null;

            //Flag death to the client
            PacketSender.SendPlayerDeath(this);

            //Event trigger
            foreach (var evt in EventLookup)
            {
                evt.Value.PlayerHasDied = true;
            }

            // Remove player from ALL threat lists.
            foreach (var instance in MapController.GetSurroundingMapInstances(Map.Id, MapInstanceId, true))
            {
                foreach (var entity in instance.GetCachedEntities())
                {
                    if (entity is Npc npc)
                    {
                        npc.RemoveFromDamageMap(this);
                    }
                }
            }

            lock (EntityLock)
            {
                base.Die(dropItems, killer);
            }


            // EXP Loss - don't lose in shared instance, or in an Arena zone
            if (InstanceType != MapInstanceType.Shared || Options.Instance.Instancing.LoseExpOnInstanceDeath)
            {
                if (Options.Instance.PlayerOpts.ExpLossOnDeathPercent > 0)
                {
                    if (Options.Instance.PlayerOpts.ExpLossFromCurrentExp)
                    {
                        var ExpLoss = (this.Exp * (Options.Instance.PlayerOpts.ExpLossOnDeathPercent / 100.0));
                        TakeExperience((long)ExpLoss);
                    }
                    else
                    {
                        var ExpLoss = (GetExperienceToNextLevel(this.Level) * (Options.Instance.PlayerOpts.ExpLossOnDeathPercent / 100.0));
                        TakeExperience((long)ExpLoss);
                    }
                }
            }
            PacketSender.SendEntityDie(this);
            Reset();
            Respawn();
            PacketSender.SendInventory(this);
        }

        public override void ProcessRegen()
        {
            Debug.Assert(ClassBase.Lookup != null, "ClassBase.Lookup != null");

            var playerClass = ClassBase.Get(ClassId);
            if (playerClass?.VitalRegen == null)
            {
                return;
            }

            foreach (Vitals vital in Enum.GetValues(typeof(Vitals)))
            {
                if (vital >= Vitals.VitalCount)
                {
                    continue;
                }

                var vitalId = (int) vital;
                var vitalValue = GetVital(vital);
                var maxVitalValue = GetMaxVital(vital);
                if (vitalValue >= maxVitalValue)
                {
                    continue;
                }

                var vitalRegenRate = (playerClass.VitalRegen[vitalId] + GetEquipmentVitalRegen(vital)) / 100f;
                var regenValue = (int) Math.Max(1, maxVitalValue * vitalRegenRate) *
                                 Math.Abs(Math.Sign(vitalRegenRate));

                AddVital(vital, regenValue);
            }
        }

        public override int GetMaxVital(int vital)
        {
            var classDescriptor = ClassBase.Get(this.ClassId);
            var classVital = 20;
            if (classDescriptor != null)
            {
                if (classDescriptor.IncreasePercentage)
                {
                    classVital = (int) (classDescriptor.BaseVital[vital] *
                                        Math.Pow(1 + (double) classDescriptor.VitalIncrease[vital] / 100, Level - 1));
                }
                else
                {
                    classVital = classDescriptor.BaseVital[vital] + classDescriptor.VitalIncrease[vital] * (Level - 1);
                }
            }

            var baseVital = classVital;

            // TODO: Alternate implementation for the loop
            //            classVital += Equipment?.Select(equipment => ItemBase.Get(Items.ElementAt(equipment)?.ItemId ?? Guid.Empty))
            //                .Sum(
            //                    itemDescriptor => itemDescriptor.VitalsGiven[vital] +
            //                                      (itemDescriptor.PercentageVitalsGiven[vital] * baseVital) / 100
            //                ) ?? 0;
            // Loop through equipment and see if any items grant vital buffs
            for (var i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                if (Equipment[i] >= 0 && Equipment[i] < Options.MaxInvItems && Equipment[i] < Items.Count)
                {
                    if (Items[Equipment[i]].ItemId != Guid.Empty)
                    {
                        var item = ItemBase.Get(Items[Equipment[i]].ItemId);
                        if (item != null)
                        {
                            classVital += item.VitalsGiven[vital] + item.PercentageVitalsGiven[vital] * baseVital / 100;
                        }
                    }
                }
            }

            //Must have at least 1 hp and no less than 0 mp
            if (vital == (int) Vitals.Health)
            {
                classVital = Math.Max(classVital, 1);
            }
            else if (vital == (int) Vitals.Mana)
            {
                classVital = Math.Max(classVital, 0);
            }

            return classVital;
        }

        public override int GetMaxVital(Vitals vital)
        {
            return GetMaxVital((int) vital);
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
            if (level < 1)
            {
                return;
            }

            Level = Math.Min(Options.MaxLevel, level);
            if (resetExperience)
            {
                Exp = 0;
            }

            RecalculateStatsAndPoints();
            UnequipInvalidItems();
            PacketSender.SendEntityDataToProximity(this);
            PacketSender.SendExperience(this);
        }

        public void LevelUp(bool resetExperience = true, int levels = 1)
        {
            var messages = new List<string>();
            if (Level < Options.MaxLevel)
            {
                for (var i = 0; i < levels; i++)
                {
                    SetLevel(Level + 1, resetExperience);

                    //Let's pull up class - leveling info
                    var classDescriptor = ClassBase.Get(ClassId);
                    if (classDescriptor?.Spells == null)
                    {
                        continue;
                    }

                    foreach (var spell in classDescriptor.Spells)
                    {
                        if (spell.Level != Level)
                        {
                            continue;
                        }

                        var spellInstance = new Spell(spell.Id);
                        if (TryTeachSpell(spellInstance, true))
                        {
                            messages.Add(
                                Strings.Player.spelltaughtlevelup.ToString(SpellBase.GetName(spellInstance.SpellId))
                            );
                        }
                    }
                }
            }

            PacketSender.SendChatMsg(this, Strings.Player.levelup.ToString(Level), ChatMessageType.Experience, CustomColors.Combat.LevelUp, Name);
            PacketSender.SendActionMsg(this, Strings.Combat.levelup, CustomColors.Combat.LevelUp);
            foreach (var message in messages)
            {
                PacketSender.SendChatMsg(this, message, ChatMessageType.Experience, CustomColors.Alerts.Info, Name);
            }

            if (StatPoints > 0)
            {
                PacketSender.SendChatMsg(
                    this, Strings.Player.statpoints.ToString(StatPoints), ChatMessageType.Experience, CustomColors.Combat.StatPoints, Name
                );
            }

            RecalculateStatsAndPoints();
            UnequipInvalidItems();
            PacketSender.SendExperience(this);
            PacketSender.SendPointsTo(this);
            PacketSender.SendEntityDataToProximity(this);

            //Search for level up activated events and run them
            StartCommonEventsWithTrigger(CommonEventTrigger.LevelUp);
        }

        public void GiveExperience(long amount)
        {
            Exp += (int) Math.Round(amount + (amount * (GetEquipmentBonusEffect(EffectType.EXP) / 100f)));
            if (Exp < 0)
            {
                Exp = 0;
            }

            if (!CheckLevelUp())
            {
                PacketSender.SendExperience(this);
            }
        }

        public void TakeExperience(long amount)
        {
            Exp -= amount;
            if (Exp < 0)
            {
                Exp = 0;
            }

            PacketSender.SendExperience(this);
        }

        private bool CheckLevelUp()
        {
            var levelCount = 0;
            while (Exp >= GetExperienceToNextLevel(Level + levelCount) &&
                   GetExperienceToNextLevel(Level + levelCount) > 0)
            {
                Exp -= GetExperienceToNextLevel(Level + levelCount);
                levelCount++;
            }

            if (levelCount <= 0)
            {
                return false;
            }

            LevelUp(false, levelCount);

            return true;
        }

        //Combat
        public override void KilledEntity(Entity entity)
        {
            switch (entity)
            {
                case Npc npc:
                    {
                        var descriptor = npc.Base;
                        var playerEvent = descriptor.OnDeathEvent;
                        var partyEvent = descriptor.OnDeathPartyEvent;

                        // If in party, split the exp.
                        if (Party != null && Party.Count > 0)
                        {
                            var partyMembersInXpRange = Party.Where(partyMember => partyMember.InRangeOf(this, Options.Party.SharedXpRange)).ToArray();
                            float bonusExp = Options.Instance.PartyOpts.BonusExperiencePercentPerMember / 100;
                            var multiplier = 1.0f + (partyMembersInXpRange.Length * bonusExp);
                            var partyExperience = (int)(descriptor.Experience * multiplier) / partyMembersInXpRange.Length;
                            foreach (var partyMember in partyMembersInXpRange)
                            {
                                partyMember.GiveExperience(partyExperience);
                                partyMember.UpdateQuestKillTasks(entity);
                            }

                            if (partyEvent != null)
                            {
                                foreach (var partyMember in Party)
                                {
                                    if ((Options.Party.NpcDeathCommonEventStartRange <= 0 || partyMember.InRangeOf(this, Options.Party.NpcDeathCommonEventStartRange)) && !(partyMember == this && playerEvent != null))
                                    {
                                        partyMember.StartCommonEvent(partyEvent);
                                    }
                                }
                            }
                        }
                        else
                        {
                            GiveExperience(descriptor.Experience);
                            UpdateQuestKillTasks(entity);
                        }

                        if (playerEvent != null)
                        {
                            StartCommonEvent(playerEvent);
                        }

                        break;
                    }

                case Resource resource:
                    {
                        var descriptor = resource.Base;
                        if (descriptor?.Event != null)
                        {
                            StartCommonEvent(descriptor.Event);
                        }

                        break;
                    }
            }
        }

        public void UpdateQuestKillTasks(Entity en)
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
                                    PacketSender.SendQuestsProgress(this);
                                    PacketSender.SendChatMsg(
                                        this,
                                        Strings.Quests.npctask.ToString(
                                            quest.Name, questProgress.TaskProgress, questTask.Quantity,
                                            NpcBase.GetName(questTask.TargetId)
                                        ),
                                        ChatMessageType.Quest
                                    );
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void TryAttack(
            Entity target,
            ProjectileBase projectile,
            SpellBase parentSpell,
            ItemBase parentItem,
            byte projectileDir
        )
        {
            if (!CanAttack(target, parentSpell))
            {
                return;
            }

            //If Entity is resource, check for the correct tool and make sure its not a spell cast.
            if (target is Resource resource)
            {
                if (resource.IsDead())
                {
                    return;
                }

                // We don't here deal in them fancy projectile tools o'er in dis town!
                if (parentSpell != null || projectile != null)
                {
                    return;
                }

                // Check that a resource is actually required.
                var descriptor = resource.Base;

                //Check Dynamic Requirements
                if (!Conditions.MeetsConditionLists(descriptor.HarvestingRequirements, this, null))
                {
                    if (!string.IsNullOrWhiteSpace(descriptor.CannotHarvestMessage))
                    {
                        PacketSender.SendChatMsg(this, descriptor.CannotHarvestMessage, ChatMessageType.Error);
                    }
                    else
                    {
                        PacketSender.SendChatMsg(this, Strings.Combat.resourcereqs, ChatMessageType.Error);
                    }

                    return;
                }

                if (descriptor.Tool > -1 && descriptor.Tool < Options.ToolTypes.Count)
                {
                    if (parentItem == null || descriptor.Tool != parentItem.Tool)
                    {
                        PacketSender.SendChatMsg(
                            this, Strings.Combat.toolrequired.ToString(Options.ToolTypes[descriptor.Tool]), ChatMessageType.Error
                        );

                        return;
                    }
                }
            }

            base.TryAttack(target, projectile, parentSpell, parentItem, projectileDir);
        }

        public void TryAttack(Entity target)
        {
            if (IsCasting)
            {
                if (Options.Combat.EnableCombatChatMessages)
                {
                    PacketSender.SendChatMsg(this, Strings.Combat.channelingnoattack, ChatMessageType.Combat);
                }

                return;
            }

            if (!IsOneBlockAway(target))
            {
                return;
            }

            if (!CanAttack(target, null))
            {
                return;
            }

            if (target is EventPage)
            {
                return;
            }

            ItemBase weapon = null;
            if (Options.WeaponIndex > -1 &&
                Options.WeaponIndex < Equipment.Length &&
                Equipment[Options.WeaponIndex] >= 0)
            {
                weapon = ItemBase.Get(Items[Equipment[Options.WeaponIndex]].ItemId);
            }

            //If Entity is resource, check for the correct tool and make sure its not a spell cast.
            if (target is Resource resource)
            {
                if (resource.IsDead())
                {
                    return;
                }

                // Check that a resource is actually required.
                var descriptor = resource.Base;

                //Check Dynamic Requirements
                if (!Conditions.MeetsConditionLists(descriptor.HarvestingRequirements, this, null))
                {
                    if (!string.IsNullOrWhiteSpace(descriptor.CannotHarvestMessage))
                    {
                        PacketSender.SendChatMsg(this, descriptor.CannotHarvestMessage, ChatMessageType.Error);
                    }
                    else
                    {
                        PacketSender.SendChatMsg(this, Strings.Combat.resourcereqs, ChatMessageType.Error);
                    }

                    return;
                }

                if (descriptor.Tool > -1 && descriptor.Tool < Options.ToolTypes.Count)
                {
                    if (weapon == null || descriptor.Tool != weapon.Tool)
                    {
                        PacketSender.SendChatMsg(
                            this, Strings.Combat.toolrequired.ToString(Options.ToolTypes[descriptor.Tool]), ChatMessageType.Error
                        );

                        return;
                    }
                }
            }

            if (weapon != null)
            {
                base.TryAttack(
                    target, weapon.Damage, (DamageType) weapon.DamageType, (Stats) weapon.ScalingStat, weapon.Scaling,
                    weapon.CritChance, weapon.CritMultiplier, null, null, weapon
                );
            }
            else
            {
                var classBase = ClassBase.Get(ClassId);
                if (classBase != null)
                {
                    base.TryAttack(
                        target, classBase.Damage, (DamageType) classBase.DamageType, (Stats) classBase.ScalingStat,
                        classBase.Scaling, classBase.CritChance, classBase.CritMultiplier
                    );
                }
                else
                {
                    base.TryAttack(target, 1, DamageType.Physical, Stats.Attack, 100, 10, 1.5);
                }
            }
        }

        public override bool CanAttack(Entity entity, SpellBase spell)
        {
            // If self-cast, AoE, Projectile or Dash.. always accept.
            if (spell?.Combat?.TargetType == SpellTargetTypes.Self ||
                spell?.Combat?.TargetType == SpellTargetTypes.Projectile ||
                spell?.SpellType == SpellTypes.Dash
                )
            {
                return true;
            }

            if (!base.CanAttack(entity, spell))
            {
                return false;
            }

            if (entity is EventPage)
            {
                return false;
            }

            var friendly = spell?.Combat != null && spell.Combat.Friendly;
            if (entity is Player player)
            {
                if (player.InParty(this) || this == player || (!Options.Instance.Guild.AllowGuildMemberPvp && friendly != (player.Guild != null && player.Guild == this.Guild)))
                {
                    return friendly;
                }

                // Only count safe zones and friendly fire if its a dangerous spell! (If one has been used)
                // Projectiles are ignored here, because we can always fire those.. Whether they hit or not is a problem for later.
                if (!friendly && (spell?.Combat?.TargetType != SpellTargetTypes.Self && spell?.Combat?.TargetType != SpellTargetTypes.AoE && spell?.SpellType == SpellTypes.CombatSpell))
                {
                    // Check if either the attacker or the defender is in a "safe zone" (Only apply if combat is PVP)
                    if (MapController.Get(MapId).ZoneType == MapZones.Safe || MapController.Get(player.MapId).ZoneType == MapZones.Safe)
                    {
                        return false;
                    }

                    // Also consider this an issue if either player is in a different map zone type.
                    if (MapController.Get(MapId).ZoneType != MapController.Get(player.MapId).ZoneType)
                    {
                        return false;
                    }

                }
            }

            if (entity is Resource)
            {
                if (spell != null)
                {
                    return false;
                }
            }

            if (entity is Npc npc)
            {
                return !friendly && npc.CanPlayerAttack(this) || friendly && npc.IsAllyOf(this);
            }

            return true;
        }

        public override void NotifySwarm(Entity attacker)
        {
            var mapController = MapController.Get(MapId);
            if (mapController == null) return;

            if (MapController.TryGetInstanceFromMap(MapId, MapInstanceId, out var instance))
            {
                instance.GetEntities(true).ForEach(
                    entity =>
                    {
                        if (entity is Npc npc &&
                            npc.Target == null &&
                            npc.IsAllyOf(this) &&
                            InRangeOf(npc, npc.Base.SightRange))
                        {
                            npc.AssignTarget(attacker);
                        }
                    }
                );
            }
        }

        public override int CalculateAttackTime()
        {
            ItemBase weapon = null;
            var attackTime = base.CalculateAttackTime();

            var cls = ClassBase.Get(ClassId);
            if (cls != null && cls.AttackSpeedModifier == 1) //Static
            {
                attackTime = cls.AttackSpeedValue;
            }

            if (Options.WeaponIndex > -1 &&
                Options.WeaponIndex < Equipment.Length &&
                Equipment[Options.WeaponIndex] >= 0)
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

            return
                attackTime -
                60; //subtracting 60 to account for a moderate ping to the server so some attacks dont get cancelled.
        }

        /// <summary>
        /// Get all StatBuffs for the relevant <see cref="Stats"/>
        /// </summary>
        /// <param name="statType">The <see cref="Stats"/> to retrieve the amounts for.</param>
        /// <returns>Returns a <see cref="Tuple"/> containing the Flat stats on Item1, and Percentage stats on Item2</returns>
        public Tuple<int, int> GetItemStatBuffs(Stats statType)
        {
            var flatStats = 0;
            var percentageStats = 0;

            //Add up player equipment values
            for (var i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                var equipment = Equipment[i];
                if (equipment >= 0 && equipment < Items.Count)
                {
                    var item = Items[equipment];
                    if (item.ItemId != Guid.Empty)
                    {
                        var descriptor = ItemBase.Get(item.ItemId);
                        if (descriptor != null)
                        {
                            flatStats += descriptor.StatsGiven[(int)statType] + item.StatBuffs[(int)statType];
                            percentageStats += descriptor.PercentageStatsGiven[(int)statType];
                        }
                    }
                }
            }

            return new Tuple<int, int>(flatStats, percentageStats);
        }

        public void RecalculateStatsAndPoints()
        {
            var playerClass = ClassBase.Get(ClassId);

            if (playerClass != null)
            {
                for (var i = 0; i < (int) Stats.StatCount; i++)
                {
                    var s = playerClass.BaseStat[i];

                    //Add class stat scaling
                    if (playerClass.IncreasePercentage) //% increase per level
                    {
                        s = (int) (s * Math.Pow(1 + (double) playerClass.StatIncrease[i] / 100, Level - 1));
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
                    StatPoints += expectedPoints - currentPoints;
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
                        if (i >= (int) Stats.StatCount)
                        {
                            i = 0;
                        }
                    }
                }
            }
        }

        //Warping
        public override void Warp(Guid newMapId, float newX, float newY, bool adminWarp = false)
        {
            Warp(newMapId, newX, newY, (byte)Directions.Up, adminWarp, 0, false);
        }

        public void AdminWarp(Guid newMapId, float newX, float newY, Guid newMapInstanceId, MapInstanceType instanceType, bool force)
        {
            PreviousMapInstanceId = MapInstanceId;
            PreviousMapInstanceType = InstanceType;

            MapInstanceId = newMapInstanceId;
            InstanceType = instanceType;
            // If we've warped the player out of their overworld, keep a reference to their overworld just in case.
            if (PreviousMapInstanceType == MapInstanceType.Overworld)
            {
                UpdateLastOverworldLocation(MapId, X, Y);
            }
            if (PreviousMapInstanceId != MapInstanceId)
            {
                PacketSender.SendChatMsg(this, Strings.Player.InstanceUpdate.ToString(PreviousMapInstanceId.ToString(), MapInstanceId.ToString()), ChatMessageType.Admin, CustomColors.Alerts.Info);
            }
            Warp(newMapId, newX, newY, (byte)Directions.Up, forceInstanceChange: force);
        }

        public override void Warp(
            Guid newMapId,
            float newX,
            float newY,
            byte newDir,
            bool adminWarp = false,
            byte zOverride = 0,
            bool mapSave = false,
            bool fromWarpEvent = false,
            MapInstanceType? mapInstanceType = null,
            bool fromLogin = false,
            bool forceInstanceChange = false
        )
        {
            #region shortcircuit exits
            // First, deny the warp entirely if we CAN'T, for some reason, warp to the requested instance type. ONly do this if we're not forcing a change
            if (!forceInstanceChange && !CanChangeToInstanceType(mapInstanceType, fromLogin, newMapId))
            {
                return;
            }
            #endregion

            // If we are leaving the overworld to go to a new instance, save the overworld location
            if (!fromLogin && InstanceType == MapInstanceType.Overworld && mapInstanceType != MapInstanceType.Overworld && mapInstanceType != null)
            {
                UpdateLastOverworldLocation(MapId, X, Y);
            }
            // If we are moving TO a new shared instance, update the shared respawn point (if enabled)
            if (!fromLogin && mapInstanceType == MapInstanceType.Shared && Options.Instance.Instancing.SharedInstanceRespawnInInstance && MapController.Get(newMapId) != null)
            {
                UpdateSharedInstanceRespawnLocation(newMapId, (int)newX, (int)newY, (int)newDir);
            }

            // Make sure we're heading to a map that exists - otherwise, to spawn you go
            var newMap = MapController.Get(newMapId);
            if (newMap == null)
            {
                WarpToSpawn();

                return;
            }

            X = (int)newX;
            Y = (int)newY;
            Z = zOverride;
            Dir = newDir;

            var newSurroundingMaps = newMap.GetSurroundingMapIds(true);

            #region Map instance traversal
            // Set up player properties if we have changed instance types
            bool onNewInstance = forceInstanceChange || ProcessMapInstanceChange(mapInstanceType, fromLogin);

            // Ensure there exists a map instance with the Player's InstanceId. A player is the sole entity that can create new map instances
            MapInstance newMapInstance;
            lock (EntityLock)
            {
                if (!newMap.TryGetInstance(MapInstanceId, out newMapInstance))
                {
                    // Create a new instance for the map we're on
                    newMap.TryCreateInstance(MapInstanceId, out newMapInstance);
                    foreach (var surrMap in newSurroundingMaps)
                    {
                        MapController.Get(surrMap).TryCreateInstance(MapInstanceId, out var surrMapInstance);
                    }
                }
            }

            // An instance of the map MUST exist. Otherwise, head to spawn.
            if (newMapInstance == null)
            {
                Log.Error($"Player {Name} requested a new map Instance with ID {MapInstanceId} and failed to get it.");
                WarpToSpawn();

                return;
            }

            // If we've changed instances, send data to instance entities/entities to player
            if (onNewInstance || forceInstanceChange)
            {
                SendToNewMapInstance(newMap);
                // Clear all events - get fresh ones from the new instance to re-fresh event locations
                foreach (var evt in EventLookup)
                {
                    RemoveEvent(evt.Value.Id, false);
                }
            }
            else
            {
                // Clear events that are no longer on a surrounding map.
                foreach (var evt in EventLookup)
                {
                    // Remove events that aren't relevant (on a surrounding map) anymore
                    if (evt.Value.MapId != Guid.Empty && (!newSurroundingMaps.Contains(evt.Value.MapId) || mapSave))
                    {
                        RemoveEvent(evt.Value.Id, false);
                    }
                }
            }
            #endregion

            if (newMapId != MapId || mSentMap == false) // Player warped to a new map?
            {
                // Remove the entity from the old map instance
                var oldMap = MapController.Get(MapId);
                if (oldMap != null && oldMap.TryGetInstance(PreviousMapInstanceId, out var oldMapInstance))
                {
                    oldMapInstance.RemoveEntity(this);
                }

                PacketSender.SendEntityLeave(this); // We simply changed maps - leave the old one
                MapId = newMapId;
                newMapInstance.PlayerEnteredMap(this);
                PacketSender.SendEntityPositionToAll(this);

                //If map grid changed then send the new map grid
                if (!adminWarp && (oldMap == null || !oldMap.SurroundingMapIds.Contains(newMapId)) || fromLogin)
                {
                    PacketSender.SendMapGrid(this.Client, newMap.MapGrid, true);
                }

                mSentMap = true;

                StartCommonEventsWithTrigger(CommonEventTrigger.MapChanged);
            }
            else // Player moved on same map?
            {
                if (onNewInstance)
                {
                    // But instance changed? Add player to the new instance (will also send stats thru SendEntityDataToProximity)
                    newMapInstance.PlayerEnteredMap(this);
                }
                else
                {
                    PacketSender.SendEntityStats(this);
                }
                PacketSender.SendEntityPositionToAll(this);
            }
        }

        /// <summary>
        /// Warps the player on login, taking care of instance management depending on the instance type the player
        /// is attempting to login to.
        /// </summary>
        public void LoginWarp()
        {
            if (MapId == null || MapId == Guid.Empty)
            {
                WarpToSpawn();
            }
            else
            {
                if (!CanChangeToInstanceType(InstanceType, true, MapId))
                {
                    WarpToLastOverworldLocation(true);
                }
                else
                {
                    // Will warp to spawn if we fail to create an instance for the relevant map
                    Warp(
                        MapId, (byte)X, (byte)Y, (byte)Dir, false, (byte)Z, false, false, InstanceType, true
                    );
                }
            }
        }

        /// <summary>
        /// Warps the player to the last location they were at on the "Overworld" (empty Guid) map instance. Useful for kicking out of
        /// instances in a variety of situations.
        /// </summary>
        /// <param name="fromLogin">Whether or not we're coming to this method via the player login/join game flow</param>
        public void WarpToLastOverworldLocation(bool fromLogin)
        {
            Warp(
                LastOverworldMapId, (byte)LastOverworldX, (byte)LastOverworldY, (byte)Dir, zOverride: (byte)Z, mapInstanceType: MapInstanceType.Overworld, fromLogin: fromLogin
            );
        }

        public void SendLivesRemainingMessage()
        {
            if (InstanceLives > 0)
            {
                PacketSender.SendChatMsg(this, Strings.Parties.InstanceLivesRemaining.ToString(InstanceLives), ChatMessageType.Party, CustomColors.Chat.PartyChat);
            }
            else
            {
                PacketSender.SendChatMsg(this, Strings.Parties.NoMoreLivesRemaining, ChatMessageType.Party, CustomColors.Chat.PartyChat);
            }
        }

        public void WarpToSpawn(bool forceClassRespawn = false)
        {
            var mapId = Guid.Empty;
            byte x = 0;
            byte y = 0;
            byte dir = 0;

            if (Options.Instance.Instancing.SharedInstanceRespawnInInstance && InstanceType == MapInstanceType.Shared && !forceClassRespawn)
            {
                if (SharedInstanceRespawn == null)
                {
                    // invalid map - try overworld (which will throw to class spawn if itself is invalid)
                    WarpToLastOverworldLocation(false);
                    return;
                }

                if (Options.Instance.Instancing.MaxSharedInstanceLives <= 0) // User has not configured shared instances to have lives
                {
                    // Warp to the start of the shared instance - no concern for life total
                    Warp(SharedInstanceRespawnId, SharedInstanceRespawnX, SharedInstanceRespawnY, (Byte)SharedInstanceRespawnDir);
                    return;
                }

                // Check if the player/party have enough lives to spawn in-instance
                if (InstanceLives > 0)
                {
                    // If they do, subtract from this player's life total...
                    InstanceLives--;
                    SendLivesRemainingMessage();
                    // And the totals from any party members
                    if (Party != null && Party.Count > 1)
                    {
                        foreach (Player member in Party)
                        {
                            if (member.Id != Id)
                            {
                                // Keep party member instance lives in sync
                                member.InstanceLives--;
                                if (member.InstanceType == MapInstanceType.Shared)
                                {
                                    member.SendLivesRemainingMessage();
                                }
                            }
                        }
                    }

                    // And warp to the instance start
                    Warp(SharedInstanceRespawnId, SharedInstanceRespawnX, SharedInstanceRespawnY, (byte)SharedInstanceRespawnDir);
                }
                else
                {
                    // The player has ran out of lives - too bad, back to instance entrance you go.
                    if (!Options.Instance.Instancing.BootAllFromInstanceWhenOutOfLives || Party == null || Party.Count < 2)
                    {
                        WarpToLastOverworldLocation(false);
                    }
                    else
                    {
                        // Oh shit, hard mode enabled - boot ALL party members out of instance. No more lives.
                        foreach (Player member in Party)
                        {
                            // Only warp players in the instance
                            if (member.InstanceType == MapInstanceType.Shared)
                            {
                                lock (EntityLock)
                                {
                                    member.WarpToLastOverworldLocation(false);
                                    PacketSender.SendChatMsg(member, Strings.Parties.InstanceFailed, ChatMessageType.Party, CustomColors.Chat.PartyChat);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                var cls = ClassBase.Get(ClassId);
                if (cls != null)
                {
                    if (MapController.Lookup.Keys.Contains(cls.SpawnMapId))
                    {
                        mapId = cls.SpawnMapId;
                    }

                    x = (byte)cls.SpawnX;
                    y = (byte)cls.SpawnY;
                    dir = (byte)cls.SpawnDir;
                }

                if (mapId == Guid.Empty)
                {
                    using (var mapenum = MapController.Lookup.GetEnumerator())
                    {
                        mapenum.MoveNext();
                        mapId = mapenum.Current.Value.Id;
                    }
                }

                Warp(mapId, x, y, dir, false, 0, false, false, MapInstanceType.Overworld);
            }
        }

        // Instancing

        /// <summary>
        /// Checks to see if we CAN go to the requested instance type
        /// </summary>
        /// <param name="instanceType">The instance type we're requesting a warp to</param>
        /// <param name="fromLogin">Whether or not this is from the login flow</param>
        /// <param name="newMapId">The map ID we will be warping to</param>
        /// <returns></returns>
        public bool CanChangeToInstanceType(MapInstanceType? instanceType, bool fromLogin, Guid newMapId)
        {
            bool isValid = true;

            switch (instanceType)
            {
                case MapInstanceType.Guild:
                    if (Guild == null)
                    {
                        isValid = false;

                        if (fromLogin)
                        {
                            PacketSender.SendChatMsg(this, Strings.Guilds.NoLongerAllowedInInstance, ChatMessageType.Guild, CustomColors.Alerts.Error);
                        }
                        else
                        {
                            PacketSender.SendChatMsg(this, Strings.Guilds.NotAllowedInInstance, ChatMessageType.Guild, CustomColors.Alerts.Error);
                        }
                    }
                    break;
                case MapInstanceType.Shared:
                    if (fromLogin)
                    {
                        isValid = false;
                    }
                    if (Party != null && Party.Count > 0 && !Options.Instance.Instancing.RejoinableSharedInstances) // Always valid warp if solo/instances are rejoinable
                    {
                        if (Party[0].Id == Id) // if we are the party leader
                        {
                            // And other players are using our shared instance, deny creation of a new instance until they are finished.
                            if (Party.FindAll((Player member) => member.Id != Id && member.InstanceType == MapInstanceType.Shared).Count > 0)
                            {
                                isValid = false;
                                PacketSender.SendChatMsg(this, Strings.Parties.InstanceInUse, ChatMessageType.Party, CustomColors.Alerts.Error);
                            }
                        }
                        else
                        {
                            // Otherwise, if the party leader hasn't yet created a shared instance, deny creation of a new one.
                            if (Party[0].InstanceType != MapInstanceType.Shared)
                            {
                                isValid = false;
                                PacketSender.SendChatMsg(this, Strings.Parties.CannotCreateInstance, ChatMessageType.Party, CustomColors.Alerts.Error);
                            }
                            else if (Party[0].SharedMapInstanceId != SharedMapInstanceId)
                            {
                                isValid = false;
                                PacketSender.SendChatMsg(this, Strings.Parties.InstanceInProgress, ChatMessageType.Party, CustomColors.Alerts.Error);
                            }
                            else if (newMapId != Party[0].SharedInstanceRespawn.Id)
                            {
                                isValid = false;
                                PacketSender.SendChatMsg(this, Strings.Parties.WrongInstance, ChatMessageType.Party, CustomColors.Alerts.Error);
                            }
                        }
                    }
                    break;
            }

            return isValid;
        }

        /// <summary>
        /// In charge of sending the necessary packet information on an instance change
        /// </summary>
        /// <param name="newMap">The <see cref="MapController"/> we are warping to</param>
        private void SendToNewMapInstance(MapController newMap)
        {
            // Refresh the client's entity list
            var oldMap = MapController.Get(MapId);
            // Get the entities from the old map - we need to clear them off the player's global entities on their client
            if (oldMap != null && oldMap.TryGetInstance(PreviousMapInstanceId, out var oldMapInstance))
            {
                PacketSender.SendMapInstanceChangedPacket(this, oldMap, PreviousMapInstanceId);
                oldMapInstance.ClearEntityTargetsOf(this); // Remove targets of this entity
            }
            // Clear events - we'll get them again from the map instance's event cache
            EventTileLookup.Clear();
            EventLookup.Clear();
            EventBaseIdLookup.Clear();
            Log.Debug($"Player {Name} has joined instance {MapInstanceId} of map: {newMap.Name}");
            Log.Info($"Previous instance was {PreviousMapInstanceId}");
            // We changed maps AND instance layers - remove from the old instance
            PacketSender.SendEntityLeaveInstanceOfMap(this, oldMap.Id, PreviousMapInstanceId);
            // Remove any trace of our player from the old instance's processing
            newMap.RemoveEntityFromAllSurroundingMapsInInstance(this, PreviousMapInstanceId);
        }

        /// <summary>
        /// Checks to see if the <see cref="MapInstanceType"/> we're warping to is different than what type we are currently
        /// on, and, if so, takes care of updating our instance settings.
        /// </summary>
        /// <param name="mapInstanceType">The <see cref="MapInstanceType"/> the player is currently on</param>
        /// <param name="fromLogin">Whether or not we're coming to this method via a login warp.</param>
        /// <returns></returns>
        public bool ProcessMapInstanceChange(MapInstanceType? mapInstanceType, bool fromLogin)
        {
            // Save values before change for reference/emergency recall
            PreviousMapInstanceId = MapInstanceId;
            PreviousMapInstanceType = InstanceType;
            if (mapInstanceType != null) // If we're requesting an instance type change
            {
                // Update our saved instance type - this helps us determine what to do on login, warps, etc
                InstanceType = (MapInstanceType) mapInstanceType;
                // Requests a new instance id, using the type of instance to determine creation logic
                MapInstanceId = CreateNewInstanceIdFromType(mapInstanceType, fromLogin);
            }
            return MapInstanceId != PreviousMapInstanceId;
        }

        /// <summary>
        /// Creates an instance id based on the type of instance we are heading to, and whether or not we should generate a fresh id or use a saved id.
        /// </summary>
        /// <remarks>
        /// Note that if we are coming to this method, we have already checked to see whether or not we CAN go to the requested instance.
        /// </remarks>
        /// <param name="mapInstanceType">The <see cref="MapInstanceType"/> we are switching to</param>
        /// <param name="fromLogin">Whether or not we are coming to this method via player login. We may prefer to use saved values instead of generate new
        /// values if this is the case.</param>
        /// <returns></returns>
        public Guid CreateNewInstanceIdFromType(MapInstanceType? mapInstanceType, bool fromLogin)
        {
            Guid newMapLayerId = MapInstanceId;
            switch (mapInstanceType)
            {
                case MapInstanceType.Overworld:
                    ResetSavedInstanceIds();
                    newMapLayerId = Guid.Empty;
                    break;
                case MapInstanceType.Personal:
                    if (!fromLogin) // If we're logging into a personal instance, we want to login to the SAME instance.
                    {
                        PersonalMapInstanceId = Guid.NewGuid();
                    }
                    newMapLayerId = PersonalMapInstanceId;
                    break;
                case MapInstanceType.Guild:
                    if (Guild != null)
                    {
                        newMapLayerId = Guild.GuildInstanceId;
                    }
                    else
                    {
                        Log.Error($"Player {Name} requested a guild warp with no guild, and proceeded to warp to map anyway");
                        newMapLayerId = Guid.Empty;
                    }
                    break;
                case MapInstanceType.Shared:
                    bool isSolo = Party == null || Party.Count < 2;
                    bool isPartyLeader = Party != null && Party.Count > 0 && Party[0].Id == Id;

                    if (isSolo) // Solo instance initialization
                    {
                        if (Options.Instance.Instancing.MaxSharedInstanceLives > 0)
                        {
                            InstanceLives = Options.Instance.Instancing.MaxSharedInstanceLives;
                        }
                        SharedMapInstanceId = Guid.NewGuid();
                        newMapLayerId = SharedMapInstanceId;
                    }
                    else if (!Options.Instance.Instancing.RejoinableSharedInstances && isPartyLeader) // Non-rejoinable instance initialization
                    {
                        // Generate a new instance
                        SharedMapInstanceId = Guid.NewGuid();
                        // If we are the leader, propogate your shared instance ID to all current members of the party.
                        if (isPartyLeader && !Options.Instance.Instancing.RejoinableSharedInstances)
                        {
                            foreach (Player member in Party)
                            {
                                member.SharedMapInstanceId = SharedMapInstanceId;
                                if (Options.Instance.Instancing.MaxSharedInstanceLives > 0)
                                {
                                    member.InstanceLives = Options.Instance.Instancing.MaxSharedInstanceLives;
                                }
                            }
                        }
                    }
                    else if (Party != null && Party.Count > 0 && Options.Instance.Instancing.RejoinableSharedInstances) // Joinable instance initialization
                    {
                        // Scan party members for an active shared instance - if one is found, use it
                        var memberInInstance = Party.Find((Player member) => member.SharedMapInstanceId != Guid.Empty);
                        if (memberInInstance != null)
                        {
                            SharedMapInstanceId = memberInInstance.SharedMapInstanceId;
                        }
                        else
                        {
                            // Otherwise, if no one is on an instance, create a new instance
                            SharedMapInstanceId = Guid.NewGuid();

                            // And give your party members their instance lives - though this can be exploited when instances are rejoinable, so you'd really
                            // have to be a freak to have both options on
                            if (Options.Instance.Instancing.MaxSharedInstanceLives > 0)
                            {
                                foreach (Player member in Party)
                                {
                                    member.InstanceLives = Options.Instance.Instancing.MaxSharedInstanceLives;
                                }
                            }
                        }
                    }
                    // Use whatever your shared instance id is for the warp
                    newMapLayerId = SharedMapInstanceId;

                    break;
                default:
                    Log.Error($"Player {Name} requested an instance type that is not supported. Their map instance settings will not change.");
                    break;
            }

            return newMapLayerId;
        }

        /// <summary>
        /// /// Updates the player's last overworld location. Useful for warping out of instances if need be.
        /// </summary>
        /// <param name="overworldMapId">Which map we were on before the instance change</param>
        /// <param name="overworldX">X before instance change</param>
        /// <param name="overworldY">Y before instance change</param>
        public void UpdateLastOverworldLocation(Guid overworldMapId, int overworldX, int overworldY)
        {
            LastOverworldMapId = overworldMapId;
            LastOverworldX = overworldX;
            LastOverworldY = overworldY;
        }

        /// <summary>
        /// Updates the shared instance respawn location - for respawning on death in a shared instance (when this is enabled)
        /// </summary>
        /// <param name="respawnMapId"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="dir"></param>
        public void UpdateSharedInstanceRespawnLocation(Guid respawnMapId, int x, int y, int dir)
        {
            SharedInstanceRespawnId = respawnMapId;
            SharedInstanceRespawnX = x;
            SharedInstanceRespawnY = y;
            SharedInstanceRespawnDir = dir;
        }

        /// <summary>
        /// Resets instance ids we've saved on the player. Generally called when going back to the overworld.
        /// </summary>
        public void ResetSavedInstanceIds()
        {
            PersonalMapInstanceId = Guid.Empty;
            SharedMapInstanceId = Guid.Empty;
        }

        /// <summary>
        /// Checks whether a player can or can not receive the specified item and its quantity.
        /// </summary>
        /// <param name="itemId">The item Id to check if the player can receive.</param>
        /// <param name="quantity">The amount of this item to check if the player can receive.</param>
        /// <param name="slot">The inventory slot to check against.</param>
        /// <returns></returns>
        public bool CanGiveItem(Guid itemId, int quantity, int slot) => CanGiveItem(new Item(itemId, quantity), slot);

        /// <summary>
        /// Checks whether a player can or can not receive the specified item and its quantity.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> to check if this player can receive.</param>
        /// /// <param name="slot">The inventory slot to check against.</param>
        /// <returns></returns>
        public bool CanGiveItem(Item item, int slot)
        {
            // Is this a valid item and slot?
            if (slot >= 0 && slot < Items.Count)
            {
                // Is this slot empty?
                if (Items[slot] != null && Items[slot].ItemId == Guid.Empty)
                {
                    // It is! Can we store the full quantity of this item though?
                    return CanGiveItem(item);
                }
            }
            else
            {
                // Not a valid slot, just treat it as a normal query.
                return CanGiveItem(item);
            }

            return false;
        }

        /// <summary>
        /// Checks whether a player can or can not receive the specified item and its quantity.
        /// </summary>
        /// <param name="itemId">The item Id to check if the player can receive.</param>
        /// <param name="quantity">The amount of this item to check if the player can receive.</param>
        /// <returns></returns>
        public bool CanGiveItem(Guid itemId, int quantity) => CanGiveItem(new Item(itemId, quantity));

        //Inventory
        /// <summary>
        /// Checks whether a player can or can not receive the specified item and its quantity.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> to check if this player can receive.</param>
        /// <returns></returns>
        public bool CanGiveItem(Item item)
        {
            if (item.Descriptor != null)
            {
                // Is the item stackable?
                if (item.Descriptor.IsStackable)
                {
                    // Does the user have this item already?
                    var itemSlots = FindInventoryItemSlots(item.ItemId);
                    var slotsRequired = Math.Ceiling((double)item.Quantity / item.Descriptor.MaxInventoryStack);

                    // User doesn't have this item yet.
                    if (itemSlots.Count == 0)
                    {
                        // Does the user have enough free space for these stacks?
                        if (slotsRequired <= FindOpenInventorySlots().Count)
                        {
                            return true;
                        }
                    }
                    else // We need to check to see how much space we'd have if we first filled all possible stacks
                    {
                        // Keep track of how much we have given to each stack
                        var giveRemainder = item.Quantity;

                        // For each stack while we still have items to give
                        for (var i = 0; i < itemSlots.Count && giveRemainder > 0; i++)
                        {
                            // Give as much as possible to this stack
                            giveRemainder -= item.Descriptor.MaxInventoryStack - itemSlots[i].Quantity;
                        }

                        // We don't have anymore stuff to give after filling up our available stacks - we good
                        bool roomInStacks = giveRemainder <= 0;
                        // We still have leftover even after maxing each of our current stacks. See if we have empty slots in the inventory.
                        bool roomInInventory = giveRemainder > 0 && Math.Ceiling((double)giveRemainder / item.Descriptor.MaxInventoryStack) <= FindOpenInventorySlots().Count;

                        return roomInStacks || roomInInventory;
                    }
                }
                // Not a stacking item, so can we contain the amount we want to give them?
                else
                {
                    if (FindOpenInventorySlots().Count >= item.Quantity)
                    {
                        return true;
                    }
                }
            }

            // Nothing matches in here, give up!
            return false;
        }

        /// <summary>
        /// Checks whether or not a player has enough items in their inventory to be taken.
        /// </summary>
        /// <param name="itemId">The ItemId to see if it can be taken away from the player.</param>
        /// <param name="quantity">The quantity of above item to see if we can take away from the player.</param>
        /// <returns>Whether or not the item can be taken away from the player in the requested quantity.</returns>
        public bool CanTakeItem(Guid itemId, int quantity) => FindInventoryItemQuantity(itemId) >= quantity;

        /// <summary>
        /// Checks whether or not a player has enough items in their inventory to be taken.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> to see if it can be taken away from the player.</param>
        /// <returns>Whether or not the item can be taken away from the player.</returns>
        public bool CanTakeItem(Item item) => CanTakeItem(item.ItemId, item.Quantity);

        /// <summary>
        /// Gets the item at <paramref name="slotIndex"/> and stores it in <paramref name="slot"/>.
        /// </summary>
        /// <param name="slotIndex">the slot to load the <see cref="Item"/> from</param>
        /// <param name="slot">the <see cref="Item"/> at <paramref name="slotIndex"/></param>
        /// <param name="createSlotIfNull">if the slot is in an invalid state (<see langword="null"/>), set it</param>
        /// <returns>returns <see langword="false"/> if <paramref name="slot"/> is set to <see langword="null"/></returns>
        public bool TryGetSlot(int slotIndex, out InventorySlot slot, bool createSlotIfNull = false)
        {
            // ReSharper disable once AssignNullToNotNullAttribute Justification: slot is never null when this returns true.
            slot = Items[slotIndex];

            // ReSharper disable once InvertIf
            if (default == slot && createSlotIfNull)
            {
                var createdSlot = new InventorySlot(slotIndex);
                Log.Error("Creating inventory slot " + slotIndex + " for player " + Name + Environment.NewLine + Environment.StackTrace);
                Items[slotIndex] = createdSlot;
                slot = createdSlot;
            }

            return default != slot;
        }

        /// <summary>
        /// Gets the item at <paramref name="slotIndex"/> and stores it in <paramref name="item"/>.
        /// </summary>
        /// <param name="slotIndex">the slot to load the <see cref="Item"/> from</param>
        /// <param name="item">the <see cref="Item"/> at <paramref name="slotIndex"/></param>
        /// <returns>returns <see langword="false"/> if <paramref name="item"/> is set to <see langword="null"/></returns>
        public bool TryGetItemAt(int slotIndex, out Item item)
        {
            TryGetSlot(slotIndex, out var slot);
            item = slot;
            return default != item;
        }

        /// <summary>
        /// Attempts to give the player an item. Returns whether or not it succeeds.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> to give to the player.</param>
        /// <param name="slot">The inventory slot to put this item into.</param>
        /// <returns>Whether the player received the item or not.</returns>
        public bool TryGiveItem(Item item, int slot = -1) => TryGiveItem(item, ItemHandling.Normal, false, slot, true);

        /// <summary>
        /// Attempts to give the player an item. Returns whether or not it succeeds.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> to give to the player.</param>
        /// <param name="handler">The way to handle handing out this item.</param>
        /// <returns>Whether the player received the item or not.</returns>
        public bool TryGiveItem(Item item, ItemHandling handler) => TryGiveItem(item, handler, false, -1, true);

        /// <summary>
        /// Attempts to give the player an item. Returns whether or not it succeeds.
        /// </summary>
        /// <param name="itemId">The Id for the item to be handed out to the player.</param>
        /// <param name="quantity">The quantity of items to be handed out to the player.</param>
        /// <returns>Whether the player received the item or not.</returns>
        public bool TryGiveItem(Guid itemId, int quantity) => TryGiveItem(new Item(itemId, quantity), ItemHandling.Normal, false, -1, true);

        /// <summary>
        /// Attempts to give the player an item. Returns whether or not it succeeds.
        /// </summary>
        /// <param name="itemId">The Id for the item to be handed out to the player.</param>
        /// <param name="quantity">The quantity of items to be handed out to the player.</param>
        /// <param name="handler">The way to handle handing out this item.</param>
        /// <returns>Whether the player received the item or not.</returns>
        public bool TryGiveItem(Guid itemId, int quantity, ItemHandling handler) => TryGiveItem(new Item(itemId, quantity), handler, false, -1, true);

        /// <summary>
        /// Attempts to give the player an item. Returns whether or not it succeeds.
        /// </summary>
        /// <param name="itemId">The Id for the item to be handed out to the player.</param>
        /// <param name="quantity">The quantity of items to be handed out to the player.</param>
        /// <param name="handler">The way to handle handing out this item.</param>
        /// <param name="bankOverflow">Should we allow the items to overflow into the player's bank when their inventory is full.</param>
        /// <param name="slot">The inventory slot to put this item into.</param>
        /// <param name="sendUpdate">Should we send an inventory update when we are done changing the player's items.</param>
        /// <returns>Whether the player received the item or not.</returns>
        public bool TryGiveItem(Guid itemId, int quantity, ItemHandling handler, bool bankOverflow = false, int slot = -1, bool sendUpdate = true) => TryGiveItem(new Item(itemId, quantity), handler, bankOverflow, slot, sendUpdate);

        /// <summary>
        /// Attempts to give the player an item. Returns whether or not it succeeds.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> to give to the player.</param>
        /// <param name="handler">The way to handle handing out this item.</param>
        /// <param name="bankOverflow">Should we allow the items to overflow into the player's bank when their inventory is full.</param>
        /// <param name="slot">The inventory slot to put this item into.</param>
        /// <param name="sendUpdate">Should we send an inventory update when we are done changing the player's items.</param>
        /// <param name="overflowTileX">The x coordinate of the tile in which overflow should spawn on, if the player cannot hold the full amount.</param>
        /// <param name="overflowTileY">The y coordinate of the tile in which overflow should spawn on, if the player cannot hold the full amount.</param>
        /// <returns>Whether the player received the item or not.</returns>
        public bool TryGiveItem(Item item, ItemHandling handler = ItemHandling.Normal, bool bankOverflow = false, int slot = -1, bool sendUpdate = true, int overflowTileX = -1, int overflowTileY = -1)
        {
            // Is this a valid item?
            if (item.Descriptor == null)
            {
                return false;
            }

            if (item.Quantity <= 0)
            {
                return true;
            }

            // Get this information so we can use it later.
            var openSlots = FindOpenInventorySlots().Count;
            var slotsRequired = (int)Math.Ceiling(item.Quantity / (double) item.Descriptor.MaxInventoryStack);

            int spawnAmount = 0;

            // How are we going to be handling this?
            var success = false;
            switch (handler)
            {
                // Handle this item like normal, there's no special rules attached to this method.
                case ItemHandling.Normal:
                    if (CanGiveItem(item, slot)) // Can receive item under regular rules.
                    {
                        GiveItem(item, slot, sendUpdate);
                        success = true;
                    }

                    break;
                case ItemHandling.Overflow:
                    if (CanGiveItem(item)) // Can receive item under regular rules.
                    {
                        GiveItem(item, slot, sendUpdate);
                        success = true;
                        break;
                    }
                    else if (item.Descriptor.Stackable && openSlots < slotsRequired) // Is stackable, but no inventory space.
                    {
                        spawnAmount = item.Quantity;
                    }
                    else // Time to give them as much as they can take, and spawn the rest on the map!
                    {
                        spawnAmount = item.Quantity - openSlots;
                        if (openSlots > 0)
                        {
                            item.Quantity = openSlots;
                            GiveItem(item, slot, sendUpdate);
                        }
                    }

                    // Do we have any items to spawn to the map?
                    if (spawnAmount > 0 && MapController.TryGetInstanceFromMap(Map.Id, MapInstanceId, out var instance))
                    {
                        instance.SpawnItem(overflowTileX > -1 ? overflowTileX : X, overflowTileY > -1 ? overflowTileY : Y, item, spawnAmount, Id);
                        return spawnAmount != item.Quantity;
                    }

                    break;
                case ItemHandling.UpTo:
                    if (CanGiveItem(item, slot)) // Can receive item under regular rules.
                    {
                        GiveItem(item, slot, sendUpdate);
                        success = true;
                    }
                    else if (!item.Descriptor.Stackable && openSlots >= slotsRequired) // Is not stackable, has space for some.
                    {
                        item.Quantity = openSlots;
                        GiveItem(item, slot, sendUpdate);
                        success = true;
                    }

                    break;
                    // Did you forget to change this method when you added something? ;)
                default:
                    throw new NotImplementedException();
            }

            if (success)
            {
                // Start common events related to inventory changes.
                StartCommonEventsWithTrigger(CommonEventTrigger.InventoryChanged);

                return true;
            }

            var bankInterface = new BankInterface(this, ((IEnumerable<Item>)Bank).ToList(), new object(), null, Options.Instance.Bank.MaxSlots);
            return bankOverflow && bankInterface.TryDepositItem(item, sendUpdate);
        }


        /// <summary>
        /// Gives the player an item. NOTE: This method MAKES ZERO CHECKS to see if this is possible!
        /// Use TryGiveItem where possible!
        /// </summary>
        /// <param name="item"></param>
        /// <param name="sendUpdate"></param>
        private void GiveItem(Item item, int destSlot, bool sendUpdate)
        {

            // Decide how we're going to handle this item.
            var existingSlots = FindInventoryItemSlots(item.Descriptor.Id);
            var updateSlots = new List<int>();
            if (item.Descriptor.Stackable && existingSlots.Count > 0) // Stackable, but already exists in the inventory.
            {
                // So this gets complicated.. First let's hand out the quantity we can hand out before we hit a stack limit.
                var toGive = item.Quantity;
                foreach (var slot in existingSlots)
                {
                    if (toGive == 0)
                    {
                        break;
                    }

                    if (slot.Quantity >= item.Descriptor.MaxInventoryStack)
                    {
                        continue;
                    }

                    var canAdd = item.Descriptor.MaxInventoryStack - slot.Quantity;
                    if (canAdd > toGive)
                    {
                        slot.Quantity += toGive;
                        updateSlots.Add(slot.Slot);
                        toGive = 0;
                    }
                    else
                    {
                        slot.Quantity += canAdd;
                        updateSlots.Add(slot.Slot);
                        toGive -= canAdd;
                    }
                }

                // Is there anything left to hand out? If so, hand out max stacks and what remains until we run out!
                if (toGive > 0)
                {
                    // Are we trying to put the item into a specific slot? If so, put as much in as possible!
                    if (destSlot != -1)
                    {
                        if (toGive > item.Descriptor.MaxInventoryStack)
                        {
                            Items[destSlot].Set(new Item(item.ItemId, item.Descriptor.MaxInventoryStack));
                            updateSlots.Add(destSlot);
                            toGive -= item.Descriptor.MaxInventoryStack;
                        }
                        else
                        {
                            Items[destSlot].Set(new Item(item.ItemId, toGive));
                            updateSlots.Add(destSlot);
                            toGive = 0;
                        }
                    }

                    var openSlots = FindOpenInventorySlots();
                    var total = toGive; // Copy this as we're going to be editing toGive.
                    for (var slot = 0; slot < Math.Ceiling((double)total / item.Descriptor.MaxInventoryStack); slot++)
                    {
                        var quantity = item.Descriptor.MaxInventoryStack <= toGive ?
                            item.Descriptor.MaxInventoryStack :
                            toGive;

                        toGive -= quantity;
                        openSlots[slot].Set(new Item(item.ItemId, quantity));
                        updateSlots.Add(openSlots[slot].Slot);
                    }
                }
            }
            else if (!item.Descriptor.Stackable && item.Quantity > 1) // Not stackable, but multiple items.
            {
                var toGive = item.Quantity;
                if (destSlot != -1)
                {
                    Items[destSlot].Set(new Item(item.ItemId, 1));
                    updateSlots.Add(destSlot);
                    toGive -= 1;
                }

                var openSlots = FindOpenInventorySlots();
                for (var slot = 0; slot < toGive; slot++)
                {
                    openSlots[slot].Set(new Item(item.ItemId, 1));
                    updateSlots.Add(openSlots[slot].Slot);
                }
            }
            else // Hand out without any special treatment. Either a single item or a stackable item we don't have yet.
            {
                // If the item is not stackable, or the amount is below our stack cap just blindly hand it out.
                if (!item.Descriptor.Stackable || item.Quantity < item.Descriptor.MaxInventoryStack)
                {
                    if (destSlot != -1)
                    {
                        Items[destSlot].Set(item);
                        updateSlots.Add(destSlot);
                    }
                    else
                    {
                        var newSlot = FindOpenInventorySlot();
                        newSlot.Set(item);
                        updateSlots.Add(newSlot.Slot);
                    }
                }
                // The item is above our stack cap.. Let's start handing them phat stacks out!
                else
                {
                    var toGive = item.Quantity;

                    if (destSlot != -1)
                    {
                        Items[destSlot].Set(new Item(item.ItemId, item.Descriptor.MaxInventoryStack));
                        updateSlots.Add(destSlot);
                        toGive -= item.Descriptor.MaxInventoryStack;
                    }

                    var openSlots = FindOpenInventorySlots();
                    for (var slot = 0; slot < Math.Ceiling((double) item.Quantity / item.Descriptor.MaxInventoryStack); slot++)
                    {
                        var quantity = item.Descriptor.MaxInventoryStack <= toGive ?
                            item.Descriptor.MaxInventoryStack :
                            toGive;

                        toGive -= quantity;
                        openSlots[slot].Set(new Item(item.ItemId, quantity));
                        updateSlots.Add(openSlots[slot].Slot);
                    }
                }

            }

            // Do we need to update the player's inventory?
            if (sendUpdate)
            {
                foreach (var slot in updateSlots)
                {
                    PacketSender.SendInventoryItemUpdate(this, slot);
                }
            }

            // Update quests for this item.
            UpdateGatherItemQuests(item.ItemId);

        }

        /// <summary>
        /// Retrieves a list of open inventory slots for this player.
        /// </summary>
        /// <returns>A list of <see cref="InventorySlot"/></returns>
        public List<InventorySlot> FindOpenInventorySlots()
        {
            var slots = new List<InventorySlot>();
            for (var i = 0; i < Options.MaxInvItems; i++)
            {
                var inventorySlot = Items[i];

                if (inventorySlot != null && inventorySlot.ItemId == Guid.Empty)
                {
                    slots.Add(inventorySlot);
                }
            }
            return slots;
        }

        /// <summary>
        /// Finds the first open inventory slot this player has.
        /// </summary>
        /// <returns>An <see cref="InventorySlot"/> instance, or null if none are found.</returns>
        public InventorySlot FindOpenInventorySlot() => FindOpenInventorySlots().FirstOrDefault();

        /// <summary>
        /// Swap items between <paramref name="fromSlotIndex"/> and <paramref name="toSlotIndex"/>.
        /// </summary>
        /// <param name="fromSlotIndex">the slot index to swap from</param>
        /// <param name="toSlotIndex">the slot index to swap to</param>
        public void SwapItems(int fromSlotIndex, int toSlotIndex)
        {
            TryGetSlot(fromSlotIndex, out var fromSlot, true);
            TryGetSlot(toSlotIndex, out var toSlot, true);

            var toSlotClone = toSlot.Clone();
            toSlot.Set(fromSlot);
            fromSlot.Set(toSlotClone);

            PacketSender.SendInventoryItemUpdate(this, fromSlotIndex);
            PacketSender.SendInventoryItemUpdate(this, toSlotIndex);
            EquipmentProcessItemSwap(fromSlotIndex, toSlotIndex);
        }

        /// <summary>
        /// Attempt to drop <paramref name="amount"/> of the item in the slot
        /// identified by <paramref name="slotIndex"/>, returning false if it
        /// is unable to drop the item for any reason.
        /// </summary>
        /// <param name="slotIndex">the slot to drop from</param>
        /// <param name="amount">the amount to drop</param>
        /// <returns>if an item was dropped</returns>
        public bool TryDropItemFrom(int slotIndex, int amount)
        {
            if (!TryGetItemAt(slotIndex, out var itemInSlot))
            {
                return false;
            }

            amount = Math.Min(amount, itemInSlot.Quantity);
            if (amount < 1)
            {
                // Abort if the amount we are trying to drop is below 1.
                return false;
            }

            if (Equipment?.Any(equipmentSlotIndex => equipmentSlotIndex == slotIndex) ?? false)
            {
                PacketSender.SendChatMsg(this, Strings.Items.equipped, ChatMessageType.Inventory, CustomColors.Items.Bound);
                return false;
            }

            var itemDescriptor = itemInSlot.Descriptor;
            if (itemDescriptor == null)
            {
                return false;
            }

            if (!itemDescriptor.CanDrop)
            {
                PacketSender.SendChatMsg(this, Strings.Items.bound, ChatMessageType.Inventory, CustomColors.Items.Bound);
                return false;
            }

            if (itemInSlot.TryGetBag(out var bag) && !bag.IsEmpty)
            {
                PacketSender.SendChatMsg(this, Strings.Bags.dropnotempty, ChatMessageType.Inventory, CustomColors.Alerts.Error);
                return false;
            }

            if (!MapController.TryGetInstanceFromMap(MapId, MapInstanceId, out var mapInstance))
            {
                Log.Error(
                    Map == default
                        ? $"Could not find map {MapId} for player '{Name}'."
                        : $"Could not find map instance {MapInstanceId} for player '{Name}' on map {Map.Name}."
                );
                return false;
            }

            mapInstance.SpawnItem(X, Y, itemInSlot, itemDescriptor.IsStackable ? amount : 1, Id);

            itemInSlot.Quantity = Math.Max(0, itemInSlot.Quantity - amount);

            if (itemInSlot.Quantity == 0)
            {
                itemInSlot.Set(Item.None);
                EquipmentProcessItemLoss(slotIndex);
            }

            StartCommonEventsWithTrigger(CommonEventTrigger.InventoryChanged);
            UpdateGatherItemQuests(itemDescriptor.Id);
            PacketSender.SendInventoryItemUpdate(this, slotIndex);
            return true;
        }

        /// <summary>
        /// Drops <paramref name="amount"/> of the item in the slot identified by <paramref name="slotIndex"/>.
        /// </summary>
        /// <param name="slotIndex">the slot to drop from</param>
        /// <param name="amount">the amount to drop</param>
        /// <see cref="TryDropItemFrom(int, int)"/>
        [Obsolete("Use TryDropItemFrom(int, int).")]
        public void DropItemFrom(int slotIndex, int amount) => TryDropItemFrom(slotIndex, amount);

        protected override bool ShouldDropItem(Entity killer, ItemBase itemDescriptor, Item item, float dropRateModifier, out Guid lootOwner)
        {
            lootOwner = (killer as Player)?.Id ?? Id;

            if (itemDescriptor.DropChanceOnDeath == 0)
            {
                return false;
            }

            var dropRate = itemDescriptor.DropChanceOnDeath * dropRateModifier;
            var dropResult = Randomization.Next(1, 101);
            return dropResult < dropRate;
        }

        protected override void OnDropItem(InventorySlot slot, Item drop)
        {
            _ = TryTakeItem(slot, drop.Quantity);
        }

        public void UseItem(int slot, Entity target = null)
        {
            var equipped = false;
            var Item = Items[slot];
            var itemBase = ItemBase.Get(Item.ItemId);
            if (itemBase != null && Item.Quantity > 0)
            {

                //Check if the user is silenced or stunned
                foreach (var status in CachedStatuses)
                {
                    if (status.Type == StatusTypes.Stun)
                    {
                        PacketSender.SendChatMsg(this, Strings.Items.stunned, ChatMessageType.Error);

                        return;
                    }

                    if (status.Type == StatusTypes.Sleep)
                    {
                        PacketSender.SendChatMsg(this, Strings.Items.sleep, ChatMessageType.Error);

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
                            StartCommonEventsWithTrigger(CommonEventTrigger.EquipChange);
                            PacketSender.SendPlayerEquipmentToProximity(this);
                            PacketSender.SendEntityStats(this);

                            return;
                        }
                    }
                }

                if (!Conditions.MeetsConditionLists(itemBase.UsageRequirements, this, null))
                {
                    if (!string.IsNullOrWhiteSpace(itemBase.CannotUseMessage))
                    {
                        PacketSender.SendChatMsg(this, itemBase.CannotUseMessage, ChatMessageType.Error);
                    }
                    else
                    {
                        PacketSender.SendChatMsg(this, Strings.Items.dynamicreq, ChatMessageType.Error);
                    }

                    return;
                }

                if (ItemCooldowns.ContainsKey(itemBase.Id) && ItemCooldowns[itemBase.Id] > Timing.Global.MillisecondsUtc)
                {
                    //Cooldown warning!
                    PacketSender.SendChatMsg(this, Strings.Items.cooldown, ChatMessageType.Error);

                    return;
                }

                switch (itemBase.ItemType)
                {
                    case ItemTypes.None:
                    case ItemTypes.Currency:
                        PacketSender.SendChatMsg(this, Strings.Items.cannotuse, ChatMessageType.Error);

                        return;
                    case ItemTypes.Consumable:
                        var value = 0;
                        var color = CustomColors.Items.ConsumeHp;
                        var die = false;

                        switch (itemBase.Consumable.Type)
                        {
                            case ConsumableType.Health:
                                value = itemBase.Consumable.Value +
                                        GetMaxVital((int) itemBase.Consumable.Type) *
                                        itemBase.Consumable.Percentage /
                                        100;

                                AddVital(Vitals.Health, value);
                                if (value < 0)
                                {
                                    color = CustomColors.Items.ConsumePoison;

                                    //Add a death handler for poison.
                                    die = !HasVital(Vitals.Health);
                                }

                                break;

                            case ConsumableType.Mana:
                                value = itemBase.Consumable.Value +
                                        GetMaxVital((int) itemBase.Consumable.Type) *
                                        itemBase.Consumable.Percentage /
                                        100;

                                AddVital(Vitals.Mana, value);
                                color = CustomColors.Items.ConsumeMp;

                                break;

                            case ConsumableType.Experience:
                                value = itemBase.Consumable.Value +
                                        (int) (GetExperienceToNextLevel(Level) * itemBase.Consumable.Percentage / 100);

                                GiveExperience(value);
                                color = CustomColors.Items.ConsumeExp;

                                break;

                            default:
                                throw new IndexOutOfRangeException();
                        }

                        var symbol = value < 0 ? Strings.Combat.removesymbol : Strings.Combat.addsymbol;
                        var number = $"{symbol}{Math.Abs(value)}";
                        PacketSender.SendActionMsg(this, number, color);

                        if (die)
                        {
                            lock (EntityLock)
                            {
                                Die(true, this);
                            }
                        }

                        TryTakeItem(Items[slot], 1);

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

                        if (equipped)
                        {
                            FixVitals();
                            StartCommonEventsWithTrigger(CommonEventTrigger.EquipChange);
                            PacketSender.SendPlayerEquipmentToProximity(this);
                            PacketSender.SendEntityStats(this);

                            return;
                        }

                        EquipItem(itemBase, slot);

                        break;
                    case ItemTypes.Spell:
                        if (itemBase.SpellId == Guid.Empty)
                        {
                            return;
                        }

                        if (itemBase.QuickCast)
                        {
                            if (!CanCastSpell(itemBase.Spell, target, false, out var _))
                            {
                                return;
                            }

                            CastTarget = target;
                            CastSpell(itemBase.SpellId);
                        }
                        else if (!TryTeachSpell(new Spell(itemBase.SpellId)))
                        {
                            return;
                        }

                        if (itemBase.SingleUse)
                        {
                            TryTakeItem(Items[slot], 1);
                        }

                        break;
                    case ItemTypes.Event:
                        var evt = EventBase.Get(itemBase.EventId);
                        if (evt == null || !StartCommonEvent(evt))
                        {
                            return;
                        }

                        if (itemBase.SingleUse)
                        {
                            TryTakeItem(Items[slot], 1);
                        }

                        break;
                    case ItemTypes.Bag:
                        OpenBag(Item, itemBase);

                        break;
                    default:
                        PacketSender.SendChatMsg(this, Strings.Items.notimplemented, ChatMessageType.Error);

                        return;
                }

                if (itemBase.Animation != null)
                {
                    PacketSender.SendAnimationToProximity(
                        itemBase.Animation.Id, 1, base.Id, MapId, 0, 0, (sbyte)Dir, MapInstanceId
                    ); //Target Type 1 will be global entity
                }

                // Does this item have a cooldown to process of its own?
                if (itemBase.Cooldown > 0)
                {
                    UpdateCooldown(itemBase);
                }

                // Update the global cooldown, if we can trigger it here.
                if (!itemBase.IgnoreGlobalCooldown)
                {
                    UpdateGlobalCooldown();
                }
            }
        }

        /// <summary>
        /// Try to take an item away from the player by slot.
        /// </summary>
        /// <param name="slot">The inventory slot to take the item away from.</param>
        /// <param name="amount">The amount of this item we intend to take away from the player.</param>
        /// <param name="handler">The method in which we intend to handle taking away the item from our player.</param>
        /// <param name="sendUpdate">Do we need to send an inventory update after taking away the item.</param>
        /// <returns></returns>
        public bool TryTakeItem(InventorySlot slot, int amount, ItemHandling handler = ItemHandling.Normal, bool sendUpdate = true)
        {
            if (Items == null || slot == Item.None || slot == null)
            {
                return false;
            }

            // Figure out how many we can take!
            var toTake = 0;
            switch (handler)
            {
                case ItemHandling.Normal:
                case ItemHandling.Overflow: // We can't overflow a take command, so process it as if it's a normal one.
                    if (slot.Quantity < amount) // Cancel out if we don't have enough items to cover for this.
                    {
                        return false;
                    }

                    // We can take all of the items we need!
                    toTake = amount;

                    break;
                case ItemHandling.UpTo:
                    // Can we take all our items or just some?
                    toTake = slot.Quantity >= amount ? amount : slot.Quantity;

                    break;

                // Did you forget something? ;)
                default:
                    throw new NotImplementedException();
            }

            // Can we actually take any?
            if (toTake == 0)
            {
                return false;
            }

            // Figure out what we're dealing with here.
            var itemDescriptor = slot.Descriptor;

            // is this stackable? if so try to take as many as we can each time.
            if (itemDescriptor.Stackable)
            {
                if (slot.Quantity >= toTake)
                {
                    TakeItem(slot, toTake, sendUpdate);
                    toTake = 0;
                }
                else // Take away the entire quantity of the item and lower our items that we still need to take!
                {
                    toTake -= slot.Quantity;
                    TakeItem(slot, slot.Quantity, sendUpdate);
                }
            }
            else // Not stackable, so just take one item away.
            {
                toTake -= 1;
                TakeItem(slot, 1, sendUpdate);
            }

            // Update quest progress and we're done!
            UpdateGatherItemQuests(slot.ItemId);

            // Start common events related to inventory changes.
            StartCommonEventsWithTrigger(CommonEventTrigger.InventoryChanged);

            return true;

        }

        /// <summary>
        /// Try to take away an item from the player by Id.
        /// </summary>
        /// <param name="itemId">The Id of the item we're trying to take away from the player.</param>
        /// <param name="amount">The amount of this item we intend to take away from the player.</param>
        /// <param name="handler">The method in which we intend to handle taking away the item from our player.</param>
        /// <param name="sendUpdate">Do we need to send an inventory update after taking away the item.</param>
        /// <returns>Whether the item was taken away successfully or not.</returns>
        public bool TryTakeItem(Guid itemId, int amount, ItemHandling handler = ItemHandling.Normal, bool sendUpdate = true)
        {
            if (Items == null)
            {
                return false;
            }

            // Figure out how many we can take!
            var toTake = 0;
            switch (handler)
            {
                case ItemHandling.Normal:
                case ItemHandling.Overflow: // We can't overflow a take command, so process it as if it's a normal one.
                    if (!CanTakeItem(itemId, amount)) // Cancel out if we don't have enough items to cover for this.
                    {
                        return false;
                    }

                    // We can take all of the items we need!
                    toTake = amount;

                    break;
                case ItemHandling.UpTo:
                    // Can we take all our items or just some?
                    var itemCount = FindInventoryItemQuantity(itemId);
                    toTake = itemCount >= amount ? amount : itemCount;

                    break;

                // Did you forget something? ;)
                default:
                    throw new NotImplementedException();
            }

            // Can we actually take any?
            if (toTake == 0)
            {
                return false;
            }

            // Figure out what we're dealing with here.
            var itemDescriptor = ItemBase.Get(itemId);

            // Go through our inventory and take what we need!
            foreach (var slot in FindInventoryItemSlots(itemId))
            {
                // Do we still have items to take? If not leave the loop!
                if (toTake == 0)
                {
                    break;
                }

                // is this stackable? if so try to take as many as we can each time.
                if (itemDescriptor.Stackable)
                {
                    if (slot.Quantity >= toTake)
                    {
                        TakeItem(slot, toTake, sendUpdate);
                        toTake = 0;
                    }
                    else // Take away the entire quantity of the item and lower our items that we still need to take!
                    {
                        toTake -= slot.Quantity;
                        TakeItem(slot, slot.Quantity, sendUpdate);
                    }
                }
                else // Not stackable, so just take one item away.
                {
                    toTake -= 1;
                    TakeItem(slot, 1, sendUpdate);
                }
            }

            // Update quest progress and we're done!
            UpdateGatherItemQuests(itemId);

            // Start common events related to inventory changes.
            StartCommonEventsWithTrigger(CommonEventTrigger.InventoryChanged);

            return true;
        }

        /// <summary>
        /// Take an item away from the player, or an amount of it if they have more. NOTE: This method MAKES ZERO CHECKS to see if this is possible!
        /// Use TryTakeItem where possible!
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="amount"></param>
        /// <param name="sendUpdate"></param>
        private void TakeItem(InventorySlot slot, int amount, bool sendUpdate = true)
        {
            if (slot.Quantity > amount) // This slot contains more than what we're trying to take away here. Update the quantity.
            {
                slot.Quantity -= amount;
            }
            else // Take the entire thing away!
            {
                slot.Set(Item.None);
                EquipmentProcessItemLoss(slot.Slot);
            }

            if (sendUpdate)
            {
                PacketSender.SendInventoryItemUpdate(this, slot.Slot);
            }
        }

        /// <summary>
        /// Find the amount of a specific item a player has.
        /// </summary>
        /// <param name="itemId">The item Id to look for.</param>
        /// <returns>The amount of the requested item the player has on them.</returns>
        public int FindInventoryItemQuantity(Guid itemId)
        {
            if (Items == null)
            {
                return 0;
            }

            long itemCount = 0;
            for (var i = 0; i < Options.MaxInvItems; i++)
            {
                var item = Items[i];
                if (item.ItemId == itemId)
                {
                    itemCount = item.Descriptor.Stackable ? itemCount += item.Quantity : itemCount += 1;
                }
            }

            // TODO: Stop using Int32 for item quantities
            return itemCount >= Int32.MaxValue ? Int32.MaxValue : (int) itemCount;
        }

        /// <summary>
        /// Finds an inventory slot matching the desired item and quantity.
        /// </summary>
        /// <param name="itemId">The item Id to look for.</param>
        /// <param name="quantity">The quantity of the item to look for.</param>
        /// <returns>An <see cref="InventorySlot"/> that contains the item, or null if none are found.</returns>
        public InventorySlot FindInventoryItemSlot(Guid itemId, int quantity = 1) => FindInventoryItemSlots(itemId, quantity).FirstOrDefault();

        /// <summary>
        /// Finds the index of a given inventory slot
        /// </summary>
        /// <param name="slot">The <see cref="InventorySlot"/> to find</param>
        /// <returns>An <see cref="int"/>containing the relevant index, or -1 if not found</returns>
        public int FindInventoryItemSlotIndex(InventorySlot slot) 
        {
            return Items.FindIndex(sl => sl.Id == slot.Id);
        } 

        /// <summary>
        /// Finds all inventory slots matching the desired item and quantity.
        /// </summary>
        /// <param name="itemId">The item Id to look for.</param>
        /// <param name="quantity">The quantity of the item to look for.</param>
        /// <returns>A list of <see cref="InventorySlot"/> containing the requested item.</returns>
        public List<InventorySlot> FindInventoryItemSlots(Guid itemId, int quantity = 1)
        {
            var slots = new List<InventorySlot>();
            if (Items == null)
            {
                return slots;
            }

            for (var i = 0; i < Options.MaxInvItems; i++)
            {
                var item = Items[i];
                if (item?.ItemId != itemId)
                {
                    continue;
                }

                if (item.Quantity >= quantity)
                {
                    slots.Add(Items[i]);
                }
            }

            return slots;
        }

        public int CountItems(Guid itemId, bool inInventory = true, bool inBank = false)
        {
            if (inInventory == false && inBank == false)
            {
                throw new ArgumentException(
                    $@"At least one of either {nameof(inInventory)} or {nameof(inBank)} must be true to count items."
                );
            }

            var count = 0;

            int QuantityFromSlot(Item item)
            {
                return item?.ItemId == itemId ? Math.Max(1, item.Quantity) : 0;
            }

            if (inInventory)
            {
                count += Items.Sum(QuantityFromSlot);
            }

            if (inBank)
            {
                count += Bank.Sum(QuantityFromSlot);
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

        /// <summary>
        /// Gets the percentage value of a bonus effect as granted by the currently equipped gear.
        /// </summary>
        /// <param name="effect">The <see cref="EffectType"/> to retrieve the amount for.</param>
        /// <returns></returns>
        public int GetEquipmentBonusEffect(EffectType effect)
        {
            var value = 0;

            for (var i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                if (Equipment[i] > -1)
                {
                    if (Items[Equipment[i]].ItemId != Guid.Empty)
                    {
                        var item = ItemBase.Get(Items[Equipment[i]].ItemId);
                        if (item != null)
                        {
                            if (item.Effect.Type == effect)
                            {
                                value += item.Effect.Percentage;
                            }
                        }
                    }
                }
            }

            return value;
        }

        public int GetEquipmentVitalRegen(Vitals vital)
        {
            var regen = 0;

            for (var i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                if (Equipment[i] > -1)
                {
                    if (Items[Equipment[i]].ItemId != Guid.Empty)
                    {
                        var item = ItemBase.Get(Items[Equipment[i]].ItemId);
                        if (item != null)
                        {
                            regen += item.VitalsRegen[(int) vital];
                        }
                    }
                }
            }

            return regen;
        }

        //Shop
        public bool OpenShop(ShopBase shop)
        {
            if (IsBusy())
            {
                return false;
            }

            InShop = shop;
            PacketSender.SendOpenShop(this, shop);

            return true;
        }

        public void CloseShop()
        {
            if (InShop != null)
            {
                InShop = null;
                PacketSender.SendCloseShop(this);
            }
        }

        public void SellItem(int slot, int amount)
        {
            var canSellItem = true;
            var rewardItemId = Guid.Empty;
            var rewardItemVal = 0;

            TryGetSlot(slot, out var itemInSlot, true);
            var sellItemNum = itemInSlot.ItemId;
            var shop = InShop;
            if (shop != null)
            {
                var itemDescriptor = Items[slot].Descriptor;
                if (itemDescriptor != null)
                {
                    if (!itemDescriptor.CanSell)
                    {
                        PacketSender.SendChatMsg(this, Strings.Shops.bound, ChatMessageType.Inventory, CustomColors.Items.Bound);

                        return;
                    }

                    //Check if this is a bag with items.. if so don't allow sale
                    if (itemDescriptor.ItemType == ItemTypes.Bag)
                    {
                        if (itemInSlot.TryGetBag(out var bag))
                        {
                            if (!bag.IsEmpty)
                            {
                                PacketSender.SendChatMsg(this, Strings.Bags.onlysellempty, ChatMessageType.Inventory, CustomColors.Alerts.Error);
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
                                PacketSender.SendChatMsg(this, Strings.Shops.doesnotaccept, ChatMessageType.Inventory, CustomColors.Alerts.Error);

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
                            PacketSender.SendChatMsg(this, Strings.Shops.doesnotaccept, ChatMessageType.Inventory, CustomColors.Alerts.Error);

                            return;
                        }
                        else
                        {
                            rewardItemId = shop.DefaultCurrencyId;
                            rewardItemVal = itemDescriptor.Price;
                        }
                    }

                    amount = Math.Min(itemInSlot.Quantity, amount);

                    if (amount == itemInSlot.Quantity)
                    {
                        // Definitely can get reward.
                        itemInSlot.Set(Item.None);
                        EquipmentProcessItemLoss(slot);
                    }
                    else
                    {
                        //check if can get reward
                        if (!CanGiveItem(rewardItemId, rewardItemVal))
                        {
                            canSellItem = false;
                        }
                        else
                        {
                            itemInSlot.Quantity -= amount;
                        }
                    }

                    if (canSellItem)
                    {
                        TryGiveItem(rewardItemId, rewardItemVal * amount);

                        if (!TextUtils.IsNone(shop.SellSound))
                        {
                            PacketSender.SendPlaySound(this, shop.SellSound);
                        }
                    }

                    PacketSender.SendInventoryItemUpdate(this, slot);
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
                        if (itemBase.IsStackable)
                        {
                            buyItemAmt = Math.Max(1, amount);
                        }

                        if (shop.SellingItems[slot].CostItemQuantity == 0 ||
                            FindInventoryItemSlot(
                                shop.SellingItems[slot].CostItemId,
                                shop.SellingItems[slot].CostItemQuantity * buyItemAmt
                            ) !=
                            null)
                        {
                            if (CanGiveItem(buyItemNum, buyItemAmt))
                            {
                                if (shop.SellingItems[slot].CostItemQuantity > 0)
                                {
                                    TryTakeItem(
                                        FindInventoryItemSlot(
                                            shop.SellingItems[slot].CostItemId,
                                            shop.SellingItems[slot].CostItemQuantity * buyItemAmt
                                        ), shop.SellingItems[slot].CostItemQuantity * buyItemAmt
                                    );
                                }

                                TryGiveItem(buyItemNum, buyItemAmt);

                                if (!TextUtils.IsNone(shop.BuySound))
                                {
                                    PacketSender.SendPlaySound(this, shop.BuySound);
                                }
                            }
                            else
                            {
                                if (shop.SellingItems[slot].CostItemQuantity * buyItemAmt ==
                                    Items[
                                            FindInventoryItemSlot(
                                                shop.SellingItems[slot].CostItemId,
                                                shop.SellingItems[slot].CostItemQuantity * buyItemAmt
                                            ).Slot]
                                        .Quantity)
                                {
                                    TryTakeItem(
                                        FindInventoryItemSlot(
                                            shop.SellingItems[slot].CostItemId,
                                            shop.SellingItems[slot].CostItemQuantity * buyItemAmt
                                        ), shop.SellingItems[slot].CostItemQuantity * buyItemAmt
                                    );

                                    TryGiveItem(buyItemNum, buyItemAmt);

                                    if (!TextUtils.IsNone(shop.BuySound))
                                    {
                                        PacketSender.SendPlaySound(this, shop.BuySound);
                                    }
                                }
                                else
                                {
                                    PacketSender.SendChatMsg(
                                        this, Strings.Shops.inventoryfull, ChatMessageType.Inventory, CustomColors.Alerts.Error, Name
                                    );
                                }
                            }
                        }
                        else
                        {
                            PacketSender.SendChatMsg(this, Strings.Shops.cantafford, ChatMessageType.Inventory, CustomColors.Alerts.Error, Name);
                        }
                    }
                }
            }
        }

        //Crafting
        public bool OpenCraftingTable(CraftingTableBase table)
        {
            if (IsBusy())
            {
                return false;
            }

            if (table != null)
            {
                CraftingTableId = table.Id;
                PacketSender.SendOpenCraftingTable(this, table);
            }

            return true;
        }

        public void CloseCraftingTable()
        {
            if (CraftingTableId != Guid.Empty && CraftId == Guid.Empty)
            {
                CraftingTableId = Guid.Empty;
                PacketSender.SendCloseCraftingTable(this);
            }
        }

        //Craft a new item
        public void CraftItem(Guid id)
        {
            if (CraftingTableId == Guid.Empty)
            {
                return;
            }

            var table = CraftingTableBase.Get(CraftingTableId);
            if (table == null)
            {
                return;
            }

            if (!table.Crafts.Contains(id))
            {
                return;
            }

            var craftDescriptor = CraftBase.Get(id);

            if (craftDescriptor == null)
            {
                return;
            }

            if (!Conditions.MeetsConditionLists(craftDescriptor.CraftingRequirements, this, null))
            {
                PacketSender.SendChatMsg(this, Strings.Crafting.RequirementsNotMet.ToString(), ChatMessageType.Error);

                return;
            }

            var backupItems = new List<Item>();
            foreach (var backupItem in Items)
            {
                backupItems.Add(backupItem.Clone());
            }

            var craft = CraftBase.Get(id);
            var craftItem = ItemBase.Get(craft.ItemId);
            if (craftItem == null)
            {
                PacketSender.SendChatMsg(this, Strings.Errors.UnknownErrorTryAgain, ChatMessageType.Error, CustomColors.Alerts.Error);
                Log.Error($"Unable to find item descriptor {craftItem.Id} for craft {craft.Id}.");
                return;
            }

            //Quickly Look through the inventory and create a catalog of what items we have, and how many
            var inventoryItems = new Dictionary<Guid, int>();
            foreach (var inventoryItem in Items)
            {
                if (inventoryItem != null)
                {
                    var quantity = inventoryItem.Quantity;
                    if (inventoryItems.ContainsKey(inventoryItem.ItemId))
                    {
                        quantity += inventoryItems[inventoryItem.ItemId];
                    }
                    inventoryItems[inventoryItem.ItemId] = quantity;
                }
            }

            //Check the player actually has the items
            foreach (var ingredient in craft.Ingredients)
            {
                if (!inventoryItems.ContainsKey(ingredient.ItemId))
                {
                    CraftId = Guid.Empty;
                    return;
                }

                var currentQuantity = inventoryItems[ingredient.ItemId];
                if (currentQuantity < ingredient.Quantity)
                {
                    CraftId = Guid.Empty;
                    return;
                }
                inventoryItems[ingredient.ItemId] = currentQuantity - ingredient.Quantity;
            }

            //Return true if the craft was a success
            if (Randomization.Next(0, 101) > craft.FailureChance)
            {
                //Take the items
                foreach (var ingredient in craft.Ingredients)
                {
                    if (TryTakeItem(ingredient.ItemId, ingredient.Quantity))
                    {
                        continue;
                    }

                    for (var slotIndex = 0; slotIndex < backupItems.Count; slotIndex++)
                    {
                        Items[slotIndex].Set(backupItems[slotIndex]);
                    }

                    PacketSender.SendInventory(this);
                    CraftId = Guid.Empty;

                    return;
                }

                //Give them the craft
                var quantity = Math.Max(craft.Quantity, 1);
                if (!craftItem.IsStackable)
                {
                    quantity = 1;
                }

                if (TryGiveItem(craftItem.Id, quantity))
                {
                    PacketSender.SendChatMsg(
                        this, Strings.Crafting.crafted.ToString(craftItem.Name), ChatMessageType.Crafting,
                        CustomColors.Alerts.Success
                    );

                    if (craft.Event != default)
                    {
                        StartCommonEvent(craft.Event);
                    }
                }
                else
                {
                    for (var i = 0; i < backupItems.Count; i++)
                    {
                        Items[i].Set(backupItems[i]);
                    }

                    PacketSender.SendInventory(this);
                    PacketSender.SendChatMsg(
                        this, Strings.Crafting.nospace.ToString(craftItem.Name), ChatMessageType.Crafting,
                        CustomColors.Alerts.Error
                    );
                }
            }
            else
            {
                var message = Strings.Crafting.CraftFailure;

                //Returns true if items should be taken from the player
                if (Randomization.Next(0, 101) < craft.ItemLossChance)
                {
                    //Take the items
                    foreach (var ingredient in craft.Ingredients)
                    {
                        if (TryTakeItem(ingredient.ItemId, ingredient.Quantity))
                        {
                            continue;
                        }

                        for (var i = 0; i < backupItems.Count; i++)
                        {
                            Items[i].Set(backupItems[i]);
                        }

                        PacketSender.SendInventory(this);
                        CraftId = Guid.Empty;

                        return;
                    }

                    message = Strings.Crafting.CraftFailureLostItems;
                }

                PacketSender.SendInventory(this);
                PacketSender.SendChatMsg(this, message.ToString(craftItem.Name), ChatMessageType.Crafting, CustomColors.Alerts.Error);
            }

            CraftId = Guid.Empty;
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
        private bool IsBusy() =>
            InShop != null ||
            InBank ||
            CraftingTableId != Guid.Empty ||
            Trading.Counterparty != null ||
            Trading.Requester != null ||
            PartyRequester != null ||
            FriendRequester != null;

        //Bank
        public bool OpenBank(bool guild = false)
        {
            if (IsBusy())
            {
                return false;
            }

            if (guild && Guild == null)
            {
                return false;
            }

            var bankItems = ((IEnumerable<Item>)Bank).ToList();

            if (guild)
            {
                bankItems = ((IEnumerable<Item>)Guild.Bank).ToList();
            }

            BankInterface = new BankInterface(this, bankItems, guild ? Guild.Lock : new object(), guild ? Guild : null, guild ? Guild.BankSlotsCount : Options.Instance.PlayerOpts.InitialBankslots);

            GuildBank = guild;
            BankInterface.SendOpenBank();

            return true;
        }

        public void CloseBank()
        {
            if (InBank)
            {
                BankInterface.Dispose();
            }
        }

        // TODO: Document this. The TODO on bagItem == null needs to be resolved before this is.
        public bool OpenBag(Item bagItem, ItemBase itemDescriptor)
        {
            if (IsBusy())
            {
                return false;
            }

            // TODO: Figure out what to return in the event of a bad argument. An NRE would have happened anyway, and I don't have enough awareness of the bag feature to do this differently.
            if (bagItem == null)
            {
                throw new ArgumentNullException(nameof(bagItem));
            }

            // If the bag does not exist, create one.
            if (!bagItem.TryGetBag(out var bag))
            {
                var slotCount = itemDescriptor.SlotCount;
                if (slotCount < 1)
                {
                    slotCount = 1;
                }

                bag = new Bag(slotCount);
                bagItem.Bag = bag;
            }

            //Send the bag to the player (this will make it appear on screen)
            InBag = bag;
            PacketSender.SendOpenBag(this, bag.SlotCount, bag);

            return true;
        }

        public bool HasBag(Bag bag)
        {
            for (var i = 0; i < Items.Count; i++)
            {
                if (Items[i] != null && Items[i].Bag == bag)
                {
                    return true;
                }
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
                PacketSender.SendCloseBag(this);
            }
        }

        private bool TryFillInventoryStacksOfItemFroTradeOffer(Item tradeItem, int offerIdx, List<InventorySlot> inventorySlots, ItemBase itemDescriptor, int amountToGive = 1)
        {
            int amountRemainder = amountToGive;
            foreach (var invItem in inventorySlots)
            {
                // If we've fulfilled our stacking desires, we're done
                if (amountRemainder <= 0 || FindOpenInventorySlots().Count <= 0)
                {
                    return amountRemainder <= 0;
                }
                var currSlot = FindInventoryItemSlotIndex(invItem);

                // Otherwise, first update how many of our item we still need to put in this current slot
                amountToGive = amountRemainder;
                var maxDiff = itemDescriptor.MaxInventoryStack - invItem.Quantity;
                amountRemainder = amountToGive - maxDiff;
                amountToGive = MathHelper.Clamp(amountToGive, 0, maxDiff);

                // Then, determine what the slots _new_ quantity should be
                var newQuantity = MathHelper.Clamp(invItem.Quantity + amountToGive, 0, itemDescriptor.MaxInventoryStack);
                // If the slot we're going to fill is empty, give it the item from the inventory
                if (invItem.ItemId == default)
                {
                    invItem.Set(tradeItem);
                }
                invItem.Quantity = newQuantity;

                // If we drained the inventory item's stack with that transaction, remove the inventory item from the inventory
                if (amountToGive >= tradeItem.Quantity)
                {
                    tradeItem.Set(Item.None);
                }
                // Otherwise, just reduce its quantity
                else
                {
                    tradeItem.Quantity -= amountToGive;
                }

                // Aaaand tell the client, provided any amount got given
                if (amountToGive > 0)
                {
                    PacketSender.SendInventoryItemUpdate(this, invItem.Slot);
                    PacketSender.SendTradeUpdate(this, this, offerIdx);
                    PacketSender.SendTradeUpdate(Trading.Counterparty, this, offerIdx);
                }
            } // repeat until we've either filled the bag or fulfilled our stack requirements

            return amountRemainder <= 0;
        }

        /// <summary>
        /// Fills an inventory with items from a <see cref="BagSlot"/>, mostly making sure stacks play along correctly
        /// </summary>
        /// <param name="bag">The <see cref="Bag"/> we're pulling from</param>
        /// <param name="bagSlotIdx">The index of the bag that we're choosing to withrdaw</param>
        /// <param name="inventorySlots">A list of inventory slots that are valid locations for the withdrawal</param>
        /// <param name="itemDescriptor">The <see cref="ItemBase"/> of the item that is getting moved around.</param>
        /// <param name="amountToGive">How many of the item that we're moving, if stackable.</param>
        /// <returns>A <see cref="bool"/> saying whether or not we successfully deposited ALL the items</returns>
        private bool TryFillInventoryStacksOfItemFromBagSlot(Bag bag, int bagSlotIdx, List<InventorySlot> inventorySlots, ItemBase itemDescriptor, int amountToGive = 1)
        {
            int amountRemainder = amountToGive;
            var bagSlots = bag.Slots;
            foreach (var inventorySlotsWithItem in inventorySlots)
            {
                // If we've fulfilled our stacking desires, we're done
                if (amountRemainder < 1 || FindOpenInventorySlots().Count < 1)
                {
                    return amountRemainder < 1;
                }
                var currSlot = FindInventoryItemSlotIndex(inventorySlotsWithItem);

                // Otherwise, first update how many of our item we still need to put in this current slot
                amountToGive = amountRemainder;
                var maxDiff = itemDescriptor.MaxInventoryStack - inventorySlotsWithItem.Quantity;
                amountRemainder = amountToGive - maxDiff;
                amountToGive = MathHelper.Clamp(amountToGive, 0, maxDiff);

                // Then, determine what the slots _new_ quantity should be
                var newQuantity = MathHelper.Clamp(inventorySlotsWithItem.Quantity + amountToGive, 0, itemDescriptor.MaxInventoryStack);
                // If the slot we're going to fill is empty, give it the item from the inventory
                if (inventorySlotsWithItem.ItemId == default)
                {
                    inventorySlotsWithItem.Set(bagSlots[bagSlotIdx]);
                }
                inventorySlotsWithItem.Quantity = newQuantity;

                // If we drained the inventory item's stack with that transaction, remove the inventory item from the inventory
                if (amountToGive >= bagSlots[bagSlotIdx].Quantity)
                {
                    bagSlots[bagSlotIdx].Set(Item.None);
                }
                // Otherwise, just reduce its quantity
                else
                {
                    bagSlots[bagSlotIdx].Quantity -= amountToGive;
                }

                // Aaaand tell the client (if we actually dumped any amount of the item)
                if (amountToGive > 0)
                {
                    PacketSender.SendInventoryItemUpdate(this, currSlot);
                    PacketSender.SendBagUpdate(this, bagSlotIdx, bagSlots[bagSlotIdx]);
                }
            } // repeat until we've either filled the bag or fulfilled our stack requirements

            return amountRemainder < 1;
        }

        /// <summary>
        /// Fills a bag with items from a <see cref="InventorySlot"/>, mostly making sure stacks play along correctly
        /// </summary>
        /// <param name="bag">The <see cref="Bag"/> we're putting items into from</param>
        /// <param name="inventorySlotIdx">The index of the players inventory that we're withrdawing from</param>
        /// <param name="bagSlots">A list of valid bag slots that could ccontain the item</param>
        /// <param name="itemDescriptor">The <see cref="ItemBase"/> of the item that is getting moved around.</param>
        /// <param name="amountToGive">How many of the item that we're moving, if stackable.</param>
        /// <returns>A <see cref="bool"/> saying whether or not we successfully deposited ALL the items</returns>
        private bool TryFillBagStacksOfItemFromInventorySlot(Bag bag, int inventorySlotIdx, List<BagSlot> bagSlots, ItemBase itemDescriptor, int amountToGive = 1)
        {
            int amountRemainder = amountToGive;
            foreach (var bagSlotWithItem in bagSlots)
            {
                if (amountRemainder < 1 || bag.FindOpenBagSlots().Count < 1)
                {
                    return amountRemainder < 1;
                }
                var currSlot = bag.FindSlotIndex(bagSlotWithItem);

                amountToGive = amountRemainder;
                var maxDiff = itemDescriptor.MaxInventoryStack - bagSlotWithItem.Quantity;
                amountRemainder = amountToGive - maxDiff;
                amountToGive = MathHelper.Clamp(amountToGive, 0, maxDiff);

                var newQuantity = MathHelper.Clamp(bagSlotWithItem.Quantity + amountToGive, 0, itemDescriptor.MaxInventoryStack);
                if (bagSlotWithItem.ItemId == default)
                {
                    bagSlotWithItem.Set(Items[inventorySlotIdx]);
                }
                bagSlotWithItem.Quantity = newQuantity;

                if (amountToGive >= Items[inventorySlotIdx].Quantity)
                {
                    Items[inventorySlotIdx].Set(Item.None);
                    EquipmentProcessItemLoss(inventorySlotIdx);
                }
                else
                {
                    Items[inventorySlotIdx].Quantity -= amountToGive;
                }

                if (amountToGive > 0)
                {
                    PacketSender.SendInventoryItemUpdate(this, inventorySlotIdx);
                    PacketSender.SendBagUpdate(this, currSlot, bagSlotWithItem);
                }
            }

            return amountRemainder < 1;
        }

        public void StoreBagItem(int slot, int amount, int bagSlot)
        {
            if (InBag == null || !HasBag(InBag))
            {
                return;
            }

            var inventoryItem = Items[slot];
            var itemBase = inventoryItem.Descriptor;
            var bag = GetBag();

            if (itemBase == null || bag == null || inventoryItem.ItemId == default)
            {
                return;
            }

            if (!itemBase.CanBag)
            {
                PacketSender.SendChatMsg(this, Strings.Items.nobag, ChatMessageType.Inventory, CustomColors.Items.Bound);
                return;
            }

            //Make Sure we are not Storing a Bag inside of itself
            if (inventoryItem.Bag == InBag)
            {
                PacketSender.SendChatMsg(this, Strings.Bags.baginself, ChatMessageType.Inventory, CustomColors.Alerts.Error);

                return;
            }
            if (itemBase.ItemType == ItemTypes.Bag)
            {
                PacketSender.SendChatMsg(this, Strings.Bags.baginbag, ChatMessageType.Inventory, CustomColors.Alerts.Error);

                return;
            }

            bool specificSlot = bagSlot != -1;
            // Sanitize amount
            if (itemBase.IsStackable)
            {
                if (amount >= inventoryItem.Quantity)
                {
                    amount = Math.Min(inventoryItem.Quantity, inventoryItem.Descriptor.MaxInventoryStack);
                }
            }
            else
            {
                amount = 1;
            }

            // Sanitize currSlot - this is the slot we want to fill
            int currSlot = 0;
            // First, we'll get our slots in the order we wish to fill them - prioritizing the user's requested slot, otherwise going from the first instance of the item found
            var relevantSlots = new List<BagSlot>();
            if (specificSlot)
            {
                currSlot = bagSlot;
                var requestedSlot = bag.Slots[currSlot];
                // If the slot we're trying to fill is occupied...
                if (requestedSlot.ItemId != default && (!itemBase.IsStackable || requestedSlot.ItemId != itemBase.Id))
                {
                    // Alert the user
                    PacketSender.SendChatMsg(this, Strings.Bags.SlotOccupied, ChatMessageType.Inventory, CustomColors.Alerts.Error);
                    return;
                }

                relevantSlots.Add(requestedSlot);
            }
            // If the item is stackable, add slots that contain that item into the mix
            if (itemBase.IsStackable)
            {
                relevantSlots.AddRange(bag.FindBagItemSlots(itemBase.Id));
            }
            // And last, add any and all open slots as valid locations for this item, if need be
            relevantSlots.AddRange(bag.FindOpenBagSlots());
            relevantSlots.Select(sl => sl).Distinct();

            // Otherwise, fill in the empty slots as much as possible
            if (!TryFillBagStacksOfItemFromInventorySlot(bag, slot, relevantSlots, itemBase, amount))
            {
                // If we're STILL not done, alert the user that we didn't have enough slots
                PacketSender.SendChatMsg(this, Strings.Bags.bagnospace, ChatMessageType.Inventory, CustomColors.Alerts.Error);
            }
        }

        public void RetrieveBagItem(int slot, int amount, int invSlot)
        {
            if (InBag == null || !HasBag(InBag))
            {
                return;
            }

            var bag = GetBag();
            if (bag == null || slot > bag.Slots.Count || bag.Slots[slot] == null)
            {
                return;
            }

            var itemBase = bag.Slots[slot].Descriptor;
            var inventorySlot = -1;
            if (itemBase == null || bag.Slots[slot] == null || bag.Slots[slot].ItemId == Guid.Empty)
            {
                return;
            }

            // Sanitize amounts
            if (itemBase.IsStackable)
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

            // Sanitize currSlot - this is the slot we want to fill
            int currSlot = 0;
            // First, we'll get our slots in the order we wish to fill them - prioritizing the user's requested slot, otherwise going from the first instance of the item found
            var relevantSlots = new List<InventorySlot>();
            bool specificSlot = invSlot != -1;
            if (specificSlot)
            {
                currSlot = invSlot;
                var requestedSlot = Items[currSlot];
                // If the slot we're trying to fill is occupied...
                if (requestedSlot.ItemId != default && (!itemBase.IsStackable || requestedSlot.ItemId != itemBase.Id))
                {
                    // Alert the user
                    PacketSender.SendChatMsg(this, Strings.Bags.SlotOccupied, ChatMessageType.Inventory, CustomColors.Alerts.Error);
                    return;
                }

                relevantSlots.Add(requestedSlot);
            }
            // If the item is stackable, add slots that contain that item into the mix
            if (itemBase.IsStackable)
            {
                relevantSlots.AddRange(FindInventoryItemSlots(itemBase.Id));
            }
            // And last, add any and all open slots as valid locations for this item, if need be
            relevantSlots.AddRange(FindOpenInventorySlots());
            relevantSlots.Select(sl => sl).Distinct();

            // Otherwise, fill in the empty slots as much as possible
            if (!TryFillInventoryStacksOfItemFromBagSlot(bag, slot, relevantSlots, itemBase, amount))
            {
                // If we're STILL not done, alert the user that we didn't have enough slots
                PacketSender.SendChatMsg(this, Strings.Bags.withdrawinvalid, ChatMessageType.Inventory, CustomColors.Alerts.Error);
            }
        }

        public void SwapBagItems(int item1, int item2)
        {
            if (InBag == null || !HasBag(InBag))
            {
                return;
            }

            var bag = GetBag();
            Item tmpInstance = null;
            if (bag.Slots[item2] != null)
            {
                tmpInstance = bag.Slots[item2].Clone();
            }

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

            PacketSender.SendBagUpdate(this, item1, bag.Slots[item1]);
            PacketSender.SendBagUpdate(this, item2, bag.Slots[item2]);
        }

        //Friends
        public void FriendRequest(Player fromPlayer)
        {
            if (fromPlayer.FriendRequests.ContainsKey(this))
            {
                fromPlayer.FriendRequests.Remove(this);
            }

            if (!FriendRequests.ContainsKey(fromPlayer) || !(FriendRequests[fromPlayer] > Timing.Global.Milliseconds))
            {
                if (!IsBusy())
                {
                    FriendRequester = fromPlayer;
                    PacketSender.SendFriendRequest(this, fromPlayer);
                    PacketSender.SendChatMsg(fromPlayer, Strings.Friends.sent, ChatMessageType.Friend, CustomColors.Alerts.RequestSent);
                }
                else
                {
                    PacketSender.SendChatMsg(
                        fromPlayer, Strings.Friends.busy.ToString(Name), ChatMessageType.Friend, CustomColors.Alerts.Error
                    );
                }
            }
        }

        public bool HasFriend(string name)
        {
            return CachedFriends.Values.Any(f => string.Equals(f, name,StringComparison.OrdinalIgnoreCase));
        }

        public Guid GetFriendId(string name)
        {
            var friend = CachedFriends.FirstOrDefault(f => string.Equals(f.Value, name, StringComparison.OrdinalIgnoreCase));
            if (friend.Value != null)
            {
                return friend.Key;
            }
            return Guid.Empty;
        }

        //Trading
        public void InviteToTrade(Player fromPlayer)
        {
            if (Trading.Requests == null)
            {
                Trading = new Trading(this);
            }

            if (fromPlayer.Trading.Requests == null)
            {
                fromPlayer.Trading = new Trading(fromPlayer);
            }

            if (fromPlayer.Trading.Requests.ContainsKey(this))
            {
                fromPlayer.Trading.Requests.Remove(this);
            }

            if (Trading.Requests.ContainsKey(fromPlayer) && Trading.Requests[fromPlayer] > Timing.Global.Milliseconds)
            {
                PacketSender.SendChatMsg(fromPlayer, Strings.Trading.alreadydenied, ChatMessageType.Trading, CustomColors.Alerts.Error);
            }
            else
            {
                if (!IsBusy())
                {
                    Trading.Requester = fromPlayer;
                    PacketSender.SendTradeRequest(this, fromPlayer);
                }
                else
                {
                    PacketSender.SendChatMsg(
                        fromPlayer, Strings.Trading.busy.ToString(Name), ChatMessageType.Trading, CustomColors.Alerts.Error
                    );
                }
            }
        }

        public void OfferItem(int slot, int amount)
        {
            // TODO: Accessor cleanup
            if (Trading.Counterparty == null)
            {
                return;
            }

            var itemBase = Items[slot].Descriptor;
            if (itemBase != null)
            {
                if (Items[slot].ItemId != Guid.Empty)
                {
                    if (itemBase.IsStackable)
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

                    //Check if the item is bound.. if so don't allow trade
                    if (!itemBase.CanTrade)
                    {
                        PacketSender.SendChatMsg(this, Strings.Bags.tradebound, ChatMessageType.Trading, CustomColors.Items.Bound);

                        return;
                    }

                    //Check if this is a bag with items.. if so don't allow sale
                    if (itemBase.ItemType == ItemTypes.Bag)
                    {
                        if (Items[slot].TryGetBag(out var bag))
                        {
                            if (!bag.IsEmpty)
                            {
                                PacketSender.SendChatMsg(this, Strings.Bags.onlytradeempty, ChatMessageType.Trading, CustomColors.Alerts.Error);
                                return;
                            }
                        }
                    }

                    //Find a spot in the trade for it!
                    if (itemBase.IsStackable)
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

                                PacketSender.SendInventoryItemUpdate(this, slot);
                                PacketSender.SendTradeUpdate(this, this, i);
                                PacketSender.SendTradeUpdate(Trading.Counterparty, this, i);

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

                            PacketSender.SendInventoryItemUpdate(this, slot);
                            PacketSender.SendTradeUpdate(this, this, i);
                            PacketSender.SendTradeUpdate(Trading.Counterparty, this, i);

                            return;
                        }
                    }

                    PacketSender.SendChatMsg(this, Strings.Trading.tradenospace, ChatMessageType.Trading, CustomColors.Alerts.Error);
                }
                else
                {
                    PacketSender.SendChatMsg(this, Strings.Trading.offerinvalid, ChatMessageType.Trading, CustomColors.Alerts.Error);
                }
            }
        }

        public void RevokeItem(int slot, int amount)
        {
            if (Trading.Counterparty == null)
            {
                return;
            }

            if (slot < 0 || slot >= Trading.Offer.Length || Trading.Offer[slot] == null)
            {
                return;
            }

            var itemBase = Trading.Offer[slot].Descriptor;
            if (itemBase == null)
            {
                return;
            }

            if (Trading.Offer[slot] == null || Trading.Offer[slot].ItemId == Guid.Empty)
            {
                PacketSender.SendChatMsg(this, Strings.Trading.revokeinvalid, ChatMessageType.Trading, CustomColors.Alerts.Error);

                return;
            }

            // Sanitize amounts
            var inventorySlot = -1;
            var stackable = itemBase.IsStackable;
            var tradeItem = Trading.Offer[slot];
            if (stackable)
            {
                if (amount >= tradeItem.Quantity)
                {
                    amount = tradeItem.Quantity;
                }
            }
            else
            {
                amount = 1;
            }

            if (Trading.Counterparty.Trading.Accepted || Trading.Accepted)
            {
                PacketSender.SendChatMsg(this, Strings.Trading.RevokeNotAllowed, ChatMessageType.Trading, CustomColors.Alerts.Error);

                return;
            }

            // Sanitize currSlot - this is the slot we want to fill
            int currSlot = 0;
            // First, we'll get our slots in the order we wish to fill them - prioritizing the user's requested slot, otherwise going from the first instance of the item found
            var relevantSlots = new List<InventorySlot>();
            // If the item is stackable, add slots that contain that item into the mix
            if (itemBase.IsStackable)
            {
                relevantSlots.AddRange(FindInventoryItemSlots(itemBase.Id));
            }
            // And last, add any and all open slots as valid locations for this item, if need be
            relevantSlots.AddRange(FindOpenInventorySlots());
            relevantSlots.Select(sl => sl).Distinct();

            // Otherwise, fill in the empty slots as much as possible
            if (!TryFillInventoryStacksOfItemFroTradeOffer(tradeItem, slot, relevantSlots, itemBase, amount))
            {
                // If we're STILL not done, alert the user that we didn't have enough slots
                PacketSender.SendChatMsg(this, Strings.Bags.withdrawinvalid, ChatMessageType.Inventory, CustomColors.Alerts.Error);
            }
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

                if (!TryGiveItem(offer, -1) && MapController.TryGetInstanceFromMap(MapId, MapInstanceId, out var instance))
                {
                    instance.SpawnItem(X, Y, offer, offer.Quantity, Id);
                    PacketSender.SendChatMsg(this, Strings.Trading.itemsdropped, ChatMessageType.Inventory, CustomColors.Alerts.Error);
                }

                offer.ItemId = Guid.Empty;
                offer.Quantity = 0;
            }

            PacketSender.SendInventory(this);
        }

        public void CancelTrade()
        {
            if (Trading.Counterparty == null)
            {
                return;
            }

            Trading.Counterparty.ReturnTradeItems();
            PacketSender.SendChatMsg(Trading.Counterparty, Strings.Trading.declined, ChatMessageType.Trading, CustomColors.Alerts.Error);
            PacketSender.SendTradeClose(Trading.Counterparty);
            Trading.Counterparty.Trading.Counterparty = null;

            ReturnTradeItems();
            PacketSender.SendChatMsg(this, Strings.Trading.declined, ChatMessageType.Trading, CustomColors.Alerts.Error);
            PacketSender.SendTradeClose(this);
            Trading.Counterparty = null;
        }

        //Parties
        public void InviteToParty(Player fromPlayer)
        {
            if (Party.Count != 0)
            {
                PacketSender.SendChatMsg(fromPlayer, Strings.Parties.inparty.ToString(Name), ChatMessageType.Party, CustomColors.Alerts.Error);

                return;
            }

            if (fromPlayer.PartyRequests.ContainsKey(this))
            {
                fromPlayer.PartyRequests.Remove(this);
            }

            if (PartyRequests.ContainsKey(fromPlayer) && PartyRequests[fromPlayer] > Timing.Global.Milliseconds)
            {
                PacketSender.SendChatMsg(fromPlayer, Strings.Parties.alreadydenied, ChatMessageType.Party, CustomColors.Alerts.Error);
            }
            else
            {
                if (!IsBusy())
                {
                    PartyRequester = fromPlayer;
                    PacketSender.SendPartyInvite(this, fromPlayer);
                }
                else
                {
                    PacketSender.SendChatMsg(
                        fromPlayer, Strings.Parties.busy.ToString(Name), ChatMessageType.Party, CustomColors.Alerts.Error
                    );
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
                    PacketSender.SendChatMsg(this, Strings.Parties.leaderinvonly, ChatMessageType.Party, CustomColors.Alerts.Error);

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

            if (Party.Count < Options.Party.MaximumMembers)
            {
                target.LeaveParty();
                Party.Add(target);

                //Update all members of the party with the new list
                for (var i = 0; i < Party.Count; i++)
                {
                    Party[i].Party = Party;
                    PacketSender.SendParty(Party[i]);
                    PacketSender.SendChatMsg(
                        Party[i], Strings.Parties.joined.ToString(target.Name), ChatMessageType.Party, CustomColors.Alerts.Accepted
                    );
                }
            }
            else
            {
                PacketSender.SendChatMsg(this, Strings.Parties.limitreached, ChatMessageType.Party, CustomColors.Alerts.Error);
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
                        PacketSender.SendParty(oldMember);
                        PacketSender.SendChatMsg(oldMember, Strings.Parties.kicked, ChatMessageType.Party, CustomColors.Alerts.Error);
                        Party.Remove(oldMember);

                        if (Party.Count > 1) //Need atleast 2 party members to function
                        {
                            //Update all members of the party with the new list
                            for (var i = 0; i < Party.Count; i++)
                            {
                                Party[i].Party = Party;
                                PacketSender.SendParty(Party[i]);
                                PacketSender.SendChatMsg(
                                    Party[i], Strings.Parties.memberkicked.ToString(oldMember.Name),
                                    ChatMessageType.Party,
                                    CustomColors.Alerts.Error
                                );
                            }
                        }
                        else if (Party.Count > 0) //Check if anyone is left on their own
                        {
                            var remainder = Party[0];
                            remainder.Party.Clear();
                            PacketSender.SendParty(remainder);
                            PacketSender.SendChatMsg(remainder, Strings.Parties.disbanded, ChatMessageType.Party, CustomColors.Alerts.Error);
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
                        PacketSender.SendParty(Party[i]);
                        PacketSender.SendChatMsg(
                            Party[i], Strings.Parties.memberleft.ToString(oldMember.Name), ChatMessageType.Party, CustomColors.Alerts.Error
                        );
                    }
                }
                else if (Party.Count > 0) //Check if anyone is left on their own
                {
                    var remainder = Party[0];
                    remainder.Party.Clear();
                    PacketSender.SendParty(remainder);
                    PacketSender.SendChatMsg(remainder, Strings.Parties.disbanded, ChatMessageType.Party, CustomColors.Alerts.Error);
                }

                PacketSender.SendChatMsg(this, Strings.Parties.left, ChatMessageType.Party, CustomColors.Alerts.Error);
            }

            Party = new List<Player>();
            PacketSender.SendParty(this);
        }

        public bool InParty(Player member)
        {
            return Party.Contains(member);
        }

        public void StartTrade(Player target)
        {
            if (target?.Trading.Counterparty != null)
            {
                return;
            }

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
            PacketSender.StartTrade(target, this);
            PacketSender.StartTrade(this, target);
        }

        //Spells
        public bool TryTeachSpell(Spell spell, bool sendUpdate = true)
        {
            if (spell == null || spell.SpellId == Guid.Empty)
            {
                return false;
            }

            if (KnowsSpell(spell.SpellId))
            {
                return false;
            }

            if (SpellBase.Get(spell.SpellId) == null)
            {
                return false;
            }

            for (var i = 0; i < Options.MaxPlayerSkills; i++)
            {
                if (Spells[i].SpellId == Guid.Empty)
                {
                    Spells[i].Set(spell);
                    if (sendUpdate)
                    {
                        PacketSender.SendPlayerSpellUpdate(this, i);
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
            PacketSender.SendPlayerSpellUpdate(this, spell1);
            PacketSender.SendPlayerSpellUpdate(this, spell2);
        }

        public void ForgetSpell(int spellSlot)
        {
            if (!SpellBase.Get(Spells[spellSlot].SpellId).Bound)
            {
                Spells[spellSlot].Set(Spell.None);
                PacketSender.SendPlayerSpellUpdate(this, spellSlot);
            }
            else
            {
                PacketSender.SendChatMsg(this, Strings.Combat.tryforgetboundspell, ChatMessageType.Spells);
            }
        }

        public bool TryForgetSpell(Spell spell, bool sendUpdate = true)
        {
            Spell slot = null;
            var slotIndex = -1;

            for (var index = 0; index < Spells.Count; ++index)
            {
                var spellSlot = Spells[index];

                // Avoid continue;
                // ReSharper disable once InvertIf
                if (spellSlot?.SpellId == spell.SpellId)
                {
                    slot = spellSlot;
                    slotIndex = index;

                    break;
                }
            }

            if (slot == null)
            {
                return false;
            }

            var spellBase = SpellBase.Get(spell.SpellId);
            if (spellBase == null)
            {
                return false;
            }

            if (spellBase.Bound)
            {
                PacketSender.SendChatMsg(this, Strings.Combat.tryforgetboundspell, ChatMessageType.Spells);

                return false;
            }

            slot.Set(Spell.None);
            PacketSender.SendPlayerSpellUpdate(this, slotIndex);

            return true;
        }

        public override bool IsAllyOf(Entity otherEntity)
        {
            switch (otherEntity)
            {
                case Player otherPlayer:
                    return IsAllyOf(otherPlayer);
                case Npc otherNpc:
                    return otherNpc.IsAllyOf(this);
                default:
                    return base.IsAllyOf(otherEntity);
            }
        }

        public virtual bool IsAllyOf(Player otherPlayer)
        {
            return this.InParty(otherPlayer) || this == otherPlayer;
        }

        public override bool CanCastSpell(SpellBase spell, Entity target, bool checkVitalReqs, out SpellCastFailureReason reason)
        {
            // Do we fail our base class check? If so cancel out with a reason!
            if (!base.CanCastSpell(spell, target, checkVitalReqs, out reason))
            {
                // Let our user know the reason if configured.
                if (Options.Combat.EnableCombatChatMessages)
                {
                    switch (reason)
                    {
                        case SpellCastFailureReason.InsufficientMP:
                            PacketSender.SendChatMsg(this, Strings.Combat.lowmana, ChatMessageType.Combat);
                            break;
                        case SpellCastFailureReason.InsufficientHP:
                            PacketSender.SendChatMsg(this, Strings.Combat.lowhealth, ChatMessageType.Combat);
                            break;
                        case SpellCastFailureReason.InvalidTarget:
                            PacketSender.SendChatMsg(this, Strings.Combat.InvalidTarget, ChatMessageType.Combat);
                            break;
                        case SpellCastFailureReason.Silenced:
                            PacketSender.SendChatMsg(this, Strings.Combat.silenced, ChatMessageType.Combat);
                            break;
                        case SpellCastFailureReason.Stunned:
                            PacketSender.SendChatMsg(this, Strings.Combat.stunned, ChatMessageType.Combat);
                            break;
                        case SpellCastFailureReason.Asleep:
                            PacketSender.SendChatMsg(this, Strings.Combat.sleep, ChatMessageType.Combat);
                            break;
                        case SpellCastFailureReason.Snared:
                            PacketSender.SendChatMsg(this, Strings.Combat.Snared, ChatMessageType.Combat);
                            break;
                        case SpellCastFailureReason.OutOfRange:
                            PacketSender.SendChatMsg(this, Strings.Combat.targetoutsiderange, ChatMessageType.Combat);
                            break;
                        case SpellCastFailureReason.OnCooldown:
                            PacketSender.SendChatMsg(this, Strings.Combat.cooldown, ChatMessageType.Combat);
                            break;
                    }
                }
                return false;
            }

            // Check for dynamic conditions.
            if (!Conditions.MeetsConditionLists(spell.CastingRequirements, this, null))
            {
                if (!string.IsNullOrWhiteSpace(spell.CannotCastMessage))
                {
                    PacketSender.SendChatMsg(this, spell.CannotCastMessage, ChatMessageType.Error);
                }
                else
                {
                    PacketSender.SendChatMsg(this, Strings.Combat.dynamicreq, ChatMessageType.Spells);
                }

                reason = SpellCastFailureReason.ConditionsNotMet;
                return false;
            }

            // If this is a projectile, check if the user has the corresponding item.
            if (spell.SpellType == SpellTypes.CombatSpell &&
                spell.Combat.TargetType == SpellTargetTypes.Projectile &&
                spell.Combat.ProjectileId != Guid.Empty)
            {
                var projectileBase = spell.Combat.Projectile;
                if (projectileBase == null)
                {
                    reason = SpellCastFailureReason.InvalidProjectile;
                    return false;
                }

                if (projectileBase.AmmoItemId != Guid.Empty)
                {
                    if (FindInventoryItemSlot(projectileBase.AmmoItemId, projectileBase.AmmoRequired) == null)
                    {
                        if (Options.Combat.EnableCombatChatMessages)
                        {
                            PacketSender.SendChatMsg(
                                this, Strings.Items.notenough.ToString(ItemBase.GetName(projectileBase.AmmoItemId)),
                                ChatMessageType.Inventory,
                                CustomColors.Alerts.Error
                            );
                        }

                        reason = SpellCastFailureReason.InsufficientItems;
                        return false;
                    }
                }
            }

            // We have no reason to stop this player from casting!
            return true;
        }

        public void UseSpell(int spellSlot, Entity target)
        {
            var spellNum = Spells[spellSlot].SpellId;
            Target = target;
            var spell = SpellBase.Get(spellNum);
            if (spell == null)
            {
                return;
            }

            if (!CanCastSpell(spell, target, true, out var _))
            {
                return;
            }


            if (CastTime == 0)
            {
                CastTime = Timing.Global.Milliseconds + spell.CastDuration;

                //Remove stealth status.
                foreach (var status in CachedStatuses)
                {
                    if (status.Type == StatusTypes.Stealth)
                    {
                        status.RemoveStatus();
                    }
                }

                SpellCastSlot = spellSlot;
                CastTarget = Target;

                //Check if the caster has the right ammunition if a projectile
                if (spell.SpellType == SpellTypes.CombatSpell &&
                    spell.Combat.TargetType == SpellTargetTypes.Projectile &&
                    spell.Combat.ProjectileId != Guid.Empty)
                {
                    var projectileBase = spell.Combat.Projectile;
                    if (projectileBase != null && projectileBase.AmmoItemId != Guid.Empty)
                    {
                        TryTakeItem(projectileBase.AmmoItemId, projectileBase.AmmoRequired);
                    }
                }

                    if (spell.CastAnimationId != Guid.Empty)
                    {
                        PacketSender.SendAnimationToProximity(
                            spell.CastAnimationId, 1, base.Id, MapId, 0, 0, (sbyte) Dir, MapInstanceId
                        ); //Target Type 1 will be global entity
                    }

                // Check if the player isn't casting a spell already.
                if (!IsCasting)
                {
                    // Player is not casting a spell, cast now!
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
                if (Options.Combat.EnableCombatChatMessages)
                {
                    PacketSender.SendChatMsg(this, Strings.Combat.channeling, ChatMessageType.Combat);
                }
            }
        }

        public override void CastSpell(Guid spellId, int spellSlot = -1)
        {
            var spellBase = SpellBase.Get(spellId);
            if (spellBase == null)
            {
                return;
            }

            switch (spellBase.SpellType)
            {
                case SpellTypes.Event:
                    {
                        var evt = spellBase.Event;
                        if (evt != null)
                        {
                            StartCommonEvent(evt);
                        }

                        base.CastSpell(spellId, spellSlot); //To get cooldown :P

                        break;
                    }
                default:
                    base.CastSpell(spellId, spellSlot);

                    break;
            }
        }

        //Equipment
        public void EquipItem(ItemBase itemBase, int slot = -1)
        {
            if (itemBase == null)
            {
                return;
            }

            if (itemBase.ItemType == ItemTypes.Equipment)
            {
                if (slot == -1)
                {
                    for (var i = 0; i < Options.MaxInvItems; i++)
                    {
                        var tempItemBase = Items[i].Descriptor;
                        if (itemBase != null)
                        {
                            if (itemBase == tempItemBase)
                            {
                                slot = i;

                                break;
                            }
                        }
                    }
                }

                if (slot != -1)
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
                                var item = Items[Equipment[Options.WeaponIndex]].Descriptor;
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
                StartCommonEventsWithTrigger(CommonEventTrigger.EquipChange);
                PacketSender.SendPlayerEquipmentToProximity(this);
                PacketSender.SendEntityStats(this);
            }
        }

        public void UnequipItem(Guid itemId, bool sendUpdate = true)
        {
            var updated = false;
            for (int i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                var itemSlot = Equipment[i];
                if (itemSlot > -1 && Items[itemSlot]?.ItemId == itemId)
                {
                    Equipment[i] = -1;
                    updated = true;
                }
            }
            if (updated)
            {
                FixVitals();
                StartCommonEventsWithTrigger(CommonEventTrigger.EquipChange);
                if (sendUpdate)
                {
                    PacketSender.SendPlayerEquipmentToProximity(this);
                    PacketSender.SendEntityStats(this);
                }
            }
        }

        public void UnequipItem(int slot, bool sendUpdate = true)
        {
            Equipment[slot] = -1;
            FixVitals();
            StartCommonEventsWithTrigger(CommonEventTrigger.EquipChange);

            if (sendUpdate)
            {
                PacketSender.SendPlayerEquipmentToProximity(this);
                PacketSender.SendEntityStats(this);
            }
        }

        public void EquipmentProcessItemSwap(int item1, int item2)
        {
            for (var i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                if (Equipment[i] == item1)
                {
                    Equipment[i] = item2;
                }
                else if (Equipment[i] == item2)
                {
                    Equipment[i] = item1;
                }
            }

            FixVitals();
            PacketSender.SendPlayerEquipmentToProximity(this);
            PacketSender.SendEntityStats(this);
        }

        public void EquipmentProcessItemLoss(int slot)
        {
            var changed = false;
            for (var i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                if (Equipment[i] == slot)
                {
                    changed |= Equipment[i] != -1;
                    Equipment[i] = -1;
                }
            }


            if (changed)
            {
                FixVitals();
                StartCommonEventsWithTrigger(CommonEventTrigger.EquipChange);
                PacketSender.SendPlayerEquipmentToProximity(this);
                PacketSender.SendEntityStats(this);
            }
        }

        /// <summary>
        /// Unequips any items that the player is currently wearing in which they no longer meet the conditions for
        /// Also unequips any items that are not actually equipment anymore
        /// </summary>
        public void UnequipInvalidItems()
        {
            var updated = false;
            for (int i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                var itemSlot = Equipment[i];
                if (itemSlot < 0)
                {
                    continue;
                }

                var item = Items[itemSlot];
                var descriptor = item?.Descriptor;

                if (descriptor == default ||
                    descriptor.ItemType != ItemTypes.Equipment ||
                    !Conditions.MeetsConditionLists(descriptor.UsageRequirements, this, null))
                {
                    Equipment[i] = -1;
                    updated = true;
                }
            }
            if (updated)
            {
                FixVitals();
                StartCommonEventsWithTrigger(CommonEventTrigger.EquipChange);
                PacketSender.SendPlayerEquipmentToProximity(this);
                PacketSender.SendEntityStats(this);
            }
        }

        public void StartCommonEventsWithTrigger(CommonEventTrigger trigger, string command = "", string param = "")
        {
            foreach (var value in EventBase.Lookup.Values)
            {
                if (value is EventBase eventDescriptor && eventDescriptor.Pages.Any(p => p.CommonTrigger == trigger))
                {
                    StartCommonEvent(eventDescriptor, trigger, command, param);
                }
            }
        }

        public static void StartCommonEventsWithTriggerForAll(CommonEventTrigger trigger, string command = "", string param = "")
        {
            var players = Player.OnlineList;
            foreach (var value in EventBase.Lookup.Values)
            {
                if (value is EventBase eventDescriptor && eventDescriptor.Pages.Any(p => p.CommonTrigger == trigger))
                {
                    foreach (var player in players)
                    {
                        player.StartCommonEvent(eventDescriptor, trigger, command, param);
                    }
                }
            }
        }

        //Stats
        public void UpgradeStat(int statIndex)
        {
            if (Stat[statIndex].BaseStat + StatPointAllocations[statIndex] < Options.MaxStatValue && StatPoints > 0)
            {
                StatPointAllocations[statIndex]++;
                StatPoints--;
                PacketSender.SendEntityStats(this);
                PacketSender.SendPointsTo(this);
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
            if (!Conditions.MeetsConditionLists(quest.Requirements, this, null, true, quest))
            {
                return false;
            }

            if (quest.Tasks.Count == 0)
            {
                return false;
            }

            return true;
        }

        public bool QuestCompleted(Guid questId)
        {
            var questProgress = FindQuest(questId);
            if (questProgress != null)
            {
                if (questProgress.Completed)
                {
                    return true;
                }
            }

            return false;
        }

        public bool QuestInProgress(Guid questId, QuestProgressState progress, Guid taskId)
        {
            var questProgress = FindQuest(questId);
            if (questProgress != null)
            {
                var quest = QuestBase.Get(questId);
                if (quest != null)
                {
                    if (questProgress.TaskId != Guid.Empty && quest.GetTaskIndex(questProgress.TaskId) != -1)
                    {
                        switch (progress)
                        {
                            case QuestProgressState.OnAnyTask:
                                return true;
                            case QuestProgressState.BeforeTask:
                                if (quest.GetTaskIndex(taskId) != -1)
                                {
                                    return quest.GetTaskIndex(taskId) > quest.GetTaskIndex(questProgress.TaskId);
                                }

                                break;
                            case QuestProgressState.OnTask:
                                if (quest.GetTaskIndex(taskId) != -1)
                                {
                                    return quest.GetTaskIndex(taskId) == quest.GetTaskIndex(questProgress.TaskId);
                                }

                                break;
                            case QuestProgressState.AfterTask:
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
                if (quest.QuestId == questId)
                {
                    return quest;
                }
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
                PacketSender.SendChatMsg(
                    this, Strings.Quests.started.ToString(quest.Name), ChatMessageType.Quest, CustomColors.QuestAlert.Started
                );

                PacketSender.SendQuestsProgress(this);
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
                        foreach (var evt in EventLookup)
                        {
                            if (evt.Value.CallStack.Count <= 0)
                            {
                                continue;
                            }

                            var stackInfo = evt.Value.CallStack.Peek();
                            if (stackInfo.WaitingForResponse != CommandInstance.EventResponse.Quest)
                            {
                                continue;
                            }

                            if (((StartQuestCommand) stackInfo.WaitingOnCommand).QuestId == questId)
                            {
                                var tmpStack = new CommandInstance(stackInfo.Page, stackInfo.BranchIds[0]);
                                evt.Value.CallStack.Peek().WaitingForResponse = CommandInstance.EventResponse.None;
                                evt.Value.CallStack.Push(tmpStack);
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
                    PacketSender.SendChatMsg(
                        this, Strings.Quests.declined.ToString(QuestBase.GetName(questId)), ChatMessageType.Quest, CustomColors.QuestAlert.Declined
                    );

                    foreach (var evt in EventLookup)
                    {
                        if (evt.Value.CallStack.Count <= 0)
                        {
                            continue;
                        }

                        var stackInfo = evt.Value.CallStack.Peek();
                        if (stackInfo.WaitingForResponse != CommandInstance.EventResponse.Quest)
                        {
                            continue;
                        }

                        if (((StartQuestCommand) stackInfo.WaitingOnCommand).QuestId == questId)
                        {
                            //Run failure branch
                            var tmpStack = new CommandInstance(stackInfo.Page, stackInfo.BranchIds[1]);
                            stackInfo.WaitingForResponse = CommandInstance.EventResponse.None;
                            evt.Value.CallStack.Push(tmpStack);
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
                if (QuestInProgress(quest.Id, QuestProgressState.OnAnyTask, Guid.Empty))
                {
                    //Cancel the quest somehow...
                    if (quest.Quitable)
                    {
                        var questProgress = FindQuest(quest.Id);
                        questProgress.TaskId = Guid.Empty;
                        questProgress.TaskProgress = -1;
                        PacketSender.SendChatMsg(
                            this, Strings.Quests.abandoned.ToString(QuestBase.GetName(questId)), ChatMessageType.Quest,
                            CustomColors.QuestAlert.Abandoned
                        );

                        PacketSender.SendQuestsProgress(this);
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
                                PacketSender.SendChatMsg(this, Strings.Quests.taskcompleted, ChatMessageType.Quest);
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
                                    PacketSender.SendChatMsg(
                                        this, Strings.Quests.completed.ToString(quest.Name), ChatMessageType.Quest,
                                        CustomColors.QuestAlert.Completed
                                    );
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

                                    PacketSender.SendChatMsg(
                                        this, Strings.Quests.updated.ToString(quest.Name),
                                        ChatMessageType.Quest,
                                        CustomColors.QuestAlert.TaskUpdated
                                    );
                                }
                            }
                        }
                    }

                    PacketSender.SendQuestsProgress(this);
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
                        PacketSender.SendChatMsg(
                            this, Strings.Quests.completed.ToString(quest.Name), ChatMessageType.Quest,
                            CustomColors.QuestAlert.Completed
                        );
                    }
                    PacketSender.SendQuestsProgress(this);
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
                                        PacketSender.SendQuestsProgress(this);
                                        PacketSender.SendChatMsg(
                                            this,
                                            Strings.Quests.itemtask.ToString(
                                                quest.Name, questProgress.TaskProgress, questTask.Quantity,
                                                ItemBase.GetName(questTask.TargetId)
                                            ),
                                            ChatMessageType.Quest
                                        );
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        //Switches and Variables
        private PlayerVariable GetSwitch(Guid id)
        {
            foreach (var s in Variables)
            {
                if (s.VariableId == id)
                {
                    return s;
                }
            }

            return null;
        }

        public bool GetSwitchValue(Guid id)
        {
            var s = GetSwitch(id);
            if (s == null)
            {
                return false;
            }

            return s.Value.Boolean;
        }

        public void SetSwitchValue(Guid id, bool value)
        {
            var s = GetSwitch(id);
            var changed = true;
            if (s != null)
            {
                if (s.Value?.Boolean == value)
                {
                    changed = false;
                }
                s.Value.Boolean = value;
            }
            else
            {
                s = new PlayerVariable(id);
                s.Value.Boolean = value;
                Variables.Add(s);
            }

            if (changed)
            {
                StartCommonEventsWithTrigger(CommonEventTrigger.PlayerVariableChange, "", id.ToString());
            }
        }

        public PlayerVariable GetVariable(Guid id, bool createIfNull = false)
        {
            foreach (var v in Variables)
            {
                if (v.VariableId == id)
                {
                    return v;
                }
            }

            if (createIfNull)
            {
                return CreateVariable(id);
            }

            return null;
        }

        private PlayerVariable CreateVariable(Guid id)
        {
            if (PlayerVariableBase.Get(id) == null)
            {
                return null;
            }

            var variable = new PlayerVariable(id);
            Variables.Add(variable);

            return variable;
        }

        public VariableValue GetVariableValue(Guid id)
        {
            var v = GetVariable(id);
            if (v == null)
            {
                v = CreateVariable(id);
            }

            if (v == null)
            {
                return new VariableValue();
            }

            return v.Value;
        }

        public void SetVariableValue(Guid id, long value)
        {
            var v = GetVariable(id);
            var changed = true;
            if (v != null)
            {
                if (v.Value?.Integer == value)
                {
                    changed = false;
                }
                v.Value.Integer = value;
            }
            else
            {
                v = new PlayerVariable(id);
                v.Value.Integer = value;
                Variables.Add(v);
            }

            if (changed)
            {
                StartCommonEventsWithTrigger(CommonEventTrigger.PlayerVariableChange, "", id.ToString());
            }
        }

        public void SetVariableValue(Guid id, string value)
        {
            var v = GetVariable(id);
            var changed = true;
            if (v != null)
            {
                if (v.Value?.String == value)
                {
                    changed = false;
                }
                v.Value.String = value;
            }
            else
            {
                v = new PlayerVariable(id);
                v.Value.String = value;
                Variables.Add(v);
            }

            if (changed)
            {
                StartCommonEventsWithTrigger(CommonEventTrigger.PlayerVariableChange, "", id.ToString());
            }
        }

        //Event Processing Methods
        public Event EventExists(MapTileLoc loc)
        {
            if (EventTileLookup.TryGetValue(loc, out Event val)) {
                return val;
            }

            return null;
        }

        public EventPageInstance EventAt(Guid mapId, int x, int y, int z)
        {
            foreach (var evt in EventLookup)
            {
                if (evt.Value != null && evt.Value.PageInstance != null)
                {
                    if (evt.Value.PageInstance.MapId == mapId &&
                        evt.Value.PageInstance.X == x &&
                        evt.Value.PageInstance.Y == y &&
                        evt.Value.PageInstance.Z == z)
                    {
                        return evt.Value.PageInstance;
                    }
                }
            }

            return null;
        }

        public void TryActivateEvent(Guid eventId)
        {
            foreach (var evt in EventLookup)
            {
                if (evt.Value.PageInstance != null && evt.Value.PageInstance.Id == eventId)
                {
                    if (evt.Value.PageInstance.Trigger != EventTrigger.ActionButton)
                    {
                        return;
                    }

                    if (!IsEventOneBlockAway(evt.Value))
                    {
                        return;
                    }

                    if (evt.Value.CallStack.Count != 0)
                    {
                        return;
                    }

                    var newStack = new CommandInstance(evt.Value.PageInstance.MyPage);
                    evt.Value.CallStack.Push(newStack);
                    if (!evt.Value.Global)
                    {
                        evt.Value.PageInstance.TurnTowardsPlayer();
                    }
                    else
                    {
                        //Turn the global event opposite of the player
                        switch (Dir)
                        {
                            case 0:
                                evt.Value.PageInstance.GlobalClone.ChangeDir(1);

                                break;
                            case 1:
                                evt.Value.PageInstance.GlobalClone.ChangeDir(0);

                                break;
                            case 2:
                                evt.Value.PageInstance.GlobalClone.ChangeDir(3);

                                break;
                            case 3:
                                evt.Value.PageInstance.GlobalClone.ChangeDir(2);

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
                foreach (var evt in EventLookup)
                {
                    if (evt.Value.PageInstance != null && evt.Value.PageInstance.Id == eventId)
                    {
                        if (evt.Value.CallStack.Count <= 0)
                        {
                            return;
                        }

                        var stackInfo = evt.Value.CallStack.Peek();
                        if (stackInfo.WaitingForResponse != CommandInstance.EventResponse.Dialogue)
                        {
                            return;
                        }

                        stackInfo.WaitingForResponse = CommandInstance.EventResponse.None;
                        if (stackInfo.WaitingOnCommand != null &&
                            stackInfo.WaitingOnCommand.Type == EventCommandType.ShowOptions)
                        {
                            var tmpStack = new CommandInstance(stackInfo.Page, stackInfo.BranchIds[responseId - 1]);
                            evt.Value.CallStack.Push(tmpStack);
                        }

                        return;
                    }
                }
            }
        }

        public void PictureClosed(Guid eventId)
        {
            lock (mEventLock)
            {
                foreach (var evt in EventLookup)
                {
                    if (evt.Value.PageInstance != null && evt.Value.PageInstance.Id == eventId)
                    {
                        if (evt.Value.CallStack.Count <= 0)
                        {
                            return;
                        }

                        var stackInfo = evt.Value.CallStack.Peek();
                        if (stackInfo.WaitingForResponse != CommandInstance.EventResponse.Picture)
                        {
                            return;
                        }

                        stackInfo.WaitingForResponse = CommandInstance.EventResponse.None;

                        return;
                    }
                }
            }
        }

        public void RespondToEventInput(Guid eventId, int newValue, string newValueString, bool canceled = false)
        {
            lock (mEventLock)
            {
                foreach (var evt in EventLookup)
                {
                    if (evt.Value.PageInstance != null && evt.Value.PageInstance.Id == eventId)
                    {
                        if (evt.Value.CallStack.Count <= 0)
                        {
                            return;
                        }

                        var stackInfo = evt.Value.CallStack.Peek();
                        if (stackInfo.WaitingForResponse != CommandInstance.EventResponse.Dialogue)
                        {
                            return;
                        }

                        stackInfo.WaitingForResponse = CommandInstance.EventResponse.None;
                        if (stackInfo.WaitingOnCommand != null &&
                            stackInfo.WaitingOnCommand.Type == EventCommandType.InputVariable)
                        {
                            var cmd = (InputVariableCommand)stackInfo.WaitingOnCommand;
                            VariableValue value = null;
                            var type = VariableDataTypes.Boolean;
                            if (cmd.VariableType == VariableTypes.PlayerVariable)
                            {
                                var variable = PlayerVariableBase.Get(cmd.VariableId);
                                if (variable != null)
                                {
                                    type = variable.Type;
                                }

                                value = GetVariableValue(cmd.VariableId);
                            }
                            else if (cmd.VariableType == VariableTypes.ServerVariable)
                            {
                                var variable = ServerVariableBase.Get(cmd.VariableId);
                                if (variable != null)
                                {
                                    type = variable.Type;
                                }

                                value = ServerVariableBase.Get(cmd.VariableId)?.Value;
                            }
                            else if (cmd.VariableType == VariableTypes.GuildVariable)
                            {
                                var variable = GuildVariableBase.Get(cmd.VariableId);
                                if (variable != null)
                                {
                                    type = variable.Type;
                                }

                                value = Guild?.GetVariableValue(cmd.VariableId) ?? new VariableValue();
                            }

                            if (value == null)
                            {
                                value = new VariableValue();
                            }

                            var success = false;
                            var changed = false;

                            if (!canceled)
                            {
                                switch (type)
                                {
                                    case VariableDataTypes.Integer:
                                        if (newValue >= cmd.Minimum && newValue <= cmd.Maximum)
                                        {
                                            if (value.Integer != newValue)
                                            {
                                                changed = true;
                                            }
                                            value.Integer = newValue;
                                            success = true;
                                        }

                                        break;
                                    case VariableDataTypes.Number:
                                        if (newValue >= cmd.Minimum && newValue <= cmd.Maximum)
                                        {
                                            if (value.Number != newValue)
                                            {
                                                changed = true;
                                            }
                                            value.Number = newValue;
                                            success = true;
                                        }

                                        break;
                                    case VariableDataTypes.String:
                                        if (newValueString.Length >= cmd.Minimum &&
                                            newValueString.Length <= cmd.Maximum)
                                        {
                                            if (value.String != newValueString)
                                            {
                                                changed = true;
                                            }
                                            value.String = newValueString;
                                            success = true;
                                        }

                                        break;
                                    case VariableDataTypes.Boolean:
                                        if (value.Boolean != newValue > 0)
                                        {
                                            changed = true;
                                        }
                                        value.Boolean = newValue > 0;
                                        success = true;

                                        break;
                                }
                            }

                            //Reassign variable values in case they didnt already exist and we made them from scratch at the null check above
                            if (cmd.VariableType == VariableTypes.PlayerVariable)
                            {
                                var variable = GetVariable(cmd.VariableId);
                                if (changed)
                                {
                                    variable.Value = value;
                                    StartCommonEventsWithTrigger(CommonEventTrigger.PlayerVariableChange, "", cmd.VariableId.ToString());
                                }
                            }
                            else if (cmd.VariableType == VariableTypes.ServerVariable)
                            {
                                var variable = ServerVariableBase.Get(cmd.VariableId);
                                if (changed)
                                {
                                    variable.Value = value;
                                    StartCommonEventsWithTriggerForAll(CommonEventTrigger.ServerVariableChange, "", cmd.VariableId.ToString());
                                    DbInterface.UpdatedServerVariables.AddOrUpdate(variable.Id, variable, (key, oldValue) => variable);
                                }
                            }
                            else if (cmd.VariableType == VariableTypes.GuildVariable)
                            {
                                if (Guild != null)
                                {
                                    var variable = Guild.GetVariable(cmd.VariableId);
                                    if (variable.Value?.Value != value.Value)
                                    {
                                        variable.Value = value;
                                        Guild.StartCommonEventsWithTriggerForAll(Enums.CommonEventTrigger.GuildVariableChange, "", cmd.VariableId.ToString());
                                        Guild.UpdatedVariables.AddOrUpdate(cmd.VariableId, GuildVariableBase.Get(cmd.VariableId), (key, oldValue) => GuildVariableBase.Get(cmd.VariableId));
                                    }
                                }
                            }

                            var tmpStack = success
                                ? new CommandInstance(stackInfo.Page, stackInfo.BranchIds[0])
                                : new CommandInstance(stackInfo.Page, stackInfo.BranchIds[1]);

                            evt.Value.CallStack.Push(tmpStack);
                        }

                        return;
                    }
                }
            }
        }

        static bool IsEventOneBlockAway(Event evt)
        {
            //todo this
            return true;
        }

        public Event FindGlobalEventInstance(EventPageInstance en)
        {
            if (GlobalPageInstanceLookup.TryGetValue(en, out Event val))
            {
                return val;
            }

            return null;
        }

        public void SendEvents()
        {
            foreach (var evt in EventLookup)
            {
                if (evt.Value.PageInstance != null)
                {
                    evt.Value.PageInstance.SendToPlayer();
                }
            }
        }

        public bool StartCommonEvent(
            EventBase baseEvent,
            CommonEventTrigger trigger = CommonEventTrigger.None,
            string command = "",
            string param = ""
        )
        {
            if (baseEvent == null)
            {
                return false;
            }

            if (!baseEvent.CommonEvent && baseEvent.MapId != Guid.Empty)
            {
                return false;
            }

            if (EventBaseIdLookup.ContainsKey(baseEvent.Id))
            {
                return false;
            }

            lock (mEventLock)
            {
                mCommonEventLaunches++;
                var commonEventLaunch = mCommonEventLaunches;

                //Use Fake Ids for Common Events Since they are not tied to maps and such
                var evtId = Guid.NewGuid();
                var mapId = Guid.Empty;

                Event newEvent = null;

                //Try to Spawn a PageInstance.. if we can
                for (var i = baseEvent.Pages.Count - 1; i >= 0; i--)
                {
                    if ((trigger == CommonEventTrigger.None || baseEvent.Pages[i].CommonTrigger == trigger) && Conditions.CanSpawnPage(baseEvent.Pages[i], this, null))
                    {
                        if (trigger == CommonEventTrigger.SlashCommand && command.ToLower() != baseEvent.Pages[i].TriggerCommand.ToLower())
                        {
                            continue;
                        }

                        if (trigger == CommonEventTrigger.PlayerVariableChange && param != baseEvent.Pages[i].TriggerId.ToString())
                        {
                            continue;
                        }

                        if (trigger == CommonEventTrigger.ServerVariableChange && param != baseEvent.Pages[i].TriggerId.ToString())
                        {
                            continue;
                        }

                        newEvent = new Event(evtId, null, this, baseEvent)
                        {
                            MapId = mapId,
                            SpawnX = -1,
                            SpawnY = -1
                        };
                        newEvent.PageInstance = new EventPageInstance(
                            baseEvent, baseEvent.Pages[i], mapId, MapInstanceId, newEvent, this
                        );

                        newEvent.PageIndex = i;

                        if (trigger == CommonEventTrigger.SlashCommand)
                        {
                            //Split params up
                            var prams = param.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            for (var x = 0; x < prams.Length; x++)
                            {
                                newEvent.SetParam("slashParam" + x, prams[x]);
                            }
                        }

                        switch (trigger)
                        {
                            case CommonEventTrigger.None:
                                break;
                            case CommonEventTrigger.Login:
                                break;
                            case CommonEventTrigger.LevelUp:
                                break;
                            case CommonEventTrigger.OnRespawn:
                                break;
                            case CommonEventTrigger.SlashCommand:
                                break;
                            case CommonEventTrigger.Autorun:
                                break;
                            case CommonEventTrigger.PVPKill:
                                //Add victim as a parameter
                                newEvent.SetParam("victim", param);

                                break;
                            case CommonEventTrigger.PVPDeath:
                                //Add killer as a parameter
                                newEvent.SetParam("killer", param);

                                break;
                            case CommonEventTrigger.PlayerInteract:
                                //Interactee as a parameter
                                newEvent.SetParam("triggered", param);

                                break;
                            case CommonEventTrigger.GuildMemberJoined:
                            case CommonEventTrigger.GuildMemberKicked:
                            case CommonEventTrigger.GuildMemberLeft:
                                newEvent.SetParam("member", param);
                                newEvent.SetParam("guild", command);

                                break;
                        }

                        var newStack = new CommandInstance(newEvent.PageInstance.MyPage);
                        newEvent.CallStack.Push(newStack);

                        break;
                    }
                }

                if (newEvent != null)
                {
                    EventLookup.AddOrUpdate(evtId, newEvent, (key, oldValue) => newEvent);
                    EventBaseIdLookup.AddOrUpdate(baseEvent.Id, newEvent, (key, oldvalue) => newEvent);
                    return true;
                }
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

            foreach (var evt in EventLookup)
            {
                if (evt.Value.HoldingPlayer)
                {
                    return -5;
                }
            }

            return base.CanMove(moveDir);
        }

        protected override int IsTileWalkable(MapController map, int x, int y, int z)
        {
            if (base.IsTileWalkable(map, x, y, z) == -1)
            {
                foreach (var evt in EventLookup)
                {
                    if (evt.Value.PageInstance != null)
                    {
                        var instance = evt.Value.PageInstance;
                        if (instance.GlobalClone != null)
                        {
                            instance = instance.GlobalClone;
                        }

                        if (instance.Map == map &&
                            instance.X == x &&
                            instance.Y == y &&
                            instance.Z == z &&
                            !instance.Passable)
                        {
                            return (int) EntityTypes.Event;
                        }
                    }
                }
            }

            return -1;
        }

        public override void Move(int moveDir, Player forPlayer, bool dontUpdate = false, bool correction = false)
        {
            lock (EntityLock)
            {
                var oldMap = MapId;
                base.Move(moveDir, forPlayer, dontUpdate, correction);

                // Check for a warp, if so warp the player.
                var attribute = MapController.Get(MapId).Attributes[X, Y];
                if (attribute != null && attribute.Type == MapAttributes.Warp)
                {
                    var warpAtt = (MapWarpAttribute)attribute;
                    var dir = (byte)Dir;
                    if (warpAtt.Direction != WarpDirection.Retain)
                    {
                        dir = (byte)(warpAtt.Direction - 1);
                    }

                    MapInstanceType? instanceType = null;
                    if (warpAtt.ChangeInstance)
                    {
                        instanceType = warpAtt.InstanceType;
                    }

                    if (warpAtt.WarpSound != null)
                    {
                        PacketSender.SendPlaySound(this, warpAtt.WarpSound);
                    }

                    Warp(warpAtt.MapId, warpAtt.X, warpAtt.Y, dir, false, 0, false, false, instanceType);
                }

                foreach (var evt in EventLookup)
                {
                    if (evt.Value.MapId == MapId)
                    {
                        if (evt.Value.PageInstance != null && evt.Value.PageInstance.MapId == MapId)
                        {
                            var x = evt.Value.PageInstance.GlobalClone?.X ?? evt.Value.PageInstance.X;
                            var y = evt.Value.PageInstance.GlobalClone?.Y ?? evt.Value.PageInstance.Y;
                            var z = evt.Value.PageInstance.GlobalClone?.Z ?? evt.Value.PageInstance.Z;
                            if (x == X && y == Y && z == Z)
                            {
                                HandleEventCollision(evt.Value, -1);
                            }
                        }
                    }
                }

                // If we've changed maps, start relevant events!
                if (oldMap != MapId)
                {
                    StartCommonEventsWithTrigger(CommonEventTrigger.MapChanged);
                }
            }
        }

        public void TryBumpEvent(Guid mapId, Guid eventId)
        {
            foreach (var evt in EventLookup)
            {
                if (evt.Value.MapId == mapId)
                {
                    if (evt.Value.PageInstance != null && evt.Value.PageInstance.MapId == mapId && evt.Value.BaseEvent.Id == eventId)
                    {
                        var x = evt.Value.PageInstance.GlobalClone?.X ?? evt.Value.PageInstance.X;
                        var y = evt.Value.PageInstance.GlobalClone?.Y ?? evt.Value.PageInstance.Y;
                        var z = evt.Value.PageInstance.GlobalClone?.Z ?? evt.Value.PageInstance.Z;
                        if (IsOneBlockAway(mapId, x, y, z))
                        {
                            if (evt.Value.PageInstance.Trigger != EventTrigger.PlayerBump)
                            {
                                return;
                            }

                            if (evt.Value.CallStack.Count != 0)
                            {
                                return;
                            }

                            var newStack = new CommandInstance(evt.Value.PageInstance.MyPage);
                            evt.Value.CallStack.Push(newStack);
                        }
                    }
                }
            }
        }

        public void HandleEventCollision(Event evt, int pageNum)
        {
            var eventInstance = evt;
            if (evt.Player == null) //Global
            {
                eventInstance = null;
                foreach (var e in EventLookup)
                {
                    if (e.Value.BaseEvent.Id == evt.BaseEvent.Id)
                    {
                        if (e.Value.PageInstance.MyPage == e.Value.BaseEvent.Pages[pageNum])
                        {
                            eventInstance = e.Value;

                            break;
                        }
                    }
                }
            }

            if (eventInstance != null)
            {
                if (eventInstance.PageInstance.Trigger != EventTrigger.PlayerCollide)
                {
                    return;
                }

                if (eventInstance.CallStack.Count != 0)
                {
                    return;
                }

                var newStack = new CommandInstance(eventInstance.PageInstance.MyPage);
                eventInstance.CallStack.Push(newStack);
            }
        }

        /// <summary>
        /// Update the cooldown for a specific item.
        /// </summary>
        /// <param name="item">The <see cref="ItemBase"/> to update the cooldown for.</param>
        public void UpdateCooldown(ItemBase item)
        {
            if (item == null)
            {
                return;
            }

            // Are we dealing with a cooldown group?
            if (item.CooldownGroup.Trim().Length > 0)
            {
                // Yes, so handle it!
                UpdateCooldownGroup(GameObjectType.Item, item.CooldownGroup, item.Cooldown, item.IgnoreCooldownReduction);
            }
            else
            {
                // No, handle singular cooldown as normal.

                var cooldownReduction = 1 - (item.IgnoreCooldownReduction ? 0 : GetEquipmentBonusEffect(EffectType.CooldownReduction) / 100f);
                AssignItemCooldown(item.Id, Timing.Global.MillisecondsUtc + (long)(item.Cooldown * cooldownReduction));
                PacketSender.SendItemCooldown(this, item.Id);
            }
        }

        /// <summary>
        /// Update the cooldown for a specific spell.
        /// </summary>
        /// <param name="item">The <see cref="SpellBase"/> to update the cooldown for.</param>
        public void UpdateCooldown(SpellBase spell)
        {
            if (spell == null)
            {
                return;
            }

            // Are we dealing with a cooldown group?
            if (spell.CooldownGroup.Trim().Length > 0)
            {
                // Yes, so handle it!
                UpdateCooldownGroup(GameObjectType.Spell, spell.CooldownGroup, spell.CooldownDuration, spell.IgnoreCooldownReduction);
            }
            else
            {
                // No, handle singular cooldown as normal.
                var cooldownReduction = 1 - (spell.IgnoreCooldownReduction ? 0 : GetEquipmentBonusEffect(EffectType.CooldownReduction) / 100f);
                AssignSpellCooldown(spell.Id, Timing.Global.MillisecondsUtc + (long)(spell.CooldownDuration * cooldownReduction));
                PacketSender.SendSpellCooldown(this, spell.Id);
            }
        }

        /// <summary>
        /// Forces an update of the global cooldown.
        /// Does nothing when disabled by configuration.
        /// </summary>
        public void UpdateGlobalCooldown()
        {
            // Are we allowed to execute this code?
            if (!Options.Combat.EnableGlobalCooldowns)
            {
                return;
            }

            // Calculate our global cooldown.
            var cooldownReduction = 1 - GetEquipmentBonusEffect(EffectType.CooldownReduction) / 100f;
            var cooldown = Timing.Global.MillisecondsUtc + (long)(Options.Combat.GlobalCooldownDuration * cooldownReduction);

            // Go through each item and spell to assign this cooldown.
            // Do not allow this to overwrite things that are still on a cooldown above our new cooldown though, don't want us to lower cooldowns!
            // We do however want to overwrite lower cooldowns than our new one, it is a GLOBAL cooldown after all!
            foreach(var item in ItemBase.Lookup)
            {
                // Skip this item if it is unaffected by global cooldowns.
                if (((ItemBase)item.Value).IgnoreGlobalCooldown)
                {
                    continue;
                }

                if (!ItemCooldowns.ContainsKey(item.Key) || ItemCooldowns[item.Key] < cooldown)
                {
                    AssignItemCooldown(item.Key, cooldown);
                }
            }
            foreach (var spell in SpellBase.Lookup)
            {
                // Skip this item if it is unaffected by global cooldowns.
                if (((SpellBase)spell.Value).IgnoreGlobalCooldown)
                {
                    continue;
                }

                if (!SpellCooldowns.ContainsKey(spell.Key) || SpellCooldowns[spell.Key] < cooldown)
                {
                    AssignSpellCooldown(spell.Key, cooldown);
                }
            }

            // Send these cooldowns to the user!
            PacketSender.SendItemCooldowns(this);
            PacketSender.SendSpellCooldowns(this);
        }

        /// <summary>
        /// Update all cooldowns within the specified cooldown group on a type of object, or all when configured as such.
        /// </summary>
        /// <param name="type">The <see cref="GameObjectType"/> to set trigger the cooldown group for. Currently only accepts Items and Spells</param>
        /// <param name="group">The cooldown group to trigger.</param>
        /// <param name="cooldown">The base cooldown of the object that triggered this cooldown group.</param>
        /// <param name="ignoreCdr">Whether or not this item/spell is set to ignore cdr, in which case the group will ignore it too.</param>
        private void UpdateCooldownGroup(GameObjectType type, string group, int cooldown, bool ignoreCdr)
        {
            // We're only dealing with these two types for now.
            if (type != GameObjectType.Item && type != GameObjectType.Spell)
            {
                return;
            }

            var cooldownReduction = 1 - (ignoreCdr ? 0 : GetEquipmentBonusEffect(EffectType.CooldownReduction) / 100f);

            // Retrieve a list of all items and/or spells depending on our settings to set the cooldown for.
            var matchingItems = Array.Empty<ItemBase>();
            var matchingSpells = Array.Empty<SpellBase>();
            var itemsUpdated = false;
            var spellsUpdated = false;

            if (type == GameObjectType.Item || Options.Combat.LinkSpellAndItemCooldowns)
            {
                matchingItems = ItemBase.GetCooldownGroup(group);
            }
            if (type == GameObjectType.Spell || Options.Combat.LinkSpellAndItemCooldowns)
            {
                matchingSpells = SpellBase.GetCooldownGroup(group);
            }

            // Set our matched cooldown, should we need to use it.
            var matchedCooldowntime = cooldown;
            if (Options.Combat.MatchGroupCooldownHighest)
            {
                // Get our highest cooldown value from all available options.
                matchedCooldowntime = Math.Max(
                    matchingItems.Length > 0 ? matchingItems.Max(i => i.Cooldown) : 0,
                    matchingSpells.Length > 0 ? matchingSpells.Max(i => i.CooldownDuration) : 0);
            }

            // Set the cooldown for all items matching this cooldown group.
            var baseTime = Timing.Global.MillisecondsUtc;
            if (type == GameObjectType.Item || Options.Combat.LinkSpellAndItemCooldowns)
            {
                foreach (var item in matchingItems)
                {
                    // Do we have to match our cooldown times, or do we use each individual item cooldown?
                    var tempCooldown = Options.Combat.MatchGroupCooldowns ? matchedCooldowntime : item.Cooldown;

                    // Asign it! Assuming our cooldown isn't already going..
                    if (!ItemCooldowns.ContainsKey(item.Id) || ItemCooldowns[item.Id] < Timing.Global.MillisecondsUtc)
                    {
                        AssignItemCooldown(item.Id, baseTime + (long)(tempCooldown * cooldownReduction));
                        itemsUpdated = true;
                    }
                }
            }

            // Set the cooldown for all Spells matching this cooldown group.
            if (type == GameObjectType.Spell || Options.Combat.LinkSpellAndItemCooldowns)
            {
                foreach (var spell in matchingSpells)
                {
                    // Do we have to match our cooldown times, or do we use each individual item cooldown?
                    var tempCooldown = Options.Combat.MatchGroupCooldowns ? matchedCooldowntime : spell.CooldownDuration;

                    // Asign it! Assuming our cooldown isn't already going...
                    if (!SpellCooldowns.ContainsKey(spell.Id) || SpellCooldowns[spell.Id] < Timing.Global.MillisecondsUtc)
                    {
                        AssignSpellCooldown(spell.Id, baseTime + (long)(tempCooldown * cooldownReduction));
                        spellsUpdated = true;
                    }
                }
            }

            // Send all of our updated cooldowns.
            if (itemsUpdated)
            {
                PacketSender.SendItemCooldowns(this);
            }
            if (spellsUpdated)
            {
                PacketSender.SendSpellCooldowns(this);
            }
        }

        /// <summary>
        /// Assign a cooldown time to a specified item.
        /// WARNING: Makes no checks at all to see whether this SHOULD happen!
        /// </summary>
        /// <param name="itemId">The <see cref="ItemBase"/> id to assign the cooldown for.</param>
        /// <param name="cooldownTime">The cooldown time to assign.</param>
        private void AssignItemCooldown(Guid itemId, long cooldownTime)
        {
            // Do we already have a cooldown entry for this item?
            if (ItemCooldowns.ContainsKey(itemId))
            {
                ItemCooldowns[itemId] = cooldownTime;
            }
            else
            {
                ItemCooldowns.TryAdd(itemId, cooldownTime);
            }
        }

        /// <summary>
        /// Assign a cooldown time to a specified spell.
        /// WARNING: Makes no checks at all to see whether this SHOULD happen!
        /// </summary>
        /// <param name="itemId">The <see cref="SpellBase"/> id to assign the cooldown for.</param>
        /// <param name="cooldownTime">The cooldown time to assign.</param>
        private void AssignSpellCooldown(Guid spellId, long cooldownTime)
        {
            // Do we already have a cooldown entry for this item?
            if (SpellCooldowns.ContainsKey(spellId))
            {
                SpellCooldowns[spellId] = cooldownTime;
            }
            else
            {
                SpellCooldowns.TryAdd(spellId, cooldownTime);
            }
        }

        public bool TryChangeName(string newName)
        {
            // Is the name available?
            if (!FieldChecking.IsValidUsername(newName, Strings.Regex.username))
            {
                return false;
            }

            if (PlayerExists(newName))
            {
                return false;
            }

            // Change their name and force save it!
            var oldName = Name;
            Name = newName;
            User?.Save();

            // Log the activity.
            UserActivityHistory.LogActivity(UserId, Id, Client?.GetIp(), UserActivityHistory.PeerType.Client, UserActivityHistory.UserAction.NameChange, $"Changing Character name from {oldName} to {newName}");

            // Send our data around!
            PacketSender.SendEntityDataToProximity(this);

            return true;

        }

        //TODO: Clean all of this stuff up

        #region Temporary Values

        [NotMapped, JsonIgnore] public bool InGame;

        [NotMapped, JsonIgnore] public Guid LastMapEntered = Guid.Empty;

        [JsonIgnore, NotMapped] public Client Client { get; set; }

        [JsonIgnore, NotMapped]
        public UserRights Power => Client?.Power ?? UserRights.None;

        [JsonIgnore, NotMapped] private bool mSentMap;

        [JsonIgnore, NotMapped] private int mCommonEventLaunches = 0;

        [JsonIgnore, NotMapped] private object mEventLock = new object();

        [JsonIgnore, NotMapped]
        public ConcurrentDictionary<Guid, Event> EventLookup = new ConcurrentDictionary<Guid, Event>();

        [JsonIgnore, NotMapped]
        public ConcurrentDictionary<MapTileLoc, Event> EventTileLookup = new ConcurrentDictionary<MapTileLoc, Event>();

        [JsonIgnore, NotMapped]
        public ConcurrentDictionary<Guid, Event> EventBaseIdLookup = new ConcurrentDictionary<Guid, Event>();

        [JsonIgnore, NotMapped]
        public ConcurrentDictionary<EventPageInstance, Event> GlobalPageInstanceLookup = new ConcurrentDictionary<EventPageInstance, Event>();

        #endregion

        #region Trading

        [JsonProperty(nameof(Trading))]
        private Guid JsonTradingId => Trading.Counterparty?.Id ?? Guid.Empty;

        [JsonIgnore, NotMapped] public Trading Trading;

        #endregion

        #region Crafting

        [NotMapped, JsonIgnore] public Guid CraftingTableId = Guid.Empty;

        [NotMapped, JsonIgnore] public Guid CraftId = Guid.Empty;

        [NotMapped, JsonIgnore] public long CraftTimer = 0;

        #endregion

        #region Parties

        private List<Guid> JsonPartyIds => Party.Select(partyMember => partyMember?.Id ?? Guid.Empty).ToList();

        private Guid JsonPartyRequesterId => PartyRequester?.Id ?? Guid.Empty;

        private Dictionary<Guid, long> JsonPartyRequests => PartyRequests.ToDictionary(
            pair => pair.Key?.Id ?? Guid.Empty, pair => pair.Value
        );

        [JsonIgnore, NotMapped] public List<Player> Party = new List<Player>();

        [JsonIgnore, NotMapped] public Player PartyRequester;

        [JsonIgnore, NotMapped] public Dictionary<Player, long> PartyRequests = new Dictionary<Player, long>();

        #endregion

        #region Friends

        private Guid JsonFriendRequesterId => FriendRequester?.Id ?? Guid.Empty;

        private Dictionary<Guid, long> JsonFriendRequests => FriendRequests.ToDictionary(
            pair => pair.Key?.Id ?? Guid.Empty, pair => pair.Value
        );

        [JsonIgnore, NotMapped] public Player FriendRequester;

        [JsonIgnore, NotMapped] public Dictionary<Player, long> FriendRequests = new Dictionary<Player, long>();

        #endregion

        #region Bag/Shops/etc

        [JsonProperty(nameof(InBag))]
        private bool JsonInBag => InBag != null;

        [JsonProperty(nameof(InShop))]
        private bool JsonInShop => InShop != null;

        [JsonIgnore, NotMapped] public Bag InBag;

        [JsonIgnore, NotMapped] public ShopBase InShop;

        [NotMapped] public bool InBank => BankInterface != null;

        [NotMapped][JsonIgnore] public BankInterface BankInterface;

        #endregion

        #region Item Cooldowns

        [JsonIgnore, Column("ItemCooldowns")]
        public string ItemCooldownsJson
        {
            get => JsonConvert.SerializeObject(ItemCooldowns);
            set => ItemCooldowns = JsonConvert.DeserializeObject<ConcurrentDictionary<Guid, long>>(value ?? "{}");
        }

        [JsonIgnore] public ConcurrentDictionary<Guid, long> ItemCooldowns = new ConcurrentDictionary<Guid, long>();

        #endregion

    }

}
