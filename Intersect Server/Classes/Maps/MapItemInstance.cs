using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect_Library;
using Intersect_Server.Classes.Items;

namespace Intersect_Server.Classes.Maps
{
    public class MapItemInstance : ItemInstance
    {
        public int X = 0;
        public int Y = 0;
        public int AttributeSpawnX = -1;
        public int AttributeSpawnY = -1;
        public long DespawnTime;

        public MapItemInstance(int itemNum, int itemVal) : base(itemNum, itemVal)
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

    public class MapItemRespawn
    {
        public int AttributeSpawnX = -1;
        public int AttributeSpawnY = -1;
        public long RespawnTime = -1;
    }
}
