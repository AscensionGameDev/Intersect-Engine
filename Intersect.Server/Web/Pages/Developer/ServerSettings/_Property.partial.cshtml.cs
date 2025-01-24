using System.Reflection;
using Intersect.Framework.Reflection;

namespace Intersect.Server.Web.Pages.Developer.ServerSettings;

public class PropertyPartialPageModel(
    ILogger logger,
    object target,
    string? parentId,
    PropertyInfo propertyInfo,
    bool isEditing = false
)
    : PropertyInfoPageModel(
        logger: logger,
        target: target,
        parentId: parentId,
        propertyInfo: propertyInfo,
        isEditing: isEditing
    )
{
    public bool IsRoot { get; init; }

    public string ClassString => (IsEditing && !Info.PropertyType.IsReadOnly() ? "editing" : "display");

    public bool IsReadOnly => !IsEditing || Info.IsReadOnly();
}