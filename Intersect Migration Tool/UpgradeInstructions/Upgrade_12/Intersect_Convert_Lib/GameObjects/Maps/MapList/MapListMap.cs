using System;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Collections;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.GameObjects.Maps.MapList
{
    public class MapListMap : MapListItem, IComparable<MapListMap>
    {
        public Guid MapId;

        public MapListMap() : base()
        {
            Name = "New Map";
            Type = 1;
        }

        public int CompareTo(MapListMap obj)
        {
            return MapId.CompareTo(obj.MapId);
        }

        public void GetData(ByteBuffer myBuffer, DatabaseObjectLookup gameMaps)
        {
            base.GetData(myBuffer);
            myBuffer.WriteGuid(MapId);
            myBuffer.WriteString(gameMaps[MapId]?.Name ?? "Deleted");
        }

        public bool Load(ByteBuffer myBuffer, DatabaseObjectLookup gameMaps, bool isServer = true)
        {
            base.Load(myBuffer);
            MapId = myBuffer.ReadGuid();
            Name = myBuffer.ReadString();
            if (isServer)
            {
                if (!gameMaps.Keys.Contains(MapId)) return false;
            }
            else
            {
                if (gameMaps.Keys.Contains(MapId))
                {
                    gameMaps[MapId].Name = Name;
                }
            }
            return true;
        }
    }
}