using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Intersect.Logging;
using Intersect.Server.Core;
using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.General;
using Intersect.Server.Networking;
using Intersect.Server.Web.RestApi.Payloads;
using Intersect.Server.Database.PlayerData.Players;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

namespace Intersect.Server.Entities
{

    public partial class Player
    {

        #region Account

        [ForeignKey(nameof(User))]
        public Guid UserId { get; private set; }

        [JsonIgnore]
        public virtual User User { get; private set; }

        [NotMapped, JsonIgnore]
        public long SaveTimer { get; set; } = Globals.Timing.Milliseconds + Options.Instance.Processing.PlayerSaveInterval;

        #endregion

        #region Entity Framework

        #region Lookup

        public static Tuple<Client, Player> Fetch(LookupKey lookupKey)
        {
            if (!lookupKey.HasName && !lookupKey.HasId)
            {
                return new Tuple<Client, Player>(null, null);
            }

            // HasName checks if null or empty
            // ReSharper disable once AssignNullToNotNullAttribute
            return lookupKey.HasId ? Fetch(lookupKey.Id) : Fetch(lookupKey.Name);
        }

        public static Tuple<Client, Player> Fetch(string playerName)
        {
            var client = Globals.Clients.Find(queryClient => Entity.CompareName(playerName, queryClient?.Entity?.Name));

            return new Tuple<Client, Player>(client, client?.Entity ?? Player.Find(playerName));
        }

        public static Tuple<Client, Player> Fetch(Guid playerId)
        {
            var client = Globals.Clients.Find(queryClient => playerId == queryClient?.Entity?.Id);

            return new Tuple<Client, Player>(client, client?.Entity ?? Player.Find(playerId));
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
                using (var context = DbInterface.CreatePlayerContext())
                {
                    return Load(QueryPlayerById(context, playerId));
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }

        public static Player Find(string playerName)
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
                using (var context = DbInterface.CreatePlayerContext())
                {
                    return Load(QueryPlayerByName(context, playerName));
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
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
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }

        #endregion

        #region Loading

        public static Player Load(Guid playerId)
        {
            var player = Find(playerId);

            return Load(player);
        }

        public static Player Load(string playerName)
        {
            var player = Find(playerName);

            return Load(player);
        }

        public static Player Load(Player player)
        {
            if (player == null)
            {
                return null;
            }

            // ReSharper disable once InvertIf
            if (!player.ValidateLists())
            {
                player.Bank = player.Bank.OrderBy(bankSlot => bankSlot?.Slot).ToList();
                player.Items = player.Items.OrderBy(inventorySlot => inventorySlot?.Slot).ToList();
                player.Hotbar = player.Hotbar.OrderBy(hotbarSlot => hotbarSlot?.Slot).ToList();
                player.Spells = player.Spells.OrderBy(spellSlot => spellSlot?.Slot).ToList();
            }

            return player;
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
                Log.Error(ex, $"Failed to load friends for {Name}.");
                //ServerContext.DispatchUnhandledException(new Exception("Failed to save user, shutting down to prevent rollbacks!"), true);
            }
        }

        public void AddFriend(Player friend)
        {
            if (friend == null || friend == this)
            {
                return;
            }

            if (CachedFriends.ContainsKey(friend.Id))
            {
                return;
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
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to add friend " + friend.Name + " to " + Name + "'s friends list.");
                //ServerContext.DispatchUnhandledException(new Exception("Failed to save user, shutting down to prevent rollbacks!"), true);
            }
        }

        public static void RemoveFriendship(Guid id, Guid otherId)
        {
            if (id == Guid.Empty || otherId == Guid.Empty)
            {
                return;
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
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to remove friendship between {id} and {otherId}.");
                //ServerContext.DispatchUnhandledException(new Exception("Failed to save user, shutting down to prevent rollbacks!"), true);
            }
        }
        #endregion

        #region "Guilds"
        public void LoadGuild()
        {
            using (var context = DbInterface.CreatePlayerContext())
            {
                var guildId = context.Players.Where(p => p.Id == Id && p.DbGuild.Id != null && p.DbGuild.Id != Guid.Empty).Select(p => p.DbGuild.Id).FirstOrDefault();
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
            catch (Exception ex)
            {
                Log.Error(ex);
                return 0;
            }
        }

        public static IList<Player> List(string query, string sortBy, SortDirection sortDirection, int skip, int take, out int total)
        {
            try
            {
                using (var context = DbInterface.CreatePlayerContext())
                {
                    var compiledQuery = string.IsNullOrWhiteSpace(query) ? context.Players : context.Players.Where(p => EF.Functions.Like(p.Name, $"%{query}%"));

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
                        case "name":
                        default:
                            compiledQuery = sortDirection == SortDirection.Ascending ? compiledQuery.OrderBy(u => u.Name.ToUpper()) : compiledQuery.OrderByDescending(u => u.Name.ToUpper());
                            break;
                    }
                    
                    return compiledQuery.Skip(skip).Take(take).ToList();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
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
            catch (Exception ex)
            {
                Log.Error(ex);
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
                    .Include(p => p.Hotbar)
                    .Include(p => p.Quests)
                    .Include(p => p.Variables)
                    .Include(p => p.Items)
                    .Include(p => p.Spells)
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
                    .Include(p => p.Hotbar)
                    .Include(p => p.Quests)
                    .Include(p => p.Variables)
                    .Include(p => p.Items)
                    .Include(p => p.Spells)
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
                    .FirstOrDefault()
            ) ??
            throw new InvalidOperationException();

        private static readonly Func<PlayerContext, string, bool> AnyPlayerByName =
            EF.CompileQuery(
                (PlayerContext context, string name) => context.Players.Where(p => p.Name == name).Any());

        #endregion

        #endregion

    }

}
