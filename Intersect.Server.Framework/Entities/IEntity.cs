namespace Intersect.Server.Framework.Entities;

public interface IEntity: IDisposable
{
    Guid Id { get; }
    string Name { get; }
    Guid MapInstanceId { get; }
}