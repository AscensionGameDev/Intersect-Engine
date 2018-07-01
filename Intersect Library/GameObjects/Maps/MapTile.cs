using System;

namespace Intersect.GameObjects.Maps
{
    public struct Tile
    {
        public Guid TilesetId;
        public int X;
        public int Y;
        public byte Autotile;
    }
}