using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Intersect.Editor.Classes;
using Intersect.GameObjects.Maps.MapList;

namespace Intersect.Editor.Forms.Controls
{
    public partial class MapTreeList : UserControl
    {
        //Cross Thread Delegates
        public delegate void TryUpdateMapList(int selectMap = -1, List<int> restrictMaps = null);

        private bool _canEdit = false;
        private List<int> _restrictMaps = null;
        public bool Chronological = false;
        public TryUpdateMapList MapListDelegate;
        private List<int> OpenFolders = new List<int>();
        private System.Drawing.Point ScrollPoint;
        private int SelectedMap = -1; //id or map or folder that is selected
        private int SelectionType = -1; //0 for none, 1 for map, 2 for folder

        public MapTreeList()
        {
            InitializeComponent();
            //Init Delegates
            MapListDelegate = UpdateMapList;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

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
            TreeNode draggedNode = (TreeNode) e.Data.GetData(typeof(TreeNode));
            int srcType = -1;
            int srcId = -1;

            if (draggedNode.Tag.GetType() == typeof(MapListMap))
            {
                srcType = 1;
                srcId = ((MapListMap) draggedNode.Tag).MapNum;
            }
            else
            {
                srcType = 0;
                srcId = ((MapListFolder) draggedNode.Tag).FolderId;
            }

            // Confirm that the node at the drop location is not 
            // the dragged node and that target node isn't null
            // (for example if you drag outside the control)
            if (!draggedNode.Equals(targetNode) && targetNode != null)
            {
                if (targetNode.Tag.GetType() == typeof(MapListMap))
                {
                    int destType = 1;
                    int destId = ((MapListMap) targetNode.Tag).MapNum;
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
                    int destId = ((MapListFolder) targetNode.Tag).FolderId;
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
                if (!string.IsNullOrEmpty(e.Label))
                {
                    if (e.Node.Tag.GetType() == typeof(MapListMap))
                    {
                        ((MapListMap) e.Node.Tag).Name = e.Label;
                        //Send Rename Map
                        PacketSender.SendRename((MapListItem) e.Node.Tag, e.Label);
                        e.Node.Text = ((MapListMap) e.Node.Tag).Name;
                    }
                    else
                    {
                        ((MapListFolder) e.Node.Tag).Name = e.Label;
                        //Send Rename Folder
                        PacketSender.SendRename((MapListItem) e.Node.Tag, e.Label);
                        e.Node.Text = e.Label;
                    }
                }
                else
                {
                    if (e.Node.Tag.GetType() == typeof(MapListMap))
                    {
                        e.Node.Text = ((MapListMap) e.Node.Tag).Name;
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
                    if (e.Node.Text == ((MapListMap) e.Node.Tag).Name && Chronological)
                    {
                        e.Node.Text = ((MapListMap) e.Node.Tag).Name;
                    }
                }
            }
        }

        private void treeMapList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag.GetType() == typeof(MapListMap))
            {
                SelectionType = 0;
                SelectedMap = ((MapListMap) e.Node.Tag).MapNum;
            }
            else if (e.Node.Tag.GetType() == typeof(MapListFolder))
            {
                SelectionType = 1;
                SelectedMap = ((MapListFolder) e.Node.Tag).FolderId;
            }
            else
            {
                SelectionType = -1;
                SelectedMap = -1;
            }
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

        public void UpdateMapList(int selectMap = -1, List<int> restrictMaps = null)
        {
            list.Nodes.Clear();
            _restrictMaps = restrictMaps;
            AddMapListToTree(MapList.GetList(), null, selectMap, _restrictMaps);
        }

        private void AddMapListToTree(MapList mapList, TreeNode parent, int selectMap = -1,
            List<int> restrictMaps = null)
        {
            TreeNode tmpNode;
            if (Chronological)
            {
                for (int i = 0; i < MapList.GetOrderedMaps().Count; i++)
                {
                    if (restrictMaps == null || restrictMaps.Contains(MapList.GetOrderedMaps()[i].MapNum))
                    {
                        tmpNode = list.Nodes.Add(MapList.GetOrderedMaps()[i].Name);
                        tmpNode.Tag = (MapList.GetOrderedMaps()[i]);
                        tmpNode.ImageIndex = 1;
                        tmpNode.SelectedImageIndex = 1;
                        if (selectMap != -1)
                        {
                            if (MapList.GetOrderedMaps()[i].MapNum == selectMap)
                            {
                                list.SelectedNode = tmpNode;
                                list.Focus();
                            }
                        }
                        else
                        {
                            if (SelectionType == 0 && SelectedMap == MapList.GetOrderedMaps()[i].MapNum)
                            {
                                list.SelectedNode = tmpNode;
                                list.Focus();
                            }
                        }
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
                            tmpNode.Tag = ((MapListFolder) mapList.Items[i]);
                            AddMapListToTree(((MapListFolder) mapList.Items[i]).Children, tmpNode, selectMap,
                                restrictMaps);
                        }
                        else
                        {
                            tmpNode = parent.Nodes.Add(mapList.Items[i].Name);
                            tmpNode.Tag = ((MapListFolder) mapList.Items[i]);
                            AddMapListToTree(((MapListFolder) mapList.Items[i]).Children, tmpNode, selectMap,
                                restrictMaps);
                        }
                        if (OpenFolders.Contains(((MapListFolder) mapList.Items[i]).FolderId))
                        {
                            tmpNode.Expand();
                        }
                        if (SelectionType == 1 && SelectedMap == ((MapListFolder) mapList.Items[i]).FolderId)
                        {
                            list.SelectedNode = tmpNode;
                            list.Focus();
                        }
                        tmpNode.ImageIndex = 0;
                        tmpNode.SelectedImageIndex = 0;
                    }
                    else
                    {
                        if (restrictMaps == null || restrictMaps.Contains(((MapListMap) mapList.Items[i]).MapNum))
                        {
                            if (parent == null)
                            {
                                tmpNode = list.Nodes.Add(mapList.Items[i].Name);
                                tmpNode.Tag = ((MapListMap) mapList.Items[i]);
                            }
                            else
                            {
                                tmpNode = parent.Nodes.Add(mapList.Items[i].Name);
                                tmpNode.Tag = ((MapListMap) mapList.Items[i]);
                            }
                            if (selectMap != -1)
                            {
                                if (((MapListMap) mapList.Items[i]).MapNum == selectMap)
                                {
                                    list.SelectedNode = tmpNode;
                                    list.Focus();
                                }
                            }
                            else
                            {
                                if (SelectionType == 0 && SelectedMap == ((MapListMap) mapList.Items[i]).MapNum)
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

        public void SetSelect(TreeViewEventHandler handler)
        {
            list.AfterSelect += handler;
        }

        private void list_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag.GetType() == typeof(MapListFolder))
            {
                if (!OpenFolders.Contains(((MapListFolder) e.Node.Tag).FolderId))
                    OpenFolders.Add(((MapListFolder) e.Node.Tag).FolderId);
            }
        }

        private void list_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag.GetType() == typeof(MapListFolder))
            {
                if (OpenFolders.Contains(((MapListFolder) e.Node.Tag).FolderId))
                    OpenFolders.Remove(((MapListFolder) e.Node.Tag).FolderId);
            }
        }

        public static void Scroll(Control control)
        {
            var pt = control.PointToClient(Cursor.Position);

            if ((pt.Y + 20) > control.Height)
            {
                // scroll down
                SendMessage(control.Handle, 277, (IntPtr) 1, (IntPtr) 0);
            }
            else if (pt.Y < 20)
            {
                // scroll up
                SendMessage(control.Handle, 277, (IntPtr) 0, (IntPtr) 0);
            }
        }

        private void list_DragOver(object sender, DragEventArgs e)
        {
            Scroll(list);
        }
    }
}