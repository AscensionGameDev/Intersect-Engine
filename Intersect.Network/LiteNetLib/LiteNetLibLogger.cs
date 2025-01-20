
using Intersect.Core;
using LiteNetLib;
using Microsoft.Extensions.Logging;

namespace Intersect.Network.LiteNetLib;

public sealed class LiteNetLibLogger : INetLogger
{
    public void WriteNet(NetLogLevel level, string format, params object[] args) =>
        ApplicationContext.Context.Value?.Logger.Log(level.ToLogLevel(), format, args);
}