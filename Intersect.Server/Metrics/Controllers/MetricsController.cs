using App.Metrics;
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

        protected virtual IMetricsRoot mAppMetricsRoot { get; set; }

        public virtual object Data(MetricsDataValueSource snapshot)
        {
            var ctx = snapshot.Contexts.FirstOrDefault(c => c.Context == Context);
            var result = new ExpandoObject() as IDictionary<string, object>;
            if (ctx != null)
            {
                foreach (var hist in ctx.Histograms)
                {
                    result.Add(hist.Name.Split('|')[0], hist.Value);
                }
                foreach (var counter in ctx.Counters)
                {
                    result.Add(counter.Name.Split('|')[0], counter.Value);
                }
            }
            return result;
        }
    }
}
