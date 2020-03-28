using Intersect.Server.Database.PlayerData;
using Intersect.Server.Localization;
using Intersect.Server.Networking;

namespace Intersect.Server.Core.Commands
{

    internal sealed class MuteCommand : ModeratorActionCommand
    {

        public MuteCommand() : base(
            Strings.Commands.Mute, Strings.Commands.Arguments.TargetMute, Strings.Commands.Arguments.DurationMute,
            Strings.Commands.Arguments.IpMute, Strings.Commands.Arguments.ReasonMute
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
            if (string.IsNullOrEmpty(Mute.FindMuteReason(target.User.Id, "")))
            {
                Mute.Add(target, duration, reason, Strings.Commands.muteuser, ip ? target.GetIp() : "");
                PacketSender.SendGlobalMsg(Strings.Account.muted.ToString(name));
                Console.WriteLine($@"    {Strings.Account.muted.ToString(name)}");
            }
            else
            {
                Console.WriteLine($@"    {Strings.Account.alreadymuted.ToString(name)}");
            }
        }

    }

}
