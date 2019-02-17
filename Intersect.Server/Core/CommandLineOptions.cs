using CommandLine;

namespace Intersect.Server.Core
{
    internal class CommandLineOptions
    {
        public CommandLineOptions(bool doNotShowConsole, bool doNotHaltOnError)
        {
            DoNotShowConsole = doNotShowConsole;
            DoNotHaltOnError = doNotHaltOnError;
        }

        [Option("no-console", Default = false, Required = false)]
        public bool DoNotShowConsole { get; }

        [Option("no-halt", Default = false, Required = false)]
        public bool DoNotHaltOnError { get; }
    }
}
