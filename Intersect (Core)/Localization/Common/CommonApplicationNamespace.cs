using Newtonsoft.Json;

namespace Intersect.Localization.Common;

public partial class CommonApplicationNamespace : LocaleNamespace
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString Name = @"Intersect";
}
