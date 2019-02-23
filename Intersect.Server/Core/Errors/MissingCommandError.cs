using Intersect.Localization;
using JetBrains.Annotations;

namespace Intersect.Server.Core.Errors
{
    public class MissingCommandError : ParserError
    {
        [NotNull]
        public string CommandName { get; }

        private MissingCommandError(
            [NotNull] string commandName,
            [NotNull] string message
        ) : base(message)
        {
            CommandName = commandName;
        }

        [NotNull]
        public static MissingCommandError Create(
            [NotNull] string commandName,
            [NotNull] LocalizedString message
        )
        {
            return new MissingCommandError(commandName, message.ToString(commandName));
        }
    }
}