namespace Intersect.Config
{
    /// <summary>
    /// Contains configurable options pertaining to stat/metrics collecting
    /// </summary>
    public class MetricsOptions
    {
        /// <summary>
        /// Track game performance metrics
        /// </summary>
        public bool Enable { get; set; } = true;
    }
}
