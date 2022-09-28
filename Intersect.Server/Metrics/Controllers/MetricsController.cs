using System.Collections.Generic;

namespace Intersect.Server.Metrics.Controllers
{
    public partial class MetricsController
    {
        public virtual string Context { get; set; }

        public virtual List<Histogram> Measurements { get; set; } = new List<Histogram>();

        public virtual void Clear()
        {
            for (var measurementIndex = 0; measurementIndex < Measurements.Count; measurementIndex++)
            {
                var histogram = Measurements[measurementIndex];
                histogram.Clear();
            }
        }

        public IDictionary<string, object> Data() => Data(0);

        protected virtual IDictionary<string, object> Data(int additionalCapacity)
        {
            var result = new Dictionary<string, object>(Measurements.Count + additionalCapacity);
            foreach (var hist in Measurements)
            {
                result.Add(hist.Name, hist.Snapshot);
            }
            return result;
        }
    }
}
