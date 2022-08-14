using Intersect.Client.Framework.UserInterface.Styling;
using ImGuiNET;
using Intersect.Time;
using System.Numerics;
using Intersect.Client.Framework.Content;
using Intersect.Numerics;

namespace Intersect.Client.Framework.UserInterface.Components;

public class Canvas : Window
{
    protected const ImGuiWindowFlags CanvasFlags =
        ImGuiWindowFlags.MenuBar |
        ImGuiWindowFlags.NoBringToFrontOnFocus |
        ImGuiWindowFlags.NoCollapse |
        ImGuiWindowFlags.NoDocking |
        ImGuiWindowFlags.NoMove |
        ImGuiWindowFlags.NoNavFocus |
        ImGuiWindowFlags.NoResize |
        ImGuiWindowFlags.NoSavedSettings |
        ImGuiWindowFlags.NoTitleBar;

    private readonly IContentManager _contentManager;

    public Canvas(string name, IContentManager contentManager)  : base(name)
    {
        _contentManager = contentManager;

        Display = DisplayMode.Block;
        Flags = CanvasFlags;
    }

    protected override IContentManager? ContentManager => _contentManager;

    public override FloatRect WorkspaceBounds
    {
        get
        {
            var imguiViewport = ImGui.GetWindowViewport();
            var size = imguiViewport.WorkSize;
            size -= Vector2.UnitY * (StatusBar?.CalculatedSize.Y ?? 0);
            return new(
                imguiViewport.WorkPos,
                size
            );
        }
    }

    protected override void StyleBegin(FrameTime frameTime)
    {
        var viewport = ImGui.GetMainViewport();
        ImGui.SetNextWindowPos(WorkspacePosition);
        ImGui.SetNextWindowSize(WorkspaceSize);
        ImGui.SetNextWindowViewport(viewport.ID);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2());
    }

    protected override void StyleEnd(FrameTime frameTime)
    {
        ImGui.PopStyleVar(3);

        var dockspaceId = ImGui.GetID("dockspace_main");
        _ = ImGui.DockSpace(dockspaceId, default, ImGuiDockNodeFlags.PassthruCentralNode);
    }
}
