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
            Strings.Commands.GlobalExpModifier,
            new VariableArgument<string>(Strings.Commands.Arguments.RateFloat, RequiredIfNotHelp, true)
        )
        {
        }

        [NotNull]
        private VariableArgument<string> Rate => FindArgumentOrThrow<VariableArgument<string>>();

        protected override void HandleValue(ServerContext context, ParserResult result)
        {
            //globalexpmodified
            float _rate = 1.0f;
            if (float.TryParse(result.Find(Rate), out _rate))
            {
                Console.WriteLine($@"    {Strings.Commandoutput.globalexpmodified.ToString(result.Find(Rate))}");
                Options.GlobalEXPModifier = _rate;
                Options.SaveToDisk();
            } else
            {
                Console.WriteLine($@"    {Strings.Commandoutput.globalexpmodifiedinvalid.ToString(result.Find(Rate))}");
            }
        }

    }

}
