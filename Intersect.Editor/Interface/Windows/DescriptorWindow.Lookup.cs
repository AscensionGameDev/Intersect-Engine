using ImGuiNET;

using Intersect.Comparison;
using Intersect.Enums;
using Intersect.Models;
using Intersect.Time;

namespace Intersect.Editor.Interface.Windows;

internal partial class DescriptorWindow
{
    private readonly NullHandlingStringComparer _lookupComparer = new()
    {
        EmptyStringComparison = EmptyStringComparison.AsNull,
        NullComparison = NullComparison.NullLast,
    };

    private bool _groupByFolder = true;
    private string _searchQuery = string.Empty;
    private Guid _selectedObjectId;

    public virtual bool LayoutLookup(FrameTime frameTime)
    {
        _ = ImGui.InputText("###descriptor_lookup_searchQuery", ref _searchQuery, 100, ImGuiInputTextFlags.AutoSelectAll);
        var inputSize = ImGui.GetItemRectSize();

        _ = ImGui.BeginChild(string.Empty, new(inputSize.X, 500), true);

        var descriptorLookup = _descriptorType.GetLookup();
        IEnumerable<IDatabaseObject> descriptorValues = descriptorLookup.Values;
        if (!string.IsNullOrEmpty(_searchQuery))
        {
            descriptorValues = descriptorValues.Where(
                descriptor =>
                    (descriptor.Name?.Contains(_searchQuery, StringComparison.CurrentCultureIgnoreCase) ?? false)
                    || ((descriptor as IFolderable)?.Folder?.Contains(_searchQuery, StringComparison.CurrentCultureIgnoreCase) ?? false)
            );
        }

        var groupedDescriptors = descriptorValues
                .OrderBy(descriptor => descriptor.Name, _lookupComparer)
                .GroupBy(descriptor => _groupByFolder ? (descriptor as IFolderable)?.Folder : default)
                .OrderBy(group => group.Key, _lookupComparer);

        foreach (var descriptorGroup in groupedDescriptors)
        {
            if (!string.IsNullOrEmpty(descriptorGroup.Key))
            {
                var groupSelected = descriptorGroup.Any(descriptor => descriptor.Id == _selectedObjectId);
                var nodeFlags = groupSelected ? ImGuiTreeNodeFlags.Selected : ImGuiTreeNodeFlags.None;

                // TODO: When the descriptor groups have true folders, descriptorGroup.Key will be replaced with a Guid
                if (!ImGui.TreeNodeEx($"{descriptorGroup.Key?.Trim() ?? string.Empty}###descriptor_group_{descriptorGroup.Key}", nodeFlags))
                {
                    continue;
                }
            }

            foreach (var descriptor in descriptorGroup)
            {
                ImGui.TreePush(descriptor.Id.ToString());

                var currentlySelected = _selectedObjectId == descriptor.Id;
                if (ImGui.Selectable($"{descriptor.Name}###{descriptor.Id}", currentlySelected, ImGuiSelectableFlags.SpanAllColumns))
                {
                    _selectedObjectId = descriptor.Id;
                }
                else if (currentlySelected)
                {
                    _selectedObjectId = default;
                }

                ImGui.TreePop();
            }

            if (!string.IsNullOrEmpty(descriptorGroup.Key))
            {
                ImGui.TreePop();
            }
        }

        ImGui.EndChild();

        return true;
    }
}
