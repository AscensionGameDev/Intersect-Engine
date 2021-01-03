using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Net.Http;
using System.Web.Http.Routing;

namespace Intersect.Server.Web.RestApi.Payloads
{

    [TypeConverter(typeof(Converter))]
    public struct LookupKey
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

        public class Converter : TypeConverter
        {

            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                return typeof(string) == sourceType || typeof(Guid) == sourceType;
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                if (value == null)
                {
                    return default(LookupKey);
                }

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

        internal sealed class Constraint : IHttpRouteConstraint
        {

            /// <inheritdoc />
            public bool Match(
                HttpRequestMessage request,
                IHttpRoute route,
                string parameterName,
                IDictionary<string, object> values,
                HttpRouteDirection routeDirection
            )
            {
                return values.TryGetValue(parameterName, out var value) && value != null;
            }

        }

    }

}
