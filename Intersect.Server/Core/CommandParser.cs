using Intersect.Localization;
using Intersect.Server.Core.Errors;
using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Intersect.Server.Core
{
    public sealed class CommandParsingNamespace : LocaleNamespace
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
        public readonly CommandParserErrorsNamespace Errors = new CommandParserErrorsNamespace();
    }

    public sealed class CommandParserErrorsNamespace : LocaleNamespace
    {
        /// <summary>
        /// Generic parser error message.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
        public readonly LocalizedString GenericError =
            @"An error occurred while trying to parse the command.";

        /// <summary>
        /// No input was provided.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
        public readonly LocalizedString NoInput =
            @"No input was provided. If this is not the case, please report this error.";

        /// <summary>
        /// Command not found for the given name.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
        public readonly LocalizedString CommandNotFound =
            @"The command '{00}' is not recoginized. Enter '{01}' for a list of commands.";

        /// <summary>
        /// Named argument is not valid for the given command.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
        public readonly LocalizedString UnhandledNamedArgument =
            @"The argument '{00}' is not accepted for the command '{01}'.";

        /// <summary>
        /// Named argument is required but missing for the given command.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
        public readonly LocalizedString MissingNamedArgument =
            @"The argument '{00}{01}' is required for the command '{02}' but is missing.";

        /// <summary>
        /// Named argument was specified multiple times.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
        public readonly LocalizedString DuplicateNamedArgument = @"The argument '{00}' was specified more than once.";

        /// <summary>
        /// Positional argument is not valid for the given command.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
        public readonly LocalizedString UnhandledPositionalArgument =
            @"The argument '{00}' in position {01} is not accepted for the command '{02}'.";

        /// <summary>
        /// The value provided is not valid for this argument.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
        public readonly LocalizedString
            InvalidArgumentValue = @"The value '{00}' is not valid for the argument '{01}'.";

        /// <summary>
        /// The value provided is not valid for this argument, expected one of the specified type.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
        public readonly LocalizedString InvalidArgumentValueWithType =
            @"The value '{00}' is not valid for the argument '{01}' (expected type '{02}').";

        /// <summary>
        /// Flag argument is provided a value, but they do not accept them.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
        public readonly LocalizedString FlagArgumentsIgnoreValue =
            @"'{00}' is a flag argument and will ignore provided values.";

        /// <summary>
        /// Argument matches neither the short nor long argument name format and is not positional.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
        public readonly LocalizedString BadArgumentFormat =
            @"The argument '{00}' is not a valid short or long-form argument.";

        /// <summary>
        /// Argument matches both the short and long argument name formats.
        ///
        /// Note that should not actually be possible.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
        public readonly LocalizedString IllegalArgumentFormat =
            @"The argument '{00}' matches both the short and long-form argument formats (e.g. '{01}h' and '{02}help').";
    }

    public delegate CommandParsingNamespace CommandParserLocalizationProvider();

    public class CommandParser
    {
        [NotNull] protected static readonly CommandParsingNamespace DefaultLocalization;

        static CommandParser()
        {
            DefaultLocalization = new CommandParsingNamespace();
        }

        [CanBeNull]
        public CommandParserLocalizationProvider LocalizationProvider { get; set; }

        [NotNull] public static readonly string DefaultPrefixShort = @"-";
        [NotNull] public static readonly string DefaultPrefixLong = @"--";

        [NotNull] private string mPrefixShort = DefaultPrefixShort;
        [NotNull] private string mPrefixLong = DefaultPrefixLong;

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
        protected CommandParsingNamespace Localization =>
            LocalizationProvider?.Invoke() ?? DefaultLocalization;

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
                    Localization.Errors.NoInput,
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
                return MissingCommandError.Create(commandName, Localization.Errors.CommandNotFound).AsResult();
            }

            if (cleanArgs.Count < 2)
            {
                var constructedArguments = ConstructDefaultArguments(command.Arguments);
                if (constructedArguments != null)
                {
                    return constructedArguments.AsResult(command);
                }

                return new ParserError(
                    Localization.Errors.GenericError,
                    new InvalidOperationException(
                        $@"Failed to construct default arguments for command of type {command.GetType().FullName}."
                    )
                ).AsResult(command);
            }

            IDictionary<ICommandArgument, ArgumentValues> parsed = new Dictionary<ICommandArgument, ArgumentValues>();
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
                            Localization.Errors.BadArgumentFormat.ToString(cleanArg)
                        )
                    );
                    return;
                }

                if (canBeShortName && canBeLongName)
                {
                    errors.Add(
                        new ParserError(
                            Localization.Errors.IllegalArgumentFormat.ToString(cleanArg, PrefixShort, PrefixLong)
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
                    errors.Add(
                        UnhandledArgumentError.Create(
                            command.Name,
                            cleanArgName,
                            Localization.Errors.UnhandledNamedArgument
                        )
                    );
                    return;
                }

                List<object> values;
                if (parsed.TryGetValue(argument, out var argumentValues))
                {
                    if (!argument.AllowsMultiple)
                    {
                        errors.Add(new ParserError(Localization.Errors.DuplicateNamedArgument.ToString(cleanArgName),
                            false));
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
                                Localization.Errors.FlagArgumentsIgnoreValue.ToString(cleanArgName),
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

                                if (argument.ValueType != typeof(object))
                                {
                                    errors.Add(
                                        new ParserError(
                                            Localization.Errors.InvalidArgumentValueWithType.ToString(
                                                valuePart,
                                                cleanArgName,
                                                argument.ValueType.Name
                                            ),
                                            false
                                        )
                                    );
                                }
                                else
                                {
                                    errors.Add(
                                        new ParserError(
                                            Localization.Errors.InvalidArgumentValue.ToString(valuePart, cleanArgName),
                                            false
                                        )
                                    );
                                }

                                return defaultValue;
                            });

                        values.AddRange(parsedPartValues);
                    }
                }
                else
                {
                    if (!TryParseArgument(argument.ValueType, argument.DefaultValue, cleanArgValue, out var value))
                    {
                        if (argument.ValueType != typeof(object))
                        {
                            errors.Add(
                                new ParserError(
                                    Localization.Errors.InvalidArgumentValueWithType.ToString(
                                        cleanArgValue,
                                        cleanArgName,
                                        argument.ValueType.Name
                                    ),
                                    false
                                )
                            );
                        }
                        else
                        {
                            errors.Add(
                                new ParserError(
                                    Localization.Errors.InvalidArgumentValue.ToString(cleanArgValue, cleanArgName),
                                    false
                                )
                            );
                        }
                    }

                    values.Add(value);
                }

                parsed[argument] = new ArgumentValues(values);
            });

            foreach (var argument in command.Arguments)
            {
                if (!argument.IsRequired || parsed.ContainsKey(argument))
                {
                    continue;
                }

                errors.Add(
                    new ParserError(
                        Localization.Errors.MissingNamedArgument.ToString(PrefixLong, argument.Name, command.Name)
                    )
                );
            }

            return new ParserResult(command, new ArgumentValuesMap(parsed), errors);
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

        protected virtual bool TryParseArgument(
            [NotNull] Type type,
            [CanBeNull] object defaultValue,
            [NotNull] string source,
            out object parsed
        )
        {
            switch (defaultValue)
            {
                case bool defaultParsed:
                {
                    var success = bool.TryParse(source, out var value);
                    parsed = success ? value : defaultParsed;
                    return success;
                }

                case byte defaultParsed:
                {
                    var success = byte.TryParse(source, out var value);
                    parsed = success ? value : defaultParsed;
                    return success;
                }

                case sbyte defaultParsed:
                {
                    var success = sbyte.TryParse(source, out var value);
                    parsed = success ? value : defaultParsed;
                    return success;
                }

                case ushort defaultParsed:
                {
                    var success = ushort.TryParse(source, out var value);
                    parsed = success ? value : defaultParsed;
                    return success;
                }

                case short defaultParsed:
                {
                    var success = short.TryParse(source, out var value);
                    parsed = success ? value : defaultParsed;
                    return success;
                }

                case uint defaultParsed:
                {
                    var success = uint.TryParse(source, out var value);
                    parsed = success ? value : defaultParsed;
                    return success;
                }

                case int defaultParsed:
                {
                    var success = int.TryParse(source, out var value);
                    parsed = success ? value : defaultParsed;
                    return success;
                }

                case ulong defaultParsed:
                {
                    var success = ulong.TryParse(source, out var value);
                    parsed = success ? value : defaultParsed;
                    return success;
                }

                case long defaultParsed:
                {
                    var success = long.TryParse(source, out var value);
                    parsed = success ? value : defaultParsed;
                    return success;
                }

                case float defaultParsed:
                {
                    var success = float.TryParse(source, out var value);
                    parsed = success ? value : defaultParsed;
                    return success;
                }

                case double defaultParsed:
                {
                    var success = double.TryParse(source, out var value);
                    parsed = success ? value : defaultParsed;
                    return success;
                }

                case decimal defaultParsed:
                {
                    var success = decimal.TryParse(source, out var value);
                    parsed = success ? value : defaultParsed;
                    return success;
                }

                case char defaultParsed:
                {
                    var success = char.TryParse(source, out var value);
                    parsed = success ? value : defaultParsed;
                    return success;
                }

                case string defaultParsed:
                {
                    parsed = string.IsNullOrWhiteSpace(source) ? defaultParsed : source;
                    return true;
                }

                default:
                {
                    return TryParseType(type, defaultValue, source, out parsed);
                }
            }
        }

        protected virtual bool TryParseType([NotNull] Type type, [CanBeNull] object defaultValue,
            [NotNull] string source,
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
    }
}