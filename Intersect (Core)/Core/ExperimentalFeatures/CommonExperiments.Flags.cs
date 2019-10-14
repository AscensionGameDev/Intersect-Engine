
using Newtonsoft.Json;

namespace Intersect.Core.ExperimentalFeatures
{
    public abstract partial class CommonExperiments<TExperiments> where TExperiments : CommonExperiments<TExperiments>
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public ExperimentalFlag All { get; protected set; } = new ExperimentalFlag(nameof(All), NamespaceId);
    }
}
