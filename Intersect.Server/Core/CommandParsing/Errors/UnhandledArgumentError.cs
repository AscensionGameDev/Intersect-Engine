using Intersect.Localization;

using JetBrains.Annotations;

namespace Intersect.Server.Core.CommandParsing.Errors
{

    public class UnhandledArgumentError : ParserError
    {

        protected UnhandledArgumentError(
            [NotNull] string commandName,
            [NotNull] string argumentName,
            [NotNull] string message
        ) : base(message)
        {
            CommandName = commandName;
            ArgumentName = argumentName;
        }

        protected UnhandledArgumentError(
            [NotNull] string commandName,
            int position,
            [NotNull] string value,
            [NotNull] string message
        ) : base(message)
        {
            CommandName = commandName;
            Position = position;
            Value = value;
        }

        [NotNull]
        public string CommandName { get; }

        [CanBeNull]
        public string ArgumentName { get; }

        public int Position { get; }

        [CanBeNull]
        public string Value { get; }

        public bool Positional => string.IsNullOrWhiteSpace(ArgumentName);

        [NotNull]
        public static UnhandledArgumentError Create(
            [NotNull] string commandName,
            [NotNull] string argumentName,
            [NotNull] LocalizedString message
        )
        {
            return new UnhandledArgumentError(commandName, argumentName, message.ToString(argumentName, commandName));
        }

        [NotNull]
        public static UnhandledArgumentError Create(
            [NotNull] string commandName,
            int position,
            [NotNull] string argumentValue,
            [NotNull] LocalizedString message
        )
        {
            return new UnhandledArgumentError(
                commandName, position, argumentValue, message.ToString(argumentValue, position, commandName)
            );
        }

    }

}
