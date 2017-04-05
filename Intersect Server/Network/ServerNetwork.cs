using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Security.Cryptography;
using Intersect.Logging;
using Intersect.Memory;
using Intersect.Network;
using Intersect.Threading;
using Lidgren.Network;
using System.Linq;
using Intersect.Network.Packets.Ping;
using Intersect.Server.Network.Handlers;

namespace Intersect.Server.Network
{
    public class ServerNetwork : AbstractNetwork
    {
        public new NetServer Peer => (NetServer)base.Peer;

        public ServerNetwork(NetPeerConfiguration config) : base(config, new NetServer(config))
        {
            
        }

        protected override RSAParameters GetRsaKey()
            => LoadKeyFromAssembly(Assembly.GetExecutingAssembly(), "Intersect.Server.s3auxSt4RhVSbr7p5Vrkw9w9NwAMjbHUmsxZ7vSv3bQt9RXY", true);

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
            var decryptedData = Rsa.Decrypt(encryptedData, true);
            
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

                    Log.Verbose($"Exponent: {BitConverter.ToString(rsaParameters.Exponent)}");
                    Log.Verbose($"Modulus: {BitConverter.ToString(rsaParameters.Modulus)}");

                    responseBuffer.Write(handshakeSecret, 32);

                    var aesKey = new byte[32];
                    Rng.GetNonZeroBytes(aesKey);
                    responseBuffer.Write(aesKey, 32);

                    var metadata = new ConnectionMetadata(this, request.SenderConnection, aesKey, rsaParameters);
                    AddConnection(metadata);

                    responseBuffer.Write(metadata.Guid.ToByteArray(), 16);

                    var encryptedResponse = metadata.Rsa.Encrypt(responseBuffer.ToArray(), true);
                    var response = Peer.CreateMessage(encryptedResponse.Length + sizeof(int));
                    response.Write(encryptedResponse.Length);
                    response.Write(encryptedResponse, 0, encryptedResponse.Length);

                    request.SenderConnection.Approve(response);
                    return true;
                }
            }
        }

        protected override void RegisterHandlers()
        {
            Dispatcher.RegisterHandler(typeof(PingPacket), new PingHandler().HandlePing);
        }

        protected override bool HandleConnected(NetIncomingMessage request)
        {
            var lidgrenId = request.SenderConnection.RemoteUniqueIdentifier;
            if (HasConnection(lidgrenId)) return true;
            Log.Error($"Disconnected client that isn't listed ({lidgrenId}).");
            request.SenderConnection.Disconnect("You weren't approved?");
            return false;
        }

        protected override int CalculateNumberOfThreads()
        {
            const int numReservedThreads = 2;
            const int numSuggestClientsPerThread = 32;

            var numTotalThreads = Environment.ProcessorCount;
            var numAvailableThreads = Math.Max(1, numTotalThreads - numReservedThreads);
            var numTotalClients = Config.MaximumConnections;
            var numSuggestedThreads = numTotalClients / numSuggestClientsPerThread;
            return Math.Max(1, Math.Min(numAvailableThreads, numSuggestedThreads));
        }

        protected override IThreadYield CreateThreadYield()
            => new ThreadYieldNet40();
    }
}