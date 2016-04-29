using System;
using System.Collections.Generic;

namespace Intersect_Library.GameObjects.Maps.MapList
{
    public class MapList
    {
        private static MapList _mapList = new MapList();
        private static List<FolderMap> _orderedMaps = new List<FolderMap>();
        private Random rand = new Random();
        public List<FolderItem> Items = new List<FolderItem>();

        public static MapList GetList()
        {
            return _mapList;
        }

        public static List<FolderMap> GetOrderedMaps()
        {
            return _orderedMaps;
        } 

        public byte[] Data(Dictionary<int, MapStruct> gameMaps)
        {
            ByteBuffer myBuffer = new ByteBuffer();
            myBuffer.WriteInteger(Items.Count);
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].GetType() == typeof(FolderMap))
                {

                    ((FolderMap)Items[i]).GetData(myBuffer,gameMaps);
                }
                else
                {
                    ((FolderDirectory)Items[i]).GetData(myBuffer);
                }
            }
            return myBuffer.ToArray();
        }

        public bool Load(ByteBuffer myBuffer, Dictionary<int, MapStruct> gameMaps, bool isServer = true)
        {
            _orderedMaps.Clear();
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
                    if (tmpDir.Load(myBuffer,gameMaps, isServer))
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
                    if (tmpMap.Load(myBuffer,gameMaps, isServer))
                    {
                        if (gameMaps.ContainsKey(tmpMap.MapNum) || !isServer)
                        {
                            Items.Add(tmpMap);
                            _orderedMaps.Add(tmpMap);
                        }
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            _orderedMaps.Sort();
            return result;
        }

        public void AddMap(int mapNum, Dictionary<int, MapStruct> gameMaps )
        {
            if (!gameMaps.ContainsKey(mapNum)) return;
            var tmp = new FolderMap();
            tmp.Name = gameMaps[mapNum].MyName;
            tmp.MapNum = mapNum;
            Items.Add(tmp);
            //PacketSender.SendMapListToEditors();
        }

        public void AddFolder(string folderName)
        {
            var tmp = new FolderDirectory();
            tmp.Name = folderName;
            tmp.FolderId = int.Parse("" + rand.Next(1, 10) + rand.Next(0, 10) + rand.Next(0, 10) + rand.Next(0, 10) + rand.Next(0, 10) + rand.Next(0, 10) + rand.Next(0, 10) + rand.Next(0, 10));
            while (_mapList.FindFolderParent(tmp.FolderId, null) != null)
            {
                tmp.FolderId = int.Parse("" + rand.Next(1, 10) + rand.Next(0, 10) + rand.Next(0, 10) + rand.Next(0, 10) + rand.Next(0, 10) + rand.Next(0, 10) + rand.Next(0, 10) + rand.Next(0, 10));
            }
            Items.Add(tmp);

            //PacketSender.SendMapListToEditors();
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
                if (destParent == null)
                {
                    targetList = _mapList;
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
                    targetList = _mapList;
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
                    sourceList = _mapList;
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
                    sourceList = _mapList;
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
            //PacketSender.SendMapListToEditors();
        }

        public void DeleteFolder(int folderid)
        {
            FolderDirectory parent = FindFolderParent(folderid, null);
            FolderDirectory self = FindDir(folderid);
            if (parent == null)
            {
                _mapList.Items.AddRange(self.Children.Items);
                _mapList.Items.Remove(self);
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
                _mapList.Items.Remove(self);
            }
            else
            {
                parent.Children.Items.Remove(self);
            }
        }

        public int FindFirstMap()
        {
            int lowestMap = -1;
            if (_orderedMaps.Count > 0)
            {
                lowestMap = _orderedMaps[0].MapNum;
            }
            return lowestMap;
        }

    }
}
