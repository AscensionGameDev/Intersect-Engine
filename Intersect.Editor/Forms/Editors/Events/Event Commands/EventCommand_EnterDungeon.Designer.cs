using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    partial class EventCommandEnterDungeon
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
            this.grpDungeon = new DarkUI.Controls.DarkGroupBox();
            this.cmbDungeon = new DarkUI.Controls.DarkComboBox();
            this.lblDungeon = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.grpDungeon.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpDungeon
            // 
            this.grpDungeon.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpDungeon.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpDungeon.Controls.Add(this.cmbDungeon);
            this.grpDungeon.Controls.Add(this.lblDungeon);
            this.grpDungeon.Controls.Add(this.btnCancel);
            this.grpDungeon.Controls.Add(this.btnSave);
            this.grpDungeon.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpDungeon.Location = new System.Drawing.Point(3, 3);
            this.grpDungeon.Name = "grpDungeon";
            this.grpDungeon.Size = new System.Drawing.Size(176, 126);
            this.grpDungeon.TabIndex = 17;
            this.grpDungeon.TabStop = false;
            this.grpDungeon.Text = "Enter Dungeon";
            // 
            // cmbDungeon
            // 
            this.cmbDungeon.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbDungeon.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbDungeon.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbDungeon.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbDungeon.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDungeon.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbDungeon.FormattingEnabled = true;
            this.cmbDungeon.Location = new System.Drawing.Point(47, 19);
            this.cmbDungeon.Name = "cmbDungeon";
            this.cmbDungeon.Size = new System.Drawing.Size(117, 21);
            this.cmbDungeon.TabIndex = 22;
            // 
            // lblDungeon
            // 
            this.lblDungeon.AutoSize = true;
            this.lblDungeon.Location = new System.Drawing.Point(4, 22);
            this.lblDungeon.Name = "lblDungeon";
            this.lblDungeon.Size = new System.Drawing.Size(35, 13);
            this.lblDungeon.TabIndex = 21;
            this.lblDungeon.Text = "Dungeon:";
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
            // EventCommand_EnterDungeon
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpDungeon);
            this.Name = "EventCommandEnterDungeon";
            this.Size = new System.Drawing.Size(182, 132);
            this.grpDungeon.ResumeLayout(false);
            this.grpDungeon.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpDungeon;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private System.Windows.Forms.Label lblDungeon;
        private DarkComboBox cmbDungeon;
    }
}
