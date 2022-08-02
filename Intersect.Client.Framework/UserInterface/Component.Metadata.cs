using Intersect.Localization;

namespace Intersect.Client.Framework.UserInterface;

public partial class Component
{
    public delegate string CustomFormatter(LocalizedString? localizedString);

    protected ReactiveString CreateLabelWithId(LocalizedString? localizedString, CustomFormatter? customFormatter = default) =>
        new(
            localizedString,
            (text) => $"{customFormatter?.Invoke(text) ?? text}###{_metadata.Id}"
        );
}
