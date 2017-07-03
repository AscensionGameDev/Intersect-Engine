using Intersect.Logging;
using Intersect.Memory;
using Intersect.Network;
using Intersect.Threading;
using Lidgren.Network;
using System;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using Intersect.Network.Packets;
using Intersect.Server.Classes.General;
using Intersect.Server.Classes.Networking;

namespace Intersect.Server.Network
{
    public class ServerNetwork : AbstractNetwork
    {
        public new NetServer Peer => (NetServer)base.Peer;

        private PacketHandler packetHandler = new PacketHandler();

        public ServerNetwork(NetworkConfiguration config) : base(config, typeof(NetServer))
        {
        }

        protected override RSAParameters GetRsaKey()
            => LoadKeyFromAssembly(Assembly.GetExecutingAssembly(), "Intersect.Server.private-intersect.bek", false);
            //=> LoadKeyFromFile("private.bk1", false);

        protected override void OnStart()
        {
            Log.Info("Starting the server...");
            Peer.Start();
        }

        protected override void OnStop()
        {
            Log.Info("Stopping the server...");
        }

        protected override bool HandleConnectionApproval(NetIncomingMessage request)
        {
            var encryptedSize = request.ReadInt32();
            var encryptedData = request.ReadBytes(encryptedSize);

            byte[] decryptedData;
            try
            {
                decryptedData = Rsa.Decrypt(encryptedData, true);
            }
            catch (Exception exception)
            {
                Log.Debug(exception);

                request.SenderConnection?.Deny("bad_public_key");
                return true;
            }
            
            using (var requestBuffer = new MemoryBuffer(decryptedData))
            {
                using (var responseBuffer = new MemoryBuffer())
                {
                    byte[] sharedSecret;
                    if (!requestBuffer.Read(out sharedSecret)) return false;
                    if (!SharedConstants.VERSION_DATA.SequenceEqual(sharedSecret)) return false;
                    
                    byte[] handshakeSecret;
                    if (!requestBuffer.Read(out handshakeSecret, 32)) return false;

                    short rsaBits;
                    if (!requestBuffer.Read(out rsaBits)) return false;

                    var rsaParameters = new RSAParameters();
                    if (!requestBuffer.Read(out rsaParameters.Exponent, 3)) return false;
                    if (!requestBuffer.Read(out rsaParameters.Modulus, rsaBits / 8)) return false;

                    DumpKey(rsaParameters, true);

                    responseBuffer.Write(handshakeSecret, 32);

                    var aesKey = new byte[32];
                    Rng.GetNonZeroBytes(aesKey);
                    responseBuffer.Write(aesKey, 32);

                    var metadata = new LidgrenConnection(this, request.SenderConnection, aesKey, rsaParameters);
                    AddConnection(metadata);

                    responseBuffer.Write(metadata.Guid.ToByteArray(), 16);

                    var encryptedResponse = metadata.Rsa.Encrypt(responseBuffer.ToArray(), true);
                    var response = Peer.CreateMessage(encryptedResponse.Length + sizeof(int));
                    response.Write(encryptedResponse.Length);
                    response.Write(encryptedResponse, 0, encryptedResponse.Length);

                    Client.CreateBeta4Client(metadata);

                    request.SenderConnection.Approve(response);
                    return true;
                }
            }
        }

        protected override void RemoveConnection(LidgrenConnection connection)
        {
            base.RemoveConnection(connection);

            Client.RemoveBeta4Client(connection);
        }

        protected override void RegisterHandlers()
        {
            base.RegisterHandlers();

            Dispatcher.RegisterHandler(typeof(BinaryPacket), packetHandler.HandlePacket);
        }

        protected override bool HandleConnected(NetIncomingMessage request)
        {
            var lidgrenId = request.SenderConnection.RemoteUniqueIdentifier;
            if (HasConnection(lidgrenId)) return true;
            Log.Error($"Disconnected client that isn't listed ({lidgrenId}).");
            request.SenderConnection.Disconnect("You weren't approved?");
            return true;
        }

        protected override int CalculateNumberOfThreads()
        {
            const int numReservedThreads = 2;
            const int numSuggestClientsPerThread = 32;

            var numTotalThreads = Environment.ProcessorCount;
            var numAvailableThreads = Math.Max(1, numTotalThreads - numReservedThreads);
            var numTotalClients = Config.MaximumConnections;
            var numSuggestedThreads = (int)Math.Ceiling((float)numTotalClients / numSuggestClientsPerThread);
            return Math.Max(1, Math.Min(numAvailableThreads, numSuggestedThreads));
        }

        protected override IThreadYield CreateThreadYield()
            => new ThreadYieldNet40();
    }
}