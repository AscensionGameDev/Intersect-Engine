using Intersect.Client.Framework.Platform;
using Intersect.Client.Framework.UserInterface.Components;
using Intersect.Editor.Localization;
using Intersect.Localization;
using Intersect.Time;

namespace Intersect.Editor.Interface;

public abstract class EditorWindow
{
    private readonly Canvas _canvas;
    private readonly PlatformWindow _systemWindow;

    protected EditorWindow(
        PlatformWindow systemWindow
    )
    {
        _systemWindow = systemWindow;

        _canvas = new Canvas("root");

        _canvas.MenuBar.AddRange(EditorMenuAttribute.FindMenus(this));
    }

    public bool IsClosing { get; private set; }

    public LocalizedString Title => Strings.Application.Name;

    public void Close() => IsClosing = true;

    public void Update(FrameTime frameTime)
    {
        if (!string.Equals(_systemWindow.Title, Title, StringComparison.Ordinal))
        {
            _systemWindow.Title = Title;
        }

        _canvas.Bounds = new(0, 0, _systemWindow.Viewport.Width, _systemWindow.Viewport.Height);
        _canvas.Layout(frameTime);
    }
}
