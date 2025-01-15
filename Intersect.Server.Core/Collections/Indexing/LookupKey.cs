using System.ComponentModel;
using System.Globalization;
using Intersect.Server.Localization;
using Intersect.Utilities;

namespace Intersect.Server.Collections.Indexing;

// TODO: Figure out how to get LookupKey to show up in swagger.json components/schemas despite being a "string", one or more of the following commented out attributes may help
// [SwaggerSubType(typeof(Guid))]
// [SwaggerSubType(typeof(string))]
// [KnownType(typeof(LookupKey))]
// [SwaggerSchema]
// [TypeConverter(typeof(Converter))]
/// <summary>
/// An ID (<see cref="Guid"/>) or a name (<see cref="string"/>) used to look up an entity.
/// </summary>
[Description($"{nameof(LookupKey)}_Description")]
public readonly record struct LookupKey
{
    // ReSharper disable once MemberCanBePrivate.Global
    public LookupKey(Guid id)
    {
        Id = id;
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public LookupKey(string name)
    {
        Name = name;
    }

    /// <summary>
    /// The ID of the entity, if this <see cref="LookupKey"/> is a <see cref="Guid"/>.
    /// </summary>
    /// <seealso cref="IsId"/>
    /// <seealso cref="IsIdInvalid"/>
    public readonly Guid Id;

    /// <summary>
    /// The name of the entity, if this <see cref="LookupKey"/> is a <see cref="string"/>.
    /// </summary>
    /// <seealso cref="IsName"/>
    /// <seealso cref="IsNameInvalid"/>
    public readonly string Name;

    /// <summary>
    /// If this <see cref="LookupKey"/> is a <see cref="string"/>.
    /// </summary>
    public bool IsName => !string.IsNullOrWhiteSpace(Name);

    /// <summary>
    /// If this <see cref="LookupKey"/> is a <see cref="Guid"/>.
    /// </summary>
    public bool IsId => Guid.Empty != Id;

    public bool IsNameInvalid => !IsId && Name != null;

    public bool IsIdInvalid => !IsId && Name == null;

    public bool IsInvalid => !IsId && !IsName;

    public bool Matches(Guid id, string name) => (IsId && Id == id) ||
                                                 (IsName && string.Equals(
                                                     Name,
                                                     name,
                                                     StringComparison.OrdinalIgnoreCase
                                                 ));

    public override string ToString()
    {
        if (IsInvalid)
        {
            return $"{{ {nameof(IsInvalid)}={true}, {nameof(Id)}={Id}, {nameof(Name)}={Name} }}";
        }

        return IsId ? Id.ToString() : Name;
    }

    public static implicit operator LookupKey(Guid id) => new(id);

    public static implicit operator LookupKey(string input) =>
        TryParse(input, out var lookupKey)
            ? lookupKey
            : throw new InvalidCastException($"'{input}' is not a valid {nameof(LookupKey)} representation");

    // ReSharper disable once MemberCanBePrivate.Global
    public static bool TryParse(string input, out LookupKey lookupKey)
    {
        if (Guid.TryParse(input, out var guid))
        {
            lookupKey = new LookupKey(guid);
            return true;
        }

        if (string.IsNullOrWhiteSpace(input))
        {
            lookupKey = default;
            return false;
        }

        if (!FieldChecking.IsValidUsername(input, Strings.Regex.Username))
        {
            lookupKey = default;
            return false;
        }

        lookupKey = new LookupKey(input);
        return true;
    }

    public partial class Converter : TypeConverter
    {

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return typeof(string) == sourceType || typeof(Guid) == sourceType;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            LookupKey lookupKey = value as string;
            return lookupKey;
        }
    }
}
