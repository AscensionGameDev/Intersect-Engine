namespace Intersect.Migration.UpgradeInstructions.Upgrade_1.Intersect_Convert_Lib.GameObjects.Maps.MapList
{
    public class MapListItem
    {
        public string Name = "";
        public int Type = -1; //0 for directory, 1 for map

        public void GetData(Upgrade_10.Intersect_Convert_Lib.ByteBuffer myBuffer)
        {
            myBuffer.WriteInteger(Type);
            myBuffer.WriteString(Name);
        }

        public void Load(Upgrade_10.Intersect_Convert_Lib.ByteBuffer myBuffer)
        {
            Name = myBuffer.ReadString();
        }
    }
}