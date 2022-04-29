using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    partial class EventCommandOpenShop
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
            this.grpShop = new DarkUI.Controls.DarkGroupBox();
            this.cmbShop = new DarkUI.Controls.DarkComboBox();
            this.lblShop = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.grpShop.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpShop
            // 
            this.grpShop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpShop.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpShop.Controls.Add(this.cmbShop);
            this.grpShop.Controls.Add(this.lblShop);
            this.grpShop.Controls.Add(this.btnCancel);
            this.grpShop.Controls.Add(this.btnSave);
            this.grpShop.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpShop.Location = new System.Drawing.Point(3, 3);
            this.grpShop.Name = "grpShop";
            this.grpShop.Size = new System.Drawing.Size(176, 126);
            this.grpShop.TabIndex = 17;
            this.grpShop.TabStop = false;
            this.grpShop.Text = "Open Shop";
            // 
            // cmbShop
            // 
            this.cmbShop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbShop.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbShop.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbShop.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbShop.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbShop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbShop.FormattingEnabled = true;
            this.cmbShop.Location = new System.Drawing.Point(47, 19);
            this.cmbShop.Name = "cmbShop";
            this.cmbShop.Size = new System.Drawing.Size(117, 21);
            this.cmbShop.TabIndex = 22;
            // 
            // lblShop
            // 
            this.lblShop.AutoSize = true;
            this.lblShop.Location = new System.Drawing.Point(4, 22);
            this.lblShop.Name = "lblShop";
            this.lblShop.Size = new System.Drawing.Size(35, 13);
            this.lblShop.TabIndex = 21;
            this.lblShop.Text = "Shop:";
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
            // EventCommand_OpenShop
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpShop);
            this.Name = "EventCommandOpenShop";
            this.Size = new System.Drawing.Size(182, 132);
            this.grpShop.ResumeLayout(false);
            this.grpShop.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpShop;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private System.Windows.Forms.Label lblShop;
        private DarkComboBox cmbShop;
    }
}
