using Intersect.Localization;

using Newtonsoft.Json;

namespace Intersect.Editor.Localization.MenuBar;

internal abstract partial class MenuNamespace : LocaleNamespace
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    protected readonly LocalizedString Name;

    protected MenuNamespace(LocalizedString name) => Name = name;

    public static implicit operator LocalizedString(MenuNamespace menuNamespace) =>
        menuNamespace.Name;

    public override string ToString() => Name;
}
