using System.Diagnostics.CodeAnalysis;
using Intersect.Server.Framework.Items;
using Intersect.Server.Framework.Maps;
using Intersect.Server.Maps;

namespace Intersect.Server.Plugins.Helpers;

public partial class MapHelper : IMapHelper
{
    private static readonly Lazy<MapHelper> _instance = new Lazy<MapHelper>(() => new MapHelper());
    public static MapHelper Instance => _instance.Value;
    private MapHelper() { }
    public event ItemAddedHandler ItemAdded;
    
    public bool TryGetMapInstance(Guid mapId, Guid instanceId, [NotNullWhen(true)] out IMapInstance? instance)
    {
        instance = null;
        if (MapController.TryGetInstanceFromMap(mapId, instanceId, out var mapInstance))
        {
            instance = mapInstance;
            return mapInstance != null;
        }
        return false;
    }
    
    public void InvokeItemAdded(IItemSource source, IItem item)
    {
        ItemAdded?.Invoke(source, item);
    }
}