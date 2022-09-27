using Intersect.Localization;

namespace Intersect.Editor.Interface;

public partial class Component
{
    public delegate string CustomFormatter(LocalizedString? localizedString);

    protected ReactiveString CreateLabelWithId(LocalizedString? localizedString, CustomFormatter? customFormatter = default) =>
        new(
            localizedString,
            (text) => $"{customFormatter?.Invoke(text) ?? text}###{_metadata.Id}"
        );

    protected static ReactiveString CreateLabelWithId(LocalizedString? localizedString, string id) =>
        new(
            localizedString,
            (text) => $"{text}###{id}"
        );
}
