using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intersect_Client.Classes.Items
{
    public class ItemStruct
    {
        public string Name = "";
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
        }
    }
}
