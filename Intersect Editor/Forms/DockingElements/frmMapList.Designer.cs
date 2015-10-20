namespace Intersect_Editor.Forms
{
    partial class frmMapList
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMapList));
            this.treeMapList = new System.Windows.Forms.TreeView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.folderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnChronological = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnNewMap = new System.Windows.Forms.ToolStripButton();
            this.btnNewFolder = new System.Windows.Forms.ToolStripButton();
            this.btnRename = new System.Windows.Forms.ToolStripButton();
            this.btnDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnRefreshList = new System.Windows.Forms.ToolStripButton();
            this.btnGridView = new System.Windows.Forms.ToolStripButton();
            this.contextMenuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeMapList
            // 
            this.treeMapList.AllowDrop = true;
            this.treeMapList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeMapList.ContextMenuStrip = this.contextMenuStrip1;
            this.treeMapList.ImageIndex = 1;
            this.treeMapList.ImageList = this.imageList;
            this.treeMapList.LabelEdit = true;
            this.treeMapList.Location = new System.Drawing.Point(0, 25);
            this.treeMapList.Name = "treeMapList";
            this.treeMapList.SelectedImageIndex = 0;
            this.treeMapList.Size = new System.Drawing.Size(188, 181);
            this.treeMapList.TabIndex = 0;
            this.treeMapList.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeMapList_BeforeLabelEdit);
            this.treeMapList.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeMapList_AfterLabelEdit);
            this.treeMapList.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeMapList_ItemDrag);
            this.treeMapList.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeMapList_AfterSelect);
            this.treeMapList.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeMapList_NodeMouseDoubleClick);
            this.treeMapList.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeMapList_DragDrop);
            this.treeMapList.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeMapList_DragEnter);
            this.treeMapList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeMapList_MouseDown);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newMapToolStripMenuItem,
            this.newFolderToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(118, 70);
            // 
            // newMapToolStripMenuItem
            // 
            this.newMapToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mapToolStripMenuItem,
            this.folderToolStripMenuItem});
            this.newMapToolStripMenuItem.Name = "newMapToolStripMenuItem";
            this.newMapToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.newMapToolStripMenuItem.Text = "New...";
            // 
            // mapToolStripMenuItem
            // 
            this.mapToolStripMenuItem.Name = "mapToolStripMenuItem";
            this.mapToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.mapToolStripMenuItem.Text = "Map";
            this.mapToolStripMenuItem.Click += new System.EventHandler(this.btnNewMap_Click);
            // 
            // folderToolStripMenuItem
            // 
            this.folderToolStripMenuItem.Name = "folderToolStripMenuItem";
            this.folderToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.folderToolStripMenuItem.Text = "Folder";
            this.folderToolStripMenuItem.Click += new System.EventHandler(this.btnNewFolder_Click);
            // 
            // newFolderToolStripMenuItem
            // 
            this.newFolderToolStripMenuItem.Name = "newFolderToolStripMenuItem";
            this.newFolderToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.newFolderToolStripMenuItem.Text = "Rename";
            this.newFolderToolStripMenuItem.Click += new System.EventHandler(this.btnRename_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "folder_Open_16xLG.png");
            this.imageList.Images.SetKeyName(1, "resource_16xLG.png");
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnChronological,
            this.btnGridView,
            this.toolStripSeparator1,
            this.btnNewMap,
            this.btnNewFolder,
            this.btnRename,
            this.btnDelete,
            this.toolStripSeparator2,
            this.btnRefreshList});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0);
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(189, 25);
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnChronological
            // 
            this.btnChronological.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnChronological.Image = ((System.Drawing.Image)(resources.GetObject("btnChronological.Image")));
            this.btnChronological.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnChronological.Name = "btnChronological";
            this.btnChronological.Size = new System.Drawing.Size(23, 22);
            this.btnChronological.Text = "Order Chronologically";
            this.btnChronological.Click += new System.EventHandler(this.btnChronological_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnNewMap
            // 
            this.btnNewMap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnNewMap.Image = ((System.Drawing.Image)(resources.GetObject("btnNewMap.Image")));
            this.btnNewMap.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNewMap.Name = "btnNewMap";
            this.btnNewMap.Size = new System.Drawing.Size(23, 22);
            this.btnNewMap.Text = "New Map";
            this.btnNewMap.Click += new System.EventHandler(this.btnNewMap_Click);
            // 
            // btnNewFolder
            // 
            this.btnNewFolder.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnNewFolder.Image = ((System.Drawing.Image)(resources.GetObject("btnNewFolder.Image")));
            this.btnNewFolder.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNewFolder.Name = "btnNewFolder";
            this.btnNewFolder.Size = new System.Drawing.Size(23, 22);
            this.btnNewFolder.Text = "New Folder";
            this.btnNewFolder.Click += new System.EventHandler(this.btnNewFolder_Click);
            // 
            // btnRename
            // 
            this.btnRename.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRename.Image = ((System.Drawing.Image)(resources.GetObject("btnRename.Image")));
            this.btnRename.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRename.Name = "btnRename";
            this.btnRename.Size = new System.Drawing.Size(23, 22);
            this.btnRename.Text = "Rename";
            this.btnRename.Click += new System.EventHandler(this.btnRename_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnDelete.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.Image")));
            this.btnDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(23, 22);
            this.btnDelete.Text = "Delete Map or Folder";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnRefreshList
            // 
            this.btnRefreshList.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRefreshList.Image = ((System.Drawing.Image)(resources.GetObject("btnRefreshList.Image")));
            this.btnRefreshList.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRefreshList.Name = "btnRefreshList";
            this.btnRefreshList.Size = new System.Drawing.Size(23, 20);
            this.btnRefreshList.Text = "Refresh";
            this.btnRefreshList.Click += new System.EventHandler(this.btnRefreshList_Click);
            // 
            // btnGridView
            // 
            this.btnGridView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnGridView.Image = ((System.Drawing.Image)(resources.GetObject("btnGridView.Image")));
            this.btnGridView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnGridView.Name = "btnGridView";
            this.btnGridView.Size = new System.Drawing.Size(23, 22);
            this.btnGridView.Text = "Show Grid View";
            this.btnGridView.Click += new System.EventHandler(this.btnGridView_Click);
            // 
            // frmMapList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(189, 207);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.treeMapList);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frmMapList";
            this.Text = "Map List";
            this.Load += new System.EventHandler(this.frmMapList_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeMapList;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnNewFolder;
        private System.Windows.Forms.ToolStripButton btnRefreshList;
        private System.Windows.Forms.ToolStripButton btnRename;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btnChronological;
        private System.Windows.Forms.ToolStripButton btnDelete;
        private System.Windows.Forms.ToolStripButton btnNewMap;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem newMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem folderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton btnGridView;

    }
}