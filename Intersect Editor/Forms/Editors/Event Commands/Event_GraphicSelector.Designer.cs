namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    partial class Event_GraphicSelector
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.cmbGraphic = new System.Windows.Forms.ComboBox();
            this.lblGraphic = new System.Windows.Forms.Label();
            this.cmbGraphicType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.grpPreview = new System.Windows.Forms.GroupBox();
            this.pnlGraphicContainer = new System.Windows.Forms.Panel();
            this.pnlGraphic = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            this.grpPreview.SuspendLayout();
            this.pnlGraphicContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlGraphic)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnOk);
            this.groupBox1.Controls.Add(this.cmbGraphic);
            this.groupBox1.Controls.Add(this.lblGraphic);
            this.groupBox1.Controls.Add(this.cmbGraphicType);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(613, 62);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Graphic Selector";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(525, 22);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(82, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(439, 22);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(82, 23);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // cmbGraphic
            // 
            this.cmbGraphic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGraphic.FormattingEnabled = true;
            this.cmbGraphic.Location = new System.Drawing.Point(291, 24);
            this.cmbGraphic.Name = "cmbGraphic";
            this.cmbGraphic.Size = new System.Drawing.Size(132, 21);
            this.cmbGraphic.TabIndex = 3;
            this.cmbGraphic.SelectedIndexChanged += new System.EventHandler(this.cmbGraphic_SelectedIndexChanged);
            // 
            // lblGraphic
            // 
            this.lblGraphic.AutoSize = true;
            this.lblGraphic.Location = new System.Drawing.Point(238, 27);
            this.lblGraphic.Name = "lblGraphic";
            this.lblGraphic.Size = new System.Drawing.Size(47, 13);
            this.lblGraphic.TabIndex = 2;
            this.lblGraphic.Text = "Graphic:";
            // 
            // cmbGraphicType
            // 
            this.cmbGraphicType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGraphicType.FormattingEnabled = true;
            this.cmbGraphicType.Items.AddRange(new object[] {
            "None",
            "Sprite",
            "Tileset"});
            this.cmbGraphicType.Location = new System.Drawing.Point(89, 23);
            this.cmbGraphicType.Name = "cmbGraphicType";
            this.cmbGraphicType.Size = new System.Drawing.Size(121, 21);
            this.cmbGraphicType.TabIndex = 1;
            this.cmbGraphicType.SelectedIndexChanged += new System.EventHandler(this.cmbGraphicType_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Graphic Type: ";
            // 
            // grpPreview
            // 
            this.grpPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpPreview.Controls.Add(this.pnlGraphicContainer);
            this.grpPreview.Location = new System.Drawing.Point(3, 71);
            this.grpPreview.Name = "grpPreview";
            this.grpPreview.Size = new System.Drawing.Size(613, 445);
            this.grpPreview.TabIndex = 1;
            this.grpPreview.TabStop = false;
            this.grpPreview.Text = "Graphic Preview";
            // 
            // pnlGraphicContainer
            // 
            this.pnlGraphicContainer.AutoScroll = true;
            this.pnlGraphicContainer.BackColor = System.Drawing.SystemColors.Control;
            this.pnlGraphicContainer.Controls.Add(this.pnlGraphic);
            this.pnlGraphicContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGraphicContainer.Location = new System.Drawing.Point(3, 16);
            this.pnlGraphicContainer.Name = "pnlGraphicContainer";
            this.pnlGraphicContainer.Size = new System.Drawing.Size(607, 426);
            this.pnlGraphicContainer.TabIndex = 20;
            // 
            // pnlGraphic
            // 
            this.pnlGraphic.Location = new System.Drawing.Point(1, 1);
            this.pnlGraphic.Name = "pnlGraphic";
            this.pnlGraphic.Size = new System.Drawing.Size(167, 148);
            this.pnlGraphic.TabIndex = 2;
            this.pnlGraphic.TabStop = false;
            this.pnlGraphic.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlGraphic_MouseDown);
            this.pnlGraphic.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlGraphic_MouseMove);
            this.pnlGraphic.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlGraphic_MouseUp);
            // 
            // Event_GraphicSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpPreview);
            this.Controls.Add(this.groupBox1);
            this.Name = "Event_GraphicSelector";
            this.Size = new System.Drawing.Size(625, 519);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpPreview.ResumeLayout(false);
            this.pnlGraphicContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlGraphic)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cmbGraphic;
        private System.Windows.Forms.Label lblGraphic;
        private System.Windows.Forms.ComboBox cmbGraphicType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox grpPreview;
        private System.Windows.Forms.Panel pnlGraphicContainer;
        public System.Windows.Forms.PictureBox pnlGraphic;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
    }
}
