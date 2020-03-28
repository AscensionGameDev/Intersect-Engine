using Newtonsoft.Json;

namespace Intersect.Core.ExperimentalFeatures
{

    public abstract partial class CommonExperiments<TExperiments> where TExperiments : CommonExperiments<TExperiments>
    {

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public IExperimentalFlag All { get; protected set; } = new ExperimentalFlag(nameof(All), NamespaceId);

        [JsonIgnore, ExperimentalFlagAlias(nameof(All))]
        public IExperimentalFlag ExperimentalFeatures { get; protected set; }

    }

}
