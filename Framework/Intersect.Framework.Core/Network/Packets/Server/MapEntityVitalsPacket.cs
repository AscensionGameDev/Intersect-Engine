﻿using MessagePack;
using Intersect.Enums;

namespace Intersect.Network.Packets.Server;

[MessagePackObject]
public partial class MapEntityVitalsPacket : IntersectPacket
{
    [Key(0)]
    public Guid MapId { get; set; }

    [Key(1)]
    public EntityVitalData[] EntityUpdates { get; set; }

    public MapEntityVitalsPacket(Guid mapId, EntityVitalData[] entityUpdates)
    {
        MapId = mapId;
        EntityUpdates = entityUpdates;
    }

    // MessagePack compatibility
    public MapEntityVitalsPacket()
    {
    }
}

[MessagePackObject]
public partial class EntityVitalData
{
    [Key(0)]
    public Guid Id { get; set; }

    [Key(1)]
    public Enums.EntityType Type { get; set; }

    [Key(2)]
    public long[] Vitals { get; set; } = new long[Enum.GetValues<Vital>().Length];

    [Key(3)]
    public long[] MaxVitals { get; set; } = new long[Enum.GetValues<Vital>().Length];

    [Key(4)]
    public long CombatTimeRemaining { get; set; }
}
