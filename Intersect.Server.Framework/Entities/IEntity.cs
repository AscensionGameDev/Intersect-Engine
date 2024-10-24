using Intersect.Enums;

namespace Intersect.Server.Framework.Entities;

public interface IEntity: IDisposable
{
    Guid Id { get; }
    string Name { get; }
    Guid MapInstanceId { get; set; }
    int X { get; }
    int Y { get; }
    int Z { get; }
}