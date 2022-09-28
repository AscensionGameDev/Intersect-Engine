using Intersect.Server.Metrics.Controllers;

namespace Intersect.Server.Metrics
{
    public struct HistogramData
    {
        public long Count;
        public long Min;
        public long Max;
        public long Sum;

        public double Mean => Count > 0 ? Sum / (double)Count : 0;
    }

    public partial class Histogram
    {
        public string Name { get; private set; }

        private HistogramData Data;

        public Histogram(string name, MetricsController controller)
        {
            Name = name;
            controller.Measurements.Add(this);
        }

        public void Record(long val)
        {
            if (val < Data.Min || Data.Count == 0)
            {
                Data.Min = val;
            }

            if (val > Data.Max || Data.Count == 0)
            {
                Data.Max = val;
            }
            Data.Sum += val;
            Data.Count++;
        }

        public void Clear()
        {
            Data.Min = 0;
            Data.Max = 0;
            Data.Sum = 0;
            Data.Count = 0;
        }

        public HistogramData Snapshot => Data;
    }
}
