using System;
using Intersect.Collections;

namespace Intersect.GameObjects.Maps.MapList
{
    public class MapListMap : MapListItem, IComparable<MapListMap>
    {
        public int MapNum = -1;

        public MapListMap() : base()
        {
            Name = "New Map";
            type = 1;
        }

        public int CompareTo(MapListMap obj)
        {
            return MapNum.CompareTo(obj.MapNum);
        }

        public void GetData(ByteBuffer myBuffer, IntObjectLookup<MapBase> gameMaps)
        {
            base.GetData(myBuffer);
            myBuffer.WriteInteger(MapNum);
            myBuffer.WriteString(gameMaps[MapNum].Name);
        }

        public bool Load(ByteBuffer myBuffer, IntObjectLookup<MapBase> gameMaps, bool isServer = true)
        {
            base.Load(myBuffer);
            MapNum = myBuffer.ReadInteger();
            Name = myBuffer.ReadString();
            if (isServer)
            {
                if (!gameMaps.Keys.Contains(MapNum)) return false;
            }
            else
            {
                if (gameMaps.Keys.Contains(MapNum))
                {
                    gameMaps[MapNum].Name = Name;
                }
            }
            return true;
        }
    }
}