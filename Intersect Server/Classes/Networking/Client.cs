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
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Intersect_Server.Classes
{
    public class Client
    {

        //Game Incorperation Variables
        public int ClientIndex;
        public int EntityIndex;
        public Player Entity;

        //Ping Values
        public long ConnectionTimeout;
        public long TimeoutLength = 10;
        public long PingTime = 0;

        //Client Properties
        public bool IsEditor;
        public int Power = 0;

        //Database ID
        public int Id = -1;

        //Network Variables
        private byte[] readBuff = new byte[1];
        private long connectTime;
        private TcpClient mySocket;
        private NetworkStream myStream;
        private PacketHandler packetHandler = new PacketHandler();
        private ByteBuffer _myBuffer = new ByteBuffer();
        private Object _bufferLock = new Object();
        public bool isConnected;


        public Client(int myIndex, int entIndex, TcpClient socket)
        {
            mySocket = socket;
            if (mySocket.Connected) { myStream = mySocket.GetStream(); }
            readBuff = new byte[mySocket.ReceiveBufferSize];
            connectTime = Environment.TickCount;
            if (myStream != null) { myStream.BeginRead(readBuff, 0, mySocket.ReceiveBufferSize, OnReceiveData, connectTime); }
            ClientIndex = myIndex;
            EntityIndex = entIndex;
            if (EntityIndex > -1) { Entity = (Player)Globals.Entities[EntityIndex]; }
            if (mySocket.Connected) { PacketSender.SendPing(this); }
            isConnected = true;
            ConnectionTimeout = -1;
        }

        private void OnReceiveData(IAsyncResult ar)
        {
            int packetLen;
            var newBytes = new byte[1];
            if ((long)ar.AsyncState != connectTime) { return; }
            lock (_bufferLock)
            {
                try
                {
                    var readbytes = myStream.EndRead(ar);
                    if (readbytes <= 0) { Globals.GeneralLogs.Add("No bytes read, disconnecting."); HandleDisconnect(); return; }
                    newBytes = new byte[readbytes];
                    Buffer.BlockCopy(readBuff, 0, newBytes, 0, readbytes);
                    _myBuffer.WriteBytes(newBytes);
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
                                packetHandler.HandlePacket(this, _myBuffer.ReadBytes(packetLen));
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
                    readBuff = new byte[mySocket.ReceiveBufferSize];
                    myStream.BeginRead(readBuff, 0, mySocket.ReceiveBufferSize, OnReceiveData, connectTime);
                }
                catch (Exception)
                {
                    Globals.GeneralLogs.Add("Socket end read error.");
                    HandleDisconnect();
                    return;
                }
            }
        }

        public void HandleDisconnect()
        {
            if (isConnected)
            {
                isConnected = false;
                if (myStream != null) { myStream.Close(); }
                myStream = null;
                if (mySocket != null) { mySocket.Close(); }
                mySocket = null;
                try
                {
                    Globals.GeneralLogs.Add("Client disconnected.");
                    if (EntityIndex > -1 && Globals.Entities[EntityIndex] != null)
                    {
                        Database.SavePlayer(Globals.Clients[ClientIndex]);
                        PacketSender.SendEntityLeave(EntityIndex, 0, Globals.Entities[EntityIndex].CurrentMap);
                        if (Globals.Entities[EntityIndex] == null) { return; }
                        PacketSender.SendGlobalMsg(Globals.Entities[EntityIndex].MyName + " has left the Intersect engine");
                        Globals.Clients[ClientIndex] = null;
                        Globals.Entities[EntityIndex] = null;
                        Entity = null;

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
                    if (IsConnected(mySocket))
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
                                PacketSender.SendPing(this);
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

        public void SendPacket(byte[] packet)
        {
            var buff = new ByteBuffer();
            try
            {
                if (isConnected)
                {
                    buff.WriteInteger(packet.Length);
                    buff.WriteBytes(packet);
                    myStream.Write(buff.ToArray(), 0, buff.Count());
                }
            }
            catch (Exception ex)
            {
                Globals.GeneralLogs.Add("Send exception, disconnecting.");
                Globals.GeneralLogs.Add(ex.InnerException.ToString());
                Globals.GeneralLogs.Add(ex.ToString());
                HandleDisconnect();
                return;
            }
        }

        public static bool IsConnected(TcpClient socket)
        {
            return socket.Connected;
        }
    }
}

