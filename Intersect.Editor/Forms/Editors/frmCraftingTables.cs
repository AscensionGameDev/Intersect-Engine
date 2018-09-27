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

        public void InitEditor()
        {
            lstTables.Items.Clear();
            lstTables.Items.AddRange(CraftingTableBase.Names);
        }

        private void lstCrafts_Click(object sender, EventArgs e)
        {
            if (mChangingName) return;
            mEditorItem =
                CraftingTableBase.Get(
                    CraftingTableBase.IdFromList(lstTables.SelectedIndex));
            UpdateEditor();
        }

        private void UpdateEditor()
        {
            if (mEditorItem != null)
            {
                pnlContainer.Show();

                txtName.Text = mEditorItem.Name;

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
            if (lstTables.SelectedIndex > -1)
            {
                lstTables.Items[lstTables.SelectedIndex] = txtName.Text;
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
                if (DarkMessageBox.ShowWarning(Strings.CraftingTableEditor.deleteprompt,
                        Strings.CraftingTableEditor.delete, DarkDialogButton.YesNo,
                        Properties.Resources.Icon) ==
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
                mEditorItem.Load(mCopiedItem);
                UpdateEditor();
            }
        }

        private void toolStripItemUndo_Click(object sender, EventArgs e)
        {
            if (mChanged.Contains(mEditorItem) && mEditorItem != null)
            {
                if (DarkMessageBox.ShowWarning(Strings.CraftingTableEditor.undoprompt,
                        Strings.CraftingTableEditor.undotitle, DarkDialogButton.YesNo,
                        Properties.Resources.Icon) ==
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

            btnSave.Text = Strings.CraftingTableEditor.save;
            btnCancel.Text = Strings.CraftingTableEditor.cancel;
        }

        private void lstAvailableCrafts_SelectedValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Crafts.Clear();
            for (int i = 0; i < lstAvailableCrafts.Items.Count; i++)
            {
                if (lstAvailableCrafts.CheckedIndices.Contains(i))
                {
                    mEditorItem.Crafts.Add(CraftBase.IdFromList(i));
                }
            }
        }
    }
}