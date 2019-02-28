using Intersect.Localization;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Intersect.Server.Core.CommandParsing
{
    public sealed class CommandParsingNamespace : LocaleNamespace
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
        public readonly CommandParserErrorsNamespace Errors = new CommandParserErrorsNamespace();

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
        public readonly LocalizedString ValueTrue =
            @"true";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
        public readonly LocalizedString ValueFalse =
            @"false";
    }
}