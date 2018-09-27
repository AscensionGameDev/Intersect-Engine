using System;
using System.Collections.Generic;
using System.Linq;
using Intersect.GameObjects;
using Intersect.Server.Classes.Core;
using Intersect.Server.Classes.Entities;
using Intersect.Server.Classes.Maps;
using Intersect.Server.Classes.Networking;
using Intersect.Server.Network;
using Intersect.Server.WebApi;

namespace Intersect.Server.Classes.General
{
    public static class Globals
    {
        //Api
        public static ServerApi Api { get; set; }
        public static ServerNetwork Network { get; set; }

        //Console Variables
        public static long Cps = 0;

        public static bool CpsLock = true;
        public static bool ServerStarted;
        public static bool ServerStopped;
        public static ServerSystem System = new ServerSystem();

        public static object ClientLock = new object();
        public static List<Client> Clients = new List<Client>();
        public static IDictionary<Guid, Client> ClientLookup = new Dictionary<Guid, Client>();

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