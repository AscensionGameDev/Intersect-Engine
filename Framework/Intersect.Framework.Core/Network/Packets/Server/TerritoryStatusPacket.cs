using MessagePack;

namespace Intersect.Network.Packets.Server;

/// <summary>Live state of one territory: who holds it and who (if anyone) is currently capturing it.</summary>
[MessagePackObject]
public partial class TerritoryStatusPacket : IntersectPacket
{
    public TerritoryStatusPacket()
    {
    }

    public TerritoryStatusPacket(
        Guid territoryId,
        Guid controllingFactionId,
        Guid capturingFactionId,
        int captureProgressSeconds,
        bool contested
    )
    {
        TerritoryId = territoryId;
        ControllingFactionId = controllingFactionId;
        CapturingFactionId = capturingFactionId;
        CaptureProgressSeconds = captureProgressSeconds;
        Contested = contested;
    }

    [Key(0)]
    public Guid TerritoryId { get; set; }

    /// <summary><see cref="Guid.Empty"/> = uncontrolled/neutral.</summary>
    [Key(1)]
    public Guid ControllingFactionId { get; set; }

    /// <summary>Faction currently making progress toward taking the territory, <see cref="Guid.Empty"/> if none.</summary>
    [Key(2)]
    public Guid CapturingFactionId { get; set; }

    [Key(3)]
    public int CaptureProgressSeconds { get; set; }

    /// <summary>True when two or more factions are present at once, freezing progress.</summary>
    [Key(4)]
    public bool Contested { get; set; }
}
