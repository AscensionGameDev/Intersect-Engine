namespace Intersect.Framework.Core.AssetManagement;

internal enum DownloadValidity
{
    Valid,
    Missing,
    LengthMismatch,
    ChecksumMismatch,
}