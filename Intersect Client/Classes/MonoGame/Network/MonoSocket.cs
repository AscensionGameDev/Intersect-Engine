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
using System.Net.Sockets;
using IntersectClientExtras.Network;
using Intersect_Client.Classes.General;

namespace Intersect_Client.Classes.Bridges_and_Interfaces.SFML.Network
{
    public class MonoSocket : GameSocket
    {
        public static bool Connected;
        public static bool WasConnected;
        public static bool Connecting;
        private static byte[] _tempBuff;
        public static TcpClient MySocket;
        private static NetworkStream _myStream;
        public MonoSocket() : base()
        {
            MySocket = new TcpClient { NoDelay = true };
            _tempBuff = new byte[MySocket.ReceiveBufferSize];

            MySocket.SendTimeout = 100000;
            MySocket.ReceiveTimeout = 100000;
        }

        public override void Connect(string host, int port)
        {
            MySocket.BeginConnect(host, port, ConnectCallback, null);
            Connecting = true;
        }

        /// <summary>
        /// This is called when the socket succeeded or failed in connecting to the server.
        /// This will error on the .EndConnect line if the server is offline or unreachable.
        /// Since this error is caught with the Try-Catch you can ignore it.
        /// </summary>
        /// <param name="result"></param>
        private void ConnectCallback(IAsyncResult result) 
        {
            try
            {
                MySocket.EndConnect(result);
                if (MySocket.Connected)
                {
                    Connected = true;
                    Connecting = false;
                    _myStream = MySocket.GetStream();
                    _myStream.BeginRead(_tempBuff, 0, MySocket.ReceiveBufferSize, ReceiveCallback, null);
                    OnConnected();
                }
                else
                {
                    OnConnectionFailed();
                    Connected = false;
                    Connecting = false;
                }
            }
            catch (Exception ex)
            {
                Globals.System.LogError(ex.ToString());
                Connected = false;
                Connecting = false;
                OnConnectionFailed();
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                var readAmt = _myStream.EndRead(result);
                if (readAmt <= 0)
                {
                    HandleDisconnect();
                }
                var receivedData = new byte[readAmt];
                Buffer.BlockCopy(_tempBuff, 0, receivedData, 0, readAmt);
                OnDataReceived(receivedData);
                _myStream.BeginRead(_tempBuff, 0, MySocket.ReceiveBufferSize, ReceiveCallback, null);
            }
            catch (Exception ex)
            {
                HandleDisconnect();
            }
        }

        private void HandleDisconnect()
        {
            Connected = false;
            Connecting = false;
            OnDisconnected();
        }

        public override void SendData(byte[] data)
        {
            try
            {
                _myStream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                HandleDisconnect();
            }
        }

        public override void Update()
        {
            //Data
        }

        public override void Disconnect()
        {
            _myStream.Close();
            MySocket.Close();
            Connected = false;
            Connecting = false;
            OnDisconnected();
        }

        public override void Dispose()
        {
            MySocket = null;
            _myStream = null;
        }

        public override bool IsConnected()
        {
            return Connected;
        }
    }
}
