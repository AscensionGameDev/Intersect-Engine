using Intersect.Server.Framework.Entities;

namespace Intersect.Server.Framework.Items;

public class EntityItemSource<TEntity> : EntityItemSource where TEntity : class, IEntity
{
    private WeakReference<TEntity> _entityReference;

    public new WeakReference<TEntity> EntityReference
    {
        get => _entityReference;
        init
        {
            _entityReference = value;
            if (_entityReference.TryGetTarget(out TEntity target))
            {
                base.EntityReference = new WeakReference<IEntity>(target);
            }
        }
    }
}