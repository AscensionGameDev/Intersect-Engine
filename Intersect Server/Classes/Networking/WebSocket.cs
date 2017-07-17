using System;
using Intersect.Logging;
using WebSocketSharp.Net.WebSockets;

namespace Intersect.Server.Classes.Networking
{
    public class WebSocket : GameSocket
    {
        private WebSocketContext _myContext;

        public WebSocket(WebSocketContext context)
        {
            _myContext = context;
            _myContext.WebSocket.OnMessage += WebSocket_OnMessage;
            _myContext.WebSocket.OnClose += WebSocket_OnClose;
            _myContext.WebSocket.OnError += WebSocket_OnError;
            _isConnected = true;
        }

        private void WebSocket_OnError(object sender, WebSocketSharp.ErrorEventArgs e)
        {
            HandleDisconnect();
        }

        private void WebSocket_OnClose(object sender, WebSocketSharp.CloseEventArgs e)
        {
            HandleDisconnect();
        }

        private void WebSocket_OnMessage(object sender, WebSocketSharp.MessageEventArgs e)
        {
            if (e.IsBinary)
            {
                lock (_bufferLock)
                {
                    _myBuffer.WriteBytes(e.RawData);
                }
                TryHandleData();
            }
        }

        public override void SendData(ByteBuffer bf)
        {
            try
            {
                _myContext.WebSocket.Send(bf.ToArray());
            }
            catch (Exception ex)
            {
                Log.Trace(ex);
                HandleDisconnect();
            }
        }

        public override void Update()
        {
            //Completely event driven, no need to update here.
        }

        public override void Disconnect()
        {
            base.Disconnect();
            if (_isConnected)
            {
                _myContext.WebSocket.Close();
            }
        }

        public override void Dispose()
        {
            Disconnect();
        }

        public override bool IsConnected()
        {
            return _isConnected;
        }

        public override string GetIP()
        {
            if (_myContext != null)
            {
                return _myContext.UserEndPoint.Address.ToString();
            }
            else
            {
                return "";
            }
        }
    }
}