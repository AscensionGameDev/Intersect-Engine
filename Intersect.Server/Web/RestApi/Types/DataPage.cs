using System.Collections.Generic;

namespace Intersect.Server.Web.RestApi.Types
{
    public struct DataPage<TValue>
    {
        public int Total { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int Count { get; set; }

        public IEnumerable<TValue> Values { get; set; }
    }
}
