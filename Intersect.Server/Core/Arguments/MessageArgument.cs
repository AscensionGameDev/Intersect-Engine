using Intersect.Localization;
using JetBrains.Annotations;

namespace Intersect.Server.Core.Arguments
{
    public class MessageArgument : CommandArgument<string>
    {
        public MessageArgument(
            [NotNull] LocaleArgument localization,
            bool required = false,
            bool positional = false
        ) : base(localization, required, positional)
        {
        }
    }
}