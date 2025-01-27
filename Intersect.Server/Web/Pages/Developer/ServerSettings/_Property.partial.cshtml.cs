using System.Reflection;
using Intersect.Framework.Annotations;
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

    public string ClassString
    {
        get
        {
            List<string> classes = ["field"];
            if (IsEditing && !Info.PropertyType.IsReadOnly())
            {
                classes.Add("editing");
            }
            else
            {
                classes.Add("display");
            }
            if (RequiresRestartAttribute.RequiresRestart(Info))
            {
                classes.Add("requires-restart");
            }

            return string.Join(' ', classes);
        }
    }

    public bool IsReadOnly => !IsEditing || Info.IsReadOnly();
}