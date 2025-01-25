using System.Runtime.Serialization;
using Intersect.Framework.Annotations;
using Intersect.Framework.Core.GameObjects.Maps;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Intersect.Config;

/// <summary>
///  Options for the game map.
/// </summary>
[RequiresRestart]
public partial class MapOptions
{
    public const int DefaultMapWidth = 32;
    public const int DefaultMapHeight = 26;

    private bool _enableDiagonalMovement = true;

    /// <summary>
    /// If experience can be lost in arena type maps.
    /// </summary>
    public bool DisableExpLossInArenaMaps { get; set; } = false;

    /// <summary>
    /// If items can be dropped in arena type maps.
    /// </summary>
    public bool DisablePlayerDropsInArenaMaps { get; set; } = false;

    /// <summary>
    /// If two block attributes placed diagonally block diagonal movement.
    /// </summary>
    public bool EnableCrossingDiagonalBlocks { get; set; }

    /// <summary>
    /// Indicates whether diagonal movement is enabled for entities within the map.
    /// </summary>
    public bool EnableDiagonalMovement
    {
        get { return _enableDiagonalMovement; }
        set
        {
            _enableDiagonalMovement = value;
            MovementDirections = _enableDiagonalMovement ? 8 : 4;
        }
    }

    /// <summary>
    /// The style of the game's border.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public GameBorderStyle GameBorderStyle { get; set; }

    /// <summary>
    /// The time, in milliseconds, until item attributes respawn on the map.
    /// </summary>
    public int ItemAttributeRespawnTime { get; set; } = 15000;

    /// <summary>
    /// The options for the map's layers.
    /// </summary>
    public LayerOptions Layers { get; set; } = new();

    /// <summary>
    /// The height of the map in tiles.
    /// </summary>
    public int MapHeight { get; set; } = DefaultMapHeight;

    /// <summary>
    /// The width of map items.
    /// </summary>
    public uint MapItemHeight { get; set; }

    /// <summary>
    /// The height of map items.
    /// </summary>
    public uint MapItemWidth { get; set; }

    /// <summary>
    /// The width of the map in tiles.
    /// </summary>
    public int MapWidth { get; set; } = DefaultMapWidth;

    /// <summary>
    /// The number of movement directions available in the game for entities within the map.
    /// </summary>
    [JsonIgnore]
    public int MovementDirections { get; private set; }

    /// <summary>
    /// The height of each tile in pixels.
    /// </summary>
    public int TileHeight { get; set; } = 32;

    [JsonIgnore]
    public float TileScale => 32f / Math.Min(TileWidth, TileHeight);

    /// <summary>
    /// The width of each tile in pixels.
    /// </summary>
    public int TileWidth { get; set; } = 32;

    /// <summary>
    /// The time, in milliseconds, until the map is cleaned up.
    /// </summary>
    public int TimeUntilMapCleanup { get; set; } = 30000;

    /// <summary>
    /// Indicates whether the Z-dimension is visible in the map.
    /// </summary>
    public bool ZDimensionVisible { get; set; }

    [OnDeserialized]
    internal void OnDeserializedMethod(StreamingContext context)
    {
        Validate();
    }

    /// <summary>
    /// Validates the properties of the map options object.
    /// </summary>
    public void Validate()
    {
        if (MapWidth < 10 || MapWidth > 64 || MapHeight < 10 || MapHeight > 64)
        {
            throw new Exception("Config Error: Map size out of bounds! (All values should be > 10 and < 64)");
        }

        // Forcibly reset MovementDirections to the correct value
        EnableDiagonalMovement = _enableDiagonalMovement;

        MapItemWidth = MapItemWidth < 1 ? (uint)TileWidth : MapItemWidth;
        MapItemHeight = MapItemHeight < 1 ? (uint)TileHeight : MapItemHeight;
    }
}
