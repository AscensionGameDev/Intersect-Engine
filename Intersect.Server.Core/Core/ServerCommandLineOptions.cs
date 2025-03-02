using CommandLine;
using Intersect.Core;
using Intersect.Network;

namespace Intersect.Server.Core;

internal partial record ServerCommandLineOptions : ICommandLineOptions
{
    [Option('C', "no-console", Default = false, Required = false)]
    public bool DoNotShowConsole { get; init; }

    [Option('H', "no-halt", Default = false, Required = false)]
    public bool DoNotHaltOnError { get; init; }

    [Option('m', "migrate-automatically", Default = false, Required = false)]
    public bool MigrateAutomatically { get; init; }

    [Option('U', "no-upnp", Default = false, Required = false)]
    public bool NoNatPunchthrough { get; init; }

    [Option('P', "no-port-check", Default = false, Required = false)]
    public bool NoNetworkCheck { get; init; }

    [Option('p', "port", Default = (ushort)0, Required = false)]
    public ushort Port { get; init; }

    [Option("working-directory", Default = null, Required = false)]
    public string WorkingDirectory { get; init; }

    [Option('p', "plugin-directory", Default = null, Required = false)]
    public IEnumerable<string> PluginDirectories { get; init; }

    public ushort ValidPort(ushort defaultPort)
    {
        return PortHelper.IsValidPort(Port) ? Port : defaultPort;
    }
}