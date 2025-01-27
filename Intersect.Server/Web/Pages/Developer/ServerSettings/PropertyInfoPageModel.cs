using System.Reflection;

namespace Intersect.Server.Web.Pages.Developer.ServerSettings;

public abstract class PropertyInfoPageModel(
    ILogger logger,
    object target,
    string? parentId,
    PropertyInfo propertyInfo,
    bool isEditing = false
)
    : MemberInfoPageModel<PropertyInfo>(
        logger: logger,
        target: target,
        info: propertyInfo,
        isEditing: isEditing,
        parentId: parentId
    )
{
    public override string OwnId => ParentId == null ? Info.Name : string.Join('.', ParentId, Info.Name);
}