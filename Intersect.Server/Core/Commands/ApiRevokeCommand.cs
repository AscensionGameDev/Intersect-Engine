using System;

using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Core.CommandParsing.Arguments;
using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Localization;

namespace Intersect.Server.Core.Commands
{

    internal class ApiRevokeCommand : TargetUserCommand
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
                Console.WriteLine($@"    {Strings.Account.notfound}");

                return;
            }

            if (target.Power == null)
            {
                throw new ArgumentNullException(nameof(target.Power));
            }

            var role = result.Find(Role);

            if (role == "users.query")
            {
                target.Power.ApiRoles.UserQuery = false;
                target.Power.ApiRoles.UserManage = false;
            }
            else if (role == "users.manage")
            {
                target.Power.ApiRoles.UserManage = false;
            }
            else
            {
                //Role Not Found
                Console.WriteLine(Strings.Commandoutput.apirolenotfound.ToString(role));

                return;
            }

            target.Save();

            Console.WriteLine(Strings.Commandoutput.apirolerevoked.ToString(target.Name, role));
        }

    }

}
