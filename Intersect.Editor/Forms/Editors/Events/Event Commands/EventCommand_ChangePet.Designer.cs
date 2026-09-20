using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    partial class EventCommandChangePet
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
            this.grpPet = new DarkUI.Controls.DarkGroupBox();
            this.cmbPet = new DarkUI.Controls.DarkComboBox();
            this.lblPet = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.grpPet.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpPet
            // 
            this.grpPet.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpPet.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpPet.Controls.Add(this.cmbPet);
            this.grpPet.Controls.Add(this.lblPet);
            this.grpPet.Controls.Add(this.btnCancel);
            this.grpPet.Controls.Add(this.btnSave);
            this.grpPet.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpPet.Location = new System.Drawing.Point(3, 3);
            this.grpPet.Name = "grpPet";
            this.grpPet.Size = new System.Drawing.Size(176, 126);
            this.grpPet.TabIndex = 17;
            this.grpPet.TabStop = false;
            this.grpPet.Text = "Change Pet";
            // 
            // cmbPet
            // 
            this.cmbPet.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbPet.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbPet.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbPet.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbPet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbPet.FormattingEnabled = true;
            this.cmbPet.Location = new System.Drawing.Point(47, 19);
            this.cmbPet.Name = "cmbPet";
            this.cmbPet.Size = new System.Drawing.Size(117, 21);
            this.cmbPet.TabIndex = 22;
            // 
            // lblPet
            // 
            this.lblPet.AutoSize = true;
            this.lblPet.Location = new System.Drawing.Point(4, 22);
            this.lblPet.Name = "lblPet";
            this.lblPet.Size = new System.Drawing.Size(35, 13);
            this.lblPet.TabIndex = 21;
            this.lblPet.Text = "Pet:";
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
            this.Controls.Add(this.grpPet);
            this.Name = "EventCommandChangePet";
            this.Size = new System.Drawing.Size(182, 132);
            this.grpPet.ResumeLayout(false);
            this.grpPet.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpPet;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private System.Windows.Forms.Label lblPet;
        private DarkComboBox cmbPet;
    }
}
