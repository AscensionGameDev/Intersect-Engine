namespace Intersect.Localization;

public partial class LocalizedPluralString : Localized
{
    public readonly LocalizedString Singular;

    public readonly LocalizedString Plural;

    public LocalizedPluralString(LocalizedString singular, LocalizedString plural)
    {
        Singular = singular;
        Plural = plural;
    }

    public override string ToString() => ToString(1);

    public string ToString(int count, params object[] args) =>
        (count == 1 ? Singular : Plural).ToString(args);
}
