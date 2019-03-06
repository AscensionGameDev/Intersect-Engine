using Intersect.Enums;
using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Core.CommandParsing.Arguments;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Database.PlayerData.Security;
using Intersect.Server.Localization;
using JetBrains.Annotations;

namespace Intersect.Server.Core.Commands
{
    internal sealed class PowerAccountCommand : TargetUserCommand
    {
        [NotNull]
        private VariableArgument<Access> Power => FindArgumentOrThrow<VariableArgument<Access>>();

        public PowerAccountCommand() : base(
            Strings.Commands.PowerAccount,
            Strings.Commands.Arguments.TargetPowerAccount,
            new VariableArgument<Access>(Strings.Commands.Arguments.Power, RequiredIfNotHelp, true)
        )
        {
        }

        protected override void HandleTarget(ServerContext context, ParserResult result, User target)
        {
            var power = result.Find(Power);
            if (LegacyDatabase.SetPlayerPower(target, power.AsUserRights()))
            {
                Console.WriteLine($@"    {Strings.Commandoutput.powerchanged.ToString(target?.Name)}");
            }
        }
    }
}