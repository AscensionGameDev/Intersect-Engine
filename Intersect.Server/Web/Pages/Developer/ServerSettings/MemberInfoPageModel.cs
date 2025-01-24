using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Intersect.Server.Web.Pages.Developer.ServerSettings;

public abstract class MemberInfoPageModel<TInfoType>(
    ILogger logger,
    object target,
    TInfoType info,
    bool isEditing = false,
    string? parentId = null
) : PageModel
{
    public TInfoType Info { get; } = info;

    public bool IsEditing { get; } = isEditing;

    public ILogger Logger { get; } = logger;

    public abstract string OwnId { get; }

    public string? ParentId { get; } = parentId;

    public object Target { get; } = target;
}