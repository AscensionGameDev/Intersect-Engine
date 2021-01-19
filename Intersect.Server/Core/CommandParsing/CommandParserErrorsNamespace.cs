using Intersect.Localization;

using Newtonsoft.Json;

namespace Intersect.Server.Core.CommandParsing
{

    public sealed class CommandParserErrorsNamespace : LocaleNamespace
    {

        /// <summary>
        /// Argument matches neither the short nor long argument name format and is not positional.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]        public readonly LocalizedString BadArgumentFormat =
            @"The argument '{00}' is not a valid short or long-form argument.";

        /// <summary>
        /// Command not found for the given name.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]        public readonly LocalizedString CommandNotFound =
            @"The command '{00}' is not recoginized. Enter '{01}' for a list of commands.";

        /// <summary>
        /// Named argument was specified multiple times.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]        public readonly LocalizedString DuplicateNamedArgument = @"The argument '{00}' was specified more than once.";

        /// <summary>
        /// Flag argument is provided a value, but they do not accept them.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]        public readonly LocalizedString FlagArgumentsIgnoreValue =
            @"'{00}' is a flag argument and will ignore provided values.";

        /// <summary>
        /// Generic parser error message.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]        public readonly LocalizedString GenericError = @"An error occurred while trying to parse the command.";

        /// <summary>
        /// Argument matches both the short and long argument name formats.
        ///
        /// Note that should not actually be possible.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]        public readonly LocalizedString IllegalArgumentFormat =
            @"The argument '{00}' matches both the short and long-form argument formats (e.g. '{01}h' and '{02}help').";

        /// <summary>
        /// The value provided is not valid for this argument.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]        public readonly LocalizedString
            InvalidArgumentValue = @"The value '{00}' is not valid for the argument '{01}'.";

        /// <summary>
        /// The value provided is not valid for this argument, expected one of the specified type.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]        public readonly LocalizedString InvalidArgumentValueWithType =
            @"The value '{00}' is not valid for the argument '{01}' (expected type '{02}').";

        /// <summary>
        /// Required arguments were missing.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]        public readonly LocalizedString MissingArguments = @"Missing one or more arguments: {00}";

        /// <summary>
        /// List delimeter for missing arguments.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]        public readonly LocalizedString MissingArgumentsDelimeter = @", ";

        /// <summary>
        /// Named argument is required but missing for the given command.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]        public readonly LocalizedString MissingNamedArgument =
            @"The argument '{00}{01}' is required for the command '{02}' but is missing.";

        /// <summary>
        /// Positional argument is required but missing for the given command.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]        public readonly LocalizedString MissingPositionalArgument =
            @"The argument '{00}' is required for the command '{01}' but is missing.";

        /// <summary>
        /// No input was provided.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]        public readonly LocalizedString NoInput =
            @"No input was provided. If this is not the case, please report this error.";

        /// <summary>
        /// Named argument is not valid for the given command.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]        public readonly LocalizedString UnhandledNamedArgument =
            @"The argument '{00}' is not accepted for the command '{01}'.";

        /// <summary>
        /// Positional argument is not valid for the given command.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]        public readonly LocalizedString UnhandledPositionalArgument =
            @"The argument '{00}' in position {01} is not accepted for the command '{02}'.";

    }

}
