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
using System.Windows.Forms;
using Intersect_Editor.Classes;
using Intersect_Library;
using Intersect_Library.GameObjects;


namespace Intersect_Editor.Forms.Editors
{
    public partial class frmShop : Form
    {
        private ByteBuffer[] _backups;
        private bool[] _changed;
        private int _editorIndex;

        public frmShop()
        {
            InitializeComponent();
        }

        public void InitEditor()
        {
            _backups = new ByteBuffer[Options.MaxShops];
            _changed = new bool[Options.MaxShops];
            for (var i = 0; i < Options.MaxShops; i++)
            {
                _backups[i] = new ByteBuffer();
                _backups[i].WriteBytes(Globals.GameShops[i].ShopData());
                lstShops.Items.Add((i + 1) + ". " + Globals.GameShops[i].Name);
                _changed[i] = false;
            }
        }

        private void frmShop_Load(object sender, EventArgs e)
        {
            lstShops.SelectedIndex = 0;
            cmbAddBoughtItem.Items.Clear();
            cmbAddSoldItem.Items.Clear();
            cmbBuyFor.Items.Clear();
            cmbSellFor.Items.Clear();
            cmbDefaultCurrency.Items.Clear();
            for (int i = 0; i < Options.MaxItems; i++)
            {
                cmbAddBoughtItem.Items.Add((i + 1) + ". " + Globals.GameItems[i].Name);
                cmbAddSoldItem.Items.Add((i + 1) + ". " + Globals.GameItems[i].Name);
                cmbBuyFor.Items.Add((i + 1) + ". " + Globals.GameItems[i].Name);
                cmbSellFor.Items.Add((i + 1) + ". " + Globals.GameItems[i].Name);
                cmbDefaultCurrency.Items.Add((i + 1) + ". " + Globals.GameItems[i].Name);
            }
            cmbAddBoughtItem.SelectedIndex = 0;
            cmbAddSoldItem.SelectedIndex = 0;
            cmbBuyFor.SelectedIndex = 0;
            cmbSellFor.SelectedIndex = 0;
            UpdateEditor();
        }

        private void UpdateEditor()
        {
            _editorIndex = lstShops.SelectedIndex;

            txtName.Text = Globals.GameShops[_editorIndex].Name;
            cmbDefaultCurrency.SelectedIndex = Globals.GameShops[_editorIndex].DefaultCurrency;
            if (Globals.GameShops[_editorIndex].BuyingWhitelist)
            {
                rdoBuyWhitelist.Checked = true;
            }
            else
            {
                rdoBuyBlacklist.Checked = true;
            }
            UpdateWhitelist();
            UpdateLists();
            _changed[_editorIndex] = true;
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < Options.MaxShops; i++)
            {
                if (_changed[i])
                {
                    PacketSender.SendShop(i, Globals.GameShops[i].ShopData());
                }
            }

            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var tempItem = new ShopStruct();
            var tempBuff = new ByteBuffer();
            tempBuff.WriteBytes(tempItem.ShopData());
            Globals.GameShops[_editorIndex].Load(tempBuff.ToArray(), _editorIndex);
            tempBuff.Dispose();
            UpdateEditor();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < Options.MaxShops; i++)
            {
                Globals.GameShops[i].Load(_backups[i].ToArray(), i);
            }

            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }

        private void lstShops_Click(object sender, EventArgs e)
        {
            UpdateEditor();
        }

        private void rdoBuyWhitelist_CheckedChanged(object sender, EventArgs e)
        {
            Globals.GameShops[_editorIndex].BuyingWhitelist = rdoBuyWhitelist.Checked;
            UpdateLists();
            UpdateWhitelist();
        }

        private void rdoBuyBlacklist_CheckedChanged(object sender, EventArgs e)
        {
            Globals.GameShops[_editorIndex].BuyingWhitelist = rdoBuyWhitelist.Checked;
            UpdateLists();
            UpdateWhitelist();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            Globals.GameShops[_editorIndex].Name = txtName.Text;
            lstShops.Items[_editorIndex] = (_editorIndex + 1) + ". " + txtName.Text;
        }

        private void UpdateLists()
        {
            lstSoldItems.Items.Clear();
            for (int i = 0; i < Globals.GameShops[_editorIndex].SellingItems.Count; i++)
            {
                lstSoldItems.Items.Add("Sell Item #" + (Globals.GameShops[_editorIndex].SellingItems[i].ItemNum + 1) + " " +
                                       Globals.GameItems[Globals.GameShops[_editorIndex].SellingItems[i].ItemNum].Name + " For (" +
                                       Globals.GameShops[_editorIndex].SellingItems[i].CostItemVal + ") Item #" +
                                       (Globals.GameShops[_editorIndex].SellingItems[i].CostItemNum + 1) + ". " +
                                       Globals.GameItems[Globals.GameShops[_editorIndex].SellingItems[i].CostItemNum].Name);
            }
            lstBoughtItems.Items.Clear();
            if (Globals.GameShops[_editorIndex].BuyingWhitelist)
            {
                for (int i = 0; i < Globals.GameShops[_editorIndex].BuyingItems.Count; i++)
                {
                    lstBoughtItems.Items.Add("Buy Item #" + (Globals.GameShops[_editorIndex].BuyingItems[i].ItemNum + 1) +
                                             " " +
                                             Globals.GameItems[Globals.GameShops[_editorIndex].BuyingItems[i].ItemNum]
                                                 .Name + " For (" +
                                       Globals.GameShops[_editorIndex].BuyingItems[i].CostItemVal + ") Item #" +
                                       (Globals.GameShops[_editorIndex].BuyingItems[i].CostItemNum + 1) + ". " +
                                       Globals.GameItems[Globals.GameShops[_editorIndex].BuyingItems[i].CostItemNum].Name);
                }
            }
            else
            {
                for (int i = 0; i < Globals.GameShops[_editorIndex].BuyingItems.Count; i++)
                {
                    lstBoughtItems.Items.Add("Don't Buy Item #" +
                                             (Globals.GameShops[_editorIndex].BuyingItems[i].ItemNum + 1) + " " +
                                             Globals.GameItems[Globals.GameShops[_editorIndex].BuyingItems[i].ItemNum].Name);
                }
            }

        }

        private void btnAddSoldItem_Click(object sender, EventArgs e)
        {
            bool addedItem = false;
            int cost = 0;
            int.TryParse(txtSellCost.Text, out cost);
            ShopItem newItem = new ShopItem(cmbAddSoldItem.SelectedIndex, cmbSellFor.SelectedIndex,cost);
            for (int i = 0; i < Globals.GameShops[_editorIndex].SellingItems.Count; i++)
            {
                if (Globals.GameShops[_editorIndex].SellingItems[i].ItemNum == newItem.ItemNum)
                {
                    Globals.GameShops[_editorIndex].SellingItems[i] = newItem;
                    addedItem = true;
                    break;
                }
            }
            if (!addedItem) Globals.GameShops[_editorIndex].SellingItems.Add(newItem);
            UpdateLists();
        }

        private void btnDelSoldItem_Click(object sender, EventArgs e)
        {
            if (lstSoldItems.SelectedIndex > -1)
            {
                Globals.GameShops[_editorIndex].SellingItems.RemoveAt(lstSoldItems.SelectedIndex);
            }
            UpdateLists();
        }

        private void btnAddBoughtItem_Click(object sender, EventArgs e)
        {
            bool addedItem = false;
            int cost = 0;
            int.TryParse(txtBuyAmount.Text, out cost);
            ShopItem newItem = new ShopItem(cmbAddBoughtItem.SelectedIndex, cmbBuyFor.SelectedIndex,cost);
            for (int i = 0; i < Globals.GameShops[_editorIndex].BuyingItems.Count; i++)
            {
                if (Globals.GameShops[_editorIndex].BuyingItems[i].ItemNum == newItem.ItemNum)
                {
                    Globals.GameShops[_editorIndex].BuyingItems[i] = newItem;
                    addedItem = true;
                    break;
                }
            }
            if (!addedItem) Globals.GameShops[_editorIndex].BuyingItems.Add(newItem);
            UpdateLists();
        }

        private void btnDelBoughtItem_Click(object sender, EventArgs e)
        {
            if (lstBoughtItems.SelectedIndex > -1)
            {
                Globals.GameShops[_editorIndex].BuyingItems.RemoveAt(lstBoughtItems.SelectedIndex);
            }
            UpdateLists();
        }

        private void cmbDefaultCurrency_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.GameShops[_editorIndex].DefaultCurrency = cmbDefaultCurrency.SelectedIndex;
        }
    }
}
