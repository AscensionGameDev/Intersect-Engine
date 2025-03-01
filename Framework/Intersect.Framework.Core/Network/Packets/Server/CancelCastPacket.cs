﻿using MessagePack;

namespace Intersect.Network.Packets.Server;

[MessagePackObject]
public partial class CancelCastPacket : IntersectPacket
{
    //Parameterless Constructor for MessagePack
    public CancelCastPacket()
    {
    }

    public CancelCastPacket(Guid entityId)
    {
        EntityId = entityId;
    }

    [Key(0)]
    public Guid EntityId { get; set; }

}
