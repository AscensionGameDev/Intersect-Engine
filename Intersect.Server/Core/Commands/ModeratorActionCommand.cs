using Intersect.Localization;
using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Core.CommandParsing.Arguments;
using Intersect.Server.Localization;
using Intersect.Server.Networking;

namespace Intersect.Server.Core.Commands
{

    internal abstract class ModeratorActionCommand : TargetClientCommand
    {

        protected ModeratorActionCommand(
            LocaleCommand command,
            LocaleArgument target,
            LocaleArgument duration,
            LocaleArgument ip,
            LocaleArgument reason
        ) : base(
            command, target, new VariableArgument<int>(duration, RequiredIfNotHelp, true),
            new VariableArgument<bool>(ip, RequiredIfNotHelp, true),
            new VariableArgument<string>(reason, RequiredIfNotHelp, true)
        )
        {
        }

        private VariableArgument<int> Duration => FindArgumentOrThrow<VariableArgument<int>>();

        private VariableArgument<bool> Ip => FindArgumentOrThrow<VariableArgument<bool>>();

        private VariableArgument<string> Reason => FindArgumentOrThrow<VariableArgument<string>>(1);

        protected override void HandleTarget(ServerContext context, ParserResult result, Client target)
        {
            if (target == null)
            {
                Console.WriteLine($@"    {Strings.Player.offline}");

                return;
            }

            var duration = result.Find(Duration);
            var ip = result.Find(Ip);
            var reason = result.Find(Reason) ?? "";

            // TODO: Refactor the global/console messages into ModeratorActionCommand
            HandleClient(context, target, duration, ip, reason);
        }

        protected abstract void HandleClient(
            ServerContext context,
            Client target,
            int duration,
            bool ip,
            string reason
        );

    }

}
