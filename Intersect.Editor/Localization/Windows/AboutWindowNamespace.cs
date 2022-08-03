using Intersect.Localization;

using Newtonsoft.Json;

namespace Intersect.Editor.Localization.Windows;

internal partial class AboutWindowNamespace : LocaleNamespace
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString Title = @"About";

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString JoinUsOnGitHub = @"Join us on GitHub!";

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString LabelAuthors = @"Authors";

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString LabelContributors = @"Contributors";

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString LabelDevelopers = @"Developers";

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString LabelMaintainers = @"Maintainers";

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString LabelLicenses = @"Licenses";

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString LabelLicenseComponentIntersectEditor = @"Intersect Editor";

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString LabelLicenseGroupIntersect = @"Intersect Engine";
}
