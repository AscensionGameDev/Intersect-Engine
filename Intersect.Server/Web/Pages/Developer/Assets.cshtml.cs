using Intersect.Server.Web.Controllers.AssetManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Intersect.Server.Web.Pages.Developer;

[Authorize(Policy = "Developer")]
public class AssetManagerModel : PageModel
{
    internal UpdateServerOptions UpdateServerOptions => HttpContext.GetUpdateServerOptions();

    public void OnGet()
    {
        if (!UpdateServerOptions.Enabled)
        {
            Response.Redirect("/");
        }
    }
}