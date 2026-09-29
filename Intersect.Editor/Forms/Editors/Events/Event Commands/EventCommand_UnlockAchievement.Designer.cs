using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    partial class EventCommandUnlockAchievement
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
            this.grpAchievement = new DarkUI.Controls.DarkGroupBox();
            this.cmbAchievement = new DarkUI.Controls.DarkComboBox();
            this.lblAchievement = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.grpAchievement.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpAchievement
            // 
            this.grpAchievement.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpAchievement.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpAchievement.Controls.Add(this.cmbAchievement);
            this.grpAchievement.Controls.Add(this.lblAchievement);
            this.grpAchievement.Controls.Add(this.btnCancel);
            this.grpAchievement.Controls.Add(this.btnSave);
            this.grpAchievement.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpAchievement.Location = new System.Drawing.Point(3, 3);
            this.grpAchievement.Name = "grpAchievement";
            this.grpAchievement.Size = new System.Drawing.Size(176, 126);
            this.grpAchievement.TabIndex = 17;
            this.grpAchievement.TabStop = false;
            this.grpAchievement.Text = "Unlock Achievement";
            // 
            // cmbAchievement
            // 
            this.cmbAchievement.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbAchievement.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbAchievement.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbAchievement.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbAchievement.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAchievement.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbAchievement.FormattingEnabled = true;
            this.cmbAchievement.Location = new System.Drawing.Point(47, 19);
            this.cmbAchievement.Name = "cmbAchievement";
            this.cmbAchievement.Size = new System.Drawing.Size(117, 21);
            this.cmbAchievement.TabIndex = 22;
            // 
            // lblAchievement
            // 
            this.lblAchievement.AutoSize = true;
            this.lblAchievement.Location = new System.Drawing.Point(4, 22);
            this.lblAchievement.Name = "lblAchievement";
            this.lblAchievement.Size = new System.Drawing.Size(35, 13);
            this.lblAchievement.TabIndex = 21;
            this.lblAchievement.Text = "Pet:";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(89, 97);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(7, 97);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(5);
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Ok";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // EventCommand_ChangePet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpAchievement);
            this.Name = "EventCommandUnlockAchievement";
            this.Size = new System.Drawing.Size(182, 132);
            this.grpAchievement.ResumeLayout(false);
            this.grpAchievement.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpAchievement;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private System.Windows.Forms.Label lblAchievement;
        private DarkComboBox cmbAchievement;
    }
}
