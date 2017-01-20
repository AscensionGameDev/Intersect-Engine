using DarkUI.Controls;

namespace Intersect_Editor.Forms.Editors
{
    partial class frmTime
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTime));
            this.lstTimes = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.grpRangeOptions = new DarkGroupBox();
            this.scrlAlpha = new System.Windows.Forms.HScrollBar();
            this.lblBrightness = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.pnlColor = new System.Windows.Forms.Panel();
            this.groupBox3 = new DarkGroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtTimeRate = new DarkTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbIntervals = new DarkComboBox();
            this.chkSync = new DarkCheckBox();
            this.btnCancel = new DarkButton();
            this.btnSave = new DarkButton();
            this.clrSelector = new System.Windows.Forms.ColorDialog();
            this.grpRangeOptions.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstTimes
            // 
            this.lstTimes.FormattingEnabled = true;
            this.lstTimes.Location = new System.Drawing.Point(14, 25);
            this.lstTimes.Name = "lstTimes";
            this.lstTimes.Size = new System.Drawing.Size(221, 277);
            this.lstTimes.TabIndex = 0;
            this.lstTimes.SelectedIndexChanged += new System.EventHandler(this.lstTimes_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Time of Day";
            // 
            // grpRangeOptions
            // 
            this.grpRangeOptions.Controls.Add(this.scrlAlpha);
            this.grpRangeOptions.Controls.Add(this.lblBrightness);
            this.grpRangeOptions.Controls.Add(this.label6);
            this.grpRangeOptions.Controls.Add(this.pnlColor);
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
            this.scrlAlpha.LargeChange = 1;
            this.scrlAlpha.Location = new System.Drawing.Point(141, 26);
            this.scrlAlpha.Name = "scrlAlpha";
            this.scrlAlpha.Size = new System.Drawing.Size(118, 17);
            this.scrlAlpha.TabIndex = 3;
            this.scrlAlpha.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlAlpha_Scroll);
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
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(9, 59);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(253, 30);
            this.label6.TabIndex = 1;
            this.label6.Text = "Double click the panel above to change the overlay color.";
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
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.txtTimeRate);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.cmbIntervals);
            this.groupBox3.Controls.Add(this.chkSync);
            this.groupBox3.Location = new System.Drawing.Point(249, 25);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(268, 175);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Time Settings";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(9, 100);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(218, 67);
            this.label5.TabIndex = 6;
            this.label5.Text = "Enter 1 for normal rate of time.\r\nValues larger than one for faster days. \r\nValue" +
    "s between 0 and 1 for longer days. \r\nNegative values for time to flow backwards." +
    "";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(120, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "x Normal";
            // 
            // txtTimeRate
            // 
            this.txtTimeRate.Location = new System.Drawing.Point(71, 65);
            this.txtTimeRate.Name = "txtTimeRate";
            this.txtTimeRate.Size = new System.Drawing.Size(47, 20);
            this.txtTimeRate.TabIndex = 4;
            this.txtTimeRate.TextChanged += new System.EventHandler(this.txtTimeRate_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Time Rate:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Intervals:";
            // 
            // cmbIntervals
            // 
            this.cmbIntervals.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
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
            this.btnCancel.Size = new System.Drawing.Size(114, 27);
            this.btnCancel.TabIndex = 51;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(283, 310);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(114, 27);
            this.btnSave.TabIndex = 50;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // frmTime
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(529, 347);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.grpRangeOptions);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lstTimes);
            this.DoubleBuffered = true;
            this.Name = "frmTime";
            this.Text = "Time Editor (Day/Night Settings)";
            this.grpRangeOptions.ResumeLayout(false);
            this.grpRangeOptions.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstTimes;
        private System.Windows.Forms.Label label1;
        private DarkGroupBox grpRangeOptions;
        private DarkGroupBox groupBox3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private DarkComboBox cmbIntervals;
        private DarkCheckBox chkSync;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private DarkTextBox txtTimeRate;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private System.Windows.Forms.HScrollBar scrlAlpha;
        private System.Windows.Forms.Label lblBrightness;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel pnlColor;
        private System.Windows.Forms.ColorDialog clrSelector;
    }
}