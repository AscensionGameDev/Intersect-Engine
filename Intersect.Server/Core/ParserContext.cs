using Intersect.Server.Core.Errors;
using System.Collections.Immutable;

namespace Intersect.Server.Core
{
    public struct ParserContext
    {
        public ICommand Command { get; set; }

        public ImmutableList<string> Tokens { get; set; }
        
        public ImmutableList<ParserError> Errors { get; set; }

        public ImmutableDictionary<ICommandArgument, ArgumentValues> Parsed { get; set; }
    }
}
