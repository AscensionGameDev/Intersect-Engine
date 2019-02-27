using Intersect.Localization;
using JetBrains.Annotations;

namespace Intersect.Server.Core.CommandParsing.Arguments
{
    public class VariableArgument<TValue> : CommandArgument<TValue>
    {
        public VariableArgument(
            [NotNull] LocaleArgument localization,
            bool required = false,
            bool positional = false,
            bool allowsMultiple = false
        ) : base(localization, required, positional, allowsMultiple)
        {
        }

        public VariableArgument(
            [NotNull] LocaleArgument localization,
            [NotNull] ArgumentRequiredPredicate requiredPredicate,
            bool positional = false,
            bool allowsMultiple = false
        ) : base(localization, requiredPredicate, positional, allowsMultiple)
        {
        }
    }
}