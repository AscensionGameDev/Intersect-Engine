using Intersect.GameObjects;

namespace Intersect.Client.Framework.Entities
{
    public interface IResource : IEntity
    {
        ResourceBase BaseResource { get; set; }
        
    }
}