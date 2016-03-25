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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Intersect_Editor.Classes;
using Intersect_Editor.Classes.Game_Objects;

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
            _backups = new ByteBuffer[Constants.MaxShops];
            _changed = new bool[Constants.MaxShops];
            for (var i = 0; i < Constants.MaxShops; i++)
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
            for (int i = 0; i < Constants.MaxItems; i++)
            {
                cmbAddBoughtItem.Items.Add((i + 1) + ". " + Globals.GameItems[i].Name);
                cmbAddSoldItem.Items.Add((i + 1) + ". " + Globals.GameItems[i].Name);
            }
            cmbAddBoughtItem.SelectedIndex = 0;
            cmbAddSoldItem.SelectedIndex = 0;
            UpdateEditor();
        }

        private void UpdateEditor()
        {
            _editorIndex = lstShops.SelectedIndex;

            txtName.Text = Globals.GameShops[_editorIndex].Name;
            if (Globals.GameShops[_editorIndex].BuyingWhitelist)
            {
                rdoBuyWhitelist.Checked = true;
            }
            else
            {
                rdoBuyBlacklist.Checked = true;
            }
            UpdateLists();
            _changed[_editorIndex] = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < Constants.MaxShops; i++)
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
            for (var i = 0; i < Constants.MaxShops; i++)
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
        }

        private void rdoBuyBlacklist_CheckedChanged(object sender, EventArgs e)
        {
            Globals.GameShops[_editorIndex].BuyingWhitelist = !rdoBuyWhitelist.Checked;
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            Globals.GameShops[_editorIndex].Name = txtName.Text;
            lstShops.Items[_editorIndex] = (_editorIndex + 1) + ". " + txtName.Text;
        }

        private void scrlChargeRate_Scroll(object sender, ScrollEventArgs e)
        {
            lblChargeRate.Text = "Charge Rate: " + (scrlChargeRate.Value/100.0) + "x";
        }

        private void scrlBuyRate_Scroll(object sender, ScrollEventArgs e)
        {
            lblBuyRate.Text = "Buy Rate: " + (scrlBuyRate.Value / 100.0) + "x";
        }

        private void UpdateLists()
        {
            lstSoldItems.Items.Clear();
            for (int i = 0; i < Globals.GameShops[_editorIndex].SellingItems.Count; i++)
            {
                lstSoldItems.Items.Add("Item #" + (Globals.GameShops[_editorIndex].SellingItems[i].ItemNum + 1) + " " +
                                       Globals.GameItems[Globals.GameShops[_editorIndex].SellingItems[i].ItemNum].Name + "  at " +
                                       Globals.GameShops[_editorIndex].SellingItems[i].ItemRate + "x Item Value");
            }
            lstBoughtItems.Items.Clear();
            for (int i = 0; i < Globals.GameShops[_editorIndex].BuyingItems.Count; i++)
            {
                lstBoughtItems.Items.Add("Item #" + (Globals.GameShops[_editorIndex].BuyingItems[i].ItemNum + 1) + " " +
                                       Globals.GameItems[Globals.GameShops[_editorIndex].BuyingItems[i].ItemNum].Name + "  at " +
                                       Globals.GameShops[_editorIndex].BuyingItems[i].ItemRate + "x Item Value");
            }
        }

        private void btnAddSoldItem_Click(object sender, EventArgs e)
        {
            bool addedItem = false;
            ShopItem newItem = new ShopItem(cmbAddSoldItem.SelectedIndex, (scrlChargeRate.Value / 100.0));
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
            ShopItem newItem = new ShopItem(cmbAddBoughtItem.SelectedIndex, (scrlBuyRate.Value / 100.0));
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
    }
}
