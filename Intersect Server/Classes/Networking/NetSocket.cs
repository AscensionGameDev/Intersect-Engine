

using System;
using System.IO;
using System.Net.Sockets;
using Intersect_Library.Localization;
using Intersect_Library.Logging;
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
                Log.Trace(ex);
                //Trying to read from a disconnected socket
            }
            catch (System.IO.IOException ex)
            {
                Log.Trace(ex);
                HandleDisconnect();
            }
            catch (Exception ex)
            {
                Log.Trace(ex);
                Console.WriteLine(Strings.Get("networking","badpacket"));
                MainClass.CurrentDomain_UnhandledException(null, new UnhandledExceptionEventArgs(ex,false));
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