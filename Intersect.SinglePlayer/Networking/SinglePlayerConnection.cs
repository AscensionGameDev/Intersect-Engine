using Intersect.Network;

namespace Intersect.SinglePlayer.Networking;

internal class SinglePlayerConnection : IConnection
{
    private readonly SinglePlayerNetwork _network;

    internal SinglePlayerConnection(SinglePlayerNetwork network)
    {
        _network = network;
        Statistics = new ConnectionStatistics();
    }


    public Guid Guid => default;
    public bool IsConnected => _network.IsConnected;
    public string Ip => "127.0.0.1";
    public int Port => 5400;
    public ConnectionStatistics Statistics { get; }

    public void Dispose()
    {
    }

    public bool Send(IPacket packet, TransmissionMode mode = TransmissionMode.All) => _network.Send(packet, mode);

    public void HandleConnected()
    {
        throw new NotImplementedException();
    }

    public void HandleApproved()
    {
        throw new NotImplementedException();
    }

    public void HandleDisconnected()
    {
        throw new NotImplementedException();
    }

    public void Disconnect(string? message = default) => _network.Disconnect(message);
}