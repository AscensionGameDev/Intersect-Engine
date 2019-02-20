using Intersect.Server.Localization;

namespace Intersect.Server.Core.Commands
{
    internal sealed class ExitCommand : ServerCommand
    {
        public ExitCommand() : base(Strings.Commands.Exit)
        {
        }

        public override void Handle(ServerContext context, HelpArguments arguments)
        {
            if (arguments.Help)
            {
                Console.WriteLine(@"    " + Strings.Commands.Exit.Usage.ToString(Strings.Commands.commandinfo));
                Console.WriteLine(@"    " + Strings.Commands.Exit.Description);
            } else if (arguments.UnknownArguments.IsEmpty)
            {
                context.Dispose();
            }
            else
            {
                Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
            }
        }
    }
}
