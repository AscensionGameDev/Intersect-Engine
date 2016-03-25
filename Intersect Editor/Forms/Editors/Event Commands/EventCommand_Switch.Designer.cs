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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdoGlobalSwitch = new System.Windows.Forms.RadioButton();
            this.rdoPlayerSwitch = new System.Windows.Forms.RadioButton();
            this.cmbSetSwitchVal = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.cmbSetSwitch = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdoGlobalSwitch);
            this.groupBox1.Controls.Add(this.rdoPlayerSwitch);
            this.groupBox1.Controls.Add(this.cmbSetSwitchVal);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.cmbSetSwitch);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(292, 121);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Set Switch";
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
            this.rdoGlobalSwitch.UseVisualStyleBackColor = true;
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
            this.rdoPlayerSwitch.UseVisualStyleBackColor = true;
            this.rdoPlayerSwitch.CheckedChanged += new System.EventHandler(this.rdoPlayerSwitch_CheckedChanged);
            // 
            // cmbSetSwitchVal
            // 
            this.cmbSetSwitchVal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSetSwitchVal.FormattingEnabled = true;
            this.cmbSetSwitchVal.Items.AddRange(new object[] {
            "False",
            "True"});
            this.cmbSetSwitchVal.Location = new System.Drawing.Point(205, 49);
            this.cmbSetSwitchVal.Name = "cmbSetSwitchVal";
            this.cmbSetSwitchVal.Size = new System.Drawing.Size(71, 21);
            this.cmbSetSwitchVal.TabIndex = 24;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(185, 52);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(16, 13);
            this.label15.TabIndex = 23;
            this.label15.Text = "to";
            // 
            // cmbSetSwitch
            // 
            this.cmbSetSwitch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSetSwitch.FormattingEnabled = true;
            this.cmbSetSwitch.Location = new System.Drawing.Point(64, 49);
            this.cmbSetSwitch.Name = "cmbSetSwitch";
            this.cmbSetSwitch.Size = new System.Drawing.Size(115, 21);
            this.cmbSetSwitch.TabIndex = 22;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(5, 51);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(61, 13);
            this.label16.TabIndex = 21;
            this.label16.Text = "Set Switch:";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(89, 92);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(8, 92);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Ok";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // EventCommand_Switch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.groupBox1);
            this.Name = "EventCommand_Switch";
            this.Size = new System.Drawing.Size(298, 130);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ComboBox cmbSetSwitchVal;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox cmbSetSwitch;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.RadioButton rdoGlobalSwitch;
        private System.Windows.Forms.RadioButton rdoPlayerSwitch;
    }
}
