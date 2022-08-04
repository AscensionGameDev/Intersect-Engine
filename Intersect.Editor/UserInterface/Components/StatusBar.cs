using System.Numerics;

using ImGuiNET;

using Intersect.Network;
using Intersect.Time;

namespace Intersect.Client.Framework.UserInterface.Components;

public class StatusBar : Component
{
    protected const ImGuiWindowFlags StatusBarFlags =
        ImGuiWindowFlags.NoCollapse
        //| ImGuiWindowFlags.NoBringToFrontOnFocus
        //| ImGuiWindowFlags.NoMove
        | ImGuiWindowFlags.NoNavFocus
        | ImGuiWindowFlags.NoResize
        | ImGuiWindowFlags.NoSavedSettings
        | ImGuiWindowFlags.NoTitleBar
        ;

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

    protected override bool LayoutBegin(FrameTime frameTime)
    {
        var viewport = ImGui.GetMainViewport();

        ImGui.PushStyleColor(ImGuiCol.Border, Vector4.One * 0.5f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 1);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0);
        //ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);

        var windowPadding = ImGui.GetStyle().WindowPadding;
        var textHeight = ImGui.GetTextLineHeightWithSpacing();
        var windowSize = new Vector2(2 + viewport.WorkSize.X, textHeight + windowPadding.Y * 2);
        var windowPosition = new Vector2(-1, 1 + viewport.WorkPos.Y + viewport.WorkSize.Y - windowSize.Y);

        ImGui.SetNextWindowPos(windowPosition);
        ImGui.SetNextWindowSize(windowSize);

        if (!ImGui.Begin(Name, Flags))
        {
            return false;
        }

        ImGui.PopStyleVar(2);
        ImGui.PopStyleColor();

        ImGui.Text($"Viewport: {viewport.WorkPos} {viewport.WorkSize} | Network Status: {_networkStatus} {(_connectionDenied ? "(Connection Denied)" : string.Empty)}");

        return true;
    }

    protected override void LayoutEnd(FrameTime frameTime)
    {
        ImGui.End();
    }
}
