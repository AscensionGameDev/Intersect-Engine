using System.Security.Cryptography;

namespace Intersect.Network;

public delegate INetworkLayerInterface NetworkLayerInterfaceFactory(INetwork network, RSAParameters rsaParameters);