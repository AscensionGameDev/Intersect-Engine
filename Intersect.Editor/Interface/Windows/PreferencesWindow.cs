using System.Numerics;

using ImGuiNET;

using Intersect.Client.Framework.UserInterface;
using Intersect.Client.Framework.UserInterface.Components;
using Intersect.Editor.Localization;
using Intersect.Time;

namespace Intersect.Editor.Interface.Windows;

internal partial class PreferencesWindow : Window
{
    public PreferencesWindow() : base(nameof(PreferencesWindow))
    {
        Flags = ImGuiWindowFlags.NoDocking;
        SizeConstraintMinimum = new(600, 400);
        Title = Strings.Windows.Preferences.Title;
    }

    protected override bool LayoutBegin(FrameTime frameTime)
    {
        if (!base.LayoutBegin(frameTime))
        {
            return false;
        }

        var padding = ImGui.GetStyle().WindowPadding;
        var windowContentSize = ImGui.GetWindowContentRegionMax() - ImGui.GetWindowContentRegionMin();

        var browserSize = new Vector2(
            Math.Max(200, windowContentSize.X / 10f),
            windowContentSize.Y
        );

        var pageSize = new Vector2(
            windowContentSize.X - (browserSize.X + padding.X),
            windowContentSize.Y
        );

        if (!LayoutChild(frameTime, "preferences_browser", browserSize, LayoutPreferenceBrowser))
        {
            return false;
        }

        ImGui.SameLine();

        if (!LayoutChild(frameTime, "preferences_page", pageSize, LayoutPreferencePage))
        {
            return false;
        }

        return true;
    }

    protected bool LayoutPreferenceBrowser(FrameTime frameTime)
    {
        return true;
    }

    protected bool LayoutPreferencePage(FrameTime frameTime)
    {
        return true;
    }
}

internal abstract partial class PreferencesPage : Component
{
    protected PreferencesPage(string? name = default) : base(name)
    {

    }
}
