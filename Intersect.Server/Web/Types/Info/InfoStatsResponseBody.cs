namespace Intersect.Server.Web.Types.Info;

public struct InfoStatsResponseBody(long uptime, long cps, int? connectedClients, int? onlineCount)
{
    public long Uptime { get; set; } = uptime;

    public long Cps { get; set; } = cps;

    public int? ConnectedClients { get; set; } = connectedClients;

    public int? OnlineCount { get; set; } = onlineCount;
}
