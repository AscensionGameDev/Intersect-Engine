using System.ComponentModel;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.Utility;
using Intersect.Client.Localization;

namespace Intersect.Client.Interface.Debugging;

public sealed class TexturesSearchableTreeDataProvider : ISearchableTreeDataProvider
{
    private readonly GameContentManager _contentManager;
    private readonly Dictionary<string, SearchableTreeDataEntry> _entries;
    private readonly Dictionary<string, Dictionary<string, SearchableTreeDataEntry>> _entriesByParent;
    private readonly Base _parent;

    public TexturesSearchableTreeDataProvider(GameContentManager contentManager, Base parent)
    {
        _contentManager = contentManager;
        _parent = parent;

        _entries = _contentManager.Content.SelectMany(
            contentTypeEntry =>
            {
                var contentType = contentTypeEntry.Key;
                var contentTypeId = $"{nameof(ContentType)}:{contentType}";
                if (!Strings.Content.Types.TryGetValue(contentType, out var contentTypeName))
                {
                    contentTypeName = contentType.ToString();
                }

                SearchableTreeDataEntry[] entriesForTextureType =
                [
                    new(
                        Id: contentTypeId,
                        DisplayText: contentTypeName,
                        DisplayColor: parent.Skin.Colors.Label.Default
                    ),
                    ..contentTypeEntry.Value.OfType<GameTexture>()
                        .Select(texture => EntryForAsset(contentTypeId, texture)),
                ];

                return entriesForTextureType;
            }
        ).ToDictionary(entry => entry.Id);

        _entriesByParent = _entries.Values
            .SelectMany(entry => GetAncestorIds(entry.ParentId).Select(ancestorId => (ancestorId, entry)))
            .GroupBy(tuple => tuple.ancestorId)
            .ToDictionary(
                group => group.Key,
                group => group.ToDictionary(tuple => tuple.entry.Id, tuple => tuple.entry)
            );
    }

    private string[] GetAncestorIds(string? firstAncestorId)
    {
        if (string.IsNullOrWhiteSpace(firstAncestorId))
        {
            return [];
        }

        List<string> ancestorIds = [];

        var entryId = firstAncestorId;
        while (!string.IsNullOrWhiteSpace(entryId))
        {
            ancestorIds.Add(entryId);

            if (!_entries.TryGetValue(entryId, out var entry))
            {
                entryId = null;
                continue;
            }

            entryId = entry.ParentId;
        }

        return ancestorIds.ToArray();
    }

    private SearchableTreeDataEntry EntryForAsset(string parentId, IAsset asset)
    {
        Color displayColor = Color.Green;
        if (asset.IsDisposed)
        {
            displayColor = Color.Red;
        }
        else if (!asset.IsLoaded)
        {
            displayColor = Color.White;
        }

        asset.Disposed += OnAssetStateChanged;
        asset.Loaded += OnAssetStateChanged;
        asset.Unloaded += OnAssetStateChanged;

        var assetName = asset.Name ?? asset.Id;

        var displayText = assetName;
        if (asset is GameTexture { TexturePackFrame: not null })
        {
            displayText = Strings.Debug.FormatTextureFromAtlas.ToString(displayText);
        }

        return new SearchableTreeDataEntry(
            Id: $"{parentId}:{assetName}",
            DisplayText: displayText,
            DisplayColor: displayColor,
            Name: assetName,
            ParentId: parentId
        );

        void OnAssetStateChanged(IAsset changedAsset)
        {
            var updatedEntry = EntryForAsset(parentId, changedAsset);
            _entries[updatedEntry.Id] = updatedEntry;
            var ancestorIds = GetAncestorIds(updatedEntry.ParentId);

            foreach (var ancestorId in ancestorIds)
            {
                if (!_entriesByParent.TryGetValue(ancestorId, out var entriesForAncestor))
                {
                    entriesForAncestor = [];
                    _entriesByParent[ancestorId] = entriesForAncestor;
                }

                entriesForAncestor[updatedEntry.Id] = updatedEntry;
            }

            EntriesChanged?.Invoke(
                _parent,
                new SearchableTreeDataEntriesEventArgs
                {
                    Entries = [updatedEntry],
                }
            );
        }
    }

    public event Base.GwenEventHandler<SearchableTreeDataEntriesEventArgs>? EntriesAdded;
    public event Base.GwenEventHandler<SearchableTreeDataEntriesEventArgs>? EntriesRemoved;
    public event Base.GwenEventHandler<SearchableTreeDataEntriesEventArgs>? EntriesChanged;

    public bool DoesEntryMatchFilterCriteria(
        SearchableTreeFilterCriteria filterCriteria,
        SearchableTreeDataEntry entry,
        bool checkChildren = false
    )
    {
        var query = filterCriteria.Query?.Trim();
        if (string.IsNullOrWhiteSpace(query))
        {
            return true;
        }

        if (checkChildren)
        {
            if (_entriesByParent.TryGetValue(entry.Id, out var childEntries))
            {
                if (childEntries.Values.Any(child => DoesEntryMatchFilterCriteria(filterCriteria, child)))
                {
                    return true;
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(entry.Name) && entry.Name.Contains(query))
        {
            return true;
        }

        if (entry.DisplayText.Contains(query))
        {
            return true;
        }

        var parentId = entry.ParentId;
        if (string.IsNullOrWhiteSpace(parentId))
        {
            return false;
        }

        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (!_entries.TryGetValue(parentId, out var parentEntry))
        {
            return false;
        }

        return DoesEntryMatchFilterCriteria(filterCriteria, parentEntry, checkChildren: false);
    }

    public SearchableTreeDataEntry[] FilterEntries(SearchableTreeFilterCriteria filterCriteria, ListSortDirection? sortDirection)
    {
        var matchingEntries = _entries.Values
            .Where(entry => DoesEntryMatchFilterCriteria(filterCriteria, entry, checkChildren: true))
            .ToArray();

        return sortDirection switch
        {
            ListSortDirection.Ascending => matchingEntries.OrderBy(entry => entry.DisplayText).ToArray(),
            ListSortDirection.Descending => matchingEntries.OrderByDescending(entry => entry.DisplayText).ToArray(),
            null => matchingEntries,
            _ => throw new ArgumentOutOfRangeException(nameof(sortDirection), sortDirection, "Invalid sort direction"),
        };
    }
}