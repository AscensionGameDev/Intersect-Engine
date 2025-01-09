namespace Intersect.Server.Web.RestApi.Payloads;

// TODO: Struct, right now there's an exception when it's a struct and I'm not sure how to fix it
public class PagingInfo
{
    public const int MaxPageSize = 100;
    public const int MinPageSize = 5;
    public const int DefaultPageSize = 10;

    public int Page { get; set; } = 0;

    public int Count { get; set; } = DefaultPageSize;
}