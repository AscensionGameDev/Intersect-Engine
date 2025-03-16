using System.Diagnostics;

namespace Intersect.Framework.Core.Security;

[DebuggerDisplay("{Name,nq}")]
public readonly record struct Permission(string Name)
{
    public static readonly Permission EngineVersion = nameof(EngineVersion);
    public static readonly Permission WindowAdmin = nameof(WindowAdmin);

    public static implicit operator Permission(string name) => new(name);
}