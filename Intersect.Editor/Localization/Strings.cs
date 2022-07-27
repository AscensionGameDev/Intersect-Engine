using Intersect.Editor.Localization.MenuBar;

namespace Intersect.Editor.Localization;

internal partial class Strings
{
    public static readonly EditorRootNamespace Root = new();

    public static EditorApplicationNamespace Application => Root.Application;

    public static MenuBarNamespace MenuBar => Root.MenuBar;
}
