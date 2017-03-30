using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors
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
            this.grpCommonEvents = new DarkUI.Controls.DarkGroupBox();
            this.btnNew = new DarkUI.Controls.DarkButton();
            this.btnDelete = new DarkUI.Controls.DarkButton();
            this.lstCommonEvents = new System.Windows.Forms.ListBox();
            this.grpCommonEvents.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpCommonEvents
            // 
            this.grpCommonEvents.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpCommonEvents.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpCommonEvents.Controls.Add(this.btnNew);
            this.grpCommonEvents.Controls.Add(this.btnDelete);
            this.grpCommonEvents.Controls.Add(this.lstCommonEvents);
            this.grpCommonEvents.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpCommonEvents.Location = new System.Drawing.Point(12, 12);
            this.grpCommonEvents.Name = "grpCommonEvents";
            this.grpCommonEvents.Size = new System.Drawing.Size(203, 508);
            this.grpCommonEvents.TabIndex = 16;
            this.grpCommonEvents.TabStop = false;
            this.grpCommonEvents.Text = "Common Events";
            // 
            // btnNew
            // 
            this.btnNew.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnNew.Location = new System.Drawing.Point(7, 442);
            this.btnNew.Name = "btnNew";
            this.btnNew.Padding = new System.Windows.Forms.Padding(5);
            this.btnNew.Size = new System.Drawing.Size(190, 27);
            this.btnNew.TabIndex = 33;
            this.btnNew.Text = "New";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnDelete.Location = new System.Drawing.Point(6, 475);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Padding = new System.Windows.Forms.Padding(5);
            this.btnDelete.Size = new System.Drawing.Size(190, 27);
            this.btnDelete.TabIndex = 32;
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // lstCommonEvents
            // 
            this.lstCommonEvents.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.lstCommonEvents.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstCommonEvents.ForeColor = System.Drawing.Color.Gainsboro;
            this.lstCommonEvents.FormattingEnabled = true;
            this.lstCommonEvents.Location = new System.Drawing.Point(6, 19);
            this.lstCommonEvents.Name = "lstCommonEvents";
            this.lstCommonEvents.Size = new System.Drawing.Size(191, 405);
            this.lstCommonEvents.TabIndex = 1;
            this.lstCommonEvents.SelectedIndexChanged += new System.EventHandler(this.lstCommonEvents_SelectedIndexChanged);
            this.lstCommonEvents.DoubleClick += new System.EventHandler(this.lstCommonEvents_DoubleClick);
            // 
            // frmCommonEvent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.ClientSize = new System.Drawing.Size(226, 525);
            this.Controls.Add(this.grpCommonEvents);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmCommonEvent";
            this.Text = "Common Event Editor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmCommonEvent_FormClosed);
            this.grpCommonEvents.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpCommonEvents;
        private System.Windows.Forms.ListBox lstCommonEvents;
        private DarkButton btnNew;
        private DarkButton btnDelete;
    }
}