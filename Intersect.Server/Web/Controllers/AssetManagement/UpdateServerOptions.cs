namespace Intersect.Server.Web.Controllers.AssetManagement;

public sealed partial class UpdateServerOptions
{
#if DEBUG
    public static readonly TimeSpan DefaultManifestCacheExpiry = TimeSpan.FromMinutes(1);
#else
    public static readonly TimeSpan DefaultManifestCacheExpiry = TimeSpan.FromMinutes(15);
#endif

    public bool Enabled { get; set; }

    public string AssetRoot { get; set; } = "assets";

    public TimeSpan? AutoRefreshInterval { get; set; } = TimeSpan.FromSeconds(5);

    public TimeSpan? ManifestCacheExpiry { get; set; } = DefaultManifestCacheExpiry;
}