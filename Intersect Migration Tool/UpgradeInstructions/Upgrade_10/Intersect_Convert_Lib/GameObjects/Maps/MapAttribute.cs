namespace Intersect.Migration.UpgradeInstructions.Upgrade_10.Intersect_Convert_Lib.GameObjects.Maps
{
    public class Attribute
    {
        public int data1;
        public int data2;
        public int data3;
        public string data4 = "";
        public int value;

        public byte[] Data()
        {
            var bf = new ByteBuffer();
            bf.WriteInteger(value);
            bf.WriteInteger(data1);
            bf.WriteInteger(data2);
            bf.WriteInteger(data3);
            bf.WriteString(data4);
            return bf.ToArray();
        }
    }
}