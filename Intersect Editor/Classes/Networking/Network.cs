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

namespace Intersect_Editor.Classes
{
    public class Network
    {
        TcpClient _mySocket;
        NetworkStream _myStream;
        readonly PacketHandler _packetHandler = new PacketHandler();
        readonly List<byte> _myBuffer = new List<byte>();
        public long ReconnectTime;
        public bool IsConnected;
        public bool IsConnecting;


        // Update is called once per frame
        public void Update()
        {
            var shouldInitSocket = false;
            if (ReconnectTime == -1) { ReconnectTime = Environment.TickCount + 10000; }
            if (_mySocket == null) { shouldInitSocket = true; }
            if (shouldInitSocket == false && _mySocket.Connected == false) { shouldInitSocket = true; }
            if (shouldInitSocket && Environment.TickCount > ReconnectTime)
            {
                ReconnectTime = long.MaxValue ;
                _mySocket = new TcpClient();
                _mySocket.BeginConnect("127.0.0.1", 6000, ConnectCallback, _mySocket);
                IsConnecting = true;
            }

            if (!IsConnected) { return; }


            try
            {
                var tempBuff = new byte[4096];
                if (_myStream.DataAvailable)
                {
                    var readAmt = _myStream.Read(tempBuff, 0, 4096);
                    if (readAmt > 0)
                    {
                        for (var i = 0; i < readAmt; i++)
                        {
                            _myBuffer.Add(tempBuff[i]);
                        }
                    }
                }
            }
            catch
            {
                _mySocket = null;
                IsConnected = false;
                return;
            }

            if (_myBuffer.Count < 4) return;
            var buff = new ByteBuffer();
            buff.WriteBytes(_myBuffer.ToArray());
            while (buff.Length() >= 4)
            {
                var packetLen = buff.ReadInteger(false);
                if (buff.Length() >= packetLen + 4)
                {
                    buff.ReadInteger();
                    _packetHandler.HandlePacket(buff.ReadBytes(packetLen));
                }
                else
                {
                    break;
                }
            }
            _myBuffer.Clear();
            if (buff.Length() > 0) { _myBuffer.AddRange(buff.ReadBytes(buff.Length())); }
        }


        public void SendPacket(byte[] packet)
        {
            try
            {
                var buff = new ByteBuffer();
                buff.WriteInteger(packet.Length);
                buff.WriteBytes(packet);
                
                _myStream.Write(buff.ToArray(), 0, buff.Count());
            }
            catch (Exception)
            {
                Globals.GameSocket.IsConnected = false;
                Globals.GameSocket._mySocket.Close();
            }
        }

        /*mySocket.NoDelay = true;
                myStream = mySocket.GetStream();
                isConnected = true;*/
        void ConnectCallback(IAsyncResult asyncConnect)
        {
            try
            {
                _mySocket.EndConnect(asyncConnect);
                // arriving here means the operation completed
                // (asyncConnect.IsCompleted = true) but not
                // necessarily successfully
                if (_mySocket.Connected == false)
                {
                    IsConnecting = false;
                    IsConnected = false;
                    ReconnectTime = -1;
                }
                else
                {
                    _mySocket.NoDelay = true;
                    _myStream = _mySocket.GetStream();
                    IsConnected = true;
                    IsConnecting = false;
                }
            }
            catch (Exception)
            {
                IsConnecting = false;
                IsConnected = false;
                ReconnectTime = -1;
            }
        }
    }
}
