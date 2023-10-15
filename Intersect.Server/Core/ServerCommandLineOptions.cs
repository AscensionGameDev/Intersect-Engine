using CommandLine;
using Intersect.Core;
using Intersect.Network;

namespace Intersect.Server.Core;

internal struct ServerCommandLineOptions : ICommandLineOptions
{
    public ServerCommandLineOptions(
        bool doNotShowConsole,
        bool doNotHaltOnError,
        bool migrateAutomatically,
        bool noNatPunchthrough,
        bool noNetworkCheck,
        ushort port,
        string workingDirectory,
        IEnumerable<string> pluginDirectories
    )
    {
        DoNotShowConsole = doNotShowConsole;
        DoNotHaltOnError = doNotHaltOnError;
        MigrateAutomatically = migrateAutomatically;
        NoNatPunchthrough = noNatPunchthrough;
        NoNetworkCheck = noNetworkCheck;
        Port = port;
        WorkingDirectory = workingDirectory;
        PluginDirectories = pluginDirectories?.Select(Path.GetFullPath).ToArray();
    }

    [Option('C', "no-console", Default = false, Required = false)]
    public bool DoNotShowConsole { get; }

    [Option('H', "no-halt", Default = false, Required = false)]
    public bool DoNotHaltOnError { get; }

    [Option('m', "migrate-automatically", Default = false, Required = false)]
    public bool MigrateAutomatically { get; }

    [Option('U', "no-upnp", Default = false, Required = false)]
    public bool NoNatPunchthrough { get; }

    [Option('P', "no-port-check", Default = false, Required = false)]
    public bool NoNetworkCheck { get; }

    [Option('p', "port", Default = (ushort)0, Required = false)]
    public ushort Port { get; }

    [Option("working-directory", Default = null, Required = false)]
    public string WorkingDirectory { get; }

    [Option('p', "plugin-directory", Default = null, Required = false)]
    public IEnumerable<string> PluginDirectories { get; }

    public ushort ValidPort(ushort defaultPort)
    {
        return PortHelper.IsValidPort(Port) ? Port : defaultPort;
    }
}