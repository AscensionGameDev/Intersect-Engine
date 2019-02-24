using Intersect.Server.Core.Arguments;
using Intersect.Server.Localization;
using JetBrains.Annotations;

namespace Intersect.Server.Core.Commands
{
    internal sealed class AnnouncementCommand : ServerCommand
    {
        [NotNull]
        private HelpArgument Help => FindArgumentOrThrow<HelpArgument>();

        public AnnouncementCommand() : base(
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