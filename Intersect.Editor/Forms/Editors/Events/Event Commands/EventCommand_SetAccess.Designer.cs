using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    partial class EventCommandSetAccess
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
            this.grpSetAccess = new DarkUI.Controls.DarkGroupBox();
            this.cmbAccess = new DarkUI.Controls.DarkComboBox();
            this.lblAccess = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.grpSetAccess.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpSetAccess
            // 
            this.grpSetAccess.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpSetAccess.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpSetAccess.Controls.Add(this.cmbAccess);
            this.grpSetAccess.Controls.Add(this.lblAccess);
            this.grpSetAccess.Controls.Add(this.btnCancel);
            this.grpSetAccess.Controls.Add(this.btnSave);
            this.grpSetAccess.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpSetAccess.Location = new System.Drawing.Point(3, 3);
            this.grpSetAccess.Name = "grpSetAccess";
            this.grpSetAccess.Size = new System.Drawing.Size(282, 95);
            this.grpSetAccess.TabIndex = 17;
            this.grpSetAccess.TabStop = false;
            this.grpSetAccess.Text = "Set Access:";
            // 
            // cmbAccess
            // 
            this.cmbAccess.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbAccess.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbAccess.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbAccess.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbAccess.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAccess.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbAccess.FormattingEnabled = true;
            this.cmbAccess.Location = new System.Drawing.Point(47, 19);
            this.cmbAccess.Name = "cmbAccess";
            this.cmbAccess.Size = new System.Drawing.Size(229, 21);
            this.cmbAccess.TabIndex = 22;
            // 
            // lblAccess
            // 
            this.lblAccess.AutoSize = true;
            this.lblAccess.Location = new System.Drawing.Point(4, 22);
            this.lblAccess.Name = "lblAccess";
            this.lblAccess.Size = new System.Drawing.Size(45, 13);
            this.lblAccess.TabIndex = 21;
            this.lblAccess.Text = "Access:";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(89, 66);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(7, 66);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(5);
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Ok";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // EventCommand_SetAccess
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpSetAccess);
            this.Name = "EventCommandSetAccess";
            this.Size = new System.Drawing.Size(288, 101);
            this.grpSetAccess.ResumeLayout(false);
            this.grpSetAccess.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpSetAccess;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private System.Windows.Forms.Label lblAccess;
        private DarkComboBox cmbAccess;
    }
}
