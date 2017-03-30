using Intersect;
using Intersect.Server.Classes.Items;

namespace Intersect.Server.Classes.Maps
{
    public class MapItemInstance : ItemInstance
    {
        public int AttributeSpawnX = -1;
        public int AttributeSpawnY = -1;
        public long DespawnTime;
        public int X = 0;
        public int Y = 0;

        public MapItemInstance(int itemNum, int itemVal, int bagId) : base(itemNum, itemVal, bagId)
        {
        }

        public byte[] Data()
        {
            var bf = new ByteBuffer();
            bf.WriteInteger(X);
            bf.WriteInteger(Y);
            bf.WriteBytes(base.Data());
            return bf.ToArray();
        }
    }
}