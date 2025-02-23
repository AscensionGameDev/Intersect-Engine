using System.ComponentModel;
using Intersect.Client.Framework.Graphics;
using Intersect.Core;
using Microsoft.Extensions.Logging;

namespace Intersect.Client.Framework.Gwen.Control.Utility;

public sealed class SearchableTree : Base
{
    private readonly Dictionary<string, TreeNode> _displayedNodes = [];

    private readonly TextBox _searchInput;
    private readonly TreeControl _tree;
    private readonly ISearchableTreeDataProvider _dataProvider;

    private ListSortDirection? _sortDirection;

    private bool _dirty;

    public event GwenEventHandler<EventArgs>? SelectionChanged;

    public SearchableTree(Base parent, ISearchableTreeDataProvider dataProvider, string? name = default) : base(parent: parent, name: name)
    {
        _searchInput = new TextBox(this, nameof(_searchInput))
        {
            Dock = Pos.Top,
            Margin = new Margin(0, 0, 0, 4),
        };
        _searchInput.TextChanged += SearchInputOnTextChanged;

        _tree = new TreeControl(this, nameof(_tree))
        {
            Dock = Pos.Fill,
        };
        _tree.SelectionChanged += TreeOnSelectionChanged;

        _dataProvider = dataProvider;
        _dataProvider.EntriesAdded += DataProvider_EntriesAdded;
        _dataProvider.EntriesChanged += DataProviderOnEntriesChanged;
        _dataProvider.EntriesRemoved += DataProviderOnEntriesRemoved;

        _dirty = true;
    }

    public SearchableTreeFilterCriteria FilterCriteria
    {
        get => new(_searchInput.Text);
        set
        {
            if (string.Equals(value.Query, _searchInput.Text, StringComparison.CurrentCulture))
            {
                return;
            }

            _searchInput.Text = value.Query;
            FetchEntries(value, []);
        }
    }

    public IFont? FontSearch
    {
        get => _searchInput.Font;
        set => _searchInput.Font = value;
    }

    public int FontSearchSize
    {
        get => _searchInput.FontSize;
        set => _searchInput.FontSize = value;
    }

    public IFont? FontTree
    {
        get => _tree.Font;
        set => _tree.Font = value;
    }

    public int FontTreeSize
    {
        get => _tree.FontSize;
        set => _tree.FontSize = value;
    }

    public string? SearchPlaceholderText
    {
        get => _searchInput.PlaceholderText;
        set => _searchInput.PlaceholderText = value;
    }

    public TreeNode[] SelectedNodes => _tree.SelectedChildren.ToArray();

    public ListSortDirection? SortDirection
    {
        get => _sortDirection;
        set
        {
            if (value == _sortDirection)
            {
                return;
            }

            _sortDirection = value;
            FetchEntries(FilterCriteria, [], sortDirection: value);
        }
    }

    protected override void Render(Skin.Base skin)
    {
        base.Render(skin);

        if (!_dirty)
        {
            return;
        }

        _dirty = false;
        FetchEntries(FilterCriteria, [], initial: _displayedNodes.Count < 1);
    }

    private void DataProvider_EntriesAdded(Base sender, SearchableTreeDataEntriesEventArgs arguments)
    {
        var filterCriteria = FilterCriteria;
        var matchingEntries =
            arguments.Entries.Where(entry => _dataProvider.DoesEntryMatchFilterCriteria(filterCriteria, entry)).ToArray();
        if (matchingEntries.Length < 1)
        {
            ApplicationContext.Context.Value?.Logger.LogDebug(
                "{EntryCount} entries were added but none were relevant to the current filter criteria {FilterCriteria}",
                arguments.Entries.Length,
                filterCriteria
            );
            return;
        }
        FetchEntries(filterCriteria, added: matchingEntries);
    }

    private void DataProviderOnEntriesChanged(Base sender, SearchableTreeDataEntriesEventArgs arguments)
    {
        foreach (var entry in arguments.Entries)
        {
            if (!_displayedNodes.TryGetValue(entry.Id, out var displayedNode))
            {
                continue;
            }

            displayedNode.Text = entry.DisplayText;
            displayedNode.TextColorOverride = entry.DisplayColor;
        }
    }

    private void DataProviderOnEntriesRemoved(Base sender, SearchableTreeDataEntriesEventArgs arguments)
    {
        foreach (var entry in arguments.Entries)
        {
            if (!_displayedNodes.Remove(entry.Id, out var node))
            {
                continue;
            }

            node.Parent = null;
        }
    }

    private void FetchEntries(
        SearchableTreeFilterCriteria filterCriteria,
        SearchableTreeDataEntry[] added,
        ListSortDirection? sortDirection = null,
        bool initial = false
    )
    {
        sortDirection ??= SortDirection;

        var entries = _dataProvider.FilterEntries(filterCriteria: filterCriteria, sortDirection: sortDirection);
        Dictionary<TreeNode, TreeNode> lastSiblingByParent = [];

        var visibleEntryIds = entries.Select(entry => entry.Id).ToHashSet();

        foreach (var node in _displayedNodes.Values)
        {
            if (node.UserData is not SearchableTreeDataEntry nodeEntry)
            {
                node.IsVisibleInTree = false;
                continue;
            }

            node.IsVisibleInTree = visibleEntryIds.Contains(nodeEntry.Id);
        }

        foreach (var entry in entries)
        {
            TreeNode parent = _tree;

            if (_displayedNodes.TryGetValue(entry.Id, out var node))
            {
                if (node.Parent is TreeNode existingNodeParent)
                {
                    parent = existingNodeParent;
                }
                else
                {
                    _displayedNodes.Remove(entry.Id);
                    node = null;
                }
            }

            if (node == null)
            {
                if (!added.Contains(entry) && !initial)
                {
                    ApplicationContext.CurrentContext.Logger.LogError(
                        "Entry '{EntryId}' ({EntryDisplayText}) was not found in the tree but wasn't added either, this state should be debugged",
                        entry.Id,
                        entry.Name ?? entry.DisplayText
                    );
                }

                if (entry.ParentId is { } parentId && !string.IsNullOrWhiteSpace(parentId))
                {
                    if (!_displayedNodes.TryGetValue(parentId, out var parentById))
                    {
                        throw new InvalidOperationException(
                            $"Missing parent entry '{parentId}' when trying to add '{entry.Id}'"
                        );
                    }

                    parent = parentById;
                }

                node = parent.AddNode(entry.DisplayText);
                node.TextColorOverride = entry.DisplayColor;
                node.UserData = entry;
                _displayedNodes[entry.Id] = node;
            }

            if (lastSiblingByParent.TryGetValue(parent, out var lastSibling))
            {
                node.MoveAfter(lastSibling);
            }
            else
            {
                lastSibling = parent.Children.OfType<TreeNode>().FirstOrDefault();


                if (lastSibling != node && lastSibling != null)
                {
                    node.MoveBefore(lastSibling);
                }
            }

            lastSiblingByParent[parent] = node;
        }
    }

    private void SearchInputOnTextChanged(Base sender, EventArgs arguments)
    {
        FetchEntries(FilterCriteria, []);
    }

    private void TreeOnSelectionChanged(Base sender, EventArgs arguments)
    {
        SelectionChanged?.Invoke(sender, arguments);
    }
}