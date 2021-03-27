using App.Metrics;
using App.Metrics.Histogram;

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

        private readonly HistogramOptions mReceivedPacketQueueTime = new HistogramOptions() { Name = "ReceivedPacketQueueTime", Context = CONTEXT };

        private readonly HistogramOptions mReceivedPacketProcessingTime = new HistogramOptions() { Name = "ReceivedPacketProcessingTime", Context = CONTEXT };

        private readonly HistogramOptions mTotalReceivedPacketHandlingTime = new HistogramOptions() { Name = "TotalReceivedPacketHandlingTime", Context = CONTEXT };

        private readonly HistogramOptions mSentPacketQueueTime = new HistogramOptions() { Name = "SentPacketQueueTime", Context = CONTEXT };

        private readonly HistogramOptions mSentPacketProcessingTime = new HistogramOptions() { Name = "SentPacketProcessingTime", Context = CONTEXT };

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

        public void UpdateReceivedPacketQueueTime(long time)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mReceivedPacketQueueTime, time);
        }

        public void UpdateReceivedPacketProcessingTime(long time)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mReceivedPacketProcessingTime, time);
        }

        public void UpdateTotalReceivedPacketHandlingTime(long time)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mTotalReceivedPacketHandlingTime, time);
        }

        public void UpdateSentPacketQueueTime(long time)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mSentPacketQueueTime, time);
        }

        public void UpdateSentPacketProcessingTime(long time)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mSentPacketProcessingTime, time);
        }

        public void UpdateTotalSentPacketHandlingTime(long time)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mTotalSentPacketHandlingTime, time);
        }
    }
}
