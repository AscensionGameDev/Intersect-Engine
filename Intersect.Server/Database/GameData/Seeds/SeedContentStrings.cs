using System.Globalization;

using Intersect.Framework;
using Intersect.Models;

using Microsoft.EntityFrameworkCore;

namespace Intersect.Server.Database.GameData.Seeds;

internal partial class SeedContentStrings : SeedData<ContentString, LocaleContentString>
{
    public delegate void PrivateSetId<TObject>(TObject @object, Id<TObject> id);

    public override void Seed(DbSet<ContentString> dbSetContentString, DbSet<LocaleContentString> dbSetLocaleContentString)
    {
        var chunkSize = 16;
        var count = 256;

        var locales = new List<CultureInfo>
        {
            CultureInfo.GetCultureInfo("en-US"),
            CultureInfo.GetCultureInfo("es-ES"),
            CultureInfo.GetCultureInfo("fr-FR"),
            CultureInfo.GetCultureInfo("it"),
            CultureInfo.GetCultureInfo("de"),
        };

        var contentString_set_Id = Delegate.CreateDelegate(
            typeof(PrivateSetId<ContentString>),
            typeof(ContentString).GetProperty(nameof(ContentString.Id)).SetMethod
        ) as PrivateSetId<ContentString>;

        var chunks = count / chunkSize;
        for (var chunkIndex = 0; chunkIndex < chunks; chunkIndex++)
        {
            var contentStrings = Enumerable
                .Range(0, chunkSize)
                .Select(index =>
                {
                    var absoluteIndex = chunkIndex * chunkSize + index;
                    var contentString = new ContentString
                    {
                        Comment = (absoluteIndex % chunkSize < chunkSize / 4)
                            ? default
                            : new(
                                new char[absoluteIndex / 2 % 95]
                                    .Select((_, charIndex) => (char)(' ' + charIndex))
                                    .ToArray()
                            )
                    };

                    var octets = new byte[12]
                        .Concat(BitConverter.GetBytes(absoluteIndex))
                        .ToArray();
                    contentString_set_Id(contentString, new(new(octets)));

                    var localeCount = 1 + index % (locales.Count - 1);
                    var localeStartIndex = index % locales.Count;

                    var localeContentStrings = new LocaleContentString[localeCount];
                    for (var localeIndex = 0; localeIndex < localeCount; localeIndex++)
                    {
                        var locale = locales[(localeStartIndex + localeIndex) % locales.Count];
                        localeContentStrings[localeIndex] = new LocaleContentString(
                            contentString,
                            locale,
                            $"test:{index:0000}:{locale.IetfLanguageTag}:{locale.NativeName}"
                        );
                    }

                    foreach (var localeContentString in localeContentStrings)
                    {
                        contentString.Localizations[localeContentString.Locale] = localeContentString;
                    }

                    return contentString;
                })
                .ToArray();

            dbSetContentString.AddRange(contentStrings);

            dbSetLocaleContentString.AddRange(contentStrings.SelectMany(contentString => contentString.Localizations.Values));
        }
    }
}
