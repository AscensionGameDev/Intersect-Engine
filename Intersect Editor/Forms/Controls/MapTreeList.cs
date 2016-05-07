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
using System.Windows.Forms;
using Intersect_Editor.Classes;
using Intersect_Library.GameObjects.Maps.MapList;

namespace Intersect_Editor.Forms.Controls
{
    public partial class MapTreeList : UserControl
    {

        //Cross Thread Delegates
        public delegate void TryUpdateMapList(int selectMap = -1);
        public TryUpdateMapList MapListDelegate;
        public bool Chronological = false;
        private bool _canEdit = false;

        public MapTreeList()
        {
            InitializeComponent();
            //Init Delegates
            MapListDelegate = UpdateMapList;
        }

        private void treeMapList_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (!_canEdit) return;
            DoDragDrop(e.Item, DragDropEffects.Move);
        }
        private void treeMapList_DragEnter(object sender, DragEventArgs e)
        {
            if (!_canEdit) return;
            e.Effect = DragDropEffects.Move;
        }
        private void treeMapList_DragDrop(object sender, DragEventArgs e)
        {
            if (!_canEdit) return;
            // Retrieve the client coordinates of the drop location.
            System.Drawing.Point targetPoint = list.PointToClient(new System.Drawing.Point(e.X, e.Y));

            // Retrieve the node at the drop location.
            TreeNode targetNode = list.GetNodeAt(targetPoint);

            // Retrieve the node that was dragged.
            TreeNode draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));
            int srcType = -1;
            int srcId = -1;

            if (draggedNode.Tag.GetType() == typeof(MapListMap))
            {
                srcType = 1;
                srcId = ((MapListMap)draggedNode.Tag).MapNum;
            }
            else
            {
                srcType = 0;
                srcId = ((MapListFolder)draggedNode.Tag).FolderId;
            }

            // Confirm that the node at the drop location is not 
            // the dragged node and that target node isn't null
            // (for example if you drag outside the control)
            if (!draggedNode.Equals(targetNode) && targetNode != null)
            {
                if (targetNode.Tag.GetType() == typeof(MapListMap))
                {
                    int destType = 1;
                    int destId = ((MapListMap)targetNode.Tag).MapNum;
                    PacketSender.SendMapListMove(srcType, srcId, destType, destId);
                    // Remove the node from its current 
                    // location and add it to the node at the drop location.
                    draggedNode.Remove();

                    if (targetNode.Parent == null)
                    {
                        list.Nodes.Insert(targetNode.Index, draggedNode);
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
                    int destId = ((MapListFolder)targetNode.Tag).FolderId;
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
            else if (list.ClientRectangle.Contains(targetPoint))
            {
                int destType = -1;
                int destId = -1;
                PacketSender.SendMapListMove(srcType, srcId, destType, destId);
                // Remove the node from its current 
                // location and add it to the node at the drop location.
                draggedNode.Remove();
                list.Nodes.Add(draggedNode);
            }
        }
        private void treeMapList_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (!_canEdit) return;
            if (e.Node != null)
            {
                if (!String.IsNullOrEmpty(e.Label))
                {
                    if (e.Node.Tag.GetType() == typeof(MapListMap))
                    {
                        ((MapListMap)e.Node.Tag).Name = e.Label;
                        //Send Rename Map
                        PacketSender.SendRename((MapListItem)e.Node.Tag, e.Label);
                        e.Node.Text = ((MapListMap)e.Node.Tag).Name;
                    }
                    else
                    {
                        ((MapListFolder)e.Node.Tag).Name = e.Label;
                        //Send Rename Folder
                        PacketSender.SendRename((MapListItem)e.Node.Tag, e.Label);
                        e.Node.Text = e.Label;
                    }
                }
                else
                {
                    if (e.Node.Tag.GetType() == typeof(MapListMap))
                    {
                        e.Node.Text = ((MapListMap)e.Node.Tag).Name;
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
            if (!_canEdit) return;
            if (e.Node != null)
            {
                if (e.Node.Tag.GetType() == typeof(MapListMap))
                {
                    if (e.Node.Text ==
                        ((MapListMap)e.Node.Tag).MapNum + @". " +
                        ((MapListMap)e.Node.Tag).Name && Chronological)
                    { 
                        e.Node.Text = ((MapListMap)e.Node.Tag).Name;
                        e.CancelEdit = true;
                        this.BeginInvoke(new Action(() => beginEdit(e.Node)));
                    }
                }
            }
        }
        private void treeMapList_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }
        private void treeMapList_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            
        }
        private void treeMapList_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                list.SelectedNode = list.GetNodeAt(e.Location);
            }

        }
        private void beginEdit(TreeNode node)
        {
            node.BeginEdit();
        }
        public void UpdateMapList(int selectMap = -1)
        {
            list.Nodes.Clear();
            AddMapListToTree(MapList.GetList(), null, selectMap);
        }
        private void AddMapListToTree(MapList mapList, TreeNode parent, int selectMap = -1)
        {
            TreeNode tmpNode;
            if (Chronological)
            {
                for (int i = 0; i < MapList.GetOrderedMaps().Count; i++)
                {
                    tmpNode = list.Nodes.Add(MapList.GetOrderedMaps()[i].MapNum + ". " + MapList.GetOrderedMaps()[i].Name);
                    tmpNode.Tag = (MapList.GetOrderedMaps()[i]);
                    tmpNode.ImageIndex = 1;
                    tmpNode.SelectedImageIndex = 1;
                    if (MapList.GetOrderedMaps()[i].MapNum == selectMap)
                    {
                        list.SelectedNode = tmpNode;
                        list.Focus();
                    }

                }
            }
            else
            {
                for (int i = 0; i < mapList.Items.Count; i++)
                {
                    if (mapList.Items[i].GetType() == typeof(MapListFolder))
                    {
                        if (parent == null)
                        {
                            tmpNode = list.Nodes.Add(mapList.Items[i].Name);
                            tmpNode.Tag = ((MapListFolder)mapList.Items[i]);
                            AddMapListToTree(((MapListFolder)mapList.Items[i]).Children, tmpNode,selectMap);
                        }
                        else
                        {
                            tmpNode = parent.Nodes.Add(mapList.Items[i].Name);
                            tmpNode.Tag = ((MapListFolder)mapList.Items[i]);
                            AddMapListToTree(((MapListFolder)mapList.Items[i]).Children, tmpNode,selectMap);
                        }
                        tmpNode.ImageIndex = 0;
                        tmpNode.SelectedImageIndex = 0;
                    }
                    else
                    {
                        if (parent == null)
                        {
                            tmpNode = list.Nodes.Add(mapList.Items[i].Name);
                            tmpNode.Tag = ((MapListMap)mapList.Items[i]);
                            if (((MapListMap) mapList.Items[i]).MapNum == selectMap)
                            {
                                list.SelectedNode = tmpNode;
                                list.Focus();
                            }
                        }
                        else
                        {
                            tmpNode = parent.Nodes.Add(mapList.Items[i].Name);
                            tmpNode.Tag = ((MapListMap)mapList.Items[i]);
                            if (((MapListMap) mapList.Items[i]).MapNum == selectMap)
                            {
                                list.SelectedNode = tmpNode;
                                list.Focus();
                            }
                        }
                        tmpNode.ImageIndex = 1;
                        tmpNode.SelectedImageIndex = 1;
                    }
                }
            }
        }

        public void EnableEditing(ContextMenuStrip menuStrip)
        {
            if (menuStrip != null) list.ContextMenuStrip = menuStrip;
            list.LabelEdit = true;
            _canEdit = true;
        }

        public void SetDoubleClick(TreeNodeMouseClickEventHandler handler)
        {
            list.NodeMouseDoubleClick += handler;
        }
    }
}
