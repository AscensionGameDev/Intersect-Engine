using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Intersect.Server.Core.CommandParsing.Arguments;
using Intersect.Server.Core.CommandParsing.Commands;
using Intersect.Server.Core.CommandParsing.Errors;

namespace Intersect.Server.Core.CommandParsing
{

    public class ParserResult<TCommand> where TCommand : ICommand
    {

        public ParserResult(
            ArgumentValuesMap parsed,
            IEnumerable<ParserError> errors = null,
            IEnumerable<ICommandArgument> missing = null,
            IEnumerable<ICommandArgument> omitted = null
        ) : this(default(TCommand), parsed, errors, missing, omitted)
        {
        }

        public ParserResult(
            TCommand command,
            ArgumentValuesMap parsed,
            IEnumerable<ParserError> errors = null,
            IEnumerable<ICommandArgument> missing = null,
            IEnumerable<ICommandArgument> omitted = null
        )
        {
            Command = command;
            Parsed = parsed;
            Errors = (errors ?? Array.Empty<ParserError>()).ToImmutableList() ?? throw new InvalidOperationException();
            Missing = (missing ?? Array.Empty<ICommandArgument>()).ToImmutableList() ?? throw new InvalidOperationException();
            Omitted = (omitted ?? Array.Empty<ICommandArgument>()).ToImmutableList() ?? throw new InvalidOperationException();
            Unhandled = Errors.Where(error => error is UnhandledArgumentError)
                            .Cast<UnhandledArgumentError>()
                            .ToImmutableList() ??
                        throw new InvalidOperationException();
        }

        public ParserResult(
            TCommand command,
            ParserError error,
            IEnumerable<ICommandArgument> missing = null,
            IEnumerable<ICommandArgument> omitted = null
        ) : this(command, new ArgumentValuesMap(), new[] {error}, missing, omitted)
        {
        }

        public TCommand Command { get; }

        public ArgumentValuesMap Parsed { get; }

        public ImmutableList<UnhandledArgumentError> Unhandled { get; }

        public ImmutableList<ParserError> Errors { get; }

        public ImmutableList<ICommandArgument> Missing { get; }

        public ImmutableList<ICommandArgument> Omitted { get; }

    }

    public class ParserResult : ParserResult<ICommand>
    {

        public ParserResult(
            ArgumentValuesMap parsed,
            IEnumerable<ParserError> errors = null,
            IEnumerable<ICommandArgument> missing = null,
            IEnumerable<ICommandArgument> omitted = null
        ) : base(parsed, errors, missing, omitted)
        {
        }

        public ParserResult(
            ICommand command,
            ArgumentValuesMap parsed,
            IEnumerable<ParserError> errors = null,
            IEnumerable<ICommandArgument> missing = null,
            IEnumerable<ICommandArgument> omitted = null
        ) : base(command, parsed, errors, missing, omitted)
        {
        }

        public ParserResult(
            ICommand command,
            ParserError error,
            IEnumerable<ICommandArgument> missing = null,
            IEnumerable<ICommandArgument> omitted = null
        ) : base(command, error, missing, omitted)
        {
        }

    }

}
