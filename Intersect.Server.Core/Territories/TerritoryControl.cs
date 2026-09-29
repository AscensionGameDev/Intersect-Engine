using System.ComponentModel.DataAnnotations;

namespace Intersect.Server.Territories;

/// <summary>
/// Persisted ownership of one territory. Plain EF entity (not a game object), one row per
/// <see cref="TerritoryId"/>, updated whenever the territory changes hands. The live capture progress
/// (who is currently capturing and how far along) is kept in memory only, see <see cref="TerritoryManager"/>.
/// </summary>
public class TerritoryControl
{
    /// <summary>Same id as the owning <c>TerritoryDescriptor</c>.</summary>
    [Key]
    public Guid TerritoryId { get; set; }

    /// <summary>Faction currently holding the territory, <see cref="Guid.Empty"/> = uncontrolled/neutral.</summary>
    public Guid ControllingFactionId { get; set; } = Guid.Empty;

    /// <summary>UTC unix time (ms) the territory was last captured.</summary>
    public long CapturedAt { get; set; }
}
