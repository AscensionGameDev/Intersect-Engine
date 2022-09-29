using System;
using System.Collections.Generic;

using Intersect.Server.Metrics.Controllers;
using Intersect.Utilities;

using Newtonsoft.Json;

namespace Intersect.Server.Metrics
{
    public partial class MetricsRoot
    {
        private const string EmptyJson = "{}";

        private readonly List<MetricsController> MetricsControllers;

        public GameMetricsController Game { get; private set; } = new GameMetricsController();

        public ApplicationMetricsController Application { get; private set; } = new ApplicationMetricsController();

        public NetworkMetricsController Network { get; private set; } = new NetworkMetricsController();

        public ThreadingMetricsController Threading { get; private set; } = new ThreadingMetricsController();

        public string Metrics => mLatestSnapshot == default ? EmptyJson : JsonConvert.SerializeObject(mLatestSnapshot);

        private IDictionary<string, object> mLatestSnapshot;

        /// <summary>
        /// Creates and configures metric tracking for our various controllers
        /// </summary>
        private MetricsRoot()
        {
            MetricsControllers = new List<MetricsController>(4)
            {
                Application,
                Game,
                Network,
                Threading
            };
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
            for (var i = 0; i < MetricsControllers.Count; i++)
            {
                var controller = MetricsControllers[i];
                controller.Clear();
            }
        }

        private IDictionary<string, object> Data()
        {
            var result = new Dictionary<string, object>(MetricsControllers.Count);
            for (var i = 0; i < MetricsControllers.Count; i++)
            {
                var controller = MetricsControllers[i];
                result[controller.Context] = controller.Data();
            }

            var data = new Dictionary<string, object>(3)
            {
                ["uptime"] = Timing.Global.Milliseconds,
                ["datetime"] = DateTime.UtcNow,
                ["metrics"] = result
            };

            return data;
        }
    }
}
