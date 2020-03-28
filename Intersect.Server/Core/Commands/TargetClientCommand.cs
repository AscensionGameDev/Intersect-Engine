﻿using System;

using Intersect.Localization;
using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Core.CommandParsing.Arguments;
using Intersect.Server.General;
using Intersect.Server.Networking;

using JetBrains.Annotations;

namespace Intersect.Server.Core.Commands
{

    internal abstract class TargetClientCommand : TargettedCommand<Client>
    {

        protected TargetClientCommand(
            [NotNull] LocaleCommand command,
            [NotNull] LocaleArgument argument,
            [NotNull] params ICommandArgument[] arguments
        ) : base(command, argument, arguments)
        {
        }

        protected override Client FindTarget(ServerContext context, ParserResult result, string targetName)
        {
            if (string.IsNullOrWhiteSpace(targetName))
            {
                return null;
            }

            return Globals.Clients.Find(
                client =>
                {
                    var playerName = client?.Entity?.Name;

                    return string.Equals(playerName, targetName, StringComparison.OrdinalIgnoreCase);
                }
            );
        }

    }

}
