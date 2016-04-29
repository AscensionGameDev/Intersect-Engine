using System.Collections.Generic;

namespace Intersect_Library.GameObjects.Maps.MapList
{
    public class FolderDirectory : FolderItem
    {
        public MapList Children = new MapList();
        public int FolderId = -1;
        public FolderDirectory()
            : base()
        {
            base.Name = "New Folder";
            base.type = 0;
        }

        public void GetData(ByteBuffer myBuffer, Dictionary<int, MapStruct> gameMaps )
        {
            base.GetData(myBuffer);
            myBuffer.WriteInteger(FolderId);
            myBuffer.WriteBytes(Children.Data(gameMaps));
        }

        public bool Load(ByteBuffer myBuffer, Dictionary<int, MapStruct> gameMaps, bool isServer = true )
        {
            Children.Items.Clear();
            base.Load(myBuffer);
            FolderId = myBuffer.ReadInteger();
            return Children.Load(myBuffer, gameMaps, isServer);
        }
    }
}
