using System;
using Intersect.Enums;
using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Core.CommandParsing.Arguments;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Localization;
using JetBrains.Annotations;

namespace Intersect.Server.Core.Commands
{
    internal class ApiGrantCommand : TargetUserCommand
    {
        [NotNull]
        private VariableArgument<string> Role => FindArgumentOrThrow<VariableArgument<string>>(1);

        public ApiGrantCommand() : base(
            Strings.Commands.ApiGrant,
            Strings.Commands.Arguments.TargetApi,
            new VariableArgument<string>(Strings.Commands.Arguments.ApiRole, RequiredIfNotHelp, true)
        )
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

            var role = result.Find(Role);

            if (!target.Power.Api)
            {
                Console.WriteLine(Strings.Commandoutput.apirolenotgranted.ToString(role, target.Name));
                return;
            }
            
            if (role == "users.query")
            {
                target.Power.ApiRoles.UserQuery = true;
            }
            else if (role == "users.manage")
            {
                if (target.Power.ApiRoles.UserQuery)
                {
                    target.Power.ApiRoles.UserManage = true;
                }
                else
                {
                    Console.WriteLine(Strings.Commandoutput.apiroleprereq.ToString(role, "users.query"));
                    return;
                }
            }
            else
            {
                //Role Not Found
                Console.WriteLine(Strings.Commandoutput.apirolenotfound.ToString(role));
                return;
            }

            DbInterface.SavePlayerDatabaseAsync();

            Console.WriteLine(Strings.Commandoutput.apirolegranted.ToString(target.Name, role));
        }
    }
}
