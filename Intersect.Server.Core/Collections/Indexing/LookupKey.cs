﻿using System.ComponentModel;
using System.Globalization;

namespace Intersect.Server.Web.RestApi.Payloads
{

    [TypeConverter(typeof(Converter))]
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
                        Id = guid
                    };
                }

                return new LookupKey
                {
                    Name = value as string
                };
            }
        }
    }
}
