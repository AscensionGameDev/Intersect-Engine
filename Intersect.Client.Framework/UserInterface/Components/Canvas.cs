using ImGuiNET;

namespace Intersect.Client.Framework.UserInterface.Components;

public class Canvas : Component
{
    protected const ImGuiWindowFlags CanvasFlags = ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoInputs;

    public Canvas(string? name = default) : base(name)
    {

    }

    protected override bool DrawBegin()
    {
        if (!ImGui.Begin(Name, CanvasFlags))
        {
            return false;
        }

        return true;
    }

    protected override void DrawEnd()
    {
        ImGui.End();
    }
}
