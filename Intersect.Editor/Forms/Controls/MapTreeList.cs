using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Intersect.Editor.Networking;
using Intersect.GameObjects.Maps.MapList;
using Intersect.Logging;

namespace Intersect.Editor.Forms.Controls
{

    public partial class MapTreeList : UserControl
    {
        private readonly Dictionary<MapListItem, TreeNode> _nodeLookup = new();

        //Cross Thread Delegates
        public delegate void TryUpdateMapList(Guid selectMap, List<Guid> restrictMaps = null);

        public bool Chronological = false;

        public TryUpdateMapList MapListDelegate;

        private bool mCanEdit;

        private List<Guid> mOpenFolders = new List<Guid>();

        private List<Guid>? mRestrictMapIds;

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
            var draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));
            int srcType;
            Guid srcId;
            switch (draggedNode.Tag)
            {
                case MapListMap draggedMap:
                    srcType = 1;
                    srcId = draggedMap.MapId;
                    break;

                case MapListFolder draggedFolder:
                    srcType = 0;
                    srcId = draggedFolder.FolderId;
                    break;

                default:
                    throw new InvalidOperationException($"Unknown type {draggedNode.Tag?.GetType()} is not supported.");
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
                switch (targetNode.Tag)
                {
                    case MapListMap targetMap:
                        PacketSender.SendMapListMove(srcType, srcId, 1, targetMap.MapId);

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
                        break;

                    case MapListFolder targetFolder:
                        PacketSender.SendMapListMove(srcType, srcId, 0, targetFolder.FolderId);

                        // Remove the node from its current
                        // location and add it to the node at the drop location.
                        draggedNode.Remove();
                        targetNode.Nodes.Add(draggedNode);

                        // Expand the node at the location
                        // to show the dropped node.
                        targetNode.Expand();
                        break;
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
                if (string.IsNullOrEmpty(e.Label))
                {
                    e.Node.Text = e.Node.Tag is MapListMap mapListMap ? mapListMap.Name : e.Label;
                }
                else
                {
                    switch (e.Node.Tag)
                    {
                        case MapListMap mapListMap:
                            mapListMap.Name = e.Label;

                            //Send Rename Map
                            PacketSender.SendRename(mapListMap, e.Label);
                            e.Node.Text = mapListMap.Name;
                            break;

                        case MapListFolder mapListFolder:
                            mapListFolder.Name = e.Label;

                            //Send Rename Folder
                            PacketSender.SendRename(mapListFolder, e.Label);
                            e.Node.Text = e.Label;
                            break;
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
                if (e.Node.Tag is MapListMap mapListMap)
                {
                    if (e.Node.Text == mapListMap.Name && Chronological)
                    {
                        e.Node.Text = mapListMap.Name;
                    }
                }
            }
        }

        private void treeMapList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            switch (e.Node.Tag)
            {
                case MapListMap mapListMap:
                    mSelectionType = 0;
                    mSelectedMap = mapListMap.MapId;
                    break;

                case MapListFolder mapListFolder:
                    mSelectionType = 1;
                    mSelectedMap = mapListFolder.FolderId;
                    break;

                default:
                    mSelectionType = -1;
                    mSelectedMap = Guid.Empty;
                    break;
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

        public void UpdateMapList(Guid selectMapId = default, List<Guid>? restrictMaps = null)
        {
            Log.Info("Updating list");
            var selectedMapListMap = selectMapId == default ? default : MapList.List.FindMap(selectMapId);
            if (selectedMapListMap != default && _nodeLookup.TryGetValue(selectedMapListMap, out var treeNode))
            {
                list.SelectedNode = treeNode;
            }
            else
            {
                list.Nodes.Clear();
                _nodeLookup.Clear();
                mRestrictMapIds = restrictMaps;
                AddMapListToTree(MapList.List, null, selectMapId, mRestrictMapIds);
            }
        }

        private void AddMapListToTree(
            MapList mapList,
            TreeNode? parent,
            Guid selectMapId = default,
            List<Guid>? restrictMaps = null
        )
        {
            TreeNode tmpNode;
            if (Chronological)
            {
                foreach (var map in MapList.OrderedMaps)
                {
                    if (restrictMaps != null && !restrictMaps.Contains(map.MapId))
                    {
                        continue;
                    }

                    tmpNode = list.Nodes.Add(map.Name);
                    _nodeLookup[map] = tmpNode;
                    tmpNode.Tag = map;
                    tmpNode.ImageIndex = 1;
                    tmpNode.SelectedImageIndex = 1;

                    var selectedId = selectMapId;
                    if (selectedId == default && mSelectionType == 0)
                    {
                        selectedId = mSelectedMap;
                    }

                    if (map.MapId == selectMapId)
                    {
                        list.SelectedNode = tmpNode;
                        list.Focus();
                    }
                }
            }
            else
            {
                foreach (var item in mapList.Items)
                {
                    switch (item)
                    {
                        case MapListFolder folder:
                            tmpNode = (parent?.Nodes ?? list.Nodes).Add(item.Name);
                            _nodeLookup[item] = tmpNode;
                            tmpNode.Tag = item;
                            AddMapListToTree(folder.Children, tmpNode, selectMapId, restrictMaps);

                            if (mOpenFolders.Contains(folder.FolderId))
                            {
                                tmpNode.Expand();
                            }

                            if (mSelectionType == 1 && mSelectedMap == folder.FolderId)
                            {
                                list.SelectedNode = tmpNode;
                                list.Focus();
                            }

                            tmpNode.ImageIndex = 0;
                            tmpNode.SelectedImageIndex = 0;
                            break;

                        case MapListMap map:
                            if (restrictMaps?.Contains(map.MapId) ?? true)
                            {
                                tmpNode = (parent?.Nodes ?? list.Nodes).Add(item.Name);
                                _nodeLookup[item] = tmpNode;
                                tmpNode.Tag = map;

                                var selectedId = selectMapId;
                                if (selectedId == default && mSelectionType == 0)
                                {
                                    selectedId = mSelectedMap;
                                }

                                if (map.MapId == selectMapId)
                                {
                                    list.SelectedNode = tmpNode;
                                    list.Focus();
                                }

                                tmpNode.ImageIndex = 1;
                                tmpNode.SelectedImageIndex = 1;
                                break;
                            }

                            break;
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
            if (e.Node.Tag is MapListFolder folder)
            {
                if (!mOpenFolders.Contains(folder.FolderId))
                {
                    mOpenFolders.Add(folder.FolderId);
                }
            }
        }

        private void list_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag is MapListFolder folder)
            {
                mOpenFolders.Remove(folder.FolderId);
            }
        }

        public static void Scroll(Control control)
        {
            var pt = control.PointToClient(Cursor.Position);

            if (pt.Y + 20 > control.Height)
            {
                // scroll down
                SendMessage(control.Handle, 277, (IntPtr)1, (IntPtr)0);
            }
            else if (pt.Y < 20)
            {
                // scroll up
                SendMessage(control.Handle, 277, (IntPtr)0, (IntPtr)0);
            }
        }

        private void list_DragOver(object sender, DragEventArgs e)
        {
            Scroll(list);
        }

    }

}
