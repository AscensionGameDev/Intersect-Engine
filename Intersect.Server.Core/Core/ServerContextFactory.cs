
using Intersect.Plugins.Interfaces;
using Microsoft.Extensions.Logging;

namespace Intersect.Server.Core;

internal delegate IServerContext ServerContextFactory(
    ServerCommandLineOptions startupOptions,
    ILogger logger,
    IPacketHelper packetHelper
);