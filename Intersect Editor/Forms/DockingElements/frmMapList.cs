using Intersect_Editor.Classes;
using System;
using System.Windows.Forms;
using DarkUI.Forms;
using Intersect_Editor.Classes.General;
using Intersect_Library.GameObjects.Maps.MapList;
using Intersect_Library.Localization;
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
                if (Globals.CurrentMap.Changed() && DarkMessageBox.ShowInformation(Strings.Get("mapping", "savemapdialogue"), Strings.Get("mapping", "savemap"), DarkDialogButton.YesNo) == DialogResult.Yes)
                {
                    SaveMap();
                }
                Globals.MainForm.EnterMap(((MapListMap)e.Node.Tag).MapNum);
            }
        }
        private void SaveMap()
        {
            if (Globals.CurrentTool == (int)EdittingTool.Selection)
            {
                if (Globals.Dragging == true)
                {
                    //Place the change, we done!
                    Globals.MapEditorWindow.ProcessSelectionMovement(Globals.CurrentMap, true);
                    Globals.MapEditorWindow.PlaceSelection();
                }
            }
            PacketSender.SendMap(Globals.CurrentMap);
        }
        private void frmMapList_Load(object sender, EventArgs e)
        {
            InitLocalization();
        }

        private void InitLocalization()
        {
            this.Text = Strings.Get("maplist","title");   
            btnChronological.Text  = Strings.Get("maplist", "chronological");
            toolSelectMap.Text = Strings.Get("maplist", "selectcurrent");
            btnNewMap.Text = Strings.Get("maplist", "newmap");
            btnNewFolder.Text = Strings.Get("maplist", "newfolder");
            btnRename.Text = Strings.Get("maplist", "rename");
            btnDelete.Text = Strings.Get("maplist", "delete");
            newMapToolStripMenuItem.Text = Strings.Get("maplist", "newmap");
            renameToolStripMenuItem.Text = Strings.Get("maplist", "rename");
            newFolderToolStripMenuItem.Text = Strings.Get("maplist", "newfolder");
            deleteToolStripMenuItem.Text = Strings.Get("maplist", "delete");
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
                DarkMessageBox.ShowError(Strings.Get("maplist","selecttorename"), Strings.Get("maplist","rename"));
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
                DarkMessageBox.ShowError(Strings.Get("maplist", "selecttodelete"), Strings.Get("maplist", "delete"));
            }
            else
            {
                if (DarkMessageBox.ShowWarning(Strings.Get("maplist","deleteconfirm",((MapListItem)mapTreeList.list.SelectedNode.Tag).Name), Strings.Get("maplist", "delete"), DarkDialogButton.YesNo) == System.Windows.Forms.DialogResult.Yes)
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
            if (DarkMessageBox.ShowWarning(Strings.Get("mapping", "newmap"), Strings.Get("mapping", "newmapcaption"), DarkDialogButton.YesNo) != DialogResult.Yes) return;
            if (Globals.CurrentMap.Changed() && DarkMessageBox.ShowInformation(Strings.Get("mapping", "savemapdialogue"), Strings.Get("mapping", "savemap"), DarkDialogButton.YesNo) == DialogResult.Yes)
            {
                SaveMap();
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

        private void toolSelectMap_Click(object sender, EventArgs e)
        {
            mapTreeList.UpdateMapList(Globals.CurrentMap.MyMapNum);
        }
    }
}
