using Intersect.Network;
using Intersect.Network.Events;

using JetBrains.Annotations;

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

        protected void OnConnected([NotNull] INetworkLayerInterface sender, [NotNull] ConnectionEventArgs connectionEventArgs)
        {
            Connected?.Invoke(sender, connectionEventArgs);
        }

        protected void OnConnectionFailed([NotNull] INetworkLayerInterface sender, [NotNull] ConnectionEventArgs connectionEventArgs, bool denied)
        {
            ConnectionFailed?.Invoke(sender, connectionEventArgs, denied);
        }

        protected void OnDisconnected([NotNull] INetworkLayerInterface sender, [NotNull] ConnectionEventArgs connectionEventArgs)
        {
            Disconnected?.Invoke(sender, connectionEventArgs);
        }

    }

    public delegate void DataReceivedHandler(IPacket packet);

    public delegate void ConnectedHandler([NotNull] INetworkLayerInterface sender, [NotNull] ConnectionEventArgs connectionEventArgs);

    public delegate void ConnectionFailedHandler([NotNull] INetworkLayerInterface sender, [NotNull] ConnectionEventArgs connectionEventArgs, bool denied);

    public delegate void DisconnectedHandler([NotNull] INetworkLayerInterface sender, [NotNull] ConnectionEventArgs connectionEventArgs);

}
