using Intersect_Editor.Classes;
using System;
using System.Windows.Forms;
using Intersect_Library.GameObjects.Maps.MapList;
using WeifenLuo.WinFormsUI.Docking;

namespace Intersect_Editor.Forms
{
    public partial class frmMapList : DockContent
    {
        public frmMapList()
        {
            InitializeComponent();

            //Enable Editting of the list
            mapTreeList.EnableEditing(contextMenuStrip1);
            mapTreeList.SetDoubleClick(new TreeNodeMouseClickEventHandler(NodeDoubleClick));
        }

        private void NodeDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag.GetType() == typeof(MapListMap))
            {
                //Should ask if the user wants to save changes
                if (Globals.MapEditorWindow.MapUndoStates.Count > 0 && MessageBox.Show(@"Do you want to save your current map?", @"Save current map?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    PacketSender.SendMap(Globals.CurrentMap.GetId());
                }
                Globals.MainForm.EnterMap(((MapListMap)e.Node.Tag).MapNum);
            }
        }
        private void frmMapList_Load(object sender, EventArgs e)
        {
        }
        private void btnRefreshList_Click(object sender, EventArgs e)
        {
            mapTreeList.UpdateMapList();
        }
        private void btnNewFolder_Click(object sender, EventArgs e)
        {
            if (mapTreeList.list.SelectedNode == null)
            {
                PacketSender.SendAddFolder(null);
            }
            else
            {
                PacketSender.SendAddFolder((MapListItem)mapTreeList.list.SelectedNode.Tag);
            }
        }
        private void btnRename_Click(object sender, EventArgs e)
        {
            if (mapTreeList.list.SelectedNode == null)
            {
                MessageBox.Show(@"Please select a folder or map to rename.", @"Rename");
            }
            else
            {
                mapTreeList.list.SelectedNode.BeginEdit();
            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (mapTreeList.list.SelectedNode == null)
            {
                MessageBox.Show(@"Please select a folder or map to delete.", @"Delete");
            }
            else
            {
                if (MessageBox.Show(@"Are you sure you want to delete " + ((MapListItem)mapTreeList.list.SelectedNode.Tag).Name, @"Delete", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    PacketSender.SendDelete((MapListItem)mapTreeList.list.SelectedNode.Tag);
                }
            }
        }
        private void btnChronological_Click(object sender, EventArgs e)
        {
            btnChronological.Checked = !btnChronological.Checked;
            mapTreeList.Chronological = btnChronological.Checked;
            mapTreeList.UpdateMapList();
        }
        private void btnNewMap_Click(object sender, EventArgs e)
        {
            if (
                MessageBox.Show(@"Are you sure you want to create a new, unconnected map?", @"New Map",
                    MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            if (MessageBox.Show(@"Do you want to save your current map?", @"Save current map?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                PacketSender.SendMap(Globals.CurrentMap.GetId());
            }
            if (mapTreeList.list.SelectedNode == null)
            {
                PacketSender.SendCreateMap(-1,Globals.CurrentMap.GetId(),null);
            }
            else
            {
                PacketSender.SendCreateMap(-1, Globals.CurrentMap.GetId(), (MapListItem)mapTreeList.list.SelectedNode.Tag);
            }
        }
        private void btnGridView_Click(object sender, EventArgs e)
        {
            Globals.MapGridWindow.Visible = !Globals.MapGridWindow.Visible;
        }


       

    }
}
