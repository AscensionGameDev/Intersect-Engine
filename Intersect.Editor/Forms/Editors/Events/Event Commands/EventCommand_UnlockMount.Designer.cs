using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    partial class EventCommandUnlockMount
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
            this.grpMount = new DarkUI.Controls.DarkGroupBox();
            this.cmbMount = new DarkUI.Controls.DarkComboBox();
            this.lblMount = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.grpMount.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpMount
            // 
            this.grpMount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpMount.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpMount.Controls.Add(this.cmbMount);
            this.grpMount.Controls.Add(this.lblMount);
            this.grpMount.Controls.Add(this.btnCancel);
            this.grpMount.Controls.Add(this.btnSave);
            this.grpMount.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpMount.Location = new System.Drawing.Point(3, 3);
            this.grpMount.Name = "grpMount";
            this.grpMount.Size = new System.Drawing.Size(176, 126);
            this.grpMount.TabIndex = 17;
            this.grpMount.TabStop = false;
            this.grpMount.Text = "Unlock Mount";
            // 
            // cmbMount
            // 
            this.cmbMount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbMount.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbMount.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbMount.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbMount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMount.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbMount.FormattingEnabled = true;
            this.cmbMount.Location = new System.Drawing.Point(47, 19);
            this.cmbMount.Name = "cmbMount";
            this.cmbMount.Size = new System.Drawing.Size(117, 21);
            this.cmbMount.TabIndex = 22;
            // 
            // lblMount
            // 
            this.lblMount.AutoSize = true;
            this.lblMount.Location = new System.Drawing.Point(4, 22);
            this.lblMount.Name = "lblMount";
            this.lblMount.Size = new System.Drawing.Size(35, 13);
            this.lblMount.TabIndex = 21;
            this.lblMount.Text = "Pet:";
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
            this.Controls.Add(this.grpMount);
            this.Name = "EventCommandUnlockMount";
            this.Size = new System.Drawing.Size(182, 132);
            this.grpMount.ResumeLayout(false);
            this.grpMount.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpMount;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private System.Windows.Forms.Label lblMount;
        private DarkComboBox cmbMount;
    }
}
