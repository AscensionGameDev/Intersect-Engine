using System;

using Intersect.Core.ExperimentalFeatures;

using Newtonsoft.Json;

namespace Intersect.Server.Core.ExperimentalFeatures
{

    public sealed class Experiments : CommonExperiments<Experiments>
    {

        private static readonly Guid NamespaceId = Guid.Parse("4a6db511-7d5c-4a23-a096-5d61baa58cd7");

        static Experiments()
        {
            Instance = new Experiments();
            Instance.Load();
        }

        private Experiments()
        {
            PostgreSQL = new ExperimentalFlag(nameof(PostgreSQL), NamespaceId, parentFlag: All);
        }

        public static Experiments Instance
        {
            get => CommonExperiments<Experiments>.Instance;
            private set => CommonExperiments<Experiments>.Instance = value;
        }

        [JsonProperty(
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
            ObjectCreationHandling = ObjectCreationHandling.Reuse
        )]
        public ExperimentalFlag PostgreSQL { get; private set; }

    }

}
