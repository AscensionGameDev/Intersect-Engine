namespace Intersect.Network.Events;

public partial class ConnectionEventArgs : EventArgs
{
    public Guid EventId { get; set; }

    public NetworkStatus NetworkStatus { get; set; }

    public IConnection Connection { get; set; }
}