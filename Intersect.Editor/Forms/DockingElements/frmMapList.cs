using System;
using System.Windows.Forms;

using DarkUI.Forms;

using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Editor.Networking;
using Intersect.GameObjects.Maps.MapList;

using WeifenLuo.WinFormsUI.Docking;

namespace Intersect.Editor.Forms.DockingElements
{

    public partial class FrmMapList : DockContent
    {

        public FrmMapList()
        {
            InitializeComponent();
            Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetExecutingAssembly().Location);

            //Enable Editting of the list
            mapTreeList.EnableEditing(contextMenuStrip1);
            mapTreeList.SetDoubleClick(NodeDoubleClick);
        }

        private void NodeDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag is MapListMap map)
            {
                if (Globals.CurrentMap != null &&
                    Globals.CurrentMap.Changed() &&
                    DarkMessageBox.ShowInformation(
                        Strings.Mapping.savemapdialogue, Strings.Mapping.savemap, DarkDialogButton.YesNo,
                        Icon
                    ) ==
                    DialogResult.Yes)
                {
                    SaveMap();
                }

                Globals.MainForm.EnterMap(map.MapId, true);
            }
        }

        private void SaveMap()
        {
            if (Globals.CurrentTool == (int) EditingTool.MarqueeSelection)
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
            Text = Strings.MapList.title;
            btnAlphabetical.Text = Strings.MapList.alphabetical;
            toolSelectMap.Text = Strings.MapList.selectcurrent;
            btnNewMap.Text = Strings.MapList.newmap;
            btnNewFolder.Text = Strings.MapList.newfolder;
            btnRename.Text = Strings.MapList.rename;
            btnDelete.Text = Strings.MapList.delete;
            newMapToolStripMenuItem.Text = Strings.MapList.newmap;
            renameToolStripMenuItem.Text = Strings.MapList.rename;
            newFolderToolStripMenuItem.Text = Strings.MapList.newfolder;
            deleteToolStripMenuItem.Text = Strings.MapList.delete;
            copyIdToolStripMenuItem.Text = Strings.MapList.copyid;
        }

        private void btnRefreshList_Click(object sender, EventArgs e)
        {
            mapTreeList.UpdateMapList();
        }

        private void btnNewFolder_Click(object sender, EventArgs e)
        {
            PacketSender.SendAddFolder(mapTreeList.list.SelectedNode?.Tag as MapListItem);
        }

        private void btnRename_Click(object sender, EventArgs e)
        {
            if (mapTreeList.list.SelectedNode == null)
            {
                DarkMessageBox.ShowError(
                    Strings.MapList.selecttorename, Strings.MapList.rename, DarkDialogButton.Ok,
                    Icon
                );
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
                DarkMessageBox.ShowError(
                    Strings.MapList.selecttodelete, Strings.MapList.delete, DarkDialogButton.Ok,
                    Icon
                );
            }
            else
            {
                if (DarkMessageBox.ShowWarning(
                        Strings.MapList.deleteconfirm.ToString(((MapListItem) mapTreeList.list.SelectedNode.Tag).Name),
                        Strings.MapList.delete, DarkDialogButton.YesNo, Icon
                    ) ==
                    DialogResult.Yes)
                {
                    PacketSender.SendDelete((MapListItem) mapTreeList.list.SelectedNode.Tag);
                }
            }
        }

        private void btnAlphabetical_Click(object sender, EventArgs e)
        {
            btnAlphabetical.Checked = !btnAlphabetical.Checked;
            mapTreeList.Chronological = btnAlphabetical.Checked;
            mapTreeList.UpdateMapList();
        }

        private void btnNewMap_Click(object sender, EventArgs e)
        {
            if (DarkMessageBox.ShowWarning(
                    Strings.Mapping.newmap, Strings.Mapping.newmapcaption, DarkDialogButton.YesNo,
                    Icon
                ) !=
                DialogResult.Yes)
            {
                return;
            }

            if (Globals.CurrentMap.Changed() &&
                DarkMessageBox.ShowInformation(
                    Strings.Mapping.savemapdialogue, Strings.Mapping.savemap, DarkDialogButton.YesNo,
                    Icon
                ) ==
                DialogResult.Yes)
            {
                SaveMap();
            }

            if (mapTreeList.list.SelectedNode == null)
            {
                PacketSender.SendCreateMap(-1, Globals.CurrentMap.Id, null);
            }
            else
            {
                PacketSender.SendCreateMap(-1, Globals.CurrentMap.Id, (MapListItem) mapTreeList.list.SelectedNode.Tag);
            }
        }

        private void toolSelectMap_Click(object sender, EventArgs e)
        {
            mapTreeList.UpdateMapList(Globals.CurrentMap.Id);
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var node = mapTreeList.list.SelectedNode;
            copyIdToolStripMenuItem.Visible = node != null && node.Tag is MapListMap;
        }

        private void copyIdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var node = mapTreeList.list.SelectedNode;
            if (node != null && node.Tag is MapListMap map)
            {
                var id = map.MapId;
                Clipboard.SetText(id.ToString());
            }
        }

    }

}
