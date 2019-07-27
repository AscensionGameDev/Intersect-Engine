using System;
using Intersect.Collections;

namespace Intersect.GameObjects.Maps.MapList
{
    public class MapListMap : MapListItem, IComparable<MapListMap>
    {
        public Guid MapId;
        public long TimeCreated;

        public MapListMap() : base()
        {
            Name = "New Map";
            Type = 1;
        }

        public void PostLoad(DatabaseObjectLookup gameMaps, bool isServer = true)
        {
            if (!isServer)
            { 
                if (gameMaps.Keys.Contains(MapId)) { 
                    gameMaps[MapId].Name = Name;
                    }
            }
            else
            {
                if (gameMaps.Keys.Contains(MapId))
                    Name = gameMaps[MapId].Name;
            }
        }

        public int CompareTo(MapListMap obj)
        {
            return TimeCreated.CompareTo(obj.TimeCreated);
        }
    }
}