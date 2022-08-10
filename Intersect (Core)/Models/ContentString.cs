using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.Serialization;

using Intersect.Framework;

namespace Intersect.Models;

public class ContentString :
    IComparable<ContentString>,
    IDictionary<string, LocaleContentString>,
    IStronglyIdentifiedObject<ContentString>,
    IWeaklyIdentifiedObject
{
    public ContentString()
    {
        Id = new(Guid.NewGuid());
        Localizations = new HashSet<LocaleContentString>();
    }

    public ContentString(string value) : this()
    {
        var localeContentString = new LocaleContentString(
            this,
            CultureInfo.DefaultThreadCurrentCulture,
            value
        );
        this[localeContentString.Locale] = localeContentString;
    }

    public LocaleContentString? this[string key]
    {
        get => Localizations
            .FirstOrDefault(
                localeContentString =>
                    string.Equals(
                        localeContentString.Locale,
                        key,
                        StringComparison.OrdinalIgnoreCase
                    )
            );
        set
        {
            if (TryGetValue(key, out var existingLocalization))
            {
                _ = Localizations.Remove(existingLocalization);
            }
            Localizations.Add(value);
        }
    }

    Guid IWeaklyIdentifiedObject.Id => Id.Guid;

    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Id<ContentString> Id { get; init; }

    public string? Comment { get; set; }

    public ICollection<LocaleContentString> Localizations { get; set; }

    [IgnoreDataMember, NotMapped]
    public ICollection<string> Keys =>
        Localizations
            .Select(localeContentString => localeContentString.Locale)
            .ToList();

    [IgnoreDataMember, NotMapped]
    public ICollection<LocaleContentString> Values => Localizations;

    [IgnoreDataMember, NotMapped]
    public int Count => Localizations.Count;

    [IgnoreDataMember, NotMapped]
    public bool IsReadOnly => Localizations.IsReadOnly;

    public void Add(string key, LocaleContentString value) =>
        Add(new(key, value));

    public void Add(KeyValuePair<string, LocaleContentString> item)
    {
        ValidateKeyValuePair(item);
        Localizations.Add(item.Value);
    }

    public void Clear() => Localizations.Clear();

    public virtual int CompareTo(ContentString? other) =>
        other == default ? -1 : ((string)this).CompareTo((string)other);

    public bool Contains(KeyValuePair<string, LocaleContentString> item)
    {
        ValidateKeyValuePair(item);
        return Localizations.Contains(item.Value);
    }

    public bool ContainsKey(string key) =>
        Localizations.Any(
            localeContentString =>
                string.Equals(
                    key,
                    localeContentString.Locale,
                    StringComparison.OrdinalIgnoreCase
                )
        );

    public void CopyTo(KeyValuePair<string, LocaleContentString>[] array, int arrayIndex) =>
        AsKeyValuePairs().ToList().CopyTo(array, arrayIndex);

    public IEnumerator<KeyValuePair<string, LocaleContentString>> GetEnumerator() =>
        AsKeyValuePairs().GetEnumerator();

    public bool Matches(string @string, StringComparison stringComparison) =>
        string.Equals(Comment, @string, stringComparison)
        || Localizations.Any(lcs => string.Equals(lcs, @string, stringComparison));

    public bool Remove(string key) => TryGetValue(key, out var existing) && Localizations.Remove(existing);

    public bool Remove(KeyValuePair<string, LocaleContentString> item)
    {
        ValidateKeyValuePair(item);
        return Localizations.Remove(item.Value);
    }

    protected IEnumerable<KeyValuePair<string, LocaleContentString>> AsKeyValuePairs() =>
        Localizations
            .Select(
                localeContentString =>
                    new KeyValuePair<string, LocaleContentString>(
                        localeContentString.Locale,
                        localeContentString
                    )
            );

    public override string ToString()
    {
        if (TryGetValue(CultureInfo.CurrentCulture.IetfLanguageTag, out var localized))
        {
            return localized;
        }

        var fallbackCulture = CultureInfo.DefaultThreadCurrentCulture ?? CultureInfo.InvariantCulture;
        if (TryGetValue(fallbackCulture.IetfLanguageTag, out localized))
        {
            return localized;
        }

        return Localizations.OrderBy(localeContentString => localeContentString.Locale).First();
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out LocaleContentString value)
    {
        value = this[key];
        return value != default;
    }

    protected void ValidateKeyValuePair(KeyValuePair<string, LocaleContentString> item)
    {
        if (string.Equals(item.Key, item.Value.Locale, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException($"Key '{item.Key}' does not match Value's locale '{item.Value.Locale}'.", nameof(item));
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public static implicit operator string(ContentString contentString) =>
        contentString?.ToString();
}
