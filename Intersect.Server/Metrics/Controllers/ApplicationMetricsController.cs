namespace Intersect.Server.Metrics.Controllers
{
    public class ApplicationMetricsController : MetricsController
    {
        private const string CONTEXT = "Application";

        public Histogram Cpu { get; private set; }

        public Histogram Memory { get; private set; }

        public ApplicationMetricsController()
        {
            Context = CONTEXT;

            Cpu = new Histogram(nameof(Cpu), this);
            Memory = new Histogram(nameof(Memory), this);
        }
    }
}
