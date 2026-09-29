using Intersect.Models;
using Newtonsoft.Json;

namespace Intersect.Framework.Core.GameObjects.Territories;

/// <summary>
/// Describes a capturable territory: a point on a map that a faction takes control of by keeping it
/// uncontested (only their own faction present, in range) for <see cref="CaptureTimeSeconds"/>. Territories are
/// regular game objects: created from the editor, ticked server-side by <c>TerritoryManager</c>.
/// </summary>
public partial class TerritoryDescriptor : DatabaseObject<TerritoryDescriptor>, IFolderable
{
    [JsonConstructor]
    public TerritoryDescriptor(Guid id) : base(id)
    {
        Name = "New Territory";
    }

    //EF wants NO PARAMETERS!!!!!
    public TerritoryDescriptor()
    {
        Name = "New Territory";
    }

    public string Description { get; set; } = string.Empty;

    /// <summary>Map the capture point lives on.</summary>
    public Guid MapId { get; set; } = Guid.Empty;

    public int X { get; set; }

    public int Y { get; set; }

    /// <summary>How far (in tiles, from the capture point) a player counts as "present" for capture purposes.</summary>
    public int Radius { get; set; } = 10;

    /// <summary>Seconds of uncontested presence needed to flip the territory.</summary>
    public int CaptureTimeSeconds { get; set; } = 120;

    /// <summary>Experience given every <see cref="RewardTickSeconds"/> to each online member of the controlling faction present in the territory.</summary>
    public int RewardExperience { get; set; }

    public int RewardGold { get; set; }

    public int RewardTickSeconds { get; set; } = 60;

    /// <inheritdoc />
    public string Folder { get; set; } = string.Empty;
}
