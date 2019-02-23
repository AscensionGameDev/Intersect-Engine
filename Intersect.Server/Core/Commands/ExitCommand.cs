using Intersect.Server.Core.Arguments;
using Intersect.Server.Localization;
using JetBrains.Annotations;
using System;

namespace Intersect.Server.Core.Commands
{
    internal sealed class ExitCommand : ServerCommand
    {
        [NotNull]
        private HelpArgument Help => FindArgumentOrThrow<HelpArgument>();

        public ExitCommand() : base(
            Strings.Commands.Exit,
            new HelpArgument()
        )
        {
        }

        protected override void Handle(ServerContext context, ParserResult result)
        {
            if (result.Find(Help))
            {
                Console.WriteLine(@"    " + Strings.Commands.Exit.Usage.ToString(Strings.Commands.commandinfo));
                Console.WriteLine(@"    " + Strings.Commands.Exit.Description);
                return;
            }

            context.RequestShutdown();
        }
    }
}