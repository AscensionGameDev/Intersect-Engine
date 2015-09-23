namespace Intersect_Editor.Forms
{
    partial class frmQuest
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.lstQuests = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtEndDesc = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtStartDesc = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cmbClass = new System.Windows.Forms.ComboBox();
            this.lblVariableValue = new System.Windows.Forms.Label();
            this.scrlVariableValue = new System.Windows.Forms.HScrollBar();
            this.lblVariable = new System.Windows.Forms.Label();
            this.scrlVariable = new System.Windows.Forms.HScrollBar();
            this.lblSwitch = new System.Windows.Forms.Label();
            this.scrlSwitch = new System.Windows.Forms.HScrollBar();
            this.lblQuest = new System.Windows.Forms.Label();
            this.scrlQuest = new System.Windows.Forms.HScrollBar();
            this.lblLevel = new System.Windows.Forms.Label();
            this.lblItem = new System.Windows.Forms.Label();
            this.lblClass = new System.Windows.Forms.Label();
            this.scrlLevel = new System.Windows.Forms.HScrollBar();
            this.scrlItem = new System.Windows.Forms.HScrollBar();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lblObjective2 = new System.Windows.Forms.Label();
            this.lblObjective1 = new System.Windows.Forms.Label();
            this.scrlObjective2 = new System.Windows.Forms.HScrollBar();
            this.scrlObjective1 = new System.Windows.Forms.HScrollBar();
            this.rbEvent = new System.Windows.Forms.RadioButton();
            this.rbNpc = new System.Windows.Forms.RadioButton();
            this.rbItem = new System.Windows.Forms.RadioButton();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.txtAmount = new System.Windows.Forms.TextBox();
            this.lblAmount = new System.Windows.Forms.Label();
            this.scrlExp = new System.Windows.Forms.HScrollBar();
            this.lblExp = new System.Windows.Forms.Label();
            this.scrlItemReward = new System.Windows.Forms.HScrollBar();
            this.lblItemReward = new System.Windows.Forms.Label();
            this.scrlIndex = new System.Windows.Forms.HScrollBar();
            this.lblIndex = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.txtDesc = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lblTask = new System.Windows.Forms.Label();
            this.scrlTask = new System.Windows.Forms.HScrollBar();
            this.lblMaxTasks = new System.Windows.Forms.Label();
            this.scrlMaxTasks = new System.Windows.Forms.HScrollBar();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnDelete);
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Controls.Add(this.lstQuests);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(203, 374);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Quests";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(6, 341);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(190, 27);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnDelete.Location = new System.Drawing.Point(6, 308);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(190, 27);
            this.btnDelete.TabIndex = 3;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(6, 275);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(190, 27);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lstQuests
            // 
            this.lstQuests.FormattingEnabled = true;
            this.lstQuests.Location = new System.Drawing.Point(6, 19);
            this.lstQuests.Name = "lstQuests";
            this.lstQuests.Size = new System.Drawing.Size(191, 251);
            this.lstQuests.TabIndex = 1;
            this.lstQuests.Click += new System.EventHandler(this.lstQuests_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtEndDesc);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.txtStartDesc);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.txtName);
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(221, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(259, 374);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "General";
            // 
            // txtEndDesc
            // 
            this.txtEndDesc.AcceptsReturn = true;
            this.txtEndDesc.Location = new System.Drawing.Point(101, 105);
            this.txtEndDesc.Multiline = true;
            this.txtEndDesc.Name = "txtEndDesc";
            this.txtEndDesc.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtEndDesc.Size = new System.Drawing.Size(151, 51);
            this.txtEndDesc.TabIndex = 28;
            this.txtEndDesc.TextChanged += new System.EventHandler(this.txtEndDesc_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 105);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 13);
            this.label5.TabIndex = 27;
            this.label5.Text = "End Description:";
            // 
            // txtStartDesc
            // 
            this.txtStartDesc.AcceptsReturn = true;
            this.txtStartDesc.Location = new System.Drawing.Point(101, 48);
            this.txtStartDesc.Multiline = true;
            this.txtStartDesc.Name = "txtStartDesc";
            this.txtStartDesc.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtStartDesc.Size = new System.Drawing.Size(151, 51);
            this.txtStartDesc.TabIndex = 26;
            this.txtStartDesc.TextChanged += new System.EventHandler(this.txtStartDesc_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 13);
            this.label2.TabIndex = 25;
            this.label2.Text = "Start Description:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(101, 17);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(151, 20);
            this.txtName.TabIndex = 1;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cmbClass);
            this.groupBox3.Controls.Add(this.lblVariableValue);
            this.groupBox3.Controls.Add(this.scrlVariableValue);
            this.groupBox3.Controls.Add(this.lblVariable);
            this.groupBox3.Controls.Add(this.scrlVariable);
            this.groupBox3.Controls.Add(this.lblSwitch);
            this.groupBox3.Controls.Add(this.scrlSwitch);
            this.groupBox3.Controls.Add(this.lblQuest);
            this.groupBox3.Controls.Add(this.scrlQuest);
            this.groupBox3.Controls.Add(this.lblLevel);
            this.groupBox3.Controls.Add(this.lblItem);
            this.groupBox3.Controls.Add(this.lblClass);
            this.groupBox3.Controls.Add(this.scrlLevel);
            this.groupBox3.Controls.Add(this.scrlItem);
            this.groupBox3.Location = new System.Drawing.Point(6, 162);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(246, 205);
            this.groupBox3.TabIndex = 16;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Requirements";
            // 
            // cmbClass
            // 
            this.cmbClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbClass.FormattingEnabled = true;
            this.cmbClass.Items.AddRange(new object[] {
            "None"});
            this.cmbClass.Location = new System.Drawing.Point(159, 15);
            this.cmbClass.Name = "cmbClass";
            this.cmbClass.Size = new System.Drawing.Size(81, 21);
            this.cmbClass.TabIndex = 14;
            this.cmbClass.Click += new System.EventHandler(this.cmbClass_Click);
            // 
            // lblVariableValue
            // 
            this.lblVariableValue.AutoSize = true;
            this.lblVariableValue.Location = new System.Drawing.Point(10, 175);
            this.lblVariableValue.Name = "lblVariableValue";
            this.lblVariableValue.Size = new System.Drawing.Size(87, 13);
            this.lblVariableValue.TabIndex = 13;
            this.lblVariableValue.Text = "Variable Value: 0";
            // 
            // scrlVariableValue
            // 
            this.scrlVariableValue.LargeChange = 1;
            this.scrlVariableValue.Location = new System.Drawing.Point(159, 175);
            this.scrlVariableValue.Name = "scrlVariableValue";
            this.scrlVariableValue.Size = new System.Drawing.Size(81, 17);
            this.scrlVariableValue.TabIndex = 12;
            this.scrlVariableValue.ValueChanged += new System.EventHandler(this.scrlVariableValue_ValueChanged);
            // 
            // lblVariable
            // 
            this.lblVariable.AutoSize = true;
            this.lblVariable.Location = new System.Drawing.Point(10, 150);
            this.lblVariable.Name = "lblVariable";
            this.lblVariable.Size = new System.Drawing.Size(57, 13);
            this.lblVariable.TabIndex = 11;
            this.lblVariable.Text = "Variable: 0";
            // 
            // scrlVariable
            // 
            this.scrlVariable.LargeChange = 1;
            this.scrlVariable.Location = new System.Drawing.Point(159, 150);
            this.scrlVariable.Name = "scrlVariable";
            this.scrlVariable.Size = new System.Drawing.Size(81, 17);
            this.scrlVariable.TabIndex = 10;
            this.scrlVariable.ValueChanged += new System.EventHandler(this.scrlVariable_ValueChanged);
            // 
            // lblSwitch
            // 
            this.lblSwitch.AutoSize = true;
            this.lblSwitch.Location = new System.Drawing.Point(10, 123);
            this.lblSwitch.Name = "lblSwitch";
            this.lblSwitch.Size = new System.Drawing.Size(51, 13);
            this.lblSwitch.TabIndex = 9;
            this.lblSwitch.Text = "Switch: 0";
            // 
            // scrlSwitch
            // 
            this.scrlSwitch.LargeChange = 1;
            this.scrlSwitch.Location = new System.Drawing.Point(159, 123);
            this.scrlSwitch.Name = "scrlSwitch";
            this.scrlSwitch.Size = new System.Drawing.Size(81, 17);
            this.scrlSwitch.TabIndex = 8;
            this.scrlSwitch.ValueChanged += new System.EventHandler(this.scrlSwitch_ValueChanged);
            // 
            // lblQuest
            // 
            this.lblQuest.AutoSize = true;
            this.lblQuest.Location = new System.Drawing.Point(10, 98);
            this.lblQuest.Name = "lblQuest";
            this.lblQuest.Size = new System.Drawing.Size(76, 13);
            this.lblQuest.TabIndex = 7;
            this.lblQuest.Text = "Quest: 0 None";
            // 
            // scrlQuest
            // 
            this.scrlQuest.LargeChange = 1;
            this.scrlQuest.Location = new System.Drawing.Point(159, 98);
            this.scrlQuest.Name = "scrlQuest";
            this.scrlQuest.Size = new System.Drawing.Size(81, 17);
            this.scrlQuest.TabIndex = 6;
            this.scrlQuest.ValueChanged += new System.EventHandler(this.scrlQuest_ValueChanged);
            // 
            // lblLevel
            // 
            this.lblLevel.AutoSize = true;
            this.lblLevel.Location = new System.Drawing.Point(10, 73);
            this.lblLevel.Name = "lblLevel";
            this.lblLevel.Size = new System.Drawing.Size(45, 13);
            this.lblLevel.TabIndex = 5;
            this.lblLevel.Text = "Level: 0";
            // 
            // lblItem
            // 
            this.lblItem.AutoSize = true;
            this.lblItem.Location = new System.Drawing.Point(10, 47);
            this.lblItem.Name = "lblItem";
            this.lblItem.Size = new System.Drawing.Size(68, 13);
            this.lblItem.TabIndex = 4;
            this.lblItem.Text = "Item: 0 None";
            // 
            // lblClass
            // 
            this.lblClass.AutoSize = true;
            this.lblClass.Location = new System.Drawing.Point(10, 20);
            this.lblClass.Name = "lblClass";
            this.lblClass.Size = new System.Drawing.Size(35, 13);
            this.lblClass.TabIndex = 3;
            this.lblClass.Text = "Class:";
            // 
            // scrlLevel
            // 
            this.scrlLevel.LargeChange = 1;
            this.scrlLevel.Location = new System.Drawing.Point(159, 73);
            this.scrlLevel.Name = "scrlLevel";
            this.scrlLevel.Size = new System.Drawing.Size(81, 17);
            this.scrlLevel.TabIndex = 2;
            this.scrlLevel.ValueChanged += new System.EventHandler(this.scrlLevel_ValueChanged);
            // 
            // scrlItem
            // 
            this.scrlItem.LargeChange = 1;
            this.scrlItem.Location = new System.Drawing.Point(159, 47);
            this.scrlItem.Name = "scrlItem";
            this.scrlItem.Size = new System.Drawing.Size(81, 17);
            this.scrlItem.TabIndex = 1;
            this.scrlItem.ValueChanged += new System.EventHandler(this.scrlItem_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name:";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lblObjective2);
            this.groupBox4.Controls.Add(this.lblObjective1);
            this.groupBox4.Controls.Add(this.scrlObjective2);
            this.groupBox4.Controls.Add(this.scrlObjective1);
            this.groupBox4.Controls.Add(this.rbEvent);
            this.groupBox4.Controls.Add(this.rbNpc);
            this.groupBox4.Controls.Add(this.rbItem);
            this.groupBox4.Location = new System.Drawing.Point(6, 135);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(246, 93);
            this.groupBox4.TabIndex = 17;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Objective";
            // 
            // lblObjective2
            // 
            this.lblObjective2.AutoSize = true;
            this.lblObjective2.Location = new System.Drawing.Point(131, 53);
            this.lblObjective2.Name = "lblObjective2";
            this.lblObjective2.Size = new System.Drawing.Size(63, 13);
            this.lblObjective2.TabIndex = 19;
            this.lblObjective2.Text = "Quantity: x1";
            // 
            // lblObjective1
            // 
            this.lblObjective1.AutoSize = true;
            this.lblObjective1.Location = new System.Drawing.Point(131, 16);
            this.lblObjective1.Name = "lblObjective1";
            this.lblObjective1.Size = new System.Drawing.Size(68, 13);
            this.lblObjective1.TabIndex = 18;
            this.lblObjective1.Text = "Item: 0 None";
            // 
            // scrlObjective2
            // 
            this.scrlObjective2.LargeChange = 1;
            this.scrlObjective2.Location = new System.Drawing.Point(134, 66);
            this.scrlObjective2.Name = "scrlObjective2";
            this.scrlObjective2.Size = new System.Drawing.Size(106, 17);
            this.scrlObjective2.TabIndex = 4;
            this.scrlObjective2.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlObjective2_Scroll);
            // 
            // scrlObjective1
            // 
            this.scrlObjective1.LargeChange = 1;
            this.scrlObjective1.Location = new System.Drawing.Point(134, 29);
            this.scrlObjective1.Name = "scrlObjective1";
            this.scrlObjective1.Size = new System.Drawing.Size(106, 17);
            this.scrlObjective1.TabIndex = 3;
            this.scrlObjective1.ValueChanged += new System.EventHandler(this.scrlObjective1_ValueChanged);
            // 
            // rbEvent
            // 
            this.rbEvent.AutoSize = true;
            this.rbEvent.Location = new System.Drawing.Point(13, 66);
            this.rbEvent.Name = "rbEvent";
            this.rbEvent.Size = new System.Drawing.Size(93, 17);
            this.rbEvent.TabIndex = 2;
            this.rbEvent.Text = "Event Defined";
            this.rbEvent.UseVisualStyleBackColor = true;
            this.rbEvent.Click += new System.EventHandler(this.rbEvent_Click);
            // 
            // rbNpc
            // 
            this.rbNpc.AutoSize = true;
            this.rbNpc.Location = new System.Drawing.Point(13, 43);
            this.rbNpc.Name = "rbNpc";
            this.rbNpc.Size = new System.Drawing.Size(66, 17);
            this.rbNpc.TabIndex = 1;
            this.rbNpc.Text = "Kill Npcs";
            this.rbNpc.UseVisualStyleBackColor = true;
            this.rbNpc.Click += new System.EventHandler(this.rbNpc_Click);
            // 
            // rbItem
            // 
            this.rbItem.AutoSize = true;
            this.rbItem.Checked = true;
            this.rbItem.Location = new System.Drawing.Point(13, 20);
            this.rbItem.Name = "rbItem";
            this.rbItem.Size = new System.Drawing.Size(85, 17);
            this.rbItem.TabIndex = 0;
            this.rbItem.TabStop = true;
            this.rbItem.Text = "Gather Items";
            this.rbItem.UseVisualStyleBackColor = true;
            this.rbItem.Click += new System.EventHandler(this.rbItem_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox5.Controls.Add(this.txtAmount);
            this.groupBox5.Controls.Add(this.lblAmount);
            this.groupBox5.Controls.Add(this.scrlExp);
            this.groupBox5.Controls.Add(this.lblExp);
            this.groupBox5.Controls.Add(this.scrlItemReward);
            this.groupBox5.Controls.Add(this.lblItemReward);
            this.groupBox5.Controls.Add(this.scrlIndex);
            this.groupBox5.Controls.Add(this.lblIndex);
            this.groupBox5.Location = new System.Drawing.Point(7, 234);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(246, 133);
            this.groupBox5.TabIndex = 18;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Rewards";
            // 
            // txtAmount
            // 
            this.txtAmount.Location = new System.Drawing.Point(134, 75);
            this.txtAmount.Name = "txtAmount";
            this.txtAmount.Size = new System.Drawing.Size(106, 20);
            this.txtAmount.TabIndex = 16;
            this.txtAmount.TextChanged += new System.EventHandler(this.txtAmount_TextChanged);
            // 
            // lblAmount
            // 
            this.lblAmount.AutoSize = true;
            this.lblAmount.Location = new System.Drawing.Point(10, 78);
            this.lblAmount.Name = "lblAmount";
            this.lblAmount.Size = new System.Drawing.Size(46, 13);
            this.lblAmount.TabIndex = 15;
            this.lblAmount.Text = "Amount:";
            // 
            // scrlExp
            // 
            this.scrlExp.LargeChange = 1;
            this.scrlExp.Location = new System.Drawing.Point(134, 107);
            this.scrlExp.Maximum = 10000;
            this.scrlExp.Name = "scrlExp";
            this.scrlExp.Size = new System.Drawing.Size(106, 18);
            this.scrlExp.TabIndex = 14;
            this.scrlExp.ValueChanged += new System.EventHandler(this.scrlExp_ValueChanged);
            // 
            // lblExp
            // 
            this.lblExp.AutoSize = true;
            this.lblExp.Location = new System.Drawing.Point(10, 107);
            this.lblExp.Name = "lblExp";
            this.lblExp.Size = new System.Drawing.Size(72, 13);
            this.lblExp.TabIndex = 13;
            this.lblExp.Text = "Experience: 0";
            // 
            // scrlItemReward
            // 
            this.scrlItemReward.LargeChange = 1;
            this.scrlItemReward.Location = new System.Drawing.Point(134, 44);
            this.scrlItemReward.Maximum = 3600;
            this.scrlItemReward.Name = "scrlItemReward";
            this.scrlItemReward.Size = new System.Drawing.Size(106, 18);
            this.scrlItemReward.TabIndex = 12;
            this.scrlItemReward.ValueChanged += new System.EventHandler(this.scrlItemReward_ValueChanged);
            // 
            // lblItemReward
            // 
            this.lblItemReward.AutoSize = true;
            this.lblItemReward.Location = new System.Drawing.Point(10, 49);
            this.lblItemReward.Name = "lblItemReward";
            this.lblItemReward.Size = new System.Drawing.Size(68, 13);
            this.lblItemReward.TabIndex = 11;
            this.lblItemReward.Text = "Item: 0 None";
            // 
            // scrlIndex
            // 
            this.scrlIndex.LargeChange = 1;
            this.scrlIndex.Location = new System.Drawing.Point(134, 17);
            this.scrlIndex.Maximum = 10;
            this.scrlIndex.Minimum = 1;
            this.scrlIndex.Name = "scrlIndex";
            this.scrlIndex.Size = new System.Drawing.Size(106, 18);
            this.scrlIndex.TabIndex = 10;
            this.scrlIndex.Value = 1;
            this.scrlIndex.ValueChanged += new System.EventHandler(this.scrlIndex_ValueChanged);
            // 
            // lblIndex
            // 
            this.lblIndex.AutoSize = true;
            this.lblIndex.Location = new System.Drawing.Point(10, 20);
            this.lblIndex.Name = "lblIndex";
            this.lblIndex.Size = new System.Drawing.Size(85, 13);
            this.lblIndex.TabIndex = 9;
            this.lblIndex.Text = "Reward Index: 1";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.txtDesc);
            this.groupBox6.Controls.Add(this.label4);
            this.groupBox6.Controls.Add(this.lblTask);
            this.groupBox6.Controls.Add(this.scrlTask);
            this.groupBox6.Controls.Add(this.lblMaxTasks);
            this.groupBox6.Controls.Add(this.scrlMaxTasks);
            this.groupBox6.Controls.Add(this.groupBox4);
            this.groupBox6.Controls.Add(this.groupBox5);
            this.groupBox6.Location = new System.Drawing.Point(486, 12);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(259, 374);
            this.groupBox6.TabIndex = 19;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Tasks";
            // 
            // txtDesc
            // 
            this.txtDesc.AcceptsReturn = true;
            this.txtDesc.Location = new System.Drawing.Point(95, 78);
            this.txtDesc.Multiline = true;
            this.txtDesc.Name = "txtDesc";
            this.txtDesc.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDesc.Size = new System.Drawing.Size(151, 51);
            this.txtDesc.TabIndex = 24;
            this.txtDesc.TextChanged += new System.EventHandler(this.txtDesc_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 78);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 13);
            this.label4.TabIndex = 23;
            this.label4.Text = "Description:";
            // 
            // lblTask
            // 
            this.lblTask.AutoSize = true;
            this.lblTask.Location = new System.Drawing.Point(13, 48);
            this.lblTask.Name = "lblTask";
            this.lblTask.Size = new System.Drawing.Size(88, 13);
            this.lblTask.TabIndex = 22;
            this.lblTask.Text = "Task Selected: 1";
            // 
            // scrlTask
            // 
            this.scrlTask.LargeChange = 1;
            this.scrlTask.Location = new System.Drawing.Point(122, 48);
            this.scrlTask.Maximum = 1;
            this.scrlTask.Minimum = 1;
            this.scrlTask.Name = "scrlTask";
            this.scrlTask.Size = new System.Drawing.Size(124, 17);
            this.scrlTask.TabIndex = 21;
            this.scrlTask.Value = 1;
            this.scrlTask.ValueChanged += new System.EventHandler(this.scrlTask_ValueChanged);
            // 
            // lblMaxTasks
            // 
            this.lblMaxTasks.AutoSize = true;
            this.lblMaxTasks.Location = new System.Drawing.Point(13, 20);
            this.lblMaxTasks.Name = "lblMaxTasks";
            this.lblMaxTasks.Size = new System.Drawing.Size(71, 13);
            this.lblMaxTasks.TabIndex = 20;
            this.lblMaxTasks.Text = "Max Tasks: 1";
            // 
            // scrlMaxTasks
            // 
            this.scrlMaxTasks.LargeChange = 1;
            this.scrlMaxTasks.Location = new System.Drawing.Point(122, 20);
            this.scrlMaxTasks.Maximum = 10;
            this.scrlMaxTasks.Minimum = 1;
            this.scrlMaxTasks.Name = "scrlMaxTasks";
            this.scrlMaxTasks.Size = new System.Drawing.Size(124, 17);
            this.scrlMaxTasks.TabIndex = 19;
            this.scrlMaxTasks.Value = 1;
            this.scrlMaxTasks.ValueChanged += new System.EventHandler(this.scrlMaxTasks_ValueChanged);
            // 
            // frmQuest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(752, 394);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MinimizeBox = false;
            this.Name = "frmQuest";
            this.Text = "Quest Editor";
            this.Load += new System.EventHandler(this.frmQuest_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ListBox lstQuests;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lblLevel;
        private System.Windows.Forms.Label lblItem;
        private System.Windows.Forms.Label lblClass;
        private System.Windows.Forms.HScrollBar scrlLevel;
        private System.Windows.Forms.HScrollBar scrlItem;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label lblObjective2;
        private System.Windows.Forms.Label lblObjective1;
        private System.Windows.Forms.HScrollBar scrlObjective2;
        private System.Windows.Forms.HScrollBar scrlObjective1;
        private System.Windows.Forms.RadioButton rbEvent;
        private System.Windows.Forms.RadioButton rbNpc;
        private System.Windows.Forms.RadioButton rbItem;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox txtAmount;
        private System.Windows.Forms.Label lblAmount;
        private System.Windows.Forms.HScrollBar scrlExp;
        private System.Windows.Forms.Label lblExp;
        private System.Windows.Forms.HScrollBar scrlItemReward;
        private System.Windows.Forms.Label lblItemReward;
        private System.Windows.Forms.HScrollBar scrlIndex;
        private System.Windows.Forms.Label lblIndex;
        private System.Windows.Forms.Label lblQuest;
        private System.Windows.Forms.HScrollBar scrlQuest;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox txtDesc;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblTask;
        private System.Windows.Forms.HScrollBar scrlTask;
        private System.Windows.Forms.Label lblMaxTasks;
        private System.Windows.Forms.HScrollBar scrlMaxTasks;
        private System.Windows.Forms.TextBox txtEndDesc;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtStartDesc;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblVariableValue;
        private System.Windows.Forms.HScrollBar scrlVariableValue;
        private System.Windows.Forms.Label lblVariable;
        private System.Windows.Forms.HScrollBar scrlVariable;
        private System.Windows.Forms.Label lblSwitch;
        private System.Windows.Forms.HScrollBar scrlSwitch;
        private System.Windows.Forms.ComboBox cmbClass;
    }
}