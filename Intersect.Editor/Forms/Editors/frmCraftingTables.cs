using System;
using System.Collections.Generic;
using System.Windows.Forms;

using DarkUI.Forms;

using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Editor.Networking;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Crafting;
using Intersect.Models;

namespace Intersect.Editor.Forms.Editors
{

    public partial class FrmCraftingTables : EditorForm
    {

        private List<CraftingTableBase> mChanged = new List<CraftingTableBase>();

        private string mCopiedItem;

        private CraftBase mCurrentCraft;

        private CraftingTableBase mEditorItem;

        private List<string> mExpandedFolders = new List<string>();

        private List<string> mKnownFolders = new List<string>();

        public FrmCraftingTables()
        {
            ApplyHooks();
            InitializeComponent();
            lstTables.LostFocus += itemList_FocusChanged;
            lstTables.GotFocus += itemList_FocusChanged;
        }

        protected override void GameObjectUpdatedDelegate(GameObjectType type)
        {
            if (type == GameObjectType.CraftTables)
            {
                InitEditor();
                if (mEditorItem != null && !DatabaseObject<CraftingTableBase>.Lookup.Values.Contains(mEditorItem))
                {
                    mEditorItem = null;
                    UpdateEditor();
                }
            }
        }

        private void UpdateEditor()
        {
            if (mEditorItem != null)
            {
                pnlContainer.Show();

                txtName.Text = mEditorItem.Name;

                cmbFolder.Text = mEditorItem.Folder;

                //Populate the checked list box
                lstAvailableCrafts.Items.Clear();
                lstAvailableCrafts.Items.AddRange(CraftBase.Names);

                //Clean up crafts array

                foreach (var val in mEditorItem.Crafts)
                {
                    var listIndex = CraftBase.ListIndex(val);
                    if (listIndex > -1)
                    {
                        lstAvailableCrafts.SetItemCheckState(listIndex, CheckState.Checked);
                    }
                }

                if (mChanged.IndexOf(mEditorItem) == -1)
                {
                    mChanged.Add(mEditorItem);
                    mEditorItem.MakeBackup();
                }
            }
            else
            {
                pnlContainer.Hide();
            }

            UpdateToolStripItems();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            mChangingName = true;
            mEditorItem.Name = txtName.Text;
            if (lstTables.SelectedNode != null && lstTables.SelectedNode.Tag != null)
            {
                lstTables.SelectedNode.Text = txtName.Text;
            }

            mChangingName = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            foreach (var item in mChanged)
            {
                item.RestoreBackup();
                item.DeleteBackup();
            }

            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //Send Changed items
            foreach (var item in mChanged)
            {
                PacketSender.SendSaveObject(item);
                item.DeleteBackup();
            }

            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }

        private void toolStripItemNew_Click(object sender, EventArgs e)
        {
            PacketSender.SendCreateObject(GameObjectType.CraftTables);
        }

        private void toolStripItemDelete_Click(object sender, EventArgs e)
        {
            if (mEditorItem != null && lstTables.Focused)
            {
                if (DarkMessageBox.ShowWarning(
                        Strings.CraftingTableEditor.deleteprompt, Strings.CraftingTableEditor.delete,
                        DarkDialogButton.YesNo, Properties.Resources.Icon
                    ) ==
                    DialogResult.Yes)
                {
                    PacketSender.SendDeleteObject(mEditorItem);
                }
            }
        }

        private void toolStripItemCopy_Click(object sender, EventArgs e)
        {
            if (mEditorItem != null && lstTables.Focused)
            {
                mCopiedItem = mEditorItem.JsonData;
                toolStripItemPaste.Enabled = true;
            }
        }

        private void toolStripItemPaste_Click(object sender, EventArgs e)
        {
            if (mEditorItem != null && mCopiedItem != null && lstTables.Focused)
            {
                mEditorItem.Load(mCopiedItem, true);
                UpdateEditor();
            }
        }

        private void toolStripItemUndo_Click(object sender, EventArgs e)
        {
            if (mChanged.Contains(mEditorItem) && mEditorItem != null)
            {
                if (DarkMessageBox.ShowWarning(
                        Strings.CraftingTableEditor.undoprompt, Strings.CraftingTableEditor.undotitle,
                        DarkDialogButton.YesNo, Properties.Resources.Icon
                    ) ==
                    DialogResult.Yes)
                {
                    mEditorItem.RestoreBackup();
                    UpdateEditor();
                }
            }
        }

        private void itemList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.Z)
                {
                    toolStripItemUndo_Click(null, null);
                }
                else if (e.KeyCode == Keys.V)
                {
                    toolStripItemPaste_Click(null, null);
                }
                else if (e.KeyCode == Keys.C)
                {
                    toolStripItemCopy_Click(null, null);
                }
            }
            else
            {
                if (e.KeyCode == Keys.Delete)
                {
                    toolStripItemDelete_Click(null, null);
                }
            }
        }

        private void UpdateToolStripItems()
        {
            toolStripItemCopy.Enabled = mEditorItem != null && lstTables.Focused;
            toolStripItemPaste.Enabled = mEditorItem != null && mCopiedItem != null && lstTables.Focused;
            toolStripItemDelete.Enabled = mEditorItem != null && lstTables.Focused;
            toolStripItemUndo.Enabled = mEditorItem != null && lstTables.Focused;
        }

        private void itemList_FocusChanged(object sender, EventArgs e)
        {
            UpdateToolStripItems();
        }

        private void form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.N)
                {
                    toolStripItemNew_Click(null, null);
                }
            }
        }

        private void frmCrafting_Load(object sender, EventArgs e)
        {
            InitLocalization();
        }

        private void InitLocalization()
        {
            Text = Strings.CraftingTableEditor.title;
            toolStripItemNew.Text = Strings.CraftingTableEditor.New;
            toolStripItemDelete.Text = Strings.CraftingTableEditor.delete;
            toolStripItemCopy.Text = Strings.CraftingTableEditor.copy;
            toolStripItemPaste.Text = Strings.CraftingTableEditor.paste;
            toolStripItemUndo.Text = Strings.CraftingTableEditor.undo;

            grpTables.Text = Strings.CraftingTableEditor.tables;
            grpCrafts.Text = Strings.CraftingTableEditor.crafts;

            grpGeneral.Text = Strings.CraftingTableEditor.general;
            lblName.Text = Strings.CraftingTableEditor.name;

            //Searching/Sorting
            btnChronological.ToolTipText = Strings.CraftingTableEditor.sortchronologically;
            txtSearch.Text = Strings.CraftingTableEditor.searchplaceholder;
            lblFolder.Text = Strings.CraftingTableEditor.folderlabel;

            btnSave.Text = Strings.CraftingTableEditor.save;
            btnCancel.Text = Strings.CraftingTableEditor.cancel;
        }

        private void lstAvailableCrafts_SelectedValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Crafts.Clear();
            for (var i = 0; i < lstAvailableCrafts.Items.Count; i++)
            {
                if (lstAvailableCrafts.CheckedIndices.Contains(i))
                {
                    mEditorItem.Crafts.Add(CraftBase.IdFromList(i));
                }
            }
        }

        #region "Item List - Folders, Searching, Sorting, Etc"

        public void InitEditor()
        {
            var selectedId = Guid.Empty;
            var folderNodes = new Dictionary<string, TreeNode>();
            if (lstTables.SelectedNode != null && lstTables.SelectedNode.Tag != null)
            {
                selectedId = (Guid) lstTables.SelectedNode.Tag;
            }

            lstTables.Nodes.Clear();

            //Collect folders
            var mFolders = new List<string>();
            foreach (var itm in CraftingTableBase.Lookup)
            {
                if (!string.IsNullOrEmpty(((CraftingTableBase) itm.Value).Folder) &&
                    !mFolders.Contains(((CraftingTableBase) itm.Value).Folder))
                {
                    mFolders.Add(((CraftingTableBase) itm.Value).Folder);
                    if (!mKnownFolders.Contains(((CraftingTableBase) itm.Value).Folder))
                    {
                        mKnownFolders.Add(((CraftingTableBase) itm.Value).Folder);
                    }
                }
            }

            mFolders.Sort();
            mKnownFolders.Sort();
            cmbFolder.Items.Clear();
            cmbFolder.Items.Add("");
            cmbFolder.Items.AddRange(mKnownFolders.ToArray());

            lstTables.Sorted = !btnChronological.Checked;

            if (!btnChronological.Checked && !CustomSearch())
            {
                foreach (var folder in mFolders)
                {
                    var node = lstTables.Nodes.Add(folder);
                    node.ImageIndex = 0;
                    node.SelectedImageIndex = 0;
                    folderNodes.Add(folder, node);
                }
            }

            foreach (var itm in CraftingTableBase.ItemPairs)
            {
                var node = new TreeNode(itm.Value);
                node.Tag = itm.Key;
                node.ImageIndex = 1;
                node.SelectedImageIndex = 1;

                var folder = CraftingTableBase.Get(itm.Key).Folder;
                if (!string.IsNullOrEmpty(folder) && !btnChronological.Checked && !CustomSearch())
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
                    lstTables.Nodes.Add(node);
                }

                if (CustomSearch())
                {
                    if (!node.Text.ToLower().Contains(txtSearch.Text.ToLower()))
                    {
                        node.Remove();
                    }
                }

                if (itm.Key == selectedId)
                {
                    lstTables.SelectedNode = node;
                }
            }

            var selectedNode = lstTables.SelectedNode;

            if (!btnChronological.Checked)
            {
                lstTables.Sort();
            }

            lstTables.SelectedNode = selectedNode;
            foreach (var node in mExpandedFolders)
            {
                if (folderNodes.ContainsKey(node))
                {
                    folderNodes[node].Expand();
                }
            }
        }

        private void btnAddFolder_Click(object sender, EventArgs e)
        {
            var folderName = "";
            var result = DarkInputBox.ShowInformation(
                Strings.CraftingTableEditor.folderprompt, Strings.CraftingTableEditor.foldertitle, ref folderName,
                DarkDialogButton.OkCancel
            );

            if (result == DialogResult.OK && !string.IsNullOrEmpty(folderName))
            {
                if (!cmbFolder.Items.Contains(folderName))
                {
                    mEditorItem.Folder = folderName;
                    mExpandedFolders.Add(folderName);
                    InitEditor();
                    cmbFolder.Text = folderName;
                }
            }
        }

        private void lstTables_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
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

                var hitTest = lstTables.HitTest(e.Location);
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

        private void lstTables_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (mChangingName)
            {
                return;
            }

            if (lstTables.SelectedNode == null || lstTables.SelectedNode.Tag == null)
            {
                return;
            }

            mEditorItem = CraftingTableBase.Get((Guid) lstTables.SelectedNode.Tag);
            UpdateEditor();
        }

        private void cmbFolder_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Folder = cmbFolder.Text;
            InitEditor();
        }

        private void btnChronological_Click(object sender, EventArgs e)
        {
            btnChronological.Checked = !btnChronological.Checked;
            InitEditor();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            InitEditor();
        }

        private void txtSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = Strings.CraftingTableEditor.searchplaceholder;
            }
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            txtSearch.SelectAll();
            txtSearch.Focus();
        }

        private void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = Strings.CraftingTableEditor.searchplaceholder;
        }

        private bool CustomSearch()
        {
            return !string.IsNullOrWhiteSpace(txtSearch.Text) &&
                   txtSearch.Text != Strings.CraftingTableEditor.searchplaceholder;
        }

        private void txtSearch_Click(object sender, EventArgs e)
        {
            if (txtSearch.Text == Strings.CraftingTableEditor.searchplaceholder)
            {
                txtSearch.SelectAll();
            }
        }

        #endregion

    }

}
