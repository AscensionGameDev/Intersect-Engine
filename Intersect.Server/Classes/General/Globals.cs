using System;
using System.Collections.Generic;
using System.Linq;
using Intersect.GameObjects;
using Intersect.Server.Entities;
using Intersect.Server.Maps;
using Intersect.Server.Networking;
using Intersect.Server.Networking.Lidgren;
using Intersect.Server.WebApi;
using JetBrains.Annotations;

namespace Intersect.Server.General
{
    public static class Globals
    {
        //Api
        public static ServerApi Api { get; set; }

        [NotNull]
        public static ServerTiming Timing { get; } = new ServerTiming();

        public static long Cps = 0;
        public static bool CpsLock = true;

        [NotNull] public static readonly object ClientLock = new object();
        [NotNull] public static readonly List<Client> Clients = new List<Client>();
        [NotNull] public static readonly IDictionary<Guid, Client> ClientLookup = new Dictionary<Guid, Client>();

        //Game helping stuff
        public static Random Rand = new Random();

        public static List<EntityInstance> OnlineList => Clients?
            .FindAll(client => client?.Entity != null)
            .Select<Client, EntityInstance>(client => client.Entity)
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