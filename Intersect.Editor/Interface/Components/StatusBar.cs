using System.Numerics;
using ImGuiNET;
using Intersect.Network;
using Intersect.Time;

namespace Intersect.Editor.Interface.Components;

public class StatusBar : Component
{
    protected const ImGuiWindowFlags StatusBarFlags =
        ImGuiWindowFlags.NoCollapse |
        ImGuiWindowFlags.NoDocking |
        ImGuiWindowFlags.NoMove |
        ImGuiWindowFlags.NoNavFocus |
        ImGuiWindowFlags.NoResize |
        ImGuiWindowFlags.NoSavedSettings |
        ImGuiWindowFlags.NoTitleBar;

    private bool _connectionDenied;
    private NetworkStatus _networkStatus;

    public StatusBar(string name) : base(name)
    {
        Flags = StatusBarFlags;

        Editor.Networking.Network.Socket.Connected += (_, connectionEventArgs) => _networkStatus = connectionEventArgs.NetworkStatus;

        Editor.Networking.Network.Socket.ConnectionFailed += (_, connectionEventArgs, denied) =>
        {
            _connectionDenied = denied;
            _networkStatus = connectionEventArgs.NetworkStatus;
        };

        Editor.Networking.Network.Socket.Disconnected += (_, connectionEventArgs) => _networkStatus = connectionEventArgs.NetworkStatus;
    }

    public ImGuiWindowFlags Flags { get; set; }

    public Vector2 CalculatedSize { get; private set; }

    protected override bool LayoutBegin(FrameTime frameTime)
    {
        var viewport = ImGui.GetMainViewport();

        ImGui.PushStyleColor(ImGuiCol.Border, Vector4.One * 0.5f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 1);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0);

        var windowPadding = ImGui.GetStyle().WindowPadding;
        var textHeight = ImGui.GetTextLineHeightWithSpacing();

        CalculatedSize = new Vector2(viewport.WorkSize.X, textHeight + windowPadding.Y * 2);
        var statusBarPosition = new Vector2(-1, 1 + viewport.WorkPos.Y + viewport.WorkSize.Y - CalculatedSize.Y);

        ImGui.SetNextWindowPos(statusBarPosition);
        ImGui.SetNextWindowSize(CalculatedSize + Vector2.UnitX * 2);

        if (!ImGui.Begin(Name, Flags))
        {
            return false;
        }

        ImGui.PopStyleVar(2);
        ImGui.PopStyleColor();

        Vector2 workspaceSize = new(WorkspaceSize.X, WorkspaceSize.Y);
        workspaceSize -= Vector2.UnitY * CalculatedSize.Y;
        ImGui.Text($"Viewport: {WorkspacePosition} {workspaceSize} | Network Status: {_networkStatus} {(_connectionDenied ? "(Connection Denied)" : string.Empty)}");

        return true;
    }

    protected override void LayoutEnd(FrameTime frameTime)
    {
        ImGui.End();
    }
}
