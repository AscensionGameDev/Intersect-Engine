using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Intersect.Server.Web.Pages.Developer.ServerSettings;

public class TypeBodyPartialPageModel(
    ILogger logger,
    object target,
    Type type,
    bool isEditing = false,
    string? parentId = null
)
    : TypePageModel(
        logger: logger,
        target: target,
        type: type,
        isEditing: isEditing,
        parentId: parentId
    )
{
}