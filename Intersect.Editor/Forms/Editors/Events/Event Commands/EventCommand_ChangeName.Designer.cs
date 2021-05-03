using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    partial class EventCommandChangeName

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EventCommandChangeName));
            this.grpChangeLevel = new DarkUI.Controls.DarkGroupBox();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.lblVariable = new System.Windows.Forms.Label();
            this.cmbVariable = new DarkUI.Controls.DarkComboBox();
            this.grpChangeLevel.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpChangeLevel
            // 
            this.grpChangeLevel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpChangeLevel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpChangeLevel.Controls.Add(this.lblVariable);
            this.grpChangeLevel.Controls.Add(this.cmbVariable);
            this.grpChangeLevel.Controls.Add(this.btnCancel);
            this.grpChangeLevel.Controls.Add(this.btnSave);
            this.grpChangeLevel.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpChangeLevel.Location = new System.Drawing.Point(3, 3);
            this.grpChangeLevel.Name = "grpChangeLevel";
            this.grpChangeLevel.Size = new System.Drawing.Size(188, 133);
            this.grpChangeLevel.TabIndex = 17;
            this.grpChangeLevel.TabStop = false;
            this.grpChangeLevel.Text = "Change Name:";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(102, 100);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(15, 100);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(5);
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Ok";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblVariable
            // 
            this.lblVariable.AutoSize = true;
            this.lblVariable.Location = new System.Drawing.Point(6, 26);
            this.lblVariable.Name = "lblVariable";
            this.lblVariable.Size = new System.Drawing.Size(80, 13);
            this.lblVariable.TabIndex = 57;
            this.lblVariable.Text = "Player Variable:";
            // 
            // cmbVariable
            // 
            this.cmbVariable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbVariable.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbVariable.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbVariable.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbVariable.DrawDropdownHoverOutline = false;
            this.cmbVariable.DrawFocusRectangle = false;
            this.cmbVariable.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbVariable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVariable.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbVariable.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbVariable.FormattingEnabled = true;
            this.cmbVariable.Location = new System.Drawing.Point(9, 42);
            this.cmbVariable.Name = "cmbVariable";
            this.cmbVariable.Size = new System.Drawing.Size(173, 21);
            this.cmbVariable.TabIndex = 56;
            this.cmbVariable.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // EventCommandChangeName
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpChangeLevel);
            this.Name = "EventCommandChangeName";
            this.Size = new System.Drawing.Size(197, 144);
            this.grpChangeLevel.ResumeLayout(false);
            this.grpChangeLevel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpChangeLevel;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.Label lblVariable;
        internal DarkComboBox cmbVariable;
    }
}
