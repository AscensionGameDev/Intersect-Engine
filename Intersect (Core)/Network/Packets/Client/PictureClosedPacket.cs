﻿using MessagePack;

namespace Intersect.Network.Packets.Client;

[MessagePackObject]
public partial class PictureClosedPacket : IntersectPacket
{
    //Parameterless Constructor for MessagePack
    public PictureClosedPacket()
    {
    }

    public PictureClosedPacket(Guid eventId)
    {
        EventId = eventId;
    }

    [Key(0)]
    public Guid EventId { get; set; }

}
