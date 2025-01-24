using System.Reflection;
using Intersect.Framework.Reflection;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Intersect.Server.Web.Pages.Developer.ServerSettings;

public class ServerSettingsIndexModel(ILogger<ServerSettingsIndexModel> logger) : PageModel
{
    public readonly ILogger<ServerSettingsIndexModel> Logger = logger;

    public static readonly Dictionary<string, PropertyInfo[]> PropertyGroups = typeof(Options)
        .GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(propertyInfo => !propertyInfo.IsIgnored())
        .GroupBy(
            propertyInfo => !propertyInfo.PropertyType.IsValueType && typeof(string) != propertyInfo.PropertyType
                ? propertyInfo.Name
                : "General"
        ).ToDictionary(group => group.Key, group => group.ToArray());

    public Options Target { get; set; } = Options.Instance.DeepClone();

    public void OnGet()
    {

    }

    public void OnPush(JsonPatchDocument<Options> optionsChanges)
    {

    }

    public PropertyPartialPageModel GetModelFor(PropertyInfo propertyInfo) =>
        new(
            Logger,
            target: Target,
            parentId: null,
            propertyInfo,
            isEditing: true
        )
        {
            IsRoot = true,
        };
}