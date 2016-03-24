namespace Intersect_Editor.Forms.Editors
{
    partial class frmCommonEvent
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCommonEvent));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lstCommonEvents = new System.Windows.Forms.ListBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lstCommonEvents);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(203, 489);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Common Events";
            // 
            // lstCommonEvents
            // 
            this.lstCommonEvents.FormattingEnabled = true;
            this.lstCommonEvents.Location = new System.Drawing.Point(6, 19);
            this.lstCommonEvents.Name = "lstCommonEvents";
            this.lstCommonEvents.Size = new System.Drawing.Size(191, 459);
            this.lstCommonEvents.TabIndex = 1;
            this.lstCommonEvents.DoubleClick += new System.EventHandler(this.lstCommonEvents_DoubleClick);
            // 
            // frmCommonEvent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(226, 506);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmCommonEvent";
            this.Text = "Common Event Editor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmCommonEvent_FormClosed);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox lstCommonEvents;
    }
}