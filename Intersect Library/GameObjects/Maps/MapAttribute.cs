using System;

namespace Intersect.GameObjects.Maps
{
    public class Attribute
    {
        public int Data1;
        public int Data2;
        public int Data3;
        public Guid Guid1;
        public Guid Guid2;
        public Guid Guid3;
        public string Data4 = "";
        public int Value;

        public byte[] Data()
        {
            var bf = new ByteBuffer();
            bf.WriteInteger(Value);
            bf.WriteInteger(Data1);
            bf.WriteInteger(Data2);
            bf.WriteInteger(Data3);
            bf.WriteGuid(Guid1);
            bf.WriteGuid(Guid2);
            bf.WriteGuid(Guid3);
            bf.WriteString(Data4);
            return bf.ToArray();
        }
    }
}