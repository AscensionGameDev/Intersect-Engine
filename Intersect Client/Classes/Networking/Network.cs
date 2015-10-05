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
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Windows.Forms;

namespace Intersect_Client.Classes
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
        public static int Ping = 0;

        public static void InitNetwork()
        {
            if (MySocket != null)
            {
                MySocket.Close();
            }

            MySocket = new TcpClient { NoDelay = true };
            _tempBuff = new byte[MySocket.ReceiveBufferSize];
            MySocket.BeginConnect(Globals.ServerHost, Globals.ServerPort, ConnectCb, null);
            Connecting = true;
            MySocket.SendTimeout = 100000;
            MySocket.ReceiveTimeout = 100000;
        }

        public static void Close()
        {
            try
            {
                Connected = false;
                Connecting = false;
                MySocket.Close();
                MySocket = null;
            }
            catch (Exception)
            {

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
                TryHandleData();
                _myStream.BeginRead(_tempBuff, 0, MySocket.ReceiveBufferSize, ReceiveCb, null);
            }
            catch (Exception ex)
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
            MessageBox.Show(@"Disconnected!");
            GameMain.IsRunning = false;
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
