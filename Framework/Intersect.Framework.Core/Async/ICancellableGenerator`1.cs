namespace Intersect.Async;

public interface ICancellableGenerator<TValue> : ICancellableGenerator
{
    new ICancellableGenerator<TValue> Start();

    ICancellableGenerator ICancellableGenerator.Start() => Start();
}