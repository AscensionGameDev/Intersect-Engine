using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Server.Classes.General;

namespace Intersect.Server.Classes.Items
{
    public class ItemInstance
    {
        public int BagId = -1;
        public BagInstance BagInstance;
        public int ItemNum = -1;
        public int ItemVal;
        public int[] StatBoost = new int[(int) Stats.StatCount];

        public ItemInstance()
        {
        }

        public ItemInstance(int itemNum, int itemVal, int bagId)
        {
            ItemNum = itemNum;
            ItemVal = itemVal;
            BagId = bagId;
            if (ItemBase.Lookup.Get<ItemBase>(ItemNum) != null)
            {
                if (ItemBase.Lookup.Get<ItemBase>(ItemNum).ItemType == (int) ItemTypes.Equipment)
                {
                    itemVal = 1;
                    for (int i = 0; i < (int) Stats.StatCount; i++)
                    {
                        StatBoost[i] =
                            Globals.Rand.Next(-1 * ItemBase.Lookup.Get<ItemBase>(ItemNum).StatGrowth,
                                ItemBase.Lookup.Get<ItemBase>(ItemNum).StatGrowth + 1);
                    }
                }
            }
        }

        public byte[] Data()
        {
            var bf = new ByteBuffer();
            bf.WriteInteger(ItemNum);
            bf.WriteInteger(ItemVal);
            for (int i = 0; i < (int) Stats.StatCount; i++)
            {
                bf.WriteInteger(StatBoost[i]);
            }
            return bf.ToArray();
        }

        public ItemInstance Clone()
        {
            ItemInstance newItem = new ItemInstance(ItemNum, ItemVal, BagId);
            for (int i = 0; i < (int) Stats.StatCount; i++)
            {
                newItem.StatBoost[i] = StatBoost[i];
            }
            if (BagInstance != null) newItem.BagInstance = BagInstance.Clone();
            return newItem;
        }
    }
}