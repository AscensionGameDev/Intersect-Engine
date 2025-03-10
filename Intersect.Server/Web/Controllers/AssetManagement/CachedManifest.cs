using Intersect.Framework.Core.AssetManagement;

namespace Intersect.Server.Web.Controllers.AssetManagement;

internal sealed record CachedManifest(UpdateManifest Manifest, DateTime Expiry)
{
    public bool IsExpired => DateTime.UtcNow >= Expiry;
}