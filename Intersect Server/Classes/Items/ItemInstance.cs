using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intersect_Server.Classes
{
    public class ItemInstance
    {
        public int ItemNum = -1;
        public int ItemVal = 0;
        public int[] StatBoost = new int[(int)Enums.Stats.StatCount];

        public ItemInstance()
        {

        }

        public byte[] Data()
        {
            var bf = new ByteBuffer();
            bf.WriteInteger(ItemNum);
            bf.WriteInteger(ItemVal);
            for (int i = 0; i < (int)Enums.Stats.StatCount; i++)
            {
                bf.WriteInteger(StatBoost[i]);
            }
            return bf.ToArray();
        }

        public ItemInstance Clone()
        {
            ItemInstance newItem = new ItemInstance();
            newItem.ItemNum = ItemNum;
            newItem.ItemVal = ItemVal;
            for (int i = 0; i < (int)Enums.Stats.StatCount; i++){
                newItem.StatBoost[i] = StatBoost[i];
            }
            return newItem;
        }
    }

    public class MapItemInstance : ItemInstance
    {
        public int X = 0;
        public int Y = 0;
        public int AttributeSpawnX = -1;
        public int AttributeSpawnY = -1;
        public long DespawnTime;

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
        public long RespawnTime;
    }
}
