using System;
using Intersect.Collections;

namespace Intersect.GameObjects.Maps.MapList
{
    public class MapListFolder : MapListItem
    {
        public MapList Children = new MapList();
        public Guid FolderId = Guid.Empty;

        public MapListFolder()
            : base()
        {
            Name = "New Folder";
            Type = 0;
        }

        public void GetData(ByteBuffer myBuffer, DatabaseObjectLookup gameMaps)
        {
            base.GetData(myBuffer);
            myBuffer.WriteGuid(FolderId);
            myBuffer.WriteBytes(Children.Data(gameMaps));
        }

        public bool Load(ByteBuffer myBuffer, DatabaseObjectLookup gameMaps, bool isServer = true)
        {
            Children.Items.Clear();
            base.Load(myBuffer);
            FolderId = myBuffer.ReadGuid();
            return Children.Load(myBuffer, gameMaps, isServer, false);
        }
    }
}