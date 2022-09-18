using System.Reflection;
using ImGuiNET;
using Intersect.Client.Framework.UserInterface.Components;
using Intersect.Editor.Localization;
using Intersect.Editor.Networking;
using Intersect.Enums;
using Intersect.Localization;
using Intersect.Models.Interop;
using Intersect.Network;
using Intersect.Network.Events;
using Intersect.Time;

namespace Intersect.Editor.Interface.Windows;

internal partial class DescriptorWindow : Window
{
    public DescriptorWindow(GameObjectType descriptorType)
        : base($"{nameof(DescriptorWindow)}+{descriptorType}")
    {
        DescriptorType = descriptorType;
        ObjectInteropModel = new ObjectInteropModel.Builder(DescriptorType.GetObjectType())
            .IncludeProperties()
            .IncludeReadOnly()
            .SetPropertyBindingFlags(BindingFlags.Public)
            .Build();

        SizeConstraintMinimum = new(600, 400);
        Title = Strings.Windows.Descriptor.Title.ToString(DescriptorName);

        try
        {
            void RequestDescriptors(INetworkLayerInterface? sender, ConnectionEventArgs? connectionEventArgs)
            {
                PacketSender.SendOpenEditor(DescriptorType);
            }

            if (Networking.Network.Connected)
            {
                RequestDescriptors(default, default);
            }

            Networking.Network.Socket.Connected += RequestDescriptors;
        }
        catch
        {
            throw;
        }
    }

    private LocalizedPluralString DescriptorName => Strings.Descriptors.Names[DescriptorType];

    public GameObjectType DescriptorType { get; }

    protected ObjectInteropModel ObjectInteropModel { get; }

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

        ImGui.SameLine();

        if (!LayoutEditor(frameTime))
        {
            return false;
        }

        return true;
    }
}
