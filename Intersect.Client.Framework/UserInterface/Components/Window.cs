using ImGuiNET;

using Intersect.Client.Framework.UserInterface.Styling;
using Intersect.Time;

namespace Intersect.Client.Framework.UserInterface.Components;

public class Window : Component
{
    public const ImGuiWindowFlags DefaultFlags = ImGuiWindowFlags.NoDocking;

    private bool _open;
    private string _title;

    public Window(string name) : base(name)
    {
        _open = true;

        MenuBar = new();
    }

    public bool DockingEnabled
    {
        get => !Flags.HasFlag(ImGuiWindowFlags.NoDocking);
        set => Flags = (Flags & ~ImGuiWindowFlags.NoDocking) | (value ? ImGuiWindowFlags.None : ImGuiWindowFlags.NoDocking);
    }

    public ImGuiWindowFlags Flags { get; set; } = DefaultFlags;

    public MenuBar MenuBar { get; }

    public string Title
    {
        get => _title;
        set
        {
            if (string.Equals(value, _title, StringComparison.Ordinal))
            {
                return;
            }

            _title = value;
            Invalidate();
        }
    }

    protected override bool LayoutBegin()
    {
        if (!ImGui.Begin(Name, ref _open, Flags))
        {
            return false;
        }

        var position = ImGui.GetWindowPos();
        var size = ImGui.GetWindowSize();
        SynchronizeBounds(new(position, size));

        return true;
    }

    protected override void DrawBehindChildren(FrameTime frameTime)
    {
        MenuBar.Draw(frameTime);
    }

    protected override void LayoutEnd()
    {

        ImGui.End();
    }

    protected override void Layout()
    {
        base.Layout();
        ImGui.SetWindowPos(Position);
        ImGui.SetWindowSize(Size);
    }
}
