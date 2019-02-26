using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Localization;

namespace Intersect.Server.Core.Commands
{
    internal sealed class HelpCommand : ServerCommand
    {
        public HelpCommand() : base(
            Strings.Commands.Help
        )
        {
        }

        protected override void HandleValue(ServerContext context, ParserResult result)
        {
            Console.WriteLine(@"    " + Strings.Commandoutput.helpheader);

            Strings.Commands.CommandList.ForEach(command =>
            {
                Console.WriteLine($@"    {command?.Name,-20} - {command?.Help}");
            });

            Console.WriteLine($@"    {Strings.Commandoutput.helpfooter.ToString(Strings.Commands.commandinfo)}");
        }
    }
}