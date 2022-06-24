using Intersect.Localization;

namespace Intersect.Server.Core.CommandParsing.Arguments
{

    public abstract partial class PositionalArgument<TValue> : CommandArgument<TValue>
    {

        protected PositionalArgument(LocaleArgument localization, bool required = false) : base(
            localization, required, true
        )
        {
        }

        public override bool IsFlag => false;

        public override bool AllowsMultiple => false;

    }

    public abstract partial class PositionalArgument : PositionalArgument<object>
    {

        protected PositionalArgument(LocaleArgument localization) : base(localization)
        {
        }

    }

}
