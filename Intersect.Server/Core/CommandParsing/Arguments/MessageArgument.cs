using Intersect.Localization;
using JetBrains.Annotations;

namespace Intersect.Server.Core.CommandParsing.Arguments
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

        public MessageArgument(
            [NotNull] LocaleArgument localization,
            [NotNull] ArgumentRequiredPredicate requiredPredicate,
            bool positional = false
        ) : base(localization, requiredPredicate, positional)
        {
        }
    }
}