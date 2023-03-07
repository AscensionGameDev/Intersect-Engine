using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Intersect.Config
{
    /// <summary>
    ///  Options for the game map.
    /// </summary>
    public partial class MapOptions
    {
        /// <summary>
        /// Indicates whether or not diagonal movement is enabled for entities within the map.
        /// </summary>
        public bool EnableDiagonalMovement
        {
            get { return mEnableDiagonalMovement; }
            set
            {
                mEnableDiagonalMovement = value;
                MovementDirections = mEnableDiagonalMovement ? 8 : 4;
            }
        }

        /// <summary>
        /// The style of the game's border.
        /// 0: Smart borders, 1: Non-seamless, 2: Black borders
        /// </summary>
        public int GameBorderStyle { get; set; }

        /// <summary>
        /// The time, in milliseconds, until item attributes respawn on the map.
        /// </summary>
        public int ItemAttributeRespawnTime { get; set; } = 15000;

        /// <summary>
        /// The options for the map's layers.
        /// </summary>
        public LayerOptions Layers { get; set; } = new LayerOptions();

        /// <summary>
        /// The height of the map in tiles.
        /// </summary>
        public int MapHeight { get; set; } = 26;

        /// <summary>
        /// The width of the map in tiles.
        /// </summary>
        public int MapWidth { get; set; } = 32;

        /// <summary>
        /// The number of movement directions available in the game for entities within the map.
        /// </summary>
        [JsonIgnore]
        public int MovementDirections { get; private set; }

        /// <summary>
        /// The height of each tile in pixels.
        /// </summary>
        public int TileHeight { get; set; } = 32;

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

        // A private field to hold the value of the EnableDiagonalMovement.
        private bool mEnableDiagonalMovement;

        [Obsolete("Renamed to MapHeight, this will be removed in 0.8-beta.", true)]
        public int Height
        {
            get => MapHeight;
            set => MapHeight = value;
        }

        [Obsolete("Renamed to MapWidth, this will be removed in 0.8-beta.", true)]
        public int Width
        {
            get => MapWidth;
            set => MapWidth = value;
        }

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
        }
    }
}
