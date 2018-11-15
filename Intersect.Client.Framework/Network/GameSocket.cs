namespace Intersect.Client.Framework.Network
{
    public abstract class GameSocket
    {
        public abstract void Connect(string host, int port);
        public abstract void SendData(byte[] data);
        public abstract void Update();
        public abstract void Disconnect(string reason);
        public abstract void Dispose();
        public abstract bool IsConnected();
        public abstract int Ping();
        public event DataReceivedHandler DataReceived;
        public event ConnectedHandler Connected;
        public event ConnectionFailedHandler ConnectionFailed;
        public event DisconnectedHandler Disconnected;

        protected void OnDataReceived(byte[] data)
        {
            DataReceived?.Invoke(data);
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

    public delegate void DataReceivedHandler(byte[] data);

    public delegate void ConnectedHandler();

    public delegate void ConnectionFailedHandler(bool denied);

    public delegate void DisconnectedHandler();
}