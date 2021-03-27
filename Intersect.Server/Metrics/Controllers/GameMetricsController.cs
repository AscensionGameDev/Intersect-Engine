using App.Metrics;
using App.Metrics.Histogram;

namespace Intersect.Server.Metrics.Controllers
{
    public class GameMetricsController : MetricsController
    {
        private const string CONTEXT = "Game";

        public GameMetricsController(IMetricsRoot root)
        {
            Context = CONTEXT;
            mAppMetricsRoot = root;
        }

        private readonly HistogramOptions mCps = new HistogramOptions() { Name = "Cps", Context = CONTEXT };

        private readonly HistogramOptions mPlayerCount = new HistogramOptions() { Name = "Players", Context = CONTEXT };

        private readonly HistogramOptions mActiveMapCount = new HistogramOptions() { Name = "ActiveMaps", Context = CONTEXT };

        private readonly HistogramOptions mActiveEntityCount = new HistogramOptions() { Name = "ActiveEntities", Context = CONTEXT };

        private readonly HistogramOptions mActiveEventCount = new HistogramOptions() { Name = "ActiveEvents", Context = CONTEXT };

        private readonly HistogramOptions mAutorunEventCount = new HistogramOptions() { Name = "AutorunEvents", Context = CONTEXT };

        private readonly HistogramOptions mProcessingEventsCount = new HistogramOptions() { Name = "ProcessingEvents", Context = CONTEXT };

        private readonly HistogramOptions mMapQueueUpdateOffset = new HistogramOptions() { Name = "MapQueueUpdateOffset", Context = CONTEXT };

        private readonly HistogramOptions mMapUpdateQueuedTime = new HistogramOptions() { Name = "MapUpdateQueuedTime", Context = CONTEXT };

        private readonly HistogramOptions mMapUpdateProcessingTime = new HistogramOptions() { Name = "MapUpdateProcessingTime", Context = CONTEXT };

        private readonly HistogramOptions mMapTotalUpdateTime = new HistogramOptions() { Name = "MapTotalUpdateTime", Context = CONTEXT };

        public void UpdateCps(long cps)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mCps, cps);
        }

        public void UpdatePlayerCount(long players)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mPlayerCount, players);
        }

        public void UpdateActiveMapCount(long activeMaps)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mActiveMapCount, activeMaps);
        }

        public void UpdateActiveEntityCount(long activeEntities)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mActiveEntityCount, activeEntities);
        }

        public void UpdateActiveEventCount(long activeEvents)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mActiveEventCount, activeEvents);
        }

        public void UpdateAutorunEventCount(long autorunEvents)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mAutorunEventCount, autorunEvents);
        }

        public void UpdateProcessingEventsCount(long processingEvents)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mProcessingEventsCount, processingEvents);
        }

        public void UpdateMapQueueUpdateOffset(long offset)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mMapQueueUpdateOffset, offset);
        }

        public void UpdateMapUpdateQueuedTime(long time)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mMapUpdateQueuedTime, time);
        }

        public void UpdateMapUpdateProcessingTime(long time)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mMapUpdateProcessingTime, time);
        }

        public void UpdateMapTotalUpdateTime(long time)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mMapTotalUpdateTime, time);
        }
    }
}
