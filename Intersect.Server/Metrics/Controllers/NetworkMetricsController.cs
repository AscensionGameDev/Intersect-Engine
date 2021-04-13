using App.Metrics;
using App.Metrics.Histogram;
using Intersect.Server.Networking;
using System.Collections.Generic;
using System.Linq;

namespace Intersect.Server.Metrics.Controllers
{
    public class NetworkMetricsController : MetricsController
    {
        private const string CONTEXT = "Network";

        public NetworkMetricsController(IMetricsRoot root)
        {
            Context = CONTEXT;
            mAppMetricsRoot = root;
        }

        private readonly HistogramOptions mClientCount = new HistogramOptions() { Name = "Clients", Context = CONTEXT };

        private readonly HistogramOptions mBandwidth = new HistogramOptions() { Name = "TotalBandwidth", Context = CONTEXT };

        private readonly HistogramOptions mSentBytes = new HistogramOptions() { Name = "SentBytes", Context = CONTEXT };

        private readonly HistogramOptions mSentPackets = new HistogramOptions() { Name = "SentPackets", Context = CONTEXT };

        private readonly HistogramOptions mReceivedBytes = new HistogramOptions() { Name = "ReceivedBytes", Context = CONTEXT };

        private readonly HistogramOptions mReceivedPackets = new HistogramOptions() { Name = "ReceivedPackets", Context = CONTEXT };

        private readonly HistogramOptions mAcceptedBytes = new HistogramOptions() { Name = "AcceptedBytes", Context = CONTEXT };

        private readonly HistogramOptions mAcceptedPackets = new HistogramOptions() { Name = "AcceptedPackets", Context = CONTEXT };

        private readonly HistogramOptions mDroppedBytes = new HistogramOptions() { Name = "DroppedBytes", Context = CONTEXT };

        private readonly HistogramOptions mDroppedPackets = new HistogramOptions() { Name = "DroppedPackets", Context = CONTEXT };

        private readonly HistogramOptions mTotalReceivedPacketHandlingTime = new HistogramOptions() { Name = "TotalReceivedPacketHandlingTime", Context = CONTEXT };

        private readonly HistogramOptions mTotalSentPacketHandlingTime = new HistogramOptions() { Name = "TotalSentPacketHandlingTime", Context = CONTEXT };

        public void UpdateClientCount(long count)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mClientCount, count);
        }
        public void UpdateBandwidth(long bytes)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mBandwidth, bytes);
        }

        public void UpdateSentBytes(long bytes)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mSentBytes, bytes);
        }

        public void UpdateSentPackets(long packets)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mSentPackets, packets);
        }

        public void UpdateReceivedBytes(long bytes)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mReceivedBytes, bytes);
        }

        public void UpdateReceivedPackets(long packets)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mReceivedPackets, packets);
        }

        public void UpdateAcceptedBytes(long bytes)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mAcceptedBytes, bytes);
        }

        public void UpdateAcceptedPackets(long packets)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mAcceptedPackets, packets);
        }

        public void UpdateDroppedBytes(long bytes)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mDroppedBytes, bytes);
        }

        public void UpdateDroppedPackets(long packets)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mDroppedPackets, packets);
        }

        public void UpdateTotalReceivedPacketHandlingTime(long time)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mTotalReceivedPacketHandlingTime, time);
        }

        public void UpdateTotalSentPacketHandlingTime(long time)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mTotalSentPacketHandlingTime, time);
        }
        public override IDictionary<string, object> Data(MetricsDataValueSource snapshot)
        {
            var res = base.Data(snapshot);
            var top10Sent = PacketSender.SentPacketTypes.Where(pair => pair.Value > 0).OrderByDescending(pair => pair.Value).Take(10).ToArray();
            var top10Received = PacketHandler.AcceptedPacketTypes.Where(pair => pair.Value > 0).OrderByDescending(pair => pair.Value).Take(10).ToArray();
            res.Add("MostSentPackets", top10Sent);
            res.Add("MostReceivedPackets", top10Received);
            return res;
        }
    }
}
