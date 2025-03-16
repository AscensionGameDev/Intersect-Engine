using System.Diagnostics;
using System.Runtime.CompilerServices;
using MessagePack;

namespace Intersect.Framework.Core.Security;

[DebuggerDisplay("{Name,nq}")]
[MessagePackObject]
[MessagePackFormatter(typeof(PermissionSetMessagePackFormatter))]
public sealed partial class PermissionSet
{
    private readonly Dictionary<Permission, bool> _permissionGranted = [];
    private PermissionSet? _inheritsFrom;
    private readonly string? _inheritsFromName;

    private PermissionSet(string name, PermissionSet? inheritsFrom, string? inheritsFromName)
    {
        Name = name;
        _inheritsFrom = inheritsFrom;
        _inheritsFromName = inheritsFromName;
    }

    public PermissionSet(string name, PermissionSet? inheritsFrom) : this(
        name: name,
        inheritsFrom: inheritsFrom,
        inheritsFromName: inheritsFrom?.Name
    )
    {
    }

    public PermissionSet(string name, string inheritsFromName = NameDefault) : this(
        name: name,
        inheritsFrom: string.IsNullOrWhiteSpace(inheritsFromName)
            ? null
            : KnownPermissionSets.GetValueOrDefault(inheritsFromName),
        inheritsFromName: inheritsFromName
    )
    {
    }

    public bool this[Permission permission]
    {
        get => IsGranted(permission);
        set => _permissionGranted[permission] = value;
    }

    public string Name { get; init; }

    public void ClearOverride(Permission permission) => _ = _permissionGranted.Remove(permission);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool AreAllGranted(params Permission[] permissions) => permissions.All(IsGranted);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsGranted(Permission permission)
    {
        if (_permissionGranted.TryGetValue(permission, out var granted))
        {
            return granted;
        }

        return _inheritsFrom?.IsGranted(permission) ?? false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsAnyGranted(params Permission[] permissions) => permissions.Any(IsGranted);
}