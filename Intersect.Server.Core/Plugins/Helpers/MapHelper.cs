using Intersect.Server.Framework.Items;
using Intersect.Server.Framework.Maps;
using Intersect.Server.Maps;

namespace Intersect.Server.Plugins.Helpers;

public partial class MapHelper: IMapHelper
{
    public event Action<IItemSource, IItem> ItemAdded;
    
    public IMapInstance GetMapInstanceByDescriptorId(Guid mapId, Guid mapInstanceId)
    {
        MapController.TryGetInstanceFromMap(mapId, mapInstanceId, out var instance);
        return instance;
    }
    
    public void InvokeItemAdded(IItemSource source, IItem item)
    {
        ItemAdded?.Invoke(source, item);
    }
}