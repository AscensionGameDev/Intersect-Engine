using App.Metrics;
using App.Metrics.Histogram;
using App.Metrics.ReservoirSampling.SlidingWindow;
using App.Metrics.Scheduling;
using Intersect.Server.General;
using Intersect.Server.Metrics.Controllers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace Intersect.Server.Metrics
{
    public class MetricsRoot
    {

        private static readonly IMetricsRoot mAppMetricsRoot = AppMetrics.CreateDefaultBuilder().Build();

        public GameMetricsController Game => new GameMetricsController(mAppMetricsRoot);

        public ApplicationMetricsController Application => new ApplicationMetricsController(mAppMetricsRoot);

        public NetworkMetricsController Network => new NetworkMetricsController(mAppMetricsRoot);

        public JObject Metrics => mLatestSnapshot;

        private JObject mLatestSnapshot = new JObject();

        /// <summary>
        /// Creates and configures metric tracking for our various controllers
        /// </summary>
        public MetricsRoot()
        {
            if (Instance == null)
                Instance = this;
        }

        /// <summary>
        /// Serverwide metrics collecting instance
        /// </summary>
        public static MetricsRoot Instance { get; private set; }

        /// <summary>
        /// Setup our metric counters/histograms if metrics are enabled in the server config
        /// </summary>
        public static void Init()
        {
            Instance = new MetricsRoot();
        }

        public void Capture()
        {
            mLatestSnapshot = JObject.FromObject(Data(mAppMetricsRoot.Snapshot.Get()));
            mAppMetricsRoot.Manage.Reset();
        }

        private object Data(MetricsDataValueSource snapshot)
        {
            var result = new ExpandoObject() as IDictionary<string, object>;
            result.Add(Game.Context, Game.Data(snapshot));
            result.Add(Application.Context, Application.Data(snapshot));
            result.Add(Network.Context, Network.Data(snapshot));
            return new
            {
                uptime = Globals.Timing.Milliseconds,
                datetime = DateTime.UtcNow,
                metrics = result
            };
        }
    }
}
