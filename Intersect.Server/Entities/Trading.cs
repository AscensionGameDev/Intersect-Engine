using System;
using System.Collections.Generic;

using Intersect.Server.Database;

using JetBrains.Annotations;

namespace Intersect.Server.Entities
{

    public struct Trading : IDisposable
    {

        [NotNull] private readonly Player mPlayer;

        public bool Actively => Counterparty != null;

        [CanBeNull] public Player Counterparty;

        public bool Accepted;

        [NotNull] public Item[] Offer;

        public Player Requester;

        [NotNull] public Dictionary<Player, long> Requests;

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
