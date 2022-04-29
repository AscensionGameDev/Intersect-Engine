using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    partial class EventCommandLabel
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
            this.grpLabel = new DarkUI.Controls.DarkGroupBox();
            this.txtLabel = new DarkUI.Controls.DarkTextBox();
            this.lblLabel = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.grpLabel.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpLabel
            // 
            this.grpLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpLabel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpLabel.Controls.Add(this.txtLabel);
            this.grpLabel.Controls.Add(this.lblLabel);
            this.grpLabel.Controls.Add(this.btnCancel);
            this.grpLabel.Controls.Add(this.btnSave);
            this.grpLabel.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpLabel.Location = new System.Drawing.Point(3, 3);
            this.grpLabel.Name = "grpLabel";
            this.grpLabel.Size = new System.Drawing.Size(259, 79);
            this.grpLabel.TabIndex = 17;
            this.grpLabel.TabStop = false;
            this.grpLabel.Text = "Label:";
            // 
            // txtLabel
            // 
            this.txtLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtLabel.Location = new System.Drawing.Point(46, 19);
            this.txtLabel.Name = "txtLabel";
            this.txtLabel.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.txtLabel.Size = new System.Drawing.Size(195, 20);
            this.txtLabel.TabIndex = 22;
            // 
            // lblLabel
            // 
            this.lblLabel.AutoSize = true;
            this.lblLabel.Location = new System.Drawing.Point(4, 22);
            this.lblLabel.Name = "lblLabel";
            this.lblLabel.Size = new System.Drawing.Size(36, 13);
            this.lblLabel.TabIndex = 21;
            this.lblLabel.Text = "Label:";
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
            // EventCommandLabel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpLabel);
            this.Name = "EventCommandLabel";
            this.Size = new System.Drawing.Size(268, 88);
            this.grpLabel.ResumeLayout(false);
            this.grpLabel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpLabel;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private DarkTextBox txtLabel;
        private System.Windows.Forms.Label lblLabel;
    }
}
