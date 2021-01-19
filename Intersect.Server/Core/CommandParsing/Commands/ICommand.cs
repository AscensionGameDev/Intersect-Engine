using System;
using System.Collections.Immutable;

using Intersect.Core;
using Intersect.Server.Core.CommandParsing.Arguments;

namespace Intersect.Server.Core.CommandParsing.Commands
{

    public interface ICommand
    {

        ImmutableList<ICommandArgument> Arguments { get; }

        ImmutableList<ICommandArgument> UnsortedArguments { get; }

        ImmutableList<ICommandArgument> NamedArguments { get; }

        ImmutableList<ICommandArgument> PositionalArguments { get; }

        Type ContextType { get; }

        string Name { get; }

        string Description { get; }

        ICommandArgument FindArgument(char shortName);

        ICommandArgument FindArgument(string name);

        void Handle(IApplicationContext context, ParserResult result);

        string FormatUsage(
            ParserSettings parserSettings,
            ParserContext parserContext,
            bool formatPrint = false
        );

    }

}
