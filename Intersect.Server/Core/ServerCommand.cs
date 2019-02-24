using Intersect.Localization;
using Intersect.Server.Core.Commands;
using JetBrains.Annotations;

namespace Intersect.Server.Core
{
    internal abstract class ServerCommand : HelpableCommand<ServerContext>
    {
        protected ServerCommand(
            [NotNull] LocaleCommand localization,
            [CanBeNull] params ICommandArgument[] arguments
        ) : base(localization, arguments)
        {
        }
    }
}