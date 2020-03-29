using System.Collections.Immutable;

using Intersect.Server.Core.CommandParsing.Arguments;
using Intersect.Server.Core.CommandParsing.Commands;
using Intersect.Server.Core.CommandParsing.Errors;

namespace Intersect.Server.Core.CommandParsing
{

    public struct ParserContext
    {

        public ICommand Command { get; set; }

        public ImmutableList<string> Tokens { get; set; }

        public ImmutableList<ParserError> Errors { get; set; }

        public ImmutableDictionary<ICommandArgument, ArgumentValues> Parsed { get; set; }

    }

}
