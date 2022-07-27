using Intersect.Localization;

using Newtonsoft.Json;

namespace Intersect.Editor.Localization.MenuBar;

internal partial class FileMenuNamespace : MenuNamespace
{
    public FileMenuNamespace() : base(@"File") { }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString Preferences = @"Preferences";

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString PackageGame = @"Package Game";

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString Exit = @"Exit";
}
