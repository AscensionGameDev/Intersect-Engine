using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Localization;

namespace Intersect.Server.Core.Commands
{

    internal sealed partial class ExitCommand : ServerCommand
    {

        public ExitCommand() : base(Strings.Commands.Exit)
        {
        }

        protected override void HandleValue(ServerContext context, ParserResult result)
        {
            context.RequestShutdown();
        }

    }

}
