using Intersect.Core;
using Intersect.Server.Core.MapInstancing.Controllers;
using Intersect.Server.Entities;
using Intersect.Server.Maps;
using Microsoft.Extensions.Logging;

namespace Intersect.Server.Core.MapInstancing;

public static class InstanceProcessor
{
    private static readonly Dictionary<Guid, InstanceController> InstanceControllers = new();

    private static readonly HashSet<Guid> ActiveIdScratch = [];

    public static bool TryGetInstanceController(Guid instanceId, out InstanceController controller) => InstanceControllers.TryGetValue(instanceId, out controller);

    private static void CleanupOrphanedControllers(MapInstance[] activeMaps)
    {
        ActiveIdScratch.Clear();
        for (var i = 0; i < activeMaps.Length; i++)
        {
            ActiveIdScratch.Add(activeMaps[i].MapInstanceId);
        }

        List<Guid>? toRemove = null;
        foreach (var id in InstanceControllers.Keys)
        {
            if (id == default)
            {
                continue; // never clean overworld
            }

            if (!ActiveIdScratch.Contains(id))
            {
                (toRemove ??= new List<Guid>()).Add(id);
            }
        }

        if (toRemove != null)
        {
            foreach (var id in toRemove)
            {
                InstanceControllers.Remove(id);
                ApplicationContext.Context.Value?.Logger.LogDebug($"Removing instance controller {id}");
            }
        }
    }

    public static bool TryAddInstanceController(Guid mapInstanceId, Player creator)
    {
        if (InstanceControllers.ContainsKey(mapInstanceId))
        {
            return false;
        }

        InstanceControllers[mapInstanceId] = new InstanceController(mapInstanceId, creator);
        return true;
    }

    public static void UpdateInstanceControllers(MapInstance[] activeMaps)
    {
        if (activeMaps == null || activeMaps.Length == 0)
        {
            return;
        }

        CleanupOrphanedControllers(activeMaps);

        // Manual grouping + ToDictionary allocations
        var mapsAndInstances = new Dictionary<Guid, List<MapInstance>>();
        for (var i = 0; i < activeMaps.Length; i++)
        {
            var map = activeMaps[i];
            if (!mapsAndInstances.TryGetValue(map.MapInstanceId, out var list))
            {
                mapsAndInstances[map.MapInstanceId] = list = new List<MapInstance>();
            }

            list.Add(map);
        }

        foreach (var (instanceId, mapsInInstance) in mapsAndInstances)
        {
            // Fetch our instance controller...
            if (!InstanceControllers.TryGetValue(instanceId, out var instanceController))
            {
                continue;
            }

            // TODO do update-y things in here, i.e processing permadead NPCs. Keeping empty for initial code review
        }
    }
}
