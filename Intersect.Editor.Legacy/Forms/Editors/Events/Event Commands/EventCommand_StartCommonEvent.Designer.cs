using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    partial class EventCommandStartCommonEvent
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
            this.grpCommonEvent = new DarkUI.Controls.DarkGroupBox();
            this.cmbEvent = new DarkUI.Controls.DarkComboBox();
            this.lblCommonEvent = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.grpCommonEvent.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpCommonEvent
            // 
            this.grpCommonEvent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpCommonEvent.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpCommonEvent.Controls.Add(this.cmbEvent);
            this.grpCommonEvent.Controls.Add(this.lblCommonEvent);
            this.grpCommonEvent.Controls.Add(this.btnCancel);
            this.grpCommonEvent.Controls.Add(this.btnSave);
            this.grpCommonEvent.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpCommonEvent.Location = new System.Drawing.Point(3, 3);
            this.grpCommonEvent.Name = "grpCommonEvent";
            this.grpCommonEvent.Size = new System.Drawing.Size(249, 126);
            this.grpCommonEvent.TabIndex = 17;
            this.grpCommonEvent.TabStop = false;
            this.grpCommonEvent.Text = "Start Common Event";
            // 
            // cmbEvent
            // 
            this.cmbEvent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbEvent.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbEvent.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbEvent.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbEvent.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEvent.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbEvent.FormattingEnabled = true;
            this.cmbEvent.Location = new System.Drawing.Point(89, 19);
            this.cmbEvent.Name = "cmbEvent";
            this.cmbEvent.Size = new System.Drawing.Size(154, 21);
            this.cmbEvent.TabIndex = 22;
            // 
            // lblCommonEvent
            // 
            this.lblCommonEvent.AutoSize = true;
            this.lblCommonEvent.Location = new System.Drawing.Point(4, 22);
            this.lblCommonEvent.Name = "lblCommonEvent";
            this.lblCommonEvent.Size = new System.Drawing.Size(85, 13);
            this.lblCommonEvent.TabIndex = 21;
            this.lblCommonEvent.Text = "Common Event: ";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(168, 97);
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
            // EventCommand_StartCommonEvent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpCommonEvent);
            this.Name = "EventCommandStartCommonEvent";
            this.Size = new System.Drawing.Size(257, 132);
            this.grpCommonEvent.ResumeLayout(false);
            this.grpCommonEvent.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpCommonEvent;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private System.Windows.Forms.Label lblCommonEvent;
        private DarkComboBox cmbEvent;
    }
}
