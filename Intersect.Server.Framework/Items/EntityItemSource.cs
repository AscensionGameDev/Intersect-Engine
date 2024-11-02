using Intersect.Enums;
using Intersect.Server.Framework.Entities;

namespace Intersect.Server.Framework.Items;

public partial class EntityItemSource: IItemSource
{
    public Guid Id { get; init; }
    public ItemSourceType SourceType => ItemSourceType.Entity;
    public EntityType EntityType { get; init; }
    public WeakReference<IEntity> EntityReference { get; init; }
}