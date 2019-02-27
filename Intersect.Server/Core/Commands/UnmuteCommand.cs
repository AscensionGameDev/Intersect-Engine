using Intersect.Server.Database.PlayerData;
using Intersect.Server.Localization;

namespace Intersect.Server.Core.Commands
{
    internal sealed class UnmuteCommand : TargetUserCommand
    {
        public UnmuteCommand() : base(
            Strings.Commands.Unmute,
            Strings.Commands.Arguments.TargetUnmute
        )
        {
        }

        protected override void HandleTarget(ServerContext context, User target)
        {
            if (target == null)
            {
                Console.WriteLine($@"    {Strings.Account.notfound}");
                return;
            }

            Mute.DeleteMute(target);
            Console.WriteLine($@"    {Strings.Account.unmuted.ToString(target.Name)}");
        }
    }
}