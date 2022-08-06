using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Runtime.Serialization;

using Intersect.Framework;

namespace Intersect.Models;

public sealed class LocaleContentString
{
    public LocaleContentString(ContentString contentString, string locale, string? value)
        : this(contentString.Id, locale, value)
    {
        ContentString = contentString;
    }

    public LocaleContentString(Id<ContentString> id, string locale, string value)
    {
        Id = id;
        Locale = locale ?? throw new ArgumentNullException(nameof(locale));
        Value = string.IsNullOrEmpty(value) ? default : value;
    }
    public LocaleContentString(ContentString contentString, CultureInfo cultureInfo, string? value)
        : this(contentString, cultureInfo.IetfLanguageTag, value) { }

    public LocaleContentString(Id<ContentString> id, CultureInfo cultureInfo, string value)
        : this(id, cultureInfo.IetfLanguageTag, value) { }

    public Id<ContentString> Id { get; set; }

    [ForeignKey(nameof(Id))]
    [IgnoreDataMember, NotMapped]
    public ContentString ContentString { get; set; }

    public string Locale { get; set; }

    public string Value { get; set; }

    public override string ToString() => Value;

    public static implicit operator string(LocaleContentString str) => str?.ToString();
}

public sealed class ContentString
{
    public ContentString()
    {
        Id = new(Guid.NewGuid());
    }

    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Id<ContentString> Id { get; private set; }

    public string? Comment { get; set; }

    [IgnoreDataMember, NotMapped]
    public Dictionary<string, LocaleContentString> Localizations { get; set; } = new();

    [DataMember(Name = nameof(Localizations))]
#pragma warning disable IDE0051 // Remove unused private members
    private ISet<LocaleContentString> LocalizationsBinder
#pragma warning restore IDE0051 // Remove unused private members
    {
        get => Localizations.Values.ToHashSet();
        set => Localizations = value?.ToDictionary(lcs => lcs.Locale);
    }

    public override string ToString()
    {
        if (Localizations.TryGetValue(CultureInfo.CurrentCulture.IetfLanguageTag, out var localized))
        {
            return localized;
        }

        var fallbackCulture = CultureInfo.DefaultThreadCurrentCulture ?? CultureInfo.InvariantCulture;
        if (Localizations.TryGetValue(fallbackCulture.IetfLanguageTag, out localized))
        {
            return localized;
        }

        return Localizations.OrderBy(kvp => kvp.Key).FirstOrDefault().Value;
    }

    public static implicit operator string(ContentString str) => str?.ToString();
}
