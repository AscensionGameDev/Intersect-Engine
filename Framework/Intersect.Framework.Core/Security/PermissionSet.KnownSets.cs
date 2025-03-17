using System.Diagnostics.CodeAnalysis;

namespace Intersect.Framework.Core.Security;

public sealed partial class PermissionSet
{
    private static readonly Dictionary<string, PermissionSet> KnownPermissionSets = [];
    public static IReadOnlyCollection<PermissionSet> PermissionSets => KnownPermissionSets.Values;

    public static event PermissionSetChangedHandler? PermissionSetUpdated;

    public static bool HasPermissionSetUpdatedHandler(Delegate @delegate) =>
        PermissionSetUpdated?.GetInvocationList().Contains(@delegate) ?? false;

    private static void UpdatePermissionSet(PermissionSet permissionSet)
    {
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var knownPermissionSet in KnownPermissionSets.Values)
        {
            if (permissionSet.Name == knownPermissionSet.Name)
            {
                knownPermissionSet._inheritsFrom = permissionSet;
            }
        }

        KnownPermissionSets[permissionSet.Name] = permissionSet;
        PermissionSetUpdated?.Invoke(permissionSet);
    }

    public static bool TryGet(string permissionSetName, [NotNullWhen(true)] out PermissionSet? permissionSet) =>
        KnownPermissionSets.TryGetValue(permissionSetName, out permissionSet);
}