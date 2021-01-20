using System;

using Newtonsoft.Json;

namespace Intersect.Localization
{

    [Serializable]
    public class LocaleCommand : LocaleDescribableToken
    {

        [JsonProperty(nameof(Help), NullValueHandling = NullValueHandling.Ignore)]        private LocalizedString mHelp;

        public LocaleCommand()
        {
        }

        public LocaleCommand(
            string name,
            string description = null,
            string help = null
        ) : base(name, description)
        {
            mHelp = help?.Trim();
        }

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

    }

}
