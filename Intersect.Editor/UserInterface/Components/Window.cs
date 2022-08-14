using System.Numerics;

using ImGuiNET;

using Intersect.Localization;
using Intersect.Logging;
using Intersect.Time;

namespace Intersect.Client.Framework.UserInterface.Components;

public class Window : Component
{
    public const ImGuiWindowFlags DefaultFlags = ImGuiWindowFlags.None;

    private ReactiveString _cacheTitleWithId;

    private uint _dockId;
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

    // TODO: Uncomment when viewport locking can be implemented
    //public bool IsLockedToViewport { get; set; } = true;

    public bool IsOpen
    {
        get => _open;
        set => _open = value;
    }

    protected Vector2? SizeConstraintMinimum { get; set; }

    protected Vector2? SizeConstraintMaximum { get; set; }

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

        // TODO: Get viewport locking to work
        // if it's possible without bugs we need access to the ImGuiContext,
        // which currently does not get exposed even by ImGuiInternal
        //if (IsLockedToViewport)
        //{
        //    var viewport = ImGui.GetWindowViewport();
        //    FloatRect workspaceBounds = new(
        //        viewport.WorkPos,
        //        viewport.WorkSize
        //    );

        //    var position = ImGui.GetWindowPos();
        //    var size = ImGui.GetWindowSize();

        //    Vector2 clampedSize = new(
        //        Math.Min(size.X, workspaceBounds.Width),
        //        Math.Min(size.Y, workspaceBounds.Height)
        //    );

        //    Vector2 clampedPosition = new(
        //        MathHelper.Clamp(position.X, workspaceBounds.X, workspaceBounds.Right - size.X),
        //        MathHelper.Clamp(position.Y, workspaceBounds.Y, workspaceBounds.Bottom - size.Y)
        //    );

        //    if (clampedSize != size)
        //    {
        //        ImGui.SetWindowSize(clampedSize);
        //        Size = clampedSize;
        //    }

        //    if (clampedPosition != position)
        //    {
        //        ImGui.SetWindowPos(clampedPosition);
        //        Position = clampedPosition;
        //    }
        //}

        if (!shouldLayoutChildren)
        {
            return false;
        }

        if (_menuBar?.Any() ?? false)
        {
            _menuBar.Layout(frameTime);
        }

        _statusBar?.Layout(frameTime);

        return true;
    }

    protected static bool LayoutChild(FrameTime frameTime, string childId, Vector2 size, Func<FrameTime, bool> doLayout)
    {
        bool result;
        if (result = ImGui.BeginChild(childId, size, true, ImGuiWindowFlags.None))
        {
            try
            {
                result |= doLayout(frameTime);
            }
            catch (Exception exception)
            {
                Log.Error(exception);
                result = false;
            }
        }

        ImGui.EndChild();

        return result;
    }

    protected override void LayoutDirty(FrameTime frameTime)
    {
        if (_dockId != default)
        {
            return;
        }

        var workspaceBounds = WorkspaceBounds;

        if (!workspaceBounds.Contains(Position))
        {
            Position = new(
                Math.Max(Position.X, workspaceBounds.Position.X),
                Math.Max(Position.Y, workspaceBounds.Position.Y)
            );
        }

        ImGui.SetNextWindowPos(Position);
        ImGui.SetNextWindowSize(Size);
    }

    protected override void LayoutEnd(FrameTime frameTime)
    {
        _dockId = ImGui.GetWindowDockID();
        var position = ImGui.GetWindowPos();
        var size = ImGui.GetWindowSize();
        Bounds = new(position, size);
        ImGui.End();
    }

    protected virtual void StyleBegin(FrameTime frameTime)
    {
        ImGui.SetNextWindowSizeConstraints(
            SizeConstraintMinimum ?? new(200),
            SizeConstraintMaximum ?? WorkspaceSize
        );
    }

    protected virtual void StyleEnd(FrameTime frameTime)
    {
    }
}
