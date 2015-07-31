using Intersect_Editor.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Intersect_Editor.Forms
{
    public partial class frmMapList : DockContent
    {
        //Cross Thread Delegates
        public delegate void TryUpdateMapList();
        public TryUpdateMapList MapListDelegate;

        public frmMapList()
        {
            InitializeComponent();
            //Init Delegates
            MapListDelegate = new TryUpdateMapList(UpdateMapList);
        }
        private void frmMapList_Load(object sender, EventArgs e)
        {
        }
        private void btnRefreshList_Click(object sender, EventArgs e)
        {
            UpdateMapList();
        }
        private void UpdateMapList()
        {
            treeMapList.Nodes.Clear();
            AddMapListToTree(Database.MapStructure, null);
        }
        private void AddMapListToTree(MapList mapList, TreeNode parent)
        {
            TreeNode tmpNode;
            if (btnChronological.Checked)
            {
                for (int i = 0; i < Database.OrderedMaps.Count; i++)
                {
                    tmpNode = treeMapList.Nodes.Add(Database.OrderedMaps[i].MapNum + ". " + Database.OrderedMaps[i].Name);
                    tmpNode.Tag = (Database.OrderedMaps[i]);
                    tmpNode.ImageIndex = 1;
                    tmpNode.SelectedImageIndex = 1;
                }
                treeMapList.Sort();
            }
            else
            {
                for (int i = 0; i < mapList.Items.Count; i++)
                {
                    if (mapList.Items[i].GetType() == typeof(FolderDirectory))
                    {
                        if (parent == null)
                        {
                            tmpNode = treeMapList.Nodes.Add(mapList.Items[i].Name);
                            tmpNode.Tag = ((FolderDirectory)mapList.Items[i]);
                            AddMapListToTree(((FolderDirectory)mapList.Items[i]).Children, tmpNode);
                        }
                        else
                        {
                            tmpNode = parent.Nodes.Add(mapList.Items[i].Name);
                            tmpNode.Tag = ((FolderDirectory)mapList.Items[i]);
                            AddMapListToTree(((FolderDirectory)mapList.Items[i]).Children, tmpNode);
                        }
                        tmpNode.ImageIndex = 0;
                        tmpNode.SelectedImageIndex = 0;
                    }
                    else
                    {
                        if (parent == null)
                        {
                            tmpNode = treeMapList.Nodes.Add(((FolderMap)mapList.Items[i]).MapNum + ". " + mapList.Items[i].Name);
                            tmpNode.Tag = ((FolderMap)mapList.Items[i]);
                        }
                        else
                        {
                            tmpNode = parent.Nodes.Add(((FolderMap)mapList.Items[i]).MapNum + ". " + mapList.Items[i].Name);
                            tmpNode.Tag = ((FolderMap)mapList.Items[i]);
                        }
                        tmpNode.ImageIndex = 1;
                        tmpNode.SelectedImageIndex = 1;
                    }
                }
            }
        }

        private void treeMapList_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void treeMapList_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void treeMapList_DragDrop(object sender, DragEventArgs e)
        {
            // Retrieve the client coordinates of the drop location.
            System.Drawing.Point targetPoint = treeMapList.PointToClient(new System.Drawing.Point(e.X, e.Y));

            // Retrieve the node at the drop location.
            TreeNode targetNode = treeMapList.GetNodeAt(targetPoint);

            // Retrieve the node that was dragged.
            TreeNode draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));
            int srcType = -1;
            int srcId = -1;

            if (draggedNode.Tag.GetType() == typeof(FolderMap))
            {
                srcType = 1;
                srcId = ((FolderMap)draggedNode.Tag).MapNum;
            }
            else
            {
                srcType = 0;
                srcId = ((FolderDirectory)draggedNode.Tag).FolderId;
            }

            // Confirm that the node at the drop location is not 
            // the dragged node and that target node isn't null
            // (for example if you drag outside the control)
            if (!draggedNode.Equals(targetNode) && targetNode != null)
            {
                if (targetNode.Tag.GetType() == typeof(FolderMap))
                {
                    int destType = 1;
                    int destId = ((FolderMap)targetNode.Tag).MapNum;
                    PacketSender.SendMapListMove(srcType, srcId, destType, destId);
                    // Remove the node from its current 
                    // location and add it to the node at the drop location.
                    draggedNode.Remove();

                    if (targetNode.Parent == null)
                    {
                        treeMapList.Nodes.Insert(targetNode.Index, draggedNode);
                    }
                    else
                    {
                        targetNode.Parent.Nodes.Insert(targetNode.Index, draggedNode);
                    }

                    // Expand the node at the location 
                    // to show the dropped node.
                    targetNode.Expand();
                }
                else
                {
                    int destType = 0;
                    int destId = ((FolderDirectory)targetNode.Tag).FolderId;
                    PacketSender.SendMapListMove(srcType, srcId, destType, destId);
                    // Remove the node from its current 
                    // location and add it to the node at the drop location.
                    draggedNode.Remove();
                    targetNode.Nodes.Add(draggedNode);

                    // Expand the node at the location 
                    // to show the dropped node.
                    targetNode.Expand();
                }
            }
            else if (treeMapList.ClientRectangle.Contains(targetPoint))
            {
                int destType = -1;
                int destId = -1;
                PacketSender.SendMapListMove(srcType, srcId, destType, destId);
                // Remove the node from its current 
                // location and add it to the node at the drop location.
                draggedNode.Remove();
                treeMapList.Nodes.Add(draggedNode);
            }
        }

        private void btnNewFolder_Click(object sender, EventArgs e)
        {
            if (treeMapList.SelectedNode == null)
            {
                PacketSender.SendAddFolder(null);
            }
            else
            {
                PacketSender.SendAddFolder((FolderItem)treeMapList.SelectedNode.Tag);
            }
        }

        private void btnRename_Click(object sender, EventArgs e)
        {
            if (treeMapList.SelectedNode == null)
            {
                MessageBox.Show(@"Please select a folder or map to rename.", @"Rename");
            }
            else
            {
                treeMapList.SelectedNode.BeginEdit();
            }
        }

        private void treeMapList_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node != null)
            {
                if(!String.IsNullOrEmpty(e.Label))
                {
                    if (e.Node.Tag.GetType() == typeof (FolderMap))
                    {
                        ((FolderMap)e.Node.Tag).Name = e.Label;
                        //Send Rename Map
                        PacketSender.SendRename((FolderItem)e.Node.Tag, e.Label);
                        e.Node.Text = ((FolderMap) e.Node.Tag).MapNum + @". " +
                                      ((FolderMap)e.Node.Tag).Name;
                    }
                    else
                    {
                        ((FolderDirectory) e.Node.Tag).Name = e.Label;
                        //Send Rename Folder
                        PacketSender.SendRename((FolderItem)e.Node.Tag, e.Label);
                        e.Node.Text = e.Label;
                    }
                }
                else
                {
                    if (e.Node.Tag.GetType() == typeof(FolderMap))
                    {
                        e.Node.Text = ((FolderMap)e.Node.Tag).MapNum + @". " +
                                      ((FolderMap)e.Node.Tag).Name;
                    }
                    else
                    {
                        e.Node.Text = e.Label;
                    } 
                }
            }
            e.CancelEdit = true;
        }

        private void treeMapList_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node != null)
            {
                if (e.Node.Tag.GetType() == typeof (FolderMap))
                {
                    if (e.Node.Text ==
                        ((FolderMap) e.Node.Tag).MapNum + @". " +
                        ((FolderMap)e.Node.Tag).Name)
                    {
                        e.Node.Text = ((FolderMap)e.Node.Tag).Name;
                        e.CancelEdit = true;
                        this.BeginInvoke(new Action(() => beginEdit(e.Node)));
                    }
                }
            }
        }

        private void beginEdit(TreeNode node)
        {
            node.BeginEdit();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (treeMapList.SelectedNode == null)
            {
                MessageBox.Show(@"Please select a folder or map to delete.", @"Delete");
            }
            else
            {
                if (MessageBox.Show(@"Are you sure you want to delete " + ((FolderItem)treeMapList.SelectedNode.Tag).Name, @"Delete", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    PacketSender.SendDelete((FolderItem)treeMapList.SelectedNode.Tag);
                }
            }
        }

        private void treeMapList_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void treeMapList_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag.GetType() == typeof(FolderMap))
            {
                Globals.MainForm.EnterMap(((FolderMap)e.Node.Tag).MapNum);
            }
        }

        private void btnChronological_Click(object sender, EventArgs e)
        {
            btnChronological.Checked = !btnChronological.Checked;
            UpdateMapList();
        }

        private void btnNewMap_Click(object sender, EventArgs e)
        {
            if (
                MessageBox.Show(@"Are you sure you want to create a new, unconnected map?", @"New Map",
                    MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            if (MessageBox.Show(@"Do you want to save your current map?", @"Save current map?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                PacketSender.SendMap(Globals.CurrentMap);
            }
            if (treeMapList.SelectedNode == null)
            {
                PacketSender.SendCreateMap(-1,Globals.CurrentMap,null);
            }
            else
            {
                PacketSender.SendCreateMap(-1, Globals.CurrentMap, (FolderItem)treeMapList.SelectedNode.Tag);
            }
        }


       

    }
}
