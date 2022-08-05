namespace Intersect.Localization;

public static class PluralizationExtensions
{
    public static LocalizedString SelectFrom(this Pluralization pluralization, LocalizedPluralString localizedPluralString) =>
        pluralization switch
        {
            Pluralization.Singular => localizedPluralString.Singular,
            Pluralization.SingularLower => localizedPluralString.SingularLower,
            Pluralization.SingularUpper => localizedPluralString.SingularUpper,
            Pluralization.Plural => localizedPluralString.Plural,
            Pluralization.PluralLower => localizedPluralString.PluralLower,
            Pluralization.PluralUpper => localizedPluralString.PluralUpper,
            _ => throw new ArgumentOutOfRangeException(nameof(pluralization)),
        };
}
