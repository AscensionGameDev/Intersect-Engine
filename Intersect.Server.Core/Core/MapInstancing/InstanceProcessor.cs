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
    private static Dictionary<Guid, InstanceController> InstanceControllers = new Dictionary<Guid, InstanceController>();

    public static Guid[] CurrentControllers => InstanceControllers.Keys.ToArray();

    public static bool TryGetInstanceController(Guid instanceId, out InstanceController controller)
    {
        return InstanceControllers.TryGetValue(instanceId, out controller);
    }

    private static void CleanupOrphanedControllers(List<MapInstance> activeMaps)
    {
        var processingInstances = InstanceControllers.Keys;
        var processingMaps = activeMaps.ToArray().Select(map => map.MapInstanceId);

        foreach (var id in processingInstances.Except(processingMaps).ToArray())
        {
            // Never cleanup the overworld controller
            if (id == Guid.Empty)
            {
                continue;
            }
            InstanceControllers.Remove(id);
            Logging.Log.Debug($"Removing instance controller {id}");
        }
    }

    public static void AddInstanceController(Guid mapInstanceId, Player creator)
    {
        if (!InstanceControllers.ContainsKey(mapInstanceId))
        {
            InstanceControllers[mapInstanceId] = new InstanceController(mapInstanceId, creator);
        }
    }

    public static void UpdateInstanceControllers(List<MapInstance> activeMaps)
    {
        if (activeMaps == null || activeMaps.Count == 0)
        {
            return;
        }

        // Cleanup inactive instances
        CleanupOrphanedControllers(activeMaps);

        Dictionary<Guid, List<MapInstance>> mapsAndInstances = activeMaps
            .GroupBy(m => m.MapInstanceId)
            .ToDictionary(m => m.Key, m => m.ToList());

        // For each instance...
        foreach (var mapInstanceGroup in mapsAndInstances)
        {
            // Fetch our instance controller...
            var instanceId = mapInstanceGroup.Key;
            if (!InstanceControllers.TryGetValue(instanceId, out var instanceController))
            {
                continue;
            }

            // TODO do update-y things in here, i.e processing permadead NPCs. Keeping empty for initial code review
        }
    }
}
