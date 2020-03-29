namespace Intersect.Network
{

    public interface IClient : INetwork
    {

        bool IsConnected { get; }

        bool IsServerOnline { get; }

        int Ping { get; }

        bool Connect();

    }

}
