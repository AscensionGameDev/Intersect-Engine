using Intersect.Localization;

using Newtonsoft.Json;

namespace Intersect.Editor.Localization.MenuBar;

internal partial class EditMenuNamespace : MenuNamespace
{
    public EditMenuNamespace() : base(@"Edit") { }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString Undo = @"Undo";

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString Redo = @"Redo";

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString Cut = @"Cut";

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString Copy = @"Copy";

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString Paste = @"Paste";
}
