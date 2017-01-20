using DarkUI.Controls;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    partial class EventCommand_ChangeLevel
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
            this.grpChangeLevel = new DarkUI.Controls.DarkGroupBox();
            this.scrlLevel = new DarkUI.Controls.DarkScrollBar();
            this.lblLevel = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.grpChangeLevel.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpChangeLevel
            // 
            this.grpChangeLevel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpChangeLevel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpChangeLevel.Controls.Add(this.scrlLevel);
            this.grpChangeLevel.Controls.Add(this.lblLevel);
            this.grpChangeLevel.Controls.Add(this.btnCancel);
            this.grpChangeLevel.Controls.Add(this.btnSave);
            this.grpChangeLevel.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpChangeLevel.Location = new System.Drawing.Point(3, 3);
            this.grpChangeLevel.Name = "grpChangeLevel";
            this.grpChangeLevel.Size = new System.Drawing.Size(259, 79);
            this.grpChangeLevel.TabIndex = 17;
            this.grpChangeLevel.TabStop = false;
            this.grpChangeLevel.Text = "Change Level:";
            // 
            // scrlLevel
            // 
            this.scrlLevel.Location = new System.Drawing.Point(107, 22);
            this.scrlLevel.Maximum = 255;
            this.scrlLevel.Minimum = 1;
            this.scrlLevel.Name = "scrlLevel";
            this.scrlLevel.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlLevel.Size = new System.Drawing.Size(149, 17);
            this.scrlLevel.TabIndex = 22;
            this.scrlLevel.Value = 1;
            this.scrlLevel.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlExperience_Scroll);
            // 
            // lblLevel
            // 
            this.lblLevel.AutoSize = true;
            this.lblLevel.Location = new System.Drawing.Point(4, 22);
            this.lblLevel.Name = "lblLevel";
            this.lblLevel.Size = new System.Drawing.Size(64, 13);
            this.lblLevel.TabIndex = 21;
            this.lblLevel.Text = "Set Level: 1";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(89, 47);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(7, 47);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(5);
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Ok";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // EventCommand_ChangeLevel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpChangeLevel);
            this.Name = "EventCommand_ChangeLevel";
            this.Size = new System.Drawing.Size(268, 88);
            this.grpChangeLevel.ResumeLayout(false);
            this.grpChangeLevel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpChangeLevel;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private System.Windows.Forms.Label lblLevel;
        private DarkScrollBar scrlLevel;
    }
}
