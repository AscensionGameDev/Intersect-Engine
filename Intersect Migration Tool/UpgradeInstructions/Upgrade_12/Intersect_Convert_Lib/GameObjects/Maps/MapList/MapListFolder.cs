using System;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Collections;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.GameObjects.Maps.MapList
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

        public void PostLoad(DatabaseObjectLookup gameMaps, bool isServer = true)
        {
            Children.PostLoad(gameMaps, isServer, false);
        }
    }
}