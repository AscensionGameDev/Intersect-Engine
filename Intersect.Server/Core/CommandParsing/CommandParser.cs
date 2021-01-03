using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Intersect.Server.Core.CommandParsing.Arguments;
using Intersect.Server.Core.CommandParsing.Commands;
using Intersect.Server.Core.CommandParsing.Errors;
using Intersect.Server.Core.CommandParsing.Tokenization;

using Newtonsoft.Json;

namespace Intersect.Server.Core.CommandParsing
{

    public sealed class CommandParser
    {

        public CommandParser() : this(ParserSettings.Default)
        {
        }

        public CommandParser(ParserSettings settings)
        {
            Settings = settings;
            Tokenizer = new Tokenizer(settings.TokenizerSettings);
            Lookup = new Dictionary<string, ICommand>();
        }

        public ParserSettings Settings { get; }

        public Tokenizer Tokenizer { get; }

        private IDictionary<string, ICommand> Lookup { get; }

        public bool Register<TCommand>(params object[] args) where TCommand : ICommand
        {
            var commandType = typeof(TCommand);
            if (commandType.IsAbstract || commandType.IsInterface)
            {
                throw new InvalidOperationException(
                    $@"Cannot register abstract/interface command type {commandType.Name} ({commandType.FullName})."
                );
            }

            var constructorTypes = args?.Select(arg => arg?.GetType() ?? typeof(object)).ToArray() ?? Type.EmptyTypes;
            var defaultConstructor = commandType.GetConstructor(constructorTypes);
            if (defaultConstructor == null)
            {
                throw new InvalidOperationException(
                    $@"No default constructor for command type {commandType.Name} ({commandType.FullName})."
                );
            }

            if (defaultConstructor.Invoke(args ?? Array.Empty<object>()) is ICommand command)
            {
                return Register(command);
            }

            throw new InvalidOperationException(
                $@"Failed to construct command type {commandType.Name} ({commandType.FullName})."
            );
        }

        public bool Register(ICommand command)
        {
            if (Lookup.ContainsKey(command.Name))
            {
                return false;
            }

            Lookup.Add(command.Name, command);

            return true;
        }

        public ICommand Find(string commandName)
        {
            return !Lookup.TryGetValue(commandName, out var command) ? null : command;
        }

        public ParserResult Parse(string line)
        {
            var tokens = Tokenizer.Tokenize(line).Select(token => token?.Trim()).Where(token => token != null).ToList();

            if (tokens.Count < 1)
            {
                return new ParserError(
                    Settings.Localization.Errors.NoInput,
                    new ArgumentException(
                        @"No argument values were provided so unable to find a command.", nameof(tokens)
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

            for (var tokenIndex = 1; tokenIndex < tokens.Count; ++tokenIndex)
            {
                var token = tokens[tokenIndex];

                if (token == null)
                {
                    throw new InvalidOperationException(@"None of the cleaned arguments should be null at this point.");
                }

                var canBeShortName = token.StartsWith(Settings.PrefixShort);
                if (canBeShortName)
                {
                    var expectedLength = Settings.PrefixShort.Length + 1;
                    var actualLength = token.Contains('=') ? token.IndexOf('=') : token.Length;
                    canBeShortName = expectedLength == actualLength;
                }

                var canBeLongName = token.StartsWith(Settings.PrefixLong);
                if (canBeLongName)
                {
                    var actualLength = token.Length;
                    var maximumInvalidLength = Settings.PrefixLong.Length + (token.Contains('=') ? 2 : 1);
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
                        errors.Add(new ParserError(Settings.Localization.Errors.BadArgumentFormat.ToString(token)));

                        continue;
                    }
                }

                if (canBeShortName && canBeLongName)
                {
                    errors.Add(
                        new ParserError(
                            Settings.Localization.Errors.IllegalArgumentFormat.ToString(
                                token, Settings.PrefixShort, Settings.PrefixLong
                            )
                        )
                    );

                    continue;
                }

                var cleanArgParts = token.Split('=');
                var cleanArgName = cleanArgParts[0] ?? "";
                var cleanArgValue = (cleanArgParts.Length == 2 ? cleanArgParts[1] : null) ?? "";

                if (isPositional)
                {
                    cleanArgValue = cleanArgName;
                }

                var argument = isPositional ? command.PositionalArguments[positionalArguments] :
                    canBeShortName ? command.FindArgument(cleanArgName[Settings.PrefixShort.Length]) :
                    command.FindArgument(cleanArgName.Substring(Settings.PrefixLong.Length));

                if (argument == null)
                {
                    if (isPositional)
                    {
                        errors.Add(
                            UnhandledArgumentError.Create(
                                command.Name, positionalArguments, cleanArgValue,
                                Settings.Localization.Errors.UnhandledPositionalArgument
                            )
                        );
                    }
                    else
                    {
                        errors.Add(
                            UnhandledArgumentError.Create(
                                command.Name, cleanArgName, Settings.Localization.Errors.UnhandledNamedArgument
                            )
                        );
                    }

                    continue;
                }

                var argumentDisplayName = isPositional ? argument.Name : cleanArgName;

                var typeName = argument.ValueType.Name;
                if (Settings.Localization.TypeNames.TryGetValue(typeName, out var localizedType))
                {
                    typeName = localizedType;
                }

                List<object> values;
                if (parsed.TryGetValue(argument, out var argumentValues))
                {
                    if (!argument.AllowsMultiple)
                    {
                        errors.Add(
                            new ParserError(
                                Settings.Localization.Errors.DuplicateNamedArgument.ToString(argumentDisplayName), false
                            )
                        );

                        continue;
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
                            new ParserError(
                                Settings.Localization.Errors.FlagArgumentsIgnoreValue.ToString(argumentDisplayName),
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
                        var parsedPartValues = cleanArgValue.Split(new[] {argument.Delimeter}, StringSplitOptions.None)
                            .Select(
                                valuePart =>
                                {
                                    if (string.IsNullOrEmpty(valuePart))
                                    {
                                        return defaultValue;
                                    }

                                    if (TryParseArgument(
                                        argument.ValueType, defaultValue, valuePart, out var parsedPart
                                    ))
                                    {
                                        if (!argument.IsValueAllowed(parsedPart))
                                        {
                                            errors.Add(
                                                new ParserError(
                                                    Settings.Localization.Errors.InvalidArgumentValue.ToString(
                                                        valuePart, argumentDisplayName
                                                    )
                                                )
                                            );
                                        }

                                        return parsedPart;
                                    }

                                    if (argument.ValueType != typeof(object))
                                    {
                                        errors.Add(
                                            new ParserError(
                                                Settings.Localization.Errors.InvalidArgumentValueWithType.ToString(
                                                    valuePart, argumentDisplayName, typeName
                                                ), false
                                            )
                                        );
                                    }
                                    else
                                    {
                                        errors.Add(
                                            new ParserError(
                                                Settings.Localization.Errors.InvalidArgumentValue.ToString(
                                                    valuePart, argumentDisplayName
                                                ), false
                                            )
                                        );
                                    }

                                    return defaultValue;
                                }
                            );

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
                                    Settings.Localization.Errors.InvalidArgumentValueWithType.ToString(
                                        cleanArgValue, argumentDisplayName, typeName
                                    ), true
                                )
                            );
                        }
                        else
                        {
                            errors.Add(
                                new ParserError(
                                    Settings.Localization.Errors.InvalidArgumentValue.ToString(
                                        cleanArgValue, argumentDisplayName
                                    ), false
                                )
                            );
                        }
                    }

                    if (!argument.IsValueAllowed(value))
                    {
                        errors.Add(
                            new ParserError(

                                // TODO: DisallowedArgumentValue message instead
                                Settings.Localization.Errors.InvalidArgumentValue.ToString(value, argumentDisplayName)
                            )
                        );
                    }

                    values.Add(value);
                }

                parsed[argument] = new ArgumentValues(argumentDisplayName, values);

                if (isPositional)
                {
                    ++positionalArguments;
                }
            }

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
                        parsed[argument] = ConstructDefaultArgument(argument, true);
                        omitted.Add(argument);
                    }

                    continue;
                }

                if (argument.IsPositional)
                {
                    errors.Add(
                        MissingArgumentError.Create(
                            Settings.Localization.Errors.MissingPositionalArgument, argument.Name, commandName
                        )
                    );
                }
                else
                {
                    errors.Add(MissingArgumentError.Create(Settings.PrefixLong, argument.Name, commandName));
                }

                missing.Add(argument);
            }

            return new ParserResult(command, new ArgumentValuesMap(parsed), errors, missing, omitted);
        }

        private ArgumentValuesMap ConstructDefaultArguments(IEnumerable<ICommandArgument> arguments)
        {
            return new ArgumentValuesMap(
                arguments.Where(argument => argument != null)
                    .Select(
                        argument => new KeyValuePair<ICommandArgument, ArgumentValues>(
                            argument, ConstructDefaultArgument(argument)
                        )
                    )
            );
        }

        private ArgumentValues ConstructDefaultArgument(ICommandArgument argument, bool isImplicit = false)
        {
            var argumentName = argument.ShortName == '\0'
                ? Settings.PrefixShort + argument.ShortName
                : Settings.PrefixLong + argument.Name;

            return argument.ValueType.IsArray
                ? new ArgumentValues(
                    argumentName, (argument.DefaultValue as IEnumerable)?.Cast<object>(), isImplicit: isImplicit
                )
                : new ArgumentValues(argumentName, isImplicit: isImplicit, values: argument.DefaultValue);
        }

        private bool TryParseArgument(
            Type type,
            object defaultValue,
            string source,
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
                        if (string.Equals(
                            Settings.Localization.ValueTrue.ToString(), source, StringComparison.OrdinalIgnoreCase
                        ))
                        {
                            success = true;
                            value = true;
                        }
                        else if (string.Equals(
                            Settings.Localization.ValueFalse.ToString(), source, StringComparison.OrdinalIgnoreCase
                        ))
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

        private static bool TryParseType(
            Type type,
            object defaultValue,
            string source,
            out object parsed
        )
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
