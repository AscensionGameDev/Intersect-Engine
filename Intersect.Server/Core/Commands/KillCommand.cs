using System;
using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Core.CommandParsing.Arguments;
using Intersect.Server.General;
using Intersect.Server.Localization;
using Intersect.Server.Networking;
using JetBrains.Annotations;

namespace Intersect.Server.Core.Commands
{
    internal sealed class KillCommand : ServerCommand
    {
        [NotNull]
        private MessageArgument Message => FindArgumentOrThrow<MessageArgument>();

        public KillCommand() : base(
            Strings.Commands.Kill,
            new MessageArgument(Strings.Commands.Arguments.KillName, RequiredIfNotHelp, true)
        )
        {
        }

        protected override void HandleValue(ServerContext context, ParserResult result)
        {
            var target = Globals.Clients.Find(client =>
                string.Equals(client?.Entity?.Name, result.Find(Message), StringComparison.OrdinalIgnoreCase));
            if (target != null)
            {
                target.Entity?.Die();
                PacketSender.SendGlobalMsg($@"    {Strings.Player.serverkilled.ToString(target.Entity?.Name)}");
                Console.WriteLine($@"    {Strings.Commandoutput.killsuccess.ToString(target.Entity?.Name)}");
            }
            else
            {
                Console.WriteLine(@"    " + Strings.Player.offline);
            }
        }
    }
}