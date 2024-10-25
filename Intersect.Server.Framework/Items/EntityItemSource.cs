using Intersect.Enums;
using Intersect.Server.Framework.Entities;

namespace Intersect.Server.Framework.Items;

public class EntityItemSource: IItemSource
{
    public Guid Id { get; init; }
    public EntityType EntityType { get; init; }
    public WeakReference<IEntity> EntityReference { get; init; }
}