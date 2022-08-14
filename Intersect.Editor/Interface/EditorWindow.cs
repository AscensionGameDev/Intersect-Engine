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
    private const string NAME_CONTAINER_CANVAS = "container_canvas";
    private const string NAME_CONTAINER_STATUS_BAR = "container_status_bar";
    private const string NAME_CONTAINER_WORKSPACE = "container_workspace";

    private readonly Canvas _canvas;
    private readonly IContentManager _contentManager;
    //private readonly MenuBar _menuBar;
    //private readonly StatusBar _statusBar;
    private uint _dockNodeId;
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
            MenuBar = new MenuBar(EditorMenuAttribute.FindMenus(this))
            {
                IsMainMenuBar = true,
            },
            StatusBar = new StatusBar(NAME_CONTAINER_STATUS_BAR),
        };
    }

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

    public unsafe void Update(FrameTime frameTime)
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

        if (_dockNodeId == default)
        {
            _dockNodeId = ImGui.GetID(NAME_CONTAINER_CANVAS);
            if (ImGuiInternal.DockBuilderGetNode(_dockNodeId).NativePtr != default)
            {
                ImGuiInternal.DockBuilderRemoveNode(_dockNodeId);
            }
        }

        if (ImGuiInternal.DockBuilderGetNode(_dockNodeId).NativePtr == default)
        {
            _ = ImGuiInternal.DockBuilderAddNode(_dockNodeId, ImGuiDockNodeFlags.None);

            var viewport = ImGui.GetMainViewport();
            ImGuiInternal.DockBuilderSetNodePos(_dockNodeId, default);
            ImGuiInternal.DockBuilderSetNodeSize(_dockNodeId, viewport.WorkSize);

            _ = ImGuiInternal.DockBuilderSplitNode(_dockNodeId, ImGuiDir.Up, 2, out var workspaceDockId, out var statusBarDockId);

            ImGuiInternal.DockBuilderDockWindow(NAME_CONTAINER_WORKSPACE, workspaceDockId);
            ImGuiInternal.DockBuilderDockWindow(NAME_CONTAINER_STATUS_BAR, statusBarDockId);

            ImGuiInternal.DockBuilderFinish(_dockNodeId);
        }

        _canvas.Layout(frameTime);
    }
}
