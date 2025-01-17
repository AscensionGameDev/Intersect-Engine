using Intersect.GameObjects;
using Intersect.Server.Entities;
using Intersect.Server.Maps;
using Intersect.Server.Networking;

namespace Intersect.Server.General;


public static partial class Globals
{

    public static readonly object ClientLock = new object();

    public static readonly Dictionary<Guid, Client> ClientLookup = [];

    public static readonly List<Client> Clients = [];

    public static long Cps = 0;

    public static List<Player> OnlineList => Clients.FindAll(client => client?.Entity != null)
        .Select(client => client.Entity)
        .ToList();

    public static void KillResourcesOf(ResourceBase resource)
    {
        foreach (MapController map in MapController.Lookup.Values)
        {
            map?.DespawnResourceAcrossInstances(resource);
        }
    }

    public static void KillNpcsOf(NpcBase npc)
    {
        foreach (MapController map in MapController.Lookup.Values)
        {
            map?.DespawnNpcAcrossInstances(npc);
        }
    }

    public static void KillProjectilesOf(ProjectileBase projectile)
    {
        foreach (MapController map in MapController.Lookup.Values)
        {
            map?.DespawnProjectileAcrossInstances(projectile);
        }
    }

    public static void KillItemsOf(ItemBase item)
    {
        foreach (MapController map in MapController.Lookup.Values)
        {
            map?.DespawnItemAcrossInstances(item);
        }
    }

}
