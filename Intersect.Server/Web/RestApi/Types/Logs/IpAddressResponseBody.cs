namespace Intersect.Server.Web.RestApi.Types.Logs;

public partial class IpAddressResponseBody
{
    public string Ip { get; set; }

    public DateTime LastUsed { get; set; }

    public Dictionary<Guid, string> OtherUsers { get; set; } = [];
}
