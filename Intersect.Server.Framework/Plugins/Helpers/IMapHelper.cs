using Intersect.Server.Framework.Items;
using Intersect.Server.Framework.Maps;

namespace Intersect.Server.Plugins.Helpers;

public interface IMapHelper
{
    event Action<IItemSource, IItem> ItemAdded;
    IMapInstance GetMapInstanceByDescriptorId(Guid mapId, Guid mapInstanceId);
    void InvokeItemAdded(IItemSource source, IItem item);
}