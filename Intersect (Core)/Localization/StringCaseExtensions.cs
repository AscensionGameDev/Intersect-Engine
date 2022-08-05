namespace Intersect.Localization;

public static class StringCaseExtensions
{
    public static Pluralization Pluralize(this StringCase stringCase, int count) =>
        (stringCase switch
        {
            StringCase.Default => count == 1 ? Pluralization.Singular : Pluralization.Plural,
            StringCase.Lower => count == 1 ? Pluralization.SingularLower : Pluralization.PluralLower,
            StringCase.Upper => count == 1 ? Pluralization.SingularUpper : Pluralization.PluralUpper,
            _ => throw new ArgumentOutOfRangeException(nameof(stringCase)),
        });
}
