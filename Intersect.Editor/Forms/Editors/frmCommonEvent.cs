using System;
using System.Collections.Generic;
using System.Windows.Forms;

using DarkUI.Forms;

using Intersect.Editor.Forms.Editors.Events;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Editor.Networking;
using Intersect.Enums;
using Intersect.GameObjects.Events;

namespace Intersect.Editor.Forms.Editors
{

    public partial class FrmCommonEvent : EditorForm
    {

        private string mCopiedItem;

        private List<string> mExpandedFolders = new List<string>();

        private List<string> mKnownFolders = new List<string>();

        public FrmCommonEvent()
        {
            ApplyHooks();
            InitializeComponent();
            InitLocalization();
            InitEditor();
        }

        private void InitLocalization()
        {
            Text = Strings.CommonEventEditor.title;
            grpCommonEvents.Text = Strings.CommonEventEditor.events;
            toolStripItemNew.Text = Strings.CommonEventEditor.New;
            toolStripItemDelete.Text = Strings.CommonEventEditor.delete;
            toolStripItemCopy.Text = Strings.CommonEventEditor.copy;
            toolStripItemPaste.Text = Strings.CommonEventEditor.paste;

            //Searching/Sorting
            btnChronological.ToolTipText = Strings.CommonEventEditor.sortchronologically;
            txtSearch.Text = Strings.CommonEventEditor.searchplaceholder;
            lblFolder.Text = Strings.CommonEventEditor.folderlabel;
        }

        protected override void GameObjectUpdatedDelegate(GameObjectType type)
        {
            if (type == GameObjectType.Event)
            {
                InitEditor();
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            PacketSender.SendCreateObject(GameObjectType.Event);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstCommonEvents.SelectedNode?.Tag != null &&
                EventBase.Get((Guid) lstCommonEvents.SelectedNode.Tag) != null)
            {
                if (MessageBox.Show(
                        Strings.CommonEventEditor.deleteprompt, Strings.CommonEventEditor.delete,
                        MessageBoxButtons.YesNo
                    ) ==
                    DialogResult.Yes)
                {
                    PacketSender.SendDeleteObject(EventBase.Get((Guid) lstCommonEvents.SelectedNode.Tag));
                }
            }
        }

        private void InitEditor()
        {
            var selectedId = Guid.Empty;
            var folderNodes = new Dictionary<string, TreeNode>();
            if (lstCommonEvents.SelectedNode != null && lstCommonEvents.SelectedNode.Tag != null)
            {
                selectedId = (Guid) lstCommonEvents.SelectedNode.Tag;
            }

            lstCommonEvents.Nodes.Clear();

            //Collect folders
            var mFolders = new List<string>();
            foreach (var itm in EventBase.Lookup)
            {
                if (((EventBase) itm.Value).CommonEvent)
                {
                    if (!string.IsNullOrEmpty(((EventBase) itm.Value).Folder) &&
                        !mFolders.Contains(((EventBase) itm.Value).Folder))
                    {
                        mFolders.Add(((EventBase) itm.Value).Folder);
                        if (!mKnownFolders.Contains(((EventBase) itm.Value).Folder))
                        {
                            mKnownFolders.Add(((EventBase) itm.Value).Folder);
                        }
                    }
                }
            }

            mFolders.Sort();
            mKnownFolders.Sort();
            cmbFolder.Items.Clear();
            cmbFolder.Items.Add("");
            cmbFolder.Items.AddRange(mKnownFolders.ToArray());

            lstCommonEvents.Sorted = !btnChronological.Checked;

            if (!btnChronological.Checked && !CustomSearch())
            {
                foreach (var folder in mFolders)
                {
                    var node = lstCommonEvents.Nodes.Add(folder);
                    node.ImageIndex = 0;
                    node.SelectedImageIndex = 0;
                    folderNodes.Add(folder, node);
                }
            }

            foreach (var itm in EventBase.ItemPairs)
            {
                var node = new TreeNode(itm.Value);
                node.Tag = itm.Key;
                node.ImageIndex = 1;
                node.SelectedImageIndex = 1;

                var folder = EventBase.Get(itm.Key).Folder;
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
                    lstCommonEvents.Nodes.Add(node);
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
                    lstCommonEvents.SelectedNode = node;
                }
            }

            var selectedNode = lstCommonEvents.SelectedNode;

            if (!btnChronological.Checked)
            {
                lstCommonEvents.Sort();
            }

            lstCommonEvents.SelectedNode = selectedNode;
            foreach (var node in mExpandedFolders)
            {
                if (folderNodes.ContainsKey(node))
                {
                    folderNodes[node].Expand();
                }
            }
        }

        private void frmCommonEvent_FormClosed(object sender, FormClosedEventArgs e)
        {
            Globals.CurrentEditor = -1;
        }

        private void toolStripItemCopy_Click(object sender, EventArgs e)
        {
            var evt = GetSelectedEvent();
            if (evt != null && lstCommonEvents.Focused)
            {
                mCopiedItem = evt.JsonData;
                toolStripItemPaste.Enabled = true;
            }
        }

        private void toolStripItemPaste_Click(object sender, EventArgs e)
        {
            var evt = GetSelectedEvent();
            if (evt != null && mCopiedItem != null && lstCommonEvents.Focused)
            {
                if (MessageBox.Show(
                        Strings.CommonEventEditor.pasteprompt, Strings.CommonEventEditor.pastetitle,
                        MessageBoxButtons.YesNo
                    ) ==
                    DialogResult.Yes)
                {
                    evt.Load(mCopiedItem, true);
                    PacketSender.SendSaveObject(evt);
                    InitEditor();
                }
            }
        }

        #region "Item List - Folders, Searching, Sorting, Etc"

        private EventBase GetSelectedEvent()
        {
            if (lstCommonEvents.SelectedNode == null || lstCommonEvents.SelectedNode.Tag == null)
            {
                return null;
            }

            return EventBase.Get((Guid) lstCommonEvents.SelectedNode.Tag);
        }

        private void btnAddFolder_Click(object sender, EventArgs e)
        {
            var folderName = "";
            var result = DarkInputBox.ShowInformation(
                Strings.CommonEventEditor.folderprompt, Strings.CommonEventEditor.foldertitle, ref folderName,
                DarkDialogButton.OkCancel
            );

            if (result == DialogResult.OK && !string.IsNullOrEmpty(folderName))
            {
                if (!cmbFolder.Items.Contains(folderName))
                {
                    var evt = GetSelectedEvent();
                    if (evt != null)
                    {
                        evt.Folder = folderName;
                        PacketSender.SendSaveObject(evt);
                    }

                    mExpandedFolders.Add(folderName);
                    InitEditor();
                    cmbFolder.Text = folderName;
                }
            }
        }

        private void lstCommonEvents_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
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

                var hitTest = lstCommonEvents.HitTest(e.Location);
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

        private void lstCommonEvents_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (lstCommonEvents.SelectedNode == null || lstCommonEvents.SelectedNode.Tag == null)
            {
                return;
            }

            var editor = new FrmEvent(null)
            {
                MyEvent = EventBase.Get((Guid) lstCommonEvents.SelectedNode.Tag)
            };

            editor.InitEditor(false, false, false);
            editor.ShowDialog();
            InitEditor();
            Globals.MainForm.BringToFront();
            BringToFront();
        }

        private void cmbFolder_SelectedIndexChanged(object sender, EventArgs e)
        {
            var evt = GetSelectedEvent();
            if (evt != null)
            {
                evt.Folder = cmbFolder.Text;
                PacketSender.SendSaveObject(evt);
            }

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
                txtSearch.Text = Strings.CommonEventEditor.searchplaceholder;
            }
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            txtSearch.SelectAll();
            txtSearch.Focus();
        }

        private void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = Strings.CommonEventEditor.searchplaceholder;
        }

        private bool CustomSearch()
        {
            return !string.IsNullOrWhiteSpace(txtSearch.Text) &&
                   txtSearch.Text != Strings.CommonEventEditor.searchplaceholder;
        }

        private void txtSearch_Click(object sender, EventArgs e)
        {
            if (txtSearch.Text == Strings.CommonEventEditor.searchplaceholder)
            {
                txtSearch.SelectAll();
            }
        }

        private void lstCommonEvents_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var evt = GetSelectedEvent();
            toolStripItemDelete.Enabled = false;
            toolStripItemCopy.Enabled = false;
            toolStripItemPaste.Enabled = false;
            cmbFolder.Hide();
            btnAddFolder.Hide();
            lblFolder.Hide();
            if (evt != null)
            {
                cmbFolder.Text = evt.Folder;
                toolStripItemDelete.Enabled = true;
                toolStripItemCopy.Enabled = true;
                toolStripItemPaste.Enabled = mCopiedItem != null;
                cmbFolder.Show();
                btnAddFolder.Show();
                lblFolder.Show();
            }
        }

        #endregion

    }

}
