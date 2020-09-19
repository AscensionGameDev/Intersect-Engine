﻿using System;
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
using Intersect.Server.Database.PlayerData.Players;
using Intersect.Server.Database.PlayerData.Security;
using Intersect.Server.Entities.Events;
using Intersect.Server.General;
using Intersect.Server.Localization;
using Intersect.Server.Maps;
using Intersect.Server.Networking;
using Intersect.Utilities;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace Intersect.Server.Entities
{

    public partial class Player : Entity
    {

        //Online Players List
        [NotNull] private static readonly Dictionary<Guid, Player> OnlinePlayers = new Dictionary<Guid, Player>();

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

        //Bank
        [NotNull, JsonIgnore]
        public virtual List<BankSlot> Bank { get; set; } = new List<BankSlot>();

        //Friends
        [NotNull, JsonIgnore]
        public virtual List<Friend> Friends { get; set; } = new List<Friend>();

        //HotBar
        [NotNull, JsonIgnore]
        public virtual List<HotbarSlot> Hotbar { get; set; } = new List<HotbarSlot>();

        //Quests
        [NotNull, JsonIgnore]
        public virtual List<Quest> Quests { get; set; } = new List<Quest>();

        //Variables
        [NotNull, JsonIgnore]
        public virtual List<Variable> Variables { get; set; } = new List<Variable>();

        [JsonIgnore, NotMapped]
        public bool IsValidPlayer => !IsDisposed && Client?.Entity == this;

        [NotMapped]
        public long ExperienceToNextLevel => GetExperienceToNextLevel(Level);

        // Guilds
        public Guild Guild { get; set; }

        public GuildRanks GuildRank { get; set; }

        [NotMapped]
        public Tuple<Player, Guild> GuildInvite { get; set; }

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

            //Upon Sign In Remove Any Items/Spells that have been deleted
            foreach (var itm in Items)
            {
                if (itm.ItemId != Guid.Empty && ItemBase.Get(itm.ItemId) == null)
                {
                    itm.Set(new Item());
                }
            }

            foreach (var itm in Bank)
            {
                if (itm.ItemId != Guid.Empty && ItemBase.Get(itm.ItemId) == null)
                {
                    itm.Set(new Item());
                }
            }

            foreach (var spl in Spells)
            {
                if (spl.SpellId != Guid.Empty && SpellBase.Get(spl.SpellId) == null)
                {
                    spl.Set(new Spell());
                }
            }

            OnlinePlayers[Id] = this;
        }

        public void SendPacket(CerasPacket packet)
        {
            Client?.SendPacket(packet);
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

        public void TryLogout()
        {
            if (CombatTimer < Globals.Timing.TimeMs)
            {
                Logout();
            }
        }

        private void Logout()
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
                if (t == null || t.GetType() != typeof(Npc))
                {
                    continue;
                }

                if (t.Despawnable)
                {
                    t.Die(0);
                }
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

            //Clear cooldowns that have expired
            var keys = SpellCooldowns.Keys.ToArray();
            foreach (var key in keys)
            {
                if (SpellCooldowns[key] < Globals.Timing.RealTimeMs)
                {
                    SpellCooldowns.Remove(key);
                }
            }

            keys = ItemCooldowns.Keys.ToArray();
            foreach (var key in keys)
            {
                if (ItemCooldowns[key] < Globals.Timing.RealTimeMs)
                {
                    ItemCooldowns.Remove(key);
                }
            }

            PacketSender.SendEntityLeave(this);

            if (!string.IsNullOrWhiteSpace(Strings.Player.left.ToString()))
            {
                PacketSender.SendGlobalMsg(Strings.Player.left.ToString(Name, Options.Instance.GameName));
            }

            Dispose();
        }

        ~Player()
        {
            if (OnlinePlayers.ContainsKey(Id))
            {
                OnlinePlayers.Remove(Id);
            }
        }

        //Update
        public override void Update(long timeMs)
        {
            if (!InGame || MapId == Guid.Empty)
            {
                return;
            }

            if (Client == null) //Client logged out
            {
                if (CombatTimer < Globals.Timing.TimeMs)
                {
                    Logout();

                    return;
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

            //Check for autorun common events and run them
            foreach (var obj in EventBase.Lookup)
            {
                var evt = obj.Value as EventBase;
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

                    if (map == null)
                    {
                        continue;
                    }

                    if (Monitor.TryEnter(map.GetMapLock(), new TimeSpan(0, 0, 0, 0, 1)))
                    {
                        try
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
                                            var tmpEvent = new Event(Guid.NewGuid(), map.Id, this, mapEvent)
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
                        finally
                        {
                            Monitor.Exit(map.GetMapLock());
                        }
                    }
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
                    if (evt.Value.MapId == Guid.Empty)
                    {
                        evt.Value.Update(timeMs);
                        if (evt.Value.CallStack.Count > 0)
                        {
                            eventFound = true;
                        }
                    }

                    if (evt.Value.MapId != MapId)
                    {
                        foreach (var t in MapInstance.Get(MapId).SurroundingMaps)
                        {
                            if (t == evt.Value.MapId)
                            {
                                eventFound = true;
                            }
                        }
                    }
                    else
                    {
                        eventFound = true;
                    }

                    if (eventFound)
                    {
                        continue;
                    }

                    PacketSender.SendEntityLeaveTo(this, evt.Value);
                    EventLookup.TryRemove(evt.Value.Id, out var z);
                }
            }
        }

        public void RemoveEvent(Guid id)
        {
            Event outInstance;
            EventLookup.TryRemove(id, out outInstance);
            PacketSender.SendEntityLeaveTo(this, this);
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
            pkt.Guild = Guild?.Name;
            

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

            if (CombatTimer > Globals.Timing.TimeMs)
            {
                pkt.CombatTimeRemaining = CombatTimer - Globals.Timing.TimeMs;
            }

            if (forPlayer != null && GetType() == typeof(Player))
            {
                ((PlayerEntityPacket) packet).Equipment =
                    PacketSender.GenerateEquipmentPacket(forPlayer, (Player) this);
            }

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

            CombatTimer = 0;

            var cls = ClassBase.Get(ClassId);
            if (cls != null)
            {
                Warp(cls.SpawnMapId, (byte) cls.SpawnX, (byte) cls.SpawnY, (byte) cls.SpawnDir);
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

        public override void Die(int dropitems = 0, Entity killer = null)
        {
            //Flag death to the client
            PacketSender.SendPlayerDeath(this);

            //Event trigger
            foreach (var evt in EventLookup)
            {
                evt.Value.PlayerHasDied = true;
            }

            base.Die(dropitems, killer);
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
                if (Equipment[i] >= 0 && Equipment[i] < Options.MaxInvItems)
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

            PacketSender.SendChatMsg(this, Strings.Player.levelup.ToString(Level), CustomColors.Combat.LevelUp, Name);
            PacketSender.SendActionMsg(this, Strings.Combat.levelup, CustomColors.Combat.LevelUp);
            foreach (var message in messages)
            {
                PacketSender.SendChatMsg(this, message, CustomColors.Alerts.Info, Name);
            }

            if (StatPoints > 0)
            {
                PacketSender.SendChatMsg(
                    this, Strings.Player.statpoints.ToString(StatPoints), CustomColors.Combat.StatPoints, Name
                );
            }

            RecalculateStatsAndPoints();
            PacketSender.SendExperience(this);
            PacketSender.SendPointsTo(this);
            PacketSender.SendEntityDataToProximity(this);

            //Search for level up activated events and run them
            foreach (var value in EventBase.Lookup.Values)
            {
                if (value is EventBase eventDescriptor)
                {
                    StartCommonEvent(eventDescriptor, CommonEventTrigger.LevelUp);
                }
            }
        }

        public void GiveExperience(long amount)
        {
            Exp += (int) (amount * GetExpMultiplier() / 100);
            if (Exp < 0)
            {
                Exp = 0;
            }

            if (!CheckLevelUp())
            {
                PacketSender.SendExperience(this);
            }
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
                        var partyMembersInXpRange = Party.Where(partyMember => partyMember.InRangeOf(this, Options.Party.SharedXpRange));
                        var partyExperience = descriptor.Experience / partyMembersInXpRange.Count();
                        foreach (var partyMember in partyMembersInXpRange) {
                            partyMember.GiveExperience(partyExperience);
                            partyMember.UpdateQuestKillTasks(entity);
                        }

                        if (partyEvent != null)
                        {
                            foreach (var partyMember in Party) {
                                if (partyMember.InRangeOf(this, Options.Party.NpcDeathCommonEventStartRange) && !(partyMember == this && playerEvent != null)) {
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
            var npc = (Npc) en;
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
                                    PacketSender.SendChatMsg(
                                        this,
                                        Strings.Quests.npctask.ToString(
                                            quest.Name, questProgress.TaskProgress, questTask.Quantity,
                                            NpcBase.GetName(questTask.TargetId)
                                        )
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
                if (resource.IsDead)
                {
                    return;
                }

                // Check that a resource is actually required.
                var descriptor = resource.Base;

                //Check Dynamic Requirements
                if (!Conditions.MeetsConditionLists(descriptor.HarvestingRequirements, this, null))
                {
                    PacketSender.SendChatMsg(this, Strings.Combat.resourcereqs);

                    return;
                }

                if (descriptor.Tool > -1 && descriptor.Tool < Options.ToolTypes.Count)
                {
                    if (parentItem == null || descriptor.Tool != parentItem.Tool)
                    {
                        PacketSender.SendChatMsg(
                            this, Strings.Combat.toolrequired.ToString(Options.ToolTypes[descriptor.Tool])
                        );

                        return;
                    }
                }
            }

            base.TryAttack(target, projectile, parentSpell, parentItem, projectileDir);
        }

        public override void TryAttack(Entity target)
        {
            if (CastTime >= Globals.Timing.TimeMs)
            {
                PacketSender.SendChatMsg(this, Strings.Combat.channelingnoattack);

                return;
            }

            if (!IsOneBlockAway(target))
            {
                return;
            }

            if (!IsFacingTarget(target))
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
                if (resource.IsDead)
                {
                    return;
                }

                // Check that a resource is actually required.
                var descriptor = resource.Base;

                //Check Dynamic Requirements
                if (!Conditions.MeetsConditionLists(descriptor.HarvestingRequirements, this, null))
                {
                    PacketSender.SendChatMsg(this, Strings.Combat.resourcereqs);

                    return;
                }

                if (descriptor.Tool > -1 && descriptor.Tool < Options.ToolTypes.Count)
                {
                    if (weapon == null || descriptor.Tool != weapon.Tool)
                    {
                        PacketSender.SendChatMsg(
                            this, Strings.Combat.toolrequired.ToString(Options.ToolTypes[descriptor.Tool])
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
            if (!base.CanAttack(entity, spell))
            {
                return false;
            }

            if (entity is EventPage)
            {
                return false;
            }

            //Check if the attacker is stunned or blinded.
            if (Statuses.Values.ToArray()
                .Any(status => status?.Type == StatusTypes.Stun || status?.Type == StatusTypes.Sleep))
            {
                return false;
            }

            var friendly = spell?.Combat != null && spell.Combat.Friendly;
            switch (entity)
            {
                case Player player when friendly != player.InParty(this) || (!Options.Guild.AllowGuildMemberPvP && friendly != (player.Guild == this.Guild)):
                case Resource _ when spell != null:
                    return false;
                case Npc npc:
                    return !friendly && npc.CanPlayerAttack(this) || friendly && npc.IsAllyOf(this);
                default:
                    return true;
            }
        }

        public override void NotifySwarm(Entity attacker)
        {
            MapInstance.Get(MapId)
                ?.GetEntities(true)
                .ForEach(
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
                    attackTime = (int) (attackTime * (100f / weapon.AttackSpeedValue));
                }
            }

            return
                attackTime -
                60; //subtracting 60 to account for a moderate ping to the server so some attacks dont get cancelled.
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
                            s += Items[Equipment[i]].StatBuffs[(int) statType] +
                                 item.StatsGiven[(int) statType] +
                                 (int) ((Stat[(int) statType].BaseStat + StatPointAllocations[(int) statType]) *
                                        item.PercentageStatsGiven[(int) statType] /
                                        100f);
                        }
                    }
                }
            }

            return s;
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
        public override void Warp(Guid newMapId, byte newX, byte newY, bool adminWarp = false)
        {
            Warp(newMapId, newX, newY, (byte) Directions.Up, adminWarp, 0, false);
        }

        public override void Warp(
            Guid newMapId,
            byte newX,
            byte newY,
            byte newDir,
            bool adminWarp = false,
            byte zOverride = 0,
            bool mapSave = false
        )
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
            foreach (var evt in EventLookup)
            {
                if (evt.Value.MapId != Guid.Empty && (!newSurroundingMaps.Contains(evt.Value.MapId) || mapSave))
                {
                    EventLookup.TryRemove(evt.Value.Id, out var z);
                }
            }

            if (newMapId != MapId || mSentMap == false)
            {
                var oldMap = MapInstance.Get(MapId);
                if (oldMap != null)
                {
                    oldMap.RemoveEntity(this);
                }

                PacketSender.SendEntityLeave(this);
                MapId = newMapId;
                map.PlayerEnteredMap(this);
                PacketSender.SendEntityDataToProximity(this);
                PacketSender.SendEntityPositionToAll(this);

                //If map grid changed then send the new map grid
                if (!adminWarp && (oldMap == null || !oldMap.SurroundingMaps.Contains(newMapId)))
                {
                    PacketSender.SendMapGrid(this.Client, map.MapGrid, true);
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
            var mapId = Guid.Empty;
            byte x = 0, y = 0, dir = 0;
            var cls = ClassBase.Get(ClassId);
            if (cls != null)
            {
                if (MapInstance.Lookup.Keys.Contains(cls.SpawnMapId))
                {
                    mapId = cls.SpawnMapId;
                }

                x = (byte) cls.SpawnX;
                y = (byte) cls.SpawnY;
                dir = (byte) cls.SpawnDir;
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
                    if (FindInventoryItemSlot(item.ItemId) != null)
                    {
                        return true;
                    }

                    // Does the user have a free space?
                    if (FindOpenInventorySlots().Count >= 1)
                    {
                        return true;
                    }
                }
                else
                {
                    // Not a stacking item, so can we contain the amount we want to give them?
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
        [ContractAnnotation(" => true, slot: notnull; createSlotIfNull:false => false, slot: null")]
        public bool TryGetSlot(int slotIndex, [CanBeNull] out InventorySlot slot, bool createSlotIfNull = false)
        {
            // ReSharper disable once AssignNullToNotNullAttribute Justification: slot is never null when this returns true.
            slot = Items[slotIndex];

            // ReSharper disable once InvertIf
            if (default == slot && createSlotIfNull)
            {
                var createdSlot = new InventorySlot(slotIndex);
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
        [ContractAnnotation("=> true, item: notnull; => false, item: null")]
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
        /// <returns>Whether the player received the item or not.</returns>
        public bool TryGiveItem(Item item) => TryGiveItem(item, ItemHandling.Normal, false, true);

        /// <summary>
        /// Attempts to give the player an item. Returns whether or not it succeeds.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> to give to the player.</param>
        /// <param name="handler">The way to handle handing out this item.</param>
        /// <returns>Whether the player received the item or not.</returns>
        public bool TryGiveItem(Item item, ItemHandling handler) => TryGiveItem(item, handler, false, true);

        /// <summary>
        /// Attempts to give the player an item. Returns whether or not it succeeds.
        /// </summary>
        /// <param name="itemId">The Id for the item to be handed out to the player.</param>
        /// <param name="quantity">The quantity of items to be handed out to the player.</param>
        /// <returns>Whether the player received the item or not.</returns>
        public bool TryGiveItem(Guid itemId, int quantity) => TryGiveItem(new Item(itemId, quantity), ItemHandling.Normal, false, true);

        /// <summary>
        /// Attempts to give the player an item. Returns whether or not it succeeds.
        /// </summary>
        /// <param name="itemId">The Id for the item to be handed out to the player.</param>
        /// <param name="quantity">The quantity of items to be handed out to the player.</param>
        /// <param name="handler">The way to handle handing out this item.</param>
        /// <returns>Whether the player received the item or not.</returns>
        public bool TryGiveItem(Guid itemId, int quantity, ItemHandling handler) => TryGiveItem(new Item(itemId, quantity), handler, false, true);

        /// <summary>
        /// Attempts to give the player an item. Returns whether or not it succeeds.
        /// </summary>
        /// <param name="itemId">The Id for the item to be handed out to the player.</param>
        /// <param name="quantity">The quantity of items to be handed out to the player.</param>
        /// <param name="handler">The way to handle handing out this item.</param>
        /// <param name="bankOverflow">Should we allow the items to overflow into the player's bank when their inventory is full.</param>
        /// <param name="sendUpdate">Should we send an inventory update when we are done changing the player's items.</param>
        /// <returns>Whether the player received the item or not.</returns>
        public bool TryGiveItem(Guid itemId, int quantity, ItemHandling handler, bool bankOverflow = false, bool sendUpdate = true) => TryGiveItem(new Item(itemId, quantity), handler, bankOverflow, sendUpdate);

        /// <summary>
        /// Attempts to give the player an item. Returns whether or not it succeeds.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> to give to the player.</param>
        /// <param name="handler">The way to handle handing out this item.</param>
        /// <param name="bankOverflow">Should we allow the items to overflow into the player's bank when their inventory is full.</param>
        /// <param name="sendUpdate">Should we send an inventory update when we are done changing the player's items.</param>
        /// <returns>Whether the player received the item or not.</returns>
        public bool TryGiveItem(Item item, ItemHandling handler = ItemHandling.Normal, bool bankOverflow = false, bool sendUpdate = true)
        {
            // Is this a valid item?
            if (item.Descriptor == null)
            {
                return false;
            }

            // Get this information so we can use it later.
            var openSlots = FindOpenInventorySlots().Count;
            var hasItem = FindInventoryItemSlot(item.ItemId) != null;
            int spawnAmount = 0;

            // How are we going to be handling this?
            switch (handler)
            {
                // Handle this item like normal, there's no special rules attached to this method.
                case ItemHandling.Normal:
                    if (CanGiveItem(item)) // Can receive item under regular rules.
                    {
                        GiveItem(item, sendUpdate);
                        return true;
                    }

                    break;
                case ItemHandling.Overflow:
                    if (CanGiveItem(item)) // Can receive item under regular rules.
                    {
                        GiveItem(item, sendUpdate);
                        return true;
                    }
                    else if (item.Descriptor.Stackable && openSlots == 0) // Is stackable, but no inventory space.
                    {
                        spawnAmount = item.Quantity;
                    }
                    else // Time to give them as much as they can take, and spawn the rest on the map!
                    {
                        spawnAmount = item.Quantity - openSlots;
                        if (openSlots > 0)
                        {
                            item.Quantity = openSlots;
                            GiveItem(item, sendUpdate);
                        }
                    }

                    // Do we have any items to spawn to the map?
                    if (spawnAmount > 0)
                    {
                        Map.SpawnItem(X, Y, item, spawnAmount, Id);
                        return true;
                    }

                    break;
                case ItemHandling.UpTo:
                    if (CanGiveItem(item)) // Can receive item under regular rules.
                    {
                        GiveItem(item, sendUpdate);
                        return true;
                    }
                    else if (!item.Descriptor.Stackable && openSlots > 0) // Is not stackable, has space for some.
                    {
                        item.Quantity = openSlots;
                        GiveItem(item, sendUpdate);
                        return true;
                    }

                    break;
                    // Did you forget to change this method when you added something? ;)
                default:
                    throw new NotImplementedException();
            }

            return bankOverflow && TryDepositItem(item, sendUpdate);
        }


        /// <summary>
        /// Gives the player an item. NOTE: This method MAKES ZERO CHECKS to see if this is possible!
        /// Use TryGiveItem where possible!
        /// </summary>
        /// <param name="item"></param>
        /// <param name="sendUpdate"></param>
        private void GiveItem(Item item, bool sendUpdate)
        {

            // Decide how we're going to handle this item.
            var existingSlot = FindInventoryItemSlot(item.Descriptor.Id);
            var updateSlots = new List<int>();
            if (item.Descriptor.Stackable && existingSlot != null) // Stackable, but already exists in the inventory.
            {
                Items[existingSlot.Slot].Quantity += item.Quantity;
                updateSlots.Add(existingSlot.Slot);
            }
            else if (!item.Descriptor.Stackable && item.Quantity > 1) // Not stackable, but multiple items.
            {
                var openSlots = FindOpenInventorySlots();
                for (var slot = 0; slot < item.Quantity; slot++)
                {
                    openSlots[slot].Set(new Item(item.ItemId, 1));
                    updateSlots.Add(openSlots[slot].Slot);
                }
            }
            else // Hand out without any special treatment. Either a single item or a stackable item we don't have yet.
            {
                var newSlot = FindOpenInventorySlot();
                newSlot.Set(item);
                updateSlots.Add(newSlot.Slot);
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
                PacketSender.SendChatMsg(this, Strings.Items.equipped, CustomColors.Items.Bound);
                return false;
            }

            var itemDescriptor = itemInSlot.Descriptor;
            if (itemDescriptor == null)
            {
                return false;
            }

            if (itemDescriptor.Bound)
            {
                PacketSender.SendChatMsg(this, Strings.Items.bound, CustomColors.Items.Bound);
                return false;
            }

            if (itemInSlot.TryGetBag(out var bag) && !bag.IsEmpty)
            {
                PacketSender.SendChatMsg(this, Strings.Bags.dropnotempty, CustomColors.Alerts.Error);
                return false;
            }

            var map = Map;
            if (map == null)
            {
                Log.Error($"Could not find map {MapId} for player '{Name}'.");
                return false;
            }

            map.SpawnItem(X, Y, itemInSlot, itemDescriptor.IsStackable ? amount : 1, Id);

            itemInSlot.Quantity = Math.Max(0, itemInSlot.Quantity - amount);

            if (itemInSlot.Quantity == 0)
            {
                itemInSlot.Set(Item.None);
                EquipmentProcessItemLoss(slotIndex);
            }

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

        public void UseItem(int slot, Entity target = null)
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
                        PacketSender.SendChatMsg(this, Strings.Items.stunned);

                        return;
                    }

                    if (status.Type == StatusTypes.Sleep)
                    {
                        PacketSender.SendChatMsg(this, Strings.Items.sleep);

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
                    PacketSender.SendChatMsg(this, Strings.Items.dynamicreq);

                    return;
                }

                if (ItemCooldowns.ContainsKey(itemBase.Id) && ItemCooldowns[itemBase.Id] > Globals.Timing.RealTimeMs)
                {
                    //Cooldown warning!
                    PacketSender.SendChatMsg(this, Strings.Items.cooldown);

                    return;
                }

                switch (itemBase.ItemType)
                {
                    case ItemTypes.None:
                    case ItemTypes.Currency:
                        PacketSender.SendChatMsg(this, Strings.Items.cannotuse);

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
                            Die();
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
                            if (!CanSpellCast(itemBase.Spell, target, false))
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
                        PacketSender.SendChatMsg(this, Strings.Items.notimplemented);

                        return;
                }

                if (itemBase.Animation != null)
                {
                    PacketSender.SendAnimationToProximity(
                        itemBase.Animation.Id, 1, base.Id, MapId, 0, 0, (sbyte) Dir
                    ); //Target Type 1 will be global entity
                }

                if (itemBase.Cooldown > 0)
                {
                    var cooldownReduction = 1 - this.GetCooldownReduction() / 100;
                    if (ItemCooldowns.ContainsKey(itemBase.Id))
                    {
                        ItemCooldowns[itemBase.Id] =
                            Globals.Timing.RealTimeMs + (long) (itemBase.Cooldown * cooldownReduction);
                    }
                    else
                    {
                        ItemCooldowns.Add(
                            itemBase.Id, Globals.Timing.RealTimeMs + (long) (itemBase.Cooldown * cooldownReduction)
                        );
                    }

                    PacketSender.SendItemCooldown(this, itemBase.Id);
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
            var itemDescriptor = ItemBase.Get(slot.ItemId);

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

            var itemCount = 0;
            for (var i = 0; i < Options.MaxInvItems; i++)
            {
                var item = Items[i];
                if (item.ItemId == itemId)
                {
                    itemCount = item.Descriptor.Stackable ? itemCount += item.Quantity : itemCount += 1;
                }
            }

            return itemCount;
        }

        /// <summary>
        /// Finds an inventory slot matching the desired item and quantity.
        /// </summary>
        /// <param name="itemId">The item Id to look for.</param>
        /// <param name="quantity">The quantity of the item to look for.</param>
        /// <returns>An <see cref="InventorySlot"/> that contains the item, or null if none are found.</returns>
        public InventorySlot FindInventoryItemSlot(Guid itemId, int quantity = 1) => FindInventoryItemSlots(itemId, quantity).FirstOrDefault();

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

        public decimal GetCooldownReduction()
        {
            var cooldown = 0;

            for (var i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                if (Equipment[i] > -1)
                {
                    if (Items[Equipment[i]].ItemId != Guid.Empty)
                    {
                        var item = ItemBase.Get(Items[Equipment[i]].ItemId);
                        if (item != null)
                        {
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
            var lifesteal = 0;

            for (var i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                if (Equipment[i] > -1)
                {
                    if (Items[Equipment[i]].ItemId != Guid.Empty)
                    {
                        var item = ItemBase.Get(Items[Equipment[i]].ItemId);
                        if (item != null)
                        {
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

        public double GetTenacity()
        {
            double tenacity = 0;

            for (var i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                if (Equipment[i] > -1)
                {
                    if (Items[Equipment[i]].ItemId != Guid.Empty)
                    {
                        var item = ItemBase.Get(Items[Equipment[i]].ItemId);
                        if (item != null)
                        {
                            if (item.Effect.Type == EffectType.Tenacity)
                            {
                                tenacity += item.Effect.Percentage;
                            }
                        }
                    }
                }
            }

            return tenacity;
        }

        public double GetLuck()
        {
            double luck = 0;

            for (var i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                if (Equipment[i] > -1)
                {
                    if (Items[Equipment[i]].ItemId != Guid.Empty)
                    {
                        var item = ItemBase.Get(Items[Equipment[i]].ItemId);
                        if (item != null)
                        {
                            if (item.Effect.Type == EffectType.Luck)
                            {
                                luck += item.Effect.Percentage;
                            }
                        }
                    }
                }
            }

            return luck;
        }

        public int GetExpMultiplier()
        {
            var exp = 100;

            for (var i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                if (Equipment[i] > -1)
                {
                    if (Items[Equipment[i]].ItemId != Guid.Empty)
                    {
                        var item = ItemBase.Get(Items[Equipment[i]].ItemId);
                        if (item != null)
                        {
                            if (item.Effect.Type == EffectType.EXP)
                            {
                                exp += item.Effect.Percentage;
                            }
                        }
                    }
                }
            }

            return exp;
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
                var itemDescriptor = itemInSlot.Descriptor;
                if (itemDescriptor != null)
                {
                    if (itemDescriptor.Bound)
                    {
                        PacketSender.SendChatMsg(this, Strings.Shops.bound, CustomColors.Items.Bound);

                        return;
                    }

                    //Check if this is a bag with items.. if so don't allow sale
                    if (itemDescriptor.ItemType == ItemTypes.Bag)
                    {
                        if (itemInSlot.TryGetBag(out var bag))
                        {
                            if (!bag.IsEmpty)
                            {
                                PacketSender.SendChatMsg(this, Strings.Bags.onlysellempty, CustomColors.Alerts.Error);
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
                                PacketSender.SendChatMsg(this, Strings.Shops.doesnotaccept, CustomColors.Alerts.Error);

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
                            PacketSender.SendChatMsg(this, Strings.Shops.doesnotaccept, CustomColors.Alerts.Error);

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
                                }
                                else
                                {
                                    PacketSender.SendChatMsg(
                                        this, Strings.Shops.inventoryfull, CustomColors.Alerts.Error, Name
                                    );
                                }
                            }
                        }
                        else
                        {
                            PacketSender.SendChatMsg(this, Strings.Shops.cantafford, CustomColors.Alerts.Error, Name);
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
                    if (!TryTakeItem(c.ItemId, c.Quantity))
                    {
                        for (var i = 0; i < invbackup.Count; i++)
                        {
                            Items[i].Set(invbackup[i]);
                        }

                        PacketSender.SendInventory(this);
                        CraftId = Guid.Empty;

                        return;
                    }
                }

                //Give them the craft
                var quantity = Math.Max(CraftBase.Get(id).Quantity, 1);
                var itm = ItemBase.Get(CraftBase.Get(id).ItemId);
                if (itm == null || !itm.IsStackable)
                {
                    quantity = 1;
                }

                if (TryGiveItem(CraftBase.Get(id).ItemId, quantity))
                {
                    PacketSender.SendChatMsg(
                        this, Strings.Crafting.crafted.ToString(ItemBase.GetName(CraftBase.Get(id).ItemId)),
                        CustomColors.Alerts.Success
                    );
                }
                else
                {
                    for (var i = 0; i < invbackup.Count; i++)
                    {
                        Items[i].Set(invbackup[i]);
                    }

                    PacketSender.SendInventory(this);
                    PacketSender.SendChatMsg(
                        this, Strings.Crafting.nospace.ToString(ItemBase.GetName(CraftBase.Get(id).ItemId)),
                        CustomColors.Alerts.Error
                    );
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
            if (IsBusy())
            {
                return false;
            }

            InBank = true;
            PacketSender.SendOpenBank(this);

            return true;
        }

        public void CloseBank()
        {
            if (InBank)
            {
                InBank = false;
                PacketSender.SendCloseBank(this);
            }
        }

        public bool TryDepositItem(int slot, int amount, bool sendUpdate = true)
        {
            if (!InBank)
            {
                return false;
            }

            var itemBase = ItemBase.Get(Items[slot].ItemId);
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

                    //Find a spot in the bank for it!
                    if (itemBase.IsStackable)
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

                                if (sendUpdate)
                                {
                                    PacketSender.SendInventoryItemUpdate(this, slot);
                                    PacketSender.SendBankUpdate(this, i);
                                }

                                return true;
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

                            if (sendUpdate)
                            {
                                PacketSender.SendInventoryItemUpdate(this, slot);
                                PacketSender.SendBankUpdate(this, i);
                            }

                            return true;
                        }
                    }

                    PacketSender.SendChatMsg(this, Strings.Banks.banknospace, CustomColors.Alerts.Error);
                }
                else
                {
                    PacketSender.SendChatMsg(this, Strings.Banks.depositinvalid, CustomColors.Alerts.Error);
                }
            }

            return false;
        }

        public bool TryDepositItem([NotNull] Item item, bool sendUpdate = true)
        {
            var itemBase = item.Descriptor;

            if (itemBase == null)
            {
                return false;
            }

            if (item.ItemId != Guid.Empty)
            {
                // Find a spot in the bank for it!
                if (itemBase.IsStackable)
                {
                    for (var i = 0; i < Options.MaxBankSlots; i++)
                    {
                        var bankSlot = Bank[i];
                        if (bankSlot != null && bankSlot.ItemId == item.ItemId)
                        {
                            if (item.Quantity <= int.MaxValue - bankSlot.Quantity)
                            {
                                bankSlot.Quantity += item.Quantity;

                                if (sendUpdate)
                                {
                                    PacketSender.SendBankUpdate(this, i);
                                }

                                return true;
                            }
                        }
                    }
                }

                // Either a non stacking item, or we couldn't find the item already existing in the players inventory
                for (var i = 0; i < Options.MaxBankSlots; i++)
                {
                    var bankSlot = Bank[i];

                    if (bankSlot == null || bankSlot.ItemId == Guid.Empty)
                    {
                        bankSlot.Set(item);

                        if (sendUpdate)
                        {
                            PacketSender.SendBankUpdate(this, i);
                        }

                        return true;
                    }
                }

                PacketSender.SendChatMsg(this, Strings.Banks.banknospace, CustomColors.Alerts.Error);
            }
            else
            {
                PacketSender.SendChatMsg(this, Strings.Banks.depositinvalid, CustomColors.Alerts.Error);
            }

            return false;
        }

        public void WithdrawItem(int slot, int amount)
        {
            if (!InBank)
            {
                return;
            }

            Debug.Assert(ItemBase.Lookup != null, "ItemBase.Lookup != null");
            Debug.Assert(Bank != null, "Bank != null");
            Debug.Assert(Items != null, "Inventory != null");

            var bankSlotItem = Bank[slot];
            if (bankSlotItem == null)
            {
                return;
            }

            var itemBase = ItemBase.Get(bankSlotItem.ItemId);
            var inventorySlot = -1;
            if (itemBase == null)
            {
                return;
            }

            if (bankSlotItem.ItemId != Guid.Empty)
            {
                if (itemBase.IsStackable)
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
                if (itemBase.IsStackable)
                {
                    /* Find an existing stack */
                    for (var i = 0; i < Options.MaxInvItems; i++)
                    {
                        var inventorySlotItem = Items[i];
                        if (inventorySlotItem == null)
                        {
                            continue;
                        }

                        if (inventorySlotItem.ItemId != bankSlotItem.ItemId)
                        {
                            continue;
                        }

                        inventorySlot = i;

                        break;
                    }
                }

                if (inventorySlot < 0)
                {
                    /* Find a free slot if we don't have one already */
                    for (var j = 0; j < Options.MaxInvItems; j++)
                    {
                        if (Items[j] != null && Items[j].ItemId != Guid.Empty)
                        {
                            continue;
                        }

                        inventorySlot = j;

                        break;
                    }
                }

                /* If we don't have a slot send an error. */
                if (inventorySlot < 0)
                {
                    PacketSender.SendChatMsg(this, Strings.Banks.inventorynospace, CustomColors.Alerts.Error);

                    return; //Panda forgot this :P
                }

                /* Move the items to the inventory */
                Debug.Assert(Items[inventorySlot] != null, "Inventory[inventorySlot] != null");
                amount = Math.Min(amount, int.MaxValue - Items[inventorySlot].Quantity);

                if (Items[inventorySlot] == null ||
                    Items[inventorySlot].ItemId == Guid.Empty ||
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

                PacketSender.SendInventoryItemUpdate(this, inventorySlot);
                PacketSender.SendBankUpdate(this, slot);
            }
            else
            {
                PacketSender.SendChatMsg(this, Strings.Banks.withdrawinvalid, CustomColors.Alerts.Error);
            }
        }

        public void SwapBankItems(int item1, int item2)
        {
            Item tmpInstance = null;
            if (Bank[item2] != null)
            {
                tmpInstance = Bank[item2].Clone();
            }

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

            PacketSender.SendBankUpdate(this, item1);
            PacketSender.SendBankUpdate(this, item2);
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

        public void StoreBagItem(int slot, int amount)
        {
            if (InBag == null || !HasBag(InBag))
            {
                return;
            }

            var itemBase = ItemBase.Get(Items[slot].ItemId);
            var bag = GetBag();
            if (itemBase != null && bag != null)
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

                    //Make Sure we are not Storing a Bag inside of itself
                    if (Items[slot].Bag == InBag)
                    {
                        PacketSender.SendChatMsg(this, Strings.Bags.baginself, CustomColors.Alerts.Error);

                        return;
                    }

                    if (itemBase.ItemType == ItemTypes.Bag)
                    {
                        PacketSender.SendChatMsg(this, Strings.Bags.baginbag, CustomColors.Alerts.Error);

                        return;
                    }

                    //Find a spot in the bag for it!
                    if (itemBase.IsStackable)
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
                                PacketSender.SendInventoryItemUpdate(this, slot);
                                PacketSender.SendBagUpdate(this, i, bag.Slots[i]);

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
                            PacketSender.SendInventoryItemUpdate(this, slot);
                            PacketSender.SendBagUpdate(this, i, bag.Slots[i]);

                            return;
                        }
                    }

                    PacketSender.SendChatMsg(this, Strings.Bags.bagnospace, CustomColors.Alerts.Error);
                }
                else
                {
                    PacketSender.SendChatMsg(this, Strings.Bags.depositinvalid, CustomColors.Alerts.Error);
                }
            }
        }

        public void RetrieveBagItem(int slot, int amount)
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

            var itemBase = ItemBase.Get(bag.Slots[slot].ItemId);
            var inventorySlot = -1;
            if (itemBase != null)
            {
                if (bag.Slots[slot] != null && bag.Slots[slot].ItemId != Guid.Empty)
                {
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

                    //Find a spot in the inventory for it!
                    if (itemBase.IsStackable)
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
                        PacketSender.SendChatMsg(this, Strings.Bags.inventorynospace, CustomColors.Alerts.Error);

                        return; //Panda forgot this :P
                    }

                    /* Move the items to the inventory */
                    amount = Math.Min(amount, int.MaxValue - Items[inventorySlot].Quantity);

                    if (Items[inventorySlot] == null ||
                        Items[inventorySlot].ItemId == Guid.Empty ||
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

                    PacketSender.SendInventoryItemUpdate(this, inventorySlot);
                    PacketSender.SendBagUpdate(this, slot, bag.Slots[slot]);
                }
                else
                {
                    PacketSender.SendChatMsg(this, Strings.Bags.withdrawinvalid, CustomColors.Alerts.Error);
                }
            }
        }

        public void SwapBagItems(int item1, int item2)
        {
            if (InBag != null || !HasBag(InBag))
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

            if (!FriendRequests.ContainsKey(fromPlayer) || !(FriendRequests[fromPlayer] > Globals.Timing.TimeMs))
            {
                if (Trading.Requester == null && PartyRequester == null && FriendRequester == null)
                {
                    FriendRequester = fromPlayer;
                    PacketSender.SendFriendRequest(this, fromPlayer);
                    PacketSender.SendChatMsg(fromPlayer, Strings.Friends.sent, CustomColors.Alerts.RequestSent);
                }
                else
                {
                    PacketSender.SendChatMsg(
                        fromPlayer, Strings.Friends.busy.ToString(Name), CustomColors.Alerts.Error
                    );
                }
            }
        }

        public bool HasFriend(Player character)
        {
            foreach (var friend in Friends)
            {
                if (friend.Target == character)
                {
                    return true;
                }
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
            if (friend != null)
            {
                Friends.Remove(friend);
            }
        }

        // Guilds
        public void SendGuildInvite(Player from)
        {
            // Are we already in a guild? or have a pending invite?
            if (Guild == null && GuildInvite == null)
            {
                // Thank god, we can FINALLY get started!
                // Set our invite and send our players the relevant messages.
                GuildInvite = new Tuple<Player, Guild>(from, from.Guild);

                PacketSender.SendChatMsg(from, Strings.Guilds.InviteSent.ToString(Name, from.Guild.Name), CustomColors.Alerts.Info);
                PacketSender.SendGuildInvite(this, from);
            }
            else
            {
                PacketSender.SendChatMsg(from, Strings.Guilds.InviteAlreadyInGuild, CustomColors.Alerts.Error);
            }
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

            if (Trading.Requests.ContainsKey(fromPlayer) && Trading.Requests[fromPlayer] > Globals.Timing.TimeMs)
            {
                PacketSender.SendChatMsg(fromPlayer, Strings.Trading.alreadydenied, CustomColors.Alerts.Error);
            }
            else
            {
                if (Trading.Requester == null && PartyRequester == null && FriendRequester == null)
                {
                    Trading.Requester = fromPlayer;
                    PacketSender.SendTradeRequest(this, fromPlayer);
                }
                else
                {
                    PacketSender.SendChatMsg(
                        fromPlayer, Strings.Trading.busy.ToString(Name), CustomColors.Alerts.Error
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

            var itemBase = ItemBase.Get(Items[slot].ItemId);
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
                    if (itemBase.Bound)
                    {
                        PacketSender.SendChatMsg(this, Strings.Bags.tradebound, CustomColors.Items.Bound);

                        return;
                    }

                    //Check if this is a bag with items.. if so don't allow sale
                    if (itemBase.ItemType == ItemTypes.Bag)
                    {
                        if (Items[slot].TryGetBag(out var bag))
                        {
                            if (!bag.IsEmpty)
                            {
                                PacketSender.SendChatMsg(this, Strings.Bags.onlytradeempty, CustomColors.Alerts.Error);
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

                    PacketSender.SendChatMsg(this, Strings.Trading.tradenospace, CustomColors.Alerts.Error);
                }
                else
                {
                    PacketSender.SendChatMsg(this, Strings.Trading.offerinvalid, CustomColors.Alerts.Error);
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

            var itemBase = ItemBase.Get(Trading.Offer[slot].ItemId);
            if (itemBase == null)
            {
                return;
            }

            if (Trading.Offer[slot] == null || Trading.Offer[slot].ItemId == Guid.Empty)
            {
                PacketSender.SendChatMsg(this, Strings.Trading.revokeinvalid, CustomColors.Alerts.Error);

                return;
            }

            var inventorySlot = -1;
            var stackable = itemBase.IsStackable;
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
                PacketSender.SendChatMsg(this, Strings.Trading.inventorynospace, CustomColors.Alerts.Error);
            }

            if (amount > Trading.Offer[slot].Quantity)
            {
                amount = Trading.Offer[slot].Quantity;
            }

            /* Move the items to the inventory */
            amount = Math.Min(amount, int.MaxValue - Items[inventorySlot].Quantity);

            if (Items[inventorySlot] == null ||
                Items[inventorySlot].ItemId == Guid.Empty ||
                Items[inventorySlot].Quantity < 0)
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

            PacketSender.SendInventoryItemUpdate(this, inventorySlot);
            PacketSender.SendTradeUpdate(this, this, slot);
            PacketSender.SendTradeUpdate(Trading.Counterparty, this, slot);
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

                if (!TryGiveItem(offer.ItemId, offer.Quantity))
                {
                    MapInstance.Get(MapId)?.SpawnItem(X, Y, offer, offer.Quantity, Id);
                    PacketSender.SendChatMsg(this, Strings.Trading.itemsdropped, CustomColors.Alerts.Error);
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
            PacketSender.SendChatMsg(Trading.Counterparty, Strings.Trading.declined, CustomColors.Alerts.Error);
            PacketSender.SendTradeClose(Trading.Counterparty);
            Trading.Counterparty.Trading.Counterparty = null;

            ReturnTradeItems();
            PacketSender.SendChatMsg(this, Strings.Trading.declined, CustomColors.Alerts.Error);
            PacketSender.SendTradeClose(this);
            Trading.Counterparty = null;
        }

        //Parties
        public void InviteToParty(Player fromPlayer)
        {
            if (Party.Count != 0)
            {
                PacketSender.SendChatMsg(fromPlayer, Strings.Parties.inparty.ToString(Name), CustomColors.Alerts.Error);

                return;
            }

            if (fromPlayer.PartyRequests.ContainsKey(this))
            {
                fromPlayer.PartyRequests.Remove(this);
            }

            if (PartyRequests.ContainsKey(fromPlayer) && PartyRequests[fromPlayer] > Globals.Timing.TimeMs)
            {
                PacketSender.SendChatMsg(fromPlayer, Strings.Parties.alreadydenied, CustomColors.Alerts.Error);
            }
            else
            {
                if (Trading.Requester == null && PartyRequester == null && FriendRequester == null)
                {
                    PartyRequester = fromPlayer;
                    PacketSender.SendPartyInvite(this, fromPlayer);
                }
                else
                {
                    PacketSender.SendChatMsg(
                        fromPlayer, Strings.Parties.busy.ToString(Name), CustomColors.Alerts.Error
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
                    PacketSender.SendChatMsg(this, Strings.Parties.leaderinvonly, CustomColors.Alerts.Error);

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
                target.LeaveParty();
                Party.Add(target);

                //Update all members of the party with the new list
                for (var i = 0; i < Party.Count; i++)
                {
                    Party[i].Party = Party;
                    PacketSender.SendParty(Party[i]);
                    PacketSender.SendChatMsg(
                        Party[i], Strings.Parties.joined.ToString(target.Name), CustomColors.Alerts.Accepted
                    );
                }
            }
            else
            {
                PacketSender.SendChatMsg(this, Strings.Parties.limitreached, CustomColors.Alerts.Error);
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
                        PacketSender.SendChatMsg(oldMember, Strings.Parties.kicked, CustomColors.Alerts.Error);
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
                                    CustomColors.Alerts.Error
                                );
                            }
                        }
                        else if (Party.Count > 0) //Check if anyone is left on their own
                        {
                            var remainder = Party[0];
                            remainder.Party.Clear();
                            PacketSender.SendParty(remainder);
                            PacketSender.SendChatMsg(remainder, Strings.Parties.disbanded, CustomColors.Alerts.Error);
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
                            Party[i], Strings.Parties.memberleft.ToString(oldMember.Name), CustomColors.Alerts.Error
                        );
                    }
                }
                else if (Party.Count > 0) //Check if anyone is left on their own
                {
                    var remainder = Party[0];
                    remainder.Party.Clear();
                    PacketSender.SendParty(remainder);
                    PacketSender.SendChatMsg(remainder, Strings.Parties.disbanded, CustomColors.Alerts.Error);
                }

                PacketSender.SendChatMsg(this, Strings.Parties.left, CustomColors.Alerts.Error);
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
                PacketSender.SendChatMsg(this, Strings.Combat.tryforgetboundspell);
            }
        }

        public bool TryForgetSpell([NotNull] Spell spell, bool sendUpdate = true)
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
                PacketSender.SendChatMsg(this, Strings.Combat.tryforgetboundspell);

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

        public virtual bool IsAllyOf([NotNull] Player otherPlayer)
        {
            return base.IsAllyOf(otherPlayer) || this.InParty(otherPlayer);
        }

        public bool CanSpellCast(SpellBase spell, Entity target, bool checkVitalReqs)
        {
            if (!Conditions.MeetsConditionLists(spell.CastingRequirements, this, null))
            {
                PacketSender.SendChatMsg(this, Strings.Combat.dynamicreq);

                return false;
            }

            //Check if the caster is silenced or stunned. Clense casts break the rule.
            if (spell.Combat.Effect != StatusTypes.Cleanse)
            {
                var statuses = Statuses.Values.ToArray();
                foreach (var status in statuses)
                {
                    if (status.Type == StatusTypes.Silence)
                    {
                        PacketSender.SendChatMsg(this, Strings.Combat.silenced);

                        return false;
                    }

                    if (status.Type == StatusTypes.Stun)
                    {
                        PacketSender.SendChatMsg(this, Strings.Combat.stunned);

                        return false;
                    }

                    if (status.Type == StatusTypes.Sleep)
                    {
                        PacketSender.SendChatMsg(this, Strings.Combat.sleep);

                        return false;
                    }
                }
            }

            //Check if the caster has the right ammunition if a projectile
            if (spell.SpellType == SpellTypes.CombatSpell &&
                spell.Combat.TargetType == SpellTargetTypes.Projectile &&
                spell.Combat.ProjectileId != Guid.Empty)
            {
                var projectileBase = spell.Combat.Projectile;
                if (projectileBase == null)
                {
                    return false;
                }

                if (projectileBase.AmmoItemId != Guid.Empty)
                {
                    if (FindInventoryItemSlot(projectileBase.AmmoItemId, projectileBase.AmmoRequired) == null)
                    {
                        PacketSender.SendChatMsg(
                            this, Strings.Items.notenough.ToString(ItemBase.GetName(projectileBase.AmmoItemId)),
                            CustomColors.Alerts.Error
                        );

                        return false;
                    }
                }
            }

            var singleTargetCombatSpell = spell.SpellType == SpellTypes.CombatSpell &&
                                          spell.Combat.TargetType == SpellTargetTypes.Single;

            if (target == null && (spell.SpellType == SpellTypes.WarpTo || singleTargetCombatSpell))
            {
                PacketSender.SendActionMsg(this, Strings.Combat.notarget, CustomColors.Combat.NoTarget);

                return false;
            }

            if (target != null && singleTargetCombatSpell)
            {
                if (spell.Combat.Friendly && !IsAllyOf(target))
                {
                    return false;
                }

                if (!spell.Combat.Friendly && IsAllyOf(target))
                {
                    return false;
                }
            }

            //Check for range of a single target spell
            if (spell.SpellType == (int) SpellTypes.CombatSpell &&
                spell.Combat.TargetType == SpellTargetTypes.Single &&
                target != this)
            {
                if (!InRangeOf(target, spell.Combat.CastRange))
                {
                    PacketSender.SendActionMsg(this, Strings.Combat.targetoutsiderange, CustomColors.Combat.NoTarget);

                    return false;
                }
            }

            if (checkVitalReqs)
            {
                if (spell.VitalCost[(int) Vitals.Mana] > GetVital(Vitals.Mana))
                {
                    PacketSender.SendChatMsg(this, Strings.Combat.lowmana);

                    return false;
                }

                if (spell.VitalCost[(int) Vitals.Health] > GetVital(Vitals.Health))
                {
                    PacketSender.SendChatMsg(this, Strings.Combat.lowhealth);

                    return false;
                }
            }

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

            if (!CanSpellCast(spell, target, true))
            {
                return;
            }

            if (!SpellCooldowns.ContainsKey(Spells[spellSlot].SpellId) ||
                SpellCooldowns[Spells[spellSlot].SpellId] < Globals.Timing.RealTimeMs)
            {
                if (CastTime == 0)
                {
                    CastTime = Globals.Timing.TimeMs + spell.CastDuration;

                    if (spell.VitalCost[(int) Vitals.Mana] > 0)
                    {
                        SubVital(Vitals.Mana, spell.VitalCost[(int) Vitals.Mana]);
                    }
                    else
                    {
                        AddVital(Vitals.Mana, -spell.VitalCost[(int) Vitals.Mana]);
                    }

                    if (spell.VitalCost[(int) Vitals.Health] > 0)
                    {
                        SubVital(Vitals.Health, spell.VitalCost[(int) Vitals.Health]);
                    }
                    else
                    {
                        AddVital(Vitals.Health, -spell.VitalCost[(int) Vitals.Health]);
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
                            spell.CastAnimationId, 1, base.Id, MapId, 0, 0, (sbyte) Dir
                        ); //Target Type 1 will be global entity
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
                    PacketSender.SendChatMsg(this, Strings.Combat.channeling);
                }
            }
            else
            {
                PacketSender.SendChatMsg(this, Strings.Combat.cooldown);
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
                        var tempItemBase = ItemBase.Get(Items[i].ItemId);
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
            }
        }

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
            for (var i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                if (Equipment[i] == slot)
                {
                    Equipment[i] = -1;
                }
            }

            FixVitals();
            PacketSender.SendPlayerEquipmentToProximity(this);
            PacketSender.SendEntityStats(this);
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

        public bool QuestInProgress(QuestBase quest, QuestProgressState progress, Guid taskId)
        {
            var questProgress = FindQuest(quest.Id);
            if (questProgress != null)
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
                    this, Strings.Quests.started.ToString(quest.Name), CustomColors.Quests.Started
                );

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
                        this, Strings.Quests.declined.ToString(QuestBase.GetName(questId)), CustomColors.Quests.Declined
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
                if (QuestInProgress(quest, QuestProgressState.OnAnyTask, Guid.Empty))
                {
                    //Cancel the quest somehow...
                    if (quest.Quitable)
                    {
                        var questProgress = FindQuest(quest.Id);
                        questProgress.TaskId = Guid.Empty;
                        questProgress.TaskProgress = -1;
                        PacketSender.SendChatMsg(
                            this, Strings.Quests.abandoned.ToString(QuestBase.GetName(questId)), Color.Red
                        );

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
                                PacketSender.SendChatMsg(this, Strings.Quests.taskcompleted);
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
                                        this, Strings.Quests.completed.ToString(quest.Name), Color.Green
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
                                        CustomColors.Quests.TaskUpdated
                                    );
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
                        PacketSender.SendChatMsg(this, Strings.Quests.completed.ToString(quest.Name), Color.Green);
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
                                        PacketSender.SendChatMsg(
                                            this,
                                            Strings.Quests.itemtask.ToString(
                                                quest.Name, questProgress.TaskProgress, questTask.Quantity,
                                                ItemBase.GetName(questTask.TargetId)
                                            )
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
        private Variable GetSwitch(Guid id)
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
            if (s != null)
            {
                s.Value.Boolean = value;
            }
            else
            {
                s = new Variable(id);
                s.Value.Boolean = value;
                Variables.Add(s);
            }
        }

        public Variable GetVariable(Guid id, bool createIfNull = false)
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

        private Variable CreateVariable(Guid id)
        {
            if (PlayerVariableBase.Get(id) == null)
            {
                return null;
            }

            var variable = new Variable(id);
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
            if (v != null)
            {
                v.Value.Integer = value;
            }
            else
            {
                v = new Variable(id);
                v.Value.Integer = value;
                Variables.Add(v);
            }
        }

        public void SetVariableValue(Guid id, string value)
        {
            var v = GetVariable(id);
            if (v != null)
            {
                v.Value.String = value;
            }
            else
            {
                v = new Variable(id);
                v.Value.String = value;
                Variables.Add(v);
            }
        }

        //Event Processing Methods
        public Event EventExists(Guid mapId, int x, int y)
        {
            foreach (var evt in EventLookup)
            {
                if (evt.Value.MapId == mapId && evt.Value.BaseEvent.SpawnX == x && evt.Value.BaseEvent.SpawnY == y)
                {
                    return evt.Value;
                }
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
                            var cmd = (InputVariableCommand) stackInfo.WaitingOnCommand;
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

                            if (value == null)
                            {
                                value = new VariableValue();
                            }

                            var success = false;

                            if (!canceled)
                            {
                                switch (type)
                                {
                                    case VariableDataTypes.Integer:
                                        if (newValue >= cmd.Minimum && newValue <= cmd.Maximum)
                                        {
                                            value.Integer = newValue;
                                            success = true;
                                        }

                                        break;
                                    case VariableDataTypes.Number:
                                        if (newValue >= cmd.Minimum && newValue <= cmd.Maximum)
                                        {
                                            value.Number = newValue;
                                            success = true;
                                        }

                                        break;
                                    case VariableDataTypes.String:
                                        if (newValueString.Length >= cmd.Minimum &&
                                            newValueString.Length <= cmd.Maximum)
                                        {
                                            value.String = newValueString;
                                            success = true;
                                        }

                                        break;
                                    case VariableDataTypes.Boolean:
                                        value.Boolean = newValue > 0;
                                        success = true;

                                        break;
                                }
                            }

                            //Reassign variable values in case they didnt already exist and we made them from scratch at the null check above
                            if (cmd.VariableType == VariableTypes.PlayerVariable)
                            {
                                var variable = GetVariable(cmd.VariableId);
                                variable.Value = value;
                            }
                            else if (cmd.VariableType == VariableTypes.ServerVariable)
                            {
                                var variable = ServerVariableBase.Get(cmd.VariableId);
                                if (variable != null)
                                {
                                    variable.Value = value;
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

        public Event FindEvent(EventPageInstance en)
        {
            lock (mEventLock)
            {
                foreach (var evt in EventLookup)
                {
                    if (evt.Value.PageInstance == null)
                    {
                        continue;
                    }

                    if (evt.Value.PageInstance == en || evt.Value.PageInstance.GlobalClone == en)
                    {
                        return evt.Value;
                    }
                }
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

            lock (mEventLock)
            {
                foreach (var evt in EventLookup)
                {
                    if (evt.Value.BaseEvent == baseEvent)
                    {
                        return false;
                    }
                }

                mCommonEventLaunches++;
                var commonEventLaunch = mCommonEventLaunches;

                //Use Fake Ids for Common Events Since they are not tied to maps and such
                var evtId = Guid.NewGuid();
                var mapId = Guid.Empty;
                var tmpEvent = new Event(evtId, Guid.Empty, this, baseEvent)
                {
                    MapId = mapId,
                    SpawnX = -1,
                    SpawnY = -1
                };

                EventLookup.AddOrUpdate(evtId, tmpEvent, (key, oldValue) => tmpEvent);

                //Try to Spawn a PageInstance.. if we can
                for (var i = baseEvent.Pages.Count - 1; i >= 0; i--)
                {
                    if ((trigger == CommonEventTrigger.None || baseEvent.Pages[i].CommonTrigger == trigger) &&
                        Conditions.CanSpawnPage(baseEvent.Pages[i], this, null))
                    {
                        tmpEvent.PageInstance = new EventPageInstance(
                            baseEvent, baseEvent.Pages[i], mapId, tmpEvent, this
                        );

                        tmpEvent.PageIndex = i;

                        //Check for /command trigger
                        if (trigger == CommonEventTrigger.SlashCommand)
                        {
                            if (command.ToLower() == tmpEvent.PageInstance.MyPage.TriggerCommand.ToLower())
                            {
                                //Split params up
                                var prams = param.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                                for (var x = 0; x < prams.Length; x++)
                                {
                                    tmpEvent.SetParam("slashParam" + x, prams[x]);
                                }

                                var newStack = new CommandInstance(tmpEvent.PageInstance.MyPage);
                                tmpEvent.PageInstance.Param = param;
                                tmpEvent.CallStack.Push(newStack);

                                return true;
                            }
                        }
                        else
                        {
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
                                    tmpEvent.SetParam("victim", param);

                                    break;
                                case CommonEventTrigger.PVPDeath:
                                    //Add killer as a parameter
                                    tmpEvent.SetParam("killer", param);

                                    break;
                                case CommonEventTrigger.PlayerInteract:
                                    //Interactee as a parameter
                                    tmpEvent.SetParam("triggered", param);

                                    break;
                            }

                            var newStack = new CommandInstance(tmpEvent.PageInstance.MyPage);
                            tmpEvent.CallStack.Push(newStack);

                            return true;
                        }

                        break;
                    }
                }

                EventLookup.TryRemove(evtId, out var z);

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

        protected override int IsTileWalkable(MapInstance map, int x, int y, int z)
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
            var oldMap = MapId;
            base.Move(moveDir, forPlayer, dontUpdate, correction);

            // Check for a warp, if so warp the player.
            var attribute = MapInstance.Get(MapId).Attributes[X, Y];
            if (attribute != null && attribute.Type == MapAttributes.Warp)
            {
                var warpAtt = (MapWarpAttribute) attribute;
                if (warpAtt.Direction == WarpDirection.Retain)
                {
                    Warp(warpAtt.MapId, warpAtt.X, warpAtt.Y, (byte) Dir);
                }
                else
                {
                    Warp(warpAtt.MapId, warpAtt.X, warpAtt.Y, (byte) (warpAtt.Direction - 1));
                }
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
        }

        public void TryBumpEvent(Guid mapId, Guid eventId)
        {
            foreach (var evt in EventLookup)
            {
                if (evt.Value.MapId == MapId)
                {
                    if (evt.Value.PageInstance != null && evt.Value.PageInstance.MapId == MapId && evt.Value.BaseEvent.Id == eventId)
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

        //TODO: Clean all of this stuff up

        #region Temporary Values

        [NotMapped, JsonIgnore] public bool InGame;

        [NotMapped, JsonIgnore] public Guid LastMapEntered = Guid.Empty;

        [JsonIgnore, NotMapped] public Client Client;

        [JsonIgnore, NotMapped]
        public UserRights Power => Client?.Power ?? UserRights.None;

        [JsonIgnore, NotMapped] private bool mSentMap;

        [JsonIgnore, NotMapped] private int mCommonEventLaunches = 0;

        [JsonIgnore, NotMapped] private object mEventLock = new object();

        [JsonIgnore, NotMapped]
        public ConcurrentDictionary<Guid, Event> EventLookup = new ConcurrentDictionary<Guid, Event>();

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

        [NotMapped] public bool InBank;

        #endregion

        #region Item Cooldowns

        [JsonIgnore, Column("ItemCooldowns")]
        public string ItemCooldownsJson
        {
            get => JsonConvert.SerializeObject(ItemCooldowns);
            set => ItemCooldowns = JsonConvert.DeserializeObject<Dictionary<Guid, long>>(value ?? "{}");
        }

        [JsonIgnore] public Dictionary<Guid, long> ItemCooldowns = new Dictionary<Guid, long>();

        #endregion

    }

}
