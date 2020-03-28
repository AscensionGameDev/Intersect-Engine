using CommandLine;

using Intersect.Network;

namespace Intersect.Server.Core
{

    internal class CommandLineOptions
    {

        public CommandLineOptions(
            bool doNotShowConsole,
            bool doNotHaltOnError,
            bool noNatPunchthrough,
            bool noNetworkCheck,
            ushort port,
            ushort apiport
        )
        {
            DoNotShowConsole = doNotShowConsole;
            DoNotHaltOnError = doNotHaltOnError;
            NoNatPunchthrough = noNatPunchthrough;
            NoNetworkCheck = noNetworkCheck;
            Port = port;
            ApiPort = apiport;
        }

        [Option('C', "no-console", Default = false, Required = false)]
        public bool DoNotShowConsole { get; }

        [Option('H', "no-halt", Default = false, Required = false)]
        public bool DoNotHaltOnError { get; }

        [Option('U', "no-upnp", Default = false, Required = false)]
        public bool NoNatPunchthrough { get; }

        [Option('P', "no-port-check", Default = false, Required = false)]
        public bool NoNetworkCheck { get; }

        [Option('p', "port", Default = (ushort) 0, Required = false)]
        public ushort Port { get; }

        [Option('a', "apiport", Default = (ushort) 0, Required = false)]
        public ushort ApiPort { get; }

        public ushort ValidPort(ushort defaultPort)
        {
            return NetworkHelper.IsValidPort(Port) ? Port : defaultPort;
        }

        public ushort ValidApiPort(ushort defaultApiPort)
        {
            return NetworkHelper.IsValidPort(defaultApiPort) ? ApiPort : defaultApiPort;
        }

    }

}
