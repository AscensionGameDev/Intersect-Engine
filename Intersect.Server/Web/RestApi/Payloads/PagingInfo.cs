namespace Intersect.Server.Web.RestApi.Payloads
{
    public partial class PagingInfo
    {
        public int Page { get; set; }

        public int Count { get; set; } = 10;
    }
}
