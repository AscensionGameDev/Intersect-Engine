using DarkUI.Controls;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    partial class EventCommand_Switch
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
            this.grpSetSwitch = new DarkUI.Controls.DarkGroupBox();
            this.rdoGlobalSwitch = new DarkUI.Controls.DarkRadioButton();
            this.rdoPlayerSwitch = new DarkUI.Controls.DarkRadioButton();
            this.cmbSetSwitchVal = new DarkUI.Controls.DarkComboBox();
            this.lblTo = new System.Windows.Forms.Label();
            this.cmbSetSwitch = new DarkUI.Controls.DarkComboBox();
            this.lblSwitch = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.grpSetSwitch.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpSetSwitch
            // 
            this.grpSetSwitch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpSetSwitch.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpSetSwitch.Controls.Add(this.rdoGlobalSwitch);
            this.grpSetSwitch.Controls.Add(this.rdoPlayerSwitch);
            this.grpSetSwitch.Controls.Add(this.cmbSetSwitchVal);
            this.grpSetSwitch.Controls.Add(this.lblTo);
            this.grpSetSwitch.Controls.Add(this.cmbSetSwitch);
            this.grpSetSwitch.Controls.Add(this.lblSwitch);
            this.grpSetSwitch.Controls.Add(this.btnCancel);
            this.grpSetSwitch.Controls.Add(this.btnSave);
            this.grpSetSwitch.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpSetSwitch.Location = new System.Drawing.Point(3, 3);
            this.grpSetSwitch.Name = "grpSetSwitch";
            this.grpSetSwitch.Size = new System.Drawing.Size(292, 121);
            this.grpSetSwitch.TabIndex = 17;
            this.grpSetSwitch.TabStop = false;
            this.grpSetSwitch.Text = "Set Switch";
            // 
            // rdoGlobalSwitch
            // 
            this.rdoGlobalSwitch.AutoSize = true;
            this.rdoGlobalSwitch.Location = new System.Drawing.Point(103, 20);
            this.rdoGlobalSwitch.Name = "rdoGlobalSwitch";
            this.rdoGlobalSwitch.Size = new System.Drawing.Size(90, 17);
            this.rdoGlobalSwitch.TabIndex = 26;
            this.rdoGlobalSwitch.TabStop = true;
            this.rdoGlobalSwitch.Text = "Global Switch";
            this.rdoGlobalSwitch.CheckedChanged += new System.EventHandler(this.rdoGlobalSwitch_CheckedChanged);
            // 
            // rdoPlayerSwitch
            // 
            this.rdoPlayerSwitch.AutoSize = true;
            this.rdoPlayerSwitch.Checked = true;
            this.rdoPlayerSwitch.Location = new System.Drawing.Point(8, 20);
            this.rdoPlayerSwitch.Name = "rdoPlayerSwitch";
            this.rdoPlayerSwitch.Size = new System.Drawing.Size(89, 17);
            this.rdoPlayerSwitch.TabIndex = 25;
            this.rdoPlayerSwitch.TabStop = true;
            this.rdoPlayerSwitch.Text = "Player Switch";
            this.rdoPlayerSwitch.CheckedChanged += new System.EventHandler(this.rdoPlayerSwitch_CheckedChanged);
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
            this.cmbSetSwitchVal.Items.AddRange(new object[] {
            "False",
            "True"});
            this.cmbSetSwitchVal.Location = new System.Drawing.Point(205, 49);
            this.cmbSetSwitchVal.Name = "cmbSetSwitchVal";
            this.cmbSetSwitchVal.Size = new System.Drawing.Size(71, 21);
            this.cmbSetSwitchVal.TabIndex = 24;
            // 
            // lblTo
            // 
            this.lblTo.AutoSize = true;
            this.lblTo.Location = new System.Drawing.Point(185, 52);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(16, 13);
            this.lblTo.TabIndex = 23;
            this.lblTo.Text = "to";
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
            this.cmbSetSwitch.Location = new System.Drawing.Point(64, 49);
            this.cmbSetSwitch.Name = "cmbSetSwitch";
            this.cmbSetSwitch.Size = new System.Drawing.Size(115, 21);
            this.cmbSetSwitch.TabIndex = 22;
            // 
            // lblSwitch
            // 
            this.lblSwitch.AutoSize = true;
            this.lblSwitch.Location = new System.Drawing.Point(5, 51);
            this.lblSwitch.Name = "lblSwitch";
            this.lblSwitch.Size = new System.Drawing.Size(61, 13);
            this.lblSwitch.TabIndex = 21;
            this.lblSwitch.Text = "Set Switch:";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(89, 92);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(8, 92);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(5);
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Ok";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // EventCommand_Switch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpSetSwitch);
            this.Name = "EventCommand_Switch";
            this.Size = new System.Drawing.Size(298, 130);
            this.grpSetSwitch.ResumeLayout(false);
            this.grpSetSwitch.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpSetSwitch;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private DarkComboBox cmbSetSwitchVal;
        private System.Windows.Forms.Label lblTo;
        private DarkComboBox cmbSetSwitch;
        private System.Windows.Forms.Label lblSwitch;
        private DarkRadioButton rdoGlobalSwitch;
        private DarkRadioButton rdoPlayerSwitch;
    }
}
