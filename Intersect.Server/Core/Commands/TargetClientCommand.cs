using Intersect.Localization;
using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Core.CommandParsing.Arguments;
using Intersect.Server.General;
using Intersect.Server.Networking;
using JetBrains.Annotations;
using System;

namespace Intersect.Server.Core.Commands
{
    internal abstract class TargetClientCommand : TargettedCommand<Client>
    {
        [NotNull]
        private MessageArgument Message => FindArgumentOrThrow<MessageArgument>();

        protected TargetClientCommand(
            [NotNull] LocaleCommand command,
            [NotNull] LocaleArgument argument
        ) : base(command, argument)
        {
        }

        protected override Client FindTarget([NotNull] ServerContext context, [CanBeNull] string targetName)
        {
            if (string.IsNullOrWhiteSpace(targetName))
            {
                return null;
            }

            return Globals.Clients.Find(client =>
            {
                var playerName = client?.Entity?.Name;
                return string.Equals(playerName, targetName, StringComparison.OrdinalIgnoreCase);
            });
        }
    }
}