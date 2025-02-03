namespace Intersect.Async;

public interface ICancellableGenerator : IDisposable
{
    ICancellableGenerator Start();
}