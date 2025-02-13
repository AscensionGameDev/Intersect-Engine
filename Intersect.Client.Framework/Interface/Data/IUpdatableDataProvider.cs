namespace Intersect.Client.Interface.Data;

public interface IUpdatableDataProvider : IDataProvider
{
    bool TryUpdate(TimeSpan elapsed, TimeSpan total);
}