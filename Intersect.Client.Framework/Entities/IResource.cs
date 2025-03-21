using Intersect.Framework.Core.GameObjects.Resources;

namespace Intersect.Client.Framework.Entities;

public interface IResource : IEntity
{
    ResourceDescriptor? Descriptor { get; }
}