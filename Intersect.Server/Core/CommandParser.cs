using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intersect.Server.Core
{
    public class CommandParser
    {
        [NotNull]
        public static readonly string ParserErrorMessage =
            @"An error occurred while trying to parse the command.";

        [NotNull]
        public static readonly string CommandNotFoundMessage =
            @"The command '{00}' was not found.";

        [NotNull]
        public static readonly string HelpArgumentShort = @"/?";

        public string ErrorMessage { get; set; } = ParserErrorMessage;

        public string NotFoundMesssage { get; set; } = CommandNotFoundMessage;

        public string HelpShort { get; set; } = HelpArgumentShort;

        public string HelpLong { get; set; } = null;

        [NotNull]
        protected IDictionary<string, ICommand> Lookup { get; }

        public CommandParser()
        {
            Lookup = new Dictionary<string, ICommand>();
        }

        public virtual bool Register<TCommand>() where TCommand : ICommand
        {
            var commandType = typeof(TCommand);
            if (commandType.IsAbstract || commandType.IsInterface)
            {
                throw new InvalidOperationException(
                    $@"Cannot register abstract/interface command type {commandType.Name} ({commandType.FullName})."
                );
            }

            var defaultConstructor = commandType.GetConstructor(Type.EmptyTypes);
            if (defaultConstructor == null)
            {
                throw new InvalidOperationException(
                    $@"No default constructor for command type {commandType.Name} ({commandType.FullName})."
                );
            }

            var command = defaultConstructor.Invoke(new object[0]) as ICommand;
            if (command == null)
            {
                throw new InvalidOperationException(
                    $@"Failed to construct command type {commandType.Name} ({commandType.FullName})."
                );
            }

            return Register(command);
        }

        public virtual bool Register([NotNull] ICommand command)
        {
            if (Lookup.ContainsKey(command.Name))
            {
                return false;
            }

            Lookup.Add(command.Name, command);
            return true;
        }

        [CanBeNull]
        public virtual ICommand Find([NotNull] string commandName)
        {
            return !Lookup.TryGetValue(commandName, out var command) ? null : command;
        }

        [NotNull]
        public virtual ParserResult Parse([NotNull] params string[] args)
        {
            return Parse(null, args);
        }

        [NotNull]
        public virtual ParserResult Parse([CanBeNull] Type argumentsType, [NotNull] params string[] args)
        {
            var resolvedArgumentsType = argumentsType;

            var cleanArgs = args
                .Select(arg => arg?.Trim())
                .Where(arg => !string.IsNullOrEmpty(arg))
                .ToList();

            if (cleanArgs.Count < 1)
            {
                if (resolvedArgumentsType == null)
                {
                    return new CommandError(
                        ErrorMessage,
                        new ArgumentNullException(
                            nameof(argumentsType),
                            @"No arguments were provided and the target arguments type is null."
                        )
                    ).AsResult();
                }

                var constructedArguments = ConstructDefaultArguments(resolvedArgumentsType);
                if (constructedArguments != null)
                {
                    return constructedArguments.AsResult();
                }

                return new CommandError(
                    ErrorMessage,
                    new InvalidOperationException(
                        $@"Failed to construct default arguments of type {resolvedArgumentsType.Name} ({resolvedArgumentsType.FullName})."
                    )
                ).AsResult();
            }

            var commandName = cleanArgs[0];
            var command = Find(commandName ?? throw new InvalidOperationException());
            if (command == null)
            {
                return new CommandError(string.Format(NotFoundMesssage ?? CommandNotFoundMessage, commandName)).AsResult();
            }

            if (resolvedArgumentsType == null)
            {
                resolvedArgumentsType = command.ArgumentsType;
            }
            else if (resolvedArgumentsType != command.ArgumentsType)
            {
                return new CommandError(
                    ErrorMessage,
                    new InvalidCastException(
                        $@"Arguments type mismatch between the provided type {resolvedArgumentsType.Name} and the declared type {command.ArgumentsType.Name}."
                    )
                ).AsResult(command);
            }

            if (cleanArgs.Count < 2)
            {
                var constructedArguments = ConstructDefaultArguments(resolvedArgumentsType);
                if (constructedArguments != null)
                {
                    return constructedArguments.AsResult(command);
                }

                return new CommandError(
                    ErrorMessage,
                    new InvalidOperationException(
                        $@"Failed to construct default arguments of type {resolvedArgumentsType.Name} ({resolvedArgumentsType.FullName})."
                    )
                ).AsResult(command);
            }

            if (resolvedArgumentsType == typeof(HelpArguments))
            {
                var isHelp = string.Equals(cleanArgs[1], HelpShort, StringComparison.Ordinal) ||
                             string.Equals(cleanArgs[1], HelpLong, StringComparison.Ordinal);

                var unknownArguments =
                    cleanArgs.Count < 3 ? new List<string>() : cleanArgs.GetRange(2, cleanArgs.Count - 2);

                return new HelpArguments(isHelp, unknownArguments).AsResult(command);
            }

            throw new Exception();
        }

        [NotNull]
        public virtual ParserResult<TArguments> Parse<TArguments>([NotNull] params string[] args)
            where TArguments : ICommandArguments
        {
            var result = Parse(typeof(TArguments), args);
            return new ParserResult<TArguments>(result.Command, (TArguments) result.Arguments);
        }

        [CanBeNull]
        protected virtual ICommandArguments ConstructDefaultArguments([NotNull] Type argumentsType)
        {
            if (argumentsType.IsAbstract || argumentsType.IsInterface)
            {
                return new CommandError(
                    ErrorMessage,
                    new InvalidOperationException(
                        $@"Cannot register abstract/interface arguments type {argumentsType.Name} ({argumentsType.FullName})."
                    )
                );
            }

            var defaultConstructor = argumentsType.GetConstructor(Type.EmptyTypes);
            if (defaultConstructor == null)
            {
                return new CommandError(
                    ErrorMessage,
                    new InvalidOperationException(
                        $@"No default constructor for arguments type {argumentsType.Name} ({argumentsType.FullName})."
                    )
                );
            }

            if (defaultConstructor.Invoke(new object[0]) is ICommandArguments arguments)
            {
                return arguments;
            }

            return new CommandError(
                ErrorMessage,
                new InvalidOperationException(
                    $@"Failed to construct arguments type {argumentsType.Name} ({argumentsType.FullName})."
                )
            );
        }
    }
}