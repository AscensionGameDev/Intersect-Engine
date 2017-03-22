

using Intersect;
using Intersect.GameObjects;
using Intersect_Server.Classes.General;


namespace Intersect_Server.Classes.Items
{
    public class ItemInstance
    {
        public int ItemNum = -1;
        public int ItemVal = 0;
        public int[] StatBoost = new int[(int)Stats.StatCount];
        public int BagId = -1;
        public BagInstance BagInstance = null;

        public ItemInstance()
        {
            
        }

        public ItemInstance(int itemNum, int itemVal, int bagId)
        {
            ItemNum = itemNum;
            ItemVal = itemVal;
            BagId = bagId;
            if (ItemBase.GetItem(ItemNum) != null)
            {
                if (ItemBase.GetItem(ItemNum).ItemType == (int) ItemTypes.Equipment)
                {
                    itemVal = 1;
                    for (int i = 0; i < (int) Stats.StatCount; i++)
                    {
                      StatBoost[i] =
                            Globals.Rand.Next(-1* ItemBase.GetItem(ItemNum).StatGrowth,
                                ItemBase.GetItem(ItemNum).StatGrowth + 1);
                    }
                }
            }
        }

        public byte[] Data()
        {
            var bf = new ByteBuffer();
            bf.WriteInteger(ItemNum);
            bf.WriteInteger(ItemVal);
            for (int i = 0; i < (int)Stats.StatCount; i++)
            {
                bf.WriteInteger(StatBoost[i]);
            }
            return bf.ToArray();
        }

        public ItemInstance Clone()
        {
            ItemInstance newItem = new ItemInstance(ItemNum,ItemVal, BagId);
            for (int i = 0; i < (int)Stats.StatCount; i++){
                newItem.StatBoost[i] = StatBoost[i];
            }
            if (BagInstance != null) newItem.BagInstance = BagInstance.Clone();
            return newItem;
        }
    }
}
