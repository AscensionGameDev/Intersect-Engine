namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    partial class EventCommand_ConditionalBranch
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
            this.btnSave = new System.Windows.Forms.Button();
            this.grpPlayerVariable = new System.Windows.Forms.GroupBox();
            this.txtVariableVal = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbVariableMod = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbVariable = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.grpSwitch = new System.Windows.Forms.GroupBox();
            this.cmbSwitchVal = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbSwitch = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbConditionType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpSelfSwitch = new System.Windows.Forms.GroupBox();
            this.cmbSelfSwitchVal = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.cmbSelfSwitch = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.grpLevel = new System.Windows.Forms.GroupBox();
            this.scrlLevel = new System.Windows.Forms.HScrollBar();
            this.lblLevel = new System.Windows.Forms.Label();
            this.cmbLevelComparator = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.grpClass = new System.Windows.Forms.GroupBox();
            this.cmbClass = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.grpSpell = new System.Windows.Forms.GroupBox();
            this.cmbSpell = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.grpHasItem = new System.Windows.Forms.GroupBox();
            this.scrlItemQuantity = new System.Windows.Forms.HScrollBar();
            this.cmbItem = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.lblItemQuantity = new System.Windows.Forms.Label();
            this.grpPowerIs = new System.Windows.Forms.GroupBox();
            this.cmbPower = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.grpPlayerVariable.SuspendLayout();
            this.grpSwitch.SuspendLayout();
            this.grpSelfSwitch.SuspendLayout();
            this.grpLevel.SuspendLayout();
            this.grpClass.SuspendLayout();
            this.grpSpell.SuspendLayout();
            this.grpHasItem.SuspendLayout();
            this.grpPowerIs.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.grpPowerIs);
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Controls.Add(this.grpPlayerVariable);
            this.groupBox1.Controls.Add(this.grpSwitch);
            this.groupBox1.Controls.Add(this.cmbConditionType);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.grpSelfSwitch);
            this.groupBox1.Controls.Add(this.grpLevel);
            this.groupBox1.Controls.Add(this.grpClass);
            this.groupBox1.Controls.Add(this.grpSpell);
            this.groupBox1.Controls.Add(this.grpHasItem);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(256, 202);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Conditional";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(9, 168);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Ok";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // grpPlayerVariable
            // 
            this.grpPlayerVariable.Controls.Add(this.txtVariableVal);
            this.grpPlayerVariable.Controls.Add(this.label6);
            this.grpPlayerVariable.Controls.Add(this.cmbVariableMod);
            this.grpPlayerVariable.Controls.Add(this.label4);
            this.grpPlayerVariable.Controls.Add(this.cmbVariable);
            this.grpPlayerVariable.Controls.Add(this.label5);
            this.grpPlayerVariable.Location = new System.Drawing.Point(9, 40);
            this.grpPlayerVariable.Name = "grpPlayerVariable";
            this.grpPlayerVariable.Size = new System.Drawing.Size(236, 122);
            this.grpPlayerVariable.TabIndex = 24;
            this.grpPlayerVariable.TabStop = false;
            this.grpPlayerVariable.Text = "Player Variable";
            // 
            // txtVariableVal
            // 
            this.txtVariableVal.Location = new System.Drawing.Point(79, 89);
            this.txtVariableVal.Name = "txtVariableVal";
            this.txtVariableVal.Size = new System.Drawing.Size(141, 20);
            this.txtVariableVal.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 92);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "Value:";
            // 
            // cmbVariableMod
            // 
            this.cmbVariableMod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVariableMod.FormattingEnabled = true;
            this.cmbVariableMod.Items.AddRange(new object[] {
            "Equal To",
            "Greater Than or Equal To",
            "Less Than or Equal To",
            "Greater Than",
            "Less Than",
            "Does Not Equal"});
            this.cmbVariableMod.Location = new System.Drawing.Point(79, 52);
            this.cmbVariableMod.Name = "cmbVariableMod";
            this.cmbVariableMod.Size = new System.Drawing.Size(141, 21);
            this.cmbVariableMod.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Comparator";
            // 
            // cmbVariable
            // 
            this.cmbVariable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVariable.FormattingEnabled = true;
            this.cmbVariable.Location = new System.Drawing.Point(79, 17);
            this.cmbVariable.Name = "cmbVariable";
            this.cmbVariable.Size = new System.Drawing.Size(141, 21);
            this.cmbVariable.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Variable";
            // 
            // grpSwitch
            // 
            this.grpSwitch.Controls.Add(this.cmbSwitchVal);
            this.grpSwitch.Controls.Add(this.label3);
            this.grpSwitch.Controls.Add(this.cmbSwitch);
            this.grpSwitch.Controls.Add(this.label2);
            this.grpSwitch.Location = new System.Drawing.Point(9, 40);
            this.grpSwitch.Name = "grpSwitch";
            this.grpSwitch.Size = new System.Drawing.Size(236, 89);
            this.grpSwitch.TabIndex = 23;
            this.grpSwitch.TabStop = false;
            this.grpSwitch.Text = "Player Switch";
            // 
            // cmbSwitchVal
            // 
            this.cmbSwitchVal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSwitchVal.FormattingEnabled = true;
            this.cmbSwitchVal.Items.AddRange(new object[] {
            "False",
            "True"});
            this.cmbSwitchVal.Location = new System.Drawing.Point(79, 52);
            this.cmbSwitchVal.Name = "cmbSwitchVal";
            this.cmbSwitchVal.Size = new System.Drawing.Size(141, 21);
            this.cmbSwitchVal.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(21, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Is: ";
            // 
            // cmbSwitch
            // 
            this.cmbSwitch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSwitch.FormattingEnabled = true;
            this.cmbSwitch.Location = new System.Drawing.Point(79, 17);
            this.cmbSwitch.Name = "cmbSwitch";
            this.cmbSwitch.Size = new System.Drawing.Size(141, 21);
            this.cmbSwitch.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Switch: ";
            // 
            // cmbConditionType
            // 
            this.cmbConditionType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbConditionType.FormattingEnabled = true;
            this.cmbConditionType.Items.AddRange(new object[] {
            "Player Switch is...",
            "Player Variable is...",
            "Global Switch is...",
            "Global Variable is...",
            "Has item...",
            "Class is...",
            "Knows spell...",
            "Level is....",
            "Self Switch is....",
            "Power level is...."});
            this.cmbConditionType.Location = new System.Drawing.Point(88, 13);
            this.cmbConditionType.Name = "cmbConditionType";
            this.cmbConditionType.Size = new System.Drawing.Size(157, 21);
            this.cmbConditionType.TabIndex = 22;
            this.cmbConditionType.SelectedIndexChanged += new System.EventHandler(this.cmbConditionType_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Condition Type:";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(90, 168);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // grpSelfSwitch
            // 
            this.grpSelfSwitch.Controls.Add(this.cmbSelfSwitchVal);
            this.grpSelfSwitch.Controls.Add(this.label11);
            this.grpSelfSwitch.Controls.Add(this.cmbSelfSwitch);
            this.grpSelfSwitch.Controls.Add(this.label12);
            this.grpSelfSwitch.Location = new System.Drawing.Point(9, 40);
            this.grpSelfSwitch.Name = "grpSelfSwitch";
            this.grpSelfSwitch.Size = new System.Drawing.Size(236, 89);
            this.grpSelfSwitch.TabIndex = 29;
            this.grpSelfSwitch.TabStop = false;
            this.grpSelfSwitch.Text = "Self Switch";
            // 
            // cmbSelfSwitchVal
            // 
            this.cmbSelfSwitchVal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSelfSwitchVal.FormattingEnabled = true;
            this.cmbSelfSwitchVal.Items.AddRange(new object[] {
            "False",
            "True"});
            this.cmbSelfSwitchVal.Location = new System.Drawing.Point(79, 52);
            this.cmbSelfSwitchVal.Name = "cmbSelfSwitchVal";
            this.cmbSelfSwitchVal.Size = new System.Drawing.Size(141, 21);
            this.cmbSelfSwitchVal.TabIndex = 3;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(10, 55);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(21, 13);
            this.label11.TabIndex = 2;
            this.label11.Text = "Is: ";
            // 
            // cmbSelfSwitch
            // 
            this.cmbSelfSwitch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSelfSwitch.FormattingEnabled = true;
            this.cmbSelfSwitch.Items.AddRange(new object[] {
            "A",
            "B",
            "C",
            "D"});
            this.cmbSelfSwitch.Location = new System.Drawing.Point(79, 17);
            this.cmbSelfSwitch.Name = "cmbSelfSwitch";
            this.cmbSelfSwitch.Size = new System.Drawing.Size(141, 21);
            this.cmbSelfSwitch.TabIndex = 1;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(7, 20);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(66, 13);
            this.label12.TabIndex = 0;
            this.label12.Text = "Self Switch: ";
            // 
            // grpLevel
            // 
            this.grpLevel.Controls.Add(this.scrlLevel);
            this.grpLevel.Controls.Add(this.lblLevel);
            this.grpLevel.Controls.Add(this.cmbLevelComparator);
            this.grpLevel.Controls.Add(this.label10);
            this.grpLevel.Location = new System.Drawing.Point(9, 40);
            this.grpLevel.Name = "grpLevel";
            this.grpLevel.Size = new System.Drawing.Size(236, 77);
            this.grpLevel.TabIndex = 28;
            this.grpLevel.TabStop = false;
            this.grpLevel.Text = "Level is";
            // 
            // scrlLevel
            // 
            this.scrlLevel.Location = new System.Drawing.Point(79, 49);
            this.scrlLevel.Maximum = 32000;
            this.scrlLevel.Name = "scrlLevel";
            this.scrlLevel.Size = new System.Drawing.Size(151, 17);
            this.scrlLevel.TabIndex = 5;
            this.scrlLevel.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlLevel_Scroll);
            // 
            // lblLevel
            // 
            this.lblLevel.AutoSize = true;
            this.lblLevel.Location = new System.Drawing.Point(10, 54);
            this.lblLevel.Name = "lblLevel";
            this.lblLevel.Size = new System.Drawing.Size(45, 13);
            this.lblLevel.TabIndex = 4;
            this.lblLevel.Text = "Level: 1";
            // 
            // cmbLevelComparator
            // 
            this.cmbLevelComparator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLevelComparator.FormattingEnabled = true;
            this.cmbLevelComparator.Items.AddRange(new object[] {
            "Equal To",
            "Greater Than or Equal To",
            "Less Than or Equal To",
            "Greater Than",
            "Less Than",
            "Does Not Equal"});
            this.cmbLevelComparator.Location = new System.Drawing.Point(79, 18);
            this.cmbLevelComparator.Name = "cmbLevelComparator";
            this.cmbLevelComparator.Size = new System.Drawing.Size(151, 21);
            this.cmbLevelComparator.TabIndex = 3;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(7, 20);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(64, 13);
            this.label10.TabIndex = 2;
            this.label10.Text = "Comparator:";
            // 
            // grpClass
            // 
            this.grpClass.Controls.Add(this.cmbClass);
            this.grpClass.Controls.Add(this.label9);
            this.grpClass.Location = new System.Drawing.Point(9, 40);
            this.grpClass.Name = "grpClass";
            this.grpClass.Size = new System.Drawing.Size(236, 52);
            this.grpClass.TabIndex = 27;
            this.grpClass.TabStop = false;
            this.grpClass.Text = "Class is";
            // 
            // cmbClass
            // 
            this.cmbClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbClass.FormattingEnabled = true;
            this.cmbClass.Items.AddRange(new object[] {
            "Equal To",
            "Greater Than or Equal To",
            "Less Than or Equal To",
            "Greater Than",
            "Less Than",
            "Does Not Equal"});
            this.cmbClass.Location = new System.Drawing.Point(79, 18);
            this.cmbClass.Name = "cmbClass";
            this.cmbClass.Size = new System.Drawing.Size(151, 21);
            this.cmbClass.TabIndex = 3;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 20);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(35, 13);
            this.label9.TabIndex = 2;
            this.label9.Text = "Class:";
            // 
            // grpSpell
            // 
            this.grpSpell.Controls.Add(this.cmbSpell);
            this.grpSpell.Controls.Add(this.label7);
            this.grpSpell.Location = new System.Drawing.Point(9, 40);
            this.grpSpell.Name = "grpSpell";
            this.grpSpell.Size = new System.Drawing.Size(236, 52);
            this.grpSpell.TabIndex = 26;
            this.grpSpell.TabStop = false;
            this.grpSpell.Text = "Knows Spell";
            // 
            // cmbSpell
            // 
            this.cmbSpell.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSpell.FormattingEnabled = true;
            this.cmbSpell.Items.AddRange(new object[] {
            "Equal To",
            "Greater Than or Equal To",
            "Less Than or Equal To",
            "Greater Than",
            "Less Than",
            "Does Not Equal"});
            this.cmbSpell.Location = new System.Drawing.Point(79, 18);
            this.cmbSpell.Name = "cmbSpell";
            this.cmbSpell.Size = new System.Drawing.Size(151, 21);
            this.cmbSpell.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 20);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(33, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Spell:";
            // 
            // grpHasItem
            // 
            this.grpHasItem.Controls.Add(this.scrlItemQuantity);
            this.grpHasItem.Controls.Add(this.cmbItem);
            this.grpHasItem.Controls.Add(this.label8);
            this.grpHasItem.Controls.Add(this.lblItemQuantity);
            this.grpHasItem.Location = new System.Drawing.Point(9, 40);
            this.grpHasItem.Name = "grpHasItem";
            this.grpHasItem.Size = new System.Drawing.Size(236, 83);
            this.grpHasItem.TabIndex = 25;
            this.grpHasItem.TabStop = false;
            this.grpHasItem.Text = "Has Item";
            // 
            // scrlItemQuantity
            // 
            this.scrlItemQuantity.Location = new System.Drawing.Point(104, 18);
            this.scrlItemQuantity.Maximum = 32000;
            this.scrlItemQuantity.Minimum = 1;
            this.scrlItemQuantity.Name = "scrlItemQuantity";
            this.scrlItemQuantity.Size = new System.Drawing.Size(116, 17);
            this.scrlItemQuantity.TabIndex = 4;
            this.scrlItemQuantity.Value = 1;
            this.scrlItemQuantity.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlItemQuantity_Scroll);
            // 
            // cmbItem
            // 
            this.cmbItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbItem.FormattingEnabled = true;
            this.cmbItem.Items.AddRange(new object[] {
            "Equal To",
            "Greater Than or Equal To",
            "Less Than or Equal To",
            "Greater Than",
            "Less Than",
            "Does Not Equal"});
            this.cmbItem.Location = new System.Drawing.Point(104, 52);
            this.cmbItem.Name = "cmbItem";
            this.cmbItem.Size = new System.Drawing.Size(116, 21);
            this.cmbItem.TabIndex = 3;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 55);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(30, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "Item:";
            // 
            // lblItemQuantity
            // 
            this.lblItemQuantity.AutoSize = true;
            this.lblItemQuantity.Location = new System.Drawing.Point(7, 20);
            this.lblItemQuantity.Name = "lblItemQuantity";
            this.lblItemQuantity.Size = new System.Drawing.Size(75, 13);
            this.lblItemQuantity.TabIndex = 0;
            this.lblItemQuantity.Text = "Has at least: 1";
            // 
            // grpPowerIs
            // 
            this.grpPowerIs.Controls.Add(this.cmbPower);
            this.grpPowerIs.Controls.Add(this.label15);
            this.grpPowerIs.Location = new System.Drawing.Point(9, 41);
            this.grpPowerIs.Name = "grpPowerIs";
            this.grpPowerIs.Size = new System.Drawing.Size(236, 51);
            this.grpPowerIs.TabIndex = 25;
            this.grpPowerIs.TabStop = false;
            this.grpPowerIs.Text = "Power Is...";
            // 
            // cmbPower
            // 
            this.cmbPower.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPower.FormattingEnabled = true;
            this.cmbPower.Items.AddRange(new object[] {
            "Mod or Admin",
            "Only Admin"});
            this.cmbPower.Location = new System.Drawing.Point(79, 17);
            this.cmbPower.Name = "cmbPower";
            this.cmbPower.Size = new System.Drawing.Size(141, 21);
            this.cmbPower.TabIndex = 1;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(7, 20);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(40, 13);
            this.label15.TabIndex = 0;
            this.label15.Text = "Power:";
            // 
            // EventCommand_ConditionalBranch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.groupBox1);
            this.Name = "EventCommand_ConditionalBranch";
            this.Size = new System.Drawing.Size(267, 208);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpPlayerVariable.ResumeLayout(false);
            this.grpPlayerVariable.PerformLayout();
            this.grpSwitch.ResumeLayout(false);
            this.grpSwitch.PerformLayout();
            this.grpSelfSwitch.ResumeLayout(false);
            this.grpSelfSwitch.PerformLayout();
            this.grpLevel.ResumeLayout(false);
            this.grpLevel.PerformLayout();
            this.grpClass.ResumeLayout(false);
            this.grpClass.PerformLayout();
            this.grpSpell.ResumeLayout(false);
            this.grpSpell.PerformLayout();
            this.grpHasItem.ResumeLayout(false);
            this.grpHasItem.PerformLayout();
            this.grpPowerIs.ResumeLayout(false);
            this.grpPowerIs.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.GroupBox grpSwitch;
        private System.Windows.Forms.ComboBox cmbSwitchVal;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbSwitch;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbConditionType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox grpPlayerVariable;
        private System.Windows.Forms.ComboBox cmbVariableMod;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbVariable;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtVariableVal;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox grpHasItem;
        private System.Windows.Forms.HScrollBar scrlItemQuantity;
        private System.Windows.Forms.ComboBox cmbItem;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblItemQuantity;
        private System.Windows.Forms.GroupBox grpSpell;
        private System.Windows.Forms.ComboBox cmbSpell;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox grpClass;
        private System.Windows.Forms.ComboBox cmbClass;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox grpLevel;
        private System.Windows.Forms.HScrollBar scrlLevel;
        private System.Windows.Forms.Label lblLevel;
        private System.Windows.Forms.ComboBox cmbLevelComparator;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox grpSelfSwitch;
        private System.Windows.Forms.ComboBox cmbSelfSwitchVal;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cmbSelfSwitch;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.GroupBox grpPowerIs;
        private System.Windows.Forms.ComboBox cmbPower;
        private System.Windows.Forms.Label label15;
    }
}
