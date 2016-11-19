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
using System;
using System.Net.Sockets;
using System.Windows.Forms;
using Intersect_Library;

namespace Intersect_Editor.Classes
{
    public static class Network
    {
        public static TcpClient MySocket;
        private static NetworkStream _myStream;
        public static bool Connected;
        public static bool Connecting;
        private static byte[] _tempBuff;
        private static ByteBuffer _myBuffer = new ByteBuffer();
        private static Object _bufferLock = new Object();

        public static void InitNetwork()
        {
            if (MySocket != null)
            {
                MySocket.Close();
            }

            MySocket = new TcpClient ();
            MySocket.SendBufferSize = 256000;
            MySocket.ReceiveBufferSize = 256000;
            _tempBuff = new byte[MySocket.ReceiveBufferSize];
            MySocket.BeginConnect(Globals.ServerHost, Globals.ServerPort, ConnectCb, null);
            Connecting = true;
        }

        public static void Update()
        {
            if (Connected) { TryHandleData(); }
            if (!Connected && !Connecting)
            {
                InitNetwork();
            }
        }

        private static void ConnectCb(IAsyncResult result)
        {
            try
            {
                MySocket.EndConnect(result);
                if (MySocket.Connected)
                {
                    Connected = true;
                    Connecting = false;
                    _myStream = MySocket.GetStream();
                    _myStream.BeginRead(_tempBuff, 0, MySocket.ReceiveBufferSize, ReceiveCb, null);
                }
                else
                {
                    Connected = false;
                    Connecting = false;
                }
            }
            catch (Exception)
            {
                Connected = false;
                Connecting = false;
            }
        }

        public static void CheckNetwork()
        {
            if (Connected == false && Connecting == false)
            {
                InitNetwork();
            }
            else
            {
                if (!Connected)
                {
                    //PROBLEM!
                }
            }
        }

        private static void ReceiveCb(IAsyncResult result)
        {
            try
            {
                var readAmt = _myStream.EndRead(result);
                if (readAmt <= 0)
                {
                    HandleDc();
                }
                var receivedData = new byte[readAmt];
                Buffer.BlockCopy(_tempBuff, 0, receivedData, 0, readAmt);
                lock (_bufferLock)
                {
                    _myBuffer.WriteBytes(receivedData);
                }
                _myStream.BeginRead(_tempBuff, 0, MySocket.ReceiveBufferSize, ReceiveCb, null);
            }
            catch (Exception)
            {
                HandleDc();
            }
        }

        public static void DestroyNetwork()
        {
            try
            {
                _myStream.Close();
                MySocket.Close();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private static void HandleDc()
        {
            if (Globals.MainForm != null && Globals.MainForm.Visible)
            {
                if (Globals.MainForm.DisconnectDelegate != null)
                {
                    Globals.MainForm.BeginInvoke(Globals.MainForm.DisconnectDelegate);
                    Globals.MainForm.DisconnectDelegate = null;
                }
            }
            else if (Globals.LoginForm.Visible)
            {
                Connected = false;
                Connecting = false;
                InitNetwork();
            }
            else
            {
                MessageBox.Show(@"Disconnected!");
                Application.Exit();
            }
        }

        public static void SendPacket(byte[] data)
        {
            try
            {
                var buff = new ByteBuffer();
                buff.WriteInteger(data.Length);
                buff.WriteBytes(data);
                _myStream.Write(buff.ToArray(), 0, buff.Count());
            }
            catch (Exception)
            {
                HandleDc();
            }
        }

        private static void TryHandleData()
        {
            lock (_bufferLock)
            {
                while (_myBuffer.Length() >= 4)
                {
                    var packetLen = _myBuffer.ReadInteger(false);
                    if (_myBuffer.Length() >= packetLen + 4)
                    {
                        _myBuffer.ReadInteger();
                        PacketHandler.HandlePacket(_myBuffer.ReadBytes(packetLen));
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
    }
}
