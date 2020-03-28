using Intersect.Server.Localization;

namespace Intersect.Server.Core.CommandParsing.Arguments
{

    public class HelpArgument : CommandArgument<bool>
    {

        public HelpArgument() : base(Strings.Commands.Arguments.Help)
        {
        }

        public override bool IsFlag => true;

    }

}
