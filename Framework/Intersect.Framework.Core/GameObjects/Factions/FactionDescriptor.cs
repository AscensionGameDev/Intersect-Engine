using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Models;
using Newtonsoft.Json;

namespace Intersect.Framework.Core.GameObjects.Factions;

/// <summary>
/// Describes a faction a player picks at character creation (permanent for now). Factions are regular game
/// objects: created from the editor, and referenced by <see cref="Territories.TerritoryDescriptor"/> for
/// territory-capture PvP.
/// </summary>
public partial class FactionDescriptor : DatabaseObject<FactionDescriptor>, IFolderable
{
    [JsonConstructor]
    public FactionDescriptor(Guid id) : base(id)
    {
        Name = "New Faction";
    }

    //EF wants NO PARAMETERS!!!!!
    public FactionDescriptor()
    {
        Name = "New Faction";
    }

    /// <summary>Color used for this faction's name tag, territory ownership banners, etc.</summary>
    [NotMapped]
    public Color Color { get; set; } = Color.White;

    [Column("Color")]
    [JsonIgnore]
    public string JsonColor
    {
        get => JsonConvert.SerializeObject(Color);
        set => Color = !string.IsNullOrWhiteSpace(value) ? JsonConvert.DeserializeObject<Color>(value) : Color.White;
    }

    /// <summary>
    /// Where a member of this faction spawns/respawns, like Alliance vs Horde in WoW or Elyos vs Asmodian in Aion.
    /// Takes priority over the class's own spawn point. Empty map = fall back to the class spawn.
    /// </summary>
    public Guid StartMapId { get; set; } = Guid.Empty;

    public int StartX { get; set; }

    public int StartY { get; set; }

    public int StartDir { get; set; }

    /// <inheritdoc />
    public string Folder { get; set; } = string.Empty;
}
