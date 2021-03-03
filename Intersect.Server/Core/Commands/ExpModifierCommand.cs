using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Core.CommandParsing.Arguments;
using Intersect.Server.Localization;
using Intersect.Server.Networking;

using JetBrains.Annotations;

namespace Intersect.Server.Core.Commands
{

    internal sealed class ExpModifierCommand : ServerCommand
    {

        public ExpModifierCommand() : base(
            Strings.Commands.Announcement,
            new VariableArgument<string>(Strings.Commands.Arguments.AnnouncementMessage, RequiredIfNotHelp, true)
        )
        {
        }

        [NotNull]
        private VariableArgument<string> Rate => FindArgumentOrThrow<VariableArgument<string>>();

        protected override void HandleValue(ServerContext context, ParserResult result)
        {
            //globalexpmodified
            Console.WriteLine($@"    {Strings.Commandoutput.globalexpmodified.ToString(Rate.ToString())}");
            Options.GlobalEXPModifier = float.Parse(result.Find(Rate));
            Options.SaveToDisk();
        }

    }

}
