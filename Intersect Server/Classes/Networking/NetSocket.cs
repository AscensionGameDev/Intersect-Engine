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
using System.IO;
using System.Net.Sockets;
using Intersect_Server.Classes.General;

namespace Intersect_Server.Classes.Networking
{

    public class NetSocket : GameSocket
    {
        private TcpClient _mySocket;
        private NetworkStream _myStream;
        private byte[] _readBuff;

        public NetSocket(TcpClient socket)
        {
            _mySocket = socket;
            _mySocket.SendBufferSize = 256000;
            _mySocket.ReceiveBufferSize = 256000;
            _myStream = _mySocket.GetStream();
            _readBuff = new byte[_mySocket.ReceiveBufferSize];
            if (_myStream != null) { _myStream.BeginRead(_readBuff, 0, _mySocket.ReceiveBufferSize, OnReceiveData, null); }
            _isConnected = true;
        }

        public override void SendData(byte[] data)
        {
            try
            {
                if (_mySocket != null && _mySocket.Connected && _myStream != null) _myStream.Write(data, 0, data.Length);
            }
            catch (Exception)
            {
                Globals.GeneralLogs.Add("Socket end read error.");
                HandleDisconnect();
            }
        }

        private void OnReceiveData(IAsyncResult ar)
        {
            if (_mySocket == null) return;
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
            catch (System.ObjectDisposedException ex)
            {
                //Trying to read from a disconnected socket
            }
            catch (Exception ex)
            {
                Globals.GeneralLogs.Add("Socket end read error.");
                HandleDisconnect();
            }
        }

        public override void Disconnect()
        {
            base.Disconnect();
            if (_mySocket != null)
            {
                _mySocket.Close();
                _mySocket = null;
            }
        }

        public override void Dispose()
        {

        }

        public override bool IsConnected()
        {
            return _isConnected;
        }

        public override string GetIP()
        {
            if (_mySocket != null)
            {
                return ((System.Net.IPEndPoint)_mySocket.Client.RemoteEndPoint).Address.ToString();
            }
            else
            {
                return "";
            }
        }
    }


}