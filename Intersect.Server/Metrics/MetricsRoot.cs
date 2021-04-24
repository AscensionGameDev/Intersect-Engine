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

        private List<MetricsController> MetricsControllers = new List<MetricsController>();

        public GameMetricsController Game { get; private set; } = new GameMetricsController();

        public ApplicationMetricsController Application { get; private set; } = new ApplicationMetricsController();

        public NetworkMetricsController Network { get; private set; } = new NetworkMetricsController();

        public ThreadingMetricsController Threading { get; private set; } = new ThreadingMetricsController();

        public string Metrics => mLatestSnapshot;

        private string mLatestSnapshot = "{}";

        /// <summary>
        /// Creates and configures metric tracking for our various controllers
        /// </summary>
        public MetricsRoot()
        {
            if (Instance == null)
                Instance = this;

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
            mLatestSnapshot = "{}";
        }

        public void Capture()
        {
            var data = Data();
            mLatestSnapshot = JsonConvert.SerializeObject(data);
            Clear();
        }

        public void Clear()
        {
            MetricsControllers.ForEach(m => m.Clear());
        }

        private object Data()
        {
            var result = new ExpandoObject() as IDictionary<string, object>;
            foreach (var controller in MetricsControllers)
            {
                result.Add(controller.Context, controller.Data());
            }
            return new
            {
                uptime = Globals.Timing.Milliseconds,
                datetime = DateTime.UtcNow,
                metrics = result
            };
        }
    }
}
