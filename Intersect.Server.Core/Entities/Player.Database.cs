using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Intersect.Core;
using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Networking;
using Intersect.Server.Database.PlayerData.Players;
using Intersect.Utilities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Intersect.Server.Collections.Indexing;
using Intersect.Server.Collections.Sorting;
using Microsoft.Extensions.Logging;

namespace Intersect.Server.Entities;


public partial class Player
{
    private volatile bool _saving;
    private readonly object _savingLock = new();

    #region Account

    [ForeignKey(nameof(User))]
    public Guid UserId { get; private set; }

    [JsonIgnore]
    public virtual User User { get; private set; }

    [NotMapped, JsonIgnore]
    public long SaveTimer { get; set; } = Timing.Global.Milliseconds + Options.Instance.Processing.PlayerSaveInterval;

    [NotMapped, JsonIgnore]
    public bool IsSaving
    {
        get
        {
            lock (_pendingLogoutLock)
            {
                if (_pendingLogouts.Contains(Id))
                {
                    return true;
                }
            }

            lock (_savingLock)
            {
                return _saving;
            }
        }
    }

    #endregion

    #region Entity Framework

    #region Lookup

    public static bool TryFetch(
        LookupKey lookupKey,
        [NotNullWhen(true)] out Player? player,
        bool loadRelationships = false,
        bool loadBags = false
    )
        => TryFetch(lookupKey, out _, out player, loadRelationships, loadBags);

    public static bool TryFetch(
        LookupKey lookupKey,
        out Client? client,
        [NotNullWhen(true)] out Player? player,
        bool loadRelationships = false,
        bool loadBags = false
    )
    {
        if (lookupKey.IsInvalid)
        {
            client = default;
            player = default;
            return false;
        }

        if (lookupKey.IsId)
        {
            client = Client.Instances.Find(queryClient => lookupKey.Id == queryClient?.Entity?.Id);
            player = client?.Entity ?? Find(lookupKey.Id);
            return player != default;
        }

        client = Client.Instances.Find(queryClient => CompareName(lookupKey.Name, queryClient?.Entity?.Name));
        player = client?.Entity ?? Find(lookupKey.Name, loadRelationships: loadRelationships, loadBags: loadBags);
        return player != default;
    }

    public static Player Find(Guid playerId)
    {
        if (playerId == Guid.Empty)
        {
            return null;
        }

        var player = Player.FindOnline(playerId);
        if (player != null)
        {
            return player;
        }

        try
        {
            using var context = DbInterface.CreatePlayerContext();
            player = QueryPlayerById(context, playerId);
            _ = Validate(player);
            return player;
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogError(exception, "Error while finding player {Id}", playerId);
            return null;
        }
    }

    public static Player Find(string playerName, bool loadRelationships = false, bool loadBags = false)
    {
        if (string.IsNullOrWhiteSpace(playerName))
        {
            return null;
        }

        var player = Player.FindOnline(playerName);
        if (player != null)
        {
            return player;
        }

        try
        {
            using var context = DbInterface.CreatePlayerContext();
            player = QueryPlayerByName(context, playerName);
            if (loadRelationships)
            {
                player.LoadRelationships(context, loadBags);
            }
            _ = Validate(player);
            return player;
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogError(
                exception,
                "Error while finding player '{PlayerName}'",
                playerName
            );
            return null;
        }
    }

    public static bool PlayerExists(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        var player = FindOnline(name);
        if (player != null)
        {
            return true;
        }

        try
        {
            using (var context = DbInterface.CreatePlayerContext())
            {
                return AnyPlayerByName(context, name);
            }
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogError(
                exception,
                "Error while checking if player '{PlayerName}' exists",
                name
            );
            return false;
        }
    }

    #endregion

    #region Loading

    public bool LoadRelationships(PlayerContext playerContext, bool loadBags = false)
    {
        var entityEntry = playerContext.Players.Attach(this);
        entityEntry.Collection(p => p.Bank).Load();
        entityEntry.Collection(p => p.Hotbar).Load();
        entityEntry.Collection(p => p.Items).Load();
        entityEntry.Collection(p => p.Quests).Load();
        entityEntry.Collection(p => p.Spells).Load();
        entityEntry.Collection(p => p.Variables).Load();

        if (loadBags)
        {
            foreach (var item in Items)
            {
                if (item.BagId == default)
                {
                    continue;
                }

                var navigationEntry = playerContext.Entry(item).Navigation(nameof(item.Bag));
                if (navigationEntry.IsLoaded)
                {
                    continue;
                }

                navigationEntry.Load();
                if (item.Bag != default)
                {
                    item.Bag.ValidateSlots();
                    playerContext.Bags.Entry(item.Bag).Collection(b => b.Slots).Load();
                }
            }
        }

        return Validate(this, playerContext);
    }

    public static Player Load(Guid playerId)
    {
        var player = Find(playerId);
        _ = Validate(player);
        return player;
    }

    public static Player Load(string playerName)
    {
        var player = Find(playerName);
        _ = Validate(player);
        return player;
    }

    public static bool Validate(Player? player, PlayerContext? playerContext = default)
    {
        if (player == null)
        {
            return false;
        }

        if (player.ValidateLists(playerContext))
        {
            return false;
        }

        // player.Bank = player.Bank.OrderBy(bankSlot => bankSlot?.Slot)
        // player.Items = player.Items.OrderBy(inventorySlot => inventorySlot?.Slot).ToList();
        // player.Hotbar = player.Hotbar.OrderBy(hotbarSlot => hotbarSlot?.Slot).ToList();
        // player.Spells = player.Spells.OrderBy(spellSlot => spellSlot?.Slot).ToList();

        return true;
    }

    #endregion

    #region Friends

    public void LoadFriends()
    {
        try
        {
            using (var context = DbInterface.CreatePlayerContext())
            {
                CachedFriends = context.Player_Friends.Where(f => f.Owner.Id == Id).Select(t => new { t.Target.Id, t.Target.Name }).ToDictionary(t => t.Id, t => t.Name);
            }
        }
        catch (Exception ex)
        {
            ApplicationContext.Context.Value?.Logger.LogError(ex, $"Failed to load friends for {Name}.");
            //ServerContext.DispatchUnhandledException(new Exception("Failed to save user, shutting down to prevent rollbacks!"), true);
        }
    }

    public bool TryAddFriend(Player friend)
    {
        if (friend == this)
        {
            return true;
        }

        if (CachedFriends.ContainsKey(friend.Id))
        {
            return true;
        }

        //No passing in custom contexts here.. they may already have this user in the change tracker and things just get weird.
        //The cost of making a new context is almost nil.
        try
        {
            CachedFriends.Add(friend.Id, friend.Name);

            using (var context = DbInterface.CreatePlayerContext(readOnly: false))
            {
                var friendship = new Database.PlayerData.Players.Friend(this, friend);
                context.Entry(friendship).State = EntityState.Added;
                if (context.SaveChanges() >= 0)
                {
                    return true;
                }
            }

            Client.LogAndDisconnect(Id, nameof(TryAddFriend));
            return false;
        }
        catch (Exception ex)
        {
            ApplicationContext.Context.Value?.Logger.LogError(ex, "Failed to add friend " + friend.Name + " to " + Name + "'s friends list.");
            return false;
        }
    }

    public static bool TryRemoveFriendship(Guid id, Guid otherId)
    {
        if (id == Guid.Empty || otherId == Guid.Empty)
        {
            return true;
        }

        //No passing in custom contexts here.. they may already have this user in the change tracker and things just get weird.
        //The cost of making a new context is almost nil.
        try
        {
            using (var context = DbInterface.CreatePlayerContext(readOnly: false))
            {
                var friendship = context.Player_Friends.Where(f => f.Owner.Id == id && f.Target.Id == otherId).FirstOrDefault();
                if (friendship != null)
                {
                    context.Entry(friendship).State = EntityState.Deleted;
                }

                var otherFriendship = context.Player_Friends.Where(f => f.Owner.Id == otherId && f.Target.Id == id).FirstOrDefault();
                if (otherFriendship != null)
                {
                    context.Entry(otherFriendship).State = EntityState.Deleted;
                }

                return context.SaveChanges() >= 0;
            }
        }
        catch (Exception ex)
        {
            ApplicationContext.Context.Value?.Logger.LogError(ex, $"Failed to remove friendship between {id} and {otherId}.");
            return false;
        }
    }
    #endregion

    #region "Guilds"
    public void LoadGuild()
    {
        using (var context = DbInterface.CreatePlayerContext())
        {
            var guildId = context.Players.Where(p => p.Id == Id && p.Guild != null && p.Guild.Id != Guid.Empty)
                .Select(p => p.Guild.Id).FirstOrDefault();
            if (guildId != default)
            {
                Guild = Guild.LoadGuild(guildId);
            }
        }

        if (GuildRank > Options.Instance.Guild.Ranks.Length - 1)
        {
            GuildRank = Options.Instance.Guild.Ranks.Length - 1;
        }
    }
    #endregion

    #region Saving

    public void Save(PlayerContext? playerContext = null)
    {
        if (User is {} user)
        {
            user.Save(playerContext: playerContext);
            return;
        }

        PlayerContext? createdPlayerContext = null;

        try
        {
            if (playerContext == null || playerContext.IsReadOnly)
            {
                playerContext = createdPlayerContext = DbInterface.CreatePlayerContext(readOnly: false);
            }

            playerContext.Update(this);
            playerContext.ChangeTracker.DetectChanges();
            playerContext.SaveChanges();
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogError(
                exception,
                $"Error occurred while saving player {Id} ({nameof(playerContext)}={(createdPlayerContext == null ? "not null" : "null")}"
            );
        }
        finally
        {
            createdPlayerContext?.Dispose();
        }
    }

    #endregion Saving

    #region Listing

    public static int Count()
    {
        try
        {
            using (var context = DbInterface.CreatePlayerContext())
            {
                return context.Players.Count();
            }
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogError(exception, "Error counting players");
            return 0;
        }
    }

    public static IList<Player> List(string query, string sortBy, SortDirection sortDirection, int skip, int take, out int total, Guid guildId = default(Guid))
    {
        try
        {
            using (var context = DbInterface.CreatePlayerContext())
            {
                var compiledQuery = string.IsNullOrWhiteSpace(query) ? context.Players : context.Players.Where(p => EF.Functions.Like(p.Name, $"%{query}%"));

                if (guildId != Guid.Empty)
                {
                    compiledQuery = compiledQuery.Where(p => p.Guild.Id == guildId);
                }

                total = compiledQuery.Count();

                switch (sortBy?.ToLower() ?? "")
                {
                    case "level":
                        compiledQuery = sortDirection == SortDirection.Ascending ? compiledQuery.OrderBy(u => u.Level).ThenBy(u => u.Exp) : compiledQuery.OrderByDescending(u => u.Level).ThenByDescending(u => u.Exp);
                        break;
                    case "creationdate":
                        compiledQuery = sortDirection == SortDirection.Ascending ? compiledQuery.OrderBy(u => u.CreationDate) : compiledQuery.OrderByDescending(u => u.CreationDate);
                        break;
                    case "playtime":
                        compiledQuery = sortDirection == SortDirection.Ascending ? compiledQuery.OrderBy(u => u.PlayTimeSeconds) : compiledQuery.OrderByDescending(u => u.PlayTimeSeconds);
                        break;
                    case "guildrank":
                        compiledQuery = sortDirection == SortDirection.Ascending ? compiledQuery.OrderBy(u => u.GuildRank) : compiledQuery.OrderByDescending(u => u.GuildRank);
                        break;
                    case "name":
                    default:
                        compiledQuery = sortDirection == SortDirection.Ascending ? compiledQuery.OrderBy(u => u.Name.ToUpper()) : compiledQuery.OrderByDescending(u => u.Name.ToUpper());
                        break;
                }

                return compiledQuery.Skip(skip).Take(take).ToList();
            }
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogError(exception, "Error listing players");
            total = 0;
            return null;
        }
    }

    public static IEnumerable<Player> Rank(
        int page,
        int count,
        SortDirection sortDirection
    )
    {
        try
        {
            using (var context = DbInterface.CreatePlayerContext())
            {
                var results = sortDirection == SortDirection.Ascending
                    ? QueryPlayersWithRankAscending(context, page * count, count)
                    : QueryPlayersWithRank(context, page * count, count);

                return results?.ToList();
            }
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogError(exception, "Error ranking players");
            return null;
        }
    }

    #endregion

    #region Compiled Queries

    private static readonly Func<PlayerContext, int, int, IEnumerable<Player>> QueryPlayersWithRank =
        EF.CompileQuery(
            (PlayerContext context, int offset, int count) => context.Players
                .OrderByDescending(entity => EF.Property<dynamic>(entity, "Level"))
                .ThenByDescending(entity => EF.Property<dynamic>(entity, "Exp"))
                .Skip(offset)
                .Take(count)
                .Include(p => p.Bank)
                .Include(p => p.Guild)
                .Include(p => p.Hotbar)
                .Include(p => p.Items)
                .Include(p => p.Quests)
                .Include(p => p.Spells)
                .Include(p => p.Variables)
                .AsSplitQuery()
        ) ??
        throw new InvalidOperationException();

    private static readonly Func<PlayerContext, int, int, IEnumerable<Player>> QueryPlayersWithRankAscending =
        EF.CompileQuery(
            (PlayerContext context, int offset, int count) => context.Players
                .OrderBy(entity => EF.Property<dynamic>(entity, "Level"))
                .ThenBy(entity => EF.Property<dynamic>(entity, "Exp"))
                .Skip(offset)
                .Take(count)
                .Include(p => p.Bank)
                .Include(p => p.Guild)
                .Include(p => p.Hotbar)
                .Include(p => p.Items)
                .Include(p => p.Quests)
                .Include(p => p.Spells)
                .Include(p => p.Variables)
                .AsSplitQuery()
        ) ??
        throw new InvalidOperationException();

    private static readonly Func<PlayerContext, Guid, Player> QueryPlayerById =
        EF.CompileQuery(
            (PlayerContext context, Guid id) => context.Players.Where(p => p.Id == id)
                .Include(p => p.Bank)
                .Include(p => p.Hotbar)
                .Include(p => p.Quests)
                .Include(p => p.Variables)
                .Include(p => p.Items)
                .Include(p => p.Spells)
                .AsSplitQuery()
                .FirstOrDefault()
        ) ??
        throw new InvalidOperationException();

    private static readonly Func<PlayerContext, string, Player> QueryPlayerByName =
        EF.CompileQuery(
            (PlayerContext context, string name) => context.Players
            .Where(p => p.Name == name)
                .Include(p => p.Bank)
                .Include(p => p.Hotbar)
                .Include(p => p.Quests)
                .Include(p => p.Variables)
                .Include(p => p.Items)
                .Include(p => p.Spells)
                .AsSplitQuery()
                .FirstOrDefault()
        ) ??
        throw new InvalidOperationException();

    private static readonly Func<PlayerContext, string, bool> AnyPlayerByName =
        EF.CompileQuery(
            (PlayerContext context, string name) => context.Players.Where(p => p.Name == name).Any());

    internal static readonly Func<PlayerContext, Guid, Player> QueryPlayerByGuidWithLoading =
        EF.CompileQuery(
            // ReSharper disable once SpecifyStringComparison
            (PlayerContext context, Guid playerId) => context.Players.Where(p => p.Id == playerId)
                .Include(c => c.Bank)
                .Include(c => c.Hotbar)
                .Include(c => c.Items)
                .Include(c => c.Quests)
                .Include(c => c.Spells)
                .Include(c => c.Variables)
                .AsSplitQuery()
                .FirstOrDefault()
        ) ??
        throw new InvalidOperationException();

    #endregion

    #endregion

}
