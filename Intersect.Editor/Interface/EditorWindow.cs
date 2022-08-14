using System.Numerics;

using ImGuiNET;

using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Platform;
using Intersect.Client.Framework.UserInterface;
using Intersect.Client.Framework.UserInterface.Components;
using Intersect.Editor.Localization;
using Intersect.Localization;
using Intersect.Time;

namespace Intersect.Editor.Interface;

public abstract class EditorWindow
{
    internal const string NAME_CONTAINER_CANVAS = "container_canvas";
    private const string NAME_CONTAINER_STATUS_BAR = "container_status_bar";

    private readonly Canvas _canvas;
    private readonly IContentManager _contentManager;
    private readonly PlatformWindow _platformWindow;

    protected EditorWindow(
        IContentManager contentManager,
        PlatformWindow platformWindow
    )
    {
        _contentManager = contentManager;
        _platformWindow = platformWindow;
        _platformWindow.Resized += (_, _) => _canvas?.Invalidate(true);

        _canvas = new Canvas(NAME_CONTAINER_CANVAS, _contentManager)
        {
            StatusBar = new StatusBar(NAME_CONTAINER_STATUS_BAR),
        };

        _canvas.MenuBar = new MenuBar(EditorMenuAttribute.FindMenus(this))
        {
            IsMainMenuBar = true,
        };
    }

    protected Canvas Canvas => _canvas;

    public IList<Component> Components => _canvas.Children;

    protected GraphicsDevice GraphicsDevice => _platformWindow.GraphicsDevice;

    public bool IsClosing { get; private set; }

    public Numerics.Point Position
    {
        get => _platformWindow.Position;
        set => _platformWindow.Position = value;
    }

    public Numerics.Point Size
    {
        get => _platformWindow.Size;
        set => _platformWindow.Size = value;
    }

    public LocalizedString Title => Strings.Application.Name;

    public void Close() => IsClosing = true;

    public void Update(FrameTime frameTime)
    {
        if (IsClosing)
        {
            _platformWindow.Close();
            return;
        }

        if (!string.Equals(_platformWindow.Title, Title, StringComparison.Ordinal))
        {
            _platformWindow.Title = Title;
        }

        _canvas.Layout(frameTime);
    }
}
