/*
    Intersect Game Engine (Server)
    Copyright (C) 2015  JC Snider, Joe Bridges
    
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com 

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/

using Intersect_Library;
using Intersect_Library.GameObjects;
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
