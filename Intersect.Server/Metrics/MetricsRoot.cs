using System;
using System.Collections.Generic;

using Intersect.Server.Metrics.Controllers;
using Intersect.Utilities;

using Newtonsoft.Json;

namespace Intersect.Server.Metrics
{
    public partial class MetricsRoot
    {
        private const string EmptySnapshot = "{}";

        private List<MetricsController> MetricsControllers = new List<MetricsController>();

        public GameMetricsController Game { get; private set; } = new GameMetricsController();

        public ApplicationMetricsController Application { get; private set; } = new ApplicationMetricsController();

        public NetworkMetricsController Network { get; private set; } = new NetworkMetricsController();

        public ThreadingMetricsController Threading { get; private set; } = new ThreadingMetricsController();

        public string Metrics => mLatestSnapshot.HasValue ? JsonConvert.SerializeObject(mLatestSnapshot.Value) : EmptySnapshot;

        private MetricsSnapshot? mLatestSnapshot;

        /// <summary>
        /// Creates and configures metric tracking for our various controllers
        /// </summary>
        public MetricsRoot()
        {
            if (Instance == default)
            {
                Instance = this;
            }

            MetricsControllers.Add(Application);
            MetricsControllers.Add(Game);
            MetricsControllers.Add(Network);
            MetricsControllers.Add(Threading);
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

        /// <summary>
        /// Clears our cached metrics dump so that the api returns an empty object while metrics are disabled.
        /// </summary>
        public void Disable()
        {
            mLatestSnapshot = default;
        }

        public void Capture()
        {
            mLatestSnapshot = Data();
            Clear();
        }

        public void Clear()
        {
            foreach (var metricsController in MetricsControllers)
            {
                metricsController.Clear();
            }
        }

        private MetricsSnapshot Data()
        {
            var result = new Dictionary<string, IDictionary<string, object>>(MetricsControllers.Count);
            for (var controllerIndex = 0; controllerIndex < MetricsControllers.Count; controllerIndex++)
            {
                var controller = MetricsControllers[controllerIndex];
                result.Add(controller.Context, controller.Data());
            }
            return new MetricsSnapshot(DateTime.UtcNow, result, Timing.Global.Milliseconds);
        }

        private struct MetricsSnapshot
        {
            [JsonProperty("datetime", Order = 2)]
            public readonly DateTime DateTime;
            [JsonProperty("metrics", Order = 3)]
            public readonly IDictionary<string, IDictionary<string, object>> Metrics;
            [JsonProperty("uptime", Order = 1)]
            public readonly long Uptime;

            public MetricsSnapshot(DateTime dateTime, IDictionary<string, IDictionary<string, object>> metrics, long uptime)
            {
                DateTime = dateTime;
                Metrics = metrics;
                Uptime = uptime;
            }
        }
    }
}
