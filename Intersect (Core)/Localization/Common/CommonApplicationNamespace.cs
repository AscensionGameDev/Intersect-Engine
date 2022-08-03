using Newtonsoft.Json;

namespace Intersect.Localization.Common;

public partial class CommonApplicationNamespace : LocaleNamespace
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString Name = @"Intersect";

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString Version = @"v{00}";

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString VersionSuffixDebug = @"{00}-debug";
}
