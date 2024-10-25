namespace Intersect.Server.Framework.Maps;

public interface IMapInstance: IDisposable
{
    Guid Id { get; }
    public Guid MapInstanceId { get; }
}