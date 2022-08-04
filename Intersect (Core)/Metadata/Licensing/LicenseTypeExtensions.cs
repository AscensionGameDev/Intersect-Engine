using Intersect.Localization;
using Intersect.Localization.Common;

namespace Intersect.Metadata.Licensing;

public static class LicenseTypeExtensions
{
    public static LocalizedString GetLongNotice(this LicenseType licenseType, LicensingNamespace licensingNamespace) => licenseType switch
    {
        LicenseType.GPLv3 => licensingNamespace.GPLv3LongNotice,
        LicenseType.MIT => licensingNamespace.MITNotice,
        _ => throw new ArgumentOutOfRangeException(nameof(licenseType)),
    };

    public static LocalizedString GetShortNotice(this LicenseType licenseType, LicensingNamespace licensingNamespace) => licenseType switch
    {
        LicenseType.GPLv3 => licensingNamespace.GPLv3ShortNotice,
        LicenseType.MIT => licensingNamespace.MITNotice,
        _ => throw new ArgumentOutOfRangeException(nameof(licenseType)),
    };
}
