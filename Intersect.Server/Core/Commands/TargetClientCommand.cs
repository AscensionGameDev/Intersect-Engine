using Intersect.Localization;
using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Core.CommandParsing.Arguments;
using Intersect.Server.General;
using Intersect.Server.Networking;
using JetBrains.Annotations;
using System;

namespace Intersect.Server.Core.Commands
{
    internal abstract class TargetClientCommand : ServerCommand
    {
        [NotNull]
        private MessageArgument Message => FindArgumentOrThrow<MessageArgument>();

        protected TargetClientCommand(
            [NotNull] LocaleCommand command,
            [NotNull] LocaleArgument argument
        ) : base(
            command,
            new MessageArgument(argument, RequiredIfNotHelp, true)
        )
        {
        }

        protected virtual Client FindPlayer([NotNull] ServerContext context, [CanBeNull] string targetName)
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

        protected override void HandleValue(ServerContext context, ParserResult result)
        {
            var target = FindPlayer(context, result.Find(Message));
            HandleTarget(context, target);
        }

        protected abstract void HandleTarget([NotNull] ServerContext context, [CanBeNull] Client target);
    }
}