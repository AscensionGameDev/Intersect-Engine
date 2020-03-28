using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Localization;

namespace Intersect.Server.Core.Commands
{

    internal sealed class UnmuteCommand : TargetUserCommand
    {

        public UnmuteCommand() : base(Strings.Commands.Unmute, Strings.Commands.Arguments.TargetUnmute)
        {
        }

        protected override void HandleTarget(ServerContext context, ParserResult result, User target)
        {
            if (target == null)
            {
                Console.WriteLine($@"    {Strings.Account.notfound.ToString(result.Find(Target))}");

                return;
            }

            Mute.Remove(target);
            Console.WriteLine($@"    {Strings.Account.unmuted.ToString(target.Name)}");
        }

    }

}
