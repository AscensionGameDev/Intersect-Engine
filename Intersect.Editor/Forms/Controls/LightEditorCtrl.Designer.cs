using System;
using DarkUI.Controls;

namespace Intersect.Editor.Forms.Controls
{
    partial class LightEditorCtrl
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
            components = new System.ComponentModel.Container();
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(LightEditorCtrl));
            pnlLight = new Panel();
            grpLightEditor = new DarkGroupBox();
            nudOffsetY = new DarkNumericUpDown();
            nudOffsetX = new DarkNumericUpDown();
            nudIntensity = new DarkNumericUpDown();
            nudSize = new DarkNumericUpDown();
            nudExpand = new DarkNumericUpDown();
            lblExpandAmt = new Label();
            pnlLightColor = new Panel();
            btnOkay = new DarkButton();
            imglstLightEditor = new ImageList(components);
            lblColor = new Label();
            btnCancel = new DarkButton();
            btnSelectLightColor = new DarkButton();
            lblOffsetX = new Label();
            lblOffsetY = new Label();
            lblSize = new Label();
            lblIntensity = new Label();
            colorDialog = new ColorDialog();
            pnlLight.SuspendLayout();
            grpLightEditor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudOffsetY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudOffsetX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudIntensity).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudSize).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudExpand).BeginInit();
            SuspendLayout();
            // 
            // pnlLight
            // 
            pnlLight.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlLight.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            pnlLight.Controls.Add(grpLightEditor);
            pnlLight.Dock = DockStyle.Fill;
            pnlLight.Location = new System.Drawing.Point(0, 0);
            pnlLight.Margin = new Padding(4, 3, 4, 3);
            pnlLight.Name = "pnlLight";
            pnlLight.Size = new Size(184, 192);
            pnlLight.TabIndex = 4;
            // 
            // grpLightEditor
            // 
            grpLightEditor.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpLightEditor.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpLightEditor.Controls.Add(nudOffsetY);
            grpLightEditor.Controls.Add(nudOffsetX);
            grpLightEditor.Controls.Add(nudIntensity);
            grpLightEditor.Controls.Add(nudSize);
            grpLightEditor.Controls.Add(nudExpand);
            grpLightEditor.Controls.Add(lblExpandAmt);
            grpLightEditor.Controls.Add(pnlLightColor);
            grpLightEditor.Controls.Add(btnOkay);
            grpLightEditor.Controls.Add(lblColor);
            grpLightEditor.Controls.Add(btnCancel);
            grpLightEditor.Controls.Add(btnSelectLightColor);
            grpLightEditor.Controls.Add(lblOffsetX);
            grpLightEditor.Controls.Add(lblOffsetY);
            grpLightEditor.Controls.Add(lblSize);
            grpLightEditor.Controls.Add(lblIntensity);
            grpLightEditor.ForeColor = System.Drawing.Color.Gainsboro;
            grpLightEditor.Location = new System.Drawing.Point(0, 0);
            grpLightEditor.Margin = new Padding(4);
            grpLightEditor.Name = "grpLightEditor";
            grpLightEditor.Padding = new Padding(4);
            grpLightEditor.Size = new Size(184, 192);
            grpLightEditor.TabIndex = 39;
            grpLightEditor.TabStop = false;
            grpLightEditor.Text = "Light Editor";
            // 
            // nudOffsetY
            // 
            nudOffsetY.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudOffsetY.ForeColor = System.Drawing.Color.Gainsboro;
            nudOffsetY.Location = new System.Drawing.Point(96, 43);
            nudOffsetY.Margin = new Padding(4, 3, 4, 3);
            nudOffsetY.Maximum = new decimal(new int[] { 128, 0, 0, 0 });
            nudOffsetY.Minimum = new decimal(new int[] { 128, 0, 0, int.MinValue });
            nudOffsetY.Name = "nudOffsetY";
            nudOffsetY.Size = new Size(80, 23);
            nudOffsetY.TabIndex = 45;
            nudOffsetY.Value = new decimal(new int[] { 128, 0, 0, int.MinValue });
            nudOffsetY.ValueChanged += nudOffsetY_ValueChanged;
            // 
            // nudOffsetX
            // 
            nudOffsetX.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudOffsetX.ForeColor = System.Drawing.Color.Gainsboro;
            nudOffsetX.Location = new System.Drawing.Point(8, 43);
            nudOffsetX.Margin = new Padding(4, 3, 4, 3);
            nudOffsetX.Maximum = new decimal(new int[] { 128, 0, 0, 0 });
            nudOffsetX.Minimum = new decimal(new int[] { 128, 0, 0, int.MinValue });
            nudOffsetX.Name = "nudOffsetX";
            nudOffsetX.Size = new Size(80, 23);
            nudOffsetX.TabIndex = 44;
            nudOffsetX.Value = new decimal(new int[] { 128, 0, 0, int.MinValue });
            nudOffsetX.ValueChanged += nudOffsetX_ValueChanged;
            // 
            // nudIntensity
            // 
            nudIntensity.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudIntensity.ForeColor = System.Drawing.Color.Gainsboro;
            nudIntensity.Location = new System.Drawing.Point(8, 131);
            nudIntensity.Margin = new Padding(4, 3, 4, 3);
            nudIntensity.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            nudIntensity.Name = "nudIntensity";
            nudIntensity.Size = new Size(80, 23);
            nudIntensity.TabIndex = 43;
            nudIntensity.Value = new decimal(new int[] { 255, 0, 0, 0 });
            nudIntensity.ValueChanged += nudIntensity_ValueChanged;
            // 
            // nudSize
            // 
            nudSize.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudSize.ForeColor = System.Drawing.Color.Gainsboro;
            nudSize.Location = new System.Drawing.Point(8, 87);
            nudSize.Margin = new Padding(4, 3, 4, 3);
            nudSize.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudSize.Name = "nudSize";
            nudSize.Size = new Size(80, 23);
            nudSize.TabIndex = 42;
            nudSize.Value = new decimal(new int[] { 1000, 0, 0, 0 });
            nudSize.ValueChanged += nudSize_ValueChanged;
            // 
            // nudExpand
            // 
            nudExpand.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudExpand.ForeColor = System.Drawing.Color.Gainsboro;
            nudExpand.Location = new System.Drawing.Point(96, 87);
            nudExpand.Margin = new Padding(4, 3, 4, 3);
            nudExpand.Name = "nudExpand";
            nudExpand.Size = new Size(80, 23);
            nudExpand.TabIndex = 41;
            nudExpand.Value = new decimal(new int[] { 100, 0, 0, 0 });
            nudExpand.ValueChanged += nudExpand_ValueChanged;
            // 
            // lblExpandAmt
            // 
            lblExpandAmt.AutoSize = true;
            lblExpandAmt.Location = new System.Drawing.Point(96, 70);
            lblExpandAmt.Margin = new Padding(4, 0, 4, 0);
            lblExpandAmt.Name = "lblExpandAmt";
            lblExpandAmt.Size = new Size(74, 15);
            lblExpandAmt.TabIndex = 40;
            lblExpandAmt.Text = "Expansion %";
            // 
            // pnlLightColor
            // 
            pnlLightColor.BackColor = System.Drawing.Color.White;
            pnlLightColor.BorderStyle = BorderStyle.FixedSingle;
            pnlLightColor.Location = new System.Drawing.Point(96, 131);
            pnlLightColor.Margin = new Padding(4, 3, 4, 3);
            pnlLightColor.Name = "pnlLightColor";
            pnlLightColor.Size = new Size(23, 23);
            pnlLightColor.TabIndex = 38;
            // 
            // btnOkay
            // 
            btnOkay.ImageKey = "sharp_save_white_48dp.png";
            btnOkay.ImageList = imglstLightEditor;
            btnOkay.Location = new System.Drawing.Point(8, 160);
            btnOkay.Margin = new Padding(4, 3, 4, 3);
            btnOkay.Name = "btnOkay";
            btnOkay.Padding = new Padding(6);
            btnOkay.Size = new Size(23, 23);
            btnOkay.TabIndex = 24;
            btnOkay.Click += btnLightEditorClose_Click;
            // 
            // imglstLightEditor
            // 
            imglstLightEditor.ColorDepth = ColorDepth.Depth32Bit;
            imglstLightEditor.ImageStream = (ImageListStreamer)resources.GetObject("imglstLightEditor.ImageStream");
            imglstLightEditor.TransparentColor = System.Drawing.Color.Transparent;
            imglstLightEditor.Images.SetKeyName(0, "sharp_colorize_white_48dp.png");
            imglstLightEditor.Images.SetKeyName(1, "sharp_save_white_48dp.png");
            imglstLightEditor.Images.SetKeyName(2, "sharp_undo_white_48dp.png");
            // 
            // lblColor
            // 
            lblColor.AutoSize = true;
            lblColor.Location = new System.Drawing.Point(96, 114);
            lblColor.Margin = new Padding(4, 0, 4, 0);
            lblColor.Name = "lblColor";
            lblColor.Size = new Size(36, 15);
            lblColor.TabIndex = 37;
            lblColor.Text = "Color";
            // 
            // btnCancel
            // 
            btnCancel.ImageKey = "sharp_undo_white_48dp.png";
            btnCancel.ImageList = imglstLightEditor;
            btnCancel.Location = new System.Drawing.Point(153, 160);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Padding = new Padding(6);
            btnCancel.Size = new Size(23, 23);
            btnCancel.TabIndex = 25;
            btnCancel.Click += btnLightEditorRevert_Click;
            // 
            // btnSelectLightColor
            // 
            btnSelectLightColor.ImageKey = "sharp_colorize_white_48dp.png";
            btnSelectLightColor.ImageList = imglstLightEditor;
            btnSelectLightColor.Location = new System.Drawing.Point(127, 131);
            btnSelectLightColor.Margin = new Padding(4, 3, 4, 3);
            btnSelectLightColor.Name = "btnSelectLightColor";
            btnSelectLightColor.Padding = new Padding(6);
            btnSelectLightColor.Size = new Size(23, 23);
            btnSelectLightColor.TabIndex = 36;
            btnSelectLightColor.Click += btnSelectLightColor_Click;
            // 
            // lblOffsetX
            // 
            lblOffsetX.AutoSize = true;
            lblOffsetX.Location = new System.Drawing.Point(8, 20);
            lblOffsetX.Margin = new Padding(4, 0, 4, 0);
            lblOffsetX.Name = "lblOffsetX";
            lblOffsetX.Size = new Size(49, 15);
            lblOffsetX.TabIndex = 26;
            lblOffsetX.Text = "Offset X";
            // 
            // lblOffsetY
            // 
            lblOffsetY.AutoSize = true;
            lblOffsetY.Location = new System.Drawing.Point(73, 20);
            lblOffsetY.Margin = new Padding(4, 0, 4, 0);
            lblOffsetY.Name = "lblOffsetY";
            lblOffsetY.Size = new Size(49, 15);
            lblOffsetY.TabIndex = 27;
            lblOffsetY.Text = "Offset Y";
            // 
            // lblSize
            // 
            lblSize.AutoSize = true;
            lblSize.Location = new System.Drawing.Point(8, 70);
            lblSize.Margin = new Padding(4, 0, 4, 0);
            lblSize.Name = "lblSize";
            lblSize.Size = new Size(51, 15);
            lblSize.TabIndex = 34;
            lblSize.Text = "Size (px)";
            // 
            // lblIntensity
            // 
            lblIntensity.AutoSize = true;
            lblIntensity.Location = new System.Drawing.Point(8, 114);
            lblIntensity.Margin = new Padding(4, 0, 4, 0);
            lblIntensity.Name = "lblIntensity";
            lblIntensity.Size = new Size(52, 15);
            lblIntensity.TabIndex = 31;
            lblIntensity.Text = "Intensity";
            // 
            // LightEditorCtrl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pnlLight);
            Margin = new Padding(4, 3, 4, 3);
            Name = "LightEditorCtrl";
            Size = new Size(184, 192);
            pnlLight.ResumeLayout(false);
            grpLightEditor.ResumeLayout(false);
            grpLightEditor.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudOffsetY).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudOffsetX).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudIntensity).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudSize).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudExpand).EndInit();
            ResumeLayout(false);
        }

        #endregion

        public Panel pnlLight;
        public Panel pnlLightColor;
        private ImageList imglstLightEditor;
        private Label lblColor;
        private DarkButton btnSelectLightColor;
        private Label lblSize;
        private Label lblIntensity;
        private Label lblOffsetY;
        private Label lblOffsetX;
        private DarkButton btnCancel;
        private DarkButton btnOkay;
        private ColorDialog colorDialog;
        private DarkGroupBox grpLightEditor;
        private Label lblExpandAmt;
        private DarkNumericUpDown nudOffsetY;
        private DarkNumericUpDown nudOffsetX;
        private DarkNumericUpDown nudIntensity;
        private DarkNumericUpDown nudSize;
        private DarkNumericUpDown nudExpand;
    }
}
