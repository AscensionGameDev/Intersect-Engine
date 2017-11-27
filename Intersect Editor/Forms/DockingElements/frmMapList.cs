using System;
using System.Windows.Forms;
using DarkUI.Forms;
using Intersect.Editor.Classes;
using Intersect.Editor.Classes.General;
using Intersect.GameObjects.Maps.MapList;
using Intersect.Editor.Classes.Localization;
using WeifenLuo.WinFormsUI.Docking;

namespace Intersect.Editor.Forms
{
    public partial class FrmMapList : DockContent
    {
        public FrmMapList()
        {
            InitializeComponent();

            //Enable Editting of the list
            mapTreeList.EnableEditing(contextMenuStrip1);
            mapTreeList.SetDoubleClick(NodeDoubleClick);
        }

        private void NodeDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag.GetType() == typeof(MapListMap))
            {
                if (Globals.CurrentMap != null && Globals.CurrentMap.Changed() &&
                    DarkMessageBox.ShowInformation(Strings.mapping.savemapdialogue,
                        Strings.mapping.savemap, DarkDialogButton.YesNo, Properties.Resources.Icon) ==
                    DialogResult.Yes)
                {
                    SaveMap();
                }
                Globals.MainForm.EnterMap(((MapListMap) e.Node.Tag).MapNum);
            }
        }

        private void SaveMap()
        {
            if (Globals.CurrentTool == (int) EditingTool.Selection)
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
            Text = Strings.maplist.title;
            btnChronological.Text = Strings.maplist.chronological;
            toolSelectMap.Text = Strings.maplist.selectcurrent;
            btnNewMap.Text = Strings.maplist.newmap;
            btnNewFolder.Text = Strings.maplist.newfolder;
            btnRename.Text = Strings.maplist.rename;
            btnDelete.Text = Strings.maplist.delete;
            newMapToolStripMenuItem.Text = Strings.maplist.newmap;
            renameToolStripMenuItem.Text = Strings.maplist.rename;
            newFolderToolStripMenuItem.Text = Strings.maplist.newfolder;
            deleteToolStripMenuItem.Text = Strings.maplist.delete;
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
                PacketSender.SendAddFolder((MapListItem) mapTreeList.list.SelectedNode.Tag);
            }
        }

        private void btnRename_Click(object sender, EventArgs e)
        {
            if (mapTreeList.list.SelectedNode == null)
            {
                DarkMessageBox.ShowError(Strings.maplist.selecttorename, Strings.maplist.rename,
                    DarkDialogButton.Ok, Properties.Resources.Icon);
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
                DarkMessageBox.ShowError(Strings.maplist.selecttodelete, Strings.maplist.delete,
                    DarkDialogButton.Ok, Properties.Resources.Icon);
            }
            else
            {
                if (
                    DarkMessageBox.ShowWarning(
                        Strings.maplist.deleteconfirm.ToString( ((MapListItem) mapTreeList.list.SelectedNode.Tag).Name),
                        Strings.maplist.delete, DarkDialogButton.YesNo, Properties.Resources.Icon) ==
                    DialogResult.Yes)
                {
                    PacketSender.SendDelete((MapListItem) mapTreeList.list.SelectedNode.Tag);
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
                DarkMessageBox.ShowWarning(Strings.mapping.newmap, Strings.mapping.newmapcaption,
                    DarkDialogButton.YesNo, Properties.Resources.Icon) != DialogResult.Yes) return;
            if (Globals.CurrentMap.Changed() &&
                DarkMessageBox.ShowInformation(Strings.mapping.savemapdialogue,
                    Strings.mapping.savemap, DarkDialogButton.YesNo, Properties.Resources.Icon) ==
                DialogResult.Yes)
            {
                SaveMap();
            }
            if (mapTreeList.list.SelectedNode == null)
            {
                PacketSender.SendCreateMap(-1, Globals.CurrentMap.Index, null);
            }
            else
            {
                PacketSender.SendCreateMap(-1, Globals.CurrentMap.Index,
                    (MapListItem) mapTreeList.list.SelectedNode.Tag);
            }
        }

        private void toolSelectMap_Click(object sender, EventArgs e)
        {
            mapTreeList.UpdateMapList(Globals.CurrentMap.Index);
        }
    }
}