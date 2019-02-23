using System.Collections.Generic;
using System.Collections.Immutable;
using Intersect.Server.Core.Errors;
using JetBrains.Annotations;

namespace Intersect.Server.Core
{
    public class ParserResult<TCommand>
        where TCommand : ICommand
    {
        [CanBeNull]
        public TCommand Command { get; }

        [NotNull]
        public ArgumentValuesMap Parsed { get; }

        [NotNull]
        public ArgumentValues Unhandled { get; }

        public ImmutableList<ParserError> Errors { get; }

        public ParserResult(
            [NotNull] ArgumentValuesMap parsed,
            [CanBeNull] ArgumentValues unhandled = null,
            [CanBeNull] IEnumerable<ParserError> errors = null
        ) : this(default(TCommand), parsed, unhandled)
        {
        }

        public ParserResult(
            [CanBeNull] TCommand command,
            [NotNull] ArgumentValuesMap parsed,
            [CanBeNull] ArgumentValues unhandled = null,
            [CanBeNull] IEnumerable<ParserError> errors = null
        )
        {
            Command = command;
            Parsed = parsed;
            Unhandled = unhandled ?? new ArgumentValues();
            Errors = errors?.ToImmutableList() ?? ImmutableList.Create<ParserError>();
        }

        public ParserResult(
            [CanBeNull] TCommand command,
            [NotNull] ParserError error
        ) : this(command, new ArgumentValuesMap(), null, new [] {error})
        {
        }
    }

    public class ParserResult
        : ParserResult<ICommand>
    {
        public ParserResult(
            [NotNull] ArgumentValuesMap parsed,
            [CanBeNull] ArgumentValues unhandled = null,
            [CanBeNull] IEnumerable<ParserError> errors = null
        ) : base(parsed, unhandled, errors)
        {
        }

        public ParserResult(
            [CanBeNull] ICommand command,
            [NotNull] ArgumentValuesMap parsed,
            [CanBeNull] ArgumentValues unhandled = null,
            [CanBeNull] IEnumerable<ParserError> errors = null
        ) : base(command, parsed, unhandled, errors)
        {
        }

        public ParserResult(
            [CanBeNull] ICommand command,
            [NotNull] ParserError error
        ) : base(command, error)
        {
        }
    }
}