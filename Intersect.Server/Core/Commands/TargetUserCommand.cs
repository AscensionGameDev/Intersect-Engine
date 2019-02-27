using Intersect.Localization;
using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Core.CommandParsing.Arguments;
using Intersect.Server.General;
using Intersect.Server.Networking;
using JetBrains.Annotations;
using System;
using Intersect.Server.Database.PlayerData;

namespace Intersect.Server.Core.Commands
{
    internal abstract class TargetUserCommand : TargettedCommand<User>
    {
        [NotNull]
        private MessageArgument Message => FindArgumentOrThrow<MessageArgument>();

        protected TargetUserCommand(
            [NotNull] LocaleCommand command,
            [NotNull] LocaleArgument argument
        ) : base(command, argument)
        {
        }

        protected override User FindTarget(ServerContext context, string targetName)
        {
            return string.IsNullOrWhiteSpace(targetName) ? null : LegacyDatabase.GetUser(targetName);
        }
    }
}