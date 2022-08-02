using Intersect.Localization.Common.Descriptors;

using Newtonsoft.Json;

namespace Intersect.Localization.Common;

public abstract partial class RootNamespace : AbstractRootNamespace
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly CommonApplicationNamespace Application = new();

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly DescriptorsNamespace Descriptors = new();

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly CommonGeneralNamespace General = new();
    
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly CommonLicensingNamespace Licensing = new();
}
