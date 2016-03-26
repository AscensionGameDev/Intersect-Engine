/*
    The MIT License (MIT)

    Copyright (c) 2015 JC Snider, Joe Bridges
  
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect_Client.Classes.Misc;

namespace Intersect_Client.Classes.Game_Objects
{
    public class ShopStruct
    {
        //Core info
        public const string Version = "0.0.0.1";
        public string Name = "New Shop";
        public int DefaultCurrency = 0;

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
            DefaultCurrency = myBuffer.ReadInteger();
            SellingItems.Clear();
            BuyingItems.Clear();
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
    }

    public class ShopItem
    {
        public int ItemNum;
        public int CostItemNum;
        public int CostItemVal;
        public ShopItem(ByteBuffer myBuffer)
        {
            ItemNum = myBuffer.ReadInteger();
            CostItemNum = myBuffer.ReadInteger();
            CostItemVal = myBuffer.ReadInteger();
        }
        public byte[] Data()
        {
            ByteBuffer myBuffer = new ByteBuffer();
            myBuffer.WriteInteger(ItemNum);
            myBuffer.WriteInteger(CostItemNum);
            myBuffer.WriteInteger(CostItemVal);
            return myBuffer.ToArray();
        }
    }
}
