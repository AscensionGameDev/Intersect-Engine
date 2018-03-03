using System;
using Intersect.Server.Classes.Database;
using Intersect.Server.Classes.Database.PlayerData.Characters;

namespace Intersect.Server.Classes.Maps
{
    public class MapItem : Item
    {
        public int AttributeSpawnX = -1;
        public int AttributeSpawnY = -1;
        public long DespawnTime;
        public int X = 0;
        public int Y = 0;

        public MapItem(int itemNum, int itemVal) : base(itemNum, itemVal,null, null)
        {
        }

        public MapItem(int itemNum, int itemVal, Guid? bagId, Bag bag) : base(itemNum, itemVal,bagId, bag)
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