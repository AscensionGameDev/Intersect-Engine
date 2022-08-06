using Intersect.Localization;

using Newtonsoft.Json;

namespace Intersect.Editor.Localization.Windows;

internal partial class LocalizationWindowNamespace : LocaleNamespace
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString Title = @"Localization";

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString ColumnComment = @"Comment";

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString ColumnCommentHint = @"A comment that describes the content string.";

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString ColumnDescriptorType = @"Type";

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString ColumnId = @"ID";

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString ColumnLocaleLong = @"{01} ({00})";

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString ColumnLocaleShort = @"{00}";

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString ColumnLocaleHint = @"";
}
