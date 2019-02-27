using Intersect.Server.Database.PlayerData;
using Intersect.Server.Localization;
using Intersect.Server.Networking;

namespace Intersect.Server.Core.Commands
{
    internal sealed class MuteCommand : ModeratorActionCommand
    {
        public MuteCommand() : base(
            Strings.Commands.Mute,
            Strings.Commands.Arguments.TargetMute,
            Strings.Commands.Arguments.DurationMute,
            Strings.Commands.Arguments.IpMute,
            Strings.Commands.Arguments.ReasonMute
        )
        {
        }

        protected override void HandleClient(ServerContext context, Client target, int duration, bool ip, string reason)
        {
            // TODO: Refactor the global/console messages into ModeratorActionCommand
            Mute.AddMute(target, duration, reason, Strings.Commands.muteuser, ip ? target.GetIp() : "");
            PacketSender.SendGlobalMsg(Strings.Account.muted.ToString(target.Entity?.Name));
            Console.WriteLine($@"    {Strings.Account.muted.ToString(target.Entity?.Name)}");
        }
    }
}