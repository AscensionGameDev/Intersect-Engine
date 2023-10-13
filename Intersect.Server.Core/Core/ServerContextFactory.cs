using Intersect.Logging;
using Intersect.Plugins.Interfaces;

namespace Intersect.Server.Core;

internal delegate IServerContext ServerContextFactory(
    ServerCommandLineOptions startupOptions,
    Logger logger,
    IPacketHelper packetHelper
);