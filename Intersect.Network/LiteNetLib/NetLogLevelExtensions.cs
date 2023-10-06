using System.Diagnostics;
using Intersect.Logging;
using LiteNetLib;

namespace Intersect.Network.LiteNetLib;

public static class NetLogLevelExtensions
{
    public static LogLevel ToIntersectLogLevel(this NetLogLevel netLogLevel) =>
        netLogLevel switch
        {
            NetLogLevel.Warning => LogLevel.Warn,
            NetLogLevel.Error => LogLevel.Error,
            NetLogLevel.Trace => LogLevel.Trace,
            NetLogLevel.Info => LogLevel.Info,
            _ => throw new UnreachableException(),
        };
}