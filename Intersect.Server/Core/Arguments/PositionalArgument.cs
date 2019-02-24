using Intersect.Localization;
using JetBrains.Annotations;

namespace Intersect.Server.Core.Arguments
{
    public abstract class PositionalArgument<TValue> : CommandArgument<TValue>
    {
        public override bool IsFlag => false;

        public override bool AllowsMultiple => false;

        protected PositionalArgument([NotNull] LocaleArgument localization, bool required = false)
            : base(localization, required, true)
        {
        }
    }

    public abstract class PositionalArgument : PositionalArgument<object>
    {
        protected PositionalArgument([NotNull] LocaleArgument localization)
            : base(localization)
        {
        }
    }
}