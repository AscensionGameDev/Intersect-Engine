using System;
using System.Collections.Generic;
using System.Linq;
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

        private CraftingTableBase mEditorItem;

        private List<string> mKnownFolders = new List<string>();

        public FrmCraftingTables()
        {
            ApplyHooks();
            InitializeComponent();

            lstGameObjects.Init(UpdateToolStripItems, AssignEditorItem, toolStripItemNew_Click, toolStripItemCopy_Click, toolStripItemUndo_Click, toolStripItemPaste_Click, toolStripItemDelete_Click);
        }
        private void AssignEditorItem(Guid id)
        {
            mEditorItem = CraftingTableBase.Get(id);
            UpdateEditor();
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
            mEditorItem.Name = txtName.Text;
            lstGameObjects.UpdateText(txtName.Text);
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
            if (mEditorItem != null && lstGameObjects.Focused)
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
            if (mEditorItem != null && lstGameObjects.Focused)
            {
                mCopiedItem = mEditorItem.JsonData;
                toolStripItemPaste.Enabled = true;
            }
        }

        private void toolStripItemPaste_Click(object sender, EventArgs e)
        {
            if (mEditorItem != null && mCopiedItem != null && lstGameObjects.Focused)
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

        private void UpdateToolStripItems()
        {
            toolStripItemCopy.Enabled = mEditorItem != null && lstGameObjects.Focused;
            toolStripItemPaste.Enabled = mEditorItem != null && mCopiedItem != null && lstGameObjects.Focused;
            toolStripItemDelete.Enabled = mEditorItem != null && lstGameObjects.Focused;
            toolStripItemUndo.Enabled = mEditorItem != null && lstGameObjects.Focused;
        }

        private void lstGameObjects_FocusChanged(object sender, EventArgs e)
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

            var items = CraftingTableBase.Lookup.OrderBy(p => p.Value?.TimeCreated).Select(pair => new KeyValuePair<Guid, KeyValuePair<string, string>>(pair.Key,
                new KeyValuePair<string, string>(((CraftingTableBase)pair.Value)?.Name ?? Models.DatabaseObject<CraftingTableBase>.Deleted, ((CraftingTableBase)pair.Value)?.Folder ?? ""))).ToArray();
            lstGameObjects.Repopulate(items, mFolders, btnChronological.Checked, CustomSearch(), txtSearch.Text);
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
                    lstGameObjects.ExpandFolder(folderName);
                    InitEditor();
                    cmbFolder.Text = folderName;
                }
            }
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
