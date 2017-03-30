using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DarkUI.Forms;
using Intersect;
using Intersect.Editor.Classes;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Localization;

namespace Intersect.Editor.Forms.Editors
{
    public partial class frmCrafting : Form
    {
        private List<BenchBase> _changed = new List<BenchBase>();
        private byte[] _copiedItem = null;
        private Craft _currentCraft;
        private BenchBase _editorItem = null;

        public frmCrafting()
        {
            InitializeComponent();
            PacketHandler.GameObjectUpdatedDelegate += GameObjectUpdatedDelegate;
            lstCrafts.LostFocus += itemList_FocusChanged;
            lstCrafts.GotFocus += itemList_FocusChanged;
            cmbResult.Items.Clear();
            cmbResult.Items.Add(Strings.Get("general", "none"));
            cmbResult.Items.AddRange(Database.GetGameObjectList(GameObjectType.Item));
            cmbIngredient.Items.Clear();
            cmbIngredient.Items.Add(Strings.Get("general", "none"));
            cmbIngredient.Items.AddRange(Database.GetGameObjectList(GameObjectType.Item));
        }

        private void GameObjectUpdatedDelegate(GameObjectType type)
        {
            if (type == GameObjectType.Bench)
            {
                InitEditor();
                if (_editorItem != null && !DatabaseObject<BenchBase>.Lookup.Values.Contains(_editorItem))
                {
                    _editorItem = null;
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
            _editorItem = BenchBase.Lookup.Get<BenchBase>(Database.GameObjectIdFromList(GameObjectType.Bench, lstCrafts.SelectedIndex));
            UpdateEditor();
        }

        private void UpdateEditor()
        {
            if (_editorItem != null)
            {
                pnlContainer.Show();

                lstCompositions.Items.Clear();
                foreach (var i in _editorItem.Crafts)
                {
                    lstCompositions.Items.Add(ItemBase.GetName(i.Item));
                }
                if (lstCompositions.Items.Count > 0) lstCompositions.SelectedIndex = 0;

                txtName.Text = _editorItem.Name;

                UpdateCraft();

                if (_changed.IndexOf(_editorItem) == -1)
                {
                    _changed.Add(_editorItem);
                    _editorItem.MakeBackup();
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
                _currentCraft = _editorItem.Crafts[lstCompositions.SelectedIndex];

                nudSpeed.Value = _currentCraft.Time;
                cmbResult.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Item, _currentCraft.Item) + 1;

                if (lstCrafts.SelectedIndex < 0)
                {
                    lstCrafts.SelectedIndex = 0;
                }

                lstIngredients.Items.Clear();
                cmbIngredient.Hide();
                nudQuantity.Hide();
                lblQuantity.Hide();
                lblIngredient.Hide();
                for (int i = 0; i < _currentCraft.Ingredients.Count; i++)
                {
                    if (_currentCraft.Ingredients[i].Item > -1)
                    {
                        lstIngredients.Items.Add(Strings.Get("craftingeditor", "ingredientlistitem",
                            ItemBase.GetName(_currentCraft.Ingredients[i].Item), _currentCraft.Ingredients[i].Quantity));
                    }
                    else
                    {
                        lstIngredients.Items.Add(Strings.Get("craftingeditor", "ingredientlistitem",
                            Strings.Get("craftingeditor", "ingredientnone"), _currentCraft.Ingredients[i].Quantity));
                    }
                }
                if (lstIngredients.Items.Count > 0)
                {
                    lstIngredients.SelectedIndex = 0;
                    cmbIngredient.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Item,
                                                      _currentCraft.Ingredients[lstIngredients.SelectedIndex].Item) + 1;
                    nudQuantity.Value = _currentCraft.Ingredients[lstIngredients.SelectedIndex].Quantity;
                }
            }
            else
            {
                grpIngredients.Hide();
            }
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            _editorItem.Name = txtName.Text;
            if (lstCrafts.SelectedIndex > -1)
            {
                lstCrafts.Items[lstCrafts.SelectedIndex] = txtName.Text;
            }
        }

        private void nudQuantity_ValueChanged(object sender, EventArgs e)
        {
            if (lstIngredients.SelectedIndex > -1)
            {
                _currentCraft.Ingredients[lstIngredients.SelectedIndex].Quantity = (int) nudQuantity.Value;
                if (cmbIngredient.SelectedIndex > 0)
                {
                    lstIngredients.Items[lstIngredients.SelectedIndex] = Strings.Get("craftingeditor",
                        "ingredientlistitem",
                        ItemBase.GetName(_currentCraft.Ingredients[lstIngredients.SelectedIndex].Item),
                        nudQuantity.Value);
                }
                else
                {
                    lstIngredients.Items[lstIngredients.SelectedIndex] = Strings.Get("craftingeditor",
                        "ingredientlistitem", Strings.Get("craftingeditor", "ingredientnone"), nudQuantity.Value);
                }
            }
        }

        private void nudSpeed_ValueChanged(object sender, EventArgs e)
        {
            _currentCraft.Time = (int) nudSpeed.Value;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            _currentCraft.Ingredients.Add(new CraftIngredient(-1, 1));
            lstIngredients.Items.Add(Strings.Get("general", "none"));
            lstIngredients.SelectedIndex = lstIngredients.Items.Count - 1;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lstIngredients.Items.Count > 0)
            {
                _currentCraft.Ingredients.RemoveAt(lstIngredients.SelectedIndex);
                UpdateCraft();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            foreach (var item in _changed)
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
            foreach (var item in _changed)
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
            _editorItem.Crafts.Add(new Craft());
            lstCompositions.Items.Add(Strings.Get("general", "none"));
        }

        private void btnDeleteCraft_Click(object sender, EventArgs e)
        {
            if (_editorItem.Crafts.Count > 1)
            {
                _editorItem.Crafts.RemoveAt(lstCompositions.SelectedIndex);
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
            if (_editorItem != null && lstCrafts.Focused)
            {
                if (DarkMessageBox.ShowWarning(Strings.Get("craftingeditor", "deleteprompt"),
                        Strings.Get("craftingeditor", "deletetitle"), DarkDialogButton.YesNo, Properties.Resources.Icon) ==
                    DialogResult.Yes)
                {
                    PacketSender.SendDeleteObject(_editorItem);
                }
            }
        }

        private void toolStripItemCopy_Click(object sender, EventArgs e)
        {
            if (_editorItem != null && lstCrafts.Focused)
            {
                _copiedItem = _editorItem.BinaryData;
                toolStripItemPaste.Enabled = true;
            }
        }

        private void toolStripItemPaste_Click(object sender, EventArgs e)
        {
            if (_editorItem != null && _copiedItem != null && lstCrafts.Focused)
            {
                _editorItem.Load(_copiedItem);
                UpdateEditor();
            }
        }

        private void toolStripItemUndo_Click(object sender, EventArgs e)
        {
            if (_changed.Contains(_editorItem) && _editorItem != null)
            {
                if (DarkMessageBox.ShowWarning(Strings.Get("craftingeditor", "undoprompt"),
                        Strings.Get("craftingeditor", "undotitle"), DarkDialogButton.YesNo, Properties.Resources.Icon) ==
                    DialogResult.Yes)
                {
                    _editorItem.RestoreBackup();
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
            toolStripItemCopy.Enabled = _editorItem != null && lstCrafts.Focused;
            toolStripItemPaste.Enabled = _editorItem != null && _copiedItem != null && lstCrafts.Focused;
            toolStripItemDelete.Enabled = _editorItem != null && lstCrafts.Focused;
            toolStripItemUndo.Enabled = _editorItem != null && lstCrafts.Focused;
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
                                                  _currentCraft.Ingredients[lstIngredients.SelectedIndex].Item) + 1;
                nudQuantity.Value = _currentCraft.Ingredients[lstIngredients.SelectedIndex].Quantity;
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
                _currentCraft.Ingredients.Insert(lstIngredients.SelectedIndex,
                    new CraftIngredient(_currentCraft.Ingredients[lstIngredients.SelectedIndex].Item,
                        _currentCraft.Ingredients[lstIngredients.SelectedIndex].Quantity));
                UpdateCraft();
            }
        }

        private void btnDupCraft_Click(object sender, EventArgs e)
        {
            if (lstCompositions.SelectedIndex > -1 && _currentCraft != null)
            {
                var bf = new ByteBuffer();
                var craft = new Craft();
                bf.WriteBytes(_currentCraft.Data());
                craft.Load(bf);
                var nextIndex = lstCompositions.SelectedIndex + 1;
                _editorItem.Crafts.Insert(nextIndex, craft);
                UpdateEditor();
                // TODO: Fix this so that when the selected index changes the editor actually updates
                //lstCompositions.SelectedIndex = nextIndex;
            }
        }

        private void cmbResult_SelectedIndexChanged(object sender, EventArgs e)
        {
            _currentCraft.Item = Database.GameObjectIdFromList(GameObjectType.Item, cmbResult.SelectedIndex - 1);

            if (lstCompositions.SelectedIndex > -1)
            {
                if (cmbResult.SelectedIndex > 0)
                {
                    lstCompositions.Items[lstCompositions.SelectedIndex] = ItemBase.GetName(_currentCraft.Item);
                }
                else
                {
                    lstCompositions.Items[lstCompositions.SelectedIndex] = Strings.Get("general", "none");
                }
            }
        }

        private void cmbIngredient_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstIngredients.SelectedIndex > -1)
            {
                _currentCraft.Ingredients[lstIngredients.SelectedIndex].Item =
                    Database.GameObjectIdFromList(GameObjectType.Item, cmbIngredient.SelectedIndex - 1);
                if (cmbIngredient.SelectedIndex > 0)
                {
                    lstIngredients.Items[lstIngredients.SelectedIndex] = Strings.Get("craftingeditor",
                        "ingredientlistitem",
                        ItemBase.GetName(_currentCraft.Ingredients[lstIngredients.SelectedIndex].Item),
                        nudQuantity.Value);
                }
                else
                {
                    lstIngredients.Items[lstIngredients.SelectedIndex] = Strings.Get("craftingeditor",
                        "ingredientlistitem", Strings.Get("craftingeditor", "ingredientnone"), nudQuantity.Value);
                }
            }
        }

        private void frmCrafting_Load(object sender, EventArgs e)
        {
            InitLocalization();
        }

        private void InitLocalization()
        {
            Text = Strings.Get("craftingeditor", "title");
            toolStripItemNew.Text = Strings.Get("craftingeditor", "new");
            toolStripItemDelete.Text = Strings.Get("craftingeditor", "delete");
            toolStripItemCopy.Text = Strings.Get("craftingeditor", "copy");
            toolStripItemPaste.Text = Strings.Get("craftingeditor", "paste");
            toolStripItemUndo.Text = Strings.Get("craftingeditor", "undo");

            grpBenches.Text = Strings.Get("craftingeditor", "benches");
            grpCrafts.Text = Strings.Get("craftingeditor", "crafts");
            btnNewCraft.Text = Strings.Get("craftingeditor", "newcraft");
            btnDeleteCraft.Text = Strings.Get("craftingeditor", "deletecraft");
            btnDupCraft.Text = Strings.Get("craftingeditor", "duplicatecraft");

            grpCraft.Text = Strings.Get("craftingeditor", "general");
            lblName.Text = Strings.Get("craftingeditor", "name");
            lblItem.Text = Strings.Get("craftingeditor", "item");
            lblSpeed.Text = Strings.Get("craftingeditor", "time");

            grpIngredients.Text = Strings.Get("craftingeditor", "ingredients");
            lblIngredient.Text = Strings.Get("craftingeditor", "ingredientitem");
            lblQuantity.Text = Strings.Get("craftingeditor", "ingredientquantity");
            btnAdd.Text = Strings.Get("craftingeditor", "newingredient");
            btnRemove.Text = Strings.Get("craftingeditor", "deleteingredient");
            btnDupIngredient.Text = Strings.Get("craftingeditor", "duplicateingredient");

            btnSave.Text = Strings.Get("craftingeditor", "save");
            btnCancel.Text = Strings.Get("craftingeditor", "cancel");
        }
    }
}