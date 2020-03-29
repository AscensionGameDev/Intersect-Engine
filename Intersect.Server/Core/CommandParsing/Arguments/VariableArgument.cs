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
            bool allowsMultiple = false,
            TValue defaultValue = default(TValue)
        ) : base(localization, required, positional, allowsMultiple, defaultValue)
        {
        }

        public VariableArgument(
            [NotNull] LocaleArgument localization,
            [NotNull] ArgumentRequiredPredicate requiredPredicate,
            bool positional = false,
            bool allowsMultiple = false,
            TValue defaultValue = default(TValue)
        ) : base(localization, requiredPredicate, positional, allowsMultiple, defaultValue)
        {
        }

    }

}
