﻿using Intersect.Enums;
using MessagePack;

namespace Intersect.Network.Packets.Server;

[MessagePackObject]
public partial class EntityStatsPacket : IntersectPacket
{
    //Parameterless Constructor for MessagePack
    public EntityStatsPacket()
    {
    }

    public EntityStatsPacket(Guid id, EntityType type, Guid mapId, int[] stats)
    {
        Id = id;
        Type = type;
        MapId = mapId;
        Stats = stats;
    }

    [Key(0)]
    public Guid Id { get; set; }

    [Key(1)]
    public EntityType Type { get; set; }

    [Key(2)]
    public Guid MapId { get; set; }

    [Key(3)]
    public int[] Stats { get; set; }

}
