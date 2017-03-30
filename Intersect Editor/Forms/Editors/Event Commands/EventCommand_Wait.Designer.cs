using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    partial class EventCommand_Wait
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
            this.grpWait = new DarkUI.Controls.DarkGroupBox();
            this.scrlWait = new DarkUI.Controls.DarkScrollBar();
            this.lblWait = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.grpWait.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpWait
            // 
            this.grpWait.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpWait.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpWait.Controls.Add(this.scrlWait);
            this.grpWait.Controls.Add(this.lblWait);
            this.grpWait.Controls.Add(this.btnCancel);
            this.grpWait.Controls.Add(this.btnSave);
            this.grpWait.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpWait.Location = new System.Drawing.Point(3, 3);
            this.grpWait.Name = "grpWait";
            this.grpWait.Size = new System.Drawing.Size(253, 100);
            this.grpWait.TabIndex = 17;
            this.grpWait.TabStop = false;
            this.grpWait.Text = "Wait:";
            // 
            // scrlWait
            // 
            this.scrlWait.Location = new System.Drawing.Point(81, 20);
            this.scrlWait.Maximum = 30000;
            this.scrlWait.Name = "scrlWait";
            this.scrlWait.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlWait.Size = new System.Drawing.Size(166, 17);
            this.scrlWait.TabIndex = 22;
            this.scrlWait.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlWait_Scroll);
            // 
            // lblWait
            // 
            this.lblWait.AutoSize = true;
            this.lblWait.Location = new System.Drawing.Point(4, 22);
            this.lblWait.Name = "lblWait";
            this.lblWait.Size = new System.Drawing.Size(51, 13);
            this.lblWait.TabIndex = 21;
            this.lblWait.Text = "Wait 0ms";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(89, 71);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(7, 71);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(5);
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Ok";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // EventCommand_Wait
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpWait);
            this.Name = "EventCommand_Wait";
            this.Size = new System.Drawing.Size(259, 106);
            this.grpWait.ResumeLayout(false);
            this.grpWait.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpWait;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private System.Windows.Forms.Label lblWait;
        private DarkScrollBar scrlWait;
    }
}
