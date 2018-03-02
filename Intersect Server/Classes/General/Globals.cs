using System;
using System.Collections.Generic;
using System.Linq;
using Intersect.GameObjects;
using Intersect.Server.Classes.Core;
using Intersect.Server.Classes.Entities;
using Intersect.Server.Classes.Maps;
using Intersect.Server.Classes.Networking;

namespace Intersect.Server.Classes.General
{
    public static class Globals
    {
        //Console Variables
        public static long Cps = 0;

        public static bool CpsLock = true;
        public static bool ServerStarted = true;
        public static ServerSystem System = new ServerSystem();

        public static object ClientLock = new object();
        public static List<Client> Clients = new List<Client>();
        public static IDictionary<Guid, Client> ClientLookup = new Dictionary<Guid, Client>();

        public static List<EntityInstance> Entities = new List<EntityInstance>();

        //Game helping stuff
        public static Random Rand = new Random();

        public static List<EntityInstance> OnlineList => Clients?
            .FindAll(client => client?.Entity != null)
            .Select<Client, EntityInstance>(client => client.Entity)
            .ToList();

        public static int FindOpenEntity()
        {
            for (var i = 0; i < Entities.Count; i++)
            {
                if (Entities[i] == null)
                {
                    //return i;
                }
                else if (i == Entities.Count - 1)
                {
                    Entities.Add(null);
                    return Entities.Count - 1;
                }
            }
            Entities.Add(null);
            return Entities.Count - 1;
        }

        public static void AddEntity(EntityInstance newEntity)
        {
            var slot = FindOpenEntity();
            Entities[slot] = newEntity;
        }

        public static void KillResourcesOf(ResourceBase resource)
        {
            var resources = new List<Resource>();
            for (int i = 0; i < Entities.Count; i++)
            {
                if (Entities[i] != null && Entities[i].GetType() == typeof(Resource) &&
                    ((Resource) Entities[i]).MyBase == resource)
                {
                    resources.Add((Resource) Entities[i]);
                }
            }
            foreach (var en in resources)
            {
                en.Die(0);
                en.Spawn();
            }
        }

        public static void KillNpcsOf(NpcBase npc)
        {
            var npcs = new List<Npc>();
            for (int i = 0; i < Entities.Count; i++)
            {
                if (Entities[i] != null && Entities[i].GetType() == typeof(Npc) && ((Npc) Entities[i]).MyBase == npc)
                {
                    npcs.Add((Npc) Entities[i]);
                }
            }
            foreach (var en in npcs)
            {
                en.Die(0);
            }
        }

        public static void KillProjectilesOf(ProjectileBase projectile)
        {
            var projectiles = new List<Projectile>();
            for (int i = 0; i < Entities.Count; i++)
            {
                if (Entities[i] != null && Entities[i].GetType() == typeof(Projectile) &&
                    ((Projectile) Entities[i]).MyBase == projectile)
                {
                    projectiles.Add((Projectile) Entities[i]);
                }
            }
            foreach (var en in projectiles)
            {
                en.Die(0);
            }
        }

        public static void KillItemsOf(ItemBase item)
        {
            foreach (MapInstance map in MapInstance.Lookup.IndexValues)
            {
                map?.DespawnItemsOf(item);
            }
        }
    }
}