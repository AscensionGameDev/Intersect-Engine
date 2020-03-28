using System;

using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Localization;

namespace Intersect.Server.Core.Commands
{

    internal class ApiRolesCommand : TargetUserCommand
    {

        public ApiRolesCommand() : base(Strings.Commands.ApiRoles, Strings.Commands.Arguments.TargetApi)
        {
        }

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

            Console.WriteLine(Strings.Commandoutput.apiroles.ToString(target.Name));
            Console.WriteLine("users.query: " + target.Power.ApiRoles.UserQuery);
            Console.WriteLine("users.manage: " + target.Power.ApiRoles.UserManage);
        }

    }

}
