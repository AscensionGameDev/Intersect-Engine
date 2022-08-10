using ImGuiNET;

using Intersect.Localization;
using Intersect.Time;

namespace Intersect.Client.Framework.UserInterface.Components;

public class Window : Component
{
    public const ImGuiWindowFlags DefaultFlags = ImGuiWindowFlags.NoDocking;

    private ReactiveString _cacheTitleWithId;

    private MenuBar? _menuBar;
    private bool _open;
    private StatusBar? _statusBar;
    private LocalizedString? _title;
    private CustomFormatter? _titleFormatter;

    public Window(string? name = default) : base(name)
    {
        IsOpen = true;

        _cacheTitleWithId = CreateLabelWithId(name ?? string.Empty);
    }

    public bool DockingEnabled
    {
        get => !Flags.HasFlag(ImGuiWindowFlags.NoDocking);
        set => Flags = (Flags & ~ImGuiWindowFlags.NoDocking) | (value ? ImGuiWindowFlags.None : ImGuiWindowFlags.NoDocking);
    }

    public ImGuiWindowFlags Flags { get; set; } = DefaultFlags;

    public bool HasMenuBar
    {
        get => Flags.HasFlag(ImGuiWindowFlags.MenuBar);
        set => Flags = (Flags & ~ImGuiWindowFlags.MenuBar) | (value ? ImGuiWindowFlags.MenuBar : ImGuiWindowFlags.None);
    }

    public bool HasPendingChanges
    {
        get => Flags.HasFlag(ImGuiWindowFlags.UnsavedDocument);
        set => Flags = (Flags & ~ImGuiWindowFlags.UnsavedDocument) | (value ? ImGuiWindowFlags.UnsavedDocument : ImGuiWindowFlags.None);
    }

    public MenuBar? MenuBar
    {
        get => _menuBar;
        set
        {
            if (_menuBar == value)
            {
                return;
            }

            _menuBar = value;
            HasMenuBar = _menuBar == default;
        }
    }

    public bool IsOpen
    {
        get => _open;
        set => _open = value;
    }

    public StatusBar? StatusBar
    {
        get => _statusBar;
        set
        {
            if (_statusBar == value)
            {
                return;
            }

            _statusBar = value;
        }
    }

    #region Title

    public LocalizedString? Title
    {
        get => _title;
        set
        {
            if (value == _title)
            {
                return;
            }

            _title = value;
            _cacheTitleWithId = CreateLabelWithId(_title, _titleFormatter);
        }
    }

    protected CustomFormatter? TitleFormatter
    {
        get => _titleFormatter;
        set
        {
            if (value == _titleFormatter)
            {
                return;
            }

            _titleFormatter = value;
            _cacheTitleWithId = CreateLabelWithId(_title, _titleFormatter);
        }
    }

    #endregion Title

    protected override bool LayoutBegin(FrameTime frameTime)
    {
        StyleBegin(frameTime);

        var shouldLayoutChildren = ImGui.Begin(_cacheTitleWithId, ref _open, Flags);

        StyleEnd(frameTime);

        if (!IsOpen)
        {
            Parent = default;
            return false;
        }

        if (!shouldLayoutChildren)
        {
            return false;
        }

        var position = ImGui.GetWindowPos();
        var size = ImGui.GetWindowSize();
        if (SynchronizeBounds(new(position, size)))
        {
            ImGui.SetWindowPos(Position);
            ImGui.SetWindowSize(Size);
        }

        if (_menuBar?.Any() ?? false)
        {
            _menuBar.Layout(frameTime);
        }

        _statusBar?.Layout(frameTime);

        return true;
    }

    protected override void LayoutEnd(FrameTime frameTime)
    {
        ImGui.End();
    }

    protected virtual void StyleBegin(FrameTime frameTime)
    {
    }

    protected virtual void StyleEnd(FrameTime frameTime)
    {
    }
}
