using Intersect.Server.Collections.Sorting;

namespace Intersect.Server.Web.RestApi.Types;

public interface IDataPage<TValue>
{
    int Total { get; init; }
    int Page { get; init; }
    int PageSize { get; init; }
    int Count { get; init; }
    IEnumerable<Sort> Sort { get; init; }
    IEnumerable<TValue> Values { get; init; }
    dynamic Extra { get; init; }
}