using Intersect.Editor.Localization.MenuBar;
using Intersect.Editor.Localization.Windows;
using Intersect.Localization.Common;
using Intersect.Localization.Common.Descriptors;

namespace Intersect.Editor.Localization;

internal partial class Strings
{
    public static readonly EditorRootNamespace Root = new();

    public static EditorApplicationNamespace Application => Root.Application;

    public static DescriptorsNamespace Descriptors => Root.Descriptors;

    public static CommonGeneralNamespace General => Root.General;

    public static LicensingNamespace Licensing => Root.Licensing;

    public static MenuBarNamespace MenuBar => Root.MenuBar;

    public static WindowsNamespace Windows => Root.Windows;
}
