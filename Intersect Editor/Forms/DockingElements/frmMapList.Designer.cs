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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("1. New Map");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Root", new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.treeMapList = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // treeMapList
            // 
            this.treeMapList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeMapList.Location = new System.Drawing.Point(0, 0);
            this.treeMapList.Name = "treeMapList";
            treeNode1.Name = "Node1";
            treeNode1.Text = "1. New Map";
            treeNode2.Name = "Node0";
            treeNode2.Text = "Root";
            this.treeMapList.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode2});
            this.treeMapList.Size = new System.Drawing.Size(188, 206);
            this.treeMapList.TabIndex = 0;
            // 
            // frmMapList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(189, 207);
            this.Controls.Add(this.treeMapList);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frmMapList";
            this.Text = "Map List";
            this.Load += new System.EventHandler(this.frmMapList_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeMapList;

    }
}