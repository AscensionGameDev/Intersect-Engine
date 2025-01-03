using Intersect.Network.Events;

namespace Intersect.Network;

public delegate void HandleConnectionEvent(INetworkLayerInterface sender, ConnectionEventArgs connectionEventArgs);