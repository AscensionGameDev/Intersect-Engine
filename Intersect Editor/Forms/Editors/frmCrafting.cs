/*
    Intersect Game Engine (Server)
    Copyright (C) 2015  JC Snider, Joe Bridges
    
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com 

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/

using Intersect_Editor.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Intersect_Library;
using Intersect_Library.GameObjects;
using Intersect_Editor.Classes.Core;
using Intersect_Library.GameObjects.Events;

namespace Intersect_Editor.Forms.Editors
{
    public partial class frmCrafting : Form
    {

        private List<BenchBase> _changed = new List<BenchBase>();
        private BenchBase _editorItem = null;
        private Craft _currentCraft;
        private byte[] _copiedItem = null;

        public frmCrafting()
        {
            InitializeComponent();
            PacketHandler.GameObjectUpdatedDelegate += GameObjectUpdatedDelegate;
            lstCrafts.LostFocus += itemList_FocusChanged;
            lstCrafts.GotFocus += itemList_FocusChanged;
            scrlItem.Maximum = ItemBase.ObjectCount() - 1;
            scrlIngredient.Maximum = ItemBase.ObjectCount() - 1;
        }

        private void GameObjectUpdatedDelegate(GameObject type)
        {
            if (type == GameObject.Bench)
            {
                InitEditor();
                if (_editorItem != null && !BenchBase.GetObjects().ContainsValue(_editorItem))
                {
                    _editorItem = null;
                    UpdateEditor();
                }
            }
        }


        public void InitEditor()
        {
            lstCrafts.Items.Clear();
            lstCrafts.Items.AddRange(Database.GetGameObjectList(GameObject.Bench));
            scrlIngredient.Maximum = ItemBase.ObjectCount() - 1;
            scrlItem.Maximum = ItemBase.ObjectCount() - 1;
        }

        private void lstCrafts_Click(object sender, EventArgs e)
        {
            _editorItem = BenchBase.GetCraft(Database.GameObjectIdFromList(GameObject.Bench, lstCrafts.SelectedIndex));
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
            groupBox5.Hide();
            if (lstCompositions.SelectedIndex > -1)
            {
                groupBox5.Show();
                grpCraft.Show();
                _currentCraft = _editorItem.Crafts[lstCompositions.SelectedIndex];

                scrlSpeed.Value = _currentCraft.Time;
                scrlItem.Value = Database.GameObjectListIndex(GameObject.Item, _currentCraft.Item);

                lblSpeed.Text = "Time: " + scrlSpeed.Value + "ms";

                if (scrlItem.Value > -1)
                {
                    lblItem.Text = "Item: " + ItemBase.GetName(_currentCraft.Item);
                }
                else
                {
                    lblItem.Text = "Item: None";
                }

                if (lstCrafts.SelectedIndex < 0)
                {
                    lstCrafts.SelectedIndex = 0;
                }

                lstIngredients.Items.Clear();
                scrlIngredient.Hide();
                scrlQuantity.Hide();
                lblQuantity.Hide();
                lblIngredient.Hide();
                for (int i = 0; i < _currentCraft.Ingredients.Count; i++)
                {
                    if (_currentCraft.Ingredients[i].Item > -1)
                    {
                        lstIngredients.Items.Add("Ingredient: " + ItemBase.GetName(_currentCraft.Ingredients[i].Item) +
                                                 " x" + _currentCraft.Ingredients[i].Quantity);
                    }
                    else
                    {
                        lstIngredients.Items.Add("Ingredient: None x" + _currentCraft.Ingredients[i].Quantity);
                    }
                }
                if (lstIngredients.Items.Count > 0)
                {
                    lstIngredients.SelectedIndex = 0;
                    scrlIngredient.Value = Database.GameObjectListIndex(GameObject.Item,
                        _currentCraft.Ingredients[lstIngredients.SelectedIndex].Item);
                    scrlQuantity.Value = _currentCraft.Ingredients[lstIngredients.SelectedIndex].Quantity;
                    lblQuantity.Text = "Quantity x" + scrlQuantity.Value;

                    if (scrlIngredient.Value > -1)
                    {
                        lblIngredient.Text = "Item: " +
                                             ItemBase.GetName(_currentCraft.Ingredients[lstIngredients.SelectedIndex].Item);
                    }
                    else
                    {
                        lblIngredient.Text = "Item: None";
                    }
                }
            }
            else
            {
                groupBox5.Hide();
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

        private void scrlItem_Scroll(object sender, ScrollEventArgs e)
        {
            _currentCraft.Item = Database.GameObjectIdFromList(GameObject.Item, scrlItem.Value);

            if (scrlItem.Value > -1)
            {
                lblItem.Text = "Item: " + ItemBase.GetName(_currentCraft.Item);
            }
            else
            {
                lblItem.Text = "Item: None";
            }
            if (lstCompositions.SelectedIndex > -1)
            {
                if (scrlItem.Value > -1)
                {
                    lstCompositions.Items[lstCompositions.SelectedIndex] = ItemBase.GetName(_currentCraft.Item);
                }
                else
                {
                    lstCompositions.Items[lstCompositions.SelectedIndex] = "None";
                }
            }
        }

        private void scrlSpeed_Scroll(object sender, ScrollEventArgs e)
        {
            _currentCraft.Time = scrlSpeed.Value;
            lblSpeed.Text = "Time: " + scrlSpeed.Value + "ms";
        }

        private void lstIngredients_Click(object sender, EventArgs e)
        {
            

        }

        private void scrlIngredient_Scroll(object sender, ScrollEventArgs e)
        {
            if (lstIngredients.SelectedIndex > -1)
            {
                _currentCraft.Ingredients[lstIngredients.SelectedIndex].Item =
                    Database.GameObjectIdFromList(GameObject.Item, scrlIngredient.Value);
                if (scrlIngredient.Value > -1)
                {
                    lblIngredient.Text = "Item: " +
                                         ItemBase.GetName(_currentCraft.Ingredients[lstIngredients.SelectedIndex].Item);
                    lstIngredients.Items[lstIngredients.SelectedIndex] = "Ingredient: " +
                                                                         ItemBase.GetName(
                                                                             _currentCraft.Ingredients[
                                                                                 lstIngredients.SelectedIndex].Item) +
                                                                         " x" + scrlQuantity.Value;
                }
                else
                {
                    lblIngredient.Text = "Item: None";
                    lstIngredients.Items[lstIngredients.SelectedIndex] = "Ingredient: None x" + scrlQuantity.Value;
                }
            }
        }

        private void scrlQuantity_Scroll(object sender, ScrollEventArgs e)
        {
            if (lstIngredients.SelectedIndex > -1)
            {
                _currentCraft.Ingredients[lstIngredients.SelectedIndex].Quantity = scrlQuantity.Value;
                lblQuantity.Text = "Quantity x" + scrlQuantity.Value;
                if (scrlIngredient.Value > -1)
                {
                    lstIngredients.Items[lstIngredients.SelectedIndex] = "Ingredient: " +
                                                                         ItemBase.GetName(
                                                                             _currentCraft.Ingredients[
                                                                                 lstIngredients.SelectedIndex].Item) +
                                                                         " x" + scrlQuantity.Value;
                }
                else
                {
                    lstIngredients.Items[lstIngredients.SelectedIndex] = "Ingredient: None x" + scrlQuantity.Value;
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            _currentCraft.Ingredients.Add(new CraftIngredient(-1, 1));
            lstIngredients.Items.Add("None");
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

        private void btnNewComposition_Click(object sender, EventArgs e)
        {
            _editorItem.Crafts.Add(new Craft());
            lstCompositions.Items.Add("None");
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
            PacketSender.SendCreateObject(GameObject.Bench);
        }

        private void toolStripItemDelete_Click(object sender, EventArgs e)
        {
            if (_editorItem != null && lstCrafts.Focused)
            {
                if (
                    MessageBox.Show("Are you sure you want to delete this game object? This action cannot be reverted!",
                        "Delete Object", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    PacketSender.SendDeleteObject(_editorItem);
                }
            }
        }

        private void toolStripItemCopy_Click(object sender, EventArgs e)
        {
            if (_editorItem != null && lstCrafts.Focused)
            {
                _copiedItem = _editorItem.GetData();
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
                if (MessageBox.Show("Are you sure you want to undo changes made to this game object? This action cannot be reverted!",
                        "Undo Changes", MessageBoxButtons.YesNo) == DialogResult.Yes)
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
                scrlIngredient.Show();
                scrlQuantity.Show();
                lblQuantity.Show();
                lblIngredient.Show();
                scrlIngredient.Value = Database.GameObjectListIndex(GameObject.Item,
                    _currentCraft.Ingredients[lstIngredients.SelectedIndex].Item);
                scrlQuantity.Value = _currentCraft.Ingredients[lstIngredients.SelectedIndex].Quantity;
                lblQuantity.Text = "Quantity x" + scrlQuantity.Value;

                if (scrlIngredient.Value > -1)
                {
                    lblIngredient.Text = "Item: " +
                                         ItemBase.GetName(_currentCraft.Ingredients[lstIngredients.SelectedIndex].Item);
                }
                else
                {
                    lblIngredient.Text = "Item: None";
                }
            }
            else
            {
                scrlIngredient.Hide();
                scrlQuantity.Hide();
                lblQuantity.Hide();
                lblIngredient.Hide();
            }
        }

        private void btnDupIngredient_Click(object sender, EventArgs e)
        {
            if (lstIngredients.SelectedIndex > -1)
            {
                _currentCraft.Ingredients.Insert(lstIngredients.SelectedIndex,new CraftIngredient(_currentCraft.Ingredients[lstIngredients.SelectedIndex].Item, _currentCraft.Ingredients[lstIngredients.SelectedIndex].Quantity));
                UpdateCraft();
            }
        }

        private void btnDupCraft_Click(object sender, EventArgs e)
        {
            if (lstCrafts.SelectedIndex > -1 && _currentCraft != null)
            {
                var bf = new ByteBuffer();
                var craft = new Craft();
                bf.WriteBytes(_currentCraft.Data());
                craft.Load(bf);
                _editorItem.Crafts.Insert(lstCrafts.SelectedIndex,craft);
                UpdateEditor();
            }
        }
    }
}
