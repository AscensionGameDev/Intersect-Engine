namespace Intersect.Server.Web.Types;

public partial struct AdminActionParameters
{
    public string Moderator { get; set; }

    public int Duration { get; set; }

    public bool Ip { get; set; }

    public string Reason { get; set; }

    public byte X { get; set; }

    public byte Y { get; set; }

    public Guid MapId { get; set; }
}