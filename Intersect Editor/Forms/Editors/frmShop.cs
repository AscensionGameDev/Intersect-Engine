/*
    Intersect Game Engine (Editor)
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Intersect_Editor.Classes;
using Intersect_Library;
using Intersect_Library.GameObjects;


namespace Intersect_Editor.Forms.Editors
{
    public partial class frmShop : Form
    {
        private List<ShopBase> _changed = new List<ShopBase>();
        private ShopBase _editorItem = null;

        public frmShop()
        {
            InitializeComponent();
            PacketHandler.GameObjectUpdatedDelegate += GameObjectUpdatedDelegate;
        }

        private void GameObjectUpdatedDelegate(GameObject type)
        {
            if (type == GameObject.Shop)
            {
                InitEditor();
                if (_editorItem != null && !ShopBase.GetObjects().Values.Contains(_editorItem))
                {
                    _editorItem = null;
                    UpdateEditor();
                }
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            PacketSender.SendCreateObject(GameObject.Shop);
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            if (_changed.Contains(_editorItem) && _editorItem != null)
            {
                _editorItem.RestoreBackup();
                UpdateEditor();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_editorItem != null)
            {
                if (
                    MessageBox.Show(
                        "Are you sure you want to delete this game object? This action cannot be reverted!",
                        "Delete Object", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    PacketSender.SendDeleteObject(_editorItem);
                }
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

        private void lstShops_Click(object sender, EventArgs e)
        {
            _editorItem = ShopBase.GetShop(Database.GameObjectIdFromList(GameObject.Shop, lstShops.SelectedIndex));
            UpdateEditor();
        }

        public void InitEditor()
        {
            lstShops.Items.Clear();
            lstShops.Items.AddRange(Database.GetGameObjectList(GameObject.Shop));
        }

        private void frmShop_Load(object sender, EventArgs e)
        {
            cmbAddBoughtItem.Items.Clear();
            cmbAddSoldItem.Items.Clear();
            cmbBuyFor.Items.Clear();
            cmbSellFor.Items.Clear();
            cmbDefaultCurrency.Items.Clear();
            foreach (var item in ItemBase.GetObjects())
            {
                cmbAddBoughtItem.Items.Add(item.Value.Name);
                cmbAddSoldItem.Items.Add(item.Value.Name);
                cmbBuyFor.Items.Add(item.Value.Name);
                cmbSellFor.Items.Add(item.Value.Name);
                cmbDefaultCurrency.Items.Add(item.Value.Name);
            }
            cmbAddBoughtItem.SelectedIndex = 0;
            cmbAddSoldItem.SelectedIndex = 0;
            cmbBuyFor.SelectedIndex = 0;
            cmbSellFor.SelectedIndex = 0;
            UpdateEditor();
        }

        private void UpdateEditor()
        {
            if (_editorItem != null)
            {
                pnlContainer.Show();

                txtName.Text = _editorItem.Name;
                cmbDefaultCurrency.SelectedIndex =
                    ItemBase.GetObjects().Keys.ToList().IndexOf(_editorItem.DefaultCurrency);
                if (_editorItem.BuyingWhitelist)
                {
                    rdoBuyWhitelist.Checked = true;
                }
                else
                {
                    rdoBuyBlacklist.Checked = true;
                }
                UpdateWhitelist();
                UpdateLists();
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
        }

        private void UpdateWhitelist()
        {
            if (rdoBuyWhitelist.Checked)
            {
                cmbBuyFor.Enabled = true;
                txtBuyAmount.Enabled = true;
                grpItemsBought.Text = "Items Bought (Whitelist - Buy Listed Items)";
            }
            else
            {
                cmbBuyFor.Enabled = false;
                txtBuyAmount.Enabled = false;
                grpItemsBought.Text = "Items Bought (Blacklist - Buy All But Listed Items)";
            }
        }

        private void rdoBuyWhitelist_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem.BuyingWhitelist = rdoBuyWhitelist.Checked;
            UpdateLists();
            UpdateWhitelist();
        }

        private void rdoBuyBlacklist_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem.BuyingWhitelist = rdoBuyWhitelist.Checked;
            UpdateLists();
            UpdateWhitelist();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            _editorItem.Name = txtName.Text;
            lstShops.Items[ShopBase.GetObjects().Keys.ToList().IndexOf(_editorItem.GetId())] = txtName.Text;
        }

        private void UpdateLists()
        {
            lstSoldItems.Items.Clear();
            for (int i = 0; i < _editorItem.SellingItems.Count; i++)
            {
                lstSoldItems.Items.Add("Sell Item #" + (_editorItem.SellingItems[i].ItemNum + 1) + " " +
                                       ItemBase.GetName(_editorItem.SellingItems[i].ItemNum) + " For (" +
                                       _editorItem.SellingItems[i].CostItemVal + ") Item #" +
                                       (_editorItem.SellingItems[i].CostItemNum + 1) + ". " +
                                       ItemBase.GetName(_editorItem.SellingItems[i].CostItemNum));
            }
            lstBoughtItems.Items.Clear();
            if (_editorItem.BuyingWhitelist)
            {
                for (int i = 0; i < _editorItem.BuyingItems.Count; i++)
                {
                    lstBoughtItems.Items.Add("Buy Item #" + (_editorItem.BuyingItems[i].ItemNum + 1) +
                                             " " +
                                            ItemBase.GetName(_editorItem.BuyingItems[i].ItemNum) + " For (" +
                                       _editorItem.BuyingItems[i].CostItemVal + ") Item #" +
                                       (_editorItem.BuyingItems[i].CostItemNum + 1) + ". " +
                                       ItemBase.GetName(_editorItem.BuyingItems[i].CostItemNum));
                }
            }
            else
            {
                for (int i = 0; i < _editorItem.BuyingItems.Count; i++)
                {
                    lstBoughtItems.Items.Add("Don't Buy Item #" +
                                             (_editorItem.BuyingItems[i].ItemNum + 1) + " " +
                                             ItemBase.GetName(_editorItem.BuyingItems[i].ItemNum));
                }
            }

        }

        private void btnAddSoldItem_Click(object sender, EventArgs e)
        {
            bool addedItem = false;
            int cost = 0;
            int.TryParse(txtSellCost.Text, out cost);
            ShopItem newItem = new ShopItem(ItemBase.GetObjects().Keys.ToList()[cmbAddSoldItem.SelectedIndex]
                , ItemBase.GetObjects().Keys.ToList()[cmbSellFor.SelectedIndex],cost);
            for (int i = 0; i < _editorItem.SellingItems.Count; i++)
            {
                if (_editorItem.SellingItems[i].ItemNum == newItem.ItemNum)
                {
                    _editorItem.SellingItems[i] = newItem;
                    addedItem = true;
                    break;
                }
            }
            if (!addedItem) _editorItem.SellingItems.Add(newItem);
            UpdateLists();
        }

        private void btnDelSoldItem_Click(object sender, EventArgs e)
        {
            if (lstSoldItems.SelectedIndex > -1)
            {
                _editorItem.SellingItems.RemoveAt(lstSoldItems.SelectedIndex);
            }
            UpdateLists();
        }

        private void btnAddBoughtItem_Click(object sender, EventArgs e)
        {
            bool addedItem = false;
            int cost = 0;
            int.TryParse(txtBuyAmount.Text, out cost);
            ShopItem newItem = new ShopItem(ItemBase.GetObjects().Keys.ToList()[cmbAddBoughtItem.SelectedIndex],
                ItemBase.GetObjects().Keys.ToList()[cmbBuyFor.SelectedIndex],cost);
            for (int i = 0; i < _editorItem.BuyingItems.Count; i++)
            {
                if (_editorItem.BuyingItems[i].ItemNum == newItem.ItemNum)
                {
                    _editorItem.BuyingItems[i] = newItem;
                    addedItem = true;
                    break;
                }
            }
            if (!addedItem) _editorItem.BuyingItems.Add(newItem);
            UpdateLists();
        }

        private void btnDelBoughtItem_Click(object sender, EventArgs e)
        {
            if (lstBoughtItems.SelectedIndex > -1)
            {
                _editorItem.BuyingItems.RemoveAt(lstBoughtItems.SelectedIndex);
            }
            UpdateLists();
        }

        private void cmbDefaultCurrency_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.DefaultCurrency = ItemBase.GetObjects().Keys.ToList()[cmbDefaultCurrency.SelectedIndex];
        }
    }
}
