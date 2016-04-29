using System;
using System.Collections.Generic;

namespace Intersect_Library.GameObjects.Maps.MapList
{
    public class FolderMap : FolderItem, IComparable<FolderMap>
    {
        public int MapNum = -1;
        public FolderMap()
            : base()
        {
            base.Name = "New Map";
            base.type = 1;
        }

        public void GetData(ByteBuffer myBuffer, Dictionary<int,MapStruct> gameMaps )
        {
            base.GetData(myBuffer);
            myBuffer.WriteInteger(MapNum);
            myBuffer.WriteString(gameMaps[MapNum].MyName);
        }

        public bool Load(ByteBuffer myBuffer, Dictionary<int, MapStruct> gameMaps, bool isServer = true)
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

        public int CompareTo(FolderMap obj)
        {
            return MapNum.CompareTo(obj.MapNum);
        }
    }
}
