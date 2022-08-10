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

    public bool MatchesCulture(CultureInfo cultureInfo) =>
        string.Equals(Locale, cultureInfo.IetfLanguageTag, StringComparison.OrdinalIgnoreCase);

    public override string ToString() => Value;

    public static implicit operator string(
        LocaleContentString localeContentString
    ) => localeContentString?.ToString();
}
