using System.Diagnostics.CodeAnalysis;
using Intersect.Server.Framework.Items;
using Intersect.Server.Framework.Maps;

namespace Intersect.Server.Plugins.Helpers;

public interface IMapHelper
{
    event ItemAddedHandler ItemAdded;
    bool TryGetMapInstance(Guid mapId, Guid instanceId, [NotNullWhen(true)] out IMapInstance? instance);
}

public delegate void ItemAddedHandler(IItemSource? source, IItem item);
