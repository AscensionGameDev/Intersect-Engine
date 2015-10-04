using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intersect_Client.Classes
{
    public class MapList
    {
        public List<FolderItem> Items = new List<FolderItem>();

        public byte[] Data()
        {
            ByteBuffer myBuffer = new ByteBuffer();
            myBuffer.WriteInteger(Items.Count);
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].GetType() == typeof(FolderMap))
                {
                    ((FolderMap)Items[i]).GetData(myBuffer);
                }
                else
                {
                    ((FolderDirectory)Items[i]).GetData(myBuffer);
                }
            }
            return myBuffer.ToArray();
        }

        public bool Load(ByteBuffer myBuffer)
        {
            Items.Clear();
            int count = myBuffer.ReadInteger();
            int type = -1;
            FolderMap tmpMap;
            FolderDirectory tmpDir;
            bool result = true;
            for (int i = 0; i < count; i++)
            {
                type = myBuffer.ReadInteger();
                if (type == 0)
                {
                    tmpDir = new FolderDirectory();
                    if (tmpDir.Load(myBuffer))
                    {
                        Items.Add(tmpDir);
                    }
                    else
                    {
                        result = false;
                    }
                }
                else if (type == 1)
                {
                    tmpMap = new FolderMap();
                    if (tmpMap.Load(myBuffer))
                    {
                        Items.Add(tmpMap);
                        Database.OrderedMaps.Add(tmpMap);
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            return result;
        }

        public void Add(FolderItem item)
        {
            Items.Add(item);
        }

        public int FindFirstMap()
        {
            int lowestMap = -1;
            if (Database.OrderedMaps.Count > 0)
            {
                lowestMap = Database.OrderedMaps[0].MapNum;
            }
            return lowestMap;
        }
    }
    public class FolderItem
    {
        public string Name = "";
        public int type = -1; //0 for directory, 1 for map

        public void GetData(ByteBuffer myBuffer)
        {
            myBuffer.WriteInteger(type);
            myBuffer.WriteString(Name);
        }

        public void Load(ByteBuffer myBuffer)
        {
            Name = myBuffer.ReadString();
        }

    }
    public class FolderMap : FolderItem
    {
        public int MapNum = -1;
        public FolderMap()
            : base()
        {
            base.Name = "New Map";
            base.type = 1;
        }

        public void GetData(ByteBuffer myBuffer)
        {
            base.GetData(myBuffer);
            myBuffer.WriteInteger(MapNum);
        }

        public bool Load(ByteBuffer myBuffer)
        {
            base.Load(myBuffer);
            MapNum = myBuffer.ReadInteger();
            Name = myBuffer.ReadString();
            if (Globals.GameMaps.ContainsKey(MapNum) && Globals.GameMaps[MapNum] != null)
            {
                Globals.GameMaps[MapNum].MyName = Name;
            }
            return true;
        }

    }
    public class FolderDirectory : FolderItem
    {
        public MapList Children = new MapList();
        public int FolderId = -1;
        public FolderDirectory()
            : base()
        {
            base.Name = "New Folder";
            base.type = 0;
        }

        public void GetData(ByteBuffer myBuffer)
        {
            base.GetData(myBuffer);
            myBuffer.WriteInteger(FolderId);
        }

        public bool Load(ByteBuffer myBuffer)
        {
            Children.Items.Clear();
            base.Load(myBuffer);
            FolderId = myBuffer.ReadInteger();
            return Children.Load(myBuffer);
        }
    }
}
