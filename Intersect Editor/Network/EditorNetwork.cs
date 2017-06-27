using System;
using System.Security.Cryptography;
using Intersect.Logging;
using Intersect.Memory;
using Intersect.Network;
using Intersect.Threading;
using Lidgren.Network;
using System.Linq;
using Intersect.Editor.Classes;
using Intersect.Network.Packets;

namespace Intersect.Editor.Network
{
    public class EditorNetwork : AbstractNetwork
    {
        public new NetClient Peer => (NetClient)base.Peer;

        private byte[] mHandshakeSecret;
        private RSACryptoServiceProvider mRsa;
        private bool everConnected;

        public EditorNetwork(NetworkConfiguration config) : base(config, typeof(NetClient))
        {
            mRsa = new RSACryptoServiceProvider(2048);
        }

        protected override RSAParameters GetRsaKey()
            //=> LoadKeyFromAssembly(Assembly.GetExecutingAssembly(), "Intersect.Client.PJPjjxJEkTWn7qyhSBqhg24CRy9smykBt2TMMwA4TYQZmujB", false);
            => LoadKeyFromFile("public.bk1", true);

        private const int sizeHandshakeSecret = 32;
        private const int sizeRsa = 2 + 3 + 256;
        private static readonly int sizeSharedSecret = SharedConstants.VERSION_DATA.Length;
        private static readonly int sizeSecret = 4 + sizeHandshakeSecret + sizeSharedSecret + sizeRsa;

        protected override void OnStart()
        {
            Log.Info("Starting the editor...");
            Peer.Start();
            TryConnect();
        }

        private void TryConnect()
        {
            using (var hailBuffer = new MemoryBuffer(sizeSecret))
            {
                hailBuffer.Write(SharedConstants.VERSION_DATA);

                mHandshakeSecret = new byte[32];
                Rng.GetNonZeroBytes(mHandshakeSecret);
                hailBuffer.Write(mHandshakeSecret, 32);

                var publicKey = mRsa.ExportParameters(false);
                hailBuffer.Write((short)(publicKey.Modulus.Length * 8));
                hailBuffer.Write(publicKey.Exponent, 3);
                hailBuffer.Write(publicKey.Modulus, publicKey.Modulus.Length);

                DumpKey(publicKey, true);

                var encryptedMessage = Rsa.Encrypt(hailBuffer.ToArray(), true);
                var hail = Peer.CreateMessage(sizeof(int) + encryptedMessage.Length);
                hail.Write(encryptedMessage.Length);
                hail.Write(encryptedMessage, 0, encryptedMessage.Length);

                Peer.Connect(Config.Host, Config.Port, hail);
            }
        }

        protected override void OnStop()
        {
            Log.Info("Stopping the editor...");
        }

        protected override void RegisterHandlers()
        {
            base.RegisterHandlers();

            Dispatcher.RegisterHandler(typeof(BinaryPacket), PacketHandler.HandlePacket);
        }

        protected override bool HandleConnected(NetIncomingMessage request)
        {
            var lidgrenId = request.SenderConnection.RemoteUniqueIdentifier;
            var remoteHail = request.SenderConnection.RemoteHailMessage;

            if (HasConnection(lidgrenId)) return true;

            var encryptedSize = remoteHail.ReadInt32();
            var encryptedData = remoteHail.ReadBytes(encryptedSize);
            var decryptedData = mRsa.Decrypt(encryptedData, true);
            using (var requestBuffer = new MemoryBuffer(decryptedData))
            {
                byte[] handshakeSecret;
                if (!requestBuffer.Read(out handshakeSecret, 32)) return false;
                if (!mHandshakeSecret.SequenceEqual(handshakeSecret)) return false;

                byte[] aesKey;
                if (!requestBuffer.Read(out aesKey, 32)) return false;

                byte[] guidData;
                if (!requestBuffer.Read(out guidData, 16)) return false;
                Guid = new Guid(guidData);
                var metadata = new LidgrenConnection(this, Guid, request.SenderConnection, aesKey);
                AddConnection(metadata);
                everConnected = true;
                return true;
            }
        }

        protected override bool HandleDisconnected(NetIncomingMessage request)
        {
            if (!base.HandleDisconnected(request)) return false;
            if (!everConnected)
            {
                TryConnect();
                return false;
            }
            LegacyEditorNetwork.HandleDc();

            Stop();

            return true;
        }

        protected override int CalculateNumberOfThreads() => 1;

        protected override IThreadYield CreateThreadYield()
            => new ThreadYieldNet35();
    }
}