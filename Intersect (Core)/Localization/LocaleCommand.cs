using JetBrains.Annotations;
using Newtonsoft.Json;
using System;

namespace Intersect.Localization
{
    [Serializable]
    public class LocaleCommand : LocaleDescribableToken
    {
        [JsonProperty(nameof(Help), NullValueHandling = NullValueHandling.Ignore)] [CanBeNull]
        private LocalizedString mHelp;

        [JsonProperty(nameof(Usage), NullValueHandling = NullValueHandling.Ignore)] [CanBeNull]
        private LocalizedString mUsage;

        [NotNull]
        [JsonIgnore]
        public LocalizedString Help
        {
            get => mHelp ?? "";
            set
            {
                if (mHelp == null)
                {
                    mHelp = value;
                }
            }
        }

        [NotNull]
        [JsonIgnore]
        public LocalizedString Usage
        {
            get => mUsage ?? "";
            set
            {
                if (mUsage == null)
                {
                    mUsage = value;
                }
            }
        }

        public LocaleCommand()
        {
        }

        public LocaleCommand(
            [NotNull] string name,
            [CanBeNull] string description = null,
            [CanBeNull] string help = null,
            [CanBeNull] string usage = null
        ) : base(name, description)
        {
            mHelp = help?.Trim();
            mUsage = usage?.Trim();
        }
    }
}