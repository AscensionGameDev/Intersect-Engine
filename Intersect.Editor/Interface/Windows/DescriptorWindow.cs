using ImGuiNET;

using Intersect.Client.Framework.UserInterface.Components;
using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.Time;

namespace Intersect.Editor.Interface.Windows;

internal partial class DescriptorWindow : Window
{
    public DescriptorWindow(GameObjectType descriptorType)
        : base(descriptorType.ToString())
    {
        Flags = ImGuiWindowFlags.AlwaysAutoResize;
        Title = Strings.Windows.Descriptor.Title.ToString(Strings.Descriptors.Names[descriptorType].Singular);
    }

    protected override bool LayoutBegin(FrameTime frameTime)
    {
        if (!base.LayoutBegin(frameTime))
        {
            return false;
        }

        _ = ImGui.BeginChild(string.Empty, new(200, 100), true);

        ImGui.EndChild();

        return true;
    }
}
