using System.Numerics;

using ImGuiNET;

using Intersect.Comparison;
using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.Models;
using Intersect.Numerics;
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

    private static void LayoutLookupGroup(ref Guid selectedObjectId, IGrouping<string?, IDatabaseObject> descriptorGroup)
    {
        var isNamedGroup = !string.IsNullOrEmpty(descriptorGroup.Key);
        var drawList = ImGui.GetWindowDrawList();
        var groupNodeRect = new FloatRect();
        var treeIndent = ImGui.GetStyle().IndentSpacing;
        var offsetVertical = 0f;
        var treeLineColor = ImGui.GetColorU32(ImGuiCol.Text);

        if (isNamedGroup)
        {
            var searchId = selectedObjectId;
            var groupSelected = searchId != default && descriptorGroup.Any(descriptor => descriptor.Id == searchId);
            var nodeFlags = groupSelected ? ImGuiTreeNodeFlags.Selected : ImGuiTreeNodeFlags.None;

            // TODO: When the descriptor groups have true folders, descriptorGroup.Key will be replaced with a Guid
            if (!ImGui.TreeNodeEx($"{descriptorGroup.Key?.Trim() ?? string.Empty}###descriptor_group_{descriptorGroup.Key}", nodeFlags))
            {
                return;
            }

            groupNodeRect = new(ImGui.GetItemRectMin(), ImGui.GetItemRectMax());
            offsetVertical = ImGui.GetItemRectSize().Y;
        }

        var verticalStart = groupNodeRect.Position + Vector2.UnitX * treeIndent + Vector2.UnitY * offsetVertical / 2; //ImGui.GetCursorScreenPos() - Vector2.UnitY * 8;
        verticalStart.X += 1 - treeIndent / 2;

        var verticalEnd = verticalStart;

        foreach (var descriptor in descriptorGroup)
        {
            ImGui.TreePush(descriptor.Id.ToString());

            var currentlySelected = selectedObjectId == descriptor.Id;
            if (ImGui.Selectable($"{descriptor.Name}###{descriptor.Id}", currentlySelected, ImGuiSelectableFlags.SpanAllColumns))
            {
                selectedObjectId = descriptor.Id;
            }
            else if (currentlySelected)
            {
                selectedObjectId = default;
            }

            var itemRect = new FloatRect(ImGui.GetItemRectMin(), ImGui.GetItemRectMax());
            var midpoint = (ImGui.GetItemRectMax().Y + ImGui.GetItemRectMin().Y) / 2;
            verticalEnd.Y = midpoint;

            if (isNamedGroup)
            {
                drawList.AddLine(verticalEnd, verticalEnd + Vector2.UnitX * treeIndent, treeLineColor);
            }

            ImGui.TreePop();
        }

        if (isNamedGroup)
        {
            drawList.AddLine(verticalStart, verticalEnd, treeLineColor);
            ImGui.TreePop();
        }
    }

    public virtual bool LayoutLookup(FrameTime frameTime)
    {
        _ = ImGui.InputTextWithHint("###descriptor_lookup_searchQuery", Strings.Windows.Descriptor.SearchQueryHint.ToString(DescriptorName), ref _searchQuery, 100, ImGuiInputTextFlags.AutoSelectAll);
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
            LayoutLookupGroup(ref _selectedObjectId, descriptorGroup);
        }

        ImGui.EndChild();

        return true;
    }
}
