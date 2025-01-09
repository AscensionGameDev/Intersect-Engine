using Intersect.Collections;

namespace Intersect.GameObjects.Maps.MapList;

public partial class MapListFolder : MapListItem
{
    public MapList Children = new();

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
