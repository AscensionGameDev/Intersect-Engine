using Intersect.Framework.Core.GameObjects.Titles;
using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Core.CommandParsing.Arguments;
using Intersect.Server.Localization;
using Intersect.Server.Networking;

namespace Intersect.Server.Core.Commands
{

    internal sealed partial class EquipTitleCommand : TargetClientCommand
    {

        public EquipTitleCommand() : base(
            Strings.Commands.EquipTitle, Strings.Commands.Arguments.TargetTitle,
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

            var player = target.Entity;
            var titleName = result.Find(TitleNameArgument);

            if (string.Equals(titleName, "none", StringComparison.OrdinalIgnoreCase))
            {
                player.RemoveTitle();
                player.User?.Save();
                Console.WriteLine($@"    {Strings.Commandoutput.TitleRemoved.ToString(player.Name)}");

                return;
            }

            var title = TitleDescriptor.Lookup.Values.OfType<TitleDescriptor>().FirstOrDefault(
                candidate => string.Equals(candidate.Name, titleName, StringComparison.OrdinalIgnoreCase)
            );
            if (title == null)
            {
                Console.WriteLine($@"    {Strings.Commandoutput.TitleNotFound.ToString(titleName)}");

                return;
            }

            if (!player.EquipTitle(title.Id))
            {
                Console.WriteLine($@"    {Strings.Commandoutput.TitleNotUnlocked.ToString(player.Name, title.Name)}");

                return;
            }

            player.User?.Save();
            Console.WriteLine($@"    {Strings.Commandoutput.TitleEquipped.ToString(player.Name, title.Name)}");
        }

    }

}
