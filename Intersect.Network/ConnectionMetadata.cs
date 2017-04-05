using System;
using System.IO;
using System.Security.Cryptography;
using Intersect.Logging;
using Intersect.Memory;
using Lidgren.Network;

namespace Intersect.Network
{
    public class ConnectionMetadata : IConnection
    {
        public INetwork Network { get; }
        public Guid Guid { get; }
        public NetConnection Connection { get; }
        public RSACryptoServiceProvider Rsa { get; private set; }
        public NetAESEncryption Aes { get; private set; }

        private byte[] mRsaSecret;

        public ConnectionMetadata(INetwork network, NetConnection connection, byte[] aesKey)
            : this(network, Guid.NewGuid(), connection, aesKey, null)
        {
        }

        public ConnectionMetadata(INetwork network, NetConnection connection, byte[] aesKey, RSAParameters rsaParameters)
            : this(network, Guid.NewGuid(), connection, aesKey, rsaParameters)
        {

        }

        public ConnectionMetadata(INetwork network, Guid guid, NetConnection connection, byte[] aesKey, RSAParameters? rsaParameters = null)
        {
            Network = network;
            Guid = guid;
            Connection = connection;
            Aes = new NetAESEncryption(Connection.Peer, aesKey, 0, aesKey.Length);
            Rsa = new RSACryptoServiceProvider(2048);

            if (rsaParameters.HasValue)
            {
                Rsa.ImportParameters(rsaParameters.Value);
            }
        }

        public void Dispose()
        {
            Connection.Disconnect("status_disposing");
        }

        public IBuffer CreateBuffer()
            => new LidgrenBuffer(Connection.Peer.CreateMessage());

        public bool Send(IPacket packet)
            => Network.Send(Guid, packet);

        public bool Send(Guid guid, IPacket packet)
            => Network.Send(guid, packet);
    }
}