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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmQuest));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lstQuests = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.chkLogAfterComplete = new System.Windows.Forms.CheckBox();
            this.chkLogBeforeOffer = new System.Windows.Forms.CheckBox();
            this.txtBeforeDesc = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtInProgressDesc = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtEndDesc = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtStartDesc = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.chkQuittable = new System.Windows.Forms.CheckBox();
            this.chkRepeatable = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.btnShiftTaskDown = new System.Windows.Forms.Button();
            this.btnShiftTaskUp = new System.Windows.Forms.Button();
            this.btnRemoveTask = new System.Windows.Forms.Button();
            this.lstTasks = new System.Windows.Forms.ListBox();
            this.btnAddTask = new System.Windows.Forms.Button();
            this.pnlContainer = new System.Windows.Forms.Panel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnEditCompletionEvent = new System.Windows.Forms.Button();
            this.btnEditStartEvent = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnRemoveRequirement = new System.Windows.Forms.Button();
            this.btnAddRequirement = new System.Windows.Forms.Button();
            this.lstRequirements = new System.Windows.Forms.ListBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripItemNew = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripItemDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripItemCopy = new System.Windows.Forms.ToolStripButton();
            this.toolStripItemPaste = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripItemUndo = new System.Windows.Forms.ToolStripButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.pnlContainer.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lstQuests);
            this.groupBox1.Location = new System.Drawing.Point(12, 34);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(203, 374);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Quests";
            // 
            // lstQuests
            // 
            this.lstQuests.FormattingEnabled = true;
            this.lstQuests.Location = new System.Drawing.Point(6, 19);
            this.lstQuests.Name = "lstQuests";
            this.lstQuests.Size = new System.Drawing.Size(191, 342);
            this.lstQuests.TabIndex = 1;
            this.lstQuests.Click += new System.EventHandler(this.lstQuests_Click);
            this.lstQuests.KeyDown += new System.Windows.Forms.KeyEventHandler(this.itemList_KeyDown);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.groupBox7);
            this.groupBox2.Controls.Add(this.txtBeforeDesc);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.txtInProgressDesc);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.txtEndDesc);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.txtStartDesc);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.txtName);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.groupBox5);
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(580, 164);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "General";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.chkLogAfterComplete);
            this.groupBox7.Controls.Add(this.chkLogBeforeOffer);
            this.groupBox7.Location = new System.Drawing.Point(203, 9);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(238, 60);
            this.groupBox7.TabIndex = 37;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Quest Log Options";
            // 
            // chkLogAfterComplete
            // 
            this.chkLogAfterComplete.AutoSize = true;
            this.chkLogAfterComplete.Location = new System.Drawing.Point(6, 37);
            this.chkLogAfterComplete.Name = "chkLogAfterComplete";
            this.chkLogAfterComplete.Size = new System.Drawing.Size(223, 17);
            this.chkLogAfterComplete.TabIndex = 31;
            this.chkLogAfterComplete.Text = "Show in quest log after completing quest?";
            this.chkLogAfterComplete.UseVisualStyleBackColor = true;
            this.chkLogAfterComplete.CheckedChanged += new System.EventHandler(this.chkLogAfterComplete_CheckedChanged);
            // 
            // chkLogBeforeOffer
            // 
            this.chkLogBeforeOffer.AutoSize = true;
            this.chkLogBeforeOffer.Location = new System.Drawing.Point(6, 19);
            this.chkLogBeforeOffer.Name = "chkLogBeforeOffer";
            this.chkLogBeforeOffer.Size = new System.Drawing.Size(228, 17);
            this.chkLogBeforeOffer.TabIndex = 30;
            this.chkLogBeforeOffer.Text = "Show in quest log before accepting quest?";
            this.chkLogBeforeOffer.UseVisualStyleBackColor = true;
            this.chkLogBeforeOffer.CheckedChanged += new System.EventHandler(this.chkLogBeforeOffer_CheckedChanged);
            // 
            // txtBeforeDesc
            // 
            this.txtBeforeDesc.AcceptsReturn = true;
            this.txtBeforeDesc.Location = new System.Drawing.Point(10, 90);
            this.txtBeforeDesc.Multiline = true;
            this.txtBeforeDesc.Name = "txtBeforeDesc";
            this.txtBeforeDesc.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtBeforeDesc.Size = new System.Drawing.Size(136, 68);
            this.txtBeforeDesc.TabIndex = 36;
            this.txtBeforeDesc.TextChanged += new System.EventHandler(this.txtBeforeDesc_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 75);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(123, 13);
            this.label7.TabIndex = 35;
            this.label7.Text = "Before Offer Description:";
            // 
            // txtInProgressDesc
            // 
            this.txtInProgressDesc.AcceptsReturn = true;
            this.txtInProgressDesc.Location = new System.Drawing.Point(294, 91);
            this.txtInProgressDesc.Multiline = true;
            this.txtInProgressDesc.Name = "txtInProgressDesc";
            this.txtInProgressDesc.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtInProgressDesc.Size = new System.Drawing.Size(136, 67);
            this.txtInProgressDesc.TabIndex = 33;
            this.txtInProgressDesc.TextChanged += new System.EventHandler(this.txtInProgressDesc_TextChanged);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(291, 75);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(121, 13);
            this.label6.TabIndex = 32;
            this.label6.Text = "In Progress Description:";
            // 
            // txtEndDesc
            // 
            this.txtEndDesc.AcceptsReturn = true;
            this.txtEndDesc.Location = new System.Drawing.Point(436, 91);
            this.txtEndDesc.Multiline = true;
            this.txtEndDesc.Name = "txtEndDesc";
            this.txtEndDesc.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtEndDesc.Size = new System.Drawing.Size(136, 67);
            this.txtEndDesc.TabIndex = 28;
            this.txtEndDesc.TextChanged += new System.EventHandler(this.txtEndDesc_TextChanged);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(433, 75);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(128, 13);
            this.label5.TabIndex = 27;
            this.label5.Text = "Completed Description:";
            // 
            // txtStartDesc
            // 
            this.txtStartDesc.AcceptsReturn = true;
            this.txtStartDesc.Location = new System.Drawing.Point(152, 91);
            this.txtStartDesc.Multiline = true;
            this.txtStartDesc.Name = "txtStartDesc";
            this.txtStartDesc.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtStartDesc.Size = new System.Drawing.Size(136, 68);
            this.txtStartDesc.TabIndex = 26;
            this.txtStartDesc.TextChanged += new System.EventHandler(this.txtStartDesc_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(149, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 13);
            this.label2.TabIndex = 25;
            this.label2.Text = "Offer Description:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(10, 36);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(151, 20);
            this.txtName.TabIndex = 1;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
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
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.chkQuittable);
            this.groupBox5.Controls.Add(this.chkRepeatable);
            this.groupBox5.Location = new System.Drawing.Point(446, 9);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(126, 60);
            this.groupBox5.TabIndex = 34;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Progression Options";
            // 
            // chkQuittable
            // 
            this.chkQuittable.AutoSize = true;
            this.chkQuittable.Location = new System.Drawing.Point(6, 37);
            this.chkQuittable.Name = "chkQuittable";
            this.chkQuittable.Size = new System.Drawing.Size(104, 17);
            this.chkQuittable.TabIndex = 31;
            this.chkQuittable.Text = "Can Quit Quest?";
            this.chkQuittable.UseVisualStyleBackColor = true;
            this.chkQuittable.CheckedChanged += new System.EventHandler(this.chkQuittable_CheckedChanged);
            // 
            // chkRepeatable
            // 
            this.chkRepeatable.AutoSize = true;
            this.chkRepeatable.Location = new System.Drawing.Point(6, 19);
            this.chkRepeatable.Name = "chkRepeatable";
            this.chkRepeatable.Size = new System.Drawing.Size(118, 17);
            this.chkRepeatable.TabIndex = 30;
            this.chkRepeatable.Text = "Quest Repeatable?";
            this.chkRepeatable.UseVisualStyleBackColor = true;
            this.chkRepeatable.CheckedChanged += new System.EventHandler(this.chkRepeatable_CheckedChanged);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.btnShiftTaskDown);
            this.groupBox6.Controls.Add(this.btnShiftTaskUp);
            this.groupBox6.Controls.Add(this.btnRemoveTask);
            this.groupBox6.Controls.Add(this.lstTasks);
            this.groupBox6.Controls.Add(this.btnAddTask);
            this.groupBox6.Location = new System.Drawing.Point(277, 168);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(480, 201);
            this.groupBox6.TabIndex = 19;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Quest Tasks";
            // 
            // btnShiftTaskDown
            // 
            this.btnShiftTaskDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnShiftTaskDown.Location = new System.Drawing.Point(446, 58);
            this.btnShiftTaskDown.Name = "btnShiftTaskDown";
            this.btnShiftTaskDown.Size = new System.Drawing.Size(25, 33);
            this.btnShiftTaskDown.TabIndex = 7;
            this.btnShiftTaskDown.Text = "↓";
            this.btnShiftTaskDown.UseVisualStyleBackColor = true;
            this.btnShiftTaskDown.Click += new System.EventHandler(this.btnShiftTaskDown_Click);
            // 
            // btnShiftTaskUp
            // 
            this.btnShiftTaskUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnShiftTaskUp.Location = new System.Drawing.Point(446, 19);
            this.btnShiftTaskUp.Name = "btnShiftTaskUp";
            this.btnShiftTaskUp.Size = new System.Drawing.Size(25, 33);
            this.btnShiftTaskUp.TabIndex = 6;
            this.btnShiftTaskUp.Text = "↑";
            this.btnShiftTaskUp.UseVisualStyleBackColor = true;
            this.btnShiftTaskUp.Click += new System.EventHandler(this.btnShiftTaskUp_Click);
            // 
            // btnRemoveTask
            // 
            this.btnRemoveTask.Location = new System.Drawing.Point(317, 172);
            this.btnRemoveTask.Name = "btnRemoveTask";
            this.btnRemoveTask.Size = new System.Drawing.Size(123, 23);
            this.btnRemoveTask.TabIndex = 5;
            this.btnRemoveTask.Text = "Remove Task";
            this.btnRemoveTask.UseVisualStyleBackColor = true;
            this.btnRemoveTask.Click += new System.EventHandler(this.btnRemoveTask_Click);
            // 
            // lstTasks
            // 
            this.lstTasks.FormattingEnabled = true;
            this.lstTasks.HorizontalScrollbar = true;
            this.lstTasks.Location = new System.Drawing.Point(6, 19);
            this.lstTasks.Name = "lstTasks";
            this.lstTasks.ScrollAlwaysVisible = true;
            this.lstTasks.Size = new System.Drawing.Size(434, 134);
            this.lstTasks.TabIndex = 3;
            this.lstTasks.DoubleClick += new System.EventHandler(this.lstTasks_DoubleClick);
            // 
            // btnAddTask
            // 
            this.btnAddTask.Location = new System.Drawing.Point(6, 172);
            this.btnAddTask.Name = "btnAddTask";
            this.btnAddTask.Size = new System.Drawing.Size(123, 23);
            this.btnAddTask.TabIndex = 4;
            this.btnAddTask.Text = "Add Task";
            this.btnAddTask.UseVisualStyleBackColor = true;
            this.btnAddTask.Click += new System.EventHandler(this.btnAddTask_Click);
            // 
            // pnlContainer
            // 
            this.pnlContainer.Controls.Add(this.groupBox4);
            this.pnlContainer.Controls.Add(this.groupBox2);
            this.pnlContainer.Controls.Add(this.groupBox3);
            this.pnlContainer.Controls.Add(this.groupBox6);
            this.pnlContainer.Location = new System.Drawing.Point(221, 34);
            this.pnlContainer.Name = "pnlContainer";
            this.pnlContainer.Size = new System.Drawing.Size(765, 374);
            this.pnlContainer.TabIndex = 20;
            this.pnlContainer.Visible = false;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnEditCompletionEvent);
            this.groupBox4.Controls.Add(this.btnEditStartEvent);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Location = new System.Drawing.Point(586, 0);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(171, 164);
            this.groupBox4.TabIndex = 21;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Quest Actions:";
            // 
            // btnEditCompletionEvent
            // 
            this.btnEditCompletionEvent.Location = new System.Drawing.Point(10, 106);
            this.btnEditCompletionEvent.Name = "btnEditCompletionEvent";
            this.btnEditCompletionEvent.Size = new System.Drawing.Size(152, 23);
            this.btnEditCompletionEvent.TabIndex = 3;
            this.btnEditCompletionEvent.Text = "Edit Quest Completion Event";
            this.btnEditCompletionEvent.UseVisualStyleBackColor = true;
            this.btnEditCompletionEvent.Click += new System.EventHandler(this.btnEditCompletionEvent_Click);
            // 
            // btnEditStartEvent
            // 
            this.btnEditStartEvent.Location = new System.Drawing.Point(10, 37);
            this.btnEditStartEvent.Name = "btnEditStartEvent";
            this.btnEditStartEvent.Size = new System.Drawing.Size(152, 23);
            this.btnEditStartEvent.TabIndex = 2;
            this.btnEditStartEvent.Text = "Edit Quest Start Event";
            this.btnEditStartEvent.UseVisualStyleBackColor = true;
            this.btnEditStartEvent.Click += new System.EventHandler(this.btnEditStartEvent_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 86);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(110, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "On Quest Completion:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "On Quest Start:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnRemoveRequirement);
            this.groupBox3.Controls.Add(this.btnAddRequirement);
            this.groupBox3.Controls.Add(this.lstRequirements);
            this.groupBox3.Location = new System.Drawing.Point(0, 168);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(271, 201);
            this.groupBox3.TabIndex = 20;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Quest Requirements";
            // 
            // btnRemoveRequirement
            // 
            this.btnRemoveRequirement.Location = new System.Drawing.Point(135, 172);
            this.btnRemoveRequirement.Name = "btnRemoveRequirement";
            this.btnRemoveRequirement.Size = new System.Drawing.Size(123, 23);
            this.btnRemoveRequirement.TabIndex = 2;
            this.btnRemoveRequirement.Text = "Remove Requirement";
            this.btnRemoveRequirement.UseVisualStyleBackColor = true;
            this.btnRemoveRequirement.Click += new System.EventHandler(this.btnRemoveRequirement_Click);
            // 
            // btnAddRequirement
            // 
            this.btnAddRequirement.Location = new System.Drawing.Point(6, 172);
            this.btnAddRequirement.Name = "btnAddRequirement";
            this.btnAddRequirement.Size = new System.Drawing.Size(123, 23);
            this.btnAddRequirement.TabIndex = 1;
            this.btnAddRequirement.Text = "Add Requirement";
            this.btnAddRequirement.UseVisualStyleBackColor = true;
            this.btnAddRequirement.Click += new System.EventHandler(this.btnAddRequirement_Click);
            // 
            // lstRequirements
            // 
            this.lstRequirements.FormattingEnabled = true;
            this.lstRequirements.HorizontalScrollbar = true;
            this.lstRequirements.Location = new System.Drawing.Point(6, 17);
            this.lstRequirements.Name = "lstRequirements";
            this.lstRequirements.ScrollAlwaysVisible = true;
            this.lstRequirements.Size = new System.Drawing.Size(252, 134);
            this.lstRequirements.TabIndex = 0;
            this.lstRequirements.SelectedIndexChanged += new System.EventHandler(this.lstRequirements_SelectedIndexChanged);
            this.lstRequirements.DoubleClick += new System.EventHandler(this.lstRequirements_DoubleClick);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(796, 414);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(190, 27);
            this.btnCancel.TabIndex = 39;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(600, 414);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(190, 27);
            this.btnSave.TabIndex = 36;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripItemNew,
            this.toolStripSeparator1,
            this.toolStripItemDelete,
            this.toolStripSeparator2,
            this.toolStripItemCopy,
            this.toolStripItemPaste,
            this.toolStripSeparator3,
            this.toolStripItemUndo});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(992, 25);
            this.toolStrip.TabIndex = 40;
            this.toolStrip.Text = "toolStrip1";
            // 
            // toolStripItemNew
            // 
            this.toolStripItemNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripItemNew.Image = ((System.Drawing.Image)(resources.GetObject("toolStripItemNew.Image")));
            this.toolStripItemNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripItemNew.Name = "toolStripItemNew";
            this.toolStripItemNew.Size = new System.Drawing.Size(23, 22);
            this.toolStripItemNew.Text = "New";
            this.toolStripItemNew.Click += new System.EventHandler(this.toolStripItemNew_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripItemDelete
            // 
            this.toolStripItemDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripItemDelete.Enabled = false;
            this.toolStripItemDelete.Image = ((System.Drawing.Image)(resources.GetObject("toolStripItemDelete.Image")));
            this.toolStripItemDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripItemDelete.Name = "toolStripItemDelete";
            this.toolStripItemDelete.Size = new System.Drawing.Size(23, 22);
            this.toolStripItemDelete.Text = "Delete";
            this.toolStripItemDelete.Click += new System.EventHandler(this.toolStripItemDelete_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripItemCopy
            // 
            this.toolStripItemCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripItemCopy.Enabled = false;
            this.toolStripItemCopy.Image = ((System.Drawing.Image)(resources.GetObject("toolStripItemCopy.Image")));
            this.toolStripItemCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripItemCopy.Name = "toolStripItemCopy";
            this.toolStripItemCopy.Size = new System.Drawing.Size(23, 22);
            this.toolStripItemCopy.Text = "Copy";
            this.toolStripItemCopy.Click += new System.EventHandler(this.toolStripItemCopy_Click);
            // 
            // toolStripItemPaste
            // 
            this.toolStripItemPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripItemPaste.Enabled = false;
            this.toolStripItemPaste.Image = ((System.Drawing.Image)(resources.GetObject("toolStripItemPaste.Image")));
            this.toolStripItemPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripItemPaste.Name = "toolStripItemPaste";
            this.toolStripItemPaste.Size = new System.Drawing.Size(23, 22);
            this.toolStripItemPaste.Text = "Paste";
            this.toolStripItemPaste.Click += new System.EventHandler(this.toolStripItemPaste_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripItemUndo
            // 
            this.toolStripItemUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripItemUndo.Enabled = false;
            this.toolStripItemUndo.Image = ((System.Drawing.Image)(resources.GetObject("toolStripItemUndo.Image")));
            this.toolStripItemUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripItemUndo.Name = "toolStripItemUndo";
            this.toolStripItemUndo.Size = new System.Drawing.Size(23, 22);
            this.toolStripItemUndo.Text = "Undo";
            this.toolStripItemUndo.Click += new System.EventHandler(this.toolStripItemUndo_Click);
            // 
            // frmQuest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(992, 446);
            this.ControlBox = false;
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pnlContainer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MinimizeBox = false;
            this.Name = "frmQuest";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Quest Editor";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.form_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.pnlContainer.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox lstQuests;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox txtEndDesc;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtStartDesc;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel pnlContainer;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnRemoveRequirement;
        private System.Windows.Forms.Button btnAddRequirement;
        private System.Windows.Forms.ListBox lstRequirements;
        private System.Windows.Forms.CheckBox chkRepeatable;
        private System.Windows.Forms.CheckBox chkQuittable;
        private System.Windows.Forms.Button btnShiftTaskDown;
        private System.Windows.Forms.Button btnShiftTaskUp;
        private System.Windows.Forms.Button btnRemoveTask;
        private System.Windows.Forms.ListBox lstTasks;
        private System.Windows.Forms.Button btnAddTask;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnEditCompletionEvent;
        private System.Windows.Forms.Button btnEditStartEvent;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtInProgressDesc;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.CheckBox chkLogAfterComplete;
        private System.Windows.Forms.CheckBox chkLogBeforeOffer;
        private System.Windows.Forms.TextBox txtBeforeDesc;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripItemNew;
        private System.Windows.Forms.ToolStripButton toolStripItemDelete;
        public System.Windows.Forms.ToolStripButton toolStripItemCopy;
        public System.Windows.Forms.ToolStripButton toolStripItemPaste;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        public System.Windows.Forms.ToolStripButton toolStripItemUndo;
    }
}