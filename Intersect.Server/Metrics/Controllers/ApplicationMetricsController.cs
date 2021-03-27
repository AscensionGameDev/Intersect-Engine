using App.Metrics;
using App.Metrics.Histogram;

namespace Intersect.Server.Metrics.Controllers
{
    public class ApplicationMetricsController : MetricsController
    {
        private const string CONTEXT = "Application";

        public ApplicationMetricsController(IMetricsRoot root)
        {
            Context = CONTEXT;
            mAppMetricsRoot = root;
        }

        private HistogramOptions mCpuUsage => new HistogramOptions() { Name = "Cpu", Context = CONTEXT };

        private HistogramOptions mMemoryUsage => new HistogramOptions() { Name = "Memory", Context = CONTEXT };

        public void UpdateCpuUsage(double cpu)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mCpuUsage, (long)cpu);
        }

        public void UpdateMemoryUsage(long memory)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mMemoryUsage, memory);
        }
    }
}
