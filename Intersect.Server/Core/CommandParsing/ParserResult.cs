using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Intersect.Server.Core.CommandParsing.Arguments;
using Intersect.Server.Core.CommandParsing.Commands;
using Intersect.Server.Core.CommandParsing.Errors;

using JetBrains.Annotations;

namespace Intersect.Server.Core.CommandParsing
{

    public class ParserResult<TCommand> where TCommand : ICommand
    {

        public ParserResult(
            [NotNull] ArgumentValuesMap parsed,
            [CanBeNull] IEnumerable<ParserError> errors = null,
            [CanBeNull] IEnumerable<ICommandArgument> missing = null,
            [CanBeNull] IEnumerable<ICommandArgument> omitted = null
        ) : this(default(TCommand), parsed, errors, missing, omitted)
        {
        }

        public ParserResult(
            [CanBeNull] TCommand command,
            [NotNull] ArgumentValuesMap parsed,
            [CanBeNull] IEnumerable<ParserError> errors = null,
            [CanBeNull] IEnumerable<ICommandArgument> missing = null,
            [CanBeNull] IEnumerable<ICommandArgument> omitted = null
        )
        {
            Command = command;
            Parsed = parsed;
            Errors = (errors ?? new ParserError[0]).ToImmutableList() ?? throw new InvalidOperationException();
            Missing = (missing ?? new ICommandArgument[0]).ToImmutableList() ?? throw new InvalidOperationException();
            Omitted = (omitted ?? new ICommandArgument[0]).ToImmutableList() ?? throw new InvalidOperationException();
            Unhandled = Errors.Where(error => error is UnhandledArgumentError)
                            .Cast<UnhandledArgumentError>()
                            .ToImmutableList() ??
                        throw new InvalidOperationException();
        }

        public ParserResult(
            [CanBeNull] TCommand command,
            [NotNull] ParserError error,
            [CanBeNull] IEnumerable<ICommandArgument> missing = null,
            [CanBeNull] IEnumerable<ICommandArgument> omitted = null
        ) : this(command, new ArgumentValuesMap(), new[] {error}, missing, omitted)
        {
        }

        [CanBeNull]
        public TCommand Command { get; }

        [NotNull]
        public ArgumentValuesMap Parsed { get; }

        [NotNull]
        public ImmutableList<UnhandledArgumentError> Unhandled { get; }

        [NotNull]
        public ImmutableList<ParserError> Errors { get; }

        [NotNull]
        public ImmutableList<ICommandArgument> Missing { get; }

        [NotNull]
        public ImmutableList<ICommandArgument> Omitted { get; }

    }

    public class ParserResult : ParserResult<ICommand>
    {

        public ParserResult(
            [NotNull] ArgumentValuesMap parsed,
            [CanBeNull] IEnumerable<ParserError> errors = null,
            [CanBeNull] IEnumerable<ICommandArgument> missing = null,
            [CanBeNull] IEnumerable<ICommandArgument> omitted = null
        ) : base(parsed, errors, missing, omitted)
        {
        }

        public ParserResult(
            [CanBeNull] ICommand command,
            [NotNull] ArgumentValuesMap parsed,
            [CanBeNull] IEnumerable<ParserError> errors = null,
            [CanBeNull] IEnumerable<ICommandArgument> missing = null,
            [CanBeNull] IEnumerable<ICommandArgument> omitted = null
        ) : base(command, parsed, errors, missing, omitted)
        {
        }

        public ParserResult(
            [CanBeNull] ICommand command,
            [NotNull] ParserError error,
            [CanBeNull] IEnumerable<ICommandArgument> missing = null,
            [CanBeNull] IEnumerable<ICommandArgument> omitted = null
        ) : base(command, error, missing, omitted)
        {
        }

    }

}
