namespace Intersect.Migration.UpgradeInstructions.Upgrade_6.Intersect_Convert_Lib.GameObjects.Maps.MapList
{
    public class MapListItem
    {
        public string Name = "";
        public int type = -1; //0 for directory, 1 for map

        public void GetData(ByteBuffer myBuffer)
        {
            myBuffer.WriteInteger(type);
            myBuffer.WriteString(Name);
        }

        public void Load(ByteBuffer myBuffer)
        {
            Name = myBuffer.ReadString();
        }
    }
}
