using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Intersect.Config
{
    public class MapOptions
    {
        //Maps
        public int GameBorderStyle; //0 For Smart Borders, 1 for Non-Seamless, 2 for black borders
        public int ItemSpawnTime = 15000;
        public int ItemDespawnTime = 15000;
        public bool ZDimensionVisible;
        public int MapWidth = 32;
        public int MapHeight = 26;
        public int TileWidth = 32;
        public int TileHeight = 32;


        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            Validate();
        }

        public void Validate()
        {
            if (MapWidth < 10 || MapWidth > 64 || MapHeight < 10 || Options.MapHeight > 64)
            {
                throw new Exception("Config Error: Map size out of bounds! (All values should be > 10 and < 64)");
            }
        }
    }
}
