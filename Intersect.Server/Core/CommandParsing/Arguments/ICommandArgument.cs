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
        Type ValueType { get; }

        [CanBeNull]
        object ValueTypeDefault { get; }

        [CanBeNull]
        object DefaultValue { get; }

        bool AllowsMultiple { get; }

        bool IsCollection { get; }

        bool IsFlag { get; }

        bool IsRequired(ParserContext parserContext);

        bool IsPositional { get; }

        [CanBeNull]
        string Delimeter { get; }

        [CanBeNull]
        TValue DefaultValueAsType<TValue>();

        bool IsValueAllowed([CanBeNull] object value);
    }
}