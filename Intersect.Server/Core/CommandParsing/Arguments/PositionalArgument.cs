using Intersect.Localization;

using JetBrains.Annotations;

namespace Intersect.Server.Core.CommandParsing.Arguments
{

    public abstract class PositionalArgument<TValue> : CommandArgument<TValue>
    {

        protected PositionalArgument([NotNull] LocaleArgument localization, bool required = false) : base(
            localization, required, true
        )
        {
        }

        public override bool IsFlag => false;

        public override bool AllowsMultiple => false;

    }

    public abstract class PositionalArgument : PositionalArgument<object>
    {

        protected PositionalArgument([NotNull] LocaleArgument localization) : base(localization)
        {
        }

    }

}
