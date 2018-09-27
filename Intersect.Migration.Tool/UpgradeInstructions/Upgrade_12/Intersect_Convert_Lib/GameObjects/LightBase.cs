namespace Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.GameObjects
{
    public class LightBase
    {
        public Color Color { get; set; }

        public float Expand { get; set; }

        public byte Intensity { get; set; }

        public int OffsetX { get; set; }

        public int OffsetY { get; set; }

        public int Size { get; set; }

        public int TileX { get; set; }

        public int TileY { get; set; }

        public LightBase() : this(-1, -1) {}

        public LightBase(int x, int y)
        {
            TileX = x;
            TileY = y;
            Color = Color.White;
            Intensity = 255;
        }

        public LightBase(LightBase copy)
        {
            TileX = copy.TileX;
            TileY = copy.TileY;
            OffsetX = copy.OffsetX;
            OffsetY = copy.OffsetY;
            Intensity = copy.Intensity;
            Size = copy.Size;
            Expand = copy.Expand;
            Color = Color.FromArgb(copy.Color.R, copy.Color.G, copy.Color.B);
        }

        public LightBase(ByteBuffer myBuffer)
        {
            OffsetX = myBuffer.ReadInteger();
            OffsetY = myBuffer.ReadInteger();
            TileX = myBuffer.ReadInteger();
            TileY = myBuffer.ReadInteger();
            Intensity = myBuffer.ReadByte();
            Size = myBuffer.ReadInteger();
            Expand = (float) myBuffer.ReadDouble();
            Color = Color.FromArgb(myBuffer.ReadByte(), myBuffer.ReadByte(), myBuffer.ReadByte());
        }

        public LightBase(int tileX, int tileY, int offsetX, int offsetY, byte intensity, int size, float expand,
            Color color)
        {
            TileX = tileX;
            TileY = tileY;
            OffsetX = offsetX;
            OffsetY = offsetY;
            Intensity = intensity;
            Size = size;
            Expand = expand;
            Color = color;
        }

        public byte[] LightData()
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteInteger(OffsetX);
            myBuffer.WriteInteger(OffsetY);
            myBuffer.WriteInteger(TileX);
            myBuffer.WriteInteger(TileY);
            myBuffer.WriteByte(Intensity);
            myBuffer.WriteInteger(Size);
            myBuffer.WriteDouble(Expand);
            myBuffer.WriteByte(Color.R);
            myBuffer.WriteByte(Color.G);
            myBuffer.WriteByte(Color.B);
            return myBuffer.ToArray();
        }
    }
}