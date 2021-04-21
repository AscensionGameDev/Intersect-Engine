namespace Intersect.Server.Metrics.Controllers
{
    public class GameMetricsController : MetricsController
    {
        private const string CONTEXT = "Game";

        public Histogram Cps { get; private set; }

        public Histogram Players { get; private set; }

        public Histogram ActiveMaps { get; private set; }

        public Histogram ActiveEntities { get; private set; }

        public Histogram ActiveEvents { get; private set; }

        public Histogram AutorunEvents { get; private set; }

        public Histogram ProcessingEvents { get; private set; }

        public Histogram MapQueueUpdateOffset { get; private set; }

        public Histogram MapUpdateQueuedTime { get; private set; }

        public Histogram MapUpdateProcessingTime { get; private set; }

        public Histogram MapTotalUpdateTime { get; private set; }

        public GameMetricsController()
        {
            Context = CONTEXT;

            Cps = new Histogram(nameof(Cps), this);
            Players = new Histogram(nameof(Players), this);
            ActiveMaps = new Histogram(nameof(ActiveMaps), this);
            ActiveEntities = new Histogram(nameof(ActiveEntities), this);
            ActiveEvents = new Histogram(nameof(ActiveEvents), this);
            AutorunEvents = new Histogram(nameof(AutorunEvents), this);
            ProcessingEvents = new Histogram(nameof(ProcessingEvents), this);
            MapQueueUpdateOffset = new Histogram(nameof(MapQueueUpdateOffset), this);
            MapUpdateQueuedTime = new Histogram(nameof(MapUpdateQueuedTime), this);
            MapUpdateProcessingTime = new Histogram(nameof(MapUpdateProcessingTime), this);
            MapTotalUpdateTime = new Histogram(nameof(MapTotalUpdateTime), this);
        }
    }
}
