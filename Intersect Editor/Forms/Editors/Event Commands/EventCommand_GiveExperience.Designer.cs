using DarkUI.Controls;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    partial class EventCommand_GiveExperience
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
            this.grpGiveExperience = new DarkUI.Controls.DarkGroupBox();
            this.scrlExperience = new DarkUI.Controls.DarkScrollBar();
            this.lblExperience = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.grpGiveExperience.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpGiveExperience
            // 
            this.grpGiveExperience.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpGiveExperience.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpGiveExperience.Controls.Add(this.scrlExperience);
            this.grpGiveExperience.Controls.Add(this.lblExperience);
            this.grpGiveExperience.Controls.Add(this.btnCancel);
            this.grpGiveExperience.Controls.Add(this.btnSave);
            this.grpGiveExperience.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpGiveExperience.Location = new System.Drawing.Point(3, 3);
            this.grpGiveExperience.Name = "grpGiveExperience";
            this.grpGiveExperience.Size = new System.Drawing.Size(259, 79);
            this.grpGiveExperience.TabIndex = 17;
            this.grpGiveExperience.TabStop = false;
            this.grpGiveExperience.Text = "Give Experience:";
            // 
            // scrlExperience
            // 
            this.scrlExperience.Location = new System.Drawing.Point(107, 22);
            this.scrlExperience.Maximum = 32000;
            this.scrlExperience.Name = "scrlExperience";
            this.scrlExperience.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlExperience.Size = new System.Drawing.Size(149, 17);
            this.scrlExperience.TabIndex = 22;
            this.scrlExperience.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlExperience_Scroll);
            // 
            // lblExperience
            // 
            this.lblExperience.AutoSize = true;
            this.lblExperience.Location = new System.Drawing.Point(4, 22);
            this.lblExperience.Name = "lblExperience";
            this.lblExperience.Size = new System.Drawing.Size(100, 13);
            this.lblExperience.TabIndex = 21;
            this.lblExperience.Text = "Give 0 Experience: ";
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
            // EventCommand_GiveExperience
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpGiveExperience);
            this.Name = "EventCommand_GiveExperience";
            this.Size = new System.Drawing.Size(268, 88);
            this.grpGiveExperience.ResumeLayout(false);
            this.grpGiveExperience.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpGiveExperience;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private System.Windows.Forms.Label lblExperience;
        private DarkScrollBar scrlExperience;
    }
}
