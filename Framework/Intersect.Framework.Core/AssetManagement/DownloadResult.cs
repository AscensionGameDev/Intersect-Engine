namespace Intersect.Framework.Core.AssetManagement;

internal sealed class DownloadResult
{
    public long Count { get; set; }

    public string? OverrideBaseUrl { get; init; }

    public DownloadState State { get; set; }
}