using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.Server.Core.MapInstancing.Controllers;
using Intersect.Server.Entities;
using Intersect.Server.Maps;

namespace Intersect.Server.Core.MapInstancing;
public static class InstanceProcessor
{
    private static Dictionary<Guid, InstanceController> InstanceControllers = new();

    public static Guid[] CurrentControllers => InstanceControllers.Keys.ToArray();

    public static bool TryGetInstanceController(Guid instanceId, out InstanceController controller)
    {
        return InstanceControllers.TryGetValue(instanceId, out controller);
    }

    private static void CleanupOrphanedControllers(IEnumerable<Guid> activeInstanceIds)
    {
        var processingInstances = InstanceControllers.Keys
            .Except(activeInstanceIds)
            .Except(default) // Never cleanup the overworld controller
            .ToArray();

        foreach (var id in processingInstances)
        {
            InstanceControllers.Remove(id);
            Logging.Log.Debug($"Removing instance controller {id}");
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

        // Cleanup inactive instances
        CleanupOrphanedControllers(activeMaps.Select(map => map.MapInstanceId));

        Dictionary<Guid, MapInstance[]> mapsAndInstances = activeMaps
            .GroupBy(m => m.MapInstanceId)
            .ToDictionary(m => m.Key, m => m.ToArray());

        // For each instance...
        foreach (var (instanceId, mapInstance) in mapsAndInstances)
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
