using System;

using JetBrains.Annotations;

namespace Intersect.Server.Core.CommandParsing.Arguments
{

    public interface ICommandArgument
    {

        char ShortName { get; }

        [NotNull]
        string Name { get; }

        [NotNull]
        string Description { get; }

        [NotNull]
        Type ValueType { get; }

        [CanBeNull]
        object ValueTypeDefault { get; }

        [CanBeNull]
        object DefaultValue { get; }

        bool HasShortName { get; }

        bool AllowsMultiple { get; }

        bool IsCollection { get; }

        bool IsFlag { get; }

        bool IsRequirementConditional { get; }

        bool IsRequiredByDefault { get; }

        bool IsPositional { get; }

        [CanBeNull]
        string Delimeter { get; }

        bool IsRequired(ParserContext parserContext);

        [CanBeNull]
        TValue DefaultValueAsType<TValue>();

        bool IsValueAllowed([CanBeNull] object value);

    }

}
