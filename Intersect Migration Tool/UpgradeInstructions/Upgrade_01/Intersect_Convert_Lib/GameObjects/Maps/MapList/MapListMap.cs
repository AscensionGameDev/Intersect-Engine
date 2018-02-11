using System;
using System.Collections.Generic;
using Intersect.Migration.UpgradeInstructions.Upgrade_10.Intersect_Convert_Lib;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_1.Intersect_Convert_Lib.GameObjects.Maps.MapList
{
    public class MapListMap : MapListItem, IComparable<MapListMap>
    {
        public int MapNum = -1;

        public MapListMap() : base()
        {
            Name = "New Map";
            Type = 1;
        }

        public int CompareTo(MapListMap obj)
        {
            return MapNum.CompareTo(obj.MapNum);
        }

        public void GetData(ByteBuffer myBuffer, Dictionary<int, MapBase> gameMaps)
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
    }
}