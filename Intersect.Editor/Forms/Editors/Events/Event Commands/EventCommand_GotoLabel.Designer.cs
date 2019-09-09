using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    partial class EventCommandGotoLabel
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
            this.grpGotoLabel = new DarkUI.Controls.DarkGroupBox();
            this.txtGotoLabel = new DarkUI.Controls.DarkTextBox();
            this.lblGotoLabel = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.grpGotoLabel.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpGotoLabel
            // 
            this.grpGotoLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpGotoLabel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpGotoLabel.Controls.Add(this.txtGotoLabel);
            this.grpGotoLabel.Controls.Add(this.lblGotoLabel);
            this.grpGotoLabel.Controls.Add(this.btnCancel);
            this.grpGotoLabel.Controls.Add(this.btnSave);
            this.grpGotoLabel.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpGotoLabel.Location = new System.Drawing.Point(3, 3);
            this.grpGotoLabel.Name = "grpGotoLabel";
            this.grpGotoLabel.Size = new System.Drawing.Size(259, 79);
            this.grpGotoLabel.TabIndex = 17;
            this.grpGotoLabel.TabStop = false;
            this.grpGotoLabel.Text = "Go to Label:";
            // 
            // txtGotoLabel
            // 
            this.txtGotoLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtGotoLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtGotoLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtGotoLabel.Location = new System.Drawing.Point(72, 19);
            this.txtGotoLabel.Name = "txtGotoLabel";
            this.txtGotoLabel.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.txtGotoLabel.Size = new System.Drawing.Size(175, 20);
            this.txtGotoLabel.TabIndex = 22;
            // 
            // lblGotoLabel
            // 
            this.lblGotoLabel.AutoSize = true;
            this.lblGotoLabel.Location = new System.Drawing.Point(4, 22);
            this.lblGotoLabel.Name = "lblGotoLabel";
            this.lblGotoLabel.Size = new System.Drawing.Size(65, 13);
            this.lblGotoLabel.TabIndex = 21;
            this.lblGotoLabel.Text = "Go to Label:";
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
            // EventCommandGotoLabel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpGotoLabel);
            this.Name = "EventCommandGotoLabel";
            this.Size = new System.Drawing.Size(268, 88);
            this.grpGotoLabel.ResumeLayout(false);
            this.grpGotoLabel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpGotoLabel;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private DarkTextBox txtGotoLabel;
        private System.Windows.Forms.Label lblGotoLabel;
    }
}
