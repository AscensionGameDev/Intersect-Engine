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
    public partial class FrmCrafting : EditorForm
    {
        private List<BenchBase> mChanged = new List<BenchBase>();
        private byte[] mCopiedItem;
        private Craft mCurrentCraft;
        private BenchBase mEditorItem;

        public FrmCrafting()
        {
            ApplyHooks();
            InitializeComponent();
            lstCrafts.LostFocus += itemList_FocusChanged;
            lstCrafts.GotFocus += itemList_FocusChanged;
            cmbResult.Items.Clear();
            cmbResult.Items.Add(Strings.general.none);
            cmbResult.Items.AddRange(Database.GetGameObjectList(GameObjectType.Item));
            cmbIngredient.Items.Clear();
            cmbIngredient.Items.Add(Strings.general.none);
            cmbIngredient.Items.AddRange(Database.GetGameObjectList(GameObjectType.Item));
        }

        protected override void GameObjectUpdatedDelegate(GameObjectType type)
        {
            if (type == GameObjectType.Bench)
            {
                InitEditor();
                if (mEditorItem != null && !DatabaseObject<BenchBase>.Lookup.Values.Contains(mEditorItem))
                {
                    mEditorItem = null;
                    UpdateEditor();
                }
            }
        }

        public void InitEditor()
        {
            lstCrafts.Items.Clear();
            lstCrafts.Items.AddRange(Database.GetGameObjectList(GameObjectType.Bench));
        }

        private void lstCrafts_Click(object sender, EventArgs e)
        {
            if (mChangingName) return;
            mEditorItem =
                BenchBase.Lookup.Get<BenchBase>(
                    Database.GameObjectIdFromList(GameObjectType.Bench, lstCrafts.SelectedIndex));
            UpdateEditor();
        }

        private void UpdateEditor()
        {
            if (mEditorItem != null)
            {
                pnlContainer.Show();

                lstCompositions.Items.Clear();
                foreach (var i in mEditorItem.Crafts)
                {
                    lstCompositions.Items.Add(ItemBase.GetName(i.Item));
                }
                if (lstCompositions.Items.Count > 0) lstCompositions.SelectedIndex = 0;

                txtName.Text = mEditorItem.Name;

                UpdateCraft();

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

        private void UpdateCraft()
        {
            grpCraft.Hide();
            grpIngredients.Hide();
            if (lstCompositions.SelectedIndex > -1)
            {
                grpIngredients.Show();
                grpCraft.Show();
                mCurrentCraft = mEditorItem.Crafts[lstCompositions.SelectedIndex];

                nudSpeed.Value = mCurrentCraft.Time;
                cmbResult.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Item, mCurrentCraft.Item) + 1;

                if (lstCrafts.SelectedIndex < 0)
                {
                    lstCrafts.SelectedIndex = 0;
                }

                lstIngredients.Items.Clear();
                cmbIngredient.Hide();
                nudQuantity.Hide();
                lblQuantity.Hide();
                lblIngredient.Hide();
                for (int i = 0; i < mCurrentCraft.Ingredients.Count; i++)
                {
                    if (mCurrentCraft.Ingredients[i].Item > -1)
                    {
                        lstIngredients.Items.Add(Strings.craftingeditor.ingredientlistitem.ToString(
                            ItemBase.GetName(mCurrentCraft.Ingredients[i].Item),
                            mCurrentCraft.Ingredients[i].Quantity));
                    }
                    else
                    {
                        lstIngredients.Items.Add(Strings.craftingeditor.ingredientlistitem.ToString(
                            Strings.craftingeditor.ingredientnone, mCurrentCraft.Ingredients[i].Quantity));
                    }
                }
                if (lstIngredients.Items.Count > 0)
                {
                    lstIngredients.SelectedIndex = 0;
                    cmbIngredient.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Item,
                                                      mCurrentCraft.Ingredients[lstIngredients.SelectedIndex].Item) + 1;
                    nudQuantity.Value = mCurrentCraft.Ingredients[lstIngredients.SelectedIndex].Quantity;
                }
            }
            else
            {
                grpIngredients.Hide();
            }
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
                mCurrentCraft.Ingredients[lstIngredients.SelectedIndex].Quantity = (int) nudQuantity.Value;
                if (cmbIngredient.SelectedIndex > 0)
                {
                    lstIngredients.Items[lstIngredients.SelectedIndex] = Strings.craftingeditor.ingredientlistitem.ToString(
                        ItemBase.GetName(mCurrentCraft.Ingredients[lstIngredients.SelectedIndex].Item),
                        nudQuantity.Value);
                }
                else
                {
                    lstIngredients.Items[lstIngredients.SelectedIndex] = Strings.craftingeditor.ingredientlistitem.ToString( Strings.craftingeditor.ingredientnone, nudQuantity.Value);
                }
            }
        }

        private void nudSpeed_ValueChanged(object sender, EventArgs e)
        {
            mCurrentCraft.Time = (int) nudSpeed.Value;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            mCurrentCraft.Ingredients.Add(new CraftIngredient(-1, 1));
            lstIngredients.Items.Add(Strings.general.none);
            lstIngredients.SelectedIndex = lstIngredients.Items.Count - 1;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lstIngredients.Items.Count > 0)
            {
                mCurrentCraft.Ingredients.RemoveAt(lstIngredients.SelectedIndex);
                UpdateCraft();
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

        private void lstCompositions_Click(object sender, EventArgs e)
        {
            UpdateCraft();
        }

        private void btnNewCraft_Click(object sender, EventArgs e)
        {
            mEditorItem.Crafts.Add(new Craft());
            lstCompositions.Items.Add(Strings.general.none);
        }

        private void btnDeleteCraft_Click(object sender, EventArgs e)
        {
            if (mEditorItem.Crafts.Count > 1)
            {
                mEditorItem.Crafts.RemoveAt(lstCompositions.SelectedIndex);
                lstCompositions.Items.RemoveAt(lstCompositions.SelectedIndex);
                lstCompositions.SelectedIndex = 0;
            }
        }

        private void toolStripItemNew_Click(object sender, EventArgs e)
        {
            PacketSender.SendCreateObject(GameObjectType.Bench);
        }

        private void toolStripItemDelete_Click(object sender, EventArgs e)
        {
            if (mEditorItem != null && lstCrafts.Focused)
            {
                if (DarkMessageBox.ShowWarning(Strings.craftingeditor.deleteprompt,
                        Strings.craftingeditor.deletetitle, DarkDialogButton.YesNo,
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
                mCopiedItem = mEditorItem.BinaryData;
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
                if (DarkMessageBox.ShowWarning(Strings.craftingeditor.undoprompt,
                        Strings.craftingeditor.undotitle, DarkDialogButton.YesNo,
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

        private void lstCompositions_SelectedIndexChanged(object sender, EventArgs e)
        {
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
                                                  mCurrentCraft.Ingredients[lstIngredients.SelectedIndex].Item) + 1;
                nudQuantity.Value = mCurrentCraft.Ingredients[lstIngredients.SelectedIndex].Quantity;
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
                mCurrentCraft.Ingredients.Insert(lstIngredients.SelectedIndex,
                    new CraftIngredient(mCurrentCraft.Ingredients[lstIngredients.SelectedIndex].Item,
                        mCurrentCraft.Ingredients[lstIngredients.SelectedIndex].Quantity));
                UpdateCraft();
            }
        }

        private void btnDupCraft_Click(object sender, EventArgs e)
        {
            if (lstCompositions.SelectedIndex > -1 && mCurrentCraft != null)
            {
                var bf = new ByteBuffer();
                var craft = new Craft();
                bf.WriteBytes(mCurrentCraft.Data());
                craft.Load(bf);
                var nextIndex = lstCompositions.SelectedIndex + 1;
                mEditorItem.Crafts.Insert(nextIndex, craft);
                UpdateEditor();
                // TODO: Fix this so that when the selected index changes the editor actually updates
                //lstCompositions.SelectedIndex = nextIndex;
            }
        }

        private void cmbResult_SelectedIndexChanged(object sender, EventArgs e)
        {
            mCurrentCraft.Item = Database.GameObjectIdFromList(GameObjectType.Item, cmbResult.SelectedIndex - 1);

            if (lstCompositions.SelectedIndex > -1)
            {
                if (cmbResult.SelectedIndex > 0)
                {
                    lstCompositions.Items[lstCompositions.SelectedIndex] = ItemBase.GetName(mCurrentCraft.Item);
                }
                else
                {
                    lstCompositions.Items[lstCompositions.SelectedIndex] = Strings.general.none;
                }
            }
        }

        private void cmbIngredient_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstIngredients.SelectedIndex > -1)
            {
                mCurrentCraft.Ingredients[lstIngredients.SelectedIndex].Item =
                    Database.GameObjectIdFromList(GameObjectType.Item, cmbIngredient.SelectedIndex - 1);
                if (cmbIngredient.SelectedIndex > 0)
                {
                    lstIngredients.Items[lstIngredients.SelectedIndex] = Strings.craftingeditor.ingredientlistitem.ToString(
                        ItemBase.GetName(mCurrentCraft.Ingredients[lstIngredients.SelectedIndex].Item),
                        nudQuantity.Value);
                }
                else
                {
                    lstIngredients.Items[lstIngredients.SelectedIndex] = Strings.craftingeditor.ingredientlistitem.ToString( Strings.craftingeditor.ingredientnone, nudQuantity.Value);
                }
            }
        }

        private void frmCrafting_Load(object sender, EventArgs e)
        {
            InitLocalization();
        }

        private void InitLocalization()
        {
            Text = Strings.craftingeditor.title;
            toolStripItemNew.Text = Strings.craftingeditor.New;
            toolStripItemDelete.Text = Strings.craftingeditor.delete;
            toolStripItemCopy.Text = Strings.craftingeditor.copy;
            toolStripItemPaste.Text = Strings.craftingeditor.paste;
            toolStripItemUndo.Text = Strings.craftingeditor.undo;

            grpBenches.Text = Strings.craftingeditor.benches;
            grpCrafts.Text = Strings.craftingeditor.crafts;
            btnNewCraft.Text = Strings.craftingeditor.newcraft;
            btnDeleteCraft.Text = Strings.craftingeditor.deletecraft;
            btnDupCraft.Text = Strings.craftingeditor.duplicatecraft;

            grpCraft.Text = Strings.craftingeditor.general;
            lblName.Text = Strings.craftingeditor.name;
            lblItem.Text = Strings.craftingeditor.item;
            lblSpeed.Text = Strings.craftingeditor.time;

            grpIngredients.Text = Strings.craftingeditor.ingredients;
            lblIngredient.Text = Strings.craftingeditor.ingredientitem;
            lblQuantity.Text = Strings.craftingeditor.ingredientquantity;
            btnAdd.Text = Strings.craftingeditor.newingredient;
            btnRemove.Text = Strings.craftingeditor.deleteingredient;
            btnDupIngredient.Text = Strings.craftingeditor.duplicateingredient;

            btnSave.Text = Strings.craftingeditor.save;
            btnCancel.Text = Strings.craftingeditor.cancel;
        }
    }
}