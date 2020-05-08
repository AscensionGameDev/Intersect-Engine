using DarkUI.Forms;
using Intersect.Editor.General;
using Intersect.Editor.Networking;
using Intersect.Enums;
using Intersect.GameObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Intersect.Editor.Forms.Editors
{
	public partial class FrmDropPool : EditorForm
	{
		private List<DropPoolBase> mChanged = new List<DropPoolBase>();
		private DropPoolBase mEditorItem;

		public FrmDropPool()
		{
			ApplyHooks();
			InitializeComponent();
			lstDropLoot.LostFocus += itemList_FocusChanged;
			lstDropLoot.GotFocus += itemList_FocusChanged;
		}
		
		protected override void GameObjectUpdatedDelegate(GameObjectType type)
		{
			if (type == GameObjectType.DropPool)
			{
				InitEditor();
				if (mEditorItem != null && !DropPoolBase.Lookup.Values.Contains(mEditorItem))
				{
					mEditorItem = null;
					UpdateEditor();
				}
			}
		}

		public void InitEditor()
		{
			var selectedId = Guid.Empty;
			var folderNodes = new Dictionary<string, TreeNode>();
			if (lstDropLoot.SelectedNode != null && lstDropLoot.SelectedNode.Tag != null)
			{
				selectedId = (Guid)lstDropLoot.SelectedNode.Tag;
			}

			lstDropLoot.Nodes.Clear();
						
			foreach (var itm in DropPoolBase.ItemPairs)
			{
				var node = new TreeNode(itm.Value);
				node.Tag = itm.Key;
				node.ImageIndex = 1;
				node.SelectedImageIndex = 1;

				lstDropLoot.Nodes.Add(node);

				if (itm.Key == selectedId)
				{
					lstDropLoot.SelectedNode = node;
				}
			}

			var selectedNode = lstDropLoot.SelectedNode;

			lstDropLoot.SelectedNode = selectedNode;

			cmbAddItem.Items.Clear();
			cmbAddItem.Items.AddRange(ItemBase.Names);
		}

		private void itemList_FocusChanged(object sender, EventArgs e)
		{
			UpdateToolStripItems();
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			foreach (var item in mChanged)
			{
				PacketSender.SendSaveObject(item);
				item.DeleteBackup();
			}

			Hide();
			Globals.CurrentEditor = -1;
			Dispose();
		}

		private void UpdateList()
		{
			if (mEditorItem == null)
			{
				return;
			}
			lstItems.Items.Clear();
			if (mEditorItem.ItemPool == null)
			{
				mEditorItem.ItemPool = new List<ItemPool>();
			}
			foreach (ItemPool pool in mEditorItem.ItemPool)
			{
				lstItems.Items.Add($"{ItemBase.Get(pool.ItemId).Name} x{pool.Quantity}, {pool.Chance.ToString("0.00")}% chance");
			}
		}

		private void UpdateEditor()
		{
			UpdateList();
			if (mEditorItem != null)
			{
				grpGeneral.Show();
				grpItems.Show();

				txtName.Text = mEditorItem.Name;

				if (mChanged.IndexOf(mEditorItem) == -1)
				{
					mChanged.Add(mEditorItem);
					mEditorItem.MakeBackup();
				}
			}
			else
			{
				grpGeneral.Hide();
				grpItems.Hide();
			}

			UpdateToolStripItems();
		}

		private void UpdateToolStripItems()
		{
			toolStripItemDelete.Enabled = mEditorItem != null && lstDropLoot.Focused;
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

		private void toolStripItemNew_Click(object sender, EventArgs e)
		{
			PacketSender.SendCreateObject(GameObjectType.DropPool);
		}

		private void toolStripItemDelete_Click(object sender, EventArgs e)
		{
			if (mEditorItem != null && lstDropLoot.Focused)
			{
				if (DarkMessageBox.ShowWarning(
						"Etes vous sur de vouloir supprimer la drop pool", "Supprimer la drop pool", DarkDialogButton.YesNo,
						Properties.Resources.Icon
					) ==
					DialogResult.Yes)
				{
					PacketSender.SendDeleteObject(mEditorItem);
				}
			}
		}

		private void txtName_TextChanged(object sender, EventArgs e)
		{
			mEditorItem.Name = txtName.Text;
		}

		private void btnAddItem_Click(object sender, EventArgs e)
		{
			Guid ItemId = ItemBase.IdFromList(cmbAddItem.SelectedIndex);
			if (ItemId != null && ItemId != Guid.Empty)
			{
				mEditorItem.ItemPool.Add(new ItemPool()
				{
					ItemId = ItemId,
					Quantity = (int)nudDropAmount.Value,
					Chance = (double)nudDropChance.Value
				});
			}
			UpdateList();
		}

		private void btnDelItem_Click(object sender, EventArgs e)
		{
			if (lstItems.SelectedIndex > -1)
			{
				mEditorItem.ItemPool.RemoveAt(lstItems.SelectedIndex);
			}
			UpdateList();
		}

		private void lstDropLoot_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (mChangingName)
			{
				return;
			}

			if (lstDropLoot.SelectedNode == null || lstDropLoot.SelectedNode.Tag == null)
			{
				return;
			}

			mEditorItem = DropPoolBase.Get((Guid)lstDropLoot.SelectedNode.Tag);
			UpdateEditor();
		}

		private void lstDropLoot_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
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
			}
		}

		private void FrmDropPool_Load(object sender, EventArgs e)
		{
			UpdateEditor();
		}
	}
}
