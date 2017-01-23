using DarkUI.Controls;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    partial class EventCommand_OpenBench
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
            this.grpBench = new DarkUI.Controls.DarkGroupBox();
            this.cmbbench = new DarkUI.Controls.DarkComboBox();
            this.lblBench = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.grpBench.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpBench
            // 
            this.grpBench.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpBench.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpBench.Controls.Add(this.cmbbench);
            this.grpBench.Controls.Add(this.lblBench);
            this.grpBench.Controls.Add(this.btnCancel);
            this.grpBench.Controls.Add(this.btnSave);
            this.grpBench.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpBench.Location = new System.Drawing.Point(3, 3);
            this.grpBench.Name = "grpBench";
            this.grpBench.Size = new System.Drawing.Size(176, 126);
            this.grpBench.TabIndex = 17;
            this.grpBench.TabStop = false;
            this.grpBench.Text = "Open Bench";
            // 
            // cmbbench
            // 
            this.cmbbench.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbbench.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbbench.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbbench.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbbench.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbbench.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbbench.FormattingEnabled = true;
            this.cmbbench.Location = new System.Drawing.Point(47, 19);
            this.cmbbench.Name = "cmbbench";
            this.cmbbench.Size = new System.Drawing.Size(117, 21);
            this.cmbbench.TabIndex = 22;
            // 
            // lblBench
            // 
            this.lblBench.AutoSize = true;
            this.lblBench.Location = new System.Drawing.Point(4, 22);
            this.lblBench.Name = "lblBench";
            this.lblBench.Size = new System.Drawing.Size(41, 13);
            this.lblBench.TabIndex = 21;
            this.lblBench.Text = "Bench:";
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
            // EventCommand_OpenBench
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpBench);
            this.Name = "EventCommand_OpenBench";
            this.Size = new System.Drawing.Size(182, 132);
            this.grpBench.ResumeLayout(false);
            this.grpBench.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpBench;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private System.Windows.Forms.Label lblBench;
        private DarkComboBox cmbbench;
    }
}
