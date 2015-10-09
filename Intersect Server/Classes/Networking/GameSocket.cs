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
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System;

namespace Intersect_Server.Classes
{

    public class SendDataEventArgs : EventArgs
    {
        public byte[] data { get; set; }
        public GameSocket socket { get; set; }
    }

    public class GameSocket
    {
        private TcpClient mySocket;
        
        private long connectTime;
        private NetworkStream myStream;
        private PacketHandler packetHandler = new PacketHandler();
        private ByteBuffer _myBuffer = new ByteBuffer();
        private Object _bufferLock = new Object();
        private Client myClient;
        private int EntityIndex;
        private long PingTime;
        private long ConnectionTimeout;
        private long Timeout = 10;
        public Object Sender;
        public delegate void SendDataEventHander(SendDataEventArgs e);
        public event SendDataEventHander OnSendData;
        private bool isConnected = true;

        public void SendData(byte[] data)
        {
            SendDataEventHander handler = OnSendData;
            if (handler != null)
            {
                SendDataEventArgs e = new SendDataEventArgs();
                var buff = new ByteBuffer();
                buff.WriteInteger(data.Length);
                buff.WriteBytes(data);
                e.data = buff.ToArray();
                e.socket = this;
                handler(e);
            }
        }

        public GameSocket()
        {
            CreateClient();
            connectTime = Environment.TickCount;
        }

        public void ReceiveData(byte[] data)
        {
            int packetLen;
            lock (_bufferLock)
            {
                _myBuffer.WriteBytes(data);
                while (_myBuffer.Length() >= 4)
                {
                    packetLen = _myBuffer.ReadInteger(false);
                    if (packetLen == 0)
                    {
                        break;
                    }
                    if (_myBuffer.Length() >= packetLen + 4)
                    {
                        _myBuffer.ReadInteger();
                        packetHandler.HandlePacket(myClient, _myBuffer.ReadBytes(packetLen));
                    }
                    else
                    {
                        break;
                    }
                }
                if (_myBuffer.Length() == 0)
                {
                    _myBuffer.Clear();
                }
            }
        }

        

        public void HandleDisconnect()
        {
            if (isConnected)
            {
                try
                {
                    Globals.GeneralLogs.Add("Client disconnected.");
                    if (EntityIndex > -1 && Globals.Entities[EntityIndex] != null)
                    {
                        Database.SavePlayer(myClient);
                        PacketSender.SendEntityLeave(EntityIndex, (int)Enums.EntityTypes.Player, Globals.Entities[EntityIndex].CurrentMap);
                        if (Globals.Entities[EntityIndex] == null) { return; }
                        PacketSender.SendGlobalMsg(Globals.Entities[EntityIndex].MyName + " has left the Intersect engine");
                        myClient.Entity = null;
                        myClient = null;
                        Globals.Entities[EntityIndex] = null;

                    }
                }
                catch (Exception) { }
            }
            isConnected = false;
        }

        public void Update(PacketHandler packetHandler)
        {
            var tempBuff = new byte[4096];

            do
            {
                try
                {
                    if (IsConnected())
                    {
                        if (ConnectionTimeout > -1 && ConnectionTimeout < Environment.TickCount)
                        {
                            //Disconnect
                            HandleDisconnect();
                            return;
                        }
                        else
                        {
                            if (PingTime < Environment.TickCount)
                            {
                                PacketSender.SendPing(myClient);
                            }
                        }
                    }
                    else
                    {
                        HandleDisconnect();
                        return;
                    }
                }
                catch
                {
                    HandleDisconnect();
                    return;
                }

            } while (true);
        }

        public bool IsConnected()
        {
            return isConnected;
        }

        private void CreateClient()
        {
            var tempIndex = FindOpenSocket();
            if (tempIndex > -1)
            {
                Globals.Clients[tempIndex] = new Client(tempIndex, Globals.FindOpenEntity(), this);
                myClient = Globals.Clients[tempIndex];
                EntityIndex = Globals.Clients[tempIndex].EntityIndex;
                Globals.Entities[EntityIndex] = new Player(EntityIndex, Globals.Clients[tempIndex]);
                Globals.GeneralLogs.Add("Client connected using client index of " + tempIndex);
            }
            else
            {
                Globals.GeneralLogs.Add("Rejecting client due to lack of space.");
            }
        }

        private static int FindOpenSocket()
        {
            for (var i = 0; i < Globals.Clients.Count; i++)
            {
                if (Globals.Clients[i] == null)
                {
                    //return i; --Need to fix before allowing the reuse of indices.
                }
                else if (i == Globals.Clients.Count - 1)
                {
                    Globals.Clients.Add(null);
                    Globals.ClientThread.Add(null);
                    return Globals.Clients.Count - 1;
                }
            }
            Globals.ClientThread.Add(null);
            Globals.Clients.Add(null);
            return Globals.Clients.Count - 1;
        }















    }


}
