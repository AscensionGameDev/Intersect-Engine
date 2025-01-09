namespace Intersect.Server.Web.RestApi.Payloads;

public struct PagingInfo()
{
    public const int MaxPageSize = 100;
    public const int MinPageSize = 5;
    public const int DefaultPageSize = 10;

    public int Page { get; set; } = 0;

    public int Count { get; set; } = 10;
}