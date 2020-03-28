using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Localization;
using Intersect.Server.Networking;

namespace Intersect.Server.Core.Commands
{

    internal sealed class KickCommand : TargetClientCommand
    {

        public KickCommand() : base(Strings.Commands.Kick, Strings.Commands.Arguments.TargetKick)
        {
        }

        protected override void HandleTarget(ServerContext context, ParserResult result, Client target)
        {
            if (target?.Entity == null)
            {
                Console.WriteLine($@"    {Strings.Player.offline}");

                return;
            }

            var name = target.Entity.Name;
            target.Disconnect();
            PacketSender.SendGlobalMsg(Strings.Player.serverkicked.ToString(name));
            Console.WriteLine($@"    {Strings.Player.serverkicked.ToString(name)}");
        }

    }

}
