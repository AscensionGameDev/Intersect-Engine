using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Server.Metrics.Controllers
{
    public class MetricsController
    {
        public virtual string Context { get; set; }

        public virtual List<Histogram> Measurements { get; set; } = new List<Histogram>();

        public virtual void Clear()
        {
            Measurements.ForEach(m => m.Clear());
        }

        public virtual IDictionary<string, object> Data()
        {
            var result = new ExpandoObject() as IDictionary<string, object>;
            foreach (var hist in Measurements)
            {
                result.Add(hist.Name, hist);
            }
            return result;
        }
    }
}
