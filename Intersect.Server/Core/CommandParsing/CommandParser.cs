using Intersect.Server.Core.CommandParsing.Arguments;
using Intersect.Server.Core.CommandParsing.Commands;
using Intersect.Server.Core.CommandParsing.Errors;
using Intersect.Server.Core.CommandParsing.Tokenization;
using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Intersect.Server.Core.CommandParsing
{
    public sealed class CommandParser
    {
        [NotNull]
        public ParserSettings Settings { get; }

        [NotNull]
        public Tokenizer Tokenizer { get; }

        [NotNull]
        private IDictionary<string, ICommand> Lookup { get; }

        public CommandParser() : this(ParserSettings.Default)
        {
        }

        public CommandParser([NotNull] ParserSettings settings)
        {
            Settings = settings;
            Tokenizer = new Tokenizer(settings.TokenizerSettings);
            Lookup = new Dictionary<string, ICommand>();
        }

        public bool Register<TCommand>() where TCommand : ICommand
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

        public bool Register([NotNull] ICommand command)
        {
            if (Lookup.ContainsKey(command.Name))
            {
                return false;
            }

            Lookup.Add(command.Name, command);
            return true;
        }

        [CanBeNull]
        public ICommand Find([NotNull] string commandName)
        {
            return !Lookup.TryGetValue(commandName, out var command) ? null : command;
        }

        [NotNull]
        public ParserResult Parse([NotNull] string line)
        {
            var tokens = Tokenizer
                .Tokenize(line)
                .Select(token => token?.Trim())
                .Where(token => token != null)
                .ToList();

            if (tokens.Count < 1)
            {
                return new ParserError(Settings.Localization.Errors.NoInput,
                    new ArgumentException(
                        @"No argument values were provided so unable to find a command.",
                        nameof(tokens)
                    )
                ).AsResult();
            }

            var commandName = tokens[0];
            var command = Find(commandName ?? throw new InvalidOperationException());
            if (command == null)
            {
                return MissingCommandError.Create(commandName, Settings.Localization.Errors.CommandNotFound).AsResult();
            }

            var positionalArguments = 0;
            var parsed = new Dictionary<ICommandArgument, ArgumentValues>();
            var errors = new List<ParserError>();

            tokens.Skip(1).ToList().ForEach(cleanArg =>
            {
                if (cleanArg == null)
                {
                    throw new InvalidOperationException(@"None of the cleaned arguments should be null at this point.");
                }

                var canBeShortName = cleanArg.StartsWith(Settings.PrefixShort);
                if (canBeShortName)
                {
                    var expectedLength = Settings.PrefixShort.Length + 1;
                    var actualLength = cleanArg.Contains('=') ? cleanArg.IndexOf('=') : cleanArg.Length;
                    canBeShortName = expectedLength == actualLength;
                }

                var canBeLongName = cleanArg.StartsWith(Settings.PrefixLong);
                if (canBeLongName)
                {
                    var actualLength = cleanArg.Length;
                    var maximumInvalidLength = Settings.PrefixLong.Length + (cleanArg.Contains('=') ? 2 : 1);
                    canBeLongName = actualLength > maximumInvalidLength;
                }

                var isPositional = false;
                if (!canBeShortName && !canBeLongName)
                {
                    if (positionalArguments < command.PositionalArguments.Count)
                    {
                        isPositional = true;
                    }
                    else
                    {
                        errors.Add(
                            new ParserError(Settings.Localization.Errors.BadArgumentFormat.ToString(cleanArg)
                            )
                        );
                        return;
                    }
                }

                if (canBeShortName && canBeLongName)
                {
                    errors.Add(
                        new ParserError(Settings.Localization.Errors.IllegalArgumentFormat.ToString(cleanArg, Settings.PrefixShort, Settings.PrefixLong)
                        )
                    );
                    return;
                }

                var cleanArgParts = cleanArg.Split('=');
                var cleanArgName = cleanArgParts[0] ?? "";
                var cleanArgValue = (cleanArgParts.Length == 2 ? cleanArgParts[1] : null) ?? "";

                if (isPositional)
                {
                    cleanArgValue = cleanArgName;
                }

                var argument = isPositional
                    ? command.PositionalArguments[positionalArguments]
                    : canBeShortName
                        ? command.FindArgument(cleanArgName[Settings.PrefixShort.Length])
                        : command.FindArgument(cleanArgName.Substring(Settings.PrefixLong.Length));

                if (argument == null)
                {
                    if (isPositional)
                    {
                        errors.Add(
                            UnhandledArgumentError.Create(
                                command.Name,
                                positionalArguments,
                                cleanArgValue,
                                Settings.Localization.Errors.UnhandledPositionalArgument
                            )
                        );
                    }
                    else
                    {
                        errors.Add(
                            UnhandledArgumentError.Create(
                                command.Name,
                                cleanArgName,
                                Settings.Localization.Errors.UnhandledNamedArgument
                            )
                        );
                    }

                    return;
                }

                List<object> values;
                if (parsed.TryGetValue(argument, out var argumentValues))
                {
                    if (!argument.AllowsMultiple)
                    {
                        errors.Add(
                            new ParserError(Settings.Localization.Errors.DuplicateNamedArgument.ToString(cleanArgName),
                                false
                            )
                        );
                        return;
                    }

                    values = argumentValues.Values.ToList();
                }
                else
                {
                    values = new List<object>();
                }

                if (argument.IsFlag)
                {
                    if (!string.IsNullOrEmpty(cleanArgValue))
                    {
                        errors.Add(
                            new ParserError(Settings.Localization.Errors.FlagArgumentsIgnoreValue.ToString(cleanArgName),
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
                                    if (!argument.IsValueAllowed(parsedPart))
                                    {
                                        errors.Add(
                                            new ParserError(Settings.Localization.Errors.InvalidArgumentValue.ToString(valuePart, cleanArgName)
                                            )
                                        );
                                    }

                                    return parsedPart;
                                }

                                if (argument.ValueType != typeof(object))
                                {
                                    errors.Add(
                                        new ParserError(Settings.Localization.Errors.InvalidArgumentValueWithType.ToString(
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
                                        new ParserError(Settings.Localization.Errors.InvalidArgumentValue.ToString(valuePart, cleanArgName),
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
                                new ParserError(Settings.Localization.Errors.InvalidArgumentValueWithType.ToString(
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
                                new ParserError(Settings.Localization.Errors.InvalidArgumentValue.ToString(cleanArgValue, cleanArgName),
                                    false
                                )
                            );
                        }
                    }

                    if (!argument.IsValueAllowed(value))
                    {
                        errors.Add(
                            new ParserError(
                                // TODO: DisallowedArgumentValue message instead
                                Settings.Localization.Errors.InvalidArgumentValue.ToString(value, cleanArgName)
                            )
                        );
                    }


                    values.Add(value);
                }

                parsed[argument] = new ArgumentValues(cleanArgName, values);

                if (isPositional)
                {
                    ++positionalArguments;
                }
            });

            var parserContext = new ParserContext
            {
                Command = command,
                Tokens = tokens.ToImmutableList(),
                Errors = errors.ToImmutableList(),
                Parsed = parsed.ToImmutableDictionary()
            };

            var omitted = new List<ICommandArgument>();
            var missing = new List<ICommandArgument>();
            foreach (var argument in command.Arguments)
            {
                if (!argument.IsRequired(parserContext) || parsed.ContainsKey(argument))
                {
                    if (!parsed.ContainsKey(argument))
                    {
                        parsed[argument] = ConstructDefaultArgument(argument);
                        omitted.Add(argument);
                    }

                    continue;
                }

                if (argument.IsPositional)
                {
                    errors.Add(
                        MissingArgumentError.Create(
                            Settings.Localization.Errors.MissingPositionalArgument,
                            argument.Name,
                            commandName
                        )
                    );
                }
                else
                {
                    errors.Add(
                        MissingArgumentError.Create(
                            Settings.PrefixLong,
                            argument.Name,
                            commandName
                        )
                    );
                }

                missing.Add(argument);
            }

            return new ParserResult(command, new ArgumentValuesMap(parsed), errors, missing, omitted);
        }

        [CanBeNull]
        private ArgumentValuesMap ConstructDefaultArguments([NotNull] IEnumerable<ICommandArgument> arguments)
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
        private ArgumentValues ConstructDefaultArgument([NotNull] ICommandArgument argument)
        {
            var argumentName = argument.ShortName == '\0' ? Settings.PrefixShort + argument.ShortName : Settings.PrefixLong + argument.Name;
            return argument.ValueType.IsArray
                ? new ArgumentValues(argumentName, (argument.DefaultValue as IEnumerable)?.Cast<object>())
                : new ArgumentValues(argumentName, argument.DefaultValue);
        }

        private bool TryParseArgument(
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
                    if (!success)
                    {
                        if (string.Equals(Settings.Localization.ValueTrue.ToString(), source,
                            StringComparison.OrdinalIgnoreCase))
                        {
                            success = true;
                            value = true;
                        }
                        else if (string.Equals(Settings.Localization.ValueFalse.ToString(), source,
                            StringComparison.OrdinalIgnoreCase))
                        {
                            success = true;
                            value = false;
                        }
                    }

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

                case Enum defaultParsed:
                {
                    try
                    {
                        parsed = Enum.Parse(type, source);
                        return true;
                    }
                    catch (Exception)
                    {
                        parsed = defaultParsed;
                        return false;
                    }
                }

                default:
                {
                    return TryParseType(type, defaultValue, source, out parsed);
                }
            }
        }

        private static bool TryParseType([NotNull] Type type, [CanBeNull] object defaultValue,
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
    }
}