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
        public Guid Guid { get; }
        public NetConnection Connection { get; }
        public NetworkMetaStatus Status { get; private set; }
        public RSACryptoServiceProvider Rsa { get; private set; }
        public NetAESEncryption Aes { get; private set; }
        public RandomNumberGenerator Rng { get; }

        private byte[] mRsaSecret;

        public ConnectionMetadata(NetConnection connection)
        {
            Guid = Guid.NewGuid();
            Connection = connection;
            Status = NetworkMetaStatus.ConnectionEstablished;
            Rng = new RNGCryptoServiceProvider();
        }

        public NetIncomingMessage NegotiateEncryption(NetIncomingMessage message, RSAParameters? rsaParameters = null)
        {
            if (rsaParameters.HasValue)
            {
                Rsa = new RSACryptoServiceProvider();
                Rsa.ImportParameters(rsaParameters.Value);
            }

            switch (Status)
            {
                case NetworkMetaStatus.ConnectionEstablished:
                    if (Rsa?.PublicOnly ?? false)
                    {
                        Status = NetworkMetaStatus.HandshakeReceived;
                        break;
                    }

                    Status = NetworkMetaStatus.HandshakeRequested;

                    var encryptionKey = new byte[32];
                    Rng.GetNonZeroBytes(encryptionKey);
                    Aes = new NetAESEncryption(Connection.Peer, encryptionKey, 0, 32);

                    mRsaSecret = new byte[32];
                    Rng.GetNonZeroBytes(mRsaSecret);

                    byte[] rsaMessage;
                    using (var memMessage = new MemoryStream())
                    {
                        memMessage.Write(Guid.ToByteArray(), 0, 16);
                        memMessage.Write(encryptionKey, 0, 32);
                        memMessage.Write(mRsaSecret, 0, 32);

                        rsaMessage = Rsa.Encrypt(memMessage.ToArray(), true);
                        memMessage.Close();
                    }

                    var handshakeRequest = Connection.Peer.CreateMessage(rsaMessage.Length + sizeof(int));
                    handshakeRequest.Write(rsaMessage.Length);
                    handshakeRequest.Write(rsaMessage);
                    Connection.SendMessage(handshakeRequest, NetDeliveryMethod.ReliableOrdered, 0);
                    break;

                case NetworkMetaStatus.Connected:
                    if (message.Decrypt(Aes))
                    {
                        return message;
                    }

                    Log.Error($"[{Guid}] Failed to decrypt message.");
                    break;

                case NetworkMetaStatus.HandshakeCompleted:
                    if (message.Decrypt(Aes))
                    {
                        return message;
                    }

                    Log.Error($"[{Guid}] Failed to decrypt message.");
                    break;

                case NetworkMetaStatus.HandshakeReceived:
                    var packetData = message.ReadBytes(message.LengthBytes - 1);
                    break;

                case NetworkMetaStatus.HandshakeRequested:
                    if (Aes != null)
                    {
                        Status = NetworkMetaStatus.HandshakeCompleted;
                    }
                    break;

                case NetworkMetaStatus.Unknown:
                    Log.Error($"[{Guid}] Bad network state, terminating.");
                    Connection.Disconnect("Bye.");
                    break;

                default:
                    break;
            }

            return null;
        }
        
        public void Dispose()
        {
            Connection.Disconnect("status_disposing");
        }

        public IBuffer CreateBuffer()
            => new LidgrenBuffer(Connection.Peer.CreateMessage());
    }
}