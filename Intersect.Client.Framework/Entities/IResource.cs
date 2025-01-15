using Intersect.GameObjects;

namespace Intersect.Client.Framework.Entities;

public interface IResource : IEntity
{
    ResourceBase? Descriptor { get; }
    
    bool IsDepleted { get; }
}