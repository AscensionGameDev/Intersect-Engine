using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DarkUI.Forms;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Editor.Networking;
using Intersect.GameObjects.Crafting;
using Intersect.Models;

namespace Intersect.Editor.Forms.Editors
{
    public partial class FrmCrafts : EditorForm
    {
        private List<CraftBase> mChanged = new List<CraftBase>();
        private string mCopiedItem;
        private CraftBase mEditorItem;

        public FrmCrafts()
        {
            ApplyHooks();
            InitializeComponent();
            lstCrafts.LostFocus += itemList_FocusChanged;
            lstCrafts.GotFocus += itemList_FocusChanged;
            cmbResult.Items.Clear();
            cmbResult.Items.Add(Strings.General.none);
            cmbResult.Items.AddRange(ItemBase.Names);
            cmbIngredient.Items.Clear();
            cmbIngredient.Items.Add(Strings.General.none);
            cmbIngredient.Items.AddRange(ItemBase.Names);
        }

        protected override void GameObjectUpdatedDelegate(GameObjectType type)
        {
            if (type == GameObjectType.Crafts)
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
            lstCrafts.Items.Clear();
            lstCrafts.Items.AddRange(CraftBase.Names);
        }

        private void lstCrafts_Click(object sender, EventArgs e)
        {
            if (mChangingName) return;
            mEditorItem = CraftBase.Get(CraftBase.IdFromList(lstCrafts.SelectedIndex));
            UpdateEditor();
        }

        private void UpdateEditor()
        {
            if (mEditorItem != null)
            {
                pnlContainer.Show();

                txtName.Text = mEditorItem.Name;

                //Populate ingredients and such
                nudSpeed.Value = mEditorItem.Time;
                cmbResult.SelectedIndex = ItemBase.ListIndex(mEditorItem.ItemId) + 1;

                nudCraftQuantity.Value = mEditorItem.Quantity;

                lstIngredients.Items.Clear();
                cmbIngredient.Hide();
                nudQuantity.Hide();
                lblQuantity.Hide();
                lblIngredient.Hide();
                for (int i = 0; i < mEditorItem.Ingredients.Count; i++)
                {
                    if (mEditorItem.Ingredients[i].ItemId != Guid.Empty)
                    {
                        lstIngredients.Items.Add(Strings.CraftsEditor.ingredientlistitem.ToString(
                            ItemBase.GetName(mEditorItem.Ingredients[i].ItemId),
                            mEditorItem.Ingredients[i].Quantity));
                    }
                    else
                    {
                        lstIngredients.Items.Add(Strings.CraftsEditor.ingredientlistitem.ToString(
                            Strings.CraftsEditor.ingredientnone, mEditorItem.Ingredients[i].Quantity));
                    }
                }
                if (lstIngredients.Items.Count > 0)
                {
                    lstIngredients.SelectedIndex = 0;
                    cmbIngredient.SelectedIndex = ItemBase.ListIndex(mEditorItem.Ingredients[lstIngredients.SelectedIndex].ItemId) + 1;
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
            mChangingName = true;
            mEditorItem.Name = txtName.Text;
            if (lstCrafts.SelectedIndex > -1)
            {
                lstCrafts.Items[lstCrafts.SelectedIndex] = txtName.Text;
            }
            mChangingName = false;
        }

        private void nudQuantity_ValueChanged(object sender, EventArgs e)
        {
            if (lstIngredients.SelectedIndex > -1)
            {
                mEditorItem.Ingredients[lstIngredients.SelectedIndex].Quantity = (int)nudQuantity.Value;
                if (cmbIngredient.SelectedIndex > 0)
                {
                    lstIngredients.Items[lstIngredients.SelectedIndex] = Strings.CraftsEditor.ingredientlistitem.ToString(
                        ItemBase.GetName(mEditorItem.Ingredients[lstIngredients.SelectedIndex].ItemId),
                        nudQuantity.Value);
                }
                else
                {
                    lstIngredients.Items[lstIngredients.SelectedIndex] = Strings.CraftsEditor.ingredientlistitem.ToString(Strings.CraftsEditor.ingredientnone, nudQuantity.Value);
                }
            }
        }

        private void nudSpeed_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Time = (int)nudSpeed.Value;
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
            if (mEditorItem != null && lstCrafts.Focused)
            {
                if (DarkMessageBox.ShowWarning(Strings.CraftsEditor.deleteprompt,
                        Strings.CraftsEditor.deletetitle, DarkDialogButton.YesNo,
                        Properties.Resources.Icon) ==
                    DialogResult.Yes)
                {
                    PacketSender.SendDeleteObject(mEditorItem);
                }
            }
        }

        private void toolStripItemCopy_Click(object sender, EventArgs e)
        {
            if (mEditorItem != null && lstCrafts.Focused)
            {
                mCopiedItem = mEditorItem.JsonData;
                toolStripItemPaste.Enabled = true;
            }
        }

        private void toolStripItemPaste_Click(object sender, EventArgs e)
        {
            if (mEditorItem != null && mCopiedItem != null && lstCrafts.Focused)
            {
                mEditorItem.Load(mCopiedItem, true);
                UpdateEditor();
            }
        }

        private void toolStripItemUndo_Click(object sender, EventArgs e)
        {
            if (mChanged.Contains(mEditorItem) && mEditorItem != null)
            {
                if (DarkMessageBox.ShowWarning(Strings.CraftsEditor.undoprompt,
                        Strings.CraftsEditor.undotitle, DarkDialogButton.YesNo,
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
            toolStripItemCopy.Enabled = mEditorItem != null && lstCrafts.Focused;
            toolStripItemPaste.Enabled = mEditorItem != null && mCopiedItem != null && lstCrafts.Focused;
            toolStripItemDelete.Enabled = mEditorItem != null && lstCrafts.Focused;
            toolStripItemUndo.Enabled = mEditorItem != null && lstCrafts.Focused;
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
            if (lstIngredients.SelectedIndex > -1)
            {
                cmbIngredient.Show();
                nudQuantity.Show();
                lblQuantity.Show();
                lblIngredient.Show();
                cmbIngredient.SelectedIndex = ItemBase.ListIndex(mEditorItem.Ingredients[lstIngredients.SelectedIndex].ItemId) + 1;
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
                mEditorItem.Ingredients.Insert(lstIngredients.SelectedIndex,
                    new CraftIngredient(mEditorItem.Ingredients[lstIngredients.SelectedIndex].ItemId,
                        mEditorItem.Ingredients[lstIngredients.SelectedIndex].Quantity));
                UpdateEditor();
            }
        }

        private void cmbResult_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.ItemId = ItemBase.IdFromList(cmbResult.SelectedIndex - 1);
            var itm = ItemBase.Get(mEditorItem.ItemId);
            if (itm == null || !itm.IsStackable())
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
                if (cmbIngredient.SelectedIndex > 0)
                {
                    lstIngredients.Items[lstIngredients.SelectedIndex] = Strings.CraftsEditor.ingredientlistitem.ToString(
                        ItemBase.GetName(mEditorItem.Ingredients[lstIngredients.SelectedIndex].ItemId),
                        nudQuantity.Value);
                }
                else
                {
                    lstIngredients.Items[lstIngredients.SelectedIndex] = Strings.CraftsEditor.ingredientlistitem.ToString(Strings.CraftsEditor.ingredientnone, nudQuantity.Value);
                }
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

            btnSave.Text = Strings.CraftsEditor.save;
            btnCancel.Text = Strings.CraftsEditor.cancel;
        }

        private void nudCraftQuantity_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Quantity = (int)nudCraftQuantity.Value;
        }
    }
}