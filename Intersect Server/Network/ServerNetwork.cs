using System;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Threading;
using Lidgren.Network;

namespace Intersect.Server.Network
{
    public class ServerNetwork : AbstractNetwork
    {
        public new NetServer Peer => (NetServer)base.Peer;

        public ServerNetwork(NetPeerConfiguration config) : base(config, new NetServer(config))
        {
        }

        public override void Connect()
        {
            throw new NotImplementedException();
        }

        public override void Listen()
        {
            Peer.Start();
        }

        public override bool Send(IPacket packet)
        {
            var message = Peer.CreateMessage();
            if (!packet.Write(ref message)) throw new Exception();

            try
            {
                Peer.SendToAll(message, NetDeliveryMethod.ReliableOrdered);
                return true;
            }
            catch (Exception exception)
            {
                Log.Trace(exception);
                return false;
            }
        }

        protected override void Poll()
        {
            throw new NotImplementedException();
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