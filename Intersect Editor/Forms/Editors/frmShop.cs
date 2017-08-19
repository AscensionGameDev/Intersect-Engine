using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DarkUI.Forms;
using Intersect.Editor.Classes;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Localization;

namespace Intersect.Editor.Forms.Editors
{
    public partial class frmShop : EditorForm
    {
        private List<ShopBase> _changed = new List<ShopBase>();
        private byte[] _copiedItem;
        private ShopBase _editorItem;

        public frmShop()
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
                if (_editorItem != null && !ShopBase.Lookup.Values.Contains(_editorItem))
                {
                    _editorItem = null;
                    UpdateEditor();
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
            _editorItem =
                ShopBase.Lookup.Get<ShopBase>(
                    Database.GameObjectIdFromList(GameObjectType.Shop, lstShops.SelectedIndex));
            UpdateEditor();
        }

        public void InitEditor()
        {
            lstShops.Items.Clear();
            lstShops.Items.AddRange(Database.GetGameObjectList(GameObjectType.Shop));
        }

        private void frmShop_Load(object sender, EventArgs e)
        {
            cmbAddBoughtItem.Items.Clear();
            cmbAddSoldItem.Items.Clear();
            cmbBuyFor.Items.Clear();
            cmbSellFor.Items.Clear();
            cmbDefaultCurrency.Items.Clear();
            foreach (var item in ItemBase.Lookup)
            {
                cmbAddBoughtItem.Items.Add(item.Value.Name);
                cmbAddSoldItem.Items.Add(item.Value.Name);
                cmbBuyFor.Items.Add(item.Value.Name);
                cmbSellFor.Items.Add(item.Value.Name);
                cmbDefaultCurrency.Items.Add(item.Value.Name);
            }
            if (cmbAddBoughtItem.Items.Count > 0) cmbAddBoughtItem.SelectedIndex = 0;
            if (cmbAddSoldItem.Items.Count > 0) cmbAddSoldItem.SelectedIndex = 0;
            if (cmbBuyFor.Items.Count > 0) cmbBuyFor.SelectedIndex = 0;
            if (cmbSellFor.Items.Count > 0) cmbSellFor.SelectedIndex = 0;
            InitLocalization();
            UpdateEditor();
        }

        private void InitLocalization()
        {
            Text = Strings.Get("shopeditor", "title");
            toolStripItemNew.Text = Strings.Get("shopeditor", "new");
            toolStripItemDelete.Text = Strings.Get("shopeditor", "delete");
            toolStripItemCopy.Text = Strings.Get("shopeditor", "copy");
            toolStripItemPaste.Text = Strings.Get("shopeditor", "paste");
            toolStripItemUndo.Text = Strings.Get("shopeditor", "undo");

            grpGeneral.Text = Strings.Get("shopeditor", "general");
            lblName.Text = Strings.Get("shopeditor", "name");
            lblDefaultCurrency.Text = Strings.Get("shopeditor", "defaultcurrency");

            grpItemsSold.Text = Strings.Get("shopeditor", "itemssold");
            lblAddSoldItem.Text = Strings.Get("shopeditor", "addlabel");
            lblSellFor.Text = Strings.Get("shopeditor", "sellfor");
            lblSellCost.Text = Strings.Get("shopeditor", "sellcost");
            btnAddSoldItem.Text = Strings.Get("shopeditor", "addsolditem");
            btnDelSoldItem.Text = Strings.Get("shopeditor", "removesolditem");

            grpItemsBought.Text = Strings.Get("shopeditor", "itemsboughtwhitelist");
            rdoBuyWhitelist.Text = Strings.Get("shopeditor", "whitelist");
            rdoBuyBlacklist.Text = Strings.Get("shopeditor", "blacklist");
            lblItemBought.Text = Strings.Get("shopeditor", "addboughtitem");
            lblBuyFor.Text = Strings.Get("shopeditor", "buyfor");
            lblBuyAmount.Text = Strings.Get("shopeditor", "buycost");
            btnAddBoughtItem.Text = Strings.Get("shopeditor", "addboughtitem");
            btnDelBoughtItem.Text = Strings.Get("shopeditor", "removeboughtitem");

            btnSave.Text = Strings.Get("shopeditor", "save");
            btnCancel.Text = Strings.Get("shopeditor", "cancel");
        }

        private void UpdateEditor()
        {
            if (_editorItem != null)
            {
                pnlContainer.Show();

                txtName.Text = _editorItem.Name;
                cmbDefaultCurrency.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Item,
                    _editorItem.DefaultCurrency);
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
            UpdateToolStripItems();
        }

        private void UpdateWhitelist()
        {
            if (rdoBuyWhitelist.Checked)
            {
                cmbBuyFor.Enabled = true;
                nudBuyAmount.Enabled = true;
                grpItemsBought.Text = Strings.Get("shopeditor", "itemsboughtwhitelist");
            }
            else
            {
                cmbBuyFor.Enabled = false;
                nudBuyAmount.Enabled = false;
                grpItemsBought.Text = Strings.Get("shopeditor", "itemsboughtblacklist");
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
            lstShops.Items[ShopBase.Lookup.IndexKeys.ToList().IndexOf(_editorItem.Index)] = txtName.Text;
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
            int cost = (int) nudSellCost.Value;
            ShopItem newItem = new ShopItem(ItemBase.Lookup.IndexKeys.ToList()[cmbAddSoldItem.SelectedIndex]
                , ItemBase.Lookup.IndexKeys.ToList()[cmbSellFor.SelectedIndex], cost);
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
            int cost = (int) nudBuyAmount.Value;
            ShopItem newItem = new ShopItem(ItemBase.Lookup.IndexKeys.ToList()[cmbAddBoughtItem.SelectedIndex],
                ItemBase.Lookup.IndexKeys.ToList()[cmbBuyFor.SelectedIndex], cost);
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
            _editorItem.DefaultCurrency = Database.GameObjectIdFromList(GameObjectType.Item,
                cmbDefaultCurrency.SelectedIndex);
        }

        private void toolStripItemNew_Click(object sender, EventArgs e)
        {
            PacketSender.SendCreateObject(GameObjectType.Shop);
        }

        private void toolStripItemDelete_Click(object sender, EventArgs e)
        {
            if (_editorItem != null && lstShops.Focused)
            {
                if (DarkMessageBox.ShowWarning(Strings.Get("shopeditor", "deleteprompt"),
                        Strings.Get("shopeditor", "deletetitle"), DarkDialogButton.YesNo, Properties.Resources.Icon) ==
                    DialogResult.Yes)
                {
                    PacketSender.SendDeleteObject(_editorItem);
                }
            }
        }

        private void toolStripItemCopy_Click(object sender, EventArgs e)
        {
            if (_editorItem != null && lstShops.Focused)
            {
                _copiedItem = _editorItem.BinaryData;
                toolStripItemPaste.Enabled = true;
            }
        }

        private void toolStripItemPaste_Click(object sender, EventArgs e)
        {
            if (_editorItem != null && _copiedItem != null && lstShops.Focused)
            {
                _editorItem.Load(_copiedItem);
                UpdateEditor();
            }
        }

        private void toolStripItemUndo_Click(object sender, EventArgs e)
        {
            if (_changed.Contains(_editorItem) && _editorItem != null)
            {
                if (DarkMessageBox.ShowWarning(Strings.Get("shopeditor", "undoprompt"),
                        Strings.Get("shopeditor", "undotitle"), DarkDialogButton.YesNo, Properties.Resources.Icon) ==
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
            toolStripItemCopy.Enabled = _editorItem != null && lstShops.Focused;
            toolStripItemPaste.Enabled = _editorItem != null && _copiedItem != null && lstShops.Focused;
            toolStripItemDelete.Enabled = _editorItem != null && lstShops.Focused;
            toolStripItemUndo.Enabled = _editorItem != null && lstShops.Focused;
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
    }
}