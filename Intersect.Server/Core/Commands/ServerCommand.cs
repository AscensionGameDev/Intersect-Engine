using Intersect.Localization;
using Intersect.Server.Core.CommandParsing.Arguments;
using Intersect.Server.Core.CommandParsing.Commands;

using JetBrains.Annotations;

namespace Intersect.Server.Core.Commands
{

    internal abstract class ServerCommand : HelpableCommand<ServerContext>
    {

        protected ServerCommand(
            [NotNull] LocaleCommand localization,
            [NotNull] params ICommandArgument[] arguments
        ) : base(localization, arguments)
        {
        }

    }

}
