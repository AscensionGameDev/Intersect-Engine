using ImGuiNET;

using Intersect.Client.Framework.UserInterface;
using Intersect.Client.Framework.UserInterface.Components;
using Intersect.Editor.Localization;
using Intersect.Time;

namespace Intersect.Editor.UserInterface.Components;

public class ImGuiDebugger : Component
{
    private bool _showMetricsWindow;
    private bool _showStackToolWindow;
    private bool _showStyleEditor;

    private ImGuiDebugger() : base() { }

    protected override bool LayoutBegin(FrameTime frameTime)
    {
        if (_showMetricsWindow)
        {
            ImGui.ShowMetricsWindow();
        }

        if (_showStackToolWindow)
        {
            ImGui.ShowStackToolWindow();
        }

        if (_showStyleEditor)
        {
            if (ImGui.Begin("Dear ImGui Style Editor", ref _showStyleEditor))
            {
                ImGui.ShowStyleEditor();
            }
            ImGui.End();
        }

        return true;
    }

    protected override void LayoutEnd(FrameTime frameTime) { }

    public static void Create(Canvas canvas, out MenuItem menuItem)
    {
#if DEBUG
        ImGuiDebugger imGuiDebugger = new();

        var menuItemMetricsWindow = new MenuItemOption
        {
            Name = Strings.ImGui.ShowMetricsWindow
        };

        menuItemMetricsWindow.Selected += (_, _) => imGuiDebugger._showMetricsWindow = !imGuiDebugger._showMetricsWindow;

        var menuItemShowStackToolWindow = new MenuItemOption
        {
            Name = Strings.ImGui.ShowStackToolWindow
        };

        menuItemShowStackToolWindow.Selected += (_, _) => imGuiDebugger._showStackToolWindow = !imGuiDebugger._showStackToolWindow;

        var menuItemShowStyleEditor = new MenuItemOption
        {
            Name = Strings.ImGui.ShowStyleEditor
        };

        menuItemShowStyleEditor.Selected += (_, _) => imGuiDebugger._showStyleEditor = !imGuiDebugger._showStyleEditor;

        menuItem = new MenuItem
        {
            Name = Strings.General.OpenX.ToString(Strings.ImGui.ImGuiDebugger),
            Items = new List<IMenuItem>
            {
                menuItemMetricsWindow,
                menuItemShowStackToolWindow,
                menuItemShowStyleEditor,
            }
        };

        canvas.Children.Add(imGuiDebugger);
#else
        menuItem = default;
#endif
    }
}
