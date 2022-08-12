using Intersect.Localization;

using Newtonsoft.Json;

namespace Intersect.Editor.Localization.MenuBar;

internal partial class MenuBarNamespace : LocaleNamespace
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly FileMenuNamespace File = new();

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly EditMenuNamespace Edit = new();

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly ViewMenuNamespace View = new();

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly SelectionMenuNamespace Selection = new();

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly EditorsMenuNamespace ContentEditors = new();

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly HelpMenuNamespace Help = new();
}
