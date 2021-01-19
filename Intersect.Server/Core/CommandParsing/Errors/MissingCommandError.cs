using Intersect.Localization;
using Intersect.Server.Localization;

namespace Intersect.Server.Core.CommandParsing.Errors
{

    public class MissingCommandError : ParserError
    {

        protected MissingCommandError(string commandName, string message) : base(message)
        {
            CommandName = commandName;
        }

        public string CommandName { get; }

        public static MissingCommandError Create(string commandName, LocalizedString message)
        {
            return new MissingCommandError(commandName, message.ToString(commandName, Strings.Commands.Help.Name));
        }

    }

}
