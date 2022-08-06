using System.Globalization;
using System.Runtime.Serialization;

using Intersect.Collections;

using Newtonsoft.Json;

namespace Intersect.Config;

public partial class LocalizationOptions
{
    private readonly ConverterList<CultureInfo, string> _supportedLocales;

    public LocalizationOptions()
    {
        DefaultCultureInfo = CultureInfo.DefaultThreadCurrentCulture;
        SupportedCultureInfos = new() { DefaultCultureInfo };

        _supportedLocales = new(
            enforceSorting: true,
            enforceUniqueness: true,
            equalityComparer: (cultureInfo, ietfLanguageTag) =>
                string.Equals(
                    cultureInfo.IetfLanguageTag,
                    ietfLanguageTag,
                    StringComparison.OrdinalIgnoreCase
                ),
            retrievalConversion: cultureInfo => cultureInfo.IetfLanguageTag,
            storageConversion: ietfLanguageTag => CultureInfo.GetCultureInfo(ietfLanguageTag),
            wrappedList: SupportedCultureInfos
        );
    }

    [IgnoreDataMember]
    public CultureInfo DefaultCultureInfo { get; set; }

    [DataMember(EmitDefaultValue = true, IsRequired = false)]
    public string DefaultLocale
    {
        get => DefaultCultureInfo.IetfLanguageTag;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("The default locale must be a valid IETF language tag but was either null or empty.", nameof(value));
            }

            DefaultCultureInfo = CultureInfo.GetCultureInfo(value);
        }
    }

    [IgnoreDataMember]
    public List<CultureInfo> SupportedCultureInfos { get; }

    [DataMember(EmitDefaultValue = true, IsRequired = false)]
    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public virtual IList<string> SupportedLocales
    {
        get => _supportedLocales;
        set
        {
            _supportedLocales.Clear();
            _supportedLocales.AddRange(value);

            if (!SupportedCultureInfos.Contains(DefaultCultureInfo))
            {
                SupportedCultureInfos.Add(DefaultCultureInfo);
            }
        }
    }
}
