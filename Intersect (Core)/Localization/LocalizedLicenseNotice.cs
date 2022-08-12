using System.Runtime.Serialization;

using Newtonsoft.Json;

namespace Intersect.Localization;

public partial class LocalizedLicenseNotice : Localized
{
    [DataMember(Name = nameof(LongNotice))]
    [JsonProperty(nameof(LongNotice), DefaultValueHandling = DefaultValueHandling.Ignore)]
    private readonly LocalizedString _longNotice;

    [DataMember(Name = nameof(Notice))]
    [JsonProperty(nameof(Notice), DefaultValueHandling = DefaultValueHandling.Ignore)]
    private readonly LocalizedString _notice;

    [DataMember(Name = nameof(ShortNotice))]
    [JsonProperty(nameof(ShortNotice), DefaultValueHandling = DefaultValueHandling.Ignore)]
    private readonly LocalizedString _shortNotice;

    [IgnoreDataMember]
    public LocalizedString LongNotice => _longNotice.IsNull ? _notice : _longNotice;

    [IgnoreDataMember]
    public LocalizedString Notice => _notice;

    [IgnoreDataMember]
    public LocalizedString ShortNotice => _shortNotice.IsNull ? _notice : _shortNotice;

    [JsonConstructor]
    private LocalizedLicenseNotice() { }

    private LocalizedLicenseNotice(
        LocalizedString? notice,
        LocalizedString? longNotice,
        LocalizedString? shortNotice
    )
    {
        _longNotice = longNotice ?? new(default!);
        _notice = LocalizedString.PickNonNull(notice, longNotice, shortNotice) ?? throw new ArgumentNullException(nameof(notice));
        _shortNotice = shortNotice ?? new(default!);
    }

    public LocalizedLicenseNotice(LocalizedString notice)
        : this(notice, default, default) { }

    public LocalizedLicenseNotice(LocalizedString longNotice, LocalizedString shortNotice)
        : this(default, longNotice, shortNotice) { }

    public override string ToString() => ToString(LicenseNoticeType.Normal);

    public string ToString(LicenseNoticeType licenseNoticeType = LicenseNoticeType.Normal, params object[] args) =>
        (licenseNoticeType switch
        {
            LicenseNoticeType.Long => LongNotice,
            LicenseNoticeType.Normal => Notice,
            LicenseNoticeType.Short => ShortNotice,
            _ => throw new ArgumentOutOfRangeException(nameof(licenseNoticeType)),
        }).ToString(args);

    public static implicit operator LocalizedLicenseNotice(string value) => new(value);
}

public enum LicenseNoticeType
{
    Long,

    Normal,

    Short,
}
