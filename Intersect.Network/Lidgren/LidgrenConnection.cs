using System;
using System.Linq;
using System.Security.Cryptography;

using Intersect.Logging;
using Intersect.Network.Packets;

using Lidgren.Network;

namespace Intersect.Network.Lidgren
{

    public sealed class LidgrenConnection : AbstractConnection
    {

        private byte[] mAesKey;

        private byte[] mHandshakeSecret;

        private byte[] mRsaSecret;

        public LidgrenConnection(INetwork network, NetConnection connection, byte[] aesKey) : this(
            network, Guid.NewGuid(), connection, aesKey
        )
        {
        }

        public LidgrenConnection(INetwork network, NetConnection connection, byte[] aesKey, RSAParameters rsaParameters)
            : this(network, Guid.NewGuid(), connection, aesKey)
        {
            CreateRsa(rsaParameters);
        }

        public LidgrenConnection(INetwork network, Guid guid, NetConnection netConnection, byte[] aesKey) : base(guid)
        {
            Network = network;
            NetConnection = netConnection ?? throw new ArgumentNullException();
            mAesKey = aesKey ?? throw new ArgumentNullException();

            CreateAes();
        }

        public LidgrenConnection(
            INetwork network,
            Guid guid,
            NetConnection netConnection,
            byte[] handshakeSecret,
            RSAParameters rsaParameters
        ) : base(guid)
        {
            Network = network;
            NetConnection = netConnection;
            mHandshakeSecret = handshakeSecret;
            CreateRsa(rsaParameters);
        }

        public INetwork Network { get; }

        public NetConnection NetConnection { get; }

        public RSACryptoServiceProvider Rsa { get; private set; }

        public NetAESEncryption Aes { get; private set; }

        public override string Ip => NetConnection?.RemoteEndPoint?.Address?.ToString();

        public override int Port => NetConnection?.RemoteEndPoint?.Port ?? -1;

        private void CreateRsa(RSAParameters rsaParameters)
        {
            Rsa = new RSACryptoServiceProvider();
            Rsa.ImportParameters(rsaParameters);
        }

        private void CreateAes()
        {
            if (NetConnection == null)
            {
                throw new ArgumentNullException();
            }

            if (mAesKey == null)
            {
                throw new ArgumentNullException();
            }

            Aes = new NetAESEncryption(NetConnection.Peer, mAesKey, 0, mAesKey.Length);
        }

        public bool HandleApproval(ApprovalPacket approval)
        {
            if (approval == null)
            {
                throw new ArgumentNullException();
            }

            if (approval.HandshakeSecret == null)
            {
                throw new ArgumentNullException();
            }

            if (approval.AesKey == null)
            {
                throw new ArgumentNullException();
            }

            if (mHandshakeSecret == null)
            {
                throw new ArgumentNullException();
            }

            if (!mHandshakeSecret.SequenceEqual(approval.HandshakeSecret))
            {
                Log.Error("Bad handshake secret received from server.");

                return false;
            }

            mAesKey = approval.AesKey;

            CreateAes();

            return true;
        }

        public override void Dispose()
        {
            base.Dispose();
            NetConnection?.Disconnect(NetworkStatus.Quitting.ToString());
        }

        public override bool Send(IPacket packet)
        {
            return Network?.Send(this, packet) ?? false;
        }

    }

}
