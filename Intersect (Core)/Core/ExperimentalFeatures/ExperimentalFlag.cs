using System;

using Intersect.Utilities;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace Intersect.Core.ExperimentalFeatures
{
    public interface IFlag {
        bool Enabled { get; }
    }

    public struct ExperimentalFlag : IEquatable<ExperimentalFlag>, IFlag
    {
        private readonly IFlag mParentFlag;
        
        [JsonProperty(nameof(Guid))] private Guid mGuid;
        [JsonIgnore] public Guid Guid => mGuid;
        
        [JsonProperty(nameof(Name)), NotNull] private string mName;
        [JsonIgnore, NotNull] public string Name => mName;

        [JsonProperty(nameof(Enabled))] private bool mEnabled;
        [JsonIgnore] public bool Enabled => (mParentFlag?.Enabled ?? true) && mEnabled;

        private ExperimentalFlag(ExperimentalFlag flag, bool enabled = false)
        {
            mParentFlag = flag.mParentFlag;
            mGuid = flag.Guid;
            mName = flag.Name;
            mEnabled = enabled;
        }

        public ExperimentalFlag([NotNull] string name, Guid namespaceId, bool enabled = false, IFlag parentFlag = null)
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

        /// <summary>
        /// Creates a clone of this <see cref="ExperimentalFlag"/> with the given enablement.
        /// </summary>
        /// <param name="enabled">the new enablement state</param>
        /// <returns>a clone of this flag with the new enablement state</returns>
        public ExperimentalFlag With(bool enabled) => new ExperimentalFlag(this, enabled);

        /// <inheritdoc />
        public bool Equals(ExperimentalFlag other) => this == other;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flag"></param>
        public static implicit operator bool(ExperimentalFlag flag) => flag.Enabled;

        public static bool operator ==(ExperimentalFlag a, ExperimentalFlag b)
        {
            return a.Guid == b.Guid && a.Enabled == b.Enabled;
        }

        public static bool operator !=(ExperimentalFlag a, ExperimentalFlag b)
        {
            return a.Guid != b.Guid || a.Enabled != b.Enabled;
        }
    }
}
