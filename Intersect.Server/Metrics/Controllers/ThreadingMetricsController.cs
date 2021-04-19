using App.Metrics;
using App.Metrics.Histogram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Intersect.Server.Metrics.Controllers
{
    public class ThreadingMetricsController : MetricsController
    {
        private const string CONTEXT = "Threading";

        private int mMaxSystemThreadPoolThreads = 0;
        private int mMaxSystemThreadPoolIOThreads = 0;
        private int mMinSystemThreadPoolThreads = 0;
        private int mMinSystemThreadPoolIOThreads = 0;


        public ThreadingMetricsController(IMetricsRoot root)
        {
            Context = CONTEXT;
            mAppMetricsRoot = root;

            ThreadPool.GetMaxThreads(out mMaxSystemThreadPoolThreads, out mMaxSystemThreadPoolIOThreads);
            ThreadPool.GetMinThreads(out mMinSystemThreadPoolThreads, out mMinSystemThreadPoolIOThreads);
        }

        private HistogramOptions mLogicPoolActiveThreads => new HistogramOptions() { Name = "LogicPoolActiveThreads", Context = CONTEXT };

        private HistogramOptions mLogicPoolInUseThreads => new HistogramOptions() { Name = "LogicPoolInUseThreads", Context = CONTEXT };

        private HistogramOptions mLogicPoolWorkItemsCount => new HistogramOptions() { Name = "LogicPoolWorkItemsCount", Context = CONTEXT };

        private HistogramOptions mNetworkPoolActiveThreads => new HistogramOptions() { Name = "NetworkPoolActiveThreads", Context = CONTEXT };

        private HistogramOptions mNetworkPoolInUseThreads => new HistogramOptions() { Name = "NetworkPoolInUseThreads", Context = CONTEXT };

        private HistogramOptions mNetworkPoolWorkItemsCount => new HistogramOptions() { Name = "NetworkPoolWorkItemsCount", Context = CONTEXT };

        private HistogramOptions mSavingPoolActiveThreads => new HistogramOptions() { Name = "SavingPoolActiveThreads", Context = CONTEXT };

        private HistogramOptions mSavingPoolInUseThreads => new HistogramOptions() { Name = "SavingPoolInUseThreads", Context = CONTEXT };

        private HistogramOptions mSavingPoolWorkItemsCount => new HistogramOptions() { Name = "SavingPoolWorkItemsCount", Context = CONTEXT };

        private HistogramOptions mSystemPoolInUseWorkerThreads => new HistogramOptions() { Name = "SystemPoolInUseWorkerThreads", Context = CONTEXT };

        private HistogramOptions mSystemPoolInUseIOThreads => new HistogramOptions() { Name = "SystemPoolInUseIOThreads", Context = CONTEXT };

        public void UpdateLogicPoolActiveThreads(int threads)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mLogicPoolActiveThreads, threads);
        }

        public void UpdateLogicPoolInUseThreads(int threads)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mLogicPoolInUseThreads, threads);
        }

        public void UpdateLogicPoolWorkItemsCount(int count)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mLogicPoolWorkItemsCount, count);
        }

        public void UpdateNetworkPoolActiveThreads(int threads)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mNetworkPoolActiveThreads, threads);
        }

        public void UpdateNetworkPoolInUseThreads(int threads)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mNetworkPoolInUseThreads, threads);
        }

        public void UpdateNetworkPoolWorkItemsCount(int count)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mNetworkPoolWorkItemsCount, count);
        }

        public void UpdateSavingPoolActiveThreads(int threads)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mSavingPoolActiveThreads, threads);
        }

        public void UpdateSavingPoolInUseThreads(int threads)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mSavingPoolInUseThreads, threads);
        }

        public void UpdateSavingPoolWorkItemsCount(int count)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mSavingPoolWorkItemsCount, count);
        }

        public void UpdateSystemPoolInUseWorkerThreads(int threads)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mSystemPoolInUseWorkerThreads, threads);
        }

        public void UpdateSystemPoolInUseIOThreads(int threads)
        {
            if (Options.Instance.Metrics.Enable)
                mAppMetricsRoot.Measure.Histogram.Update(mSystemPoolInUseIOThreads, threads);
        }

        public override IDictionary<string, object> Data(MetricsDataValueSource snapshot)
        {
            var res = base.Data(snapshot);

            res.Add("LogicPoolMaxThreads", Options.Instance.Processing.MaxLogicThreads);
            res.Add("LogicPoolMinThreads", Options.Instance.Processing.MinLogicThreads);

            res.Add("NetworkPoolMaxThreads", Options.Instance.Processing.MaxNetworkThreads);
            res.Add("NetworkPoolMinThreads", Options.Instance.Processing.MinNetworkThreads);

            res.Add("SavingPoolMaxThreads", Options.Instance.Processing.MaxPlayerSaveThreads);
            res.Add("SavingPoolMinThreads", Options.Instance.Processing.MinPlayerSaveThreads);

            res.Add("SystemPoolDefaultMaxThreads", mMaxSystemThreadPoolThreads);
            res.Add("SystemPoolDefaultMaxIOThreads", mMaxSystemThreadPoolIOThreads);
            res.Add("SystemPoolDefaultMinThreads", mMinSystemThreadPoolThreads);
            res.Add("SystemPoolDefaultMinIOThreads", mMinSystemThreadPoolIOThreads);

            ThreadPool.GetMaxThreads(out int maxWorkerThreads, out int maxIOThreads);
            ThreadPool.GetMinThreads(out int minWorkerThreads, out int minIOThreads);

            res.Add("SystemPoolMaxThreads", maxWorkerThreads);
            res.Add("SystemPoolMinThreads", minWorkerThreads);
            res.Add("SystemPoolMaxIOThreads", maxIOThreads);
            res.Add("SystemPoolMinIOThreads", minIOThreads);

            return res;
        }
    }
}
