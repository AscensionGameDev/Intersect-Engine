using System.Reflection;
using Intersect.Framework.Reflection;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Intersect.Server.Web.Pages.Developer.ServerSettings;

public class ServerSettingsIndexModel(ILogger<ServerSettingsIndexModel> logger) : PageModel
{
    private const string CategoryGeneral = "Category_General";

    public readonly ILogger<ServerSettingsIndexModel> Logger = logger;

    private static string GetGroupKey(PropertyInfo propertyInfo)
    {
        if (propertyInfo.PropertyType.IsValueType)
        {
            return CategoryGeneral;
        }

        if (propertyInfo.PropertyType == typeof(string))
        {
            return CategoryGeneral;
        }

        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (propertyInfo.PropertyType.IsEnumerable())
        {
            return CategoryGeneral;
        }

        return $"Category_{propertyInfo.Name}";
    }

    public static readonly Dictionary<string, PropertyInfo[]> PropertyGroups = typeof(Options)
        .GetProperties(BindingFlags.Instance | BindingFlags.Public)
        .Where(propertyInfo => !propertyInfo.IsIgnored())
        .GroupBy(GetGroupKey)
        .ToDictionary(group => group.Key, group => group.ToArray());

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