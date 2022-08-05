using System.Runtime.Serialization;

namespace Intersect.Localization;

public partial class LocalizedPluralString : Localized
{
    [DataMember(Name = nameof(Singular))]
    private readonly LocalizedString _singular;

    [DataMember(Name = nameof(SingularLower), EmitDefaultValue = false)]
    private readonly LocalizedString _singularLower;

    [DataMember(Name = nameof(SingularUpper), EmitDefaultValue = false)]
    private readonly LocalizedString _singularUpper;

    [DataMember(Name = nameof(Plural))]
    private readonly LocalizedString _plural;

    [DataMember(Name = nameof(PluralLower), EmitDefaultValue = false)]
    private readonly LocalizedString _pluralLower;

    [DataMember(Name = nameof(PluralUpper), EmitDefaultValue = false)]
    private readonly LocalizedString _pluralUpper;

    [IgnoreDataMember]
    public LocalizedString Singular => _singular;

    [IgnoreDataMember]
    public LocalizedString SingularLower => LocalizedString.PickNonNull(_singularLower, _singular);

    [IgnoreDataMember]
    public LocalizedString SingularUpper => LocalizedString.PickNonNull(_singularUpper, _singular);

    [IgnoreDataMember]
    public LocalizedString Plural => _plural;

    [IgnoreDataMember]
    public LocalizedString PluralLower => _pluralLower;

    [IgnoreDataMember]
    public LocalizedString PluralUpper => _pluralUpper;

    public LocalizedPluralString(
        LocalizedString singular,
        LocalizedString? plural = default,
        LocalizedString? singularLower = default,
        LocalizedString? singularUpper = default,
        LocalizedString? pluralLower = default,
        LocalizedString? pluralUpper = default
    )
    {
        _singular = singular ?? throw new ArgumentNullException(nameof(singular));
        _singularLower = singularLower ?? new(_singular.ToString().ToLowerInvariant());
        _singularUpper = singularUpper ?? new(_singular.ToString().ToUpperInvariant());
        _plural = plural ?? new($"{singular}s");
        _pluralLower = pluralLower ?? new((singularLower ?? _plural).ToString().ToLowerInvariant());
        _pluralUpper = pluralUpper ?? new((singularUpper ?? _plural).ToString().ToUpperInvariant());
    }

    public override string ToString() => ToString(1);

    public string ToString(int count, params object[] args) =>
        ToString(count, StringCase.Default, args);

    public string ToString(int count, StringCase stringCase, params object[] args) =>
        stringCase.Pluralize(count).SelectFrom(this).ToString(args);

    public static implicit operator LocalizedPluralString(string value) => new(value);

    public static implicit operator string(LocalizedPluralString? str) => str?.Singular;
}
