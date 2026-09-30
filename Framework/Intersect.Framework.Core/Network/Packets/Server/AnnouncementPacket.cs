using MessagePack;

namespace Intersect.Network.Packets.Server;



/// <summary>
/// Defines the layout for an AnnouncementPacket.
/// </summary>
[MessagePackObject]
public partial class AnnouncementPacket : IntersectPacket
{
    //Parameterless Constructor for MessagePack
    public AnnouncementPacket()
    {
    }

    /// <summary>
    /// The announcement message to send.
    /// </summary>
    [Key(0)]
    public string Message { get; set; }

    /// <summary>
    /// The time (in milliseconds) for the announcement to display.
    /// </summary>
    [Key(1)]
    public long Duration { get; set; }

    /// <summary>
    /// Banner style: 0 = the usual big banner in the middle of the screen, 1 = small alert banner near the top.
    /// </summary>
    [Key(2)]
    public byte Style { get; set; }

    /// <summary>
    /// Creates a new instance of the <see cref="AnnouncementPacket"/> class.
    /// </summary>
    /// <param name="message">The message to send to the client.</param>
    /// <param name="duration">The duration for this message to appear for on the client.</param>
    public AnnouncementPacket(string message, long duration, byte style = 0)
    {
        Message = message;
        Duration = duration;
        Style = style;
    }

}
