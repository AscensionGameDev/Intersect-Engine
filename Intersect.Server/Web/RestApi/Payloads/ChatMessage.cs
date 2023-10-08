﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Net.Http;
using System.Web.Http.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace Intersect.Server.Web.RestApi.Payloads
{

    //[TypeConverter(typeof(Converter))]
    public partial struct ChatMessage
    {

        public string Message { get; set; }

        public Color Color { get; set; }

        public string Target { get; set; }

        public partial class Converter : TypeConverter
        {

            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                return typeof(string) == sourceType;
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                if (value == null)
                {
                    return default(ChatMessage);
                }

                if (typeof(string) != value.GetType())
                {
                    throw new ArgumentException();
                }

                return JsonConvert.DeserializeObject<ChatMessage>(value as string);
            }

        }

        internal sealed partial class Constraint : IHttpRouteConstraint
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

        internal sealed partial class RouteConstraint : IRouteConstraint
        {
            public bool Match(
                HttpContext httpContext,
                IRouter route,
                string routeKey,
                RouteValueDictionary values,
                RouteDirection routeDirection
            )
            {
                return values.TryGetValue(routeKey, out var value) && value != null;
            }
        }

    }

}
