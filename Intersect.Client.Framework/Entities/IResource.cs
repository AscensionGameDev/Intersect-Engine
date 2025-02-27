using Intersect.Framework.Core.GameObjects.Resources;
using Intersect.GameObjects;

namespace Intersect.Client.Framework.Entities;

public interface IResource : IEntity
{
    ResourceDescriptor? Descriptor { get; }
    
    bool IsDepleted { get; }
}