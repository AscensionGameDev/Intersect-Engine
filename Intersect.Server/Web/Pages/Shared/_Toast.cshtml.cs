using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Intersect.Server.Web.Pages.Shared;

public partial class ToastModel : PageModel
{
    public const string TypeError = "error";
    public const string TypeInfo = "info";
    public const string TypeSuccess = "success";
    public const string TypeWarning = "warning";

    public string Message { get; set; }

    public string Type { get; set; }
}