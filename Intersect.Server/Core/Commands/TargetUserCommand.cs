using Intersect.Localization;
using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Core.CommandParsing.Arguments;
using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData;

namespace Intersect.Server.Core.Commands
{

    internal abstract class TargetUserCommand : TargettedCommand<User>
    {

        protected TargetUserCommand(
            LocaleCommand command,
            LocaleArgument argument,
            params ICommandArgument[] arguments
        ) : base(command, argument, arguments)
        {
        }

        protected override User FindTarget(ServerContext context, ParserResult result, string targetName)
        {
            return string.IsNullOrWhiteSpace(targetName) ? null : User.Find(targetName);
        }

    }

}
