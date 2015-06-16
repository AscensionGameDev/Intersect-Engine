using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intersect_Client.Classes
{
    public class ItemInstance
    {
        public int ItemNum = -1;
        public int ItemVal = 0;
        public int[] StatBoost = new int[(int)Enums.Stats.StatCount];

        public ItemInstance()
        {

        }

        public void Load(ByteBuffer bf)
        {
            ItemNum = bf.ReadInteger();
            ItemVal = bf.ReadInteger();
            for (int i = 0; i < (int)Enums.Stats.StatCount; i++)
            {
                StatBoost[i] = bf.ReadInteger();
            }
        }
        public ItemInstance Clone()
        {
            ItemInstance newItem = new ItemInstance();
            newItem.ItemNum = ItemNum;
            newItem.ItemVal = ItemVal;
            for (int i = 0; i < (int)Enums.Stats.StatCount; i++)
            {
                newItem.StatBoost[i] = StatBoost[i];
            }
            return newItem;
        }
    }

    public class MapItemInstance : ItemInstance
    {
        public int X = 0;
        public int Y = 0;

        public void Load(ByteBuffer bf)
        {
            X = bf.ReadInteger();
            Y = bf.ReadInteger();
            base.Load(bf);
        }
    }
}
