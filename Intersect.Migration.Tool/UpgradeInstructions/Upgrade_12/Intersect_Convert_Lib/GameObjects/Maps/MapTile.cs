using System;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.GameObjects.Maps
{
    public struct Tile
    {
        public Guid TilesetId;
        public int X;
        public int Y;
        public byte Autotile;
    }
}