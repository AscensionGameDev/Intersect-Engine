using Intersect.Client.Framework.Platform;

namespace Intersect.Editor.Interface;

internal partial class EditorMainWindow : EditorWindow
{
    public EditorMainWindow(
        PlatformWindow systemWindow
    ) : base(systemWindow)
    {
    }
}
