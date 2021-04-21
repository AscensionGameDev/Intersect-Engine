using Intersect.Server.Networking;
using System.Collections.Generic;
using System.Linq;

namespace Intersect.Server.Metrics.Controllers
{
    public class NetworkMetricsController : MetricsController
    {
        private const string CONTEXT = "Network";

        public Histogram Clients { get; private set; }

        public Histogram TotalBandwidth { get; private set; }

        public Histogram SentBytes { get; private set; }

        public Histogram SentPackets { get; private set; }

        public Histogram ReceivedBytes { get; private set; }

        public Histogram ReceivedPackets { get; private set; }

        public Histogram AcceptedBytes { get; private set; }

        public Histogram AcceptedPackets { get; private set; }

        public Histogram DroppedBytes { get; private set; }

        public Histogram DroppedPackets { get; private set; }

        public Histogram TotalReceivedPacketHandlingTime { get; private set; }

        public Histogram TotalSentPacketProcessingTime { get; private set; }

        public NetworkMetricsController()
        {
            Context = CONTEXT;

            Clients = new Histogram(nameof(Clients), this);
            TotalBandwidth = new Histogram(nameof(TotalBandwidth), this);
            SentBytes = new Histogram(nameof(SentBytes), this);
            SentPackets = new Histogram(nameof(SentPackets), this);
            ReceivedBytes = new Histogram(nameof(ReceivedBytes), this);
            ReceivedPackets = new Histogram(nameof(ReceivedPackets), this);
            AcceptedBytes = new Histogram(nameof(AcceptedBytes), this);
            AcceptedPackets = new Histogram(nameof(AcceptedPackets), this);
            DroppedBytes = new Histogram(nameof(DroppedBytes), this);
            DroppedPackets = new Histogram(nameof(DroppedPackets), this);
            TotalReceivedPacketHandlingTime = new Histogram(nameof(TotalReceivedPacketHandlingTime), this);
            TotalSentPacketProcessingTime = new Histogram(nameof(TotalSentPacketProcessingTime), this);

        }


        public override IDictionary<string, object> Data()
        {
            var res = base.Data();

            var top10Sent = PacketSender.SentPacketTypes.Where(pair => pair.Value > 0).OrderByDescending(pair => pair.Value).Take(10).ToArray();
            var top10Received = PacketHandler.AcceptedPacketTypes.Where(pair => pair.Value > 0).OrderByDescending(pair => pair.Value).Take(10).ToArray();

            res.Add("MostSentPackets", top10Sent);
            res.Add("MostReceivedPackets", top10Received);

            return res;
        }
    }
}