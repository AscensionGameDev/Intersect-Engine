using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp.Net.WebSockets;

namespace Intersect_Server.Classes.Networking
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

        public override void SendData(byte[] data)
        {
            _myContext.WebSocket.Send(data);
        }

        public override void Update()
        {
            //Completely event driven, no need to update here.
        }

        public override void Disconnect()
        {
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
    }
}
