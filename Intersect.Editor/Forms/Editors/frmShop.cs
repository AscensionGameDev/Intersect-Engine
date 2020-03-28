﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

using DarkUI.Forms;

using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Editor.Networking;
using Intersect.Enums;
using Intersect.GameObjects;

namespace Intersect.Editor.Forms.Editors
{

    public partial class FrmShop : EditorForm
    {

        private List<ShopBase> mChanged = new List<ShopBase>();

        private string mCopiedItem;

        private ShopBase mEditorItem;

        private List<string> mExpandedFolders = new List<string>();

        private List<string> mKnownFolders = new List<string>();

        public FrmShop()
        {
            ApplyHooks();
            InitializeComponent();
            lstShops.LostFocus += itemList_FocusChanged;
            lstShops.GotFocus += itemList_FocusChanged;
        }

        protected override void GameObjectUpdatedDelegate(GameObjectType type)
        {
            if (type == GameObjectType.Shop)
            {
                InitEditor();
                if (mEditorItem != null && !ShopBase.Lookup.Values.Contains(mEditorItem))
                {
                    mEditorItem = null;
                    UpdateEditor();
                }
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

        private void frmShop_Load(object sender, EventArgs e)
        {
            cmbAddBoughtItem.Items.Clear();
            cmbAddSoldItem.Items.Clear();
            cmbBuyFor.Items.Clear();
            cmbSellFor.Items.Clear();
            cmbDefaultCurrency.Items.Clear();
            cmbAddBoughtItem.Items.AddRange(ItemBase.Names);
            cmbAddSoldItem.Items.AddRange(ItemBase.Names);
            cmbBuyFor.Items.AddRange(ItemBase.Names);
            cmbSellFor.Items.AddRange(ItemBase.Names);
            cmbDefaultCurrency.Items.AddRange(ItemBase.Names);
            if (cmbAddBoughtItem.Items.Count > 0)
            {
                cmbAddBoughtItem.SelectedIndex = 0;
            }

            if (cmbAddSoldItem.Items.Count > 0)
            {
                cmbAddSoldItem.SelectedIndex = 0;
            }

            if (cmbBuyFor.Items.Count > 0)
            {
                cmbBuyFor.SelectedIndex = 0;
            }

            if (cmbSellFor.Items.Count > 0)
            {
                cmbSellFor.SelectedIndex = 0;
            }

            InitLocalization();
            UpdateEditor();
        }

        private void InitLocalization()
        {
            Text = Strings.ShopEditor.title;
            toolStripItemNew.Text = Strings.ShopEditor.New;
            toolStripItemDelete.Text = Strings.ShopEditor.delete;
            toolStripItemCopy.Text = Strings.ShopEditor.copy;
            toolStripItemPaste.Text = Strings.ShopEditor.paste;
            toolStripItemUndo.Text = Strings.ShopEditor.undo;

            grpGeneral.Text = Strings.ShopEditor.general;
            lblName.Text = Strings.ShopEditor.name;
            lblDefaultCurrency.Text = Strings.ShopEditor.defaultcurrency;

            grpItemsSold.Text = Strings.ShopEditor.itemssold;
            lblAddSoldItem.Text = Strings.ShopEditor.addlabel;
            lblSellFor.Text = Strings.ShopEditor.sellfor;
            lblSellCost.Text = Strings.ShopEditor.sellcost;
            btnAddSoldItem.Text = Strings.ShopEditor.addsolditem;
            btnDelSoldItem.Text = Strings.ShopEditor.removesolditem;

            grpItemsBought.Text = Strings.ShopEditor.itemsboughtwhitelist;
            rdoBuyWhitelist.Text = Strings.ShopEditor.whitelist;
            rdoBuyBlacklist.Text = Strings.ShopEditor.blacklist;
            lblItemBought.Text = Strings.ShopEditor.addboughtitem;
            lblBuyFor.Text = Strings.ShopEditor.buyfor;
            lblBuyAmount.Text = Strings.ShopEditor.buycost;
            btnAddBoughtItem.Text = Strings.ShopEditor.addboughtitem;
            btnDelBoughtItem.Text = Strings.ShopEditor.removeboughtitem;

            //Searching/Sorting
            btnChronological.ToolTipText = Strings.ShopEditor.sortchronologically;
            txtSearch.Text = Strings.ShopEditor.searchplaceholder;
            lblFolder.Text = Strings.ShopEditor.folderlabel;

            btnSave.Text = Strings.ShopEditor.save;
            btnCancel.Text = Strings.ShopEditor.cancel;
        }

        private void UpdateEditor()
        {
            if (mEditorItem != null)
            {
                pnlContainer.Show();

                txtName.Text = mEditorItem.Name;
                cmbFolder.Text = mEditorItem.Folder;
                cmbDefaultCurrency.SelectedIndex = ItemBase.ListIndex(mEditorItem.DefaultCurrencyId);
                if (mEditorItem.BuyingWhitelist)
                {
                    rdoBuyWhitelist.Checked = true;
                }
                else
                {
                    rdoBuyBlacklist.Checked = true;
                }

                UpdateWhitelist();
                UpdateLists();
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

        private void UpdateWhitelist()
        {
            if (rdoBuyWhitelist.Checked)
            {
                cmbBuyFor.Enabled = true;
                nudBuyAmount.Enabled = true;
                grpItemsBought.Text = Strings.ShopEditor.itemsboughtwhitelist;
            }
            else
            {
                cmbBuyFor.Enabled = false;
                nudBuyAmount.Enabled = false;
                grpItemsBought.Text = Strings.ShopEditor.itemsboughtblacklist;
            }
        }

        private void rdoBuyWhitelist_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.BuyingWhitelist = rdoBuyWhitelist.Checked;
            UpdateLists();
            UpdateWhitelist();
        }

        private void rdoBuyBlacklist_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.BuyingWhitelist = rdoBuyWhitelist.Checked;
            UpdateLists();
            UpdateWhitelist();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            mChangingName = true;
            mEditorItem.Name = txtName.Text;
            if (lstShops.SelectedNode != null && lstShops.SelectedNode.Tag != null)
            {
                lstShops.SelectedNode.Text = txtName.Text;
            }

            mChangingName = false;
        }

        private void UpdateLists()
        {
            lstSoldItems.Items.Clear();
            for (var i = 0; i < mEditorItem.SellingItems.Count; i++)
            {
                lstSoldItems.Items.Add(
                    Strings.ShopEditor.selldesc.ToString(
                        ItemBase.GetName(mEditorItem.SellingItems[i].ItemId),
                        mEditorItem.SellingItems[i].CostItemQuantity,
                        ItemBase.GetName(mEditorItem.SellingItems[i].CostItemId)
                    )
                );
            }

            lstBoughtItems.Items.Clear();
            if (mEditorItem.BuyingWhitelist)
            {
                for (var i = 0; i < mEditorItem.BuyingItems.Count; i++)
                {
                    lstBoughtItems.Items.Add(
                        Strings.ShopEditor.buydesc.ToString(
                            ItemBase.GetName(mEditorItem.BuyingItems[i].ItemId),
                            mEditorItem.BuyingItems[i].CostItemQuantity,
                            ItemBase.GetName(mEditorItem.BuyingItems[i].CostItemId)
                        )
                    );
                }
            }
            else
            {
                for (var i = 0; i < mEditorItem.BuyingItems.Count; i++)
                {
                    lstBoughtItems.Items.Add(
                        Strings.ShopEditor.dontbuy.ToString(ItemBase.GetName(mEditorItem.BuyingItems[i].ItemId))
                    );
                }
            }
        }

        private void btnAddSoldItem_Click(object sender, EventArgs e)
        {
            var addedItem = false;
            var cost = (int) nudSellCost.Value;
            var newItem = new ShopItem(
                ItemBase.IdFromList(cmbAddSoldItem.SelectedIndex), ItemBase.IdFromList(cmbSellFor.SelectedIndex), cost
            );

            for (var i = 0; i < mEditorItem.SellingItems.Count; i++)
            {
                if (mEditorItem.SellingItems[i].ItemId == newItem.ItemId)
                {
                    mEditorItem.SellingItems[i] = newItem;
                    addedItem = true;

                    break;
                }
            }

            if (!addedItem)
            {
                mEditorItem.SellingItems.Add(newItem);
            }

            UpdateLists();
        }

        private void btnDelSoldItem_Click(object sender, EventArgs e)
        {
            if (lstSoldItems.SelectedIndex > -1)
            {
                mEditorItem.SellingItems.RemoveAt(lstSoldItems.SelectedIndex);
            }

            UpdateLists();
        }

        private void btnAddBoughtItem_Click(object sender, EventArgs e)
        {
            var addedItem = false;
            var cost = (int) nudBuyAmount.Value;
            var newItem = new ShopItem(
                ItemBase.IdFromList(cmbAddBoughtItem.SelectedIndex), ItemBase.IdFromList(cmbBuyFor.SelectedIndex), cost
            );

            for (var i = 0; i < mEditorItem.BuyingItems.Count; i++)
            {
                if (mEditorItem.BuyingItems[i].ItemId == newItem.ItemId)
                {
                    mEditorItem.BuyingItems[i] = newItem;
                    addedItem = true;

                    break;
                }
            }

            if (!addedItem)
            {
                mEditorItem.BuyingItems.Add(newItem);
            }

            UpdateLists();
        }

        private void btnDelBoughtItem_Click(object sender, EventArgs e)
        {
            if (lstBoughtItems.SelectedIndex > -1)
            {
                mEditorItem.BuyingItems.RemoveAt(lstBoughtItems.SelectedIndex);
            }

            UpdateLists();
        }

        private void cmbDefaultCurrency_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.DefaultCurrency = ItemBase.FromList(cmbDefaultCurrency.SelectedIndex);
        }

        private void toolStripItemNew_Click(object sender, EventArgs e)
        {
            PacketSender.SendCreateObject(GameObjectType.Shop);
        }

        private void toolStripItemDelete_Click(object sender, EventArgs e)
        {
            if (mEditorItem != null && lstShops.Focused)
            {
                if (DarkMessageBox.ShowWarning(
                        Strings.ShopEditor.deleteprompt, Strings.ShopEditor.deletetitle, DarkDialogButton.YesNo,
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
            if (mEditorItem != null && lstShops.Focused)
            {
                mCopiedItem = mEditorItem.JsonData;
                toolStripItemPaste.Enabled = true;
            }
        }

        private void toolStripItemPaste_Click(object sender, EventArgs e)
        {
            if (mEditorItem != null && mCopiedItem != null && lstShops.Focused)
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
                        Strings.ShopEditor.undoprompt, Strings.ShopEditor.undotitle, DarkDialogButton.YesNo,
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
            toolStripItemCopy.Enabled = mEditorItem != null && lstShops.Focused;
            toolStripItemPaste.Enabled = mEditorItem != null && mCopiedItem != null && lstShops.Focused;
            toolStripItemDelete.Enabled = mEditorItem != null && lstShops.Focused;
            toolStripItemUndo.Enabled = mEditorItem != null && lstShops.Focused;
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

        #region "Item List - Folders, Searching, Sorting, Etc"

        public void InitEditor()
        {
            var selectedId = Guid.Empty;
            var folderNodes = new Dictionary<string, TreeNode>();
            if (lstShops.SelectedNode != null && lstShops.SelectedNode.Tag != null)
            {
                selectedId = (Guid) lstShops.SelectedNode.Tag;
            }

            lstShops.Nodes.Clear();

            //Collect folders
            var mFolders = new List<string>();
            foreach (var itm in ShopBase.Lookup)
            {
                if (!string.IsNullOrEmpty(((ShopBase) itm.Value).Folder) &&
                    !mFolders.Contains(((ShopBase) itm.Value).Folder))
                {
                    mFolders.Add(((ShopBase) itm.Value).Folder);
                    if (!mKnownFolders.Contains(((ShopBase) itm.Value).Folder))
                    {
                        mKnownFolders.Add(((ShopBase) itm.Value).Folder);
                    }
                }
            }

            mFolders.Sort();
            mKnownFolders.Sort();
            cmbFolder.Items.Clear();
            cmbFolder.Items.Add("");
            cmbFolder.Items.AddRange(mKnownFolders.ToArray());

            lstShops.Sorted = !btnChronological.Checked;

            if (!btnChronological.Checked && !CustomSearch())
            {
                foreach (var folder in mFolders)
                {
                    var node = lstShops.Nodes.Add(folder);
                    node.ImageIndex = 0;
                    node.SelectedImageIndex = 0;
                    folderNodes.Add(folder, node);
                }
            }

            foreach (var itm in ShopBase.ItemPairs)
            {
                var node = new TreeNode(itm.Value);
                node.Tag = itm.Key;
                node.ImageIndex = 1;
                node.SelectedImageIndex = 1;

                var folder = ShopBase.Get(itm.Key).Folder;
                if (!string.IsNullOrEmpty(folder) && !btnChronological.Checked && !CustomSearch())
                {
                    var folderNode = folderNodes[folder];
                    folderNode.Nodes.Add(node);
                    if (itm.Key == selectedId)
                    {
                        folderNode.Expand();
                    }
                }
                else
                {
                    lstShops.Nodes.Add(node);
                }

                if (CustomSearch())
                {
                    if (!node.Text.ToLower().Contains(txtSearch.Text.ToLower()))
                    {
                        node.Remove();
                    }
                }

                if (itm.Key == selectedId)
                {
                    lstShops.SelectedNode = node;
                }
            }

            var selectedNode = lstShops.SelectedNode;

            if (!btnChronological.Checked)
            {
                lstShops.Sort();
            }

            lstShops.SelectedNode = selectedNode;
            foreach (var node in mExpandedFolders)
            {
                if (folderNodes.ContainsKey(node))
                {
                    folderNodes[node].Expand();
                }
            }
        }

        private void btnAddFolder_Click(object sender, EventArgs e)
        {
            var folderName = "";
            var result = DarkInputBox.ShowInformation(
                Strings.ShopEditor.folderprompt, Strings.ShopEditor.foldertitle, ref folderName,
                DarkDialogButton.OkCancel
            );

            if (result == DialogResult.OK && !string.IsNullOrEmpty(folderName))
            {
                if (!cmbFolder.Items.Contains(folderName))
                {
                    mEditorItem.Folder = folderName;
                    mExpandedFolders.Add(folderName);
                    InitEditor();
                    cmbFolder.Text = folderName;
                }
            }
        }

        private void lstShops_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var node = e.Node;
            if (node != null)
            {
                if (e.Button == MouseButtons.Right)
                {
                    if (e.Node.Tag != null && e.Node.Tag.GetType() == typeof(Guid))
                    {
                        Clipboard.SetText(e.Node.Tag.ToString());
                    }
                }

                var hitTest = lstShops.HitTest(e.Location);
                if (hitTest.Location != TreeViewHitTestLocations.PlusMinus)
                {
                    if (node.Nodes.Count > 0)
                    {
                        if (node.IsExpanded)
                        {
                            node.Collapse();
                        }
                        else
                        {
                            node.Expand();
                        }
                    }
                }

                if (node.IsExpanded)
                {
                    if (!mExpandedFolders.Contains(node.Text))
                    {
                        mExpandedFolders.Add(node.Text);
                    }
                }
                else
                {
                    if (mExpandedFolders.Contains(node.Text))
                    {
                        mExpandedFolders.Remove(node.Text);
                    }
                }
            }
        }

        private void lstShops_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (mChangingName)
            {
                return;
            }

            if (lstShops.SelectedNode == null || lstShops.SelectedNode.Tag == null)
            {
                return;
            }

            mEditorItem = ShopBase.Get((Guid) lstShops.SelectedNode.Tag);
            UpdateEditor();
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
                txtSearch.Text = Strings.ShopEditor.searchplaceholder;
            }
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            txtSearch.SelectAll();
            txtSearch.Focus();
        }

        private void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = Strings.ShopEditor.searchplaceholder;
        }

        private bool CustomSearch()
        {
            return !string.IsNullOrWhiteSpace(txtSearch.Text) && txtSearch.Text != Strings.ShopEditor.searchplaceholder;
        }

        private void txtSearch_Click(object sender, EventArgs e)
        {
            if (txtSearch.Text == Strings.ShopEditor.searchplaceholder)
            {
                txtSearch.SelectAll();
            }
        }

        #endregion

    }

}
