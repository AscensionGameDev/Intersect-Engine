using System.Collections.Generic;

namespace Intersect.Server.Metrics.Controllers
{
    public abstract partial class MetricsController
    {
        private int _allocations;

        protected MetricsController(string context)
        {
            Context = context;
        }

        public string Context { get; }

        public List<Histogram> Measurements { get; } = new List<Histogram>();

        public virtual void Clear()
        {
            for (var i = 0; i < Measurements.Count; i++)
            {
                Measurements[i].Clear();
            }
        }

        public IDictionary<string, object> Data() {
            var data = InternalData();
            _allocations = data.Count;
            return data;
        }

        protected virtual IDictionary<string, object> InternalData()
        {
            var data = new Dictionary<string, object>(_allocations);
            for (var i = 0; i < Measurements.Count; i++)
            {
                var histogram = Measurements[i];
                data[histogram.Name] = histogram;
            }
            return data;
        }
    }
}
