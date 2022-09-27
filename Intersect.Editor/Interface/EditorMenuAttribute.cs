using System.Reflection;
using Intersect.Editor.Interface.Components;

namespace Intersect.Editor.Interface;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
internal partial class EditorMenuAttribute : Attribute
{
    private int? _order;

    public int Order
    {
        get => _order ?? 0;
        set => _order = value;
    }

    private sealed class EditorMenuAttributeOrderComparer : IComparer<int?>
    {
        public int Compare(int? optionalA, int? optionalB)
        {
            if (optionalA == default)
            {
                return optionalB.HasValue ? 1 : 0;
            }
            else if (optionalB == default)
            {
                return -1;
            }

            var a = optionalA ?? throw new InvalidOperationException();
            var b = optionalB ?? throw new InvalidOperationException();

            if (Math.Sign(a) == Math.Sign(b))
            {
                return a - b;
            }

            return Math.Sign(b);
        }
    }

    public static List<Menu> FindMenus(EditorWindow editorWindow)
    {
        return editorWindow
            .GetType()
            .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Select(methodInfo => (Info: methodInfo, Attribute: methodInfo.GetCustomAttribute<EditorMenuAttribute>(true)))
            .Where(method => method.Attribute != default)
            .OrderBy(method => method.Attribute?._order, new EditorMenuAttributeOrderComparer())
            .Select(method => method.Info.Invoke(editorWindow, Array.Empty<object>()) as Menu)
            .OfType<Menu>()
            .ToList();
    }
}
