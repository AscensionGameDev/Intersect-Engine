using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Core.CommandParsing.Arguments;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Database.PlayerData.Security;
using Intersect.Server.Localization;

namespace Intersect.Server.Core.Commands
{

    internal partial class ApiGrantCommand : TargetUserCommand
    {

        public ApiGrantCommand() : base(
            Strings.Commands.ApiGrant, Strings.Commands.Arguments.TargetApi,
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

            if (!target.Power.Api)
            {
                Console.WriteLine(Strings.Commandoutput.ApiRoleNotGranted.ToString(role, target.Name));

                return;
            }

            if (string.Equals(nameof(ApiRoles.UserQuery), role, StringComparison.OrdinalIgnoreCase))
            {
                target.Power.ApiRoles ??= new ApiRoles();
                target.Power.ApiRoles.UserQuery = true;
            }
            else if (string.Equals(nameof(ApiRoles.UserManage), role, StringComparison.OrdinalIgnoreCase))
            {
                target.Power.ApiRoles ??= new ApiRoles();
                if (target.Power.ApiRoles.UserQuery)
                {
                    target.Power.ApiRoles.UserManage = true;
                }
                else
                {
                    Console.WriteLine(Strings.Commandoutput.ApiRolePrerequisite.ToString(role, nameof(ApiRoles.UserQuery)));

                    return;
                }
            }
            else
            {
                //Role Not Found
                Console.WriteLine(Strings.Commandoutput.ApiRoleNotFound.ToString(role));

                return;
            }

            target.Save();

            Console.WriteLine(Strings.Commandoutput.ApiRoleGranted.ToString(target.Name, role));
        }

    }

}
