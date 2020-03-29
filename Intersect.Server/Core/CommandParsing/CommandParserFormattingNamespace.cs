using Intersect.Localization;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace Intersect.Server.Core.CommandParsing
{

    public sealed class CommandParserFormattingNamespace : LocaleNamespace
    {

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
        public readonly LocalizedString Optional = @"[{00}]";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
        public readonly LocalizedString Type = @":{00}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
        public readonly LocalizedString Usage = @"Usage: {00}";

    }

}
