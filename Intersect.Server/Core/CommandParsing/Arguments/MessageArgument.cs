using Intersect.Localization;
using JetBrains.Annotations;

namespace Intersect.Server.Core.CommandParsing.Arguments
{
    public class MessageArgument : CommandArgument<string>
    {
        public MessageArgument(
            [NotNull] LocaleArgument localization,
            bool required = false,
            bool positional = false,
            bool allowsMultiple = false
        ) : base(localization, required, positional)
        {
            AllowsMultiple = allowsMultiple;
        }

        public MessageArgument(
            [NotNull] LocaleArgument localization,
            [NotNull] ArgumentRequiredPredicate requiredPredicate,
            bool positional = false,
            bool allowsMultiple = false
        ) : base(localization, requiredPredicate, positional)
        {
            AllowsMultiple = allowsMultiple;
        }

        public override bool AllowsMultiple { get; }
    }
}