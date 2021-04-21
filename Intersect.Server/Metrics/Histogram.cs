using Intersect.Server.Metrics.Controllers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Intersect.Server.Metrics
{
    public class Histogram
    {
        [JsonIgnore]
        public string Name { get; private set; }

        public long Min { get; private set; }

        public long Max { get; private set; }

        public long Count { get; private set; }

        public long Sum { get; private set; }

        public double Mean => Count > 0 ? Sum / (double)Count : 0;

        public Histogram(string name, MetricsController controller)
        {
            Name = name;
            controller.Measurements.Add(this);
        }

        public void Record(long val)
        {
            if (val < Min || Count == 0)
            {
                Min = val;
            }

            if (val > Max || Count == 0)
            {
                Max = val;
            }
            Sum += val;
            Count++;
        }

        public void Clear()
        {
            Min = 0;
            Max = 0;
            Sum = 0;
            Count = 0;
        }
    }
}
