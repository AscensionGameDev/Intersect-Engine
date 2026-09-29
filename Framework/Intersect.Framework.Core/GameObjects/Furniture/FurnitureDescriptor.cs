using Intersect.Enums;
using Intersect.Models;
using Newtonsoft.Json;

namespace Intersect.Framework.Core.GameObjects.Furniture;

/// <summary>
/// Describes a piece of placeable decoration (table, chair, plant, etc.) that guild officers can
/// place inside their guild's hall instance. Purely decorative in this version - no interaction.
/// </summary>
public partial class FurnitureDescriptor : DatabaseObject<FurnitureDescriptor>, IFolderable
{
    [JsonConstructor]
    public FurnitureDescriptor(Guid id) : base(id)
    {
        Name = "New Furniture";
    }

    //EF wants NO PARAMETERS!!!!!
    public FurnitureDescriptor()
    {
        Name = "New Furniture";
    }

    /// <summary>
    /// Entity sprite (from resources/entities) used to draw this piece of furniture. Ignored when
    /// <see cref="UseTilesetSprite"/> is true.
    /// </summary>
    public string Sprite { get; set; } = string.Empty;

    /// <summary>
    /// When true, this piece of furniture is drawn from a single tile of a tileset (<see cref="TilesetId"/>,
    /// <see cref="TileX"/>, <see cref="TileY"/>) instead of an entity spritesheet - handy for decorations
    /// that are already just a tileset graphic (a rug, a small prop) rather than a full character sheet.
    /// </summary>
    public bool UseTilesetSprite { get; set; }

    /// <summary>
    /// The tileset to draw the furniture's tile from. Only used when <see cref="UseTilesetSprite"/> is true.
    /// </summary>
    public Guid TilesetId { get; set; } = Guid.Empty;

    /// <summary>
    /// The X coordinate, in tiles, of the source tile within <see cref="TilesetId"/>.
    /// </summary>
    public int TileX { get; set; }

    /// <summary>
    /// The Y coordinate, in tiles, of the source tile within <see cref="TilesetId"/>.
    /// </summary>
    public int TileY { get; set; }

    /// <summary>
    /// When false, this piece of furniture blocks movement onto its tile (e.g. a table),
    /// when true players can walk over it (e.g. a rug).
    /// </summary>
    public bool Passable { get; set; } = true;

    /// <summary>
    /// The direction the sprite faces by default when placed, before the player rotates it.
    /// </summary>
    public Direction DefaultDirection { get; set; } = Direction.Down;

    /// <inheritdoc />
    public string Folder { get; set; } = string.Empty;
}
