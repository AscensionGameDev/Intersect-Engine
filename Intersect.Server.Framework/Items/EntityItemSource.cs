using Intersect.Enums;
using Intersect.Server.Framework.Entities;

namespace Intersect.Server.Framework;

public class EntityItemSource: IItemSource
{
    public Guid Id { get; set; }
    public EntityType EntityType { get; set; }
    public WeakReference<IEntity> EntityReference { get; set; }
}

public class EntityItemSource<TEntity> : EntityItemSource where TEntity : class, IEntity
{
    public new WeakReference<TEntity> EntityReference { get; set; }
}