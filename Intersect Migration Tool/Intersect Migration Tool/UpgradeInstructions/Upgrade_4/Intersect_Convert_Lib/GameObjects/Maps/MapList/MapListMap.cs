using System;
using System.Collections.Generic;

namespace Intersect_Migration_Tool.UpgradeInstructions.Upgrade_4.Intersect_Convert_Lib.GameObjects.Maps.MapList
{
    public class MapListMap : MapListItem, IComparable<MapListMap>
    {
        public int MapNum = -1;
        public MapListMap(): base()
        {
            Name = "New Map";
            type = 1;
        }

        public void GetData(ByteBuffer myBuffer, Dictionary<int,MapBase> gameMaps )
        {
            base.GetData(myBuffer);
            myBuffer.WriteInteger(MapNum);
            myBuffer.WriteString(gameMaps[MapNum].MyName);
        }

        public bool Load(ByteBuffer myBuffer, Dictionary<int, MapBase> gameMaps, bool isServer = true)
        {
            base.Load(myBuffer);
            MapNum = myBuffer.ReadInteger();
            Name = myBuffer.ReadString();
            if (isServer)
            {
                if (!gameMaps.ContainsKey(MapNum)) return false;
            }
            else
            {
                if (gameMaps.ContainsKey(MapNum))
                {
                    gameMaps[MapNum].MyName = Name;
                }
            }
            return true;
        }

        public int CompareTo(MapListMap obj)
        {
            return MapNum.CompareTo(obj.MapNum);
        }
    }
}
