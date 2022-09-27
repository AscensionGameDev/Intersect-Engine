using System.Numerics;
using ImGuiNET;
using Intersect.Collections;
using Intersect.Comparison;
using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.Models;
using Intersect.Numerics;
using Intersect.Time;

namespace Intersect.Editor.Interface.Windows;

internal class FolderableComparer : IComparer<IFolderable>
{
    public bool FoldersFirst { get; set; }

    public int Compare(IFolderable? x, IFolderable? y)
    {
        return x switch
        {
            Folder xFolder when y is Folder yFolder => xFolder.CompareTo(yFolder),
            Descriptor xDescriptor when y is Descriptor yDescriptor => string.Compare(
                xDescriptor.Name,
                yDescriptor.Name,
                StringComparison.CurrentCulture
            ),
            Folder when y is Descriptor => FoldersFirst ? -1 : 1,
            Descriptor when y is Folder => FoldersFirst ? 1 : -1,
            _ => throw new InvalidOperationException($"Unsupported folderable subtype: {x?.GetType()} / {y?.GetType()}")
        };
    }
}

internal partial class DescriptorWindow
{
    private readonly NullHandlingStringComparer _lookupComparer = new()
    {
        EmptyStringComparison = EmptyStringComparison.AsNull, NullComparison = NullComparison.NullLast,
    };

    private readonly FolderableComparer _folderableComparer = new() { FoldersFirst = true, };

    private bool _groupByFolder = true;
    private string _searchQuery = string.Empty;
    private Guid _selectedObjectId;

    private void LayoutLookupNode(FrameTime frameTime, ref Guid selectedObjectId, IFolderable folderable)
        => LayoutLookupNode(frameTime, ref selectedObjectId, folderable, new(), out _);

    private void LayoutLookupNode(FrameTime frameTime, ref Guid selectedObjectId, IFolderable folderable,
        FloatRect groupRect, out FloatRect itemRect)
    {
        itemRect = new();

        var drawList = ImGui.GetWindowDrawList();
        var treeIndent = ImGui.GetStyle().IndentSpacing;
        var treeLineColor = ImGui.GetColorU32(ImGuiCol.Text);

        switch (folderable)
        {
            case Descriptor descriptor:
            {
                ImGui.TreePush(descriptor.Id.ToString());

                var currentlySelected = selectedObjectId == descriptor.Id;
                var wasSelected = currentlySelected;
                _ = ImGui.Selectable(
                    $"{descriptor.Name}###{descriptor.Id}",
                    ref currentlySelected,
                    ImGuiSelectableFlags.SpanAllColumns
                );
                if (currentlySelected)
                {
                    selectedObjectId = descriptor.Id;
                }
                else if (wasSelected)
                {
                    selectedObjectId = default;
                }

                var max = ImGui.GetItemRectMax();
                var min = ImGui.GetItemRectMin();
                itemRect = new(min, max - min);

                if (_groupByFolder && descriptor.ParentId != default)
                {
                    var horizontalLineStart = itemRect.Position + Vector2.UnitX * (float)Math.Ceiling(treeIndent / 2) +
                                              Vector2.UnitY * itemRect.Height / 2;
                    drawList.AddLine(horizontalLineStart, horizontalLineStart + Vector2.UnitX * treeIndent,
                        treeLineColor);
                }

                ImGui.TreePop();
                break;
            }

            case Folder folder:
            {
                var searchId = selectedObjectId;
                var groupSelected =
                    searchId != default && folder.Matches(searchId, matchParent: false, matchChildren: true);
                var nodeFlags = groupSelected ? ImGuiTreeNodeFlags.Selected : ImGuiTreeNodeFlags.None;

                if (!ImGui.TreeNodeEx($"{folder.Name ?? string.Empty}###descriptor_folder_{folder.Id}", nodeFlags))
                {
                    return;
                }

                var max = ImGui.GetItemRectMax();
                var min = ImGui.GetItemRectMin();
                itemRect = new FloatRect(min, max - min);

                var offsetVertical = itemRect.Height;

                var verticalStart = itemRect.Position + Vector2.UnitX * treeIndent + Vector2.UnitY * offsetVertical / 2;
                verticalStart.X += 1 - treeIndent / 2;

                var verticalEnd = verticalStart;

                foreach (var child in folder.Children)
                {
                    LayoutLookupNode(frameTime, ref selectedObjectId, child, itemRect, out var childRect);
                    verticalEnd.Y = (childRect.Top + childRect.Bottom) / 2;
                }

                drawList.AddLine(verticalStart, verticalEnd, treeLineColor);
                ImGui.TreePop();
                break;
            }
        }
    }

    protected virtual bool LayoutLookup(FrameTime frameTime)
    {
        var contentArea = ImGui.GetWindowContentRegionMax() - ImGui.GetWindowContentRegionMin();
        var windowPadding = ImGui.GetStyle().WindowPadding;

        ImGui.BeginChild(
            "###descriptor_lookup",
            contentArea with { X = Math.Clamp(contentArea.X * 0.2f, 250f, 400f) },
            false
        );

        ImGui.SetNextItemWidth(Math.Clamp(contentArea.X * 0.2f, 250f, 400f));
        _ = ImGui.InputTextWithHint("###descriptor_lookup_searchQuery",
            Strings.Windows.Descriptor.SearchQueryHint.ToString(DescriptorName),
            ref _searchQuery,
            100,
            ImGuiInputTextFlags.AutoSelectAll
        );

        var inputSize = ImGui.GetItemRectSize();
        var lookupTreeContainerSize = inputSize with
        {
            Y = contentArea.Y - ImGui.GetCursorPosY()
        };

        _ = ImGui.BeginChild("###descriptor_lookup_tree", lookupTreeContainerSize, true);

        var storeFolders = ObjectStore<Folder>.Instance
            .Where(kvp => kvp.Value.DescriptorType == DescriptorType)
            .Select(kvp => kvp.Value);

        var descriptorLookup = DescriptorType.GetLookup();
        var parentlessDescriptors = descriptorLookup.Values
            .OfType<Descriptor>()
            .Where(descriptor => descriptor.ParentId == default);

        IEnumerable<IFolderable> rootNodes = storeFolders
            .OfType<IFolderable>()
            .Concat(parentlessDescriptors);

        if (!string.IsNullOrEmpty(_searchQuery))
        {
            rootNodes = rootNodes.Where(
                folderable => folderable.Matches(
                    _searchQuery,
                    StringComparison.CurrentCultureIgnoreCase,
                    matchParent: false,
                    matchChildren: true
                )
            );
        }

        rootNodes = rootNodes
            .OrderBy(node => node, _folderableComparer);

        foreach (var rootNode in rootNodes)
        {
            LayoutLookupNode(frameTime, ref _selectedObjectId, rootNode);
        }

        ImGui.EndChild();
        ImGui.EndChild();

        return true;
    }
}
