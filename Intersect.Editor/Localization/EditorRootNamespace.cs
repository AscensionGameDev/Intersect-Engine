using Intersect.Editor.Localization.MenuBar;
using Intersect.Localization.Common;

using Newtonsoft.Json;

namespace Intersect.Editor.Localization;

internal partial class EditorRootNamespace : RootNamespace
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public new readonly EditorApplicationNamespace Application = new();

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly MenuBarNamespace MenuBar = new();
}
