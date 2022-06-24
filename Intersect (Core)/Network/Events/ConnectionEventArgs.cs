using System;

namespace Intersect.Network.Events
{
    public partial class ConnectionEventArgs : EventArgs
    {
        public NetworkStatus NetworkStatus { get; set; }

        public IConnection Connection { get; set; }
    }
}
