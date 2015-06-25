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

namespace Intersect_Client.Classes.Items
{
    public class ItemStruct
    {
        public string Name = "";
        public string Desc = "";
        public int Type;
        public string Pic = "";
        public int Price;
        public int Bound;
        public int Animation;
        public int ClassReq;
        public int LevelReq;
        public int[] StatsReq;
        public int[] StatsGiven;
        public int StatGrowth;
        public int Damage;
        public int Speed;
        public string Paperdoll = "";
        public int Tool;
        public int Data1;
        public int Data2;
        public int Data3;
        public int Data4;

        public ItemStruct()
        {
            Speed = 10; // Set to 10 by default.
            StatsReq = new int[Constants.MaxStats];
            StatsGiven = new int[Constants.MaxStats];
        }

        public void Load(byte[] data)
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteBytes(data);
            Name = myBuffer.ReadString();
            Desc = myBuffer.ReadString();
            Type = myBuffer.ReadInteger();
            Pic = myBuffer.ReadString();
            Price = myBuffer.ReadInteger();
            Bound = myBuffer.ReadInteger();
            Animation = myBuffer.ReadInteger();
            ClassReq = myBuffer.ReadInteger();
            LevelReq = myBuffer.ReadInteger();

            for (var i = 0; i < Constants.MaxStats; i++)
            {
                StatsReq[i] = myBuffer.ReadInteger();
                StatsGiven[i] = myBuffer.ReadInteger();
            }

            StatGrowth = myBuffer.ReadInteger();
            Damage = myBuffer.ReadInteger();
            Speed = myBuffer.ReadInteger();
            Paperdoll = myBuffer.ReadString();
            Tool = myBuffer.ReadInteger();
            Data1 = myBuffer.ReadInteger();
            Data2 = myBuffer.ReadInteger();
            Data3 = myBuffer.ReadInteger();
            Data4 = myBuffer.ReadInteger();
        }
    }
}
