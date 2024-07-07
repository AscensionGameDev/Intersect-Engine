using Intersect.Client.Framework.Entities;

namespace Intersect.Client.Framework.Maps;

public interface IMapAnimation : IAnimation
{
    Guid Id { get; }
}