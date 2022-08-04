using Intersect.Client.Framework.UserInterface.Styling;
using ImGuiNET;
using Intersect.Time;
using System.Numerics;
using Intersect.Client.Framework.Content;

namespace Intersect.Client.Framework.UserInterface.Components;

public class Canvas : Window
{
    protected const ImGuiWindowFlags CanvasFlags =
        ImGuiWindowFlags.NoCollapse |
        ImGuiWindowFlags.NoBringToFrontOnFocus |
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

    protected override void StyleBegin(FrameTime frameTime)
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
    }

    protected override void StyleEnd(FrameTime frameTime)
    {
        ImGui.PopStyleVar(3);
    }
}
