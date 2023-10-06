using Intersect.Logging;
using LiteNetLib;

namespace Intersect.Network.LiteNetLib;

public sealed class LiteNetLibLogger : INetLogger
{
    public void WriteNet(NetLogLevel level, string str, params object[] args) => Log.Write(level.ToIntersectLogLevel(), str, args);
}