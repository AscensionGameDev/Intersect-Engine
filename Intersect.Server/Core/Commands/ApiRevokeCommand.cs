using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Core.CommandParsing.Arguments;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Database.PlayerData.Security;
using Intersect.Server.Localization;

namespace Intersect.Server.Core.Commands
{

    internal partial class ApiRevokeCommand : TargetUserCommand
    {

        public ApiRevokeCommand() : base(
            Strings.Commands.ApiRevoke, Strings.Commands.Arguments.TargetApi,
            new VariableArgument<string>(Strings.Commands.Arguments.ApiRole, RequiredIfNotHelp, true)
        )
        {
        }

        private VariableArgument<string> Role => FindArgumentOrThrow<VariableArgument<string>>(1);

        protected override void HandleTarget(ServerContext context, ParserResult result, User target)
        {
            if (target == null)
            {
                Console.WriteLine($@"    {Strings.Account.NotFound}");

                return;
            }

            if (target.Power == null)
            {
                throw new ArgumentNullException(nameof(target.Power));
            }

            var role = result.Find(Role);

            if (string.Equals(nameof(ApiRoles.UserQuery), role, StringComparison.OrdinalIgnoreCase))
            {
                target.Power.ApiRoles = null;
            }
            else if (string.Equals(nameof(ApiRoles.UserManage), role, StringComparison.OrdinalIgnoreCase))
            {
                if (target.Power.ApiRoles != default)
                {
                    target.Power.ApiRoles.UserManage = false;
                }
            }
            else
            {
                //Role Not Found
                Console.WriteLine(Strings.Commandoutput.ApiRoleNotFound.ToString(role));

                return;
            }

            target.Save();

            Console.WriteLine(Strings.Commandoutput.ApiRoleRevoked.ToString(target.Name, role));
        }

    }

}
