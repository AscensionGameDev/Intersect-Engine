using Intersect.GameObjects;
using Intersect.Server.Entities;
using Intersect.Server.Maps;
using Intersect.Server.Networking;

namespace Intersect.Server.General;

public static partial class Globals
{
    public static long Cps = 0;

    public static List<Player> OnlineList => Client.Instances.FindAll(client => client?.Entity != null)
        .Select(client => client.Entity)
        .ToList();

    public static void DespawnInstancesOf(ResourceBase resource)
    {
        var allMapControllers = MapController.Lookup.Values.OfType<MapController>().ToArray();
        foreach (var map in allMapControllers)
        {
            map?.DespawnResourceAcrossInstances(resource);
        }
    }

    public static void DespawnInstancesOf(NpcBase npc)
    {
        var allMapControllers = MapController.Lookup.Values.OfType<MapController>().ToArray();
        foreach (var map in allMapControllers)
        {
            map?.DespawnNpcAcrossInstances(npc);
        }
    }

    public static void DespawnInstancesOf(ProjectileBase projectile)
    {
        var allMapControllers = MapController.Lookup.Values.OfType<MapController>().ToArray();
        foreach (var map in allMapControllers)
        {
            map?.DespawnProjectileAcrossInstances(projectile);
        }
    }

    public static void DespawnInstancesOf(ItemBase item)
    {
        var allMapControllers = MapController.Lookup.Values.OfType<MapController>().ToArray();
        foreach (var map in allMapControllers)
        {
            map?.DespawnItemAcrossInstances(item);
        }
    }
}