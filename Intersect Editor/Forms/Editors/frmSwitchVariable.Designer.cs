namespace Intersect_Editor.Forms.Editors
{
    partial class frmSwitchVariable
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSwitchVariable));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdoPlayerSwitch = new System.Windows.Forms.RadioButton();
            this.rdoPlayerVariables = new System.Windows.Forms.RadioButton();
            this.rdoGlobalVariables = new System.Windows.Forms.RadioButton();
            this.rdoGlobalSwitches = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lstObjects = new System.Windows.Forms.ListBox();
            this.grpEditor = new System.Windows.Forms.GroupBox();
            this.lblObject = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtObjectName = new System.Windows.Forms.TextBox();
            this.txtVariableVal = new System.Windows.Forms.TextBox();
            this.lblValue = new System.Windows.Forms.Label();
            this.cmbSwitchValue = new System.Windows.Forms.ComboBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.grpEditor.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdoGlobalVariables);
            this.groupBox1.Controls.Add(this.rdoGlobalSwitches);
            this.groupBox1.Controls.Add(this.rdoPlayerVariables);
            this.groupBox1.Controls.Add(this.rdoPlayerSwitch);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(490, 53);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Switch or Variable Type";
            // 
            // rdoPlayerSwitch
            // 
            this.rdoPlayerSwitch.AutoSize = true;
            this.rdoPlayerSwitch.Checked = true;
            this.rdoPlayerSwitch.Location = new System.Drawing.Point(7, 20);
            this.rdoPlayerSwitch.Name = "rdoPlayerSwitch";
            this.rdoPlayerSwitch.Size = new System.Drawing.Size(100, 17);
            this.rdoPlayerSwitch.TabIndex = 0;
            this.rdoPlayerSwitch.TabStop = true;
            this.rdoPlayerSwitch.Text = "Player Switches";
            this.rdoPlayerSwitch.UseVisualStyleBackColor = true;
            this.rdoPlayerSwitch.CheckedChanged += new System.EventHandler(this.rdoPlayerSwitch_CheckedChanged);
            // 
            // rdoPlayerVariables
            // 
            this.rdoPlayerVariables.AutoSize = true;
            this.rdoPlayerVariables.Location = new System.Drawing.Point(113, 20);
            this.rdoPlayerVariables.Name = "rdoPlayerVariables";
            this.rdoPlayerVariables.Size = new System.Drawing.Size(100, 17);
            this.rdoPlayerVariables.TabIndex = 1;
            this.rdoPlayerVariables.Text = "Player Variables";
            this.rdoPlayerVariables.UseVisualStyleBackColor = true;
            this.rdoPlayerVariables.CheckedChanged += new System.EventHandler(this.rdoPlayerVariables_CheckedChanged);
            // 
            // rdoGlobalVariables
            // 
            this.rdoGlobalVariables.AutoSize = true;
            this.rdoGlobalVariables.Location = new System.Drawing.Point(326, 20);
            this.rdoGlobalVariables.Name = "rdoGlobalVariables";
            this.rdoGlobalVariables.Size = new System.Drawing.Size(101, 17);
            this.rdoGlobalVariables.TabIndex = 3;
            this.rdoGlobalVariables.Text = "Global Variables";
            this.rdoGlobalVariables.UseVisualStyleBackColor = true;
            this.rdoGlobalVariables.CheckedChanged += new System.EventHandler(this.rdoGlobalVariables_CheckedChanged);
            // 
            // rdoGlobalSwitches
            // 
            this.rdoGlobalSwitches.AutoSize = true;
            this.rdoGlobalSwitches.Location = new System.Drawing.Point(219, 20);
            this.rdoGlobalSwitches.Name = "rdoGlobalSwitches";
            this.rdoGlobalSwitches.Size = new System.Drawing.Size(101, 17);
            this.rdoGlobalSwitches.TabIndex = 2;
            this.rdoGlobalSwitches.Text = "Global Switches";
            this.rdoGlobalSwitches.UseVisualStyleBackColor = true;
            this.rdoGlobalSwitches.CheckedChanged += new System.EventHandler(this.rdoGlobalSwitches_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lstObjects);
            this.groupBox2.Location = new System.Drawing.Point(13, 73);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 469);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Switch/Variable List";
            // 
            // lstObjects
            // 
            this.lstObjects.FormattingEnabled = true;
            this.lstObjects.Location = new System.Drawing.Point(7, 20);
            this.lstObjects.Name = "lstObjects";
            this.lstObjects.Size = new System.Drawing.Size(187, 433);
            this.lstObjects.TabIndex = 0;
            this.lstObjects.SelectedIndexChanged += new System.EventHandler(this.lstObjects_SelectedIndexChanged);
            // 
            // grpEditor
            // 
            this.grpEditor.Controls.Add(this.lblValue);
            this.grpEditor.Controls.Add(this.txtObjectName);
            this.grpEditor.Controls.Add(this.label2);
            this.grpEditor.Controls.Add(this.lblObject);
            this.grpEditor.Controls.Add(this.cmbSwitchValue);
            this.grpEditor.Controls.Add(this.txtVariableVal);
            this.grpEditor.Location = new System.Drawing.Point(219, 73);
            this.grpEditor.Name = "grpEditor";
            this.grpEditor.Size = new System.Drawing.Size(284, 111);
            this.grpEditor.TabIndex = 2;
            this.grpEditor.TabStop = false;
            this.grpEditor.Text = "Switch/Variable Editor";
            this.grpEditor.Visible = false;
            // 
            // lblObject
            // 
            this.lblObject.AutoSize = true;
            this.lblObject.Location = new System.Drawing.Point(10, 20);
            this.lblObject.Name = "lblObject";
            this.lblObject.Size = new System.Drawing.Size(87, 13);
            this.lblObject.TabIndex = 0;
            this.lblObject.Text = "Player Switch #1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Name:";
            // 
            // txtObjectName
            // 
            this.txtObjectName.Location = new System.Drawing.Point(54, 40);
            this.txtObjectName.Name = "txtObjectName";
            this.txtObjectName.Size = new System.Drawing.Size(224, 20);
            this.txtObjectName.TabIndex = 2;
            this.txtObjectName.TextChanged += new System.EventHandler(this.txtObjectName_TextChanged);
            // 
            // txtVariableVal
            // 
            this.txtVariableVal.Location = new System.Drawing.Point(54, 66);
            this.txtVariableVal.Name = "txtVariableVal";
            this.txtVariableVal.Size = new System.Drawing.Size(224, 20);
            this.txtVariableVal.TabIndex = 4;
            this.txtVariableVal.TextChanged += new System.EventHandler(this.txtVariableVal_TextChanged);
            // 
            // lblValue
            // 
            this.lblValue.AutoSize = true;
            this.lblValue.Location = new System.Drawing.Point(10, 69);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(37, 13);
            this.lblValue.TabIndex = 3;
            this.lblValue.Text = "Value:";
            // 
            // cmbSwitchValue
            // 
            this.cmbSwitchValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSwitchValue.FormattingEnabled = true;
            this.cmbSwitchValue.Items.AddRange(new object[] {
            "False",
            "True"});
            this.cmbSwitchValue.Location = new System.Drawing.Point(54, 66);
            this.cmbSwitchValue.Name = "cmbSwitchValue";
            this.cmbSwitchValue.Size = new System.Drawing.Size(224, 21);
            this.cmbSwitchValue.TabIndex = 5;
            this.cmbSwitchValue.SelectedIndexChanged += new System.EventHandler(this.cmbSwitchValue_SelectedIndexChanged);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(339, 519);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(428, 519);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmSwitchVariable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(515, 554);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.grpEditor);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmSwitchVariable";
            this.Text = "Switch and Variable Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSwitchVariable_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.grpEditor.ResumeLayout(false);
            this.grpEditor.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdoGlobalVariables;
        private System.Windows.Forms.RadioButton rdoGlobalSwitches;
        private System.Windows.Forms.RadioButton rdoPlayerVariables;
        private System.Windows.Forms.RadioButton rdoPlayerSwitch;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox lstObjects;
        private System.Windows.Forms.GroupBox grpEditor;
        private System.Windows.Forms.ComboBox cmbSwitchValue;
        private System.Windows.Forms.TextBox txtVariableVal;
        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.TextBox txtObjectName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblObject;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
    }
}