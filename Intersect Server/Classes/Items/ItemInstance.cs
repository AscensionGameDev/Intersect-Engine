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
