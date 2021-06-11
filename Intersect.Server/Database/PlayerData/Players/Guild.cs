using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using Intersect.Enums;
using Intersect.Server.Entities;
using JetBrains.Annotations;
using Intersect.Server.Networking;
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Intersect.Network.Packets.Server;
using Intersect.GameObjects;
using Intersect.GameObjects.Maps;
using Intersect.Server.General;
using Intersect.Server.Web.RestApi.Payloads;
using Intersect.Logging;
using Intersect.Utilities;
using Intersect.Server.Localization;
using Intersect.Server.Database.Logging.Entities;
using static Intersect.Server.Database.Logging.Entities.GuildHistory;

namespace Intersect.Server.Database.PlayerData.Players
{
    /// <summary>
    /// A class containing the definition of each guild, alongside the methods to use them.
    /// </summary>
    public class Guild
    {

        public static ConcurrentDictionary<Guid, Guild> Guilds = new ConcurrentDictionary<Guid, Guild>();

        // Entity Framework Garbage.
        public Guild()
        {
        }

        /// <summary>
        /// The database Id of the guild.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; private set; }

        /// <summary>
        /// The name of the guild.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The date on which this guild was founded.
        /// </summary>
        public DateTime FoundingDate { get; private set; }

        /// <summary>
        /// Guild bank slots
        /// </summary>
        [JsonIgnore]
        public virtual List<GuildBankSlot> Bank { get; set; } = new List<GuildBankSlot>();

        /// <summary>
        /// Sets the number of bank slots alotted to this guild. Banks lots can only expand.
        /// </summary>
        public int BankSlotsCount { get; set; } = Options.Instance.Guild.GuildBankSlots;

        /// <summary>
        /// Contains a record of all guild members
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public ConcurrentDictionary<Guid, GuildMember> Members { get; private set; } = new ConcurrentDictionary<Guid, GuildMember>();

        /// <summary>
        /// The last time this guilds status was updated and memberlist was send to online players
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public long LastUpdateTime { get; set; } = -1;

        /// <summary>
        /// Guild Variable Values
        /// </summary>
        [JsonIgnore]
        public virtual List<GuildVariable> Variables { get; set; } = new List<GuildVariable>();

        /// <summary>
        /// Variables that have been updated for this guild which need to be saved to the db
        /// </summary>
        public ConcurrentDictionary<Guid, GuildVariableBase> UpdatedVariables = new ConcurrentDictionary<Guid, GuildVariableBase>();

        /// <summary>
        /// Locking context to prevent saving this guild to the db twice at the same time, and to prevent bank items from being withdrawed/deposited into by 2 threads at the same time
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        private object mLock = new object();


        /// <summary>
        /// Create a new Guild instance.
        /// </summary>
        /// <param name="creator">The <see cref="Player"/> that created the guild.</param>
        /// <param name="name">The Name of the guild.</param>
        public static Guild CreateGuild(Player creator, string name)
        {
            name = name.Trim();

            if (creator != null && FieldChecking.IsValidGuildName(name, Strings.Regex.guildname))
            {
                using (var context = DbInterface.CreatePlayerContext(readOnly: false))
                {
                    var guild = new Guild()
                    {
                        Name = name,
                        FoundingDate = DateTime.UtcNow
                    };

                    SlotHelper.ValidateSlots(guild.Bank, Options.Instance.Guild.GuildBankSlots);
                    guild.Bank = guild.Bank.OrderBy(bankSlot => bankSlot?.Slot).ToList();

                    var player = context.Players.FirstOrDefault(p => p.Id == creator.Id);
                    if (player != null)
                    {
                        player.DbGuild = guild;
                        player.GuildRank = 0;
                        player.GuildJoinDate = DateTime.UtcNow;


                        context.ChangeTracker.DetectChanges();
                        context.SaveChanges();

                        var member = new GuildMember(player.Id, player.Name, player.GuildRank, player.Level, player.ClassName, player.MapName);
                        guild.Members.AddOrUpdate(player.Id, member, (key, oldValue) => member);

                        creator.Guild = guild;
                        creator.GuildRank = 0;
                        creator.GuildJoinDate = DateTime.UtcNow;

                        // Send our entity data to nearby players.
                        PacketSender.SendEntityDataToProximity(Player.FindOnline(creator.Id));

                        Guilds.AddOrUpdate(guild.Id, guild, (key, oldValue) => guild);

                        LogActivity(guild.Id, creator, null, GuildActivityType.Created, name);

                        return guild;
                    }
                }
            }
            return null;

        }

        /// <summary>
        /// Loads a guild and it's members from the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Guild LoadGuild(Guid id)
        {
            if (!Guilds.TryGetValue(id, out Guild found))
            {
                using (var context = DbInterface.CreatePlayerContext())
                {
                    var guild = context.Guilds.Where(g => g.Id == id).Include(g => g.Bank).Include(g => g.Variables).FirstOrDefault();
                    if (guild != null)
                    {
                        //Load Members
                        var members = context.Players.Where(p => p.DbGuild.Id == id).ToDictionary(t => t.Id, t => new Tuple<Guid, string, int, int, Guid, Guid>(t.Id, t.Name, t.GuildRank, t.Level, t.ClassId, t.MapId));
                        foreach (var member in members)
                        {
                            var gmember = new GuildMember(member.Value.Item1, member.Value.Item2, member.Value.Item3, member.Value.Item4, ClassBase.GetName(member.Value.Item5), MapBase.GetName(member.Value.Item6));
                            guild.Members.AddOrUpdate(member.Key, gmember, (key, oldValue) => gmember);
                        }

                        SlotHelper.ValidateSlots(guild.Bank, guild.BankSlotsCount);
                        guild.Bank = guild.Bank.OrderBy(bankSlot => bankSlot?.Slot).ToList();

                        Guilds.AddOrUpdate(id, guild, (key, oldValue) => guild);

                        return guild;
                    }
                }
            }
            else
            {
                return found;
            }
            return null;
        }

        /// <summary>
        /// Find all online members of this guild.
        /// </summary>
        /// <returns>A list of online players.</returns>
        public List<Player> FindOnlineMembers()
        {
            var online = new List<Player>();
            foreach (var member in Members)
            {
                var plyr = Player.FindOnline(member.Key);
                if (plyr != null)
                {
                    //Update Cached Member List Values
                    member.Value.Name = plyr.Name;
                    member.Value.Class = plyr.ClassName;
                    member.Value.Level = plyr.Level;
                    member.Value.Map = plyr.MapName;
                    member.Value.Rank = plyr.GuildRank;

                    online.Add(plyr);
                }
            }

            return online;
        }

        /// <summary>
        /// Joins a player into the guild.
        /// </summary>
        /// <param name="player">The player to join into the guild.</param>
        /// <param name="rank">Integer index of the rank to give this new member.</param>
        /// <param name="initiator">The player who initiated this change (null if done by the api or some other method).</param>
        public void AddMember(Player player, int rank, Player initiator = null)
        {
            if (player != null && !Members.Any(m => m.Key == player.Id))
            {
                using (var context = DbInterface.CreatePlayerContext(readOnly: false))
                {
                    var dbPlayer = context.Players.FirstOrDefault(p => p.Id == player.Id);
                    if (dbPlayer != null)
                    {
                        dbPlayer.DbGuild = this;
                        dbPlayer.GuildRank = rank;
                        dbPlayer.GuildJoinDate = DateTime.UtcNow;
                        context.ChangeTracker.DetectChanges();
                        DetachGuildFromDbContext(context, this);
                        context.SaveChanges();

                        player.Guild = this;
                        player.GuildRank = rank;
                        player.GuildJoinDate = DateTime.UtcNow;

                        var member = new GuildMember(player.Id, player.Name, player.GuildRank, player.Level, player.ClassName, player.MapName);
                        Members.AddOrUpdate(player.Id, member, (key, oldValue) => member);

                        // Send our new guild list to everyone that's online.
                        UpdateMemberList();

                        // Send our entity data to nearby players.
                        PacketSender.SendEntityDataToProximity(Player.FindOnline(player.Id));

                        LogActivity(Id, player, initiator, GuildActivityType.Joined);
                    }
                }
            }
        }

        /// <summary>
        /// Removes a player from the guild.
        /// </summary>
        /// <param name="player">The player to remove from the guild.</param>
        /// <param name="initiator">The player who initiated this change (null if done by the api or some other method).</param>
        public void RemoveMember(Player player, Player initiator = null, GuildActivityType action = GuildActivityType.Left)
        {
            if (player != null)
            {
                using (var context = DbInterface.CreatePlayerContext(readOnly: false))
                {
                    var dbPlayer = context.Players.Where(p => p.Id == player.Id && p.DbGuild.Id == this.Id).Include(p => p.DbGuild).FirstOrDefault();
                    if (dbPlayer != null)
                    {
                        dbPlayer.DbGuild = null;
                        dbPlayer.GuildRank = 0;

                        context.ChangeTracker.DetectChanges();
                        context.SaveChanges();

                        player.Guild = null;
                        player.GuildRank = 0;
                        player.GuildInvite = null;

                        if (player.BankInterface != null && player.GuildBank)
                        {
                            player.BankInterface.Dispose();
                        }

                        Members.TryRemove(player.Id, out GuildMember removed);

                        // Send our new guild list to everyone that's online.
                        UpdateMemberList();

                        // Send our entity data to nearby players.
                        PacketSender.SendEntityDataToProximity(Player.FindOnline(player.Id));

                        LogActivity(Id, player, initiator, action);
                    }
                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Update()
        {
            if (UpdatedVariables.Count > 0)
            {
                Save();
                UpdatedVariables.Clear();
            }
            UpdateMemberList();
        }

        /// <summary>
        /// Send an updated version of our guild's memberlist to each online member.
        /// </summary>
        public void UpdateMemberList()
        {
            foreach (var member in FindOnlineMembers())
            {
                PacketSender.SendGuild(member);
            }
            LastUpdateTime = Globals.Timing.Milliseconds;
        }

        /// <summary>
        /// Sets a player's guild rank.
        /// </summary>
        /// <param name="id">The player to set the rank for.</param>
        /// <param name="rank">The integer index of the rank to assign.</param>
        /// <param name="initiator">The player who initiated this change (null if done by the api or some other method).</param>
        public void SetPlayerRank(Guid id, int rank, Player initiator = null) => SetPlayerRank(Player.Find(id), rank, initiator);

        /// <summary>
        /// Sets a player's guild rank.
        /// </summary>
        /// <param name="player">The player to set the rank for.</param>
        /// <param name="rank">The integer index of the rank to assign.</param>
        /// <param name="initiator">The player who initiated this change (null if done by the api or some other method).</param>
        public void SetPlayerRank(Player player, int rank, Player initiator = null)
        {
            using (var context = DbInterface.CreatePlayerContext(readOnly: false))
            {
                var dbPlayer = context.Players.Where(p => p.Id == player.Id && p.DbGuild.Id == this.Id).FirstOrDefault();
                if (dbPlayer != null)
                {
                    var origRank = dbPlayer.GuildRank;

                    dbPlayer.GuildRank = rank;

                    context.ChangeTracker.DetectChanges();
                    context.SaveChanges();

                    player.GuildRank = rank;

                    if (Members.TryGetValue(player.Id, out GuildMember val))
                    {
                        val.Rank = rank;
                    }

                    // Send our new guild list to everyone that's online.
                    UpdateMemberList();

                    // Send our entity data to nearby players.
                    PacketSender.SendEntityDataToProximity(Player.FindOnline(player.Id));

                    //Log this change
                    if (origRank < rank)
                    {
                        LogActivity(Id, player, initiator, GuildActivityType.Demoted, rank.ToString());
                    }
                    else if (origRank > rank)
                    {
                        LogActivity(Id, player, initiator, GuildActivityType.Promoted, rank.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Returns the rank of a player within this guild.
        /// </summary>
        /// <param name="id">The Id of the player to search for.</param>
        /// <returns>Returns the integer index of the rank of a player within this guild.</returns>
        public int GetPlayerRank(Guid id) => GetPlayerRank(Player.Find(id));

        /// <summary>
        /// Returns the rank of a player within this guild.
        /// </summary>
        /// <param name="player">The player to search for.</param>
        /// <returns>Returns the integer index of the rank of a player within this guild.</returns>
        public int GetPlayerRank(Player player) => player.GuildRank;

        /// <summary>
        /// Check whether a specified player is a member of this guild.
        /// </summary>
        /// <param name="id">The Id to check against.</param>
        /// <returns>Whether or not this player is a member of this guild</returns>
        public bool IsMember(Guid id) => IsMember(Player.Find(id));

        /// <summary>
        /// Check whether a specified player is a member of this guild.
        /// </summary>
        /// <param name="player">The player to check against.</param>
        /// <returns>Whether or not this player is a member of this guild</returns>
        public bool IsMember(Player player)
        {
            return Members.ContainsKey(player?.Id ?? Guid.Empty);
        }

        /// <summary>
        /// Search for a guild by Id.
        /// </summary>
        /// <param name="id">The guild Id to search for.</param>
        /// <returns>Returns a <see cref="Guild"/> that matches the Id, if any.</returns>
        public static Guild GetGuild(Guid id)
        {
            using (var context = DbInterface.CreatePlayerContext())
            {
                var guild = context.Guilds.FirstOrDefault(g => g.Id == id);
                if (guild != null)
                {
                    return guild;
                }
            }

            return null;
        }

        /// <summary>
        /// Search for a guild by Name.
        /// </summary>
        /// <param name="name">The guild Name to search for.</param>
        /// <returns>Returns a <see cref="Guild"/> that matches the Name, if any.</returns>
        public static Guild GetGuild(string name)
        {
            name = name.Trim();
            using (var context = DbInterface.CreatePlayerContext())
            {
                var guild = context.Guilds.FirstOrDefault(g => g.Name.ToLower() == name.ToLower());
                if (guild != null)
                {
                    return guild;
                }
            }

            return null;
        }

        /// <summary>
        /// Determines wether or not a guild already exists with a given name
        /// </summary>
        /// <param name="name">Guild name to check</param>
        /// <returns></returns>
        public static bool GuildExists(string name)
        {
            name = name.Trim().ToLower();
            using (var context = DbInterface.CreatePlayerContext())
            {
                return context.Guilds.Any(g => g.Name.ToLower() == name.ToLower());
            }
        }

        /// <summary>
        /// Completely removes a guild from the game.
        /// </summary>
        /// <param name="guild">The <see cref="Guild"/> to delete.</param>
        /// <param name="initiator">The player who initiated this change (null if done by the api or some other method).</param>
        public static void DeleteGuild(Guild guild, Player initiator = null)
        {
            // Remove our members cleanly before deleting this from our database.
            using (var context = DbInterface.CreatePlayerContext(readOnly: false))
            {
                foreach (var member in guild.Members)
                {
                    var plyr = Player.FindOnline(member.Key);
                    if (plyr != null)
                    {
                        plyr.Guild = null;
                        plyr.GuildRank = 0;

                        if (plyr.BankInterface != null && plyr.GuildBank)
                        {
                            plyr.BankInterface.Dispose();
                        }

                        PacketSender.SendGuild(plyr);

                        // Send our entity data to nearby players.
                        PacketSender.SendEntityDataToProximity(plyr);
                    }
                }

                foreach (var member in context.Players.Where(p => p.DbGuild.Id == guild.Id))
                {
                    member.DbGuild = null;
                    member.GuildRank = 0;
                }

                context.Guilds.Remove(guild);

                Guilds.TryRemove(guild.Id, out Guild removed);

                context.ChangeTracker.DetectChanges();
                context.SaveChanges();

                LogActivity(guild.Id, initiator, null, GuildActivityType.Disbanded);
            }
        }


        /// <summary>
        /// Transfers guild ownership to another member
        /// </summary>
        /// <param name="newOwnerId">The new owner.</param>
        /// <param name="initiator">The player who initiated this change (null if done by the api or some other method).</param>
        public bool TransferOwnership(Player newOwner, Player initiator = null)
        {
            if (newOwner != null)
            {
                using (var context = DbInterface.CreatePlayerContext(readOnly: false))
                {
                    //Find the current owner?
                    Player dbOwner = null;
                    var ownerPair = Members.Where(m => m.Value.Rank == 0).FirstOrDefault();

                    if (ownerPair.Key != Guid.Empty)
                    {
                        dbOwner = context.Players.Where(p => p.Id == ownerPair.Key && p.DbGuild.Id == this.Id && p.GuildRank == 0).FirstOrDefault();
                    }

                    var dbNewOwner = context.Players.Where(p => p.Id == newOwner.Id && p.DbGuild.Id == this.Id).FirstOrDefault();
                    if (dbNewOwner != null)
                    {
                        if (dbOwner != null)
                        {
                            dbOwner.GuildRank = 1;
                        }
                        dbNewOwner.GuildRank = 0;

                        context.ChangeTracker.DetectChanges();
                        context.SaveChanges();

                        var onlineOwner = Player.FindOnline(ownerPair.Key);
                        if (onlineOwner != null)
                        {
                            onlineOwner.GuildRank = 1;
                        }
                        newOwner.GuildRank = 0;


                        if (Members.TryGetValue(ownerPair.Key, out GuildMember ownerMem))
                        {
                            ownerMem.Rank = 1;
                        }

                        if (Members.TryGetValue(newOwner.Id, out GuildMember newOwnerMem))
                        {
                            newOwnerMem.Rank = 0;
                        }

                        // Send our new guild list to everyone that's online.
                        UpdateMemberList();

                        // Send our entity data to nearby players.
                        PacketSender.SendEntityDataToProximity(Player.FindOnline(newOwner.Id));

                        // Send our entity data to nearby players.
                        if (onlineOwner != null)
                        {
                            PacketSender.SendEntityDataToProximity(onlineOwner);
                        }

                        LogActivity(Id, newOwner, initiator, GuildActivityType.Transfer);

                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Removes all guilds in which only have 1 member that haven't been online in the configured number of days
        /// </summary>
        public static void WipeStaleGuilds()
        {
            if (Options.Instance.Guild.DeleteStaleGuildsAfterDays > 0)
            {
                using (var context = DbInterface.CreatePlayerContext(readOnly: false))
                {
                    foreach (var guild in context.Guilds)
                    {
                        var memberCount = context.Players.Where(p => p.DbGuild.Id == guild.Id).Count();
                        if (memberCount == 0)
                        {
                            context.Entry(guild).State = EntityState.Deleted;
                            LogActivity(guild.Id, null, null, GuildActivityType.Disbanded, "Stale");
                        }
                        else if (context.Players.Where(p => p.DbGuild.Id == guild.Id).Count() == 1)
                        {
                            var lastOnline = context.Players.FirstOrDefault(p => p.DbGuild.Id == guild.Id).LastOnline;
                            if (lastOnline != null && DateTime.UtcNow - lastOnline > TimeSpan.FromDays(Options.Instance.Guild.DeleteStaleGuildsAfterDays))
                            {
                                context.Entry(guild).State = EntityState.Deleted;
                                LogActivity(guild.Id, null, null, GuildActivityType.Disbanded, "Stale");
                            }
                        }
                    }
                    context.SaveChanges();
                }
            }
        }


        /// <summary>
        /// Getter for the guild lock
        /// </summary>
        public object Lock => mLock;


        public static void DetachGuildFromDbContext(PlayerContext context, Guild guild)
        {
            context.Entry(guild).State = EntityState.Detached;

            foreach (var slot in guild.Bank)
            {
                context.Entry(slot).State = EntityState.Detached;
            }

            foreach (var variable in guild.Variables)
            {
                context.Entry(variable).State = EntityState.Detached;
            }
        }

        /// <summary>
        /// Send bank slot update to all players in this guild who are online and in the bank
        /// </summary>
        /// <param name="slot">Slot index to send the update for</param>
        public void BankSlotUpdated(int slot)
        {
            foreach (var player in FindOnlineMembers())
            {
                if (player.BankInterface != null && player.GuildBank)
                {
                    player.BankInterface.SendBankUpdate(slot, false);
                }
            }
        }

        /// <summary>
        /// Updates the number of bank slots alotted to this guild for use, only expanding because we don't want to risk wiping items
        /// </summary>
        /// <param name="count"></param>
        public void ExpandBankSlots(int count)
        {
            if (count > BankSlotsCount)
            {
                lock (mLock)
                {
                    BankSlotsCount = count;
                    SlotHelper.ValidateSlots(Bank, BankSlotsCount);
                    DbInterface.Pool.QueueWorkItem(Save);
                }
            }
        }

        /// <summary>
        /// Renames this guild and then saves the new name to the db
        /// </summary>
        /// <param name="name"></param>
        /// <param name="initiator">The player who initiated this change (null if done by the api or some other method).</param>
        public bool Rename(string name, Player initiator = null)
        {
            if (GuildExists(name) || !FieldChecking.IsValidGuildName(name, Strings.Regex.guildname))
            {
                return false;
            }

            Name = name;
            foreach (var member in FindOnlineMembers())
            {
                PacketSender.SendEntityDataToProximity(member);
            }
            LogActivity(Id, initiator, null, GuildActivityType.Rename, name);
            Save();

            return true;
        }


        /// <summary>
        /// Returns a variable object given a guild variable id
        /// </summary>
        /// <param name="id">Variable id</param>
        /// <param name="createIfNull">Creates this variable for the guild if it hasn't been set yet</param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates a variable for this guild with a given id if it doesn't already exist
        /// </summary>
        /// <param name="id">Variablke id</param>
        /// <returns></returns>
        private Variable CreateVariable(Guid id)
        {
            if (GuildVariableBase.Get(id) == null)
            {
                return null;
            }

            var variable = new GuildVariable(id);
            Variables.Add(variable);

            return variable;
        }

        /// <summary>
        /// Gets the value of a guild variable given a variable id
        /// </summary>
        /// <param name="id">Variable id</param>
        /// <returns></returns>
        public GameObjects.Switches_and_Variables.VariableValue GetVariableValue(Guid id)
        {
            var v = GetVariable(id);
            if (v == null)
            {
                v = CreateVariable(id);
            }

            if (v == null)
            {
                return new GameObjects.Switches_and_Variables.VariableValue();
            }

            return v.Value;
        }


        /// <summary>
        /// Starts all common events with a specified trigger for all online guild members
        /// </summary>
        /// <param name="trigger">The common event trigger to run</param>
        /// <param name="command">The command which started this common event</param>
        /// <param name="param">Common event parameter</param>
        public void StartCommonEventsWithTriggerForAll(CommonEventTrigger trigger, string command, string param)
        {
            foreach (var plyr in FindOnlineMembers())
            {
                plyr.StartCommonEventsWithTrigger(trigger, command, param);
            }
        }


        /// <summary>
        /// Updates the db with this guild state & bank slots
        /// </summary>
        public void Save()
        {
            lock (mLock)
            {
                using (var context = DbInterface.CreatePlayerContext(readOnly: false))
                {
                    context.Update(this);
                    context.ChangeTracker.DetectChanges();
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Retrieves a list of guilds from the db as an IList
        /// </summary>
        public static IList<KeyValuePair<Guild, int>> List(string query, string sortBy, SortDirection sortDirection, int skip, int take, out int total)
        {
            try
            {
                using (var context = DbInterface.CreatePlayerContext())
                {
                    var compiledQuery = string.IsNullOrWhiteSpace(query) ? context.Guilds : context.Guilds.Where(p => EF.Functions.Like(p.Name, $"%{query}%"));

                    total = compiledQuery.Count();

                    switch (sortBy?.ToLower() ?? "")
                    {
                        case "members":
                            compiledQuery = sortDirection == SortDirection.Ascending ? compiledQuery.OrderBy(g => context.Players.Where(p => p.DbGuild.Id == g.Id).Count()) : compiledQuery.OrderByDescending(g => context.Players.Where(p => p.DbGuild.Id == g.Id).Count());
                            break;
                        case "foundingdate":
                            compiledQuery = sortDirection == SortDirection.Ascending ? compiledQuery.OrderBy(u => u.FoundingDate) : compiledQuery.OrderByDescending(u => u.FoundingDate);
                            break;
                        case "name":
                        default:
                            compiledQuery = sortDirection == SortDirection.Ascending ? compiledQuery.OrderBy(u => u.Name.ToUpper()) : compiledQuery.OrderByDescending(u => u.Name.ToUpper());
                            break;
                    }

                    return compiledQuery.Skip(skip).Take(take).Select(g => new KeyValuePair<Guild, int>(g, context.Players.Where(p => p.DbGuild.Id == g.Id).Count())).ToList();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                total = 0;
                return null;
            }
        }
    }
}
