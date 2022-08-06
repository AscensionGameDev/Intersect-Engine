using Intersect.Localization;

using Newtonsoft.Json;

namespace Intersect.Editor.Localization.Windows;

internal partial class WindowsNamespace : LocaleNamespace
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly AboutWindowNamespace About = new();

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly DescriptorWindowNamespace Descriptor = new();

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizationWindowNamespace Localization = new();
}
