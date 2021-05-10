using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Intersect.Editor.Forms.Controls
{
    public partial class GameObjectList : TreeView
    {
        private bool mChangingName = false;

        public delegate void UpdateItemDelegate(Guid id);

        public delegate void UpdateToolstripDelegate();

        public delegate void ToolstripButtonClickDelegate(object sender, EventArgs e);

        public UpdateItemDelegate UpdateItemHandler;

        public UpdateToolstripDelegate FocusChangedHandler;

        public ToolstripButtonClickDelegate ToolStripItemNew_Click;

        public ToolstripButtonClickDelegate ToolStripItemCopy_Click;

        public ToolstripButtonClickDelegate ToolStripItemPaste_Click;

        public ToolstripButtonClickDelegate ToolStripItemUndo_Click;

        public ToolstripButtonClickDelegate ToolStripItemDelete_Click;

        private List<string> mExpandedFolders = new List<string>();


        public GameObjectList()
        {
            InitializeComponent();

            ImageList = imageList;

            AfterSelect += GameObjectList_AfterSelect;

            NodeMouseClick += GameObjectList_NodeMouseClick;

            GotFocus += GameObjectList_GotFocus;

            LostFocus += GameObjectList_LostFocus;

            KeyDown += GameObjectList_KeyDown;
        }

        /// <summary>
        /// Sets up this game object list for working with our editor
        /// </summary>
        public void Init(UpdateToolstripDelegate updateToolStripHandler, UpdateItemDelegate updateItemHandler, ToolstripButtonClickDelegate newDelegate, 
            ToolstripButtonClickDelegate copyDelegate, ToolstripButtonClickDelegate undoDelegate, ToolstripButtonClickDelegate pasteDelegate, ToolstripButtonClickDelegate deleteDelegate)
        {
            FocusChangedHandler = updateToolStripHandler;
            UpdateItemHandler = updateItemHandler;
            ToolStripItemNew_Click = newDelegate;
            ToolStripItemCopy_Click = copyDelegate;
            ToolStripItemPaste_Click = pasteDelegate;
            ToolStripItemUndo_Click = undoDelegate;
            ToolStripItemDelete_Click = deleteDelegate;
        }

        /// <summary>
        /// This function copies item ids to clipboards if right clicking. Otherwise it handles expanding/closing folders
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameObjectList_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var node = e.Node;
            if (node != null)
            {
                if (e.Button == MouseButtons.Right)
                {
                    if (e.Node.Tag != null && e.Node.Tag.GetType() == typeof(Guid))
                    {
                        Clipboard.SetText(e.Node.Tag.ToString());
                    }
                }

                var hitTest = HitTest(e.Location);
                if (hitTest.Location != TreeViewHitTestLocations.PlusMinus)
                {
                    if (node.Nodes.Count > 0)
                    {
                        if (node.IsExpanded)
                        {
                            node.Collapse();
                        }
                        else
                        {
                            node.Expand();
                        }
                    }
                }

                if (node.IsExpanded)
                {
                    if (!mExpandedFolders.Contains(node.Text))
                    {
                        mExpandedFolders.Add(node.Text);
                    }
                }
                else
                {
                    if (mExpandedFolders.Contains(node.Text))
                    {
                        mExpandedFolders.Remove(node.Text);
                    }
                }
            }
        }

        /// <summary>
        /// This function tells our editor that we selected a new item to edit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameObjectList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (mChangingName)
            {
                return;
            }

            if (SelectedNode == null || SelectedNode.Tag == null)
            {
                return;
            }

            UpdateItemHandler?.Invoke((Guid)SelectedNode.Tag);
        }


        private void GameObjectList_LostFocus(object sender, EventArgs e)
        {
            FocusChangedHandler?.Invoke();
        }


        private void GameObjectList_GotFocus(object sender, EventArgs e)
        {
            FocusChangedHandler?.Invoke();
        }

        private void GameObjectList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.Z)
                {
                    ToolStripItemUndo_Click?.Invoke(null, null);
                }
                else if (e.KeyCode == Keys.V)
                {
                    ToolStripItemPaste_Click?.Invoke(null, null);
                }
                else if (e.KeyCode == Keys.C)
                {
                    ToolStripItemCopy_Click?.Invoke(null, null);
                }
            }
            else
            {
                if (e.KeyCode == Keys.Delete)
                {
                    ToolStripItemDelete_Click?.Invoke(null, null);
                }
            }
        }


        /// <summary>
        /// Updates the name of the currently selected node
        /// </summary>
        /// <param name="text">New name</param>
        public void UpdateText(string text)
        {
            mChangingName = true;
            if (SelectedNode != null)
            {
                SelectedNode.Text = text;
            }
            mChangingName = false;
        }


        public void ExpandFolder(string name)
        {
            mExpandedFolders.Add(name);
        }

        public void ClearExpandedFolders()
        {
            mExpandedFolders.Clear();
        }

        public void Repopulate(KeyValuePair<Guid, KeyValuePair<string, string>>[] items, List<string> folders, bool chronological, bool customSearch, string search)
        {
            var selectedId = Guid.Empty;
            var folderNodes = new Dictionary<string, TreeNode>();
            if (SelectedNode != null && SelectedNode.Tag != null)
            {
                selectedId = (Guid)SelectedNode.Tag;
            }

            Nodes.Clear();

            Sorted = chronological;

            var nodes = new List<TreeNode>();
            TreeNode selectNode = null;

            if (!chronological && !customSearch)
            {
                foreach (var folder in folders)
                {
                    var node = new TreeNode(folder);
                    node.ImageIndex = 0;
                    node.SelectedImageIndex = 0;
                    folderNodes.Add(folder, node);
                    nodes.Add(node);
                }
            }

            foreach (var itm in items)
            {
                var node = new TreeNode(itm.Value.Key);
                node.Tag = itm.Key;
                node.ImageIndex = 1;
                node.SelectedImageIndex = 1;

                var folder = itm.Value.Value;
                if (!string.IsNullOrEmpty(folder) && !chronological && !customSearch)
                {
                    var folderNode = folderNodes[folder];
                    folderNode.Nodes.Add(node);
                    if (itm.Key == selectedId)
                    {
                        folderNode.Expand();
                    }
                }
                else
                {
                    nodes.Add(node);
                }

                if (customSearch)
                {
                    if (!node.Text.ToLower().Contains(search.ToLower()))
                    {
                        nodes.Remove(node);
                    }
                }

                if (itm.Key == selectedId)
                {
                    selectNode = node;
                }
            }

            foreach (var node in mExpandedFolders)
            {
                if (folderNodes.ContainsKey(node))
                {
                    folderNodes[node].Expand();
                }
            }

            Nodes.AddRange(nodes.ToArray());

            if (selectNode != null)
            {
                SelectedNode = selectNode;
            }
        }
    }
}
