using Intersect.Localization;
using JetBrains.Annotations;

namespace Intersect.Server.Core
{
    internal abstract class ServerCommand : Command<ServerContext>
    {
        protected ServerCommand(
            [NotNull] LocaleCommand localization,
            [CanBeNull] params ICommandArgument[] arguments
        ) : base(localization, arguments)
        {
        }
    }
}