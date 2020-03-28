using System;

using Intersect.Collections;

namespace Intersect.GameObjects.Maps.MapList
{

    public class MapListFolder : MapListItem
    {

        public MapList Children = new MapList();

        public Guid FolderId = Guid.Empty;

        public MapListFolder() : base()
        {
            Name = "New Folder";
            Type = 0;
        }

        public void PostLoad(DatabaseObjectLookup gameMaps, bool isServer = true)
        {
            Children.PostLoad(gameMaps, isServer, false);
        }

    }

}
