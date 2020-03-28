using Intersect.Localization;
using Intersect.Server.Localization;

using JetBrains.Annotations;

namespace Intersect.Server.Core.CommandParsing.Errors
{

    public class MissingCommandError : ParserError
    {

        protected MissingCommandError([NotNull] string commandName, [NotNull] string message) : base(message)
        {
            CommandName = commandName;
        }

        [NotNull]
        public string CommandName { get; }

        [NotNull]
        public static MissingCommandError Create([NotNull] string commandName, [NotNull] LocalizedString message)
        {
            return new MissingCommandError(commandName, message.ToString(commandName, Strings.Commands.Help.Name));
        }

    }

}
