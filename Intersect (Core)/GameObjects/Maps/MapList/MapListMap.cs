using Intersect.Collections;

namespace Intersect.GameObjects.Maps.MapList;

public partial class MapListMap : MapListItem, IComparable<MapListMap>
{
    public Guid MapId { get; set; }

    public long TimeCreated { get; set; }

    public MapListMap() : base()
    {
        Name = "New Map";
        Type = 1;
    }

    public int CompareTo(MapListMap obj) => Name.CompareTo(obj.Name);

    public void PostLoad(DatabaseObjectLookup gameMaps, bool isServer = true)
    {
        if (!gameMaps.TryGetValue(MapId, out var map))
        {
            return;
        }

        if (isServer)
        {
            Name = map.Name;
        }
        else
        {
            map.Name = Name;
        }
    }
}

