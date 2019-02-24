using JetBrains.Annotations;
using System;

namespace Intersect.Server.Core
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
    }
}