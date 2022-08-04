using ImGuiNET;

using Intersect.Client.Framework.UserInterface.Components;
using Intersect.Editor.Localization;
using Intersect.Editor.Networking;
using Intersect.Enums;
using Intersect.Time;

namespace Intersect.Editor.Interface.Windows;

internal partial class DescriptorWindow : Window
{
    private readonly GameObjectType _descriptorType;

    public DescriptorWindow(GameObjectType descriptorType)
        : base(descriptorType.ToString())
    {
        _descriptorType = descriptorType;

        Flags = ImGuiWindowFlags.AlwaysAutoResize;
        Title = Strings.Windows.Descriptor.Title.ToString(Strings.Descriptors.Names[_descriptorType].Singular);

        try
        {
            PacketSender.SendOpenEditor(_descriptorType);
        }
        catch
        {
            throw;
        }
    }

    public bool HasPendingChanges
    {
        get => Flags.HasFlag(ImGuiWindowFlags.UnsavedDocument);
        set => Flags = (Flags & ~ImGuiWindowFlags.UnsavedDocument) | (value ? ImGuiWindowFlags.UnsavedDocument : ImGuiWindowFlags.None);
    }

    protected override bool LayoutBegin(FrameTime frameTime)
    {
        if (!base.LayoutBegin(frameTime))
        {
            return false;
        }

        if (!LayoutLookup(frameTime))
        {
            return false;
        }

        return true;
    }
}
