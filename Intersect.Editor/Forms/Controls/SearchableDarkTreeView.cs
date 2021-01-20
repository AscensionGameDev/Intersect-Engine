using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using DarkUI.Controls;

using Intersect.Collections;
using Intersect.Models;

namespace Intersect.Editor.Forms.Controls
{

    public partial class SearchableDarkTreeView : UserControl
    {

        private readonly Dictionary<Guid, DarkTreeNode> mIdNodeLookup;

        private IGameObjectLookup<IDatabaseObject> mItemProvider;

        private string mPreviousSearchText;

        public SearchableDarkTreeView()
        {
            InitializeComponent();

            mIdNodeLookup = new Dictionary<Guid, DarkTreeNode>();
        }

        public DarkTreeNode SelectedNode => treeViewItems?.SelectedNodes?.FirstOrDefault();

        public IObject SelectedObject => SelectedNode?.Tag as IObject;

        public Guid SelectedId
        {
            get => SelectedObject?.Id ?? Guid.Empty;
            set => treeViewItems?.SelectNode(mIdNodeLookup.TryGetValue(value, out var node) ? node : null);
        }

        public IGameObjectLookup<IDatabaseObject> ItemProvider
        {
            get => mItemProvider;
            set
            {
                mItemProvider = value;

                UpdateNodes();
            }
        }

        public string SearchText
        {
            get => txtSearch?.Text;
            set
            {
                if (txtSearch == null)
                {
                    throw new ArgumentNullException(nameof(txtSearch));
                }

                txtSearch.Text = value;
            }
        }

        public virtual void Refresh()
        {
            UpdateNodes();
        }

        public bool FilterBySearchText(IDatabaseObject databaseObject)
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                return true;
            }

            var name = databaseObject?.Name;
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            var searchText = SearchText.Trim();
            if (searchText.Length > name.Length)
            {
                return false;
            }

            return -1 < name.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase);
        }

        protected virtual bool FilterBySearchText(KeyValuePair<Guid, IDatabaseObject> pair)
        {
            return FilterBySearchText(pair.Value);
        }

        protected virtual DarkTreeNode ObjectAsNode(IDatabaseObject databaseObject)
        {
            if (!mIdNodeLookup.TryGetValue(databaseObject.Id, out var node))
            {
                node = new DarkTreeNode(databaseObject.Name ?? "NULL");
            }

            return node;
        }

        protected virtual DarkTreeNode PairAsNode(KeyValuePair<Guid, IDatabaseObject> pair)
        {
            return ObjectAsNode(
                pair.Value ??
                throw new ArgumentNullException(nameof(pair.Value), $@"{pair.Key} has a null object associated.")
            );
        }

        protected void UpdateNodes()
        {
            treeViewItems?.Nodes?.Clear();

            // TODO: Remove this when we fix DarkUI, which currently does not invalidate (and will show stale values when the list is emptied).
            treeViewItems?.Invalidate();

            if (ItemProvider == null)
            {
                return;
            }

            mPreviousSearchText = SearchText;

            var filtered = string.IsNullOrWhiteSpace(SearchText)
                ? ItemProvider
                : ItemProvider.Where(FilterBySearchText);

            var nodes = filtered.Select(PairAsNode).ToList();

            treeViewItems?.Nodes?.AddRange(nodes);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            var searchText = SearchText?.Trim() ?? "";
            var previousSearchText = mPreviousSearchText?.Trim() ?? "";

            if (string.Equals(searchText, previousSearchText, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            UpdateNodes();
        }

        private void toolStripCreate_Click(object sender, EventArgs e)
        {
        }

    }

}
