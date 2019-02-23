using Intersect.Server.Core.Arguments;
using Intersect.Server.Localization;
using JetBrains.Annotations;
using System;

namespace Intersect.Server.Core.Commands
{
    internal sealed class ExitCommand : ServerCommand
    {
        [NotNull]
        private HelpArgument Help => FindArgument<HelpArgument>() ?? throw new InvalidOperationException($@"Unable to find argument type {typeof(HelpArgument).FullName}.");

        public ExitCommand() : base(
            Strings.Commands.Exit,
            new HelpArgument()
        )
        {
        }

        protected override void Handle(ServerContext context, ParserResult result)
        {
            var help = result.Parsed.Find(Help);
            if (help)
            {
                Console.WriteLine(@"    " + Strings.Commands.Exit.Usage.ToString(Strings.Commands.commandinfo));
                Console.WriteLine(@"    " + Strings.Commands.Exit.Description);
            }
            else if (result.Unhandled.IsEmpty)
            {
                context.RequestShutdown();
            }
            else
            {
                Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
            }
        }
    }
}