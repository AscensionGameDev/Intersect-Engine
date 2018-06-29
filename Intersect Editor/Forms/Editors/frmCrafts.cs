using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DarkUI.Forms;
using Intersect.Editor.Classes;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Editor.Classes.Localization;
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
            cmbResult.Items.AddRange(Database.GetGameObjectList(GameObjectType.Item));
            cmbIngredient.Items.Clear();
            cmbIngredient.Items.Add(Strings.General.none);
            cmbIngredient.Items.AddRange(Database.GetGameObjectList(GameObjectType.Item));
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
            lstCrafts.Items.AddRange(Database.GetGameObjectList(GameObjectType.Crafts));
        }

        private void lstCrafts_Click(object sender, EventArgs e)
        {
            if (mChangingName) return;
            mEditorItem = CraftBase.Lookup.Get<CraftBase>(Database.GameObjectIdFromList(GameObjectType.Crafts, lstCrafts.SelectedIndex));
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
                cmbResult.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Item, mEditorItem.Item) + 1;

                lstIngredients.Items.Clear();
                cmbIngredient.Hide();
                nudQuantity.Hide();
                lblQuantity.Hide();
                lblIngredient.Hide();
                for (int i = 0; i < mEditorItem.Ingredients.Count; i++)
                {
                    if (mEditorItem.Ingredients[i].Item > -1)
                    {
                        lstIngredients.Items.Add(Strings.CraftsEditor.ingredientlistitem.ToString(
                            ItemBase.GetName(mEditorItem.Ingredients[i].Item),
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
                    cmbIngredient.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Item,
                                                      mEditorItem.Ingredients[lstIngredients.SelectedIndex].Item) + 1;
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
                mEditorItem.Ingredients[lstIngredients.SelectedIndex].Quantity = (int) nudQuantity.Value;
                if (cmbIngredient.SelectedIndex > 0)
                {
                    lstIngredients.Items[lstIngredients.SelectedIndex] = Strings.CraftsEditor.ingredientlistitem.ToString(
                        ItemBase.GetName(mEditorItem.Ingredients[lstIngredients.SelectedIndex].Item),
                        nudQuantity.Value);
                }
                else
                {
                    lstIngredients.Items[lstIngredients.SelectedIndex] = Strings.CraftsEditor.ingredientlistitem.ToString( Strings.CraftsEditor.ingredientnone, nudQuantity.Value);
                }
            }
        }

        private void nudSpeed_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Time = (int) nudSpeed.Value;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            mEditorItem.Ingredients.Add(new CraftIngredient(-1, 1));
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
                mEditorItem.Load(mCopiedItem);
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
                cmbIngredient.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Item,
                                                  mEditorItem.Ingredients[lstIngredients.SelectedIndex].Item) + 1;
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
                    new CraftIngredient(mEditorItem.Ingredients[lstIngredients.SelectedIndex].Item,
                        mEditorItem.Ingredients[lstIngredients.SelectedIndex].Quantity));
                UpdateEditor();
            }
        }

        private void cmbResult_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Item = Database.GameObjectIdFromList(GameObjectType.Item, cmbResult.SelectedIndex - 1);
        }

        private void cmbIngredient_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstIngredients.SelectedIndex > -1)
            {
                mEditorItem.Ingredients[lstIngredients.SelectedIndex].Item =
                    Database.GameObjectIdFromList(GameObjectType.Item, cmbIngredient.SelectedIndex - 1);
                if (cmbIngredient.SelectedIndex > 0)
                {
                    lstIngredients.Items[lstIngredients.SelectedIndex] = Strings.CraftsEditor.ingredientlistitem.ToString(
                        ItemBase.GetName(mEditorItem.Ingredients[lstIngredients.SelectedIndex].Item),
                        nudQuantity.Value);
                }
                else
                {
                    lstIngredients.Items[lstIngredients.SelectedIndex] = Strings.CraftsEditor.ingredientlistitem.ToString( Strings.CraftsEditor.ingredientnone, nudQuantity.Value);
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
    }
}