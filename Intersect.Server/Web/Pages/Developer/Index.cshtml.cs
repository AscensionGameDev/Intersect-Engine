using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Intersect.Server.Web.Pages.Developer;

[Authorize(Policy = "Developer")]
public class DeveloperIndexModel : PageModel
{
    public static readonly DeveloperFeature[] AllFeatures = [];

    public IEnumerable<DeveloperFeature> EnabledFeatures =>
        AllFeatures.Where(feature => feature.EnablementProvider?.Invoke(HttpContext) ?? true);

    public void OnGet()
    {
    }
}