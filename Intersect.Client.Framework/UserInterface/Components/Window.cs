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

        DisplayMode = DisplayMode.Block;
        MenuBar = new();
    }

    public bool DockingEnabled
    {
        get => !Flags.HasFlag(ImGuiWindowFlags.NoDocking);
        set => Flags = (Flags & ~ImGuiWindowFlags.NoDocking) | (value ? ImGuiWindowFlags.None : ImGuiWindowFlags.NoDocking);
    }

    public ImGuiWindowFlags Flags { get; set; } = DefaultFlags;

    public MenuBar MenuBar { get; }

    public bool Open
    {
        get => _open;
        set
        {
            if (_open == value)
            {
                return;
            }

            _open = value;
            Invalidate();
        }
    }

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

    protected override bool DrawBegin()
    {
        if (!ImGui.Begin(Name, ref _open, Flags))
        {
            return false;
        }

        ImGui.SetWindowPos(Position);
        ImGui.SetWindowSize(Size);

        return true;
    }

    protected override void DrawBehindChildren(FrameTime frameTime)
    {
        MenuBar.Draw(frameTime);
    }

    protected override void DrawEnd()
    {
        ImGui.End();
    }
}
