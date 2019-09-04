using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Net.Http;
using System.Web.Http.Routing;

using Newtonsoft.Json;

namespace Intersect.Server.Web.RestApi.Payloads
{
    public struct VariableValue
    {
        public dynamic Value { get; set; }
    }
}
