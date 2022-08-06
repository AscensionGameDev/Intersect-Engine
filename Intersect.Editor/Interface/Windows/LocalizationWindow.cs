using ImGuiNET;

using Intersect.Client.Framework.UserInterface.Components;
using Intersect.Editor.Localization;
using Intersect.Time;

namespace Intersect.Editor.Interface.Windows;

internal partial class LocalizationWindow : Window
{
    public LocalizationWindow() : base(nameof(LocalizationWindow))
    {
        Flags = ImGuiWindowFlags.AlwaysAutoResize;
        Title = Strings.Windows.Localization.Title;
    }

    protected override bool LayoutBegin(FrameTime frameTime)
    {
        var workSize = ImGui.GetWindowViewport().WorkSize;
        ImGui.SetNextWindowSizeConstraints(new(600, 400), workSize);
        if (!base.LayoutBegin(frameTime))
        {
            return false;
        }

        if (!LayoutTable(frameTime))
        {
            return false;
        }

        return true;
    }
}
