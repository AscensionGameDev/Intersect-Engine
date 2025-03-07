namespace Intersect.Framework.Core.AssetManagement;

public sealed partial class UpdateManifest
{
    public List<UpdateManifestFile> Files { get; set; } = [];

    [Newtonsoft.Json.JsonProperty(DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]
    [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault)]
    public string? OverrideBaseUrl { get; set; }

    public long StreamingSizeCutoff { get; set; } = Updater.MaxBuffer;

    [Newtonsoft.Json.JsonProperty(DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]
    [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault)]
    public string? StreamingUrl { get; set; }

    public long TotalSize { get; set; }

    public bool TrustCache { get; set; }
}