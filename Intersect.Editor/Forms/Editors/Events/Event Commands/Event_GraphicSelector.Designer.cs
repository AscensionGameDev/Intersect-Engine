using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    partial class EventGraphicSelector
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
            this.grpSelector = new DarkUI.Controls.DarkGroupBox();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnOk = new DarkUI.Controls.DarkButton();
            this.cmbGraphic = new DarkUI.Controls.DarkComboBox();
            this.lblGraphic = new System.Windows.Forms.Label();
            this.cmbGraphicType = new DarkUI.Controls.DarkComboBox();
            this.lblType = new System.Windows.Forms.Label();
            this.grpPreview = new DarkUI.Controls.DarkGroupBox();
            this.pnlGraphicContainer = new System.Windows.Forms.Panel();
            this.pnlGraphic = new System.Windows.Forms.PictureBox();
            this.grpSelector.SuspendLayout();
            this.grpPreview.SuspendLayout();
            this.pnlGraphicContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlGraphic)).BeginInit();
            this.SuspendLayout();
            // 
            // grpSelector
            // 
            this.grpSelector.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpSelector.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpSelector.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpSelector.Controls.Add(this.btnCancel);
            this.grpSelector.Controls.Add(this.btnOk);
            this.grpSelector.Controls.Add(this.cmbGraphic);
            this.grpSelector.Controls.Add(this.lblGraphic);
            this.grpSelector.Controls.Add(this.cmbGraphicType);
            this.grpSelector.Controls.Add(this.lblType);
            this.grpSelector.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpSelector.Location = new System.Drawing.Point(3, 3);
            this.grpSelector.Name = "grpSelector";
            this.grpSelector.Size = new System.Drawing.Size(613, 62);
            this.grpSelector.TabIndex = 0;
            this.grpSelector.TabStop = false;
            this.grpSelector.Text = "Graphic Selector";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(525, 22);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(82, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(439, 22);
            this.btnOk.Name = "btnOk";
            this.btnOk.Padding = new System.Windows.Forms.Padding(5);
            this.btnOk.Size = new System.Drawing.Size(82, 23);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "Ok";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // cmbGraphic
            // 
            this.cmbGraphic.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbGraphic.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbGraphic.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbGraphic.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbGraphic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGraphic.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
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
            this.cmbGraphicType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbGraphicType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbGraphicType.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbGraphicType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbGraphicType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGraphicType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
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
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(6, 26);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(77, 13);
            this.lblType.TabIndex = 0;
            this.lblType.Text = "Graphic Type: ";
            // 
            // grpPreview
            // 
            this.grpPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpPreview.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpPreview.Controls.Add(this.pnlGraphicContainer);
            this.grpPreview.ForeColor = System.Drawing.Color.Gainsboro;
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
            this.pnlGraphicContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
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
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpPreview);
            this.Controls.Add(this.grpSelector);
            this.Name = "EventGraphicSelector";
            this.Size = new System.Drawing.Size(625, 519);
            this.grpSelector.ResumeLayout(false);
            this.grpSelector.PerformLayout();
            this.grpPreview.ResumeLayout(false);
            this.pnlGraphicContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlGraphic)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpSelector;
        private DarkComboBox cmbGraphic;
        private System.Windows.Forms.Label lblGraphic;
        private DarkComboBox cmbGraphicType;
        private System.Windows.Forms.Label lblType;
        private DarkGroupBox grpPreview;
        private System.Windows.Forms.Panel pnlGraphicContainer;
        public System.Windows.Forms.PictureBox pnlGraphic;
        private DarkButton btnCancel;
        private DarkButton btnOk;
    }
}
