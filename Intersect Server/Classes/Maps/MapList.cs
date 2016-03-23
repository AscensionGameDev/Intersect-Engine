using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intersect_Server.Classes.Maps;

namespace Intersect_Server.Classes
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
                        if (Globals.GameMaps[tmpMap.MapNum] != null)
                        {
                            Items.Add(tmpMap);
                        }
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            return result;
        }

        public void AddMap(int mapNum)
        {
            if (!MapHelper.IsMapValid(mapNum)) return;
            var tmp = new FolderMap();
            tmp.Name = Globals.GameMaps[mapNum].MyName;
            tmp.MapNum = mapNum;
            Items.Add(tmp);
            PacketSender.SendMapListToEditors();
        }

        public void AddFolder(string folderName)
        {
            var tmp = new FolderDirectory();
            tmp.Name = folderName;
            tmp.FolderId = int.Parse("" + Globals.Rand.Next(1, 10) + Globals.Rand.Next(0, 10) + Globals.Rand.Next(0, 10) + Globals.Rand.Next(0, 10) + Globals.Rand.Next(0, 10) + Globals.Rand.Next(0, 10) + Globals.Rand.Next(0, 10) + Globals.Rand.Next(0, 10));
            while (Database.MapStructure.FindFolderParent(tmp.FolderId, null) != null)
            {
                tmp.FolderId = int.Parse("" + Globals.Rand.Next(1, 10) + Globals.Rand.Next(0, 10) + Globals.Rand.Next(0, 10) + Globals.Rand.Next(0, 10) + Globals.Rand.Next(0, 10) + Globals.Rand.Next(0, 10) + Globals.Rand.Next(0, 10) + Globals.Rand.Next(0, 10));
            }
            Items.Add(tmp);
            PacketSender.SendMapListToEditors();
        }

        public FolderDirectory FindDir(int folderId)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].type == 0)
                {
                    if (((FolderDirectory)Items[i]).FolderId == folderId)
                    {
                        return ((FolderDirectory)Items[i]);
                    }
                    if (((FolderDirectory)Items[i]).Children.FindDir(folderId) != null)
                    {
                        return ((FolderDirectory)Items[i]).Children.FindDir(folderId);
                    }
                }
            }
            return null;
        }

        public FolderMap FindMap(int mapNum)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].type == 0)
                {
                    if (((FolderDirectory)Items[i]).Children.FindMap(mapNum) != null)
                    {
                        return ((FolderDirectory)Items[i]).Children.FindMap(mapNum);
                    }
                }
                else
                {
                    if (((FolderMap)Items[i]).MapNum == mapNum)
                    {
                        return ((FolderMap)Items[i]);
                    }
                }
            }
            return null;
        }

        public FolderDirectory FindMapParent(int mapNum, FolderDirectory parent)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].GetType() == typeof(FolderDirectory))
                {
                    if (((FolderDirectory)Items[i]).Children.FindMapParent(mapNum, (FolderDirectory)Items[i]) != null) { return ((FolderDirectory)Items[i]).Children.FindMapParent(mapNum, (FolderDirectory)Items[i]); }
                }
                else
                {
                    if (((FolderMap)Items[i]).MapNum == mapNum)
                    {
                        return parent;
                    }
                }
            }
            return null;
        }

        public FolderDirectory FindFolderParent(int folderId, FolderDirectory parent)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].GetType() == typeof(FolderDirectory))
                {
                    if (((FolderDirectory)Items[i]).FolderId == folderId)
                    {
                        return parent;
                    }
                    if (((FolderDirectory)Items[i]).Children.FindFolderParent(folderId, (FolderDirectory)Items[i]) != null) { return ((FolderDirectory)Items[i]).Children.FindFolderParent(folderId, (FolderDirectory)Items[i]); }
                }
            }
            return null;
        }

        public void HandleMove(int srcType, int srcId, int destType, int destId)
        {
            FolderDirectory sourceParent = null;
            FolderDirectory destParent = null;
            MapList targetList = null;
            MapList sourceList = null;
            FolderItem source = null;
            FolderItem dest = null;
            if (destType == 0)
            {
                destParent = FindFolderParent(destId, null);
                if (destParent == null) { 
                    targetList = Database.MapStructure;
                }
                else
                {
                    targetList = destParent.Children;
                }
                dest = FindDir(destId);
            }
            else
            {
                destParent = FindMapParent(destId, null);
                if (destParent == null)
                {
                    targetList = Database.MapStructure;
                }
                else
                {
                    targetList = destParent.Children;
                }
                dest = FindMap(destId);
            }
            if (srcType == 0)
            {
                sourceParent = FindFolderParent(srcId, null);
                if (sourceParent == null)
                {
                    sourceList = Database.MapStructure;
                }
                else
                {
                    sourceList = sourceParent.Children;
                }
                source = FindDir(srcId);
            }
            else
            {
                sourceParent = FindMapParent(srcId, null);
                if (sourceParent == null)
                {
                    sourceList = Database.MapStructure;
                }
                else
                {
                    sourceList = sourceParent.Children;
                }
                source = FindMap(srcId);
            }
            if (targetList != null && dest != null && sourceList != null && source != null)
            {
                if (destType == 0)
                {
                    ((FolderDirectory)dest).Children.Items.Add(source);
                    sourceList.Items.Remove(source);
                }
                else
                {
                    sourceList.Items.Remove(source);
                    targetList.Items.Insert(targetList.Items.IndexOf(dest), source);
                }
            }
            else if (targetList != null && sourceList != null && source != null)
            {
                if (destType == -1)
                {
                    targetList.Items.Add(source);
                    sourceList.Items.Remove(source);
                }
            }

            //Save Map List
            PacketSender.SendMapListToEditors();
        }

        public void DeleteFolder(int folderid)
        {
            FolderDirectory parent = FindFolderParent(folderid, null);
            FolderDirectory self = FindDir(folderid);
            if (parent == null)
            {
                Database.MapStructure.Items.AddRange(self.Children.Items);
                Database.MapStructure.Items.Remove(self);
            }
            else
            {
                parent.Children.Items.AddRange(self.Children.Items);
                parent.Children.Items.Remove(self);
            }
        }

        public void DeleteMap(int mapNum)
        {
            FolderDirectory parent = FindMapParent(mapNum, null);
            FolderMap self = FindMap(mapNum);
            if (parent == null)
            {
                Database.MapStructure.Items.Remove(self);
            }
            else
            {
                parent.Children.Items.Remove(self);
            }
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
            myBuffer.WriteString(Globals.GameMaps[MapNum].MyName);
        }

        public bool Load(ByteBuffer myBuffer)
        {
            base.Load(myBuffer);
            MapNum = myBuffer.ReadInteger();
            myBuffer.ReadString();
            if (!MapHelper.IsMapValid(MapNum)) return false;
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
            myBuffer.WriteBytes(Children.Data());
        }

        public bool Load(ByteBuffer myBuffer)
        {
            base.Load(myBuffer);
            FolderId = myBuffer.ReadInteger();
            return Children.Load(myBuffer);
        }
    }
}
