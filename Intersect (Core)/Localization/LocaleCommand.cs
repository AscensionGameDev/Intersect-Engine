using JetBrains.Annotations;
using Newtonsoft.Json;
using System;

namespace Intersect.Localization
{
    [Serializable]
    public class LocaleCommand : LocaleNamespace
    {
        [JsonProperty(nameof(Name), NullValueHandling = NullValueHandling.Ignore)]
        private LocalizedString mName;

        [JsonProperty(nameof(Description), NullValueHandling = NullValueHandling.Ignore)]
        private LocalizedString mDescription;

        [JsonProperty(nameof(Help), NullValueHandling = NullValueHandling.Ignore)]
        private LocalizedString mHelp;

        [JsonProperty(nameof(Usage), NullValueHandling = NullValueHandling.Ignore)]
        private LocalizedString mUsage;

        [NotNull]
        [JsonIgnore]
        public LocalizedString Name
        {
            get => mName ?? throw new InvalidOperationException();
            set
            {
                if (mName == null)
                {
                    mName = value;
                }
            }
        }

        [NotNull]
        [JsonIgnore]
        public LocalizedString Description
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
        )
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($@"Parameter '{nameof(name)}' cannot be null or whitespace.");
            }

            Name = name.Trim();
            Description = description?.Trim() ?? "";
            Help = help?.Trim() ?? "";
            Usage = usage?.Trim() ?? "";
        }
    }
}