using Newtonsoft.Json;

namespace Intersect.Server.Web.Configuration;

public sealed record StaticFilePathOptions(string SourcePath, string? RequestPath = default)
{
    [JsonProperty]
    public string SourcePath { get; init; } = SourcePath;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? RequestPath { get; init; } = RequestPath;

    public void Deconstruct(out string SourcePath, out string? RequestPath)
    {
        SourcePath = this.SourcePath;
        RequestPath = this.RequestPath;
    }
}