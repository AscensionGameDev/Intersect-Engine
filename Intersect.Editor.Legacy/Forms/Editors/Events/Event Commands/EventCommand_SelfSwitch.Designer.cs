using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    partial class EventCommandSelfSwitch
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
            this.grpSelfSwitch = new DarkUI.Controls.DarkGroupBox();
            this.cmbSetSwitchVal = new DarkUI.Controls.DarkComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.cmbSetSwitch = new DarkUI.Controls.DarkComboBox();
            this.lblSelfSwitch = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.grpSelfSwitch.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpSelfSwitch
            // 
            this.grpSelfSwitch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpSelfSwitch.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpSelfSwitch.Controls.Add(this.cmbSetSwitchVal);
            this.grpSelfSwitch.Controls.Add(this.label15);
            this.grpSelfSwitch.Controls.Add(this.cmbSetSwitch);
            this.grpSelfSwitch.Controls.Add(this.lblSelfSwitch);
            this.grpSelfSwitch.Controls.Add(this.btnCancel);
            this.grpSelfSwitch.Controls.Add(this.btnSave);
            this.grpSelfSwitch.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpSelfSwitch.Location = new System.Drawing.Point(3, 3);
            this.grpSelfSwitch.Name = "grpSelfSwitch";
            this.grpSelfSwitch.Size = new System.Drawing.Size(292, 82);
            this.grpSelfSwitch.TabIndex = 17;
            this.grpSelfSwitch.TabStop = false;
            this.grpSelfSwitch.Text = "Set Self Switch";
            // 
            // cmbSetSwitchVal
            // 
            this.cmbSetSwitchVal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbSetSwitchVal.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbSetSwitchVal.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbSetSwitchVal.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbSetSwitchVal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSetSwitchVal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbSetSwitchVal.FormattingEnabled = true;
            this.cmbSetSwitchVal.Location = new System.Drawing.Point(205, 19);
            this.cmbSetSwitchVal.Name = "cmbSetSwitchVal";
            this.cmbSetSwitchVal.Size = new System.Drawing.Size(71, 21);
            this.cmbSetSwitchVal.TabIndex = 24;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(185, 22);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(16, 13);
            this.label15.TabIndex = 23;
            this.label15.Text = "to";
            // 
            // cmbSetSwitch
            // 
            this.cmbSetSwitch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbSetSwitch.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbSetSwitch.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbSetSwitch.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbSetSwitch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSetSwitch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbSetSwitch.FormattingEnabled = true;
            this.cmbSetSwitch.Location = new System.Drawing.Point(89, 19);
            this.cmbSetSwitch.Name = "cmbSetSwitch";
            this.cmbSetSwitch.Size = new System.Drawing.Size(90, 21);
            this.cmbSetSwitch.TabIndex = 22;
            // 
            // lblSelfSwitch
            // 
            this.lblSelfSwitch.AutoSize = true;
            this.lblSelfSwitch.Location = new System.Drawing.Point(5, 21);
            this.lblSelfSwitch.Name = "lblSelfSwitch";
            this.lblSelfSwitch.Size = new System.Drawing.Size(82, 13);
            this.lblSelfSwitch.TabIndex = 21;
            this.lblSelfSwitch.Text = "Set Self Switch:";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(89, 53);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(8, 53);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(5);
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Ok";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // EventCommand_SelfSwitch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpSelfSwitch);
            this.Name = "EventCommandSelfSwitch";
            this.Size = new System.Drawing.Size(298, 88);
            this.grpSelfSwitch.ResumeLayout(false);
            this.grpSelfSwitch.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpSelfSwitch;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private DarkComboBox cmbSetSwitchVal;
        private System.Windows.Forms.Label label15;
        private DarkComboBox cmbSetSwitch;
        private System.Windows.Forms.Label lblSelfSwitch;
    }
}
