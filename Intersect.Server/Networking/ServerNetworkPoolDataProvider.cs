using Intersect.Server.Core;
using Intersect.Server.Networking.LiteNetLib;

namespace Intersect.Server.Networking;

internal sealed class ServerNetworkPoolDataProvider : INetworkPoolDataProvider
{
    public long ActiveThreads => ServerNetwork.Pool.ActiveThreads;
    public long CurrentWorkItemsCount => ServerNetwork.Pool.CurrentWorkItemsCount;
    public long InUseThreads => ServerNetwork.Pool.InUseThreads;
}