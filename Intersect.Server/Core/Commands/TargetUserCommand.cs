using Intersect.Localization;
using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Core.CommandParsing.Arguments;
using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData;

using JetBrains.Annotations;

namespace Intersect.Server.Core.Commands
{

    internal abstract class TargetUserCommand : TargettedCommand<User>
    {

        protected TargetUserCommand(
            [NotNull] LocaleCommand command,
            [NotNull] LocaleArgument argument,
            [NotNull] params ICommandArgument[] arguments
        ) : base(command, argument, arguments)
        {
        }

        protected override User FindTarget(ServerContext context, ParserResult result, string targetName)
        {
            return string.IsNullOrWhiteSpace(targetName) ? null : DbInterface.GetUser(targetName);
        }

    }

}
