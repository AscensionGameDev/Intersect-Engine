using Intersect;

namespace Intersect_Migration_Tool.UpgradeInstructions.Upgrade_6.Intersect_Convert_Lib.GameObjects
{
    public class LightBase
    {
        public Color Color = Color.White;
        public float Expand = 0f;
        public byte Intensity = 255;
        public int OffsetX;
        public int OffsetY;
        public int Size = 0;
        public int TileX;
        public int TileY;

        public LightBase()
        {
            TileX = -1;
            TileY = -1;
        }

        public LightBase(int x, int y)
        {
            TileX = x;
            TileY = y;
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
            Color clr)
        {
            TileX = tileX;
            TileY = tileY;
            OffsetX = offsetX;
            OffsetY = offsetY;
            Intensity = intensity;
            Size = size;
            Expand = expand;
            Color = clr;
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