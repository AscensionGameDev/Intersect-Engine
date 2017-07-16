namespace Intersect.Network
{
    public interface IClient : INetwork
    {
        bool Connect();

        bool IsConnected { get; }
        bool IsServerOnline { get; }
        int Ping { get; }
    }
}