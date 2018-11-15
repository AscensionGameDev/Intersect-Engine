using System;
using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData.Characters;

namespace Intersect.Server.Maps
{
    public class MapItem : Item
    {
        public int AttributeSpawnX = -1;
        public int AttributeSpawnY = -1;
        public long DespawnTime;
        public int X = 0;
        public int Y = 0;

        public MapItem(Guid itemId, int itemVal) : base(itemId, itemVal,null, null)
        {
        }

        public MapItem(Guid itemId, int itemVal, Guid? bagId, Bag bag) : base(itemId, itemVal,bagId, bag)
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