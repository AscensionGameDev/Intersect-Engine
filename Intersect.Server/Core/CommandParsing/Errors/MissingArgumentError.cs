using Intersect.Localization;

namespace Intersect.Server.Core.CommandParsing.Errors
{

    public class MissingArgumentError : ParserError
    {

        protected MissingArgumentError(string argumentName, string message) : base(message)
        {
            ArgumentName = argumentName;
        }

        public string ArgumentName { get; }

        public static MissingArgumentError Create(
            string commandName,
            string argumentName,
            LocalizedString message
        )
        {
            return new MissingArgumentError(argumentName, message.ToString(argumentName, commandName));
        }

        public static MissingArgumentError Create(
            string commandName,
            string argumentName,
            LocalizedString message,
            string prefix
        )
        {
            return new MissingArgumentError(argumentName, message.ToString(prefix, argumentName, commandName));
        }

    }

}
