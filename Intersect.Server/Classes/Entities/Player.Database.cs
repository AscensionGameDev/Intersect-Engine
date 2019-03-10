using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

using Intersect.Server.Database.PlayerData;
using Intersect.Server.General;
using Intersect.Server.Networking;

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

        public static Player Find(Guid playerId, [CanBeNull] PlayerContext playerContext = null)
        {
            return QueryPlayerById(playerContext ?? PlayerContext.Current, playerId);
        }

        public static Player Find([NotNull] string playerName, [CanBeNull] PlayerContext playerContext = null)
        {
            return QueryPlayerByName(playerContext ?? PlayerContext.Current, playerName);
        }

        public static Tuple<Client, Player> Fetch([NotNull] string playerName)
        {
            var client = Globals.Clients.Find(
                queryClient => EntityInstance.CompareName(playerName, queryClient?.Entity?.Name)
            );

            return new Tuple<Client, Player>(client, client?.Entity ?? Player.Find(playerName));
        }

        public static Tuple<Client, Player> Fetch(Guid playerId)
        {
            var client = Globals.Clients.Find(queryClient => playerId == queryClient?.Entity?.Id);

            return new Tuple<Client, Player>(client, client?.Entity ?? Player.Find(playerId));
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

        #region Compiled Queries

        [NotNull] private static readonly Func<PlayerContext, Guid, Player> QueryPlayerById =
            EF.CompileQuery((PlayerContext context, Guid id) =>
                context.Players
                    .Include(p => p.Bank)
                    .Include(p => p.Friends)
                    .ThenInclude(p => p.Target)
                    .Include(p => p.Hotbar)
                    .Include(p => p.Quests)
                    .Include(p => p.Switches)
                    .Include(p => p.Variables)
                    .Include(p => p.Items)
                    .Include(p => p.Spells)
                    .FirstOrDefault(c => c.Id == id))
            ?? throw new InvalidOperationException();

        [NotNull] private static readonly Func<PlayerContext, string, Player> QueryPlayerByName =
            EF.CompileQuery((PlayerContext context, string name) =>
                context.Players
                    .Include(p => p.Bank)
                    .Include(p => p.Friends)
                    .ThenInclude(p => p.Target)
                    .Include(p => p.Hotbar)
                    .Include(p => p.Quests)
                    .Include(p => p.Switches)
                    .Include(p => p.Variables)
                    .Include(p => p.Items)
                    .Include(p => p.Spells)
                    .FirstOrDefault(c => CompareName(name, c.Name)))
            ?? throw new InvalidOperationException();

        #endregion

        #endregion
    }
}
