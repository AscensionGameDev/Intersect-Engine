using System.Numerics;
using System.Reflection;

using ImGuiNET;

using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.UserInterface;
using Intersect.Client.Framework.UserInterface.Components;
using Intersect.Editor.Localization;
using Intersect.Editor.MonoGame.Content;
using Intersect.Localization;
using Intersect.Logging;
using Intersect.Metadata.Licensing;
using Intersect.Platform;
using Intersect.Properties;
using Intersect.Time;

namespace Intersect.Editor.Interface.Windows;

internal partial class PreferencesWindow : Window
{
    public PreferencesWindow() : base(nameof(PreferencesWindow))
    {
        Flags = ImGuiWindowFlags.NoDocking;
        Title = Strings.Windows.Preferences.Title;
    }

    protected override bool LayoutBegin(FrameTime frameTime)
    {
        var workSize = ImGui.GetWindowViewport().WorkSize;
        ImGui.SetNextWindowSizeConstraints(new(600, 400), workSize);
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
