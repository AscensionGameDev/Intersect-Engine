using System;
using System.Runtime.Serialization;

namespace Intersect.Config
{

    public class MapOptions
    {

        //Maps
        public int GameBorderStyle; //0 For Smart Borders, 1 for Non-Seamless, 2 for black borders

        public int Height = 26;

        // TODO: (panda) Move this to the LootOptions class, this is unrelated to map attribute items. (https://github.com/AscensionGameDev/Intersect-Engine/pull/181#issuecomment-619745057)
        public int ItemDespawnTime = 15000;

        // TODO: (panda) Rename to something like MapItemRespawnItem for clarity, would need an alias to the original however. (https://github.com/AscensionGameDev/Intersect-Engine/pull/181#issuecomment-619745057)
        public int ItemSpawnTime = 15000;

        public int TileHeight = 32;

        public int TileWidth = 32;

        public int Width = 32;

        public bool ZDimensionVisible;

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
