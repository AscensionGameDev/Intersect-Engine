using System;
using DarkUI.Controls;

namespace Intersect_Editor.Forms.Controls
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
            this.pnlLight = new System.Windows.Forms.Panel();
            this.grpLightEditor = new DarkUI.Controls.DarkGroupBox();
            this.scrlLightExpand = new DarkUI.Controls.DarkScrollBar();
            this.lblExpandAmt = new System.Windows.Forms.Label();
            this.txtLightExpandAmt = new DarkUI.Controls.DarkTextBox();
            this.scrlLightSize = new DarkUI.Controls.DarkScrollBar();
            this.pnlLightColor = new System.Windows.Forms.Panel();
            this.btnOkay = new DarkUI.Controls.DarkButton();
            this.lblColor = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSelectLightColor = new DarkUI.Controls.DarkButton();
            this.lblOffsetX = new System.Windows.Forms.Label();
            this.lblOffsetY = new System.Windows.Forms.Label();
            this.lblSize = new System.Windows.Forms.Label();
            this.txtLightOffsetX = new DarkUI.Controls.DarkTextBox();
            this.txtLightRange = new DarkUI.Controls.DarkTextBox();
            this.txtLightOffsetY = new DarkUI.Controls.DarkTextBox();
            this.scrlLightIntensity = new DarkUI.Controls.DarkScrollBar();
            this.txtLightIntensity = new DarkUI.Controls.DarkTextBox();
            this.lblIntensity = new System.Windows.Forms.Label();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.pnlLight.SuspendLayout();
            this.grpLightEditor.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLight
            // 
            this.pnlLight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.pnlLight.Controls.Add(this.grpLightEditor);
            this.pnlLight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlLight.Location = new System.Drawing.Point(0, 0);
            this.pnlLight.Name = "pnlLight";
            this.pnlLight.Size = new System.Drawing.Size(258, 323);
            this.pnlLight.TabIndex = 4;
            // 
            // grpLightEditor
            // 
            this.grpLightEditor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpLightEditor.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpLightEditor.Controls.Add(this.scrlLightExpand);
            this.grpLightEditor.Controls.Add(this.lblExpandAmt);
            this.grpLightEditor.Controls.Add(this.txtLightExpandAmt);
            this.grpLightEditor.Controls.Add(this.scrlLightSize);
            this.grpLightEditor.Controls.Add(this.pnlLightColor);
            this.grpLightEditor.Controls.Add(this.btnOkay);
            this.grpLightEditor.Controls.Add(this.lblColor);
            this.grpLightEditor.Controls.Add(this.btnCancel);
            this.grpLightEditor.Controls.Add(this.btnSelectLightColor);
            this.grpLightEditor.Controls.Add(this.lblOffsetX);
            this.grpLightEditor.Controls.Add(this.lblOffsetY);
            this.grpLightEditor.Controls.Add(this.lblSize);
            this.grpLightEditor.Controls.Add(this.txtLightOffsetX);
            this.grpLightEditor.Controls.Add(this.txtLightRange);
            this.grpLightEditor.Controls.Add(this.txtLightOffsetY);
            this.grpLightEditor.Controls.Add(this.scrlLightIntensity);
            this.grpLightEditor.Controls.Add(this.txtLightIntensity);
            this.grpLightEditor.Controls.Add(this.lblIntensity);
            this.grpLightEditor.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpLightEditor.Location = new System.Drawing.Point(4, 4);
            this.grpLightEditor.Name = "grpLightEditor";
            this.grpLightEditor.Size = new System.Drawing.Size(248, 316);
            this.grpLightEditor.TabIndex = 39;
            this.grpLightEditor.TabStop = false;
            this.grpLightEditor.Text = "Light Editor";
            // 
            // scrlLightExpand
            // 
            this.scrlLightExpand.Location = new System.Drawing.Point(9, 251);
            this.scrlLightExpand.Name = "scrlLightExpand";
            this.scrlLightExpand.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlLightExpand.Size = new System.Drawing.Size(225, 17);
            this.scrlLightExpand.TabIndex = 41;
            this.scrlLightExpand.Value = 2;
            this.scrlLightExpand.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlLightExpand_Scroll);
            // 
            // lblExpandAmt
            // 
            this.lblExpandAmt.AutoSize = true;
            this.lblExpandAmt.Location = new System.Drawing.Point(6, 212);
            this.lblExpandAmt.Name = "lblExpandAmt";
            this.lblExpandAmt.Size = new System.Drawing.Size(67, 13);
            this.lblExpandAmt.TabIndex = 40;
            this.lblExpandAmt.Text = "Expand Amt:";
            // 
            // txtLightExpandAmt
            // 
            this.txtLightExpandAmt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtLightExpandAmt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLightExpandAmt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtLightExpandAmt.Location = new System.Drawing.Point(9, 228);
            this.txtLightExpandAmt.Name = "txtLightExpandAmt";
            this.txtLightExpandAmt.Size = new System.Drawing.Size(225, 20);
            this.txtLightExpandAmt.TabIndex = 39;
            this.txtLightExpandAmt.TextChanged += new System.EventHandler(this.txtLightExpandAmt_TextChanged);
            // 
            // scrlLightSize
            // 
            this.scrlLightSize.Location = new System.Drawing.Point(9, 195);
            this.scrlLightSize.Maximum = 1000;
            this.scrlLightSize.Name = "scrlLightSize";
            this.scrlLightSize.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlLightSize.Size = new System.Drawing.Size(225, 17);
            this.scrlLightSize.TabIndex = 35;
            this.scrlLightSize.Value = 2;
            this.scrlLightSize.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlLightSize_Scroll);
            // 
            // pnlLightColor
            // 
            this.pnlLightColor.BackColor = System.Drawing.Color.White;
            this.pnlLightColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlLightColor.Location = new System.Drawing.Point(144, 38);
            this.pnlLightColor.Name = "pnlLightColor";
            this.pnlLightColor.Size = new System.Drawing.Size(31, 29);
            this.pnlLightColor.TabIndex = 38;
            // 
            // btnOkay
            // 
            this.btnOkay.Location = new System.Drawing.Point(9, 286);
            this.btnOkay.Name = "btnOkay";
            this.btnOkay.Padding = new System.Windows.Forms.Padding(5);
            this.btnOkay.Size = new System.Drawing.Size(75, 23);
            this.btnOkay.TabIndex = 24;
            this.btnOkay.Text = "Save";
            this.btnOkay.Click += new System.EventHandler(this.btnLightEditorClose_Click);
            // 
            // lblColor
            // 
            this.lblColor.AutoSize = true;
            this.lblColor.Location = new System.Drawing.Point(141, 22);
            this.lblColor.Name = "lblColor";
            this.lblColor.Size = new System.Drawing.Size(34, 13);
            this.lblColor.TabIndex = 37;
            this.lblColor.Text = "Color:";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(159, 286);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 25;
            this.btnCancel.Text = "Revert";
            this.btnCancel.Click += new System.EventHandler(this.btnLightEditorRevert_Click);
            // 
            // btnSelectLightColor
            // 
            this.btnSelectLightColor.Location = new System.Drawing.Point(144, 75);
            this.btnSelectLightColor.Name = "btnSelectLightColor";
            this.btnSelectLightColor.Padding = new System.Windows.Forms.Padding(5);
            this.btnSelectLightColor.Size = new System.Drawing.Size(90, 23);
            this.btnSelectLightColor.TabIndex = 36;
            this.btnSelectLightColor.Text = "Select Color";
            this.btnSelectLightColor.Click += new System.EventHandler(this.btnSelectLightColor_Click);
            // 
            // lblOffsetX
            // 
            this.lblOffsetX.AutoSize = true;
            this.lblOffsetX.Location = new System.Drawing.Point(6, 22);
            this.lblOffsetX.Name = "lblOffsetX";
            this.lblOffsetX.Size = new System.Drawing.Size(48, 13);
            this.lblOffsetX.TabIndex = 26;
            this.lblOffsetX.Text = "Offset X:";
            // 
            // lblOffsetY
            // 
            this.lblOffsetY.AutoSize = true;
            this.lblOffsetY.Location = new System.Drawing.Point(6, 61);
            this.lblOffsetY.Name = "lblOffsetY";
            this.lblOffsetY.Size = new System.Drawing.Size(48, 13);
            this.lblOffsetY.TabIndex = 27;
            this.lblOffsetY.Text = "Offset Y:";
            // 
            // lblSize
            // 
            this.lblSize.AutoSize = true;
            this.lblSize.Location = new System.Drawing.Point(6, 156);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new System.Drawing.Size(30, 13);
            this.lblSize.TabIndex = 34;
            this.lblSize.Text = "Size:";
            // 
            // txtLightOffsetX
            // 
            this.txtLightOffsetX.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtLightOffsetX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLightOffsetX.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtLightOffsetX.Location = new System.Drawing.Point(9, 38);
            this.txtLightOffsetX.Name = "txtLightOffsetX";
            this.txtLightOffsetX.Size = new System.Drawing.Size(114, 20);
            this.txtLightOffsetX.TabIndex = 28;
            this.txtLightOffsetX.TextChanged += new System.EventHandler(this.txtLightOffsetX_TextChanged);
            // 
            // txtLightRange
            // 
            this.txtLightRange.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtLightRange.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLightRange.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtLightRange.Location = new System.Drawing.Point(9, 172);
            this.txtLightRange.Name = "txtLightRange";
            this.txtLightRange.Size = new System.Drawing.Size(225, 20);
            this.txtLightRange.TabIndex = 33;
            this.txtLightRange.TextChanged += new System.EventHandler(this.txtLightRange_TextChanged);
            // 
            // txtLightOffsetY
            // 
            this.txtLightOffsetY.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtLightOffsetY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLightOffsetY.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtLightOffsetY.Location = new System.Drawing.Point(9, 77);
            this.txtLightOffsetY.Name = "txtLightOffsetY";
            this.txtLightOffsetY.Size = new System.Drawing.Size(114, 20);
            this.txtLightOffsetY.TabIndex = 29;
            this.txtLightOffsetY.TextChanged += new System.EventHandler(this.txtLightOffsetY_TextChanged);
            // 
            // scrlLightIntensity
            // 
            this.scrlLightIntensity.Location = new System.Drawing.Point(9, 139);
            this.scrlLightIntensity.Maximum = 255;
            this.scrlLightIntensity.Name = "scrlLightIntensity";
            this.scrlLightIntensity.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlLightIntensity.Size = new System.Drawing.Size(225, 17);
            this.scrlLightIntensity.TabIndex = 32;
            this.scrlLightIntensity.Value = 1;
            this.scrlLightIntensity.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlLightIntensity_Scroll);
            this.scrlLightIntensity.Click += new System.EventHandler(this.scrlLightIntensity_Click);
            // 
            // txtLightIntensity
            // 
            this.txtLightIntensity.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtLightIntensity.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLightIntensity.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtLightIntensity.Location = new System.Drawing.Point(9, 116);
            this.txtLightIntensity.Name = "txtLightIntensity";
            this.txtLightIntensity.Size = new System.Drawing.Size(225, 20);
            this.txtLightIntensity.TabIndex = 30;
            this.txtLightIntensity.TextChanged += new System.EventHandler(this.txtLightIntensity_TextChanged);
            // 
            // lblIntensity
            // 
            this.lblIntensity.AutoSize = true;
            this.lblIntensity.Location = new System.Drawing.Point(6, 100);
            this.lblIntensity.Name = "lblIntensity";
            this.lblIntensity.Size = new System.Drawing.Size(49, 13);
            this.lblIntensity.TabIndex = 31;
            this.lblIntensity.Text = "Intensity:";
            // 
            // LightEditorCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlLight);
            this.Name = "LightEditorCtrl";
            this.Size = new System.Drawing.Size(258, 323);
            this.pnlLight.ResumeLayout(false);
            this.grpLightEditor.ResumeLayout(false);
            this.grpLightEditor.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Panel pnlLight;
        public System.Windows.Forms.Panel pnlLightColor;
        private System.Windows.Forms.Label lblColor;
        private DarkButton btnSelectLightColor;
        public DarkScrollBar scrlLightSize;
        private System.Windows.Forms.Label lblSize;
        public DarkTextBox txtLightRange;
        public DarkScrollBar scrlLightIntensity;
        private System.Windows.Forms.Label lblIntensity;
        public DarkTextBox txtLightIntensity;
        public DarkTextBox txtLightOffsetY;
        public DarkTextBox txtLightOffsetX;
        private System.Windows.Forms.Label lblOffsetY;
        private System.Windows.Forms.Label lblOffsetX;
        private DarkButton btnCancel;
        private DarkButton btnOkay;
        private System.Windows.Forms.ColorDialog colorDialog;
        private DarkGroupBox grpLightEditor;
        public DarkScrollBar scrlLightExpand;
        private System.Windows.Forms.Label lblExpandAmt;
        public DarkTextBox txtLightExpandAmt;
    }
}
