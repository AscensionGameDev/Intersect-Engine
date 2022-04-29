namespace Intersect.Editor.Forms.Controls
{
    partial class MapTreeList
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapTreeList));
            this.list = new System.Windows.Forms.TreeView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // list
            // 
            this.list.AllowDrop = true;
            this.list.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.list.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.list.Dock = System.Windows.Forms.DockStyle.Fill;
            this.list.ForeColor = System.Drawing.Color.Gainsboro;
            this.list.ImageIndex = 0;
            this.list.ImageList = this.imageList;
            this.list.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.list.Location = new System.Drawing.Point(0, 0);
            this.list.Name = "list";
            this.list.SelectedImageIndex = 0;
            this.list.Size = new System.Drawing.Size(150, 150);
            this.list.TabIndex = 1;
            this.list.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeMapList_BeforeLabelEdit);
            this.list.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeMapList_AfterLabelEdit);
            this.list.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.list_AfterCollapse);
            this.list.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.list_AfterExpand);
            this.list.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeMapList_ItemDrag);
            this.list.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeMapList_AfterSelect);
            this.list.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeMapList_NodeMouseDoubleClick);
            this.list.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeMapList_DragDrop);
            this.list.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeMapList_DragEnter);
            this.list.DragOver += new System.Windows.Forms.DragEventHandler(this.list_DragOver);
            this.list.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeMapList_MouseDown);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "folder_Open_16xLG.png");
            this.imageList.Images.SetKeyName(1, "resource_16xLG.png");
            // 
            // MapTreeList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.list);
            this.Name = "MapTreeList";
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.TreeView list;
        private System.Windows.Forms.ImageList imageList;
    }
}
