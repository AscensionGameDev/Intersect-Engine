using Intersect.Core;
using JetBrains.Annotations;
using System;
using System.Collections.Immutable;

namespace Intersect.Server.Core
{
    public interface ICommand
    {
        [NotNull]
        ImmutableList<ICommandArgument> Arguments { get; }

        [NotNull]
        Type ContextType { get; }

        [NotNull]
        string Name { get; }

        ICommandArgument FindArgument(char shortName);

        ICommandArgument FindArgument([NotNull] string name);

        void Handle([NotNull] IApplicationContext context, [NotNull] ParserResult result);
    }
}