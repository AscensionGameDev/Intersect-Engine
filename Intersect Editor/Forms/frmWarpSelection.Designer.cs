namespace Intersect_Editor.Forms
{
    partial class frmWarpSelection
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.pnlMapContainer = new System.Windows.Forms.Panel();
            this.pnlMap = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkChronological = new System.Windows.Forms.CheckBox();
            this.mapTreeList1 = new Intersect_Editor.Forms.Controls.MapTreeList();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.tmrMapCheck = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.pnlMapContainer.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnOk);
            this.groupBox1.Location = new System.Drawing.Point(2, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(783, 604);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Warp Selection";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.pnlMapContainer);
            this.groupBox3.Location = new System.Drawing.Point(257, 22);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(520, 531);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Map Preview";
            // 
            // pnlMapContainer
            // 
            this.pnlMapContainer.AutoScroll = true;
            this.pnlMapContainer.Controls.Add(this.pnlMap);
            this.pnlMapContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMapContainer.Location = new System.Drawing.Point(3, 16);
            this.pnlMapContainer.Name = "pnlMapContainer";
            this.pnlMapContainer.Size = new System.Drawing.Size(514, 512);
            this.pnlMapContainer.TabIndex = 0;
            // 
            // pnlMap
            // 
            this.pnlMap.Location = new System.Drawing.Point(0, 0);
            this.pnlMap.Name = "pnlMap";
            this.pnlMap.Size = new System.Drawing.Size(200, 100);
            this.pnlMap.TabIndex = 0;
            this.pnlMap.DoubleClick += new System.EventHandler(this.pnlMap_DoubleClick);
            this.pnlMap.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlMap_MouseDown);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.Controls.Add(this.chkChronological);
            this.groupBox2.Controls.Add(this.mapTreeList1);
            this.groupBox2.Location = new System.Drawing.Point(6, 22);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(230, 531);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Map List";
            // 
            // chkChronological
            // 
            this.chkChronological.AutoSize = true;
            this.chkChronological.Location = new System.Drawing.Point(136, 13);
            this.chkChronological.Name = "chkChronological";
            this.chkChronological.Size = new System.Drawing.Size(90, 17);
            this.chkChronological.TabIndex = 1;
            this.chkChronological.Text = "Chronological";
            this.chkChronological.UseVisualStyleBackColor = true;
            this.chkChronological.CheckedChanged += new System.EventHandler(this.chkChronological_CheckedChanged);
            // 
            // mapTreeList1
            // 
            this.mapTreeList1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.mapTreeList1.Location = new System.Drawing.Point(7, 36);
            this.mapTreeList1.Name = "mapTreeList1";
            this.mapTreeList1.Size = new System.Drawing.Size(217, 489);
            this.mapTreeList1.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(695, 575);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(82, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(609, 575);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(82, 23);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // tmrMapCheck
            // 
            this.tmrMapCheck.Tick += new System.EventHandler(this.tmrMapCheck_Tick);
            // 
            // frmWarpSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(787, 609);
            this.Controls.Add(this.groupBox1);
            this.DoubleBuffered = true;
            this.Name = "frmWarpSelection";
            this.Text = "Warp Tile Selection";
            this.Load += new System.EventHandler(this.frmWarpSelection_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.pnlMapContainer.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Panel pnlMapContainer;
        private System.Windows.Forms.Panel pnlMap;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chkChronological;
        private Controls.MapTreeList mapTreeList1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Timer tmrMapCheck;
    }
}