using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.Localization;
using JetBrains.Annotations;

namespace Intersect.Server.Core.CommandParsing.Arguments
{
    public class EnumArgument<TValue> : CommandArgument<TValue>
    {
        public ImmutableArray<TValue> AllowedValues { get; }

        public EnumArgument(
            [NotNull] LocaleArgument localization,
            bool required = false,
            bool positional = false,
            [CanBeNull] params TValue[] allowedValues
        ) : base(localization, required, positional)
        {
            AllowedValues = (allowedValues ?? new TValue[0]).ToImmutableArray();
        }

        public EnumArgument(
            [NotNull] LocaleArgument localization,
            bool required = false,
            bool positional = false,
            bool allowsMultiple = false,
            [CanBeNull] params TValue[] allowedValues
        ) : base(localization, required, positional, allowsMultiple)
        {
            AllowedValues = (allowedValues ?? new TValue[0]).ToImmutableArray();
        }

        public EnumArgument(
            [NotNull] LocaleArgument localization,
            bool required = false,
            bool positional = false,
            bool allowsMultiple = false,
            TValue defaultValue = default(TValue),
            [CanBeNull] params TValue[] allowedValues
        ) : base(localization, required, positional, allowsMultiple, defaultValue)
        {
            AllowedValues = (allowedValues ?? new TValue[0]).ToImmutableArray();
        }

        public EnumArgument(
            [NotNull] LocaleArgument localization,
            [NotNull] ArgumentRequiredPredicate requiredPredicate,
            bool positional = false,
            [CanBeNull] params TValue[] allowedValues
        ) : base(localization, requiredPredicate, positional)
        {
            AllowedValues = (allowedValues ?? new TValue[0]).ToImmutableArray();
        }

        public EnumArgument(
            [NotNull] LocaleArgument localization,
            [NotNull] ArgumentRequiredPredicate requiredPredicate,
            bool positional = false,
            bool allowsMultiple = false,
            [CanBeNull] params TValue[] allowedValues
        ) : base(localization, requiredPredicate, positional, allowsMultiple)
        {
            AllowedValues = (allowedValues ?? new TValue[0]).ToImmutableArray();
        }

        public EnumArgument(
            [NotNull] LocaleArgument localization,
            [NotNull] ArgumentRequiredPredicate requiredPredicate,
            bool positional = false,
            bool allowsMultiple = false,
            TValue defaultValue = default(TValue),
            [CanBeNull] params TValue[] allowedValues
        ) : base(localization, requiredPredicate, positional, allowsMultiple, defaultValue)
        {
            AllowedValues = (allowedValues ?? new TValue[0]).ToImmutableArray();
        }

        public override bool IsValueAllowed(object value)
        {
            return value is TValue castedValue && AllowedValues.Contains(castedValue);
        }
    }
}
