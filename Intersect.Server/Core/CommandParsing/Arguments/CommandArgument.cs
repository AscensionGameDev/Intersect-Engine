using System;

using Intersect.Localization;

namespace Intersect.Server.Core.CommandParsing.Arguments
{

    public delegate bool ArgumentRequiredPredicate(ParserContext parserContext);

    public abstract class CommandArgument<TValue> : ICommandArgument
    {

        private readonly ArgumentRequiredPredicate mRequiredPredicate;

        protected CommandArgument(
            LocaleArgument localization,
            bool required = false,
            bool positional = false,
            bool allowsMultiple = false,
            TValue defaultValue = default(TValue)
        )
        {
            Localization = localization;
            IsPositional = positional;
            IsRequiredByDefault = required;
            AllowsMultiple = allowsMultiple;
            DefaultValue = defaultValue;
        }

        protected CommandArgument(
            LocaleArgument localization,
            ArgumentRequiredPredicate requiredPredicate,
            bool positional = false,
            bool allowsMultiple = false,
            TValue defaultValue = default(TValue)
        )
        {
            Localization = localization;
            IsPositional = positional;
            IsRequiredByDefault = true;
            mRequiredPredicate = requiredPredicate;
            AllowsMultiple = allowsMultiple;
            DefaultValue = defaultValue;
        }

        public LocaleArgument Localization { get; }

        public char ShortName => Localization.ShortName;

        public string Name => Localization.Name;

        public string Description => Localization.Description;

        public Type ValueType => typeof(TValue);

        public object ValueTypeDefault => default(TValue);

        public object DefaultValue { get; }

        public bool HasShortName => ShortName != '\0';

        public virtual bool IsFlag => false;

        public virtual bool AllowsMultiple { get; }

        public virtual bool IsCollection => false;

        public string Delimeter { get; protected set; }

        public bool IsRequirementConditional => mRequiredPredicate != null;

        public bool IsRequiredByDefault { get; }

        public bool IsRequired(ParserContext parserContext)
        {
            return mRequiredPredicate?.Invoke(parserContext) ?? IsRequiredByDefault;
        }

        public bool IsPositional { get; }

        public TDefaultValue DefaultValueAsType<TDefaultValue>()
        {
            return DefaultValue == null ? default(TDefaultValue) : (TDefaultValue) DefaultValue;
        }

        public virtual bool IsValueAllowed(object value)
        {
            return true;
        }

        public TValue DefaultValueAsType()
        {
            return DefaultValueAsType<TValue>();
        }

    }

    public abstract class ArrayCommandArgument<TValue> : CommandArgument<TValue>
    {

        protected ArrayCommandArgument(
            LocaleArgument localization,
            int count,
            string delimeter = null
        ) : base(localization)
        {
            if (count < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            Count = count;
            Delimeter = delimeter;
        }

        public int Count { get; }

        public override bool AllowsMultiple => Count > 1;

        public override bool IsCollection => true;

    }

    public abstract class CommandArgument : CommandArgument<object>
    {

        protected CommandArgument(LocaleArgument localization) : base(localization)
        {
        }

    }

}
