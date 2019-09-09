using Intersect.Network;

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
        public abstract int Ping();
        public event DataReceivedHandler DataReceived;
        public event ConnectedHandler Connected;
        public event ConnectionFailedHandler ConnectionFailed;
        public event DisconnectedHandler Disconnected;

        protected void OnDataReceived(IPacket packet)
        {
            DataReceived?.Invoke(packet);
        }

        protected void OnConnected()
        {
            Connected?.Invoke();
        }

        protected void OnConnectionFailed(bool denied)
        {
            ConnectionFailed?.Invoke(denied);
        }

        protected void OnDisconnected()
        {
            Disconnected?.Invoke();
        }
    }

    public delegate void DataReceivedHandler(IPacket packet);

    public delegate void ConnectedHandler();

    public delegate void ConnectionFailedHandler(bool denied);

    public delegate void DisconnectedHandler();
}