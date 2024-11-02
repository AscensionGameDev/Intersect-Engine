using Intersect.Server.Framework.Maps;

namespace Intersect.Server.Framework.Items;

public partial class MapItemSource : IItemSource
{
    public Guid Id { get; init; }
    public ItemSourceType SourceType => ItemSourceType.Map;
    public WeakReference<IMapInstance> MapInstanceReference { get; init; }
    public Guid DescriptorId { get; init; }
}