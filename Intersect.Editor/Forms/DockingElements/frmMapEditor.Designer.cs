namespace Intersect.Editor.Forms.DockingElements
{
    partial class FrmMapEditor
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
            this.pnlMapContainer = new System.Windows.Forms.Panel();
            this.picMap = new System.Windows.Forms.PictureBox();
            this.pnlMapContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picMap)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlMapContainer
            // 
            this.pnlMapContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlMapContainer.AutoScroll = true;
            this.pnlMapContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.pnlMapContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlMapContainer.Controls.Add(this.picMap);
            this.pnlMapContainer.Location = new System.Drawing.Point(0, 0);
            this.pnlMapContainer.Name = "pnlMapContainer";
            this.pnlMapContainer.Size = new System.Drawing.Size(204, 101);
            this.pnlMapContainer.TabIndex = 1;
            this.pnlMapContainer.Scroll += new System.Windows.Forms.ScrollEventHandler(this.pnlMapContainer_Scroll);
            this.pnlMapContainer.Resize += new System.EventHandler(this.pnlMapContainer_Resize);
            // 
            // picMap
            // 
            this.picMap.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.picMap.Location = new System.Drawing.Point(0, 0);
            this.picMap.Name = "picMap";
            this.picMap.Size = new System.Drawing.Size(107, 62);
            this.picMap.TabIndex = 2;
            this.picMap.TabStop = false;
            this.picMap.Visible = false;
            this.picMap.Paint += new System.Windows.Forms.PaintEventHandler(this.picMap_Paint);
            this.picMap.DoubleClick += new System.EventHandler(this.picMap_DoubleClick);
            this.picMap.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picMap_MouseDown);
            this.picMap.MouseEnter += new System.EventHandler(this.picMap_MouseEnter);
            this.picMap.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picMap_MouseMove);
            this.picMap.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picMap_MouseUp);
            this.picMap.Resize += new System.EventHandler(this.picMap_Resize);
            // 
            // FrmMapEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(204, 101);
            this.CloseButton = false;
            this.CloseButtonVisible = false;
            this.Controls.Add(this.pnlMapContainer);
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.KeyPreview = true;
            this.Name = "FrmMapEditor";
            this.Text = "Map Editor";
            this.DockStateChanged += new System.EventHandler(this.frmMapEditor_DockStateChanged);
            this.Load += new System.EventHandler(this.frmMapEditor_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmMapEditor_KeyDown);
            this.pnlMapContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picMap)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.PictureBox picMap;
        public System.Windows.Forms.Panel pnlMapContainer;
    }
}