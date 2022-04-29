using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors
{
    partial class FrmTime
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmTime));
            this.lstTimes = new System.Windows.Forms.ListBox();
            this.lblTimes = new System.Windows.Forms.Label();
            this.grpRangeOptions = new DarkUI.Controls.DarkGroupBox();
            this.scrlAlpha = new DarkUI.Controls.DarkScrollBar();
            this.lblBrightness = new System.Windows.Forms.Label();
            this.lblColorDesc = new System.Windows.Forms.Label();
            this.pnlColor = new System.Windows.Forms.Panel();
            this.grpSettings = new DarkUI.Controls.DarkGroupBox();
            this.lblRateDesc = new System.Windows.Forms.Label();
            this.lblRateSuffix = new System.Windows.Forms.Label();
            this.txtTimeRate = new DarkUI.Controls.DarkTextBox();
            this.lblRate = new System.Windows.Forms.Label();
            this.lblIntervals = new System.Windows.Forms.Label();
            this.cmbIntervals = new DarkUI.Controls.DarkComboBox();
            this.chkSync = new DarkUI.Controls.DarkCheckBox();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.clrSelector = new System.Windows.Forms.ColorDialog();
            this.grpRangeOptions.SuspendLayout();
            this.grpSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstTimes
            // 
            this.lstTimes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.lstTimes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstTimes.ForeColor = System.Drawing.Color.Gainsboro;
            this.lstTimes.FormattingEnabled = true;
            this.lstTimes.Location = new System.Drawing.Point(14, 25);
            this.lstTimes.Name = "lstTimes";
            this.lstTimes.Size = new System.Drawing.Size(221, 275);
            this.lstTimes.TabIndex = 0;
            this.lstTimes.SelectedIndexChanged += new System.EventHandler(this.lstTimes_SelectedIndexChanged);
            // 
            // lblTimes
            // 
            this.lblTimes.AutoSize = true;
            this.lblTimes.ForeColor = System.Drawing.Color.Gainsboro;
            this.lblTimes.Location = new System.Drawing.Point(11, 9);
            this.lblTimes.Name = "lblTimes";
            this.lblTimes.Size = new System.Drawing.Size(64, 13);
            this.lblTimes.TabIndex = 1;
            this.lblTimes.Text = "Time of Day";
            // 
            // grpRangeOptions
            // 
            this.grpRangeOptions.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpRangeOptions.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpRangeOptions.Controls.Add(this.scrlAlpha);
            this.grpRangeOptions.Controls.Add(this.lblBrightness);
            this.grpRangeOptions.Controls.Add(this.lblColorDesc);
            this.grpRangeOptions.Controls.Add(this.pnlColor);
            this.grpRangeOptions.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpRangeOptions.Location = new System.Drawing.Point(249, 206);
            this.grpRangeOptions.Name = "grpRangeOptions";
            this.grpRangeOptions.Size = new System.Drawing.Size(268, 98);
            this.grpRangeOptions.TabIndex = 3;
            this.grpRangeOptions.TabStop = false;
            this.grpRangeOptions.Text = "Range Overlay";
            this.grpRangeOptions.Visible = false;
            // 
            // scrlAlpha
            // 
            this.scrlAlpha.Location = new System.Drawing.Point(141, 26);
            this.scrlAlpha.Maximum = 255;
            this.scrlAlpha.Name = "scrlAlpha";
            this.scrlAlpha.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlAlpha.Size = new System.Drawing.Size(118, 17);
            this.scrlAlpha.TabIndex = 3;
            this.scrlAlpha.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlAlpha_Scroll);
            // 
            // lblBrightness
            // 
            this.lblBrightness.AutoSize = true;
            this.lblBrightness.Location = new System.Drawing.Point(47, 29);
            this.lblBrightness.Name = "lblBrightness";
            this.lblBrightness.Size = new System.Drawing.Size(80, 13);
            this.lblBrightness.TabIndex = 2;
            this.lblBrightness.Text = "Brightness: 100";
            // 
            // lblColorDesc
            // 
            this.lblColorDesc.Location = new System.Drawing.Point(9, 59);
            this.lblColorDesc.Name = "lblColorDesc";
            this.lblColorDesc.Size = new System.Drawing.Size(253, 30);
            this.lblColorDesc.TabIndex = 1;
            this.lblColorDesc.Text = "Double click the panel above to change the overlay color.";
            // 
            // pnlColor
            // 
            this.pnlColor.BackColor = System.Drawing.Color.Black;
            this.pnlColor.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pnlColor.BackgroundImage")));
            this.pnlColor.Location = new System.Drawing.Point(9, 20);
            this.pnlColor.Name = "pnlColor";
            this.pnlColor.Size = new System.Drawing.Size(32, 32);
            this.pnlColor.TabIndex = 0;
            this.pnlColor.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlColor_Paint);
            this.pnlColor.DoubleClick += new System.EventHandler(this.pnlColor_DoubleClick);
            // 
            // grpSettings
            // 
            this.grpSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpSettings.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpSettings.Controls.Add(this.lblRateDesc);
            this.grpSettings.Controls.Add(this.lblRateSuffix);
            this.grpSettings.Controls.Add(this.txtTimeRate);
            this.grpSettings.Controls.Add(this.lblRate);
            this.grpSettings.Controls.Add(this.lblIntervals);
            this.grpSettings.Controls.Add(this.cmbIntervals);
            this.grpSettings.Controls.Add(this.chkSync);
            this.grpSettings.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpSettings.Location = new System.Drawing.Point(249, 25);
            this.grpSettings.Name = "grpSettings";
            this.grpSettings.Size = new System.Drawing.Size(268, 175);
            this.grpSettings.TabIndex = 3;
            this.grpSettings.TabStop = false;
            this.grpSettings.Text = "Time Settings";
            // 
            // lblRateDesc
            // 
            this.lblRateDesc.Location = new System.Drawing.Point(9, 100);
            this.lblRateDesc.Name = "lblRateDesc";
            this.lblRateDesc.Size = new System.Drawing.Size(218, 67);
            this.lblRateDesc.TabIndex = 6;
            this.lblRateDesc.Text = "Enter 1 for normal rate of time.\r\nValues larger than one for faster days. \r\nValue" +
    "s between 0 and 1 for longer days. \r\nNegative values for time to flow backwards." +
    "";
            // 
            // lblRateSuffix
            // 
            this.lblRateSuffix.AutoSize = true;
            this.lblRateSuffix.Location = new System.Drawing.Point(120, 68);
            this.lblRateSuffix.Name = "lblRateSuffix";
            this.lblRateSuffix.Size = new System.Drawing.Size(48, 13);
            this.lblRateSuffix.TabIndex = 5;
            this.lblRateSuffix.Text = "x Normal";
            // 
            // txtTimeRate
            // 
            this.txtTimeRate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtTimeRate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTimeRate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtTimeRate.Location = new System.Drawing.Point(71, 65);
            this.txtTimeRate.Name = "txtTimeRate";
            this.txtTimeRate.Size = new System.Drawing.Size(47, 20);
            this.txtTimeRate.TabIndex = 4;
            this.txtTimeRate.TextChanged += new System.EventHandler(this.txtTimeRate_TextChanged);
            // 
            // lblRate
            // 
            this.lblRate.AutoSize = true;
            this.lblRate.Location = new System.Drawing.Point(9, 68);
            this.lblRate.Name = "lblRate";
            this.lblRate.Size = new System.Drawing.Size(59, 13);
            this.lblRate.TabIndex = 3;
            this.lblRate.Text = "Time Rate:";
            // 
            // lblIntervals
            // 
            this.lblIntervals.AutoSize = true;
            this.lblIntervals.Location = new System.Drawing.Point(6, 20);
            this.lblIntervals.Name = "lblIntervals";
            this.lblIntervals.Size = new System.Drawing.Size(50, 13);
            this.lblIntervals.TabIndex = 2;
            this.lblIntervals.Text = "Intervals:";
            // 
            // cmbIntervals
            // 
            this.cmbIntervals.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbIntervals.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbIntervals.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbIntervals.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbIntervals.DrawDropdownHoverOutline = false;
            this.cmbIntervals.DrawFocusRectangle = false;
            this.cmbIntervals.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbIntervals.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIntervals.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbIntervals.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbIntervals.FormattingEnabled = true;
            this.cmbIntervals.Items.AddRange(new object[] {
            "24 hours",
            "12 hours",
            "8 hours",
            "6 hours",
            "4 hours",
            "3 hours",
            "2 hours",
            "1 hour",
            "45 minutes",
            "30 minutes",
            "15 minutes",
            "10 minutes"});
            this.cmbIntervals.Location = new System.Drawing.Point(62, 17);
            this.cmbIntervals.Name = "cmbIntervals";
            this.cmbIntervals.Size = new System.Drawing.Size(109, 21);
            this.cmbIntervals.TabIndex = 1;
            this.cmbIntervals.Text = "24 hours";
            this.cmbIntervals.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbIntervals.SelectedIndexChanged += new System.EventHandler(this.cmbIntervals_SelectedIndexChanged);
            // 
            // chkSync
            // 
            this.chkSync.AutoSize = true;
            this.chkSync.Location = new System.Drawing.Point(9, 44);
            this.chkSync.Name = "chkSync";
            this.chkSync.Size = new System.Drawing.Size(109, 17);
            this.chkSync.TabIndex = 0;
            this.chkSync.Text = "Sync With Server";
            this.chkSync.CheckedChanged += new System.EventHandler(this.chkSync_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(403, 310);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(114, 27);
            this.btnCancel.TabIndex = 51;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(283, 310);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(5);
            this.btnSave.Size = new System.Drawing.Size(114, 27);
            this.btnSave.TabIndex = 50;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // FrmTime
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.ClientSize = new System.Drawing.Size(529, 347);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.grpSettings);
            this.Controls.Add(this.grpRangeOptions);
            this.Controls.Add(this.lblTimes);
            this.Controls.Add(this.lstTimes);
            this.DoubleBuffered = true;
            this.Name = "FrmTime";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Time Editor (Day/Night Settings)";
            this.grpRangeOptions.ResumeLayout(false);
            this.grpRangeOptions.PerformLayout();
            this.grpSettings.ResumeLayout(false);
            this.grpSettings.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstTimes;
        private System.Windows.Forms.Label lblTimes;
        private DarkGroupBox grpRangeOptions;
        private DarkGroupBox grpSettings;
        private System.Windows.Forms.Label lblRate;
        private System.Windows.Forms.Label lblIntervals;
        private DarkComboBox cmbIntervals;
        private DarkCheckBox chkSync;
        private System.Windows.Forms.Label lblRateDesc;
        private System.Windows.Forms.Label lblRateSuffix;
        private DarkTextBox txtTimeRate;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private DarkScrollBar scrlAlpha;
        private System.Windows.Forms.Label lblBrightness;
        private System.Windows.Forms.Label lblColorDesc;
        private System.Windows.Forms.Panel pnlColor;
        private System.Windows.Forms.ColorDialog clrSelector;
    }
}