using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Intersect.Server.Web.Pages.Developer.ServerSettings;

public class TypePartialPageModel(
    ILogger logger,
    object target,
    Type type,
    bool isEditing = false,
    string? parentId = null,
    bool isRoot = false,
    string? sectionName = null
)
    : TypePageModel(
        logger: logger,
        target: target,
        type: type,
        isEditing: isEditing,
        parentId: parentId
    )
{
    public bool IsRoot { get; } = isRoot;

    public string? SectionName { get; } = sectionName;
}