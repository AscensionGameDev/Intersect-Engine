namespace Intersect_Client.Classes
{
    public class ItemStruct
    {
        public string Name;
        public int Type;
        public string Pic;
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
        public string Paperdoll;
        public int Tool;
        public int Data1;
        public int Data2;
        public int Data3;

        public ItemStruct()
        {
            Speed = 10; // Set to 10 by default.
        }

        public ItemStruct(ByteBuffer myBuffer)
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

            for (var i = 1; i <= Constants.MaxStats; i++)
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
            var myBuffer = new ByteBuffer();
            myBuffer.WriteString(Name);
            myBuffer.WriteInteger(Type);
            myBuffer.WriteString(Pic);
            myBuffer.WriteInteger(Price);
            myBuffer.WriteInteger(Bound);
            myBuffer.WriteInteger(Animation);
            myBuffer.WriteInteger(ClassReq);
            myBuffer.WriteInteger(LevelReq);

            for (var i = 1; i <= Constants.MaxStats; i++)
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
    }
}

