/*
    Intersect Game Engine (Server)
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
using Intersect_Server.Classes.Entities;
using Intersect_Server.Classes.General;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Intersect_Server.Classes.Networking
{
    public class Client
    {

        //Game Incorperation Variables
        public string MyAccount = "";
        public string MyEmail = "";
        public string MyPassword = "";
        public string MySalt = "";
        public long MyId = -1;
        public int EntityIndex;
        public Player Entity;

        //Client Properties
        public bool IsEditor;
        public int Power = 0;

        //Adminastrative punnishments
        public bool Muted = false;
        public string MuteReason = "";

        //Network Variables
        private GameSocket mySocket;
        private Queue<byte[]> sendQueue = new Queue<byte[]>();
        private object sendLock = new object();

        //Processing Thead
        private Thread updateThread;

        public Client(int entIndex, GameSocket socket)
        {
            mySocket = socket;
            EntityIndex = entIndex;
            if (EntityIndex > -1) { Entity = (Player)Globals.Entities[EntityIndex]; }
            if (mySocket != null && mySocket.IsConnected()) { PacketSender.SendPing(this); }
            updateThread = new Thread(Update);
            updateThread.Start();
        }

        public void SendPacket(byte[] packet)
        {
            lock (sendLock)
            {
                var buff = new ByteBuffer();
                buff.WriteInteger(packet.Length);
                buff.WriteBytes(packet);
                sendQueue.Enqueue (buff.ToArray());
            }
        }

        public void Pinged()
        {
            if (mySocket != null && IsConnected())
            {
                mySocket.Pinged();
            }
        }

        public void Disconnect(string reason = "")
        {
            if (reason == "")
            {
                mySocket.Disconnect();
            }
            else
            {
                //send abort packet and then disconnect?
            }
        }

        public void Update()
        {
            while (mySocket != null && IsConnected())
            {
                mySocket.Update();
                if (Entity != null)
                {
                    Entity.Update();
                }
                lock (sendLock)
                {
                    while (sendQueue.Count > 0)
                    {
                        mySocket.SendData(sendQueue.Dequeue());
                    }
                }
            }
            lock (Globals.ClientLock)
            {
                Globals.Clients.Remove(this);
            }
        }

        public bool IsConnected()
        {
            if (mySocket != null)
            {
                return mySocket.IsConnected();
            }
            else
            {
                return false;
            }
        }

        public string GetIP()
        {
            if (IsConnected())
            {
                return mySocket.GetIP();
            }
            else
            {
                return "";
            }
        }
    }
}
