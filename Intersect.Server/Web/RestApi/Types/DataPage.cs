using System.Collections.Generic;

using Intersect.Server.Web.RestApi.Payloads;

namespace Intersect.Server.Web.RestApi.Types
{

    public struct DataPage<TValue>
    {

        public int Total { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int Count { get; set; }

        public IEnumerable<TValue> Values { get; set; }

        public IEnumerable<Sort> Sort { get; set; }

        public dynamic Extra { get; set; }

    }

}
