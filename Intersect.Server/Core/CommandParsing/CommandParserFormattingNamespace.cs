using Intersect.Localization;

using Newtonsoft.Json;

namespace Intersect.Server.Core.CommandParsing
{

    public sealed class CommandParserFormattingNamespace : LocaleNamespace
    {

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]        public readonly LocalizedString Optional = @"[{00}]";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]        public readonly LocalizedString Type = @":{00}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]        public readonly LocalizedString Usage = @"Usage: {00}";

    }

}
