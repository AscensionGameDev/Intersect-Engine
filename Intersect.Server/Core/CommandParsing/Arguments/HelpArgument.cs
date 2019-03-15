using Intersect.Server.Localization;

namespace Intersect.Server.Core.CommandParsing.Arguments
{
    public class HelpArgument : CommandArgument<bool>
    {
        public override bool IsFlag => true;

        public HelpArgument() : base(Strings.Commands.Arguments.Help)
        {
        }
    }
}