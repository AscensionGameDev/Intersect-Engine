using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.Core;
using Intersect.Localization;
using JetBrains.Annotations;

namespace Intersect.Server.Core
{
    public interface ICommand
    {
        [NotNull]
        Type ArgumentsType { get; }

        [NotNull]
        Type ContextType { get; }

        [NotNull]
        string Name { get; }

        void Handle([NotNull] IApplicationContext context, [NotNull] ICommandArguments arguments);
    }

    public interface ICommandArguments
    {
        ImmutableArray<string> UnknownArguments { get; }
    }

    public static class CommandArgumentsExtensions
    {
        [NotNull]
        public static ParserResult AsResult(
            [NotNull] this ICommandArguments commandArguments,
            [CanBeNull] ICommand command = null
        )
        {
            return new ParserResult(command, commandArguments);
        }

        [NotNull]
        public static ParserResult<TArguments> AsTypedResult<TArguments>(
            [NotNull] this TArguments commandArguments,
            [CanBeNull] ICommand command = null
        )
            where TArguments : ICommandArguments
        {
            return new ParserResult<TArguments>(command, commandArguments);
        }

        [NotNull]
        public static ParserResult<TCommand, TArguments> AsTypedResult<TCommand, TArguments>(
            [NotNull] this TArguments commandArguments,
            [CanBeNull] TCommand command = default(TCommand)
        )
            where TCommand : ICommand
            where TArguments : ICommandArguments
        {
            return new ParserResult<TCommand, TArguments>(command, commandArguments);
        }
    }

    public class ParserResult<TCommand, TArguments>
        where TCommand : ICommand
        where TArguments : ICommandArguments
    {
        [CanBeNull]
        public TCommand Command { get; }

        [NotNull]
        public TArguments Arguments { get; }

        public ParserResult([NotNull] TArguments arguments)
            : this(default(TCommand), arguments)
        {
        }

        public ParserResult(
            [CanBeNull] TCommand command,
            [NotNull] TArguments arguments
        )
        {
            Command = command;
            Arguments = arguments;
        }
    }

    public class ParserResult<TArguments>
        : ParserResult<ICommand, TArguments>
        where TArguments : ICommandArguments
    {
        public ParserResult([NotNull] TArguments arguments)
            : this(null, arguments)
        {
        }

        public ParserResult([CanBeNull] ICommand command, [NotNull] TArguments arguments)
            : base(command, arguments)
        {
        }
    }

    public class ParserResult
        : ParserResult<ICommand, ICommandArguments>
    {
        public ParserResult([NotNull] ICommandArguments arguments)
            : this(null, arguments)
        {
        }

        public ParserResult([CanBeNull] ICommand command, [NotNull] ICommandArguments arguments)
            : base(command, arguments)
        {
        }
    }

    public abstract class Command<TContext, TArguments> : ICommand
        where TContext : IApplicationContext
        where TArguments : ICommandArguments
    {
        public Type ArgumentsType => typeof(TArguments);

        public Type ContextType => typeof(TContext);

        public string Name => Localization.Name;

        [NotNull]
        public LocaleCommand Localization { get; }

        protected Command([NotNull] LocaleCommand localization)
        {
            Localization = localization;
        }

        public void Handle(IApplicationContext context, ICommandArguments arguments)
        {
            if (context.GetType() != ContextType)
            {
                throw new ArgumentException($@"Expected {ContextType.FullName} not {context.GetType().FullName}.",
                    nameof(context));
            }

            if (arguments.GetType() != ArgumentsType)
            {
                throw new ArgumentException($@"Expected {ArgumentsType.FullName} not {arguments.GetType().FullName}.",
                    nameof(arguments));
            }

            Handle((TContext) context, (TArguments) arguments);
        }

        public abstract void Handle([NotNull] TContext context, [NotNull] TArguments arguments);
    }

    internal abstract class ServerCommand<TArguments> : Command<ServerContext, TArguments>
        where TArguments : ICommandArguments
    {
        protected ServerCommand([NotNull] LocaleCommand localization)
            : base(localization)
        {
        }
    }

    internal abstract class ServerCommand : ServerCommand<HelpArguments>
    {
        protected ServerCommand([NotNull] LocaleCommand localization)
            : base(localization)
        {
        }
    }

    public class CommandError : ICommandArguments
    {
        [NotNull]
        public string Message { get; }

        [CanBeNull]
        public Exception Exception { get; }

        [CanBeNull]
        public string[] Arguments { get; }

        public ImmutableArray<string> UnknownArguments { get; }

        public CommandError(string message = null)
        {
            // This should be the only hard-coded English error message. All
            // other English-hardcoded strings belong as the Exception message
            // because Message will be displayed to the end-user (and also
            // logged), while the Exception will only be logged.
            Message = message ?? "Unknown command error occurred.";
            UnknownArguments = new string[0].ToImmutableArray();
        }

        public CommandError(string message, Exception exception, params string[] arguments)
            : this(message)
        {
            Exception = exception;
            Arguments = arguments;
        }
    }

    public class HelpArguments : ICommandArguments
    {
        public bool Help { get; }

        public ImmutableArray<string> UnknownArguments { get; }

        public HelpArguments()
            : this(false, new string[0])
        {
        }

        public HelpArguments(bool help, [NotNull] IEnumerable<string> unknownArguments)
        {
            Help = help;
            UnknownArguments = unknownArguments.ToImmutableArray();
        }
    }
}