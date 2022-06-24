using System;
using System.Runtime.Serialization;

namespace Intersect.Config
{

    public partial class MapOptions
    {

        //Maps
        public int GameBorderStyle; //0 For Smart Borders, 1 for Non-Seamless, 2 for black borders

        public int Height = 26;

        public int ItemAttributeRespawnTime = 15000;

        public int TileHeight = 32;

        public int TileWidth = 32;

        public int Width = 32;

        public bool ZDimensionVisible;

        public int TimeUntilMapCleanup = 30000; // It is recommended this is set to at least the time it takes to despawn a player's dropped items - or they can lose their items prematurely

        public LayerOptions Layers = new LayerOptions();

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            Validate();
        }

        public void Validate()
        {
            if (Width < 10 || Width > 64 || Height < 10 || Height > 64)
            {
                throw new Exception("Config Error: Map size out of bounds! (All values should be > 10 and < 64)");
            }
        }

    }

}
