namespace Intersect.Reflection;

public static partial class ObjectExtensions
{
    public static string GetFullishName(this object @object)
    {
        var type = @object.GetType();
        return type.FullName ?? type.Name;
    }
}