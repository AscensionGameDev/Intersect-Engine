using System;
using System.Collections.Generic;
using System.Linq;

using Intersect.GameObjects;
using Intersect.Server.Entities;
using Intersect.Server.Maps;
using Intersect.Server.Networking;
using Intersect.Utilities;

namespace Intersect.Server.General
{

    public static class Globals
    {

        public static readonly object ClientLock = new object();

        public static readonly IDictionary<Guid, Client> ClientLookup = new Dictionary<Guid, Client>();

        public static readonly List<Client> Clients = new List<Client>();

        public static Client[] ClientArray = new Client[0];

        public static long Cps = 0;

        [Obsolete] public static Timing Timing => Timing.Global;

        public static List<Player> OnlineList => Clients.FindAll(client => client?.Entity != null)
            .Select(client => client.Entity)
            .ToList();

        public static void KillResourcesOf(ResourceBase resource)
        {
            foreach (MapInstance map in MapInstance.Lookup.Values)
            {
                map?.DespawnResourcesOf(resource);
            }
        }

        public static void KillNpcsOf(NpcBase npc)
        {
            foreach (MapInstance map in MapInstance.Lookup.Values)
            {
                map?.DespawnNpcsOf(npc);
            }
        }

        public static void KillProjectilesOf(ProjectileBase projectile)
        {
            foreach (MapInstance map in MapInstance.Lookup.Values)
            {
                map?.DespawnProjectilesOf(projectile);
            }
        }

        public static void KillItemsOf(ItemBase item)
        {
            foreach (MapInstance map in MapInstance.Lookup.Values)
            {
                map?.DespawnItemsOf(item);
            }
        }

    }

}
