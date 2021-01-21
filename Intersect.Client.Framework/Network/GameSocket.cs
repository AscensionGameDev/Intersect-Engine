using Intersect.Network;
using Intersect.Network.Events;

namespace Intersect.Client.Framework.Network
{

    public abstract class GameSocket
    {

        public abstract void Connect(string host, int port);

        public abstract void SendPacket(object packet);

        public abstract void Update();

        public abstract void Disconnect(string reason);

        public abstract void Dispose();

        public abstract bool IsConnected();

        public abstract int Ping { get; }

        public event DataReceivedHandler DataReceived;

        public event ConnectedHandler Connected;

        public event ConnectionFailedHandler ConnectionFailed;

        public event DisconnectedHandler Disconnected;

        protected void OnDataReceived(IPacket packet)
        {
            DataReceived?.Invoke(packet);
        }

        protected void OnConnected(INetworkLayerInterface sender, ConnectionEventArgs connectionEventArgs)
        {
            Connected?.Invoke(sender, connectionEventArgs);
        }

        protected void OnConnectionFailed(INetworkLayerInterface sender, ConnectionEventArgs connectionEventArgs, bool denied)
        {
            ConnectionFailed?.Invoke(sender, connectionEventArgs, denied);
        }

        protected void OnDisconnected(INetworkLayerInterface sender, ConnectionEventArgs connectionEventArgs)
        {
            Disconnected?.Invoke(sender, connectionEventArgs);
        }

    }

    public delegate void DataReceivedHandler(IPacket packet);

    public delegate void ConnectedHandler(INetworkLayerInterface sender, ConnectionEventArgs connectionEventArgs);

    public delegate void ConnectionFailedHandler(INetworkLayerInterface sender, ConnectionEventArgs connectionEventArgs, bool denied);

    public delegate void DisconnectedHandler(INetworkLayerInterface sender, ConnectionEventArgs connectionEventArgs);

}
