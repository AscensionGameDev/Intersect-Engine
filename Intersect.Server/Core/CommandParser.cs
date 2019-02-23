using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Intersect.Localization;
using Intersect.Server.Core.Arguments;
using Intersect.Server.Core.Errors;
using Nancy.Json;
using Newtonsoft.Json;

namespace Intersect.Server.Core
{
    public class CommandParser
    {
        public static void ValidatePrefix(string prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix))
            {
                throw new ArgumentException(@"Prefix cannot be null, empty, or whitespace.");
            }

            if (prefix.Contains('='))
            {
                throw new ArgumentException(@"Prefixes cannot contain '='.");
            }

            if (prefix.Contains(' ') || prefix.Contains('\n') || prefix.Contains('\r') || prefix.Contains('\t'))
            {
                throw new ArgumentException(@"Prefixes cannot contain whitespace.");
            }

            if (prefix.Contains('\0'))
            {
                throw new ArgumentException(@"Prefixes cannot contain the null character.");
            }
        }

        [NotNull] public static readonly string ParserErrorMessage =
            @"An error occurred while trying to parse the command.";

        [NotNull] public static readonly string CommandNotFoundMessage =
            @"The command '{00}' was not found.";

        [NotNull] public static readonly string HelpArgumentShort = @"/?";

        [NotNull] public static readonly string DefaultPrefixShort = @"-";

        [NotNull] public static readonly string DefaultPrefixLong = @"--";

        [NotNull] private string mPrefixShort = DefaultPrefixShort;
        [NotNull] private string mPrefixLong = DefaultPrefixLong;

        private LocalizedString mErrorMessage;
        private LocalizedString mNotFoundMesssage;

        [NotNull]
        public LocalizedString ErrorMessage
        {
            get => mErrorMessage ?? ParserErrorMessage;
            set => mErrorMessage = value;
        }

        [NotNull]
        public LocalizedString NotFoundMesssage
        {
            get => mNotFoundMesssage ?? CommandNotFoundMessage;
            set => mNotFoundMesssage = value;
        }

        [NotNull]
        public string PrefixShort
        {
            get => mPrefixShort;
            set
            {
                if (value == mPrefixLong)
                {
                    throw new ArgumentException(
                        $@"Cannot set the short prefix to the same value as the long prefix ({value}).",
                        nameof(value)
                    );
                }

                ValidatePrefix(value);

                mPrefixShort = value;
            }
        }

        [NotNull]
        public string PrefixLong
        {
            get => mPrefixLong;
            set
            {
                if (value == mPrefixShort)
                {
                    throw new ArgumentException(
                        $@"Cannot set the long prefix to the same value as the short prefix ({value}).",
                        nameof(value)
                    );
                }

                ValidatePrefix(value);

                mPrefixLong = value;
            }
        }

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
            return Parse(args as IEnumerable<string>);
        }

        [NotNull]
        public virtual ParserResult Parse([NotNull] IEnumerable<string> args)
        {
            var cleanArgs = args
                .Select(arg => arg?.Trim())
                .Where(arg => !string.IsNullOrEmpty(arg))
                .ToList();

            if (cleanArgs.Count < 1)
            {
                return new ParserError(
                    ErrorMessage,
                    new ArgumentException(
                        @"No argument values were provided so unable to find a command.",
                        nameof(args)
                    )
                ).AsResult();
            }

            var commandName = cleanArgs[0];
            var command = Find(commandName ?? throw new InvalidOperationException());
            if (command == null)
            {
                return MissingCommandError.Create(commandName, NotFoundMesssage).AsResult();
            }

            if (cleanArgs.Count < 2)
            {
                var constructedArguments = ConstructDefaultArguments(command.Arguments);
                if (constructedArguments != null)
                {
                    return constructedArguments.AsResult(command);
                }

                return new ParserError(
                    ErrorMessage,
                    new InvalidOperationException(
                        $@"Failed to construct default arguments for command of type {command.GetType().FullName}."
                    )
                ).AsResult(command);
            }

            IDictionary<ICommandArgument, ArgumentValues> parsed = new Dictionary<ICommandArgument, ArgumentValues>();
            IList<object> unhandled = new List<object>();
            IList<ParserError> errors = new List<ParserError>();

            cleanArgs.Skip(1).ToList().ForEach(cleanArg =>
            {
                if (cleanArg == null)
                {
                    throw new InvalidOperationException(@"None of the cleaned arguments should be null at this point.");
                }

                var canBeShortName = cleanArg.StartsWith(PrefixShort);
                if (canBeShortName)
                {
                    var expectedLength = PrefixShort.Length + 1;
                    var actualLength = cleanArg.Contains('=') ? cleanArg.IndexOf('=') : cleanArg.Length;
                    canBeShortName = expectedLength == actualLength;
                }

                var canBeLongName = cleanArg.StartsWith(PrefixLong);
                if (canBeLongName)
                {
                    var actualLength = cleanArg.Length;
                    var maximumInvalidLength = PrefixLong.Length + (cleanArg.Contains('=') ? 2 : 1);
                    canBeLongName = actualLength > maximumInvalidLength;
                }

                if (!canBeShortName && !canBeLongName)
                {
                    errors.Add(
                        new ParserError(
                            $@"'{cleanArg}' is not a valid argument/argument-value. Positional arguments are not yet supported, and named arguments do not contain spaces."
                        )
                    );
                    return;
                }

                if (canBeShortName && canBeLongName)
                {
                    errors.Add(
                        new ParserError(
                            $@"'{cleanArg}' somehow can be both a short name or a long name, but this should not be possible. This indicates a logic error, please report this. PrefixShort ='{PrefixShort}' PrefixLong='{PrefixLong}'"
                        )
                    );
                    return;
                }

                var cleanArgParts = cleanArg.Split('=');
                var cleanArgName = cleanArgParts[0] ?? "";

                var argument = canBeShortName
                    ? command.FindArgument(cleanArgName[PrefixShort.Length])
                    : command.FindArgument(cleanArgName.Substring(PrefixLong.Length));

                if (argument == null)
                {
                    unhandled.Add(cleanArg);
                    return;
                }

                List<object> values;
                if (parsed.TryGetValue(argument, out var argumentValues))
                {
                    if (!argument.AllowsMultiple)
                    {
                        errors.Add(new ParserError($@"Duplicate argument '{cleanArgName}'.", false));
                        return;
                    }

                    values = argumentValues.Values.ToList();
                }
                else
                {
                    values = new List<object>();
                }

                var cleanArgValue = (cleanArgParts.Length == 2 ? cleanArgParts[1] : null) ?? "";
                if (argument.IsFlag)
                {
                    if (!string.IsNullOrEmpty(cleanArgValue))
                    {
                        errors.Add(
                            new ParserError(
                                $@"'{cleanArgName}' is a flag argument and will ignore provided values.",
                                false
                            )
                        );
                    }

                    values.Add(true);
                }
                else if (argument.IsCollection)
                {
                    if (argument.Delimeter == null)
                    {
                    }
                    else
                    {
                        var defaultValue = argument.ValueTypeDefault;
                        var parsedPartValues = cleanArgValue
                            .Split(new[] {argument.Delimeter}, StringSplitOptions.None)
                            .Select(valuePart =>
                            {
                                if (string.IsNullOrEmpty(valuePart))
                                {
                                    return defaultValue;
                                }

                                if (TryParseArgument(argument.ValueType, defaultValue, valuePart, out var parsedPart))
                                {
                                    return parsedPart;
                                }

                                errors.Add(
                                    new ParserError(
                                        $@"Failed to parsed '{valuePart}' for argument '{cleanArgName}.",
                                        false
                                    )
                                );

                                return defaultValue;
                            });

                        values.AddRange(parsedPartValues);
                    }
                }
                else
                {
                    if (!TryParseArgument(argument.ValueType, argument.DefaultValue, cleanArgValue, out var value))
                    {
                        errors.Add(new ParserError($@"Error parsing argument {cleanArgName} ({cleanArgValue})", false));
                    }

                    values.Add(value);
                }

                parsed[argument] = new ArgumentValues(values);
            });

            return new ParserResult(command, new ArgumentValuesMap(parsed), new ArgumentValues(unhandled), errors);
        }

        [CanBeNull]
        protected virtual ArgumentValuesMap ConstructDefaultArguments([NotNull] IList<ICommandArgument> arguments)
        {
            return new ArgumentValuesMap(
                arguments
                    .Where(argument => argument != null)
                    .Select(
                        argument => new KeyValuePair<ICommandArgument, ArgumentValues>(
                            argument,
                            ConstructDefaultArgument(argument)
                        )
                    )
            );
        }

        [NotNull]
        protected virtual ArgumentValues ConstructDefaultArgument([NotNull] ICommandArgument argument)
        {
            return argument.ValueType.IsArray
                ? new ArgumentValues((argument.DefaultValue as IEnumerable)?.Cast<object>())
                : new ArgumentValues(argument.DefaultValue);
        }

        [CanBeNull]
        protected virtual bool TryParseArgument(
            [NotNull] Type type,
            [CanBeNull] object defaultValue,
            [NotNull] string source,
            out object parsed
        )
        {
            switch (defaultValue)
            {
                case byte defaultParsed:
                {
                    parsed = byte.TryParse(source, out var value) ? value : defaultParsed;
                    return true;
                }

                case sbyte defaultParsed:
                {
                    parsed = sbyte.TryParse(source, out var value) ? value : defaultParsed;
                    return true;
                }

                case ushort defaultParsed:
                {
                    parsed = ushort.TryParse(source, out var value) ? value : defaultParsed;
                    return true;
                }

                case short defaultParsed:
                {
                    parsed = short.TryParse(source, out var value) ? value : defaultParsed;
                    return true;
                }

                case uint defaultParsed:
                {
                    parsed = uint.TryParse(source, out var value) ? value : defaultParsed;
                    return true;
                }

                case int defaultParsed:
                {
                    parsed = int.TryParse(source, out var value) ? value : defaultParsed;
                    return true;
                }

                case ulong defaultParsed:
                {
                    parsed = ulong.TryParse(source, out var value) ? value : defaultParsed;
                    return true;
                }

                case long defaultParsed:
                {
                    parsed = long.TryParse(source, out var value) ? value : defaultParsed;
                    return true;
                }

                case float defaultParsed:
                {
                    parsed = float.TryParse(source, out var value) ? value : defaultParsed;
                    return true;
                }

                case double defaultParsed:
                {
                    parsed = double.TryParse(source, out var value) ? value : defaultParsed;
                    return true;
                }

                case decimal defaultParsed:
                {
                    parsed = decimal.TryParse(source, out var value) ? value : defaultParsed;
                    return true;
                }

                case char defaultParsed:
                {
                    parsed = char.TryParse(source, out var value) ? value : defaultParsed;
                    return true;
                }

                case string defaultParsed:
                {
                    parsed = string.IsNullOrWhiteSpace(source) ? defaultParsed : source;
                    return true;
                }

                default:
                {
                    return TryParse(type, defaultValue, source, out parsed);
                }
            }
        }

        protected virtual bool TryParse([NotNull] Type type, [CanBeNull] object defaultValue, [NotNull] string source,
            out object parsed)
        {
            if (type == typeof(string))
            {
                parsed = source;
                return true;
            }

            try
            {
                parsed = JsonConvert.DeserializeObject(source);
                return true;
            }
            catch (Exception)
            {
                parsed = defaultValue;
                return false;
            }
        }
    }
}