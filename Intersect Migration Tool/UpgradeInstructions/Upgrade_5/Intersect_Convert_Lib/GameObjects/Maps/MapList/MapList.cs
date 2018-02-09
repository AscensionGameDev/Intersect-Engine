using System;
using System.Collections.Generic;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_5.Intersect_Convert_Lib.GameObjects.Maps.MapList
{
    public class MapList
    {
        private static MapList sMapList = new MapList();
        private static List<MapListMap> sOrderedMaps = new List<MapListMap>();
        public List<MapListItem> Items = new List<MapListItem>();
        private Random mRand = new Random();

        public static MapList GetList()
        {
            return sMapList;
        }

        public static List<MapListMap> GetOrderedMaps()
        {
            return sOrderedMaps;
        }

        public byte[] Data(Dictionary<int, MapBase> gameMaps)
        {
            ByteBuffer myBuffer = new ByteBuffer();
            myBuffer.WriteInteger(Items.Count);
            foreach (MapListItem item in Items)
            {
                if (item.GetType() == typeof(MapListMap))
                {
                    ((MapListMap) item).GetData(myBuffer, gameMaps);
                }
                else
                {
                    ((MapListFolder) item).GetData(myBuffer, gameMaps);
                }
            }
            return myBuffer.ToArray();
        }

        public bool Load(ByteBuffer myBuffer, Dictionary<int, MapBase> gameMaps, bool isServer = true,
            bool isTopLevel = false)
        {
            if (isTopLevel) sOrderedMaps.Clear();
            Items.Clear();
            int count = myBuffer.ReadInteger();
            int type = -1;
            MapListMap tmpMap;
            MapListFolder tmpDir;
            bool result = true;
            for (int i = 0; i < count; i++)
            {
                type = myBuffer.ReadInteger();
                if (type == 0)
                {
                    tmpDir = new MapListFolder();
                    if (tmpDir.Load(myBuffer, gameMaps, isServer))
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
                    tmpMap = new MapListMap();
                    if (tmpMap.Load(myBuffer, gameMaps, isServer))
                    {
                        if (gameMaps.ContainsKey(tmpMap.MapNum) || !isServer)
                        {
                            Items.Add(tmpMap);
                            sOrderedMaps.Add(tmpMap);
                        }
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            sOrderedMaps.Sort();
            return result;
        }

        public void AddMap(int mapNum, Dictionary<int, MapBase> gameMaps)
        {
            if (!gameMaps.ContainsKey(mapNum)) return;
            var tmp = new MapListMap()
            {
                Name = gameMaps[mapNum].MyName,
                MapNum = mapNum
            };
            Items.Add(tmp);
        }

        public void AddFolder(string folderName)
        {
            var tmp = new MapListFolder()
            {
                Name = folderName,
                FolderId =
                    int.Parse("" + mRand.Next(1, 10) + mRand.Next(0, 10) + mRand.Next(0, 10) + mRand.Next(0, 10) +
                              mRand.Next(0, 10) + mRand.Next(0, 10) + mRand.Next(0, 10) + mRand.Next(0, 10))
            };
            while (sMapList.FindFolderParent(tmp.FolderId, null) != null)
            {
                tmp.FolderId =
                    int.Parse("" + mRand.Next(1, 10) + mRand.Next(0, 10) + mRand.Next(0, 10) + mRand.Next(0, 10) +
                              mRand.Next(0, 10) + mRand.Next(0, 10) + mRand.Next(0, 10) + mRand.Next(0, 10));
            }
            Items.Add(tmp);
        }

        public MapListFolder FindDir(int folderId)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].Type == 0)
                {
                    if (((MapListFolder) Items[i]).FolderId == folderId)
                    {
                        return ((MapListFolder) Items[i]);
                    }
                    if (((MapListFolder) Items[i]).Children.FindDir(folderId) != null)
                    {
                        return ((MapListFolder) Items[i]).Children.FindDir(folderId);
                    }
                }
            }
            return null;
        }

        public MapListMap FindMap(int mapNum)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].Type == 0)
                {
                    if (((MapListFolder) Items[i]).Children.FindMap(mapNum) != null)
                    {
                        return ((MapListFolder) Items[i]).Children.FindMap(mapNum);
                    }
                }
                else
                {
                    if (((MapListMap) Items[i]).MapNum == mapNum)
                    {
                        return ((MapListMap) Items[i]);
                    }
                }
            }
            return null;
        }

        public MapListFolder FindMapParent(int mapNum, MapListFolder parent)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].GetType() == typeof(MapListFolder))
                {
                    if (((MapListFolder) Items[i]).Children.FindMapParent(mapNum, (MapListFolder) Items[i]) != null)
                    {
                        return ((MapListFolder) Items[i]).Children.FindMapParent(mapNum, (MapListFolder) Items[i]);
                    }
                }
                else
                {
                    if (((MapListMap) Items[i]).MapNum == mapNum)
                    {
                        return parent;
                    }
                }
            }
            return null;
        }

        public MapListFolder FindFolderParent(int folderId, MapListFolder parent)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].GetType() == typeof(MapListFolder))
                {
                    if (((MapListFolder) Items[i]).FolderId == folderId)
                    {
                        return parent;
                    }
                    if (((MapListFolder) Items[i]).Children.FindFolderParent(folderId, (MapListFolder) Items[i]) !=
                        null)
                    {
                        return ((MapListFolder) Items[i]).Children.FindFolderParent(folderId, (MapListFolder) Items[i]);
                    }
                }
            }
            return null;
        }

        public void HandleMove(int srcType, int srcId, int destType, int destId)
        {
            MapListFolder sourceParent = null;
            MapListFolder destParent = null;
            MapList targetList = null;
            MapList sourceList = null;
            MapListItem source = null;
            MapListItem dest = null;
            if (destType == 0)
            {
                destParent = FindFolderParent(destId, null);
                if (destParent == null)
                {
                    targetList = sMapList;
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
                    targetList = sMapList;
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
                    sourceList = sMapList;
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
                    sourceList = sMapList;
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
                    ((MapListFolder) dest).Children.Items.Add(source);
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
            MapListFolder parent = FindFolderParent(folderid, null);
            MapListFolder self = FindDir(folderid);
            if (parent == null)
            {
                sMapList.Items.AddRange(self.Children.Items);
                sMapList.Items.Remove(self);
            }
            else
            {
                parent.Children.Items.AddRange(self.Children.Items);
                parent.Children.Items.Remove(self);
            }
        }

        public void DeleteMap(int mapNum)
        {
            MapListFolder parent = FindMapParent(mapNum, null);
            MapListMap self = FindMap(mapNum);
            if (parent == null)
            {
                sMapList.Items.Remove(self);
            }
            else
            {
                parent.Children.Items.Remove(self);
            }
        }

        public int FindFirstMap()
        {
            int lowestMap = -1;
            if (sOrderedMaps.Count > 0)
            {
                lowestMap = sOrderedMaps[0].MapNum;
            }
            return lowestMap;
        }
    }
}