using System;
using System.Collections.Immutable;

using Intersect.Server.Core.CommandParsing.Commands;

namespace Intersect.Server.Core.CommandParsing.Errors
{

    public class ParserError
    {

        public ParserError(string message = null, bool fatal = true)
        {
            IsFatal = fatal;

            // This should be the only hard-coded English error message. All
            // other English-hardcoded strings belong as the Exception message
            // because Message will be displayed to the end-user (and also
            // logged), while the Exception will only be logged.
            Message = message ?? "Unknown parser error occurred.";
        }

        public ParserError(string message, Exception exception, params string[] arguments) : this(
            message, exception, true, arguments
        )
        {
        }

        public ParserError(string message, Exception exception, bool fatal = true, params string[] arguments) : this(
            message, fatal
        )
        {
            Exception = exception;
            Arguments = arguments?.ToImmutableArray() ?? ImmutableArray.Create<string>();
        }

        public bool IsFatal { get; }

        public string Message { get; }

        public Exception Exception { get; }

        public ImmutableArray<string> Arguments { get; }

        public ParserResult AsResult(ICommand command = null)
        {
            return new ParserResult(command, this);
        }

        public ParserResult<TCommand> AsResult<TCommand>(TCommand command) where TCommand : ICommand
        {
            return new ParserResult<TCommand>(command, this);
        }

        public override string ToString()
        {
            return Message;
        }

    }

}
