using Intersect.Localization;
using Intersect.Server.Core.CommandParsing.Arguments;
using Intersect.Server.Core.CommandParsing.Commands;

namespace Intersect.Server.Core.Commands
{

    internal abstract class ServerCommand : HelpableCommand<ServerContext>
    {

        protected ServerCommand(
            LocaleCommand localization,
            params ICommandArgument[] arguments
        ) : base(localization, arguments)
        {
        }

    }

}
