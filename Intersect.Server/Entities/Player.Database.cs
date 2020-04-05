using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.General;
using Intersect.Server.Networking;
using Intersect.Server.Web.RestApi.Payloads;

using JetBrains.Annotations;

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

        #endregion

        #region Entity Framework

        #region Lookup

        public static Tuple<Client, Player> Fetch(LookupKey lookupKey, [CanBeNull] PlayerContext playerContext = null)
        {
            if (!lookupKey.HasName && !lookupKey.HasId)
            {
                return new Tuple<Client, Player>(null, null);
            }

            // HasName checks if null or empty
            // ReSharper disable once AssignNullToNotNullAttribute
            return lookupKey.HasId ? Fetch(lookupKey.Id, playerContext) : Fetch(lookupKey.Name, playerContext);
        }

        public static Tuple<Client, Player> Fetch(
            [NotNull] string playerName,
            [CanBeNull] PlayerContext playerContext = null
        )
        {
            var client = Globals.Clients.Find(queryClient => Entity.CompareName(playerName, queryClient?.Entity?.Name));

            return new Tuple<Client, Player>(client, client?.Entity ?? Player.Find(playerName, playerContext));
        }

        public static Tuple<Client, Player> Fetch(Guid playerId, [CanBeNull] PlayerContext playerContext = null)
        {
            var client = Globals.Clients.Find(queryClient => playerId == queryClient?.Entity?.Id);

            return new Tuple<Client, Player>(client, client?.Entity ?? Player.Find(playerId, playerContext));
        }

        public static Player Find(Guid playerId, [CanBeNull] PlayerContext playerContext = null)
        {
            if (playerContext == null)
            {
                lock (DbInterface.GetPlayerContextLock())
                {
                    var context = DbInterface.GetPlayerContext();

                    return QueryPlayerById(context, playerId);
                }
            }
            else
            {
                return QueryPlayerById(playerContext, playerId);
            }
        }

        public static Player Find([NotNull] string playerName, [CanBeNull] PlayerContext playerContext = null)
        {
            if (playerContext == null)
            {
                lock (DbInterface.GetPlayerContextLock())
                {
                    var context = DbInterface.GetPlayerContext();

                    return QueryPlayerByName(context, playerName);
                }
            }
            else
            {
                return QueryPlayerByName(playerContext, playerName);
            }
        }

        #endregion

        #region Loading

        [CanBeNull]
        public static Player Load(Guid playerId, [CanBeNull] PlayerContext playerContext = null)
        {
            var player = Find(playerId, playerContext);

            return Load(player);
        }

        [CanBeNull]
        public static Player Load([NotNull] string playerName, [CanBeNull] PlayerContext playerContext = null)
        {
            var player = Find(playerName, playerContext);

            return Load(player);
        }

        [CanBeNull]
        public static Player Load([CanBeNull] Player player)
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

        #region Listing

        [NotNull]
        public static int Count()
        {
            lock (DbInterface.GetPlayerContextLock())
            {
                var context = DbInterface.GetPlayerContext();

                return context.Players.Count();
            }
        }

        [NotNull]
        public static IList<Player> List(int page, int count, [CanBeNull] PlayerContext playerContext = null)
        {
            if (playerContext == null)
            {
                lock (DbInterface.GetPlayerContextLock())
                {
                    var context = DbInterface.GetPlayerContext();

                    return QueryPlayers(context, page * count, count)?.ToList() ?? throw new InvalidOperationException();
                }
            }
            else
            {
                return QueryPlayers(playerContext, page * count, count)?.ToList() ?? throw new InvalidOperationException();
            }
        }

        [NotNull]
        public static IEnumerable<Player> Rank(
            int page,
            int count,
            SortDirection sortDirection,
            [CanBeNull] PlayerContext playerContext = null
        )
        {
            var context = playerContext;
            if (context != null)
            {
                throw new InvalidOperationException();
            }

            lock (DbInterface.GetPlayerContextLock())
            {
                context = DbInterface.GetPlayerContext();
                var results = sortDirection == SortDirection.Ascending
                    ? QueryPlayersWithRankAscending(context, page * count, count)
                    : QueryPlayersWithRank(context, page * count, count);

                return results?.ToList() ?? throw new InvalidOperationException();
            }
        }

        #endregion

        #region Compiled Queries

        [NotNull] private static readonly Func<PlayerContext, int, int, IEnumerable<Player>> QueryPlayers =
            EF.CompileQuery(
                (PlayerContext context, int offset, int count) => context.Players
                    .OrderBy(player => player.Id.ToString())
                    .Skip(offset)
                    .Take(count)
                    .Include(p => p.Bank)
                    .Include(p => p.Friends)
                    .ThenInclude(p => p.Target)
                    .Include(p => p.Hotbar)
                    .Include(p => p.Quests)
                    .Include(p => p.Variables)
                    .Include(p => p.Items)
                    .Include(p => p.Spells)
            ) ??
            throw new InvalidOperationException();

        [NotNull] private static readonly Func<PlayerContext, int, int, IEnumerable<Player>> QueryPlayersWithRank =
            EF.CompileQuery(
                (PlayerContext context, int offset, int count) => context.Players
                    .OrderByDescending(entity => EF.Property<dynamic>(entity, "Level"))
                    .ThenByDescending(entity => EF.Property<dynamic>(entity, "Exp"))
                    .Skip(offset)
                    .Take(count)
                    .Include(p => p.Bank)
                    .Include(p => p.Friends)
                    .ThenInclude(p => p.Target)
                    .Include(p => p.Hotbar)
                    .Include(p => p.Quests)
                    .Include(p => p.Variables)
                    .Include(p => p.Items)
                    .Include(p => p.Spells)
            ) ??
            throw new InvalidOperationException();

        [NotNull]
        private static readonly Func<PlayerContext, int, int, IEnumerable<Player>> QueryPlayersWithRankAscending =
            EF.CompileQuery(
                (PlayerContext context, int offset, int count) => context.Players
                    .OrderBy(entity => EF.Property<dynamic>(entity, "Level"))
                    .ThenBy(entity => EF.Property<dynamic>(entity, "Exp"))
                    .Skip(offset)
                    .Take(count)
                    .Include(p => p.Bank)
                    .Include(p => p.Friends)
                    .ThenInclude(p => p.Target)
                    .Include(p => p.Hotbar)
                    .Include(p => p.Quests)
                    .Include(p => p.Variables)
                    .Include(p => p.Items)
                    .Include(p => p.Spells)
            ) ??
            throw new InvalidOperationException();

        [NotNull] private static readonly Func<PlayerContext, Guid, Player> QueryPlayerById =
            EF.CompileQuery(
                (PlayerContext context, Guid id) => context.Players.Where(p => p.Id == id)
                    .Include(p => p.Bank)
                    .Include(p => p.Friends)
                    .ThenInclude(p => p.Target)
                    .Include(p => p.Hotbar)
                    .Include(p => p.Quests)
                    .Include(p => p.Variables)
                    .Include(p => p.Items)
                    .Include(p => p.Spells)
                    .FirstOrDefault()
            ) ??
            throw new InvalidOperationException();

        [NotNull] private static readonly Func<PlayerContext, string, Player> QueryPlayerByName =
            EF.CompileQuery(
                (PlayerContext context, string name) => context.Players.Where(p => p.Name == name)
                    .Include(p => p.Bank)
                    .Include(p => p.Friends)
                    .ThenInclude(p => p.Target)
                    .Include(p => p.Hotbar)
                    .Include(p => p.Quests)
                    .Include(p => p.Variables)
                    .Include(p => p.Items)
                    .Include(p => p.Spells)
                    .FirstOrDefault()
            ) ??
            throw new InvalidOperationException();

        #endregion

        #endregion

    }

}
