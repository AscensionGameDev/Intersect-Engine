using Intersect.Localization;
using JetBrains.Annotations;
using System;

namespace Intersect.Server.Core
{
    public abstract class CommandArgument<TValue> : ICommandArgument
    {
        public char ShortName => Localization.ShortName;

        public string Name => Localization.Name;

        public Type ValueType => typeof(TValue);

        public object ValueTypeDefault => default(TValue);

        public object DefaultValue => default(TValue);

        public virtual bool IsFlag => false;

        public virtual bool AllowsMultiple => false;

        public virtual bool IsCollection => false;

        public string Delimeter { get; protected set; }

        public bool IsRequired { get; }

        [NotNull]
        public LocaleArgument Localization { get; }

        protected CommandArgument([NotNull] LocaleArgument localization, bool required = false)
        {
            Localization = localization;
            IsRequired = required;
        }

        public TValue DefaultValueAsType()
        {
            return DefaultValueAsType<TValue>();
        }

        public TDefaultValue DefaultValueAsType<TDefaultValue>()
        {
            return DefaultValue == null ? default(TDefaultValue) : (TDefaultValue)DefaultValue;
        }
    }

    public abstract class ArrayCommandArgument<TValue> : CommandArgument<TValue>
    {
        public override bool AllowsMultiple => true;

        public override bool IsCollection => true;

        protected ArrayCommandArgument(
            [NotNull] LocaleArgument localization,
            [CanBeNull] string delimeter = null
        ) : base(localization)
        {
            Delimeter = delimeter;
        }
    }

    public abstract class CommandArgument : CommandArgument<object>
    {
        protected CommandArgument([NotNull] LocaleArgument localization)
            : base(localization)
        {
        }
    }
}