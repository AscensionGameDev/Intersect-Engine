using DarkUI.Forms;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
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
	public partial class frmHDV : EditorForm
	{
		private List<HDVBase> mChanged = new List<HDVBase>();

		private string mCopiedItem;

		private HDVBase mEditorItem;

		private List<string> mExpandedFolders = new List<string>();

		private List<string> mKnownFolders = new List<string>();

		public frmHDV()
		{
			ApplyHooks();
			InitializeComponent();
			lstHdv.LostFocus += itemList_FocusChanged;
			lstHdv.GotFocus += itemList_FocusChanged;
		}

		protected override void GameObjectUpdatedDelegate(GameObjectType type)
		{
			if (type == GameObjectType.HDVs)
			{
				InitEditor();
				if (mEditorItem != null && !HDVBase.Lookup.Values.Contains(mEditorItem))
				{
					mEditorItem = null;
					UpdateEditor();
				}
			}
		}

		private void UpdateToolStripItems()
		{
			toolStripItemCopy.Enabled = mEditorItem != null && lstHdv.Focused;
			toolStripItemPaste.Enabled = mEditorItem != null && mCopiedItem != null && lstHdv.Focused;
			toolStripItemDelete.Enabled = mEditorItem != null && lstHdv.Focused;
			toolStripItemUndo.Enabled = mEditorItem != null && lstHdv.Focused;
		}

		private void itemList_FocusChanged(object sender, EventArgs e)
		{
			UpdateToolStripItems();
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
			PacketSender.SendCreateObject(GameObjectType.HDVs);
		}

		private void toolStripItemDelete_Click(object sender, EventArgs e)
		{
			if (mEditorItem != null && lstHdv.Focused)
			{
				if (DarkMessageBox.ShowWarning(
						"Etes vous sur de vouloir supprimer l'hdv", "Supprimer l'HDV", DarkDialogButton.YesNo,
						Properties.Resources.Icon
					) ==
					DialogResult.Yes)
				{
					PacketSender.SendDeleteObject(mEditorItem);
				}
			}
		}

		public void InitEditor()
		{
			var selectedId = Guid.Empty;
			var folderNodes = new Dictionary<string, TreeNode>();
			if (lstHdv.SelectedNode != null && lstHdv.SelectedNode.Tag != null)
			{
				selectedId = (Guid)lstHdv.SelectedNode.Tag;
			}

			lstHdv.Nodes.Clear();

			//Collect folders
			var mFolders = new List<string>();
			foreach (var itm in HDVBase.Lookup)
			{
				if (!string.IsNullOrEmpty(((HDVBase)itm.Value).Folder) &&
					!mFolders.Contains(((HDVBase)itm.Value).Folder))
				{
					mFolders.Add(((HDVBase)itm.Value).Folder);
					if (!mKnownFolders.Contains(((HDVBase)itm.Value).Folder))
					{
						mKnownFolders.Add(((HDVBase)itm.Value).Folder);
					}
				}
			}

			mFolders.Sort();
			mKnownFolders.Sort();
			cmbFolder.Items.Clear();
			cmbFolder.Items.Add("");
			cmbFolder.Items.AddRange(mKnownFolders.ToArray());

			lstHdv.Sorted = !btnChronological.Checked;

			if (!btnChronological.Checked)
			{
				foreach (var folder in mFolders)
				{
					var node = lstHdv.Nodes.Add(folder);
					node.ImageIndex = 0;
					node.SelectedImageIndex = 0;
					folderNodes.Add(folder, node);
				}
			}

			foreach (var itm in HDVBase.ItemPairs)
			{
				var node = new TreeNode(itm.Value);
				node.Tag = itm.Key;
				node.ImageIndex = 1;
				node.SelectedImageIndex = 1;

				var folder = HDVBase.Get(itm.Key).Folder;
				if (!string.IsNullOrEmpty(folder) && !btnChronological.Checked)
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
					lstHdv.Nodes.Add(node);
				}

				if (itm.Key == selectedId)
				{
					lstHdv.SelectedNode = node;
				}
			}

			var selectedNode = lstHdv.SelectedNode;

			if (!btnChronological.Checked)
			{
				lstHdv.Sort();
			}

			lstHdv.SelectedNode = selectedNode;
			foreach (var node in mExpandedFolders)
			{
				if (folderNodes.ContainsKey(node))
				{
					folderNodes[node].Expand();
				}
			}
			cmbAddItem.Items.Clear();
			cmbAddItem.Items.AddRange(ItemBase.Names);
		}

		private void btnChronological_Click(object sender, EventArgs e)
		{
			btnChronological.Checked = !btnChronological.Checked;
			InitEditor();
		}

		private void toolStripItemCopy_Click(object sender, EventArgs e)
		{
			if (mEditorItem != null && lstHdv.Focused)
			{
				mCopiedItem = mEditorItem.JsonData;
				toolStripItemPaste.Enabled = true;
			}
		}

		private void toolStripItemPaste_Click(object sender, EventArgs e)
		{
			if (mEditorItem != null && mCopiedItem != null && lstHdv.Focused)
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
						"Etes vous sur de vouloir faire un retour en arriere ?","Retour", DarkDialogButton.YesNo,
						Properties.Resources.Icon
					) ==
					DialogResult.Yes)
				{
					mEditorItem.RestoreBackup();
					UpdateEditor();
				}
			}
		}

		private void UpdateList()
		{
			if (mEditorItem == null)
			{
				return;
			}
			lstItems.Items.Clear();
			if (mEditorItem.ItemListed == null)
			{
				mEditorItem.ItemListed = new List<Guid>();
			}
			foreach(Guid id in mEditorItem.ItemListed)
			{
				lstItems.Items.Add(ItemBase.Get(id).Name);
			}
		}

		private void UpdateEditor()
		{
			UpdateList();
			if (mEditorItem != null)
			{
				pnlContainer.Show();

				txtName.Text = mEditorItem.Name;
				cmbFolder.Text = mEditorItem.Folder;
				cmbDefaultCurrency.SelectedIndex = ItemBase.ListIndex(mEditorItem.CurrencyId);

				if (mEditorItem.isWhiteList)
				{
					rdoWhitelist.Checked = true;
					rdoBlacklist.Checked = false;
				}
				else
				{
					rdoWhitelist.Checked = false;
					rdoBlacklist.Checked = true;
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

		private void lstHdv_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (mChangingName)
			{
				return;
			}

			if (lstHdv.SelectedNode == null || lstHdv.SelectedNode.Tag == null)
			{
				return;
			}

			mEditorItem = HDVBase.Get((Guid)lstHdv.SelectedNode.Tag);
			UpdateEditor();
		}

		private void lstHdv_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
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

				var hitTest = lstHdv.HitTest(e.Location);
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

		private void cmbFolder_SelectedIndexChanged(object sender, EventArgs e)
		{
			mEditorItem.Folder = cmbFolder.Text;
			InitEditor();
		}

		private void btnAddFolder_Click(object sender, EventArgs e)
		{
			var folderName = "";
			var result = DarkInputBox.ShowInformation(
				"Entrer un nom de dossier", "Dossier", ref folderName,
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

		private void cmbDefaultCurrency_SelectedIndexChanged(object sender, EventArgs e)
		{
			mEditorItem.Currency = ItemBase.FromList(cmbDefaultCurrency.SelectedIndex);
		}

		private void txtName_TextChanged(object sender, EventArgs e)
		{
			mChangingName = true;
			mEditorItem.Name = txtName.Text;
			if (lstHdv.SelectedNode != null && lstHdv.SelectedNode.Tag != null)
			{
				lstHdv.SelectedNode.Text = txtName.Text;
			}

			mChangingName = false;
		}

		private void frmHDV_Load(object sender, EventArgs e)
		{
			cmbDefaultCurrency.Items.Clear();
			cmbDefaultCurrency.Items.AddRange(ItemBase.Names);
			UpdateEditor();
		}

		private void btnAddItem_Click(object sender, EventArgs e)
		{
			Guid ItemId = ItemBase.IdFromList(cmbAddItem.SelectedIndex);
			if (ItemId != null && ItemId != Guid.Empty)
			{
				if (!mEditorItem.ItemListed.Contains(ItemId))
				{
					mEditorItem.ItemListed.Add(ItemId);
				}
			}
			UpdateList();
		}

		private void btnDelItem_Click(object sender, EventArgs e)
		{
			if (lstItems.SelectedIndex > -1)
			{
				mEditorItem.ItemListed.RemoveAt(lstItems.SelectedIndex);
			}
			UpdateList();
		}

		private void rdoBuyWhitelist_CheckedChanged(object sender, EventArgs e)
		{
			mEditorItem.isWhiteList = rdoWhitelist.Checked;
		}

		private void rdoBuyBlacklist_CheckedChanged(object sender, EventArgs e)
		{
			mEditorItem.isWhiteList = !rdoWhitelist.Checked;
		}
	}
}
