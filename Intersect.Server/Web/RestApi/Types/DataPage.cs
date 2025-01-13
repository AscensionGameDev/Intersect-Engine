using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Intersect.Server.Web.RestApi.Types;

public readonly struct DataPage<TValue>(
    int Total,
    int Page,
    int PageSize,
    int Count,
    IEnumerable<TValue> Values,
    IEnumerable<Sort>? Sort = null,
    dynamic? Extra = null
) : IDataPage<TValue>
{
    public DataPage() : this(
        0,
        0,
        0,
        0,
        []
    )
    {
    }

    public int Total { get; init; } = Total;

    public int Page { get; init; } = Page;

    public int PageSize { get; init; } = PageSize;

    public int Count { get; init; } = Count;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<Sort>? Sort { get; init; } = Sort;

    public IEnumerable<TValue> Values { get; init; } = Values;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public dynamic? Extra { get; init; } = Extra;
}