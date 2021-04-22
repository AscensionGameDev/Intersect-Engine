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

        public Histogram LogicPoolActiveThreads { get; private set; }

        public Histogram LogicPoolInUseThreads { get; private set; }

        public Histogram LogicPoolWorkItemsCount { get; private set; }

        public Histogram NetworkPoolActiveThreads { get; private set; }

        public Histogram NetworkPoolInUseThreads { get; private set; }

        public Histogram NetworkPoolWorkItemsCount { get; private set; }

        public Histogram DatabasePoolActiveThreads { get; private set; }

        public Histogram DatabasePoolInUseThreads { get; private set; }

        public Histogram DatabasePoolWorkItemsCount { get; private set; }

        public Histogram SystemPoolInUseWorkerThreads { get; private set; }

        public Histogram SystemPoolInUseIOThreads { get; private set; }


        public ThreadingMetricsController()
        {
            Context = CONTEXT;

            ThreadPool.GetMaxThreads(out mMaxSystemThreadPoolThreads, out mMaxSystemThreadPoolIOThreads);
            ThreadPool.GetMinThreads(out mMinSystemThreadPoolThreads, out mMinSystemThreadPoolIOThreads);

            LogicPoolActiveThreads = new Histogram(nameof(LogicPoolActiveThreads), this);
            LogicPoolInUseThreads = new Histogram(nameof(LogicPoolInUseThreads), this);
            LogicPoolWorkItemsCount = new Histogram(nameof(LogicPoolWorkItemsCount), this);
            NetworkPoolActiveThreads = new Histogram(nameof(NetworkPoolActiveThreads), this);
            NetworkPoolInUseThreads = new Histogram(nameof(NetworkPoolInUseThreads), this);
            NetworkPoolWorkItemsCount = new Histogram(nameof(NetworkPoolWorkItemsCount), this);
            DatabasePoolActiveThreads = new Histogram(nameof(DatabasePoolActiveThreads), this);
            DatabasePoolInUseThreads = new Histogram(nameof(DatabasePoolInUseThreads), this);
            DatabasePoolWorkItemsCount = new Histogram(nameof(DatabasePoolWorkItemsCount), this);
            SystemPoolInUseWorkerThreads = new Histogram(nameof(SystemPoolInUseWorkerThreads), this);
            SystemPoolInUseIOThreads = new Histogram(nameof(SystemPoolInUseIOThreads), this);
        }


        public override IDictionary<string, object> Data()
        {
            var res = base.Data();

            res.Add("LogicPoolMaxThreads", Options.Instance.Processing.MaxLogicThreads);
            res.Add("LogicPoolMinThreads", Options.Instance.Processing.MinLogicThreads);

            res.Add("NetworkPoolMaxThreads", Options.Instance.Processing.MaxNetworkThreads);
            res.Add("NetworkPoolMinThreads", Options.Instance.Processing.MinNetworkThreads);

            res.Add("DatabasePoolMaxThreads", Options.Instance.Processing.MaxDatabaseThreads);
            res.Add("DatabasePoolMinThreads", Options.Instance.Processing.MinDatabaseThreads);

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
