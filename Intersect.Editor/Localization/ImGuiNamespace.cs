using Intersect.Localization;

using Newtonsoft.Json;

namespace Intersect.Editor.Localization;

internal partial class ImGuiNamespace : LocaleNamespace
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString ImGuiDebugger = @"ImGui Debugger";

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString ShowMetricsWindow = @"Show Metrics Window";

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString ShowStackToolWindow = @"Show Stack Tool Window";

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString ShowStyleEditor = @"Show Style Editor";
}
