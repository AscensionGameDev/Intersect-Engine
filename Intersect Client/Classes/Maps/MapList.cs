/*
    The MIT License (MIT)

    Copyright (c) 2015 JC Snider, Joe Bridges
  
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/
using System;
using System.Collections.Generic;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Misc;

namespace Intersect_Client.Classes.Maps
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
                        Globals.OrderedMaps.Add(tmpMap);
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
            if (Globals.OrderedMaps.Count > 0)
            {
                lowestMap = Globals.OrderedMaps[0].MapNum;
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
    public class FolderMap : FolderItem, IComparable<FolderMap>
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

        public int CompareTo(FolderMap obj)
        {
            return MapNum.CompareTo(obj.MapNum);
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
