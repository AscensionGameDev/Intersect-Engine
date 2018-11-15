using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    partial class EventCommandPlayBgm
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
            this.grpBGM = new DarkUI.Controls.DarkGroupBox();
            this.cmbBgm = new DarkUI.Controls.DarkComboBox();
            this.lblBGM = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.grpBGM.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpBGM
            // 
            this.grpBGM.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpBGM.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpBGM.Controls.Add(this.cmbBgm);
            this.grpBGM.Controls.Add(this.lblBGM);
            this.grpBGM.Controls.Add(this.btnCancel);
            this.grpBGM.Controls.Add(this.btnSave);
            this.grpBGM.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpBGM.Location = new System.Drawing.Point(3, 3);
            this.grpBGM.Name = "grpBGM";
            this.grpBGM.Size = new System.Drawing.Size(176, 126);
            this.grpBGM.TabIndex = 17;
            this.grpBGM.TabStop = false;
            this.grpBGM.Text = "Play BGM";
            // 
            // cmbBgm
            // 
            this.cmbBgm.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbBgm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbBgm.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbBgm.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbBgm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBgm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbBgm.FormattingEnabled = true;
            this.cmbBgm.Location = new System.Drawing.Point(47, 19);
            this.cmbBgm.Name = "cmbBgm";
            this.cmbBgm.Size = new System.Drawing.Size(117, 21);
            this.cmbBgm.TabIndex = 22;
            this.cmbBgm.SelectedIndexChanged += new System.EventHandler(this.cmbSprite_SelectedIndexChanged);
            // 
            // lblBGM
            // 
            this.lblBGM.AutoSize = true;
            this.lblBGM.Location = new System.Drawing.Point(4, 22);
            this.lblBGM.Name = "lblBGM";
            this.lblBGM.Size = new System.Drawing.Size(31, 13);
            this.lblBGM.TabIndex = 21;
            this.lblBGM.Text = "BGM";
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
            // EventCommand_PlayBgm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpBGM);
            this.Name = "EventCommandPlayBgm";
            this.Size = new System.Drawing.Size(182, 132);
            this.grpBGM.ResumeLayout(false);
            this.grpBGM.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpBGM;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private System.Windows.Forms.Label lblBGM;
        private DarkComboBox cmbBgm;
    }
}
