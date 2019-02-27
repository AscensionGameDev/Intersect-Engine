using Intersect.Localization;
using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Core.CommandParsing.Arguments;
using Intersect.Server.Localization;
using Intersect.Server.Networking;
using JetBrains.Annotations;
using System;

namespace Intersect.Server.Core.Commands
{
    internal abstract class ModeratorActionCommand : TargetClientCommand
    {
        [NotNull]
        // TODO: Make this into a VariableArgument<int>
        private MessageArgument Duration => FindArgumentOrThrow<MessageArgument>(1);

        [NotNull]
        // TODO: Make this into a VariableArgument<bool>
        private MessageArgument Ip => FindArgumentOrThrow<MessageArgument>(2);

        [NotNull]
        // TODO: Refactor MessageArgument into a VariableArgument<string>
        private MessageArgument Reason => FindArgumentOrThrow<MessageArgument>(3);

        protected ModeratorActionCommand(
            [NotNull] LocaleCommand command,
            [NotNull] LocaleArgument target,
            [NotNull] LocaleArgument duration,
            [NotNull] LocaleArgument ip,
            [NotNull] LocaleArgument reason
        ) : base(
            command,
            target,
            new MessageArgument(duration, RequiredIfNotHelp, true),
            new MessageArgument(ip, RequiredIfNotHelp, true),
            new MessageArgument(reason, RequiredIfNotHelp, true)
        )
        {
        }

        protected override void HandleTarget(ServerContext context, ParserResult result, Client target)
        {
            if (target == null)
            {
                Console.WriteLine(@"    " + Strings.Player.offline);
                return;
            }

            var duration = Convert.ToInt32(result.Find(Duration));
            var ip = string.Equals(result.Find(Ip), Strings.Commands.True, StringComparison.Ordinal);
            var reason = result.Find(Reason) ?? "";
            // TODO: Refactor the global/console messages into ModeratorActionCommand
            HandleClient(context, target, duration, ip, reason);
        }

        protected abstract void HandleClient([NotNull] ServerContext context, [NotNull] Client target, int duration, bool ip, [NotNull] string reason);
    }
}