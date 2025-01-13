using System.ComponentModel;
using System.Globalization;
using Intersect.Server.Localization;
using Intersect.Utilities;

namespace Intersect.Server.Web.RestApi.Types;

// TODO: Figure out how to get LookupKey to show up in swagger.json components/schemas despite being a "string", one or more of the following commented out attributes may help
// [SwaggerSubType(typeof(Guid))]
// [SwaggerSubType(typeof(string))]
// [KnownType(typeof(LookupKey))]
// [SwaggerSchema]
// [TypeConverter(typeof(Converter))]
public partial struct LookupKey
{

    public bool HasName => !string.IsNullOrWhiteSpace(Name);

    public bool HasId => Guid.Empty != Id;

    public bool IsNameInvalid => !HasId && Name != null;

    public bool IsIdInvalid => !HasId && Name == null;

    public bool IsInvalid => !HasId && !HasName;

    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public override string ToString()
    {
        return HasId ? Id.ToString() : Name;
    }

    public static bool TryParse(string input, out LookupKey lookupKey)
    {
        if (Guid.TryParse(input, out var guid))
        {
            lookupKey = new LookupKey
            {
                Id = guid,
            };
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

        lookupKey = new LookupKey
        {
            Name = input,
        };
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
            if (Guid.TryParse(value as string, out var guid))
            {
                return new LookupKey
                {
                    Id = guid,
                };
            }

            return new LookupKey
            {
                Name = value as string,
            };
        }
    }
}
