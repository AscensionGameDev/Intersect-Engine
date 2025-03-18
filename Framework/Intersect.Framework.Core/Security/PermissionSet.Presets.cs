namespace Intersect.Framework.Core.Security;

public sealed partial class PermissionSet
{
    public const string NameDefault = "default";
    public const string NameAdmin = "admin";

    public static PermissionSet Default
    {
        get
        {
            if (KnownPermissionSets.TryGetValue(NameDefault, out var permissionSet))
            {
                return permissionSet;
            }

            permissionSet = new PermissionSet(NameDefault, inheritsFrom: null);
            InsertPermissionSet(permissionSet);
            return permissionSet;
        }
    }

    public static PermissionSet Admin
    {
        get
        {
            if (KnownPermissionSets.TryGetValue(NameAdmin, out var permissionSet))
            {
                return permissionSet;
            }

            permissionSet = new PermissionSet(NameAdmin, inheritsFromName: NameDefault)
            {
                [Permission.EngineVersion] = true,
                [Permission.WindowAdmin] = true,
            };
            InsertPermissionSet(permissionSet);
            return permissionSet;
        }
    }
}