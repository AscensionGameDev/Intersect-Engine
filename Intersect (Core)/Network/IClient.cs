namespace Intersect.Network
{

    public interface IClient : INetwork
    {
        IConnection Connection { get; }
        
        bool IsConnected { get; }

        bool IsServerOnline { get; }

        int Ping { get; }

        bool Connect();

    }

}
