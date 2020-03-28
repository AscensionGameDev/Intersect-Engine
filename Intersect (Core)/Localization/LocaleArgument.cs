using System;
using System.ComponentModel;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace Intersect.Localization
{

    [Serializable]
    public class LocaleArgument : LocaleDescribableToken
    {

        [JsonProperty(
            nameof(ShortName), NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        )]
        [DefaultValue(char.MinValue)]
        private char mShortName;

        public LocaleArgument()
        {
        }

        public LocaleArgument(
            [NotNull] string name,
            char shortName = char.MinValue,
            [CanBeNull] string description = null
        ) : base(name, description)
        {
            mShortName = shortName;
        }

        [JsonIgnore]
        public virtual char ShortName
        {
            get => mShortName;
            set
            {
                if (mShortName == char.MinValue)
                {
                    mShortName = value;
                }
            }
        }

    }

}
