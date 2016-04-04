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

using System.Net.Sockets;
using System;
using Intersect_Server.Classes.Networking;

namespace Intersect_Server.Classes
{

    public class NetSocket : GameSocket
    {
        private TcpClient _mySocket;
        private NetworkStream _myStream;
        private byte[] _readBuff;

        public NetSocket(TcpClient socket)
        {
            _mySocket = socket;
            _myStream = _mySocket.GetStream();
            _mySocket.NoDelay = true;
            _readBuff = new byte[_mySocket.ReceiveBufferSize];
            if (_myStream != null) { _myStream.BeginRead(_readBuff, 0, _mySocket.ReceiveBufferSize, OnReceiveData,null); }
            _isConnected = true;
        }

        public override void SendData(byte[] data)
        {
            try
            {
                _myStream.Write(data, 0, data.Length);
            }
            catch (Exception)
            {
                
            }
        }

        private void OnReceiveData(IAsyncResult ar)
        {
            try
            {
                var readbytes = _myStream.EndRead(ar);
                if (readbytes <= 0)
                {
                    Globals.GeneralLogs.Add("No bytes read, disconnecting.");
                    HandleDisconnect();
                    return;
                }
                byte[] receivedData = new byte[readbytes];
                Buffer.BlockCopy(_readBuff, 0, receivedData, 0, readbytes);
                lock (_bufferLock)
                {
                    _myBuffer.WriteBytes(receivedData);
                }
                TryHandleData();
                _readBuff = new byte[_mySocket.ReceiveBufferSize];
                _myStream.BeginRead(_readBuff, 0, _mySocket.ReceiveBufferSize, OnReceiveData, null);
            }
            catch (Exception)
            {
                Globals.GeneralLogs.Add("Socket end read error.");
                HandleDisconnect();
                return;
            }
        }

        public void Update(PacketHandler packetHandler)
        {
            do
            {
                try
                {
                    if (IsConnected())
                    {
                        if (_connectionTimeout > -1 && _connectionTimeout < Environment.TickCount)
                        {
                            //Disconnect
                            HandleDisconnect();
                            return;
                        }
                        else
                        {
                            if (_pingTime < Environment.TickCount)
                            {
                                PacketSender.SendPing(_myClient);
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

        public override void Update()
        {

        }

        public override void Disconnect()
        {
            HandleDisconnect();
        }

        public override void Dispose()
        {

        }

        public override bool IsConnected()
        {
            return _isConnected;
        }
    }


}
