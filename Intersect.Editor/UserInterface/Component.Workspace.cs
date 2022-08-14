using System.Numerics;

using ImGuiNET;

using Intersect.Numerics;

namespace Intersect.Client.Framework.UserInterface;

public partial class Component
{
    public virtual FloatRect WorkspaceBounds
    {
        get
        {
            if (Root == this)
            {
                var imguiViewport = ImGui.GetWindowViewport();
                return new(
                    imguiViewport.WorkPos,
                    imguiViewport.WorkSize
                );
            }

            return Root.WorkspaceBounds;
        }
    }

    public Vector2 WorkspacePosition => WorkspaceBounds.Position;

    public Vector2 WorkspaceSize => WorkspaceBounds.Size;
}
