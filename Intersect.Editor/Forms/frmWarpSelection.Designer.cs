using DarkUI.Controls;
using Intersect.Editor.Forms.Controls;

namespace Intersect.Editor.Forms
{
    partial class FrmWarpSelection
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmWarpSelection));
            this.grpEverything = new DarkUI.Controls.DarkGroupBox();
            this.btnRefreshPreview = new System.Windows.Forms.Button();
            this.grpMapPreview = new DarkUI.Controls.DarkGroupBox();
            this.pnlMapContainer = new System.Windows.Forms.Panel();
            this.pnlMap = new System.Windows.Forms.Panel();
            this.grpMapList = new DarkUI.Controls.DarkGroupBox();
            this.chkChronological = new DarkUI.Controls.DarkCheckBox();
            this.mapTreeList1 = new Intersect.Editor.Forms.Controls.MapTreeList();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnOk = new DarkUI.Controls.DarkButton();
            this.tmrMapCheck = new System.Windows.Forms.Timer(this.components);
            this.grpEverything.SuspendLayout();
            this.grpMapPreview.SuspendLayout();
            this.pnlMapContainer.SuspendLayout();
            this.grpMapList.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpEverything
            // 
            this.grpEverything.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpEverything.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpEverything.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpEverything.Controls.Add(this.btnRefreshPreview);
            this.grpEverything.Controls.Add(this.grpMapPreview);
            this.grpEverything.Controls.Add(this.grpMapList);
            this.grpEverything.Controls.Add(this.btnCancel);
            this.grpEverything.Controls.Add(this.btnOk);
            this.grpEverything.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpEverything.Location = new System.Drawing.Point(2, 2);
            this.grpEverything.Name = "grpEverything";
            this.grpEverything.Size = new System.Drawing.Size(783, 604);
            this.grpEverything.TabIndex = 1;
            this.grpEverything.TabStop = false;
            // 
            // btnRefreshPreview
            // 
            this.btnRefreshPreview.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnRefreshPreview.BackgroundImage")));
            this.btnRefreshPreview.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnRefreshPreview.Enabled = false;
            this.btnRefreshPreview.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.btnRefreshPreview.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefreshPreview.ForeColor = System.Drawing.Color.Transparent;
            this.btnRefreshPreview.Location = new System.Drawing.Point(753, 2);
            this.btnRefreshPreview.Name = "btnRefreshPreview";
            this.btnRefreshPreview.Size = new System.Drawing.Size(24, 24);
            this.btnRefreshPreview.TabIndex = 8;
            this.btnRefreshPreview.UseVisualStyleBackColor = true;
            this.btnRefreshPreview.Click += new System.EventHandler(this.btnRefreshPreview_Click);
            // 
            // grpMapPreview
            // 
            this.grpMapPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpMapPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpMapPreview.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpMapPreview.Controls.Add(this.pnlMapContainer);
            this.grpMapPreview.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpMapPreview.Location = new System.Drawing.Point(257, 22);
            this.grpMapPreview.Name = "grpMapPreview";
            this.grpMapPreview.Size = new System.Drawing.Size(520, 531);
            this.grpMapPreview.TabIndex = 7;
            this.grpMapPreview.TabStop = false;
            this.grpMapPreview.Text = "Map Preview";
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
            // grpMapList
            // 
            this.grpMapList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.grpMapList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpMapList.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpMapList.Controls.Add(this.chkChronological);
            this.grpMapList.Controls.Add(this.mapTreeList1);
            this.grpMapList.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpMapList.Location = new System.Drawing.Point(6, 22);
            this.grpMapList.Name = "grpMapList";
            this.grpMapList.Size = new System.Drawing.Size(230, 531);
            this.grpMapList.TabIndex = 6;
            this.grpMapList.TabStop = false;
            this.grpMapList.Text = "Map List";
            // 
            // chkChronological
            // 
            this.chkChronological.AutoSize = true;
            this.chkChronological.Location = new System.Drawing.Point(136, 13);
            this.chkChronological.Name = "chkChronological";
            this.chkChronological.Size = new System.Drawing.Size(90, 17);
            this.chkChronological.TabIndex = 1;
            this.chkChronological.Text = "Chronological";
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
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(82, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(609, 575);
            this.btnOk.Name = "btnOk";
            this.btnOk.Padding = new System.Windows.Forms.Padding(5);
            this.btnOk.Size = new System.Drawing.Size(82, 23);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "Ok";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // tmrMapCheck
            // 
            this.tmrMapCheck.Tick += new System.EventHandler(this.tmrMapCheck_Tick);
            // 
            // FrmWarpSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.ClientSize = new System.Drawing.Size(787, 609);
            this.Controls.Add(this.grpEverything);
            this.DoubleBuffered = true;
            this.Name = "FrmWarpSelection";
            this.Text = "Warp Tile Selection";
            this.Load += new System.EventHandler(this.frmWarpSelection_Load);
            this.grpEverything.ResumeLayout(false);
            this.grpMapPreview.ResumeLayout(false);
            this.pnlMapContainer.ResumeLayout(false);
            this.grpMapList.ResumeLayout(false);
            this.grpMapList.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpEverything;
        private DarkGroupBox grpMapPreview;
        private System.Windows.Forms.Panel pnlMapContainer;
        private System.Windows.Forms.Panel pnlMap;
        private DarkGroupBox grpMapList;
        private DarkCheckBox chkChronological;
        private Controls.MapTreeList mapTreeList1;
        private DarkButton btnCancel;
        private DarkButton btnOk;
        private System.Windows.Forms.Timer tmrMapCheck;
        private System.Windows.Forms.Button btnRefreshPreview;
    }
}