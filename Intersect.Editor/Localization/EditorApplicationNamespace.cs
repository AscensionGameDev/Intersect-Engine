
using Intersect.Localization;
using Intersect.Localization.Common;

using Newtonsoft.Json;

namespace Intersect.Editor.Localization;

internal partial class EditorApplicationNamespace : CommonApplicationNamespace
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public new readonly LocalizedString Name = @"Intersect Editor";
}
