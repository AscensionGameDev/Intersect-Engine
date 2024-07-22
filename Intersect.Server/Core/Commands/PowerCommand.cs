﻿using Intersect.Enums;
using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Core.CommandParsing.Arguments;
using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData.Security;
using Intersect.Server.Localization;
using Intersect.Server.Networking;

namespace Intersect.Server.Core.Commands
{

    internal sealed partial class PowerCommand : TargetClientCommand
    {

        public PowerCommand() : base(
            Strings.Commands.Power, Strings.Commands.Arguments.TargetPower,
            new VariableArgument<Access>(Strings.Commands.Arguments.Power, RequiredIfNotHelp, true)
        )
        {
        }

        private VariableArgument<Access> Power => FindArgumentOrThrow<VariableArgument<Access>>();

        protected override void HandleTarget(ServerContext context, ParserResult result, Client target)
        {
            if (target?.Entity == null)
            {
                Console.WriteLine($@"    {Strings.Player.Offline}");

                return;
            }

            if (target.Name == null)
            {
                Console.WriteLine($@"    {Strings.Account.NotFound}");

                return;
            }

            var power = result.Find(Power);
            DbInterface.SetPlayerPower(target.Name, power.AsUserRights());
            PacketSender.SendEntityDataToProximity(target.Entity);
            PacketSender.SendGlobalMsg(
                power != Access.None
                    ? Strings.Player.Admin.ToString(target.Entity.Name)
                    : Strings.Player.Deadmin.ToString(target.Entity.Name)
            );

            Console.WriteLine($@"    {Strings.Commandoutput.PowerChanged.ToString(target.Entity.Name)}");
        }

    }

}
