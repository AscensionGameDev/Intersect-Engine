using Intersect.Server.Database.PlayerData;
using Intersect.Server.Localization;
using Intersect.Server.Networking;

namespace Intersect.Server.Core.Commands
{

    internal sealed class BanCommand : ModeratorActionCommand
    {

        public BanCommand() : base(
            Strings.Commands.Ban, Strings.Commands.Arguments.TargetBan, Strings.Commands.Arguments.DurationBan,
            Strings.Commands.Arguments.IpBan, Strings.Commands.Arguments.ReasonBan
        )
        {
        }

        protected override void HandleClient(ServerContext context, Client target, int duration, bool ip, string reason)
        {
            if (target.Entity == null)
            {
                Console.WriteLine($@"    {Strings.Player.offline}");

                return;
            }

            // TODO: Refactor the global/console messages into ModeratorActionCommand
            var name = target.Entity.Name;
            if (string.IsNullOrEmpty(Ban.CheckBan(target.User, "")))
            {
                Ban.Add(target, duration, reason, Strings.Commands.banuser, ip ? target.GetIp() : "");
                target.Disconnect();
                PacketSender.SendGlobalMsg(Strings.Account.banned.ToString(name));
                Console.WriteLine($@"    {Strings.Account.banned.ToString(name)}");
            }
            else
            {
                Console.WriteLine($@"    {Strings.Account.alreadybanned.ToString(name)}");
            }
        }

    }

}
