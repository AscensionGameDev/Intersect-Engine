using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Localization;
using Intersect.Server.Networking;

namespace Intersect.Server.Core.Commands
{

    internal sealed class KillCommand : TargetClientCommand
    {

        public KillCommand() : base(Strings.Commands.Kill, Strings.Commands.Arguments.TargetKill)
        {
        }

        protected override void HandleTarget(ServerContext context, ParserResult result, Client target)
        {
            if (target?.Entity == null)
            {
                Console.WriteLine($@"    {Strings.Player.offline}");

                return;
            }

            lock (target.Entity)
            {
                target.Entity.Die();
            }
            
            PacketSender.SendGlobalMsg($@"    {Strings.Player.serverkilled.ToString(target.Entity.Name)}");
            Console.WriteLine($@"    {Strings.Commandoutput.killsuccess.ToString(target.Entity.Name)}");
        }

    }

}
