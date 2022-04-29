using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    partial class EventCommandPlayBgs
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
            this.grpPlayBGS = new DarkUI.Controls.DarkGroupBox();
            this.cmbSound = new DarkUI.Controls.DarkComboBox();
            this.lblSound = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.grpPlayBGS.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpPlayBGS
            // 
            this.grpPlayBGS.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpPlayBGS.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpPlayBGS.Controls.Add(this.cmbSound);
            this.grpPlayBGS.Controls.Add(this.lblSound);
            this.grpPlayBGS.Controls.Add(this.btnCancel);
            this.grpPlayBGS.Controls.Add(this.btnSave);
            this.grpPlayBGS.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpPlayBGS.Location = new System.Drawing.Point(3, 3);
            this.grpPlayBGS.Name = "grpPlayBGS";
            this.grpPlayBGS.Size = new System.Drawing.Size(176, 126);
            this.grpPlayBGS.TabIndex = 17;
            this.grpPlayBGS.TabStop = false;
            this.grpPlayBGS.Text = "Play Sound";
            // 
            // cmbSound
            // 
            this.cmbSound.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbSound.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbSound.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbSound.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbSound.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSound.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbSound.FormattingEnabled = true;
            this.cmbSound.Location = new System.Drawing.Point(47, 19);
            this.cmbSound.Name = "cmbSound";
            this.cmbSound.Size = new System.Drawing.Size(117, 21);
            this.cmbSound.TabIndex = 22;
            this.cmbSound.SelectedIndexChanged += new System.EventHandler(this.cmbSprite_SelectedIndexChanged);
            // 
            // lblSound
            // 
            this.lblSound.AutoSize = true;
            this.lblSound.Location = new System.Drawing.Point(4, 22);
            this.lblSound.Name = "lblSound";
            this.lblSound.Size = new System.Drawing.Size(41, 13);
            this.lblSound.TabIndex = 21;
            this.lblSound.Text = "Sound:";
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
            // EventCommand_PlayBgs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpPlayBGS);
            this.Name = "EventCommandPlayBgs";
            this.Size = new System.Drawing.Size(182, 132);
            this.grpPlayBGS.ResumeLayout(false);
            this.grpPlayBGS.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpPlayBGS;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private System.Windows.Forms.Label lblSound;
        private DarkComboBox cmbSound;
    }
}
