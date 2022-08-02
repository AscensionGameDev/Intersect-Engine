using Intersect.Localization;

using Newtonsoft.Json;

namespace Intersect.Editor.Localization.MenuBar;

internal partial class HelpMenuNamespace : MenuNamespace
{
    public HelpMenuNamespace() : base(@"Help") { }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString About = @"About";

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString Documentation = @"Documentation";

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString PostQuestion = @"Post Question";

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString ReportBug = @"Report Bug";
}
