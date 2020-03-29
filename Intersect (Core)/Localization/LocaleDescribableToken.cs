using System;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace Intersect.Localization
{

    [Serializable]
    public class LocaleDescribableToken : LocaleToken
    {

        [JsonProperty(nameof(Description), NullValueHandling = NullValueHandling.Ignore)]
        private LocalizedString mDescription;

        public LocaleDescribableToken()
        {
        }

        public LocaleDescribableToken([NotNull] string name, [CanBeNull] string description = null) : base(name)
        {
            mDescription = description?.Trim();
        }

        [NotNull]
        [JsonIgnore]
        public virtual LocalizedString Description
        {
            get => mDescription ?? "";
            set
            {
                if (mDescription == null)
                {
                    mDescription = value;
                }
            }
        }

    }

}
