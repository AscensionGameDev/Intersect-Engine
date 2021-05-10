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

    public partial class FrmCrafts : EditorForm
    {

        private List<CraftBase> mChanged = new List<CraftBase>();

        private string mCopiedItem;

        private CraftBase mEditorItem;

        private List<string> mKnownFolders = new List<string>();

        private bool updatingIngedients = false;

        public FrmCrafts()
        {
            ApplyHooks();
            InitializeComponent();
            lstGameObjects.LostFocus += itemList_FocusChanged;
            lstGameObjects.GotFocus += itemList_FocusChanged;
            cmbResult.Items.Clear();
            cmbResult.Items.Add(Strings.General.none);
            cmbResult.Items.AddRange(ItemBase.Names);
            cmbIngredient.Items.Clear();
            cmbIngredient.Items.Add(Strings.General.none);
            cmbIngredient.Items.AddRange(ItemBase.Names);

            lstGameObjects.Init(UpdateToolStripItems, AssignEditorItem, toolStripItemNew_Click, toolStripItemCopy_Click, toolStripItemUndo_Click, toolStripItemPaste_Click, toolStripItemDelete_Click);
        }
        private void AssignEditorItem(Guid id)
        {
            mEditorItem = CraftBase.Get(id);
            UpdateEditor();
        }

        protected override void GameObjectUpdatedDelegate(GameObjectType type)
        {
            if (type == GameObjectType.Crafts)
            {
                InitEditor();
                if (mEditorItem != null && !DatabaseObject<CraftBase>.Lookup.Values.Contains(mEditorItem))
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

                //Populate ingredients and such
                nudSpeed.Value = mEditorItem.Time;
                cmbResult.SelectedIndex = ItemBase.ListIndex(mEditorItem.ItemId) + 1;

                nudCraftQuantity.Value = mEditorItem.Quantity;

                lstIngredients.Items.Clear();
                cmbIngredient.Hide();
                nudQuantity.Hide();
                lblQuantity.Hide();
                lblIngredient.Hide();
                for (var i = 0; i < mEditorItem.Ingredients.Count; i++)
                {
                    if (mEditorItem.Ingredients[i].ItemId != Guid.Empty)
                    {
                        lstIngredients.Items.Add(
                            Strings.CraftsEditor.ingredientlistitem.ToString(
                                ItemBase.GetName(mEditorItem.Ingredients[i].ItemId), mEditorItem.Ingredients[i].Quantity
                            )
                        );
                    }
                    else
                    {
                        lstIngredients.Items.Add(
                            Strings.CraftsEditor.ingredientlistitem.ToString(
                                Strings.CraftsEditor.ingredientnone, mEditorItem.Ingredients[i].Quantity
                            )
                        );
                    }
                }

                if (lstIngredients.Items.Count > 0)
                {
                    lstIngredients.SelectedIndex = 0;
                    cmbIngredient.SelectedIndex =
                        ItemBase.ListIndex(mEditorItem.Ingredients[lstIngredients.SelectedIndex].ItemId) + 1;

                    nudQuantity.Value = mEditorItem.Ingredients[lstIngredients.SelectedIndex].Quantity;
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

        private void nudQuantity_ValueChanged(object sender, EventArgs e)
        {
            // This should never be below 1. We shouldn't accept giving 0 items!
            nudQuantity.Value = Math.Max(1, nudQuantity.Value);

            if (lstIngredients.SelectedIndex > -1)
            {
                mEditorItem.Ingredients[lstIngredients.SelectedIndex].Quantity = (int) nudQuantity.Value;
                updatingIngedients = true;
                if (cmbIngredient.SelectedIndex > 0)
                {
                    lstIngredients.Items[lstIngredients.SelectedIndex] =
                        Strings.CraftsEditor.ingredientlistitem.ToString(
                            ItemBase.GetName(mEditorItem.Ingredients[lstIngredients.SelectedIndex].ItemId),
                            nudQuantity.Value
                        );
                }
                else
                {
                    lstIngredients.Items[lstIngredients.SelectedIndex] =
                        Strings.CraftsEditor.ingredientlistitem.ToString(
                            Strings.CraftsEditor.ingredientnone, nudQuantity.Value
                        );
                }

                updatingIngedients = false;
            }
        }

        private void nudSpeed_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Time = (int) nudSpeed.Value;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            mEditorItem.Ingredients.Add(new CraftIngredient(Guid.Empty, 1));
            lstIngredients.Items.Add(Strings.General.none);
            lstIngredients.SelectedIndex = lstIngredients.Items.Count - 1;
            cmbIngredient_SelectedIndexChanged(null, null);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lstIngredients.Items.Count > 0)
            {
                mEditorItem.Ingredients.RemoveAt(lstIngredients.SelectedIndex);
                UpdateEditor();
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

        private void toolStripItemNew_Click(object sender, EventArgs e)
        {
            PacketSender.SendCreateObject(GameObjectType.Crafts);
        }

        private void toolStripItemDelete_Click(object sender, EventArgs e)
        {
            if (mEditorItem != null && lstGameObjects.Focused)
            {
                if (DarkMessageBox.ShowWarning(
                        Strings.CraftsEditor.deleteprompt, Strings.CraftsEditor.deletetitle, DarkDialogButton.YesNo,
                        Properties.Resources.Icon
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
                        Strings.CraftsEditor.undoprompt, Strings.CraftsEditor.undotitle, DarkDialogButton.YesNo,
                        Properties.Resources.Icon
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
            toolStripItemCopy.Enabled = mEditorItem != null && lstGameObjects.Focused;
            toolStripItemPaste.Enabled = mEditorItem != null && mCopiedItem != null && lstGameObjects.Focused;
            toolStripItemDelete.Enabled = mEditorItem != null && lstGameObjects.Focused;
            toolStripItemUndo.Enabled = mEditorItem != null && lstGameObjects.Focused;
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

        private void lstIngredients_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingIngedients)
            {
                return;
            }

            if (lstIngredients.SelectedIndex > -1)
            {
                cmbIngredient.Show();
                nudQuantity.Show();
                lblQuantity.Show();
                lblIngredient.Show();
                cmbIngredient.SelectedIndex =
                    ItemBase.ListIndex(mEditorItem.Ingredients[lstIngredients.SelectedIndex].ItemId) + 1;

                nudQuantity.Value = mEditorItem.Ingredients[lstIngredients.SelectedIndex].Quantity;
            }
            else
            {
                cmbIngredient.Hide();
                nudQuantity.Hide();
                lblQuantity.Hide();
                lblIngredient.Hide();
            }
        }

        private void btnDupIngredient_Click(object sender, EventArgs e)
        {
            if (lstIngredients.SelectedIndex > -1)
            {
                mEditorItem.Ingredients.Insert(
                    lstIngredients.SelectedIndex,
                    new CraftIngredient(
                        mEditorItem.Ingredients[lstIngredients.SelectedIndex].ItemId,
                        mEditorItem.Ingredients[lstIngredients.SelectedIndex].Quantity
                    )
                );

                UpdateEditor();
            }
        }

        private void cmbResult_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.ItemId = ItemBase.IdFromList(cmbResult.SelectedIndex - 1);
            var itm = ItemBase.Get(mEditorItem.ItemId);
            if (itm == null || !itm.IsStackable)
            {
                nudCraftQuantity.Value = 1;
                mEditorItem.Quantity = 1;
                nudCraftQuantity.Enabled = false;
            }
            else
            {
                nudCraftQuantity.Enabled = true;
            }
        }

        private void cmbIngredient_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstIngredients.SelectedIndex > -1)
            {
                mEditorItem.Ingredients[lstIngredients.SelectedIndex].ItemId =
                    ItemBase.IdFromList(cmbIngredient.SelectedIndex - 1);

                updatingIngedients = true;
                if (cmbIngredient.SelectedIndex > 0)
                {
                    lstIngredients.Items[lstIngredients.SelectedIndex] =
                        Strings.CraftsEditor.ingredientlistitem.ToString(
                            ItemBase.GetName(mEditorItem.Ingredients[lstIngredients.SelectedIndex].ItemId),
                            nudQuantity.Value
                        );
                }
                else
                {
                    lstIngredients.Items[lstIngredients.SelectedIndex] =
                        Strings.CraftsEditor.ingredientlistitem.ToString(
                            Strings.CraftsEditor.ingredientnone, nudQuantity.Value
                        );
                }

                updatingIngedients = false;
            }
        }

        private void frmCrafting_Load(object sender, EventArgs e)
        {
            InitLocalization();
        }

        private void InitLocalization()
        {
            Text = Strings.CraftsEditor.title;
            toolStripItemNew.Text = Strings.CraftsEditor.New;
            toolStripItemDelete.Text = Strings.CraftsEditor.delete;
            toolStripItemCopy.Text = Strings.CraftsEditor.copy;
            toolStripItemPaste.Text = Strings.CraftsEditor.paste;
            toolStripItemUndo.Text = Strings.CraftsEditor.undo;

            grpCrafts.Text = Strings.CraftsEditor.crafts;

            grpGeneral.Text = Strings.CraftsEditor.general;
            lblName.Text = Strings.CraftsEditor.name;
            lblItem.Text = Strings.CraftsEditor.item;
            lblCraftQuantity.Text = Strings.CraftsEditor.craftquantity;
            lblSpeed.Text = Strings.CraftsEditor.time;

            grpIngredients.Text = Strings.CraftsEditor.ingredients;
            lblIngredient.Text = Strings.CraftsEditor.ingredientitem;
            lblQuantity.Text = Strings.CraftsEditor.ingredientquantity;
            btnAdd.Text = Strings.CraftsEditor.newingredient;
            btnRemove.Text = Strings.CraftsEditor.deleteingredient;
            btnDupIngredient.Text = Strings.CraftsEditor.duplicateingredient;

            //Searching/Sorting
            btnChronological.ToolTipText = Strings.CraftsEditor.sortchronologically;
            txtSearch.Text = Strings.CraftsEditor.searchplaceholder;
            lblFolder.Text = Strings.CraftsEditor.folderlabel;

            btnSave.Text = Strings.CraftsEditor.save;
            btnCancel.Text = Strings.CraftsEditor.cancel;
        }

        private void nudCraftQuantity_ValueChanged(object sender, EventArgs e)
        {
            // This should never be below 1. We shouldn't accept giving 0 items!
            nudCraftQuantity.Value = Math.Max(1, nudCraftQuantity.Value);
            mEditorItem.Quantity = (int) nudCraftQuantity.Value;
        }

        #region "Item List - Folders, Searching, Sorting, Etc"

        public void InitEditor()
        {
            //Collect folders
            var mFolders = new List<string>();
            foreach (var itm in CraftBase.Lookup)
            {
                if (!string.IsNullOrEmpty(((CraftBase) itm.Value).Folder) &&
                    !mFolders.Contains(((CraftBase) itm.Value).Folder))
                {
                    mFolders.Add(((CraftBase) itm.Value).Folder);
                    if (!mKnownFolders.Contains(((CraftBase) itm.Value).Folder))
                    {
                        mKnownFolders.Add(((CraftBase) itm.Value).Folder);
                    }
                }
            }

            mFolders.Sort();
            mKnownFolders.Sort();
            cmbFolder.Items.Clear();
            cmbFolder.Items.Add("");
            cmbFolder.Items.AddRange(mKnownFolders.ToArray());

            var items = CraftBase.Lookup.OrderBy(p => p.Value?.TimeCreated).Select(pair => new KeyValuePair<Guid, KeyValuePair<string, string>>(pair.Key,
                new KeyValuePair<string, string>(((CraftBase)pair.Value)?.Name ?? Models.DatabaseObject<CraftBase>.Deleted, ((CraftBase)pair.Value)?.Folder ?? ""))).ToArray();
            lstGameObjects.Repopulate(items, mFolders, btnChronological.Checked, CustomSearch(), txtSearch.Text);
        }

        private void btnAddFolder_Click(object sender, EventArgs e)
        {
            var folderName = "";
            var result = DarkInputBox.ShowInformation(
                Strings.CraftsEditor.folderprompt, Strings.CraftsEditor.foldertitle, ref folderName,
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
                txtSearch.Text = Strings.CraftsEditor.searchplaceholder;
            }
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            txtSearch.SelectAll();
            txtSearch.Focus();
        }

        private void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = Strings.CraftsEditor.searchplaceholder;
        }

        private bool CustomSearch()
        {
            return !string.IsNullOrWhiteSpace(txtSearch.Text) &&
                   txtSearch.Text != Strings.CraftsEditor.searchplaceholder;
        }

        private void txtSearch_Click(object sender, EventArgs e)
        {
            if (txtSearch.Text == Strings.CraftsEditor.searchplaceholder)
            {
                txtSearch.SelectAll();
            }
        }

        #endregion

    }

}
