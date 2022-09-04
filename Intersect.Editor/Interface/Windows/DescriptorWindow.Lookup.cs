using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Reflection;
using ImGuiNET;
using Intersect.Collections;
using Intersect.Comparison;
using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.Framework.Reflection;
using Intersect.Logging;
using Intersect.Models;
using Intersect.Models.Annotations;
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
        var contentArea = ImGui.GetWindowContentRegionMax();
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
            Y = contentArea.Y - (ImGui.GetCursorPosY() + windowPadding.Y + inputSize.Y)
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

        ImGui.SameLine();

        var editorContentArea = ImGui.GetWindowContentRegionMax();
        editorContentArea -= ImGui.GetCursorPos();
        ImGui.BeginChild("###descriptor_editor", editorContentArea, true);

        if (_selectedObjectId != default)
        {
            var selectedDescriptor = descriptorLookup.Get<Descriptor>(_selectedObjectId);

            foreach (var groupInteropInfo in ObjectInteropModel.Groups)
            {
                ImGui.BeginChild(
                    $"descriptor_editor_group_{groupInteropInfo.Name?.Get(Strings.Root)}",
                    new(),
                    true,
                    ImGuiWindowFlags.AlwaysAutoResize
                );

                var descriptorName = DescriptorType.GetLocalizedName(Strings.Descriptors);

                foreach (var propertyInteropInfo in groupInteropInfo.Properties)
                {
                    try
                    {
                        var propertyName = propertyInteropInfo.PropertyInfo.GetFullName();
                        var propertyValue = propertyInteropInfo.DelegateGet.DynamicInvoke(selectedDescriptor);
                        var propertyId = $"descriptor_editor_{selectedDescriptor.Id}_{propertyName}";

                        var inputAttribute = propertyInteropInfo.Input;
                        var label = propertyInteropInfo.Label?.Get(Strings.Root);
                        if (label != default)
                        {
                            ImGui.Text(label.ToString(descriptorName));
                            ImGui.SameLine();
                        }

                        switch (inputAttribute)
                        {
                            case null:
                                ImGui.Text($"{propertyValue}");
                                break;

                            case InputLookupAttribute inputLookupAttribute:
                            {
                                // https://gist.github.com/harold-b/7dcc02557c2b15d76c61fde1186e31d0
                                var selectedCombo = 0;
                                // ImGui.BeginCombo(
                                //     $"{inputLookupAttribute.ForeignKeyName}###{propertyId}",
                                //     propertyValue?.ToString()
                                // );
                                ImGui.Combo(
                                    $"###{propertyId}",
                                    ref selectedCombo,
                                    new[] { propertyValue?.ToString() ?? "Test" },
                                    1
                                );
                                break;
                            }

                            case InputTextAttribute inputTextAttribute:
                            {
                                var hint = inputTextAttribute.GetHint(Strings.Root);
                                var inputTextValue = propertyValue as string ?? propertyValue?.ToString();
                                var textInputFlags = ImGuiInputTextFlags.AutoSelectAll;
                                var maxLength = inputTextAttribute.MaximumLength;
                                if (propertyInteropInfo.IsReadOnly || inputTextAttribute.ReadOnly)
                                {
                                    textInputFlags |= ImGuiInputTextFlags.ReadOnly;
                                }

                                var result = hint == default
                                    ? ImGui.InputText(
                                        $"###{propertyId}",
                                        ref inputTextValue,
                                        maxLength,
                                        textInputFlags
                                    )
                                    : ImGui.InputTextWithHint(
                                        $"###{propertyId}",
                                        hint,
                                        ref inputTextValue,
                                        maxLength,
                                        textInputFlags
                                    );

                                propertyInteropInfo.DelegateSet?.DynamicInvoke(selectedDescriptor, inputTextValue);
                                break;
                            }
                        }

                        var tooltip = propertyInteropInfo.Tooltip?.Get(Strings.Root);
                        if (tooltip != default && ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.Text(tooltip.ToString(descriptorName));
                            ImGui.EndTooltip();
                        }
                    }
                    catch (Exception exception)
                    {
                        Log.Error(exception);
                        throw;
                    }
                }

                ImGui.EndChild();
            }
            // var propertyInfos = DescriptorType.GetObjectType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            //
            // foreach (var propertyInfo in propertyInfos)
            // {
            //     if (propertyInfo.PropertyType == typeof(string))
            //     {
            //         var inputValue = propertyInfo.GetValue(selectedDescriptor) as string;
            //         var textInputFlags = ImGuiInputTextFlags.AutoSelectAll;
            //         if (propertyInfo.SetMethod == default)
            //         {
            //             textInputFlags |= ImGuiInputTextFlags.ReadOnly;
            //         }
            //         ImGui.Text(propertyInfo.Name);
            //         _ = ImGui.InputTextWithHint(
            //             $"###{propertyInfo.GetFullName()}+{selectedDescriptor.Id}",
            //             "hint",
            //             ref inputValue,
            //             255,
            //             textInputFlags
            //         );
            //         if (propertyInfo.SetMethod != default)
            //         {
            //             propertyInfo.SetValue(selectedDescriptor, inputValue);
            //         }
            //     }
            // }
        }

        ImGui.EndChild();

        return true;
    }
}
