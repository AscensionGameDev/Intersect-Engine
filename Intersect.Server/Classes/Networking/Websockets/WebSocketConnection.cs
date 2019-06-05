using System;
using Intersect.Network;
using Intersect.Network.Packets.Reflectable;
using WebSocketSharp.Net.WebSockets;
using WebSocketSharp.Server;

namespace Intersect.Server.Networking.Websockets
{
    public class WebSocketConnection : AbstractConnection
    {
        private readonly WebSocketBehavior mBehavior;
        private readonly WebSocketContext mContext;
        private ByteBuffer mBuffer = new ByteBuffer();
        private object mBufferLock = new Object();
        private bool mClientRemoved;
        private PacketHandler mPacketHandler = new PacketHandler();

        public WebSocketConnection(WebSocketContext context, WebSocketBehavior behavior)
        {
            this.mContext = context;
            context.WebSocket.OnMessage += WebSocket_OnMessage;
            context.WebSocket.OnClose += WebSocket_OnClose;
            context.WebSocket.OnError += WebSocket_OnError;
            Client.CreateBeta4Client(this);
        }

        public override string Ip
        {
            get { return this.mContext.UserEndPoint.Address.ToString(); }
        }

        public override int Port
        {
            get { return this.mContext.UserEndPoint.Port; }
        }

        private void WebSocket_OnError(object sender, WebSocketSharp.ErrorEventArgs e)
        {
            if (!mClientRemoved)
            {
                mClientRemoved = true;
                Client.RemoveBeta4Client(this);
            }
        }

        private void WebSocket_OnClose(object sender, WebSocketSharp.CloseEventArgs e)
        {
            if (!mClientRemoved)
            {
                mClientRemoved = true;
                Client.RemoveBeta4Client(this);
            }
        }

        private void WebSocket_OnMessage(object sender, WebSocketSharp.MessageEventArgs e)
        {
            if (e.IsBinary && !mClientRemoved)
            {
                lock (mBufferLock)
                {
                    mBuffer.WriteBytes(e.RawData);
                    ParseData();
                }
            }
        }

        private void ParseData()
        {
            int packetLen;
            if (mClientRemoved) return;
            lock (mBufferLock)
            {
                while (mBuffer.Length() >= 4)
                {
                    packetLen = mBuffer.ReadInteger(false);
                    if (packetLen == 0)
                    {
                        break;
                    }
                    if (mBuffer.Length() >= packetLen + 4)
                    {
                        mBuffer.ReadInteger();
                        var data = mBuffer.ReadBytes(packetLen);
                        var bf = new ByteBuffer();
                        bf.WriteBytes(data);
                        var packet = new BinaryPacket(this, bf);
                        mPacketHandler.HandlePacket(this, packet);
                    }
                    else
                    {
                        break;
                    }
                }
                if (mBuffer.Length() == 0)
                {
                    mBuffer.Clear();
                }
            }
        }

        public override bool Send(IPacket packet)
        {
            try
            {
                if (packet.GetType() == typeof(BinaryPacket))
                {
                    BinaryPacket bpacket = (BinaryPacket) packet;
                    var bf = new ByteBuffer();
                    bf.WriteInteger(bpacket.GetBuffer().ToArray().Length);
                    bf.WriteBytes(bpacket.GetBuffer().ToArray());
                    mContext.WebSocket.SendAsync(bf.ToArray(), null);
                    return true;
                }
                else
                {
                    throw new Exception("Websockets cannot send non-binary packets yet!");
                }
            }
            catch (InvalidOperationException ex)
            {
                //Do Nothing.. the socket is just disconnected.
            }
            return false;
        }
    }
}