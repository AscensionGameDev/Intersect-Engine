namespace Intersect.Framework.Core.AssetManagement;

public enum UpdateStatus
{
    None,
    Error,
    DownloadingManifest,
    UpdateInProgress,
    UpdateCompleted,
    Restart,
}