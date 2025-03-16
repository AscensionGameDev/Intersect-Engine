using Intersect.Core;
using Microsoft.Extensions.Logging;

namespace Intersect.Framework.Core.Security;

public sealed partial class PermissionSet
{
    private static string _activePermissionSetName = NameDefault;
    private static PermissionSet? _activePermissionSet;

    public static event ActivePermissionSetChangedHandler? ActivePermissionSetChanged;

    public static PermissionSet ActivePermissionSet
    {
        get
        {
            var activePermissionSet = _activePermissionSet;
            if (activePermissionSet != null)
            {
                return activePermissionSet;
            }

            var activePermissionSetName = _activePermissionSetName;
            if (!TryGet(activePermissionSetName, out activePermissionSet))
            {
                ApplicationContext.CurrentContext.Logger.LogError(
                    "Unknown active permission set '{PermissionSetName}'",
                    activePermissionSetName
                );
                return Default;
            }

            _activePermissionSet = activePermissionSet;
            return activePermissionSet;
        }
        set
        {
            if (value == _activePermissionSet)
            {
                return;
            }

            SetActivePermissionSet(value, value.Name);
        }
    }

    public static string ActivePermissionSetName
    {
        get => _activePermissionSetName;
        set
        {
            if (value == _activePermissionSetName)
            {
                return;
            }

            SetActivePermissionSet(null, value);
        }
    }

    private static void SetActivePermissionSet(PermissionSet? permissionSet, string permissionSetName) {

        ApplicationContext.Context.Value?.Logger.LogInformation(
            "Updating active permission set to '{PermissionSetName}'",
            permissionSetName
        );

        _activePermissionSetName = permissionSetName;
        _activePermissionSet = permissionSet;
        ActivePermissionSetChanged?.Invoke(permissionSetName);
    }
}