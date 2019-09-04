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
    public struct PagingInfo
    {

        public int Page { get; set; }

        public int Count { get; set; }

    }
}
