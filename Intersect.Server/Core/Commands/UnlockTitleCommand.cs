using Intersect.Framework.Core.GameObjects.Titles;
using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Core.CommandParsing.Arguments;
using Intersect.Server.Localization;
using Intersect.Server.Networking;

namespace Intersect.Server.Core.Commands
{

    internal sealed partial class UnlockTitleCommand : TargetClientCommand
    {

        public UnlockTitleCommand() : base(
            Strings.Commands.UnlockTitle, Strings.Commands.Arguments.TargetTitle,
            new VariableArgument<string>(Strings.Commands.Arguments.TitleName, RequiredIfNotHelp, true)
        )
        {
        }

        // Index 1: the target player name (from TargettedCommand) is also a VariableArgument<string> at index 0.
        private VariableArgument<string> TitleNameArgument => FindArgumentOrThrow<VariableArgument<string>>(1);

        protected override void HandleTarget(ServerContext context, ParserResult result, Client target)
        {
            if (target?.Entity == null)
            {
                Console.WriteLine($@"    {Strings.Player.Offline}");

                return;
            }

            var titleName = result.Find(TitleNameArgument);
            var title = TitleDescriptor.Lookup.Values.OfType<TitleDescriptor>().FirstOrDefault(
                candidate => string.Equals(candidate.Name, titleName, StringComparison.OrdinalIgnoreCase)
            );
            if (title == null)
            {
                Console.WriteLine($@"    {Strings.Commandoutput.TitleNotFound.ToString(titleName)}");

                return;
            }

            var player = target.Entity;
            if (!player.UnlockTitle(title.Id))
            {
                Console.WriteLine($@"    {Strings.Commandoutput.TitleAlreadyUnlocked.ToString(player.Name, title.Name)}");

                return;
            }

            player.User?.Save();
            Console.WriteLine($@"    {Strings.Commandoutput.TitleUnlocked.ToString(player.Name, title.Name)}");
        }

    }

}
