using System;
using System.Collections.Generic;
using System.Linq;
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

        private List<string> mKnownFolders = new List<string>();

        public FrmCommonEvent()
        {
            ApplyHooks();
            InitializeComponent();
            InitLocalization();
            InitEditor();

            lstGameObjects.NodeMouseDoubleClick += lstGameObjects_NodeMouseDoubleClick;
            lstGameObjects.AfterSelect += lstGameObjects_AfterSelect;
            lstGameObjects.Init(null, null, toolStripItemNew_Click, toolStripItemCopy_Click, null, toolStripItemPaste_Click, toolStripItemDelete_Click);
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

        private void toolStripItemNew_Click(object sender, EventArgs e)
        {
            PacketSender.SendCreateObject(GameObjectType.Event);
        }

        private void toolStripItemDelete_Click(object sender, EventArgs e)
        {
            if (lstGameObjects.SelectedNode?.Tag != null &&
                EventBase.Get((Guid) lstGameObjects.SelectedNode.Tag) != null)
            {
                if (MessageBox.Show(
                        Strings.CommonEventEditor.deleteprompt, Strings.CommonEventEditor.delete,
                        MessageBoxButtons.YesNo
                    ) ==
                    DialogResult.Yes)
                {
                    PacketSender.SendDeleteObject(EventBase.Get((Guid) lstGameObjects.SelectedNode.Tag));
                }
            }
        }

        private void InitEditor()
        {
            //Collect folders
            var mFolders = new List<string>();
            foreach (var itm in EventBase.Lookup)
            {
                if (((EventBase)itm.Value).CommonEvent)
                {
                    if (!string.IsNullOrEmpty(((EventBase)itm.Value).Folder) &&
                        !mFolders.Contains(((EventBase)itm.Value).Folder))
                    {
                        mFolders.Add(((EventBase)itm.Value).Folder);
                        if (!mKnownFolders.Contains(((EventBase)itm.Value).Folder))
                        {
                            mKnownFolders.Add(((EventBase)itm.Value).Folder);
                        }
                    }
                }
            }

            mFolders.Sort();
            mKnownFolders.Sort();
            cmbFolder.Items.Clear();
            cmbFolder.Items.Add("");
            cmbFolder.Items.AddRange(mKnownFolders.ToArray());

            var items = EventBase.Lookup.Where(pair => ((EventBase)pair.Value)?.CommonEvent ?? false).OrderBy(p => p.Value?.TimeCreated).Select(pair => new KeyValuePair<Guid, KeyValuePair<string, string>>(pair.Key,
                new KeyValuePair<string, string>(((EventBase)pair.Value)?.Name ?? Models.DatabaseObject<EventBase>.Deleted, ((EventBase)pair.Value)?.Folder ?? ""))).ToArray();
            lstGameObjects.Repopulate(items, mFolders, btnChronological.Checked, CustomSearch(), txtSearch.Text);
        }

        private void frmCommonEvent_FormClosed(object sender, FormClosedEventArgs e)
        {
            Globals.CurrentEditor = -1;
        }

        private void toolStripItemCopy_Click(object sender, EventArgs e)
        {
            var evt = GetSelectedEvent();
            if (evt != null && lstGameObjects.Focused)
            {
                mCopiedItem = evt.JsonData;
                toolStripItemPaste.Enabled = true;
            }
        }

        private void toolStripItemPaste_Click(object sender, EventArgs e)
        {
            var evt = GetSelectedEvent();
            if (evt != null && mCopiedItem != null && lstGameObjects.Focused)
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
            if (lstGameObjects.SelectedNode == null || lstGameObjects.SelectedNode.Tag == null)
            {
                return null;
            }

            return EventBase.Get((Guid) lstGameObjects.SelectedNode.Tag);
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

                    lstGameObjects.ExpandFolder(folderName);
                    InitEditor();
                    cmbFolder.Text = folderName;
                }
            }
        }

        private void lstGameObjects_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (lstGameObjects.SelectedNode == null || lstGameObjects.SelectedNode.Tag == null)
            {
                return;
            }

            var editor = new FrmEvent(null)
            {
                MyEvent = EventBase.Get((Guid) lstGameObjects.SelectedNode.Tag)
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

        private void lstGameObjects_AfterSelect(object sender, EventArgs e)
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
