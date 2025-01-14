using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

using Intersect.Enums;
using Intersect.Server.Entities;
using Intersect.Server.Networking;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Intersect.Collections.Slotting;
using Intersect.Extensions;
using Microsoft.EntityFrameworkCore;
using Intersect.Network.Packets.Server;
using Intersect.GameObjects;
using Intersect.GameObjects.Maps;
using Intersect.Logging;
using Intersect.Utilities;
using Intersect.Server.Localization;
using Intersect.Server.Web.RestApi.Payloads;
using static Intersect.Server.Database.Logging.Entities.GuildHistory;

namespace Intersect.Server.Database.PlayerData.Players;

/// <summary>
/// A class containing the definition of each guild, alongside the methods to use them.
/// </summary>
public partial class Guild
{
    public static readonly ConcurrentDictionary<Guid, Guild> Guilds = new();

    // Entity Framework Garbage.
    public Guild()
    {
    }

    /// <summary>
    /// The database Id of the guild.
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; private set; } = Guid.NewGuid();

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
    public virtual SlotList<GuildBankSlot> Bank { get; set; } = new SlotList<GuildBankSlot>(
        Options.Instance.Guild.InitialBankSlots,
        GuildBankSlot.Create
    );

    /// <summary>
    /// Sets the number of bank slots alotted to this guild. Banks lots can only expand.
    /// </summary>
    public int BankSlotsCount { get; set; } = Options.Instance.Guild.InitialBankSlots;

    /// <summary>
    /// The guild's instance id. This is a unique identifier generated at guild creation time that
    /// we can use to reference this guild alone when requesting a warp to a "Guild" instance, ensuring
    /// that players in the same guild will be warped to the same instance.
    /// </summary>
    public Guid GuildInstanceId { get; private set; }

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

    [JsonIgnore]
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
    public static Guild? CreateGuild(Player creator, string name)
    {
        name = name.Trim();

        if (creator == null || !FieldChecking.IsValidGuildName(name, Strings.Regex.GuildName))
        {
            return null;
        }

        using var context = DbInterface.CreatePlayerContext(readOnly: false);
        var guild = new Guild
        {
            Name = name,
            FoundingDate = DateTime.UtcNow,
            GuildInstanceId = Guid.NewGuid(),
        };

        SlotHelper.ValidateSlotList(guild.Bank, Options.Instance.Guild.InitialBankSlots);

        creator.Guild = guild;
        creator.GuildRank = 0;
        creator.GuildJoinDate = DateTime.UtcNow;

        context.ChangeTracker.DetectChanges();
        context.SaveChanges();

        LogActivity(guild.Id, creator, null, GuildActivityType.Created, name);

        Guilds.AddOrUpdate(guild.Id, guild, (_, _) => guild);

        var member = new GuildMember(creator.Id, creator.Name, creator.GuildRank, creator.Level, creator.ClassName, creator.MapName);
        guild.Members.AddOrUpdate(creator.Id, member, (_, _) => member);

        // Send our entity data to nearby players.
        PacketSender.SendEntityDataToProximity(Player.FindOnline(creator.Id));

        return guild;
    }

    public static bool TryGet(Guid guildId, [NotNullWhen(true)] out Guild? guild)
    {
        guild = LoadGuild(guildId);
        return guild != default;
    }

    /// <summary>
    /// Loads a guild and it's members from the database
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static Guild LoadGuild(Guid id)
    {
        if (Guilds.TryGetValue(id, out Guild found))
        {
            return found;
        }

        using var context = DbInterface.CreatePlayerContext();
        var guild = context.Guilds.Where(g => g.Id == id)
            .Include(g => g.Bank)
            .Include(g => g.Variables)
            .AsSplitQuery()
            .FirstOrDefault();
        if (guild == default)
        {
            return default;
        }

        // Load Members
        var members = context.Players.Where(p => p.Guild.Id == id).ToDictionary(
            player => player.Id,
            player => new Tuple<string, int, int, Guid, Guid>(
                player.Name,
                player.GuildRank,
                player.Level,
                player.ClassId,
                player.MapId
            )
        );

        foreach (var (memberId, membership) in members)
        {
            var (memberName, memberRank, memberLevel, memberClassId, memberMapId) = membership;
            var className = ClassBase.GetName(memberClassId);
            var mapName = MapBase.GetName(memberMapId);
            var guildMember = new GuildMember(
                memberId,
                memberName,
                memberRank,
                memberLevel,
                className,
                mapName
            );
            guild.Members.AddOrUpdate(memberId, guildMember, (_, _) => guildMember);
        }

        SlotHelper.ValidateSlotList(guild.Bank, guild.BankSlotsCount);

        Guilds.AddOrUpdate(id, guild, (_, _) => guild);

        return guild;

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
            var player = Player.FindOnline(member.Key);
            if (player == null)
            {
                continue;
            }

            //Update Cached Member List Values
            member.Value.Name = player.Name;
            member.Value.ClassName = player.ClassName;
            member.Value.Level = player.Level;
            member.Value.MapName = player.MapName;
            member.Value.Rank = player.GuildRank;

            online.Add(player);
        }

        return online;
    }

    /// <summary>
    /// Joins a player into the guild.
    /// </summary>
    /// <param name="player">The player to join into the guild.</param>
    /// <param name="rank">Integer index of the rank to give this new member.</param>
    /// <param name="initiator">The player who initiated this change (null if done by the api or some other method).</param>
    public bool TryAddMember(Player player, int rank, Player? initiator = null)
    {
        if (player == null)
        {
            return false;
        }

        if (Members.ContainsKey(player.Id))
        {
            return false;
        }

        try
        {
            // Save the guild before adding a new player
            Save();
        }
        catch (Exception exception)
        {
            Log.Error(exception, $"Failed to save guild {Id} before adding player {player.Id}");
            return false;
        }

        try
        {
            // Save the player before adding them to the guild
            player.User.Save();
        }
        catch (Exception exception)
        {
            Log.Error(exception, $"Failed to save player {player.Id} before adding them to guild {Id}");
            return false;
        }

        using var context = DbInterface.CreatePlayerContext(readOnly: false);
        player.Guild = this;
        player.GuildRank = rank;
        player.GuildJoinDate = DateTime.UtcNow;
        context.Update(player);
        context.ChangeTracker.DetectChanges();
        context.SaveChanges();

        LogActivity(Id, player, initiator, GuildActivityType.Joined);

        var member = new GuildMember(
            player.Id,
            player.Name,
            player.GuildRank,
            player.Level,
            player.ClassName,
            player.MapName
        );
        Members.AddOrUpdate(player.Id, member, (_, _) => member);

        // Send our new guild list to everyone that's online.
        UpdateMemberList();

        // Send our entity data to nearby players.
        PacketSender.SendEntityDataToProximity(Player.FindOnline(player.Id));

        return true;
    }

    /// <summary>
    /// Removes a player from the guild.
    /// </summary>
    /// <param name="targetId">The ID of the player to remove from the guild.</param>
    /// <param name="targetPlayer">The player to remove from the guild.</param>
    /// <param name="initiator">The player who initiated this change (null if done by the api or some other method).</param>
    /// <param name="guildActivityType">The reason the member is being removed.</param>
    public bool TryRemoveMember(Guid targetId, Player targetPlayer, Player? initiator = null, GuildActivityType guildActivityType = GuildActivityType.Left)
    {
        if (targetId == default)
        {
            return false;
        }

        targetPlayer ??= Player.Find(targetId);

        if (targetPlayer == null)
        {
            Log.Warn($"Failed to remove non-existent player {targetId} from the guild {Id}");
            return false;
        }

        using var context = DbInterface.CreatePlayerContext(readOnly: false);
        context.Attach(targetPlayer);
        targetPlayer.Guild = default;
        targetPlayer.GuildRank = 0;
        targetPlayer.GuildJoinDate = default;
        targetPlayer.PendingGuildInvite = default;
        context.ChangeTracker.DetectChanges();
        context.SaveChanges();

        LogActivity(
            Id,
            targetPlayer.UserId,
            targetId,
            targetPlayer.Client?.Ip ?? string.Empty,
            initiator,
            guildActivityType
        );

        if (targetPlayer.BankInterface != null && targetPlayer.GuildBank)
        {
            targetPlayer.BankInterface.Dispose();
        }

        _ = Members.TryRemove(targetId, out GuildMember _);

        // Send our new guild list to everyone that's online.
        UpdateMemberList();

        // Send our entity data to nearby players.
        PacketSender.SendEntityDataToProximity(Player.FindOnline(targetId));

        return true;
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

    internal void NotifyPlayerDisposed(Player player)
    {
        var onlineCount = FindOnlineMembers().Count;
        if (onlineCount == 0)
        {
            Save();

            if (Guilds.TryRemove(Id, out _))
            {
                Log.Info($"[Guild][{Id}] Removed self from {nameof(Guilds)} after player {player.Id} logged out");
            }
            else
            {
                Log.Warn($"[Guild][{Id}] Failed to remove self from {nameof(Guilds)} after player {player.Id} logged out");
            }
        }
        else
        {
            Log.Info($"[Guild][{Id}] Player {player.Id} logged out but there are {onlineCount} members still online");
        }
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
        LastUpdateTime = Timing.Global.Milliseconds;
    }

    /// <summary>
    /// Sets a player's guild rank.
    /// </summary>
    /// <param name="id">The player to set the rank for.</param>
    /// <param name="rank">The integer index of the rank to assign.</param>
    /// <param name="initiator">The player who initiated this change (null if done by the api or some other method).</param>
    public void SetPlayerRank(Guid id, int rank, Player initiator = null) => SetPlayerRank(id, default, rank, initiator);

    /// <summary>
    /// Sets a player's guild rank.
    /// </summary>
    /// <param name="targetId">The player to set the rank for.</param>
    /// <param name="targetPlayer">The player to set the rank for.</param>
    /// <param name="rank">The integer index of the rank to assign.</param>
    /// <param name="initiator">The player who initiated this change (null if done by the api or some other method).</param>
    public void SetPlayerRank(Guid targetId, Player targetPlayer, int rank, Player initiator = null)
    {
        targetPlayer ??= Player.Find(targetId);

        if (targetPlayer == null)
        {
            Log.Warn($"Unable to set guild rank to {rank} for non-existent player {targetId} in guild {Id}");
            return;
        }

        using var context = DbInterface.CreatePlayerContext(readOnly: false);
        var originalRank = targetPlayer.GuildRank;
        targetPlayer.GuildRank = rank;
        context.Update(targetPlayer);
        context.ChangeTracker.DetectChanges();
        context.SaveChanges();

        LogActivity(
            Id,
            targetPlayer.UserId,
            targetId,
            targetPlayer.Client?.Ip ?? string.Empty,
            initiator,
            originalRank < rank ? GuildActivityType.Demoted : GuildActivityType.Promoted,
            rank.ToString()
        );

        if (Members.TryGetValue(targetId, out GuildMember val))
        {
            val.Rank = rank;
        }

        // Send our new guild list to everyone that's online.
        UpdateMemberList();

        // Send our entity data to nearby players.
        PacketSender.SendEntityDataToProximity(Player.FindOnline(targetId));
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
        foreach (var member in guild.Members)
        {
            var player = Player.FindOnline(member.Key);
            if (player == null)
            {
                continue;
            }

            player.Guild = null;
            player.GuildRank = 0;

            if (player.BankInterface != null && player.GuildBank)
            {
                player.BankInterface.Dispose();
            }

            PacketSender.SendGuild(player);

            // Send our entity data to nearby players.
            PacketSender.SendEntityDataToProximity(player);
        }

        // Remove our members cleanly before deleting this from our database.
        using var context = DbInterface.CreatePlayerContext(readOnly: false);
        context.Guilds.Remove(guild);
        context.ChangeTracker.DetectChanges();
        context.SaveChanges();

        LogActivity(guild.Id, initiator, null, GuildActivityType.Disbanded);

        _ = Guilds.TryRemove(guild.Id, out _);
    }


    /// <summary>
    /// Transfers guild ownership to another member
    /// </summary>
    /// <param name="newOwner">The new owner.</param>
    /// <param name="initiator">The player who initiated this change (null if done by the api or some other method).</param>
    public bool TransferOwnership(Player newOwner, Player initiator = null)
    {
        if (newOwner == null)
        {
            return false;
        }

        if (newOwner.GuildId != Id && newOwner.Guild != this)
        {
            return false;
        }

        // Find the current owner?
        var oldOwnerId = Members.FirstOrDefault(m => m.Value.Rank == 0).Key;
        var oldOwner = Player.Find(oldOwnerId);

        using var context = DbInterface.CreatePlayerContext(readOnly: false);

        if (oldOwner != null)
        {
            oldOwner.GuildRank = 1;
            context.Update(oldOwner);
        }

        newOwner.GuildRank = 0;
        context.Update(newOwner);

        context.ChangeTracker.DetectChanges();
        context.SaveChanges();

        LogActivity(Id, newOwner, initiator, GuildActivityType.Transfer);

        if (Members.TryGetValue(oldOwnerId, out GuildMember oldOwnerMembership))
        {
            oldOwnerMembership.Rank = 1;
        }

        if (Members.TryGetValue(newOwner.Id, out GuildMember newOwnerMembership))
        {
            newOwnerMembership.Rank = 0;
        }

        // Send our new guild list to everyone that's online.
        UpdateMemberList();

        if (newOwner.Online)
        {
            // Send our entity data to nearby players.
            PacketSender.SendEntityDataToProximity(newOwner);
        }

        if (oldOwner?.Online ?? false)
        {
            // Send our entity data to nearby players.
            PacketSender.SendEntityDataToProximity(oldOwner);
        }

        return true;

    }

    /// <summary>
    /// Removes all guilds in which only have 1 member that haven't been online in the configured number of days
    /// </summary>
    public static void WipeStaleGuilds()
    {
        var guildDeleteStaleGuildsAfterDays = Options.Instance.Guild.DeleteStaleGuildsAfterDays;
        if (guildDeleteStaleGuildsAfterDays <= 0)
        {
            return;
        }

        var now = DateTime.UtcNow;
        var staleTimeSpan = TimeSpan.FromDays(guildDeleteStaleGuildsAfterDays);

        using var context = DbInterface.CreatePlayerContext(readOnly: false);
        foreach (var guild in context.Guilds)
        {
            var memberCount = context.Players.Count(p => p.Guild.Id == guild.Id);
            bool stale = false;
            switch (memberCount)
            {
                case 0:
                    stale = true;
                    break;
                case 1:
                {
                    var player = context.Players.FirstOrDefault(p => p.Guild.Id == guild.Id);
                    stale = player?.LastOnline is not { } lastOnline || staleTimeSpan < (now - lastOnline);
                    break;
                }
            }

            if (!stale)
            {
                continue;
            }

            context.Entry(guild).State = EntityState.Deleted;
            LogActivity(guild.Id, null, null, GuildActivityType.Disbanded, "Stale");
        }

        context.SaveChanges();
    }


    /// <summary>
    /// Getter for the guild lock
    /// </summary>
    public object Lock => mLock;

    private static void MarkUnchangedForMemberAdd(PlayerContext context, Guild guild)
    {
        context.Entry(guild).State = EntityState.Unchanged;

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
        if (BankSlotsCount >= count || count > Options.Instance.Bank.MaxSlots)
        {
            return;
        }

        lock (mLock)
        {
            BankSlotsCount = count;
            SlotHelper.ValidateSlotList(Bank, BankSlotsCount);
            DbInterface.Pool.QueueWorkItem(Save);
        }
    }

    /// <summary>
    /// Renames this guild and then saves the new name to the db
    /// </summary>
    /// <param name="name"></param>
    /// <param name="initiator">The player who initiated this change (null if done by the api or some other method).</param>
    public bool Rename(string name, Player initiator = null)
    {
        if (GuildExists(name) || !FieldChecking.IsValidGuildName(name, Strings.Regex.GuildName))
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
            using var context = DbInterface.CreatePlayerContext();
            var compiledQuery = string.IsNullOrWhiteSpace(query) ? context.Guilds : context.Guilds.Where(p => EF.Functions.Like(p.Name, $"%{query}%"));

            total = compiledQuery.Count();

            switch (sortBy?.ToLower())
            {
                case "members":
                {
                    compiledQuery = sortDirection == SortDirection.Ascending
                        ? compiledQuery.OrderBy(g => context.Players.Count(p => p.Guild.Id == g.Id))
                        : compiledQuery.OrderByDescending(g => context.Players.Count(p => p.Guild.Id == g.Id));
                    break;
                }
                case "foundingdate":
                    compiledQuery = sortDirection == SortDirection.Ascending
                        ? compiledQuery.OrderBy(g => g.FoundingDate)
                        : compiledQuery.OrderByDescending(u => u.FoundingDate);
                    break;
                case "name":
                default:
                    compiledQuery = sortDirection == SortDirection.Ascending
                        ? compiledQuery.OrderBy(g => g.Name.ToUpper())
                        : compiledQuery.OrderByDescending(u => u.Name.ToUpper());
                    break;
            }

            return compiledQuery.Skip(skip).Take(take).Select(
                g => new KeyValuePair<Guild, int>(g, context.Players.Count(p => p.Guild.Id == g.Id))
            ).ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            total = 0;
            return null;
        }
    }
}
