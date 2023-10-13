namespace Intersect.Server.Core;

internal interface INetworkPoolDataProvider
{
    long ActiveThreads { get; }
    long CurrentWorkItemsCount { get; }
    long InUseThreads { get; }
}