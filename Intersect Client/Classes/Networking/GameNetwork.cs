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
using IntersectClientExtras.Network;
using Intersect_Client.Classes.General;
using Intersect_Library;

namespace Intersect_Client.Classes.Networking
{
    public static class GameNetwork
    {
        public static GameSocket MySocket;
        
        public static bool Connected;
        public static bool Connecting;
        private static byte[] _tempBuff;
        private static ByteBuffer _myBuffer = new ByteBuffer();
        private static Object _bufferLock = new Object();
        public static int Ping = 0;

        public static void InitNetwork()
        {
            if (MySocket != null)
            {
                MySocket.Connected += MySocket_OnConnected;
                MySocket.Disconnected += MySocket_OnDisconnected;
                MySocket.DataReceived += MySocket_OnDataReceived;
                MySocket.ConnectionFailed += MySocket_OnConnectionFailed;
                TryConnect();
            }
        }

        private static void TryConnect()
        {
            MySocket.Connect(Globals.Database.ServerHost, Globals.Database.ServerPort);
        }

        private static void MySocket_OnConnectionFailed()
        {
            TryConnect();
        }

        public static void PushData(byte[] data)
        {
            lock (_bufferLock)
            {
                _myBuffer.WriteBytes(data);
            }
            TryHandleData();

        }

        private static void MySocket_OnDataReceived(byte[] data)
        {
            lock (_bufferLock)
            {
                _myBuffer.WriteBytes(data);
            }
            TryHandleData();
        }

        private static void MySocket_OnDisconnected()
        {
            //Not sure how to handle this yet!
            Globals.IsRunning = false;
        }

        private static void MySocket_OnConnected()
        {
            //Not sure how to handle this yet!
            Connected = true;
        }

        public static void Close()
        {
            try
            {
                Connected = false;
                Connecting = false;
                MySocket.Disconnect();
                MySocket.Dispose();
                MySocket = null;
            }
            catch (Exception)
            {

            }
        }

        public static void SendPacket(byte[] data)
        {
            try
            {
                var buff = new ByteBuffer();
                buff.WriteInteger(data.Length);
                buff.WriteBytes(data);
                MySocket.SendData(buff.ToArray());
            }
            catch (Exception)
            {
            }
        }

        public static void Update()
        {
            if (MySocket != null)
            {
                MySocket.Update();
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
