using DarkUI.Controls;
using DarkUI.Renderers;

namespace Intersect.Editor.Forms.DockingElements
{
    partial class FrmMapGrid
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMapGrid));
            this.pnlMapGrid = new System.Windows.Forms.Panel();
            this.toolStrip1 = new DarkUI.Controls.DarkToolStrip();
            this.btnFetchPreview = new System.Windows.Forms.ToolStripDropDownButton();
            this.downloadMissingPreviewsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reDownloadAllPreviewsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnGridView = new System.Windows.Forms.ToolStripButton();
            this.btnScreenshotWorld = new System.Windows.Forms.ToolStripButton();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.unlinkMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.linkMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recacheMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMapGrid
            // 
            this.pnlMapGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlMapGrid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.pnlMapGrid.Location = new System.Drawing.Point(0, 25);
            this.pnlMapGrid.Name = "pnlMapGrid";
            this.pnlMapGrid.Size = new System.Drawing.Size(362, 146);
            this.pnlMapGrid.TabIndex = 0;
            this.pnlMapGrid.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pnlMapGrid_MouseDoubleClick);
            this.pnlMapGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlMapGrid_MouseDown);
            this.pnlMapGrid.MouseLeave += new System.EventHandler(this.pnlMapGrid_MouseLeave);
            this.pnlMapGrid.MouseHover += new System.EventHandler(this.pnlMapGrid_MouseHover);
            this.pnlMapGrid.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlMapGrid_MouseMove);
            this.pnlMapGrid.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlMapGrid_MouseUp);
            this.pnlMapGrid.Resize += new System.EventHandler(this.pnlMapGrid_Resize);
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.toolStrip1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnFetchPreview,
            this.btnGridView,
            this.btnScreenshotWorld});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(5, 0, 1, 0);
            this.toolStrip1.Size = new System.Drawing.Size(362, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnFetchPreview
            // 
            this.btnFetchPreview.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnFetchPreview.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnFetchPreview.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.downloadMissingPreviewsToolStripMenuItem,
            this.reDownloadAllPreviewsToolStripMenuItem});
            this.btnFetchPreview.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.btnFetchPreview.Image = ((System.Drawing.Image)(resources.GetObject("btnFetchPreview.Image")));
            this.btnFetchPreview.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFetchPreview.Name = "btnFetchPreview";
            this.btnFetchPreview.Size = new System.Drawing.Size(29, 22);
            this.btnFetchPreview.Text = "Fetch Preview";
            // 
            // downloadMissingPreviewsToolStripMenuItem
            // 
            this.downloadMissingPreviewsToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.downloadMissingPreviewsToolStripMenuItem.Name = "downloadMissingPreviewsToolStripMenuItem";
            this.downloadMissingPreviewsToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.downloadMissingPreviewsToolStripMenuItem.Text = "Download Missing Previews";
            this.downloadMissingPreviewsToolStripMenuItem.Click += new System.EventHandler(this.downloadMissingPreviewsToolStripMenuItem_Click);
            // 
            // reDownloadAllPreviewsToolStripMenuItem
            // 
            this.reDownloadAllPreviewsToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.reDownloadAllPreviewsToolStripMenuItem.Name = "reDownloadAllPreviewsToolStripMenuItem";
            this.reDownloadAllPreviewsToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.reDownloadAllPreviewsToolStripMenuItem.Text = "Re-Download All Previews";
            this.reDownloadAllPreviewsToolStripMenuItem.Click += new System.EventHandler(this.reDownloadAllPreviewsToolStripMenuItem_Click);
            // 
            // btnGridView
            // 
            this.btnGridView.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnGridView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnGridView.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.btnGridView.Image = ((System.Drawing.Image)(resources.GetObject("btnGridView.Image")));
            this.btnGridView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnGridView.Name = "btnGridView";
            this.btnGridView.Size = new System.Drawing.Size(23, 22);
            this.btnGridView.Text = "Show/Hide Grid Lines";
            this.btnGridView.Click += new System.EventHandler(this.btnGridView_Click);
            // 
            // btnScreenshotWorld
            // 
            this.btnScreenshotWorld.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnScreenshotWorld.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.btnScreenshotWorld.Image = ((System.Drawing.Image)(resources.GetObject("btnScreenshotWorld.Image")));
            this.btnScreenshotWorld.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnScreenshotWorld.Name = "btnScreenshotWorld";
            this.btnScreenshotWorld.Size = new System.Drawing.Size(23, 22);
            this.btnScreenshotWorld.Text = "Take a world screenshot";
            this.btnScreenshotWorld.Click += new System.EventHandler(this.btnScreenshotWorld_Click);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.unlinkMapToolStripMenuItem,
            this.linkMapToolStripMenuItem,
            this.recacheMapToolStripMenuItem});
            this.contextMenuStrip.Name = "mapMenuStrip";
            this.contextMenuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.contextMenuStrip.Size = new System.Drawing.Size(146, 70);
            // 
            // unlinkMapToolStripMenuItem
            // 
            this.unlinkMapToolStripMenuItem.ForeColor = System.Drawing.Color.Gainsboro;
            this.unlinkMapToolStripMenuItem.Name = "unlinkMapToolStripMenuItem";
            this.unlinkMapToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.unlinkMapToolStripMenuItem.Text = "Unlink Map";
            // 
            // linkMapToolStripMenuItem
            // 
            this.linkMapToolStripMenuItem.ForeColor = System.Drawing.Color.Gainsboro;
            this.linkMapToolStripMenuItem.Name = "linkMapToolStripMenuItem";
            this.linkMapToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.linkMapToolStripMenuItem.Text = "Link Map";
            // 
            // recacheMapToolStripMenuItem
            // 
            this.recacheMapToolStripMenuItem.ForeColor = System.Drawing.Color.Gainsboro;
            this.recacheMapToolStripMenuItem.Name = "recacheMapToolStripMenuItem";
            this.recacheMapToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.recacheMapToolStripMenuItem.Text = "Recache Map";
            // 
            // FrmMapGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(362, 171);
            this.CloseButton = false;
            this.CloseButtonVisible = false;
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.pnlMapGrid);
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.KeyPreview = true;
            this.Name = "FrmMapGrid";
            this.Text = "Map Grid";
            this.DockStateChanged += new System.EventHandler(this.frmMapGrid_DockStateChanged);
            this.Load += new System.EventHandler(this.frmMapGrid_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmMapGrid_KeyDown);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlMapGrid;
        private DarkToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton btnFetchPreview;
        private System.Windows.Forms.ToolStripMenuItem downloadMissingPreviewsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reDownloadAllPreviewsToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton btnGridView;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem unlinkMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem linkMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton btnScreenshotWorld;
        private System.Windows.Forms.ToolStripMenuItem recacheMapToolStripMenuItem;
    }
}