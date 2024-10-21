using Intersect.Enums;
using Intersect.Server.Framework.Entities;

namespace Intersect.Server.Framework;

public interface IItemSource
{
    Guid Id { get; }
    EntityType EntityType { get; }
    WeakReference<IEntity> EntityReference { get; }
}