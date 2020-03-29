using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Localization;

namespace Intersect.Server.Core.Commands
{

    internal sealed class UnbanCommand : TargetUserCommand
    {

        public UnbanCommand() : base(Strings.Commands.Unban, Strings.Commands.Arguments.TargetUnban)
        {
        }

        protected override void HandleTarget(ServerContext context, ParserResult result, User target)
        {
            if (target == null)
            {
                Console.WriteLine($@"    {Strings.Account.notfound.ToString(result.Find(Target))}");

                return;
            }

            Ban.Remove(target);
            Console.WriteLine($@"    {Strings.Account.unbanned.ToString(target.Name)}");
        }

    }

}
