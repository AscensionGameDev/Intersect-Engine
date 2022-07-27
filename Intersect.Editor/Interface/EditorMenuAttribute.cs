using System.Reflection;

using Intersect.Client.Framework.UserInterface.Components;

namespace Intersect.Editor.Interface;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
internal class EditorMenuAttribute : Attribute
{
    public static List<Menu> FindMenus(EditorWindow editorWindow)
    {
        return editorWindow
            .GetType()
            .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(methodInfo => methodInfo.GetCustomAttribute<EditorMenuAttribute>(true) != default)
            .Select(methodInfo => methodInfo.Invoke(editorWindow, Array.Empty<object>()) as Menu)
            .Where(menu => menu != default)
            .Cast<Menu>()
            .ToList();
    }
}
