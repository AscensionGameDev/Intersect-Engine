namespace Intersect_Editor.Forms
{
    partial class frmGridView
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmGridView));
            this.gridContainer = new System.Windows.Forms.Panel();
            this.mapGridView = new System.Windows.Forms.DataGridView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnToggleNames = new System.Windows.Forms.ToolStripButton();
            this.btnTogglePreviews = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.cmbZoom = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnFetchPreview = new System.Windows.Forms.ToolStripDropDownButton();
            this.downloadMissingPreviewsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reDownloadAllPreviewsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnScreenshotWorld = new System.Windows.Forms.ToolStripButton();
            this.mapMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.unlinkMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.linkMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gridContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mapGridView)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.mapMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridContainer
            // 
            this.gridContainer.AutoScroll = true;
            this.gridContainer.Controls.Add(this.mapGridView);
            this.gridContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridContainer.Location = new System.Drawing.Point(0, 0);
            this.gridContainer.Name = "gridContainer";
            this.gridContainer.Size = new System.Drawing.Size(784, 601);
            this.gridContainer.TabIndex = 2;
            // 
            // mapGridView
            // 
            this.mapGridView.AllowUserToAddRows = false;
            this.mapGridView.AllowUserToDeleteRows = false;
            this.mapGridView.AllowUserToResizeColumns = false;
            this.mapGridView.AllowUserToResizeRows = false;
            this.mapGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.mapGridView.ColumnHeadersVisible = false;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.mapGridView.DefaultCellStyle = dataGridViewCellStyle2;
            this.mapGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.mapGridView.Location = new System.Drawing.Point(0, 28);
            this.mapGridView.MultiSelect = false;
            this.mapGridView.Name = "mapGridView";
            this.mapGridView.ReadOnly = true;
            this.mapGridView.RowHeadersVisible = false;
            this.mapGridView.RowTemplate.ReadOnly = true;
            this.mapGridView.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.mapGridView.Size = new System.Drawing.Size(44, 44);
            this.mapGridView.TabIndex = 1;
            this.mapGridView.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.mapGridView_CellContentDoubleClick);
            this.mapGridView.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.mapGridView_CellEnter);
            this.mapGridView.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.mapGridView_CellPainting);
            this.mapGridView.SelectionChanged += new System.EventHandler(this.mapGridView_SelectionChanged);
            this.mapGridView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.mapGridView_MouseClick);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnToggleNames,
            this.btnTogglePreviews,
            this.toolStripSeparator2,
            this.cmbZoom,
            this.toolStripSeparator1,
            this.btnFetchPreview,
            this.btnScreenshotWorld});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(784, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnToggleNames
            // 
            this.btnToggleNames.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnToggleNames.Image = ((System.Drawing.Image)(resources.GetObject("btnToggleNames.Image")));
            this.btnToggleNames.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnToggleNames.Name = "btnToggleNames";
            this.btnToggleNames.Size = new System.Drawing.Size(23, 22);
            this.btnToggleNames.Text = "Show/Hide Map Names";
            this.btnToggleNames.Click += new System.EventHandler(this.btnToggleNames_Click);
            // 
            // btnTogglePreviews
            // 
            this.btnTogglePreviews.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnTogglePreviews.Image = ((System.Drawing.Image)(resources.GetObject("btnTogglePreviews.Image")));
            this.btnTogglePreviews.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnTogglePreviews.Name = "btnTogglePreviews";
            this.btnTogglePreviews.Size = new System.Drawing.Size(23, 22);
            this.btnTogglePreviews.Text = "Show/Hide Previews";
            this.btnTogglePreviews.Click += new System.EventHandler(this.btnTogglePreviews_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // cmbZoom
            // 
            this.cmbZoom.AutoSize = false;
            this.cmbZoom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbZoom.Items.AddRange(new object[] {
            "Zoom: Fit",
            "Zoom: 12.5%",
            "Zoom 25%",
            "Zoom : 50%",
            "Zoom : 75%",
            "Zoom: 100%"});
            this.cmbZoom.Name = "cmbZoom";
            this.cmbZoom.Size = new System.Drawing.Size(121, 23);
            this.cmbZoom.SelectedIndexChanged += new System.EventHandler(this.cmbZoom_SelectedIndexChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnFetchPreview
            // 
            this.btnFetchPreview.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnFetchPreview.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.downloadMissingPreviewsToolStripMenuItem,
            this.reDownloadAllPreviewsToolStripMenuItem});
            this.btnFetchPreview.Image = ((System.Drawing.Image)(resources.GetObject("btnFetchPreview.Image")));
            this.btnFetchPreview.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFetchPreview.Name = "btnFetchPreview";
            this.btnFetchPreview.Size = new System.Drawing.Size(29, 22);
            this.btnFetchPreview.Text = "Fetch Preview";
            // 
            // downloadMissingPreviewsToolStripMenuItem
            // 
            this.downloadMissingPreviewsToolStripMenuItem.Name = "downloadMissingPreviewsToolStripMenuItem";
            this.downloadMissingPreviewsToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.downloadMissingPreviewsToolStripMenuItem.Text = "Download Missing Previews";
            this.downloadMissingPreviewsToolStripMenuItem.Click += new System.EventHandler(this.downloadMissingPreviewsToolStripMenuItem_Click);
            // 
            // reDownloadAllPreviewsToolStripMenuItem
            // 
            this.reDownloadAllPreviewsToolStripMenuItem.Name = "reDownloadAllPreviewsToolStripMenuItem";
            this.reDownloadAllPreviewsToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.reDownloadAllPreviewsToolStripMenuItem.Text = "Re-Download All Previews";
            this.reDownloadAllPreviewsToolStripMenuItem.Click += new System.EventHandler(this.reDownloadAllPreviewsToolStripMenuItem_Click);
            // 
            // btnScreenshotWorld
            // 
            this.btnScreenshotWorld.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnScreenshotWorld.Image = ((System.Drawing.Image)(resources.GetObject("btnScreenshotWorld.Image")));
            this.btnScreenshotWorld.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnScreenshotWorld.Name = "btnScreenshotWorld";
            this.btnScreenshotWorld.Size = new System.Drawing.Size(23, 22);
            this.btnScreenshotWorld.Text = "Take a world screenshot";
            this.btnScreenshotWorld.Click += new System.EventHandler(this.btnScreenshotWorld_Click);
            // 
            // mapMenuStrip
            // 
            this.mapMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.unlinkMapToolStripMenuItem,
            this.linkMapToolStripMenuItem});
            this.mapMenuStrip.Name = "mapMenuStrip";
            this.mapMenuStrip.Size = new System.Drawing.Size(136, 48);
            // 
            // unlinkMapToolStripMenuItem
            // 
            this.unlinkMapToolStripMenuItem.Name = "unlinkMapToolStripMenuItem";
            this.unlinkMapToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.unlinkMapToolStripMenuItem.Text = "Unlink Map";
            this.unlinkMapToolStripMenuItem.Click += new System.EventHandler(this.unlinkMapToolStripMenuItem_Click);
            // 
            // linkMapToolStripMenuItem
            // 
            this.linkMapToolStripMenuItem.Name = "linkMapToolStripMenuItem";
            this.linkMapToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.linkMapToolStripMenuItem.Text = "Link Map";
            // 
            // frmGridView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(784, 601);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.gridContainer);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmGridView";
            this.Text = "Grid Viewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmGridView_FormClosing);
            this.SizeChanged += new System.EventHandler(this.frmGridView_ResizeEnd);
            this.gridContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mapGridView)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.mapMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel gridContainer;
        private System.Windows.Forms.DataGridView mapGridView;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnToggleNames;
        private System.Windows.Forms.ToolStripButton btnTogglePreviews;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripComboBox cmbZoom;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ContextMenuStrip mapMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem unlinkMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem linkMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton btnScreenshotWorld;
        private System.Windows.Forms.ToolStripMenuItem downloadMissingPreviewsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reDownloadAllPreviewsToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton btnFetchPreview;
    }
}