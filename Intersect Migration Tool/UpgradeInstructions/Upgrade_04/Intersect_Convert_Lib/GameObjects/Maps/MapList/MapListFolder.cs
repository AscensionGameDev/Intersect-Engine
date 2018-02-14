using System.Collections.Generic;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_4.Intersect_Convert_Lib.GameObjects.Maps.MapList
{
    public class MapListFolder : MapListItem
    {
        public MapList Children = new MapList();
        public int FolderId = -1;

        public MapListFolder()
            : base()
        {
            Name = "New Folder";
            Type = 0;
        }

        public void GetData(ByteBuffer myBuffer, Dictionary<int, MapBase> gameMaps)
        {
            base.GetData(myBuffer);
            myBuffer.WriteInteger(FolderId);
            myBuffer.WriteBytes(Children.Data(gameMaps));
        }

        public bool Load(ByteBuffer myBuffer, Dictionary<int, MapBase> gameMaps, bool isServer = true)
        {
            Children.Items.Clear();
            base.Load(myBuffer);
            FolderId = myBuffer.ReadInteger();
            return Children.Load(myBuffer, gameMaps, isServer, false);
        }
    }
}