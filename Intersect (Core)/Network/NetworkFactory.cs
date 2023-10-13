using System.Security.Cryptography;
using Intersect.Core;

namespace Intersect.Network;

public delegate INetwork NetworkFactory(
    IApplicationContext applicationContext,
    RSAParameters rsaParameters,
    HandlePacket handlePacket,
    ShouldProcessPacket? shouldProcessPacket
);