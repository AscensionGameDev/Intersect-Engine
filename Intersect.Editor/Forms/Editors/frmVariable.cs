using System;
using System.Collections.Generic;
using System.Windows.Forms;

using DarkUI.Forms;

using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Editor.Networking;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Models;

namespace Intersect.Editor.Forms.Editors
{

    public partial class FrmSwitchVariable : EditorForm
    {

        private List<IDatabaseObject> mChanged = new List<IDatabaseObject>();

        private IDatabaseObject mEditorItem;

        private List<string> mExpandedFolders = new List<string>();

        private List<string> mGlobalExpandedFolders = new List<string>();

        private List<string> mGlobalKnownFolders = new List<string>();

        private List<string> mKnownFolders = new List<string>();

        public FrmSwitchVariable()
        {
            ApplyHooks();
            InitializeComponent();
            InitLocalization();
            nudVariableValue.Minimum = long.MinValue;
            nudVariableValue.Maximum = long.MaxValue;
        }

        private void InitLocalization()
        {
            Text = Strings.VariableEditor.title;
            grpTypes.Text = Strings.VariableEditor.type;
            grpList.Text = Strings.VariableEditor.list;
            rdoPlayerVariables.Text = Strings.VariableEditor.playervariables;
            rdoGlobalVariables.Text = Strings.VariableEditor.globalvariables;
            grpEditor.Text = Strings.VariableEditor.editor;
            lblName.Text = Strings.VariableEditor.name;
            grpValue.Text = Strings.VariableEditor.value;
            cmbBooleanValue.Items.Clear();
            cmbBooleanValue.Items.Add(Strings.VariableEditor.False);
            cmbBooleanValue.Items.Add(Strings.VariableEditor.True);
            cmbVariableType.Items.Clear();
            foreach (var itm in Strings.VariableEditor.types)
            {
                cmbVariableType.Items.Add(itm.Value);
            }

            toolStripItemNew.ToolTipText = Strings.VariableEditor.New;
            toolStripItemDelete.ToolTipText = Strings.VariableEditor.delete;
            toolStripItemUndo.ToolTipText = Strings.VariableEditor.undo;

            //Searching/Sorting
            btnChronological.ToolTipText = Strings.VariableEditor.sortchronologically;
            txtSearch.Text = Strings.VariableEditor.searchplaceholder;
            lblFolder.Text = Strings.VariableEditor.folderlabel;

            btnSave.Text = Strings.VariableEditor.save;
            btnCancel.Text = Strings.VariableEditor.cancel;
        }

        protected override void GameObjectUpdatedDelegate(GameObjectType type)
        {
            if (type == GameObjectType.PlayerVariable)
            {
                InitEditor();
                if (mEditorItem != null && !PlayerVariableBase.Lookup.Values.Contains(mEditorItem))
                {
                    mEditorItem = null;
                    UpdateEditor();
                }
            }
            else if (type == GameObjectType.ServerVariable)
            {
                InitEditor();
                if (mEditorItem != null && !ServerVariableBase.Lookup.Values.Contains(mEditorItem))
                {
                    mEditorItem = null;
                    UpdateEditor();
                }
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (rdoPlayerVariables.Checked)
            {
                PacketSender.SendCreateObject(GameObjectType.PlayerVariable);
            }
            else if (rdoGlobalVariables.Checked)
            {
                PacketSender.SendCreateObject(GameObjectType.ServerVariable);
            }
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            if (mChanged.Contains(mEditorItem) && mEditorItem != null)
            {
                mEditorItem.RestoreBackup();
                UpdateEditor();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (mEditorItem != null)
            {
                if (DarkMessageBox.ShowWarning(
                        Strings.VariableEditor.deleteprompt, Strings.VariableEditor.deletecaption,
                        DarkDialogButton.YesNo, Properties.Resources.Icon
                    ) ==
                    DialogResult.Yes)
                {
                    PacketSender.SendDeleteObject(mEditorItem);
                }
            }
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

        private void rdoPlayerVariables_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem = null;
            InitEditor();
        }

        private void rdoGlobalVariables_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem = null;
            InitEditor();
        }

        private void UpdateToolStripItems()
        {
            toolStripItemDelete.Enabled = mEditorItem != null && lstVariables.Focused;
            toolStripItemUndo.Enabled = mEditorItem != null && lstVariables.Focused;
        }

        private void UpdateEditor()
        {
            if (mEditorItem != null)
            {
                grpEditor.Show();
                grpValue.Hide();
                if (rdoPlayerVariables.Checked)
                {
                    lblObject.Text = Strings.VariableEditor.playervariable;
                    txtObjectName.Text = ((PlayerVariableBase) mEditorItem).Name;
                    txtId.Text = ((PlayerVariableBase) mEditorItem).TextId;
                    cmbFolder.Text = ((PlayerVariableBase) mEditorItem).Folder;
                    cmbVariableType.SelectedIndex = (int) (((PlayerVariableBase) mEditorItem).Type - 1);
                }
                else if (rdoGlobalVariables.Checked)
                {
                    lblObject.Text = Strings.VariableEditor.globalvariable;
                    txtObjectName.Text = ((ServerVariableBase) mEditorItem).Name;
                    txtId.Text = ((ServerVariableBase) mEditorItem).TextId;
                    cmbFolder.Text = ((ServerVariableBase) mEditorItem).Folder;
                    cmbVariableType.SelectedIndex = (int) (((ServerVariableBase) mEditorItem).Type - 1);
                    grpValue.Show();
                }

                InitValueGroup();
            }
            else
            {
                grpEditor.Hide();
            }

            UpdateToolStripItems();
        }

        private void UpdateSelection()
        {
            if (lstVariables.SelectedNode != null && lstVariables.SelectedNode.Tag != null)
            {
                grpEditor.Show();
                if (rdoPlayerVariables.Checked)
                {
                    var obj = PlayerVariableBase.Get((Guid) lstVariables.SelectedNode.Tag);
                    lstVariables.SelectedNode.Text = obj.Name;
                    grpValue.Hide();
                }
                else if (rdoGlobalVariables.Checked)
                {
                    var obj = ServerVariableBase.Get((Guid) lstVariables.SelectedNode.Tag);
                    lstVariables.SelectedNode.Text = obj.Name + " = " + obj.Value.ToString(obj.Type);
                }
            }
        }

        private void txtObjectName_TextChanged(object sender, EventArgs e)
        {
            if (lstVariables.SelectedNode != null && lstVariables.SelectedNode.Tag != null)
            {
                grpEditor.Show();
                grpValue.Hide();
                if (rdoPlayerVariables.Checked)
                {
                    var obj = PlayerVariableBase.Get((Guid) lstVariables.SelectedNode.Tag);
                    obj.Name = txtObjectName.Text;
                    lstVariables.SelectedNode.Text = obj.Name;
                }
                else if (rdoGlobalVariables.Checked)
                {
                    var obj = ServerVariableBase.Get((Guid) lstVariables.SelectedNode.Tag);
                    obj.Name = txtObjectName.Text;
                    lstVariables.SelectedNode.Text = obj.Name + " = " + obj.Value.ToString();
                }
            }
        }

        private void txtId_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = e.KeyChar == ' ';
        }

        private void txtId_TextChanged(object sender, EventArgs e)
        {
            if (lstVariables.SelectedNode != null && lstVariables.SelectedNode.Tag != null)
            {
                if (rdoPlayerVariables.Checked)
                {
                    var obj = PlayerVariableBase.Get((Guid) lstVariables.SelectedNode.Tag);
                    obj.TextId = txtId.Text;
                }
                else if (rdoGlobalVariables.Checked)
                {
                    var obj = ServerVariableBase.Get((Guid) lstVariables.SelectedNode.Tag);
                    obj.TextId = txtId.Text;
                }
            }
        }

        private void nudVariableValue_ValueChanged(object sender, EventArgs e)
        {
            if (lstVariables.SelectedNode != null && lstVariables.SelectedNode.Tag != null)
            {
                if (rdoGlobalVariables.Checked)
                {
                    var obj = ServerVariableBase.Get((Guid) lstVariables.SelectedNode.Tag);
                    if (obj != null)
                    {
                        obj.Value.Integer = (long) nudVariableValue.Value;
                        UpdateSelection();
                    }
                }
            }
        }

        private void cmbVariableType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstVariables.SelectedNode != null && lstVariables.SelectedNode.Tag != null)
            {
                if (rdoPlayerVariables.Checked)
                {
                    var obj = PlayerVariableBase.Get((Guid) lstVariables.SelectedNode.Tag);
                    obj.Type = (VariableDataTypes) (cmbVariableType.SelectedIndex + 1);
                }
                else if (rdoGlobalVariables.Checked)
                {
                    var obj = ServerVariableBase.Get((Guid) lstVariables.SelectedNode.Tag);
                    obj.Type = (VariableDataTypes) (cmbVariableType.SelectedIndex + 1);
                }

                InitValueGroup();
                UpdateSelection();
            }
        }

        private void InitValueGroup()
        {
            if (rdoPlayerVariables.Checked)
            {
                grpValue.Hide();
            }
            else
            {
                if (lstVariables.SelectedNode != null && lstVariables.SelectedNode.Tag != null)
                {
                    var obj = ServerVariableBase.Get((Guid) lstVariables.SelectedNode.Tag);
                    cmbBooleanValue.Hide();
                    nudVariableValue.Hide();
                    txtStringValue.Hide();
                    switch (obj.Type)
                    {
                        case VariableDataTypes.Boolean:
                            cmbBooleanValue.Show();
                            cmbBooleanValue.SelectedIndex = Convert.ToInt32(obj.Value.Boolean);

                            break;

                        case VariableDataTypes.Integer:
                            nudVariableValue.Show();
                            nudVariableValue.Value = obj.Value.Integer;

                            break;

                        case VariableDataTypes.Number:
                            break;

                        case VariableDataTypes.String:
                            txtStringValue.Show();
                            txtStringValue.Text = obj.Value.String;

                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        private void cmbBooleanValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstVariables.SelectedNode != null && lstVariables.SelectedNode.Tag != null)
            {
                if (rdoGlobalVariables.Checked)
                {
                    var obj = ServerVariableBase.Get((Guid) lstVariables.SelectedNode.Tag);
                    if (obj != null)
                    {
                        obj.Value.Boolean = Convert.ToBoolean(cmbBooleanValue.SelectedIndex);
                        UpdateSelection();
                    }
                }

                UpdateSelection();
            }
        }

        private void txtStringValue_TextChanged(object sender, EventArgs e)
        {
            if (lstVariables.SelectedNode != null && lstVariables.SelectedNode.Tag != null)
            {
                if (rdoGlobalVariables.Checked)
                {
                    var obj = ServerVariableBase.Get((Guid) lstVariables.SelectedNode.Tag);
                    if (obj != null)
                    {
                        obj.Value.String = txtStringValue.Text;
                        UpdateSelection();
                    }
                }
            }
        }

        #region "Item List - Folders, Searching, Sorting, Etc"

        public void InitEditor()
        {
            var selectedId = Guid.Empty;
            var folderNodes = new Dictionary<string, TreeNode>();
            if (lstVariables.SelectedNode != null && lstVariables.SelectedNode.Tag != null)
            {
                selectedId = (Guid) lstVariables.SelectedNode.Tag;
            }

            lstVariables.Nodes.Clear();

            //Fix Title
            if (rdoPlayerVariables.Checked)
            {
                grpVariables.Text = rdoPlayerVariables.Text;
            }
            else
            {
                grpVariables.Text = rdoGlobalVariables.Text;
            }

            grpEditor.Hide();
            cmbBooleanValue.Hide();
            nudVariableValue.Hide();
            txtStringValue.Hide();

            //Collect folders
            var mFolders = new List<string>();
            cmbFolder.Items.Clear();
            cmbFolder.Items.Add("");

            if (rdoPlayerVariables.Checked)
            {
                foreach (var itm in PlayerVariableBase.Lookup)
                {
                    if (!string.IsNullOrEmpty(((PlayerVariableBase) itm.Value).Folder) &&
                        !mFolders.Contains(((PlayerVariableBase) itm.Value).Folder))
                    {
                        mFolders.Add(((PlayerVariableBase) itm.Value).Folder);
                        if (!mKnownFolders.Contains(((PlayerVariableBase) itm.Value).Folder))
                        {
                            mKnownFolders.Add(((PlayerVariableBase) itm.Value).Folder);
                        }
                    }
                }

                mKnownFolders.Sort();
                cmbFolder.Items.AddRange(mKnownFolders.ToArray());
                lblId.Text = Strings.VariableEditor.textidpv;
            }
            else if (rdoGlobalVariables.Checked)
            {
                foreach (var itm in ServerVariableBase.Lookup)
                {
                    if (!string.IsNullOrEmpty(((ServerVariableBase) itm.Value).Folder) &&
                        !mFolders.Contains(((ServerVariableBase) itm.Value).Folder))
                    {
                        mFolders.Add(((ServerVariableBase) itm.Value).Folder);
                        if (!mGlobalKnownFolders.Contains(((ServerVariableBase) itm.Value).Folder))
                        {
                            mGlobalKnownFolders.Add(((ServerVariableBase) itm.Value).Folder);
                        }
                    }
                }

                mGlobalKnownFolders.Sort();
                cmbFolder.Items.AddRange(mGlobalKnownFolders.ToArray());
                lblId.Text = Strings.VariableEditor.textidgv;
            }

            mFolders.Sort();

            lstVariables.Sorted = !btnChronological.Checked;

            if (!btnChronological.Checked && !CustomSearch())
            {
                foreach (var folder in mFolders)
                {
                    var node = lstVariables.Nodes.Add(folder);
                    node.ImageIndex = 0;
                    node.SelectedImageIndex = 0;
                    folderNodes.Add(folder, node);
                }
            }

            var itemPairs = rdoPlayerVariables.Checked ? PlayerVariableBase.ItemPairs : ServerVariableBase.ItemPairs;

            foreach (var itm in itemPairs)
            {
                var node = new TreeNode(itm.Value);
                if (rdoGlobalVariables.Checked)
                {
                    node.Text = node.Text +
                                " = " +
                                ServerVariableBase.Get(itm.Key).Value.ToString(ServerVariableBase.Get(itm.Key).Type);
                }

                node.Tag = itm.Key;
                node.ImageIndex = 1;
                node.SelectedImageIndex = 1;

                var folder = "";
                if (rdoPlayerVariables.Checked)
                {
                    folder = PlayerVariableBase.Get(itm.Key).Folder;
                }

                if (rdoGlobalVariables.Checked)
                {
                    folder = ServerVariableBase.Get(itm.Key).Folder;
                }

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
                    lstVariables.Nodes.Add(node);
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
                    lstVariables.SelectedNode = node;
                }
            }

            var selectedNode = lstVariables.SelectedNode;

            if (!btnChronological.Checked)
            {
                lstVariables.Sort();
            }

            lstVariables.SelectedNode = selectedNode;
            var expandedFolders = mExpandedFolders;
            if (rdoGlobalVariables.Checked)
            {
                expandedFolders = mGlobalExpandedFolders;
            }

            foreach (var node in expandedFolders)
            {
                if (folderNodes.ContainsKey(node))
                {
                    folderNodes[node].Expand();
                }
            }

            UpdateEditor();
        }

        private void btnAddFolder_Click(object sender, EventArgs e)
        {
            var folderName = "";
            var result = DarkInputBox.ShowInformation(
                Strings.VariableEditor.folderprompt, Strings.VariableEditor.foldertitle, ref folderName,
                DarkDialogButton.OkCancel
            );

            if (result == DialogResult.OK && !string.IsNullOrEmpty(folderName))
            {
                if (!cmbFolder.Items.Contains(folderName))
                {
                    if (lstVariables.SelectedNode != null && lstVariables.SelectedNode.Tag != null)
                    {
                        if (rdoPlayerVariables.Checked)
                        {
                            var obj = PlayerVariableBase.Get((Guid) lstVariables.SelectedNode.Tag);
                            obj.Folder = folderName;
                            mExpandedFolders.Add(folderName);
                        }
                        else if (rdoGlobalVariables.Checked)
                        {
                            var obj = ServerVariableBase.Get((Guid) lstVariables.SelectedNode.Tag);
                            obj.Folder = folderName;
                            mGlobalExpandedFolders.Add(folderName);
                        }

                        InitEditor();
                        cmbFolder.Text = folderName;
                    }
                }
            }
        }

        private void lstVariables_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
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

                var hitTest = lstVariables.HitTest(e.Location);
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
                    if (rdoPlayerVariables.Checked && !mExpandedFolders.Contains(node.Text))
                    {
                        mExpandedFolders.Add(node.Text);
                    }

                    if (rdoGlobalVariables.Checked && !mGlobalExpandedFolders.Contains(node.Text))
                    {
                        mGlobalExpandedFolders.Add(node.Text);
                    }
                }
                else
                {
                    if (rdoPlayerVariables.Checked && mExpandedFolders.Contains(node.Text))
                    {
                        mExpandedFolders.Remove(node.Text);
                    }

                    if (rdoGlobalVariables.Checked && mGlobalExpandedFolders.Contains(node.Text))
                    {
                        mGlobalExpandedFolders.Remove(node.Text);
                    }
                }
            }
        }

        private void lstVariables_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (mChangingName)
            {
                return;
            }

            if (lstVariables.SelectedNode == null || lstVariables.SelectedNode.Tag == null)
            {
                mEditorItem = null;
                UpdateEditor();

                return;
            }

            var id = (Guid) lstVariables.SelectedNode.Tag;
            if (id != Guid.Empty)
            {
                IDatabaseObject obj = null;
                if (rdoPlayerVariables.Checked)
                {
                    obj = PlayerVariableBase.Get(id);
                }
                else if (rdoGlobalVariables.Checked)
                {
                    obj = ServerVariableBase.Get(id);
                }

                if (obj != null)
                {
                    mEditorItem = obj;
                    if (!mChanged.Contains(obj))
                    {
                        mChanged.Add(obj);
                        obj.MakeBackup();
                    }
                }
            }

            UpdateEditor();
        }

        private void cmbFolder_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstVariables.SelectedNode != null && lstVariables.SelectedNode.Tag != null)
            {
                if (rdoPlayerVariables.Checked)
                {
                    var obj = PlayerVariableBase.Get((Guid) lstVariables.SelectedNode.Tag);
                    obj.Folder = cmbFolder.Text;
                }
                else if (rdoGlobalVariables.Checked)
                {
                    var obj = ServerVariableBase.Get((Guid) lstVariables.SelectedNode.Tag);
                    obj.Folder = cmbFolder.Text;
                }

                InitEditor();
            }
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
                txtSearch.Text = Strings.VariableEditor.searchplaceholder;
            }
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            txtSearch.SelectAll();
            txtSearch.Focus();
        }

        private void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = Strings.VariableEditor.searchplaceholder;
        }

        private bool CustomSearch()
        {
            return !string.IsNullOrWhiteSpace(txtSearch.Text) &&
                   txtSearch.Text != Strings.VariableEditor.searchplaceholder;
        }

        private void txtSearch_Click(object sender, EventArgs e)
        {
            if (txtSearch.Text == Strings.VariableEditor.searchplaceholder)
            {
                txtSearch.SelectAll();
            }
        }

        #endregion

    }

}
