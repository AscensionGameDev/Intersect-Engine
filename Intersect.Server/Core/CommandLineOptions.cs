using CommandLine;
using Intersect.Network;

namespace Intersect.Server.Core
{
    internal class CommandLineOptions
    {
        public CommandLineOptions(bool doNotShowConsole, bool doNotHaltOnError, bool noNatPunchthrough, ushort port)
        {
            DoNotShowConsole = doNotShowConsole;
            DoNotHaltOnError = doNotHaltOnError;
            NoNatPunchthrough = noNatPunchthrough;
            Port = port;
        }

        [Option("no-console", Default = false, Required = false)]
        public bool DoNotShowConsole { get; }

        [Option("no-halt", Default = false, Required = false)]
        public bool DoNotHaltOnError { get; }

        [Option("no-upnp", Default = false, Required = false)]
        public bool NoNatPunchthrough { get; }

        [Option('p', "port", Default = (ushort)0, Required = false)]
        public ushort Port { get; }

        public ushort ValidPort(ushort defaultPort)
        {
            return NetworkHelper.IsValidPort(Port) ? Port : defaultPort;
        }
    }
}
