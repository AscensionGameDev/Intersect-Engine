/*
    Intersect Game Engine (Editor)
    Copyright (C) 2015  JC Snider, Joe Bridges
    
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com 

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/

using Intersect_Library;
using Intersect_Library.GameObjects.Maps.MapList;

namespace Intersect_Editor.Classes
{
    public static class PacketSender
    {

        public static void SendPing()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.Ping);
            Network.SendPacket(bf.ToArray());

        }

        public static void SendLogin(string username, string password)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.EditorLogin);
            bf.WriteString(username);
            bf.WriteString(password);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendNeedMap(int mapNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.NeedMap);
            bf.WriteLong(mapNum);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendTilesets()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.SaveTilesetArray);
            bf.WriteLong(Globals.Tilesets.Length);
            foreach (var t in Globals.Tilesets)
            {
                bf.WriteString(t);
            }
            Network.SendPacket(bf.ToArray());
        }

        public static void SendMap(int mapnum)
        {
            var bf = new ByteBuffer();
            var mapData = Globals.GameMaps[mapnum].Save();
            bf.WriteLong((int)ClientPackets.SaveMap);
            bf.WriteLong(mapnum);
            bf.WriteLong(mapData.Length);
            bf.WriteBytes(mapData);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendCreateMap(int location, int currentMap, FolderItem parent)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.CreateMap);
            bf.WriteInteger(location);
            if (location > -1)
            {
                bf.WriteLong(currentMap);
            }
            else
            {
                if (parent == null)
                {
                    bf.WriteInteger(-1);
                    bf.WriteInteger(-1);
                }
                else
                {
                    if (parent.GetType() == typeof(FolderMap))
                    {
                        bf.WriteInteger(1);
                        bf.WriteInteger(((FolderMap)parent).MapNum);
                    }
                    else
                    {
                        bf.WriteInteger(0);
                        bf.WriteInteger(((FolderDirectory)parent).FolderId);
                    }
                }
            }
            Network.SendPacket(bf.ToArray());
        }

        public static void SendItemEditor()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.OpenItemEditor);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendItem(int itemNum, byte[] itemData)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.SaveItem);
            bf.WriteInteger(itemNum);
            bf.WriteBytes(itemData);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendNpcEditor()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.OpenNpcEditor);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendNpc(int npcNum, byte[] npcData)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.SaveNpc);
            bf.WriteInteger(npcNum);
            bf.WriteBytes(npcData);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendSpellEditor()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.OpenSpellEditor);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendSpell(int index, byte[] data)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.SaveSpell);
            bf.WriteInteger(index);
            bf.WriteBytes(data);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendAnimationEditor()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.OpenAnimationEditor);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendAnimation(int index, byte[] data)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.SaveAnimation);
            bf.WriteInteger(index);
            bf.WriteBytes(data);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendResourceEditor()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.OpenResourceEditor);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendResource(int ResourceNum, byte[] ResourceData)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.SaveResource);
            bf.WriteInteger(ResourceNum);
            bf.WriteBytes(ResourceData);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendClassEditor()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.OpenClassEditor);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendClass(int ClassNum, byte[] ClassData)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.SaveClass);
            bf.WriteInteger(ClassNum);
            bf.WriteBytes(ClassData);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendQuestEditor()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.OpenQuestEditor);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendQuest(int QuestNum, byte[] QuestData)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.SaveQuest);
            bf.WriteInteger(QuestNum);
            bf.WriteBytes(QuestData);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendProjectileEditor()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.OpenProjectileEditor);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendProjectile(int ProjectileNum, byte[] ProjectileData)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.SaveProjectile);
            bf.WriteInteger(ProjectileNum);
            bf.WriteBytes(ProjectileData);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendMapListMove(int srcType, int srcId, int destType, int destId)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.MapListUpdate);
            bf.WriteInteger((int)MapListUpdates.MoveItem);
            bf.WriteInteger(srcType);
            bf.WriteInteger(srcId);
            bf.WriteInteger(destType);
            bf.WriteInteger(destId);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendAddFolder(FolderItem parent)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.MapListUpdate);
            bf.WriteInteger((int)MapListUpdates.AddFolder);
            if (parent == null)
            {
                bf.WriteInteger(-1);
                bf.WriteInteger(-1);
            }
            else
            {
                if (parent.GetType() == typeof(FolderMap))
                {
                    bf.WriteInteger(1);
                    bf.WriteInteger(((FolderMap)parent).MapNum);
                }
                else
                {
                    bf.WriteInteger(0);
                    bf.WriteInteger(((FolderDirectory)parent).FolderId);
                }
            }
            Network.SendPacket(bf.ToArray());
        }

        public static void SendRename(FolderItem parent, string name)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.MapListUpdate);
            bf.WriteInteger((int)MapListUpdates.Rename);
            if (parent.GetType() == typeof(FolderMap))
            {
                bf.WriteInteger(1);
                bf.WriteInteger(((FolderMap)parent).MapNum);
            }
            else
            {
                bf.WriteInteger(0);
                bf.WriteInteger(((FolderDirectory)parent).FolderId);
            }
            bf.WriteString(name);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendDelete(FolderItem target)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.MapListUpdate);
            bf.WriteInteger((int)MapListUpdates.Delete);
            if (target.GetType() == typeof(FolderMap))
            {
                bf.WriteInteger(1);
                bf.WriteInteger(((FolderMap)target).MapNum);
            }
            else
            {
                bf.WriteInteger(0);
                bf.WriteInteger(((FolderDirectory)target).FolderId);
            }
            Network.SendPacket(bf.ToArray());
        }

        public static void SendNeedGrid(int mapNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.NeedGrid);
            bf.WriteLong(mapNum);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendUnlinkMap(int mapNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.UnlinkMap);
            bf.WriteLong(mapNum);
            bf.WriteLong(Globals.CurrentMap);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendLinkMap(int adjacentMap, int linkMap, int gridX, int gridY)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.LinkMap);
            bf.WriteLong(adjacentMap);
            bf.WriteLong(linkMap);
            bf.WriteLong(gridX);
            bf.WriteLong(gridY);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendOpenCommonEventEditor()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.OpenCommonEventEditor);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendCommonEvent(int index, byte[] data)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.SaveCommonEvent);
            bf.WriteInteger(index);
            bf.WriteBytes(data);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendOpenVariableSwitchEditor()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.OpenSwitchVariableEditor);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendOpenShopEditor()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.OpenShopEditor);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendShop(int index, byte[] data)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.SaveShop);
            bf.WriteInteger(index);
            bf.WriteBytes(data);
            Network.SendPacket(bf.ToArray());
        }
    }
}
