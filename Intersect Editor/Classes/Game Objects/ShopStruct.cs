/*
    Intersect Game Engine (Editor)
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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect_Editor.Classes.Game_Objects
{
    public class ShopStruct
    {
        //Core info
        public const string Version = "0.0.0.1";
        public string Name = "";

        //Selling List
        public List<ShopItem> SellingItems = new List<ShopItem>();

        //Buying List
        public bool BuyingWhitelist = true;
        public List<ShopItem> BuyingItems = new List<ShopItem>();


        public ShopStruct()
        {
        }

        public void Load(byte[] packet, int index)
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteBytes(packet);
            string loadedVersion = myBuffer.ReadString();
            if (loadedVersion != Version)
                throw new Exception("Failed to load shop #" + index + ". Loaded Version: " + loadedVersion + " Expected Version: " + Version);
            Name = myBuffer.ReadString();
            var sellingCount = myBuffer.ReadInteger();
            for (int i = 0; i < sellingCount; i++)
            {
                SellingItems.Add(new ShopItem(myBuffer));
            }
            BuyingWhitelist = Convert.ToBoolean(myBuffer.ReadByte());
            var buyingCount = myBuffer.ReadInteger();
            for (int i = 0; i < buyingCount; i++)
            {
                BuyingItems.Add(new ShopItem(myBuffer));
            }
            myBuffer.Dispose();
        }

        public byte[] ShopData()
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteString(Version);
            myBuffer.WriteString(Name);
            myBuffer.WriteInteger(SellingItems.Count);
            for (int i = 0; i < SellingItems.Count; i++)
            {
                myBuffer.WriteBytes(SellingItems[i].Data());
            }
            myBuffer.WriteByte(Convert.ToByte(BuyingWhitelist));
            myBuffer.WriteInteger(BuyingItems.Count);
            for (int i = 0; i < BuyingItems.Count; i++)
            {
                myBuffer.WriteBytes(BuyingItems[i].Data());
            }

            return myBuffer.ToArray();
        }
    }

    public class ShopItem
    {
        public int ItemNum;
        public double ItemRate;
        public ShopItem(ByteBuffer myBuffer)
        {
            ItemNum = myBuffer.ReadInteger();
            ItemRate = myBuffer.ReadDouble();
        }
        public ShopItem(int itemNum, double rate)
        {
            ItemNum = itemNum;
            ItemRate = rate;
        }
        public byte[] Data()
        {
            ByteBuffer myBuffer = new ByteBuffer();
            myBuffer.WriteInteger(ItemNum);
            myBuffer.WriteDouble(ItemRate);
            return myBuffer.ToArray();
        }
    }
}
