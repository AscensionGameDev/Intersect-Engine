using System.Diagnostics;
using LiteNetLib;
using Microsoft.Extensions.Logging;

namespace Intersect.Network.LiteNetLib;

public static class NetLogLevelExtensions
{
    public static LogLevel ToLogLevel(this NetLogLevel netLogLevel) =>
        netLogLevel switch
        {
            NetLogLevel.Warning => LogLevel.Warning,
            NetLogLevel.Error => LogLevel.Error,
            NetLogLevel.Trace => LogLevel.Trace,
            NetLogLevel.Info => LogLevel.Information,
            _ => throw new UnreachableException(),
        };
}