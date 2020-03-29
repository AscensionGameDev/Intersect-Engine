using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Intersect.Editor.Networking;
using Intersect.GameObjects.Maps.MapList;

namespace Intersect.Editor.Forms.Controls
{

    public partial class MapTreeList : UserControl
    {

        //Cross Thread Delegates
        public delegate void TryUpdateMapList(Guid selectMap, List<Guid> restrictMaps = null);

        public bool Chronological = false;

        public TryUpdateMapList MapListDelegate;

        private bool mCanEdit;

        private List<Guid> mOpenFolders = new List<Guid>();

        private List<Guid> mRestrictMapIds;

        private System.Drawing.Point mScrollPoint;

        private Guid mSelectedMap = Guid.Empty; //id or map or folder that is selected

        private int mSelectionType = -1; //0 for none, 1 for map, 2 for folder

        public MapTreeList()
        {
            InitializeComponent();

            //Init Delegates
            MapListDelegate = UpdateMapList;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        private void treeMapList_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (!mCanEdit)
            {
                return;
            }

            DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void treeMapList_DragEnter(object sender, DragEventArgs e)
        {
            if (!mCanEdit)
            {
                return;
            }

            e.Effect = DragDropEffects.Move;
        }

        private void treeMapList_DragDrop(object sender, DragEventArgs e)
        {
            if (!mCanEdit)
            {
                return;
            }

            // Retrieve the client coordinates of the drop location.
            var targetPoint = list.PointToClient(new System.Drawing.Point(e.X, e.Y));

            // Retrieve the node at the drop location.
            var targetNode = list.GetNodeAt(targetPoint);

            // Retrieve the node that was dragged.
            var draggedNode = (TreeNode) e.Data.GetData(typeof(TreeNode));
            var srcType = -1;
            var srcId = Guid.Empty;

            if (draggedNode.Tag.GetType() == typeof(MapListMap))
            {
                srcType = 1;
                srcId = ((MapListMap) draggedNode.Tag).MapId;
            }
            else
            {
                srcType = 0;
                srcId = ((MapListFolder) draggedNode.Tag).FolderId;
            }

            var parent = targetNode;
            while (parent != null)
            {
                if (parent == draggedNode)
                {
                    return;
                }

                parent = parent.Parent;
            }

            // Confirm that the node at the drop location is not 
            // the dragged node and that target node isn't null
            // (for example if you drag outside the control)
            if (!draggedNode.Equals(targetNode) && targetNode != null)
            {
                if (targetNode.Tag.GetType() == typeof(MapListMap))
                {
                    var destType = 1;
                    var destId = ((MapListMap) targetNode.Tag).MapId;
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
                    var destType = 0;
                    var destId = ((MapListFolder) targetNode.Tag).FolderId;
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
                var destType = -1;
                var destId = Guid.Empty;
                PacketSender.SendMapListMove(srcType, srcId, destType, destId);

                // Remove the node from its current 
                // location and add it to the node at the drop location.
                draggedNode.Remove();
                list.Nodes.Add(draggedNode);
            }
        }

        private void treeMapList_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (!mCanEdit)
            {
                return;
            }

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
            if (!mCanEdit)
            {
                return;
            }

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
                mSelectionType = 0;
                mSelectedMap = ((MapListMap) e.Node.Tag).MapId;
            }
            else if (e.Node.Tag.GetType() == typeof(MapListFolder))
            {
                mSelectionType = 1;
                mSelectedMap = ((MapListFolder) e.Node.Tag).FolderId;
            }
            else
            {
                mSelectionType = -1;
                mSelectedMap = Guid.Empty;
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

        private void BeginEdit(TreeNode node)
        {
            node.BeginEdit();
        }

        public void UpdateMapList(Guid selectMapId = new Guid(), List<Guid> restrictMaps = null)
        {
            list.Nodes.Clear();
            mRestrictMapIds = restrictMaps;
            AddMapListToTree(MapList.List, null, selectMapId, mRestrictMapIds);
        }

        private void AddMapListToTree(
            MapList mapList,
            TreeNode parent,
            Guid selectMapId = new Guid(),
            List<Guid> restrictMaps = null
        )
        {
            TreeNode tmpNode;
            if (Chronological)
            {
                for (var i = 0; i < MapList.OrderedMaps.Count; i++)
                {
                    if (restrictMaps == null || restrictMaps.Contains(MapList.OrderedMaps[i].MapId))
                    {
                        tmpNode = list.Nodes.Add(MapList.OrderedMaps[i].Name);
                        tmpNode.Tag = MapList.OrderedMaps[i];
                        tmpNode.ImageIndex = 1;
                        tmpNode.SelectedImageIndex = 1;
                        if (selectMapId != Guid.Empty)
                        {
                            if (MapList.OrderedMaps[i].MapId == selectMapId)
                            {
                                list.SelectedNode = tmpNode;
                                list.Focus();
                            }
                        }
                        else
                        {
                            if (mSelectionType == 0 && mSelectedMap == MapList.OrderedMaps[i].MapId)
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
                for (var i = 0; i < mapList.Items.Count; i++)
                {
                    if (mapList.Items[i].GetType() == typeof(MapListFolder))
                    {
                        if (parent == null)
                        {
                            tmpNode = list.Nodes.Add(mapList.Items[i].Name);
                            tmpNode.Tag = (MapListFolder) mapList.Items[i];
                            AddMapListToTree(
                                ((MapListFolder) mapList.Items[i]).Children, tmpNode, selectMapId, restrictMaps
                            );
                        }
                        else
                        {
                            tmpNode = parent.Nodes.Add(mapList.Items[i].Name);
                            tmpNode.Tag = (MapListFolder) mapList.Items[i];
                            AddMapListToTree(
                                ((MapListFolder) mapList.Items[i]).Children, tmpNode, selectMapId, restrictMaps
                            );
                        }

                        if (mOpenFolders.Contains(((MapListFolder) mapList.Items[i]).FolderId))
                        {
                            tmpNode.Expand();
                        }

                        if (mSelectionType == 1 && mSelectedMap == ((MapListFolder) mapList.Items[i]).FolderId)
                        {
                            list.SelectedNode = tmpNode;
                            list.Focus();
                        }

                        tmpNode.ImageIndex = 0;
                        tmpNode.SelectedImageIndex = 0;
                    }
                    else
                    {
                        if (restrictMaps == null || restrictMaps.Contains(((MapListMap) mapList.Items[i]).MapId))
                        {
                            if (parent == null)
                            {
                                tmpNode = list.Nodes.Add(mapList.Items[i].Name);
                                tmpNode.Tag = (MapListMap) mapList.Items[i];
                            }
                            else
                            {
                                tmpNode = parent.Nodes.Add(mapList.Items[i].Name);
                                tmpNode.Tag = (MapListMap) mapList.Items[i];
                            }

                            if (selectMapId != Guid.Empty)
                            {
                                if (((MapListMap) mapList.Items[i]).MapId == selectMapId)
                                {
                                    list.SelectedNode = tmpNode;
                                    list.Focus();
                                }
                            }
                            else
                            {
                                if (mSelectionType == 0 && mSelectedMap == ((MapListMap) mapList.Items[i]).MapId)
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
            if (menuStrip != null)
            {
                list.ContextMenuStrip = menuStrip;
            }

            list.LabelEdit = true;
            mCanEdit = true;
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
                if (!mOpenFolders.Contains(((MapListFolder) e.Node.Tag).FolderId))
                {
                    mOpenFolders.Add(((MapListFolder) e.Node.Tag).FolderId);
                }
            }
        }

        private void list_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag.GetType() == typeof(MapListFolder))
            {
                if (mOpenFolders.Contains(((MapListFolder) e.Node.Tag).FolderId))
                {
                    mOpenFolders.Remove(((MapListFolder) e.Node.Tag).FolderId);
                }
            }
        }

        public static void Scroll(Control control)
        {
            var pt = control.PointToClient(Cursor.Position);

            if (pt.Y + 20 > control.Height)
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
