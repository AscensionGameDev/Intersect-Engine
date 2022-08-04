using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImGuiNET;

using Intersect.Enums;
using Intersect.Time;

namespace Intersect.Editor.Interface.Windows;

internal partial class DescriptorWindow
{
    public virtual bool LayoutLookup(FrameTime frameTime)
    {
        _ = ImGui.BeginChild(string.Empty, new(200, 500), true);

        var descriptorLookup = _descriptorType.GetLookup();
        foreach (var descriptor in descriptorLookup)
        {
            _ = ImGui.Selectable(descriptor.Value.Name);
        }

        ImGui.EndChild();

        return true;
    }
}
