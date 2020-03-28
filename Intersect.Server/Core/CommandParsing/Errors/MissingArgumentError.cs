using Intersect.Localization;

using JetBrains.Annotations;

namespace Intersect.Server.Core.CommandParsing.Errors
{

    public class MissingArgumentError : ParserError
    {

        protected MissingArgumentError([NotNull] string argumentName, [NotNull] string message) : base(message)
        {
            ArgumentName = argumentName;
        }

        [NotNull]
        public string ArgumentName { get; }

        [NotNull]
        public static MissingArgumentError Create(
            [NotNull] string commandName,
            [NotNull] string argumentName,
            [NotNull] LocalizedString message
        )
        {
            return new MissingArgumentError(argumentName, message.ToString(argumentName, commandName));
        }

        [NotNull]
        public static MissingArgumentError Create(
            [NotNull] string commandName,
            [NotNull] string argumentName,
            [NotNull] LocalizedString message,
            [NotNull] string prefix
        )
        {
            return new MissingArgumentError(argumentName, message.ToString(prefix, argumentName, commandName));
        }

    }

}
