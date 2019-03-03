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

        public LocaleCommand()
        {
        }

        public LocaleCommand(
            [NotNull] string name,
            [CanBeNull] string description = null,
            [CanBeNull] string help = null
        ) : base(name, description)
        {
            mHelp = help?.Trim();
        }
    }
}