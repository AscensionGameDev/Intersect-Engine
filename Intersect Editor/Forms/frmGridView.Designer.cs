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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmGridView));
            this.gridContainer = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnToggleNames = new System.Windows.Forms.ToolStripButton();
            this.btnTogglePreviews = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.cmbZoom = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnFetchPreview = new System.Windows.Forms.ToolStripButton();
            this.gridContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridContainer
            // 
            this.gridContainer.AutoScroll = true;
            this.gridContainer.Controls.Add(this.dataGridView1);
            this.gridContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridContainer.Location = new System.Drawing.Point(0, 0);
            this.gridContainer.Name = "gridContainer";
            this.gridContainer.Size = new System.Drawing.Size(784, 601);
            this.gridContainer.TabIndex = 2;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView1.ColumnHeadersVisible = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView1.Location = new System.Drawing.Point(0, 28);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.ReadOnly = true;
            this.dataGridView1.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dataGridView1.Size = new System.Drawing.Size(44, 44);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentDoubleClick);
            this.dataGridView1.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dataGridView1_CellPainting);
            this.dataGridView1.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnToggleNames,
            this.btnTogglePreviews,
            this.toolStripSeparator2,
            this.cmbZoom,
            this.toolStripSeparator1,
            this.btnFetchPreview});
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
            this.btnFetchPreview.Image = ((System.Drawing.Image)(resources.GetObject("btnFetchPreview.Image")));
            this.btnFetchPreview.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFetchPreview.Name = "btnFetchPreview";
            this.btnFetchPreview.Size = new System.Drawing.Size(23, 22);
            this.btnFetchPreview.Text = "Fetch Preview";
            this.btnFetchPreview.Click += new System.EventHandler(this.btnFetchPreview_Click);
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
            this.gridContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel gridContainer;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnToggleNames;
        private System.Windows.Forms.ToolStripButton btnFetchPreview;
        private System.Windows.Forms.ToolStripButton btnTogglePreviews;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripComboBox cmbZoom;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}