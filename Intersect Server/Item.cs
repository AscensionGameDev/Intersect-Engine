using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace IntersectServer
{
    public class Item
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

        public Item()
        {
            Speed = 10; // Set to 10 by default.
            StatsReq = new int[Constants.MAX_STATS];
            StatsGiven = new int[Constants.MAX_STATS];
        }

        public Item(ByteBuffer myBuffer)
        {
            LoadItem(myBuffer);
        }

        public void LoadItem(ByteBuffer myBuffer)
        {
            Name = myBuffer.ReadString();
            Type = myBuffer.ReadInteger();
            Pic = myBuffer.ReadString();
            Price = myBuffer.ReadInteger();
            Bound = myBuffer.ReadInteger();
            Animation = myBuffer.ReadInteger();
            ClassReq = myBuffer.ReadInteger();
            LevelReq = myBuffer.ReadInteger();

            for (int i = 0; i < Constants.MAX_STATS; i++)
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

        public byte[] ItemData()
        {
            ByteBuffer myBuffer = new ByteBuffer();
            myBuffer.WriteString(Name);
            myBuffer.WriteInteger(Type);
            myBuffer.WriteString(Pic);
            myBuffer.WriteInteger(Price);
            myBuffer.WriteInteger(Bound);
            myBuffer.WriteInteger(Animation);
            myBuffer.WriteInteger(ClassReq);
            myBuffer.WriteInteger(LevelReq);

            for (int i = 0; i < Constants.MAX_STATS; i++)
            {
                myBuffer.WriteInteger(StatsReq[i]);
                myBuffer.WriteInteger(StatsGiven[i]);
            }

            myBuffer.WriteInteger(StatGrowth);
            myBuffer.WriteInteger(Damage);
            myBuffer.WriteInteger(Speed);
            myBuffer.WriteString(Paperdoll);
            myBuffer.WriteInteger(Tool);
            myBuffer.WriteInteger(Data1);
            myBuffer.WriteInteger(Data2);
            myBuffer.WriteInteger(Data3);

            return myBuffer.ToArray();
        }

        public void LoadByte(byte[] data)
        {
            ByteBuffer bf;
            bf = new ByteBuffer();
            bf.WriteBytes(data);
            LoadItem(bf);
        }

        public void Save(int itemNum)
        {
            Stream stream = File.Create("Resources/Items/" + itemNum + ".item");
            stream.Write(ItemData(), 0, ItemData().Length);
            stream.Close();
        }
    }
}

