using System;
using System.Security.Cryptography;
using Intersect.Memory;
using Lidgren.Network;

namespace Intersect.Network
{
    public class LidgrenConnection : IConnection
    {
        public INetwork Network { get; }
        public Guid Guid { get; }
        public NetConnection NetConnection { get; }
        public RSACryptoServiceProvider Rsa { get; private set; }
        public NetAESEncryption Aes { get; private set; }

        public bool IsConnected => NetConnection?.Status == NetConnectionStatus.Connected;
        public string Ip => NetConnection?.RemoteEndPoint?.Address?.ToString();

        private byte[] mRsaSecret;

        public LidgrenConnection(INetwork network, NetConnection connection, byte[] aesKey)
            : this(network, Guid.NewGuid(), connection, aesKey, null)
        {
        }

        public LidgrenConnection(INetwork network, NetConnection connection, byte[] aesKey, RSAParameters rsaParameters)
            : this(network, Guid.NewGuid(), connection, aesKey, rsaParameters)
        {

        }

        public LidgrenConnection(INetwork network, Guid guid, NetConnection netConnection, byte[] aesKey, RSAParameters? rsaParameters = null)
        {
            Network = network;
            Guid = guid;
            NetConnection = netConnection;
            Aes = new NetAESEncryption(NetConnection.Peer, aesKey, 0, aesKey.Length);
            Rsa = new RSACryptoServiceProvider(2048);

            if (rsaParameters.HasValue)
            {
                Rsa.ImportParameters(rsaParameters.Value);
            }
        }

        public void Dispose()
        {
            NetConnection.Disconnect("status_disposing");
        }

        public IBuffer CreateBuffer()
            => new LidgrenBuffer(NetConnection.Peer.CreateMessage());

        public bool Send(IPacket packet)
            => Network.Send(Guid, packet);

        public bool Send(Guid guid, IPacket packet)
            => Network.Send(guid, packet);
    }
}