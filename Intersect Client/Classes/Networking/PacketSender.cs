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
using System.Security.Cryptography;
using System.Text;
using Intersect_Client.Classes.Entities;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Misc;


namespace Intersect_Client.Classes.Networking
{
    public static class PacketSender
    {
        public static void SendPing()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.Ping);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendLogin(string username, string password)
        {
            var bf = new ByteBuffer();
            var sha = new SHA256Managed();
            bf.WriteLong((int)Enums.ClientPackets.Login);
            bf.WriteString(username.ToLower().Trim());
            bf.WriteString(password.Trim());
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendNeedMap(int mapNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.NeedMap);
            bf.WriteLong(mapNum);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendMove()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.SendMove);
            bf.WriteInteger(Globals.Entities[Globals.MyIndex].CurrentMap);
            bf.WriteInteger(Globals.Entities[Globals.MyIndex].CurrentX);
            bf.WriteInteger(Globals.Entities[Globals.MyIndex].CurrentY);
            bf.WriteInteger(Globals.Entities[Globals.MyIndex].Dir);
            GameNetwork.SendPacket(bf.ToArray());
       }

        public static void SendChatMsg(string msg)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.LocalMessage);
            bf.WriteString(msg);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendEnterMap()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.EnterMap);
            bf.WriteLong(Globals.CurrentMap);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendAttack(int index)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.TryAttack);
            bf.WriteLong(index);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendDir(int dir)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.SendDir);
            bf.WriteLong(dir);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendEnterGame()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.EnterGame);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendActivateEvent(int mapNum, int eventIndex)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.ActivateEvent);
            bf.WriteInteger(mapNum);
            bf.WriteInteger(eventIndex);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendEventResponse(int response, EventDialog ed)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.EventResponse);
            bf.WriteInteger(ed.EventMap);
            bf.WriteInteger(ed.EventIndex);
            bf.WriteInteger(response);
            Globals.EventDialogs.Remove(ed);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendCreateAccount(string username, string password, string email)
        {
            var bf = new ByteBuffer();
            var sha = new SHA256Managed();
            bf.WriteLong((int)Enums.ClientPackets.CreateAccount);
            bf.WriteString(username.ToLower().Trim());
            bf.WriteString(BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(password.Trim()))).Replace("-", ""));
            bf.WriteString(email.ToLower().Trim());
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendCreateCharacter(string Name, int Class, int Sprite)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.CreateCharacter);
            bf.WriteString(Name.ToLower().Trim());
            bf.WriteInteger(Class);
            bf.WriteInteger(Sprite);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendPickupItem(int index)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.PickupItem);
            bf.WriteInteger(index);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendSwapItems(int item1, int item2)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.SwapItems);
            bf.WriteInteger(item1);
            bf.WriteInteger(item2);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendDropItem(int slot, int amount)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.DropItems);
            bf.WriteInteger(slot);
            bf.WriteInteger(amount);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendUseItem(int slot)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.UseItem);
            bf.WriteInteger(slot);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendSwapSpells(int spell1, int spell2)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.SwapSpells);
            bf.WriteInteger(spell1);
            bf.WriteInteger(spell2);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendForgetSpell(int slot)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.ForgetSpell);
            bf.WriteInteger(slot);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendUseSpell(int slot)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.UseSpell);
            bf.WriteInteger(slot);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendUnequipItem(int slot)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.UnequipItem);
            bf.WriteInteger(slot);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendUpgradeStat(int stat)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.UpgradeStat);
            bf.WriteInteger(stat);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendHotbarChange(int slot)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.HotbarChange);
            bf.WriteInteger(slot);
            bf.WriteInteger(Globals.Me.Hotbar[slot].Type);
            bf.WriteInteger(Globals.Me.Hotbar[slot].Slot);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendOpenAdminWindow()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.OpenAdminWindow);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendAdminAction(int action, int val)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.AdminAction);
            bf.WriteInteger(action);
            bf.WriteInteger(val);
            GameNetwork.SendPacket(bf.ToArray());
        }

    }
}
