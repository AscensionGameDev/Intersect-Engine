﻿using MessagePack;

namespace Intersect.Network.Packets.Client;

[MessagePackObject]
public partial class PartyKickPacket : IntersectPacket
{
    //Parameterless Constructor for MessagePack
    public PartyKickPacket()
    {
    }

    public PartyKickPacket(Guid targetId)
    {
        TargetId = targetId;
    }

    [Key(0)]
    public Guid TargetId { get; set; }

}
