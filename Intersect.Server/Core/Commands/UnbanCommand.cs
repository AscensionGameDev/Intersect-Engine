using Intersect.Server.Database.PlayerData;
using Intersect.Server.Localization;

namespace Intersect.Server.Core.Commands
{
    internal sealed class UnbanCommand : TargetUserCommand
    {
        public UnbanCommand() : base(
            Strings.Commands.Unban,
            Strings.Commands.Arguments.TargetUnban
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

            Ban.DeleteBan(target);
            Console.WriteLine($@"    {Strings.Account.unbanned.ToString(target.Name)}");
        }
    }
}