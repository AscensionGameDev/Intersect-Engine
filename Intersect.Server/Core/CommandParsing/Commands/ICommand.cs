using System;
using System.Collections.Immutable;
using Intersect.Core;
using Intersect.Server.Core.CommandParsing.Arguments;
using JetBrains.Annotations;

namespace Intersect.Server.Core.CommandParsing.Commands
{
    public interface ICommand
    {
        [NotNull]
        ImmutableList<ICommandArgument> Arguments { get; }

        [NotNull]
        ImmutableList<ICommandArgument> NamedArguments { get; }

        [NotNull]
        ImmutableList<ICommandArgument> PositionalArguments { get; }

        [NotNull]
        Type ContextType { get; }

        [NotNull]
        string Name { get; }

        ICommandArgument FindArgument(char shortName);

        ICommandArgument FindArgument([NotNull] string name);

        void Handle([NotNull] IApplicationContext context, [NotNull] ParserResult result);
    }
}