using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Intersect.Server.Web.Pages.Developer;

[Authorize(Policy = "Developer")]
public class DeveloperIndexModel : PageModel
{
    public static readonly DeveloperFeature[] AllFeatures = [
        new(
            "AssetManager",
            "/Developer/Assets",
            () => AssetManagerResources.AssetManager,
            context => context.IsUpdateServerEnabled()
        ),
        new(
            "ServerSettings",
            "/Developer/ServerSettings",
            () => "STUBServerSettings",
            context => false
        ),
    ];

    public IEnumerable<DeveloperFeature> EnabledFeatures =>
        AllFeatures.Where(feature => feature.EnablementProvider?.Invoke(HttpContext) ?? true);

    public void OnGet()
    {
    }
}