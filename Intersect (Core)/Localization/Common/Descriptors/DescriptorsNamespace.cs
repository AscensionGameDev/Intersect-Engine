using Intersect.Localization.Common.Descriptors.Maps;

using Newtonsoft.Json;

namespace Intersect.Localization.Common.Descriptors;

public partial class DescriptorsNamespace : LocaleNamespace
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly MapDescriptorNamespace Map = new();
}
