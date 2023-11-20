using System.Runtime.CompilerServices;

namespace Intersect.Framework.Reflection;

public static class ObjectExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetQualifiedTypeName(this object @object) => @object.GetType().GetQualifiedName();
}