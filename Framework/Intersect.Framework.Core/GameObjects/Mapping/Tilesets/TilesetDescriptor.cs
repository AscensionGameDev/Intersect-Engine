using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Models;
using Newtonsoft.Json;
using Point = Intersect.Point;

namespace Intersect.Framework.Core.GameObjects.Mapping.Tilesets;

/// <summary>
/// The four quarters of a single tile cell, used to give a tile partial (sub-tile) passability
/// instead of only "fully walkable" or "fully blocked" - e.g. a rock sprite that only blocks the
/// corner it visually occupies. Combine with bitwise OR for tiles that block more than one quarter.
/// </summary>
[Flags]
public enum TileQuadrant : byte
{
    None = 0,

    TopLeft = 1,

    TopRight = 2,

    BottomLeft = 4,

    BottomRight = 8,

    All = TopLeft | TopRight | BottomLeft | BottomRight,
}

public partial class TilesetDescriptor : DatabaseObject<TilesetDescriptor>
{
    [JsonConstructor]
    public TilesetDescriptor(Guid id) : base(id)
    {
        Name = string.Empty;
    }

    //Ef Parameterless Constructor
    public TilesetDescriptor()
    {
        Name = string.Empty;
    }

    public new string Name
    {
        get => base.Name;
        set => base.Name = value?.Trim().ToLower();
    }

    /// <summary>
    /// Per-tile (in tileset tile units, not pixels) blocked-quadrant masks. A tile marked here is
    /// treated as blocked wherever it's placed on any map, unless a map explicitly overrides that
    /// tile with a "Walkable" attribute. Set once per tileset (e.g. via the editor's tileset
    /// palette) instead of having to paint the Blocked attribute on every map that uses it.
    /// </summary>
    [NotMapped]
    [JsonIgnore]
    public Dictionary<Point, byte> BlockedTileMasks { get; set; } = new();

    [JsonProperty("BlockedTileMasks")]
    private List<int[]> BlockedTileMaskEntries
    {
        get => BlockedTileMasks.Select(pair => new[] { pair.Key.X, pair.Key.Y, (int)pair.Value }).ToList();
        set
        {
            BlockedTileMasks = new Dictionary<Point, byte>();
            if (value == null)
            {
                return;
            }

            foreach (var entry in value)
            {
                if (entry.Length >= 3)
                {
                    BlockedTileMasks[new Point(entry[0], entry[1])] = (byte)entry[2];
                }
            }
        }
    }

    [Column("BlockedTiles")]
    [JsonIgnore]
    public string BlockedTilesData
    {
        get => JsonConvert.SerializeObject(BlockedTileMaskEntries);
        set => BlockedTileMaskEntries = string.IsNullOrWhiteSpace(value)
            ? new List<int[]>()
            : JsonConvert.DeserializeObject<List<int[]>>(value) ?? new List<int[]>();
    }

    /// <summary>
    /// The blocked-quadrant mask for a tile (0 = fully walkable, see <see cref="TileQuadrant"/>).
    /// </summary>
    public byte GetTileBlockMask(int tileX, int tileY) =>
        BlockedTileMasks.TryGetValue(new Point(tileX, tileY), out var mask) ? mask : (byte)0;

    /// <summary>True if ANY part of this tile is blocked, regardless of approach direction.</summary>
    public bool IsTileBlocked(int tileX, int tileY) => GetTileBlockMask(tileX, tileY) != 0;

    public void SetTileQuadrantBlocked(int tileX, int tileY, TileQuadrant quadrant, bool blocked)
    {
        var point = new Point(tileX, tileY);
        var mask = GetTileBlockMask(tileX, tileY);
        mask = blocked ? (byte)(mask | (byte)quadrant) : (byte)(mask & ~(byte)quadrant);

        if (mask == 0)
        {
            BlockedTileMasks.Remove(point);
        }
        else
        {
            BlockedTileMasks[point] = mask;
        }
    }
}
