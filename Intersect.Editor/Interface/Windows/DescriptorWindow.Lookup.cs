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
    public static readonly FolderableComparer Instance = new();

    public bool FoldersFirst { get; set; }

    public int Compare(IFolderable? x, IFolderable? y)
    {
        if (x is Folder xFolder && y is Folder yFolder)
        {
            return xFolder.CompareTo(yFolder);
        }

        if (x is Descriptor xDescriptor && y is Descriptor yDescriptor)
        {
            return xDescriptor.Name.CompareTo(yDescriptor.Name);
        }

        if (x is Folder && y is Descriptor)
        {
            return FoldersFirst ? -1 : 1;
        }

        if (x is Descriptor && y is Folder)
        {
            return FoldersFirst ? 1 : -1;
        }

        throw new InvalidOperationException($"Unsupported folderable subtype: {x?.GetType()} / {y?.GetType()}");
    }
}

internal partial class DescriptorWindow
{
    private readonly NullHandlingStringComparer _lookupComparer = new()
    {
        EmptyStringComparison = EmptyStringComparison.AsNull,
        NullComparison = NullComparison.NullLast,
    };

    private readonly FolderableComparer _folderableComparer = new()
    {
        FoldersFirst = true,
    };

    private bool _groupByFolder = true;
    private string _searchQuery = string.Empty;
    private Guid _selectedObjectId;

    private void LayoutLookupNode(FrameTime frameTime, ref Guid selectedObjectId, IFolderable folderable)
        => LayoutLookupNode(frameTime, ref selectedObjectId, folderable, new(), out _);

    private void LayoutLookupNode(FrameTime frameTime, ref Guid selectedObjectId, IFolderable folderable, FloatRect groupRect, out FloatRect itemRect)
    {
        itemRect = new();

        var drawList = ImGui.GetWindowDrawList();
        var treeIndent = ImGui.GetStyle().IndentSpacing;
        var treeLineColor = ImGui.GetColorU32(ImGuiCol.Text);

        if (folderable is Descriptor descriptor)
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

            var max = ImGui.GetItemRectMax();
            var min = ImGui.GetItemRectMin();
            itemRect = new FloatRect(min, max - min);

            if (_groupByFolder && descriptor.ParentId != default)
            {
                var horizontalLineStart = itemRect.Position + Vector2.UnitX * (float)Math.Ceiling(treeIndent / 2) + Vector2.UnitY * itemRect.Height / 2;
                drawList.AddLine(horizontalLineStart, horizontalLineStart + Vector2.UnitX * treeIndent, treeLineColor);
            }

            ImGui.TreePop();
        }
        else if (folderable is Folder folder)
        {
            var searchId = selectedObjectId;
            var groupSelected = searchId != default && folder.Matches(searchId, matchParent: false, matchChildren: true);
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
        }
    }

    public virtual bool LayoutLookup(FrameTime frameTime)
    {
        _ = ImGui.InputTextWithHint("###descriptor_lookup_searchQuery", Strings.Windows.Descriptor.SearchQueryHint.ToString(DescriptorName), ref _searchQuery, 100, ImGuiInputTextFlags.AutoSelectAll);
        var inputSize = ImGui.GetItemRectSize();

        _ = ImGui.BeginChild(string.Empty, new(inputSize.X, 500), true);

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

        SizeConstraintMinimum = ImGui.GetCursorPos();

        return true;
    }
}
