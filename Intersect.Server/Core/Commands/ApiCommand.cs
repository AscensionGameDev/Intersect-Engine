using System;

using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Core.CommandParsing.Arguments;
using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Localization;

using JetBrains.Annotations;

namespace Intersect.Server.Core.Commands
{

    internal class ApiCommand : TargetUserCommand
    {

        public ApiCommand() : base(
            Strings.Commands.Api, Strings.Commands.Arguments.TargetApi,
            new VariableArgument<bool>(Strings.Commands.Arguments.AccessBoolean, RequiredIfNotHelp, true)
        )
        {
        }

        [NotNull]
        private VariableArgument<bool> Access => FindArgumentOrThrow<VariableArgument<bool>>();

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

            var access = result.Find(Access);
            target.Power.Api = access;
            if (!access)
            {
                target.Power.ApiRoles = new Database.PlayerData.Security.ApiRoles();
            }

            DbInterface.SavePlayerDatabaseAsync();

            Console.WriteLine(
                access
                    ? $@"    {Strings.Commandoutput.apigranted.ToString(target.Name)}"
                    : $@"    {Strings.Commandoutput.apirevoked.ToString(target.Name)}"
            );
        }

    }

}
