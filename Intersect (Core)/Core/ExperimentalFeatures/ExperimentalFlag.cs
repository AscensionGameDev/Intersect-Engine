using System;

using Intersect.Utilities;

using Newtonsoft.Json;

namespace Intersect.Core.ExperimentalFeatures
{

    public struct ExperimentalFlag : IExperimentalFlag
    {

        private readonly IExperimentalFlag mParentFlag;

        [JsonProperty(nameof(Guid))] private Guid mGuid;

        [JsonIgnore]
        public Guid Guid => mGuid;

        [JsonProperty(nameof(Name))] private string mName;

        [JsonIgnore]
        public string Name => mName;

        [JsonProperty(nameof(Enabled))] private bool mEnabled;

        [JsonIgnore]
        public bool Enabled => (mParentFlag?.Enabled ?? true) && mEnabled;

        private ExperimentalFlag(ExperimentalFlag flag, bool enabled = false)
        {
            mParentFlag = flag.mParentFlag;
            mGuid = flag.Guid;
            mName = flag.Name;
            mEnabled = enabled;
        }

        public ExperimentalFlag(
            string name,
            Guid namespaceId,
            bool enabled = false,
            IExperimentalFlag parentFlag = null
        )
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            mParentFlag = parentFlag;
            mGuid = GuidUtils.CreateNamed(namespaceId, name);
            mName = name;
            mEnabled = enabled;
        }

        public IExperimentalFlag With(bool enabled)
        {
            return new ExperimentalFlag(this, enabled);
        }

        /// <inheritdoc cref="IEquatable{T}" />
        public bool Equals(IExperimentalFlag other)
        {
            return this == other;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flag"></param>
        public static implicit operator bool(ExperimentalFlag flag)
        {
            return flag.Enabled;
        }

        public static bool operator ==(ExperimentalFlag a, IExperimentalFlag b)
        {
            return a.Guid == b.Guid && a.Enabled == b.Enabled;
        }

        public static bool operator !=(ExperimentalFlag a, IExperimentalFlag b)
        {
            return a.Guid != b.Guid || a.Enabled != b.Enabled;
        }

    }

}
