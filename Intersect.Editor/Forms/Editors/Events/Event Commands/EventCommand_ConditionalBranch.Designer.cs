using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    partial class EventCommandConditionalBranch
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
            this.grpConditional = new DarkUI.Controls.DarkGroupBox();
            this.grpMapZoneType = new DarkUI.Controls.DarkGroupBox();
            this.lblMapZoneType = new System.Windows.Forms.Label();
            this.cmbMapZoneType = new DarkUI.Controls.DarkComboBox();
            this.grpInGuild = new DarkUI.Controls.DarkGroupBox();
            this.lblRank = new System.Windows.Forms.Label();
            this.cmbRank = new DarkUI.Controls.DarkComboBox();
            this.chkHasElse = new DarkUI.Controls.DarkCheckBox();
            this.chkNegated = new DarkUI.Controls.DarkCheckBox();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.cmbConditionType = new DarkUI.Controls.DarkComboBox();
            this.grpQuestCompleted = new DarkUI.Controls.DarkGroupBox();
            this.lblQuestCompleted = new System.Windows.Forms.Label();
            this.cmbCompletedQuest = new DarkUI.Controls.DarkComboBox();
            this.lblType = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.grpVariable = new DarkUI.Controls.DarkGroupBox();
            this.grpSelectVariable = new DarkUI.Controls.DarkGroupBox();
            this.rdoPlayerVariable = new DarkUI.Controls.DarkRadioButton();
            this.cmbVariable = new DarkUI.Controls.DarkComboBox();
            this.rdoGlobalVariable = new DarkUI.Controls.DarkRadioButton();
            this.grpStringVariable = new DarkUI.Controls.DarkGroupBox();
            this.lblStringTextVariables = new System.Windows.Forms.Label();
            this.lblStringComparatorValue = new System.Windows.Forms.Label();
            this.txtStringValue = new DarkUI.Controls.DarkTextBox();
            this.cmbStringComparitor = new DarkUI.Controls.DarkComboBox();
            this.lblStringComparator = new System.Windows.Forms.Label();
            this.grpBooleanVariable = new DarkUI.Controls.DarkGroupBox();
            this.optBooleanTrue = new DarkUI.Controls.DarkRadioButton();
            this.optBooleanFalse = new DarkUI.Controls.DarkRadioButton();
            this.cmbBooleanComparator = new DarkUI.Controls.DarkComboBox();
            this.lblBooleanComparator = new System.Windows.Forms.Label();
            this.cmbBooleanGlobalVariable = new DarkUI.Controls.DarkComboBox();
            this.cmbBooleanPlayerVariable = new DarkUI.Controls.DarkComboBox();
            this.optBooleanPlayerVariable = new DarkUI.Controls.DarkRadioButton();
            this.optBooleanGlobalVariable = new DarkUI.Controls.DarkRadioButton();
            this.grpNumericVariable = new DarkUI.Controls.DarkGroupBox();
            this.cmbNumericComparitor = new DarkUI.Controls.DarkComboBox();
            this.nudVariableValue = new DarkUI.Controls.DarkNumericUpDown();
            this.lblNumericComparator = new System.Windows.Forms.Label();
            this.cmbCompareGlobalVar = new DarkUI.Controls.DarkComboBox();
            this.rdoVarCompareStaticValue = new DarkUI.Controls.DarkRadioButton();
            this.cmbComparePlayerVar = new DarkUI.Controls.DarkComboBox();
            this.rdoVarComparePlayerVar = new DarkUI.Controls.DarkRadioButton();
            this.rdoVarCompareGlobalVar = new DarkUI.Controls.DarkRadioButton();
            this.grpQuestInProgress = new DarkUI.Controls.DarkGroupBox();
            this.lblQuestTask = new System.Windows.Forms.Label();
            this.cmbQuestTask = new DarkUI.Controls.DarkComboBox();
            this.cmbTaskModifier = new DarkUI.Controls.DarkComboBox();
            this.lblQuestIs = new System.Windows.Forms.Label();
            this.lblQuestProgress = new System.Windows.Forms.Label();
            this.cmbQuestInProgress = new DarkUI.Controls.DarkComboBox();
            this.grpStartQuest = new DarkUI.Controls.DarkGroupBox();
            this.lblStartQuest = new System.Windows.Forms.Label();
            this.cmbStartQuest = new DarkUI.Controls.DarkComboBox();
            this.grpTime = new DarkUI.Controls.DarkGroupBox();
            this.lblEndRange = new System.Windows.Forms.Label();
            this.lblStartRange = new System.Windows.Forms.Label();
            this.cmbTime2 = new DarkUI.Controls.DarkComboBox();
            this.cmbTime1 = new DarkUI.Controls.DarkComboBox();
            this.lblAnd = new System.Windows.Forms.Label();
            this.grpPowerIs = new DarkUI.Controls.DarkGroupBox();
            this.cmbPower = new DarkUI.Controls.DarkComboBox();
            this.lblPower = new System.Windows.Forms.Label();
            this.grpSelfSwitch = new DarkUI.Controls.DarkGroupBox();
            this.cmbSelfSwitchVal = new DarkUI.Controls.DarkComboBox();
            this.lblSelfSwitchIs = new System.Windows.Forms.Label();
            this.cmbSelfSwitch = new DarkUI.Controls.DarkComboBox();
            this.lblSelfSwitch = new System.Windows.Forms.Label();
            this.grpSpell = new DarkUI.Controls.DarkGroupBox();
            this.cmbSpell = new DarkUI.Controls.DarkComboBox();
            this.lblSpell = new System.Windows.Forms.Label();
            this.grpClass = new DarkUI.Controls.DarkGroupBox();
            this.cmbClass = new DarkUI.Controls.DarkComboBox();
            this.lblClass = new System.Windows.Forms.Label();
            this.grpLevelStat = new DarkUI.Controls.DarkGroupBox();
            this.chkStatIgnoreBuffs = new DarkUI.Controls.DarkCheckBox();
            this.nudLevelStatValue = new DarkUI.Controls.DarkNumericUpDown();
            this.cmbLevelStat = new DarkUI.Controls.DarkComboBox();
            this.lblLevelOrStat = new System.Windows.Forms.Label();
            this.lblLvlStatValue = new System.Windows.Forms.Label();
            this.cmbLevelComparator = new DarkUI.Controls.DarkComboBox();
            this.lblLevelComparator = new System.Windows.Forms.Label();
            this.grpMapIs = new DarkUI.Controls.DarkGroupBox();
            this.btnSelectMap = new DarkUI.Controls.DarkButton();
            this.grpGender = new DarkUI.Controls.DarkGroupBox();
            this.cmbGender = new DarkUI.Controls.DarkComboBox();
            this.lblGender = new System.Windows.Forms.Label();
            this.grpEquippedItem = new DarkUI.Controls.DarkGroupBox();
            this.cmbEquippedItem = new DarkUI.Controls.DarkComboBox();
            this.lblEquippedItem = new System.Windows.Forms.Label();
            this.grpInventoryConditions = new DarkUI.Controls.DarkGroupBox();
            this.grpVariableAmount = new DarkUI.Controls.DarkGroupBox();
            this.cmbInvVariable = new DarkUI.Controls.DarkComboBox();
            this.lblInvVariable = new System.Windows.Forms.Label();
            this.rdoInvGlobalVariable = new DarkUI.Controls.DarkRadioButton();
            this.rdoInvPlayerVariable = new DarkUI.Controls.DarkRadioButton();
            this.grpManualAmount = new DarkUI.Controls.DarkGroupBox();
            this.nudItemAmount = new DarkUI.Controls.DarkNumericUpDown();
            this.lblItemQuantity = new System.Windows.Forms.Label();
            this.grpAmountType = new DarkUI.Controls.DarkGroupBox();
            this.rdoVariable = new DarkUI.Controls.DarkRadioButton();
            this.rdoManual = new DarkUI.Controls.DarkRadioButton();
            this.cmbItem = new DarkUI.Controls.DarkComboBox();
            this.lblItem = new System.Windows.Forms.Label();
            this.grpConditional.SuspendLayout();
            this.grpMapZoneType.SuspendLayout();
            this.grpInGuild.SuspendLayout();
            this.grpQuestCompleted.SuspendLayout();
            this.grpVariable.SuspendLayout();
            this.grpSelectVariable.SuspendLayout();
            this.grpStringVariable.SuspendLayout();
            this.grpBooleanVariable.SuspendLayout();
            this.grpNumericVariable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudVariableValue)).BeginInit();
            this.grpQuestInProgress.SuspendLayout();
            this.grpStartQuest.SuspendLayout();
            this.grpTime.SuspendLayout();
            this.grpPowerIs.SuspendLayout();
            this.grpSelfSwitch.SuspendLayout();
            this.grpSpell.SuspendLayout();
            this.grpClass.SuspendLayout();
            this.grpLevelStat.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLevelStatValue)).BeginInit();
            this.grpMapIs.SuspendLayout();
            this.grpGender.SuspendLayout();
            this.grpEquippedItem.SuspendLayout();
            this.grpInventoryConditions.SuspendLayout();
            this.grpVariableAmount.SuspendLayout();
            this.grpManualAmount.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudItemAmount)).BeginInit();
            this.grpAmountType.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpConditional
            // 
            this.grpConditional.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpConditional.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpConditional.Controls.Add(this.grpMapZoneType);
            this.grpConditional.Controls.Add(this.grpInGuild);
            this.grpConditional.Controls.Add(this.chkHasElse);
            this.grpConditional.Controls.Add(this.chkNegated);
            this.grpConditional.Controls.Add(this.btnSave);
            this.grpConditional.Controls.Add(this.cmbConditionType);
            this.grpConditional.Controls.Add(this.grpQuestCompleted);
            this.grpConditional.Controls.Add(this.lblType);
            this.grpConditional.Controls.Add(this.btnCancel);
            this.grpConditional.Controls.Add(this.grpVariable);
            this.grpConditional.Controls.Add(this.grpQuestInProgress);
            this.grpConditional.Controls.Add(this.grpStartQuest);
            this.grpConditional.Controls.Add(this.grpTime);
            this.grpConditional.Controls.Add(this.grpPowerIs);
            this.grpConditional.Controls.Add(this.grpSelfSwitch);
            this.grpConditional.Controls.Add(this.grpSpell);
            this.grpConditional.Controls.Add(this.grpClass);
            this.grpConditional.Controls.Add(this.grpLevelStat);
            this.grpConditional.Controls.Add(this.grpMapIs);
            this.grpConditional.Controls.Add(this.grpGender);
            this.grpConditional.Controls.Add(this.grpEquippedItem);
            this.grpConditional.Controls.Add(this.grpInventoryConditions);
            this.grpConditional.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpConditional.Location = new System.Drawing.Point(3, 3);
            this.grpConditional.Name = "grpConditional";
            this.grpConditional.Size = new System.Drawing.Size(278, 337);
            this.grpConditional.TabIndex = 17;
            this.grpConditional.TabStop = false;
            this.grpConditional.Text = "Conditional";
            // 
            // grpMapZoneType
            // 
            this.grpMapZoneType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpMapZoneType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpMapZoneType.Controls.Add(this.lblMapZoneType);
            this.grpMapZoneType.Controls.Add(this.cmbMapZoneType);
            this.grpMapZoneType.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpMapZoneType.Location = new System.Drawing.Point(8, 39);
            this.grpMapZoneType.Name = "grpMapZoneType";
            this.grpMapZoneType.Size = new System.Drawing.Size(262, 71);
            this.grpMapZoneType.TabIndex = 58;
            this.grpMapZoneType.TabStop = false;
            this.grpMapZoneType.Text = "Map Zone Type Is:";
            this.grpMapZoneType.Visible = false;
            // 
            // lblMapZoneType
            // 
            this.lblMapZoneType.AutoSize = true;
            this.lblMapZoneType.Location = new System.Drawing.Point(6, 21);
            this.lblMapZoneType.Name = "lblMapZoneType";
            this.lblMapZoneType.Size = new System.Drawing.Size(86, 13);
            this.lblMapZoneType.TabIndex = 5;
            this.lblMapZoneType.Text = "Map Zone Type:";
            // 
            // cmbMapZoneType
            // 
            this.cmbMapZoneType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbMapZoneType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbMapZoneType.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbMapZoneType.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbMapZoneType.DrawDropdownHoverOutline = false;
            this.cmbMapZoneType.DrawFocusRectangle = false;
            this.cmbMapZoneType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbMapZoneType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMapZoneType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbMapZoneType.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbMapZoneType.FormattingEnabled = true;
            this.cmbMapZoneType.Location = new System.Drawing.Point(92, 18);
            this.cmbMapZoneType.Name = "cmbMapZoneType";
            this.cmbMapZoneType.Size = new System.Drawing.Size(162, 21);
            this.cmbMapZoneType.TabIndex = 3;
            this.cmbMapZoneType.Text = null;
            this.cmbMapZoneType.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // grpInGuild
            // 
            this.grpInGuild.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpInGuild.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpInGuild.Controls.Add(this.lblRank);
            this.grpInGuild.Controls.Add(this.cmbRank);
            this.grpInGuild.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpInGuild.Location = new System.Drawing.Point(9, 40);
            this.grpInGuild.Name = "grpInGuild";
            this.grpInGuild.Size = new System.Drawing.Size(262, 71);
            this.grpInGuild.TabIndex = 33;
            this.grpInGuild.TabStop = false;
            this.grpInGuild.Text = "In Guild With At Least Rank:";
            this.grpInGuild.Visible = false;
            // 
            // lblRank
            // 
            this.lblRank.AutoSize = true;
            this.lblRank.Location = new System.Drawing.Point(6, 21);
            this.lblRank.Name = "lblRank";
            this.lblRank.Size = new System.Drawing.Size(36, 13);
            this.lblRank.TabIndex = 5;
            this.lblRank.Text = "Rank:";
            // 
            // cmbRank
            // 
            this.cmbRank.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbRank.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbRank.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbRank.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbRank.DrawDropdownHoverOutline = false;
            this.cmbRank.DrawFocusRectangle = false;
            this.cmbRank.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbRank.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRank.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbRank.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbRank.FormattingEnabled = true;
            this.cmbRank.Location = new System.Drawing.Point(92, 18);
            this.cmbRank.Name = "cmbRank";
            this.cmbRank.Size = new System.Drawing.Size(162, 21);
            this.cmbRank.TabIndex = 3;
            this.cmbRank.Text = null;
            this.cmbRank.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // chkHasElse
            // 
            this.chkHasElse.Location = new System.Drawing.Point(112, 279);
            this.chkHasElse.Name = "chkHasElse";
            this.chkHasElse.Size = new System.Drawing.Size(72, 17);
            this.chkHasElse.TabIndex = 56;
            this.chkHasElse.Text = "Has Else";
            // 
            // chkNegated
            // 
            this.chkNegated.Location = new System.Drawing.Point(199, 279);
            this.chkNegated.Name = "chkNegated";
            this.chkNegated.Size = new System.Drawing.Size(72, 17);
            this.chkNegated.TabIndex = 34;
            this.chkNegated.Text = "Negated";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(12, 307);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(5);
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Ok";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // cmbConditionType
            // 
            this.cmbConditionType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbConditionType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbConditionType.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbConditionType.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbConditionType.DrawDropdownHoverOutline = false;
            this.cmbConditionType.DrawFocusRectangle = false;
            this.cmbConditionType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbConditionType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbConditionType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbConditionType.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbConditionType.FormattingEnabled = true;
            this.cmbConditionType.Items.AddRange(new object[] {
            "Variable is...",
            "Has item...",
            "Class is...",
            "Knows spell...",
            "Level is....",
            "Self Switch is....",
            "Power level is....",
            "Time is between....",
            "Can Start Quest....",
            "Quest In Progress....",
            "Quest Completed....",
            "Player death...",
            "No NPCs on the map...",
            "Gender is...",
            "Item Equipped Is...",
            "Has X free Inventory slots...",
            "In Guild With At Least Rank..."});
            this.cmbConditionType.Location = new System.Drawing.Point(88, 13);
            this.cmbConditionType.Name = "cmbConditionType";
            this.cmbConditionType.Size = new System.Drawing.Size(183, 21);
            this.cmbConditionType.TabIndex = 22;
            this.cmbConditionType.Text = "Variable is...";
            this.cmbConditionType.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbConditionType.SelectedIndexChanged += new System.EventHandler(this.cmbConditionType_SelectedIndexChanged);
            // 
            // grpQuestCompleted
            // 
            this.grpQuestCompleted.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpQuestCompleted.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpQuestCompleted.Controls.Add(this.lblQuestCompleted);
            this.grpQuestCompleted.Controls.Add(this.cmbCompletedQuest);
            this.grpQuestCompleted.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpQuestCompleted.Location = new System.Drawing.Point(9, 40);
            this.grpQuestCompleted.Name = "grpQuestCompleted";
            this.grpQuestCompleted.Size = new System.Drawing.Size(262, 71);
            this.grpQuestCompleted.TabIndex = 32;
            this.grpQuestCompleted.TabStop = false;
            this.grpQuestCompleted.Text = "Quest Completed:";
            this.grpQuestCompleted.Visible = false;
            // 
            // lblQuestCompleted
            // 
            this.lblQuestCompleted.AutoSize = true;
            this.lblQuestCompleted.Location = new System.Drawing.Point(6, 21);
            this.lblQuestCompleted.Name = "lblQuestCompleted";
            this.lblQuestCompleted.Size = new System.Drawing.Size(38, 13);
            this.lblQuestCompleted.TabIndex = 5;
            this.lblQuestCompleted.Text = "Quest:";
            // 
            // cmbCompletedQuest
            // 
            this.cmbCompletedQuest.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbCompletedQuest.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbCompletedQuest.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbCompletedQuest.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbCompletedQuest.DrawDropdownHoverOutline = false;
            this.cmbCompletedQuest.DrawFocusRectangle = false;
            this.cmbCompletedQuest.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbCompletedQuest.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCompletedQuest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbCompletedQuest.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbCompletedQuest.FormattingEnabled = true;
            this.cmbCompletedQuest.Location = new System.Drawing.Point(92, 18);
            this.cmbCompletedQuest.Name = "cmbCompletedQuest";
            this.cmbCompletedQuest.Size = new System.Drawing.Size(162, 21);
            this.cmbCompletedQuest.TabIndex = 3;
            this.cmbCompletedQuest.Text = null;
            this.cmbCompletedQuest.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(6, 16);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(81, 13);
            this.lblType.TabIndex = 21;
            this.lblType.Text = "Condition Type:";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(113, 307);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // grpVariable
            // 
            this.grpVariable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpVariable.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpVariable.Controls.Add(this.grpSelectVariable);
            this.grpVariable.Controls.Add(this.grpStringVariable);
            this.grpVariable.Controls.Add(this.grpBooleanVariable);
            this.grpVariable.Controls.Add(this.grpNumericVariable);
            this.grpVariable.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpVariable.Location = new System.Drawing.Point(9, 40);
            this.grpVariable.Name = "grpVariable";
            this.grpVariable.Size = new System.Drawing.Size(262, 233);
            this.grpVariable.TabIndex = 24;
            this.grpVariable.TabStop = false;
            this.grpVariable.Text = "Variable is...";
            // 
            // grpSelectVariable
            // 
            this.grpSelectVariable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpSelectVariable.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpSelectVariable.Controls.Add(this.rdoPlayerVariable);
            this.grpSelectVariable.Controls.Add(this.cmbVariable);
            this.grpSelectVariable.Controls.Add(this.rdoGlobalVariable);
            this.grpSelectVariable.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpSelectVariable.Location = new System.Drawing.Point(7, 16);
            this.grpSelectVariable.Name = "grpSelectVariable";
            this.grpSelectVariable.Size = new System.Drawing.Size(247, 75);
            this.grpSelectVariable.TabIndex = 50;
            this.grpSelectVariable.TabStop = false;
            this.grpSelectVariable.Text = "Select Variable";
            // 
            // rdoPlayerVariable
            // 
            this.rdoPlayerVariable.AutoSize = true;
            this.rdoPlayerVariable.Checked = true;
            this.rdoPlayerVariable.Location = new System.Drawing.Point(6, 19);
            this.rdoPlayerVariable.Name = "rdoPlayerVariable";
            this.rdoPlayerVariable.Size = new System.Drawing.Size(95, 17);
            this.rdoPlayerVariable.TabIndex = 34;
            this.rdoPlayerVariable.TabStop = true;
            this.rdoPlayerVariable.Text = "Player Variable";
            this.rdoPlayerVariable.CheckedChanged += new System.EventHandler(this.rdoPlayerVariable_CheckedChanged);
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
            this.cmbVariable.Location = new System.Drawing.Point(6, 42);
            this.cmbVariable.Name = "cmbVariable";
            this.cmbVariable.Size = new System.Drawing.Size(235, 21);
            this.cmbVariable.TabIndex = 22;
            this.cmbVariable.Text = null;
            this.cmbVariable.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbVariable.SelectedIndexChanged += new System.EventHandler(this.cmbVariable_SelectedIndexChanged);
            // 
            // rdoGlobalVariable
            // 
            this.rdoGlobalVariable.AutoSize = true;
            this.rdoGlobalVariable.Location = new System.Drawing.Point(145, 19);
            this.rdoGlobalVariable.Name = "rdoGlobalVariable";
            this.rdoGlobalVariable.Size = new System.Drawing.Size(96, 17);
            this.rdoGlobalVariable.TabIndex = 35;
            this.rdoGlobalVariable.Text = "Global Variable";
            this.rdoGlobalVariable.CheckedChanged += new System.EventHandler(this.rdoGlobalVariable_CheckedChanged);
            // 
            // grpStringVariable
            // 
            this.grpStringVariable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpStringVariable.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpStringVariable.Controls.Add(this.lblStringTextVariables);
            this.grpStringVariable.Controls.Add(this.lblStringComparatorValue);
            this.grpStringVariable.Controls.Add(this.txtStringValue);
            this.grpStringVariable.Controls.Add(this.cmbStringComparitor);
            this.grpStringVariable.Controls.Add(this.lblStringComparator);
            this.grpStringVariable.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpStringVariable.Location = new System.Drawing.Point(6, 96);
            this.grpStringVariable.Name = "grpStringVariable";
            this.grpStringVariable.Size = new System.Drawing.Size(247, 134);
            this.grpStringVariable.TabIndex = 53;
            this.grpStringVariable.TabStop = false;
            this.grpStringVariable.Text = "String Variable:";
            // 
            // lblStringTextVariables
            // 
            this.lblStringTextVariables.AutoSize = true;
            this.lblStringTextVariables.BackColor = System.Drawing.Color.Transparent;
            this.lblStringTextVariables.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStringTextVariables.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.lblStringTextVariables.Location = new System.Drawing.Point(8, 109);
            this.lblStringTextVariables.Name = "lblStringTextVariables";
            this.lblStringTextVariables.Size = new System.Drawing.Size(218, 13);
            this.lblStringTextVariables.TabIndex = 69;
            this.lblStringTextVariables.Text = "Text variables work here. Click here for a list!";
            this.lblStringTextVariables.Click += new System.EventHandler(this.lblStringTextVariables_Click);
            // 
            // lblStringComparatorValue
            // 
            this.lblStringComparatorValue.AutoSize = true;
            this.lblStringComparatorValue.Location = new System.Drawing.Point(9, 52);
            this.lblStringComparatorValue.Name = "lblStringComparatorValue";
            this.lblStringComparatorValue.Size = new System.Drawing.Size(37, 13);
            this.lblStringComparatorValue.TabIndex = 63;
            this.lblStringComparatorValue.Text = "Value:";
            // 
            // txtStringValue
            // 
            this.txtStringValue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtStringValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtStringValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtStringValue.Location = new System.Drawing.Point(87, 50);
            this.txtStringValue.Name = "txtStringValue";
            this.txtStringValue.Size = new System.Drawing.Size(153, 20);
            this.txtStringValue.TabIndex = 62;
            // 
            // cmbStringComparitor
            // 
            this.cmbStringComparitor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbStringComparitor.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbStringComparitor.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbStringComparitor.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbStringComparitor.DrawDropdownHoverOutline = false;
            this.cmbStringComparitor.DrawFocusRectangle = false;
            this.cmbStringComparitor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbStringComparitor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStringComparitor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbStringComparitor.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbStringComparitor.FormattingEnabled = true;
            this.cmbStringComparitor.Items.AddRange(new object[] {
            "Equal To",
            "Contains"});
            this.cmbStringComparitor.Location = new System.Drawing.Point(87, 20);
            this.cmbStringComparitor.Name = "cmbStringComparitor";
            this.cmbStringComparitor.Size = new System.Drawing.Size(153, 21);
            this.cmbStringComparitor.TabIndex = 3;
            this.cmbStringComparitor.Text = "Equal To";
            this.cmbStringComparitor.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // lblStringComparator
            // 
            this.lblStringComparator.AutoSize = true;
            this.lblStringComparator.Location = new System.Drawing.Point(9, 23);
            this.lblStringComparator.Name = "lblStringComparator";
            this.lblStringComparator.Size = new System.Drawing.Size(64, 13);
            this.lblStringComparator.TabIndex = 2;
            this.lblStringComparator.Text = "Comparator:";
            // 
            // grpBooleanVariable
            // 
            this.grpBooleanVariable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpBooleanVariable.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpBooleanVariable.Controls.Add(this.optBooleanTrue);
            this.grpBooleanVariable.Controls.Add(this.optBooleanFalse);
            this.grpBooleanVariable.Controls.Add(this.cmbBooleanComparator);
            this.grpBooleanVariable.Controls.Add(this.lblBooleanComparator);
            this.grpBooleanVariable.Controls.Add(this.cmbBooleanGlobalVariable);
            this.grpBooleanVariable.Controls.Add(this.cmbBooleanPlayerVariable);
            this.grpBooleanVariable.Controls.Add(this.optBooleanPlayerVariable);
            this.grpBooleanVariable.Controls.Add(this.optBooleanGlobalVariable);
            this.grpBooleanVariable.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpBooleanVariable.Location = new System.Drawing.Point(8, 95);
            this.grpBooleanVariable.Name = "grpBooleanVariable";
            this.grpBooleanVariable.Size = new System.Drawing.Size(247, 134);
            this.grpBooleanVariable.TabIndex = 52;
            this.grpBooleanVariable.TabStop = false;
            this.grpBooleanVariable.Text = "Boolean Variable:";
            // 
            // optBooleanTrue
            // 
            this.optBooleanTrue.AutoSize = true;
            this.optBooleanTrue.Location = new System.Drawing.Point(10, 48);
            this.optBooleanTrue.Name = "optBooleanTrue";
            this.optBooleanTrue.Size = new System.Drawing.Size(47, 17);
            this.optBooleanTrue.TabIndex = 50;
            this.optBooleanTrue.Text = "True";
            // 
            // optBooleanFalse
            // 
            this.optBooleanFalse.AutoSize = true;
            this.optBooleanFalse.Location = new System.Drawing.Point(72, 48);
            this.optBooleanFalse.Name = "optBooleanFalse";
            this.optBooleanFalse.Size = new System.Drawing.Size(50, 17);
            this.optBooleanFalse.TabIndex = 49;
            this.optBooleanFalse.Text = "False";
            // 
            // cmbBooleanComparator
            // 
            this.cmbBooleanComparator.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbBooleanComparator.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbBooleanComparator.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbBooleanComparator.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbBooleanComparator.DrawDropdownHoverOutline = false;
            this.cmbBooleanComparator.DrawFocusRectangle = false;
            this.cmbBooleanComparator.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbBooleanComparator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBooleanComparator.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbBooleanComparator.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbBooleanComparator.FormattingEnabled = true;
            this.cmbBooleanComparator.Items.AddRange(new object[] {
            "Equal To",
            "Not Equal To"});
            this.cmbBooleanComparator.Location = new System.Drawing.Point(115, 20);
            this.cmbBooleanComparator.Name = "cmbBooleanComparator";
            this.cmbBooleanComparator.Size = new System.Drawing.Size(125, 21);
            this.cmbBooleanComparator.TabIndex = 3;
            this.cmbBooleanComparator.Text = "Equal To";
            this.cmbBooleanComparator.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // lblBooleanComparator
            // 
            this.lblBooleanComparator.AutoSize = true;
            this.lblBooleanComparator.Location = new System.Drawing.Point(9, 23);
            this.lblBooleanComparator.Name = "lblBooleanComparator";
            this.lblBooleanComparator.Size = new System.Drawing.Size(61, 13);
            this.lblBooleanComparator.TabIndex = 2;
            this.lblBooleanComparator.Text = "Comparator";
            // 
            // cmbBooleanGlobalVariable
            // 
            this.cmbBooleanGlobalVariable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbBooleanGlobalVariable.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbBooleanGlobalVariable.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbBooleanGlobalVariable.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbBooleanGlobalVariable.DrawDropdownHoverOutline = false;
            this.cmbBooleanGlobalVariable.DrawFocusRectangle = false;
            this.cmbBooleanGlobalVariable.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbBooleanGlobalVariable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBooleanGlobalVariable.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbBooleanGlobalVariable.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbBooleanGlobalVariable.FormattingEnabled = true;
            this.cmbBooleanGlobalVariable.Location = new System.Drawing.Point(146, 102);
            this.cmbBooleanGlobalVariable.Name = "cmbBooleanGlobalVariable";
            this.cmbBooleanGlobalVariable.Size = new System.Drawing.Size(94, 21);
            this.cmbBooleanGlobalVariable.TabIndex = 48;
            this.cmbBooleanGlobalVariable.Text = null;
            this.cmbBooleanGlobalVariable.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // cmbBooleanPlayerVariable
            // 
            this.cmbBooleanPlayerVariable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbBooleanPlayerVariable.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbBooleanPlayerVariable.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbBooleanPlayerVariable.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbBooleanPlayerVariable.DrawDropdownHoverOutline = false;
            this.cmbBooleanPlayerVariable.DrawFocusRectangle = false;
            this.cmbBooleanPlayerVariable.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbBooleanPlayerVariable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBooleanPlayerVariable.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbBooleanPlayerVariable.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbBooleanPlayerVariable.FormattingEnabled = true;
            this.cmbBooleanPlayerVariable.Location = new System.Drawing.Point(146, 75);
            this.cmbBooleanPlayerVariable.Name = "cmbBooleanPlayerVariable";
            this.cmbBooleanPlayerVariable.Size = new System.Drawing.Size(94, 21);
            this.cmbBooleanPlayerVariable.TabIndex = 47;
            this.cmbBooleanPlayerVariable.Text = null;
            this.cmbBooleanPlayerVariable.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // optBooleanPlayerVariable
            // 
            this.optBooleanPlayerVariable.AutoSize = true;
            this.optBooleanPlayerVariable.Location = new System.Drawing.Point(10, 76);
            this.optBooleanPlayerVariable.Name = "optBooleanPlayerVariable";
            this.optBooleanPlayerVariable.Size = new System.Drawing.Size(128, 17);
            this.optBooleanPlayerVariable.TabIndex = 45;
            this.optBooleanPlayerVariable.Text = "Player Variable Value:";
            // 
            // optBooleanGlobalVariable
            // 
            this.optBooleanGlobalVariable.AutoSize = true;
            this.optBooleanGlobalVariable.Location = new System.Drawing.Point(10, 103);
            this.optBooleanGlobalVariable.Name = "optBooleanGlobalVariable";
            this.optBooleanGlobalVariable.Size = new System.Drawing.Size(129, 17);
            this.optBooleanGlobalVariable.TabIndex = 46;
            this.optBooleanGlobalVariable.Text = "Global Variable Value:";
            // 
            // grpNumericVariable
            // 
            this.grpNumericVariable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpNumericVariable.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpNumericVariable.Controls.Add(this.cmbNumericComparitor);
            this.grpNumericVariable.Controls.Add(this.nudVariableValue);
            this.grpNumericVariable.Controls.Add(this.lblNumericComparator);
            this.grpNumericVariable.Controls.Add(this.cmbCompareGlobalVar);
            this.grpNumericVariable.Controls.Add(this.rdoVarCompareStaticValue);
            this.grpNumericVariable.Controls.Add(this.cmbComparePlayerVar);
            this.grpNumericVariable.Controls.Add(this.rdoVarComparePlayerVar);
            this.grpNumericVariable.Controls.Add(this.rdoVarCompareGlobalVar);
            this.grpNumericVariable.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpNumericVariable.Location = new System.Drawing.Point(8, 95);
            this.grpNumericVariable.Name = "grpNumericVariable";
            this.grpNumericVariable.Size = new System.Drawing.Size(247, 134);
            this.grpNumericVariable.TabIndex = 51;
            this.grpNumericVariable.TabStop = false;
            this.grpNumericVariable.Text = "Numeric Variable:";
            // 
            // cmbNumericComparitor
            // 
            this.cmbNumericComparitor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbNumericComparitor.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbNumericComparitor.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbNumericComparitor.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbNumericComparitor.DrawDropdownHoverOutline = false;
            this.cmbNumericComparitor.DrawFocusRectangle = false;
            this.cmbNumericComparitor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbNumericComparitor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbNumericComparitor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbNumericComparitor.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbNumericComparitor.FormattingEnabled = true;
            this.cmbNumericComparitor.Items.AddRange(new object[] {
            "Equal To",
            "Greater Than or Equal To",
            "Less Than or Equal To",
            "Greater Than",
            "Less Than",
            "Does Not Equal"});
            this.cmbNumericComparitor.Location = new System.Drawing.Point(115, 20);
            this.cmbNumericComparitor.Name = "cmbNumericComparitor";
            this.cmbNumericComparitor.Size = new System.Drawing.Size(125, 21);
            this.cmbNumericComparitor.TabIndex = 3;
            this.cmbNumericComparitor.Text = "Equal To";
            this.cmbNumericComparitor.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // nudVariableValue
            // 
            this.nudVariableValue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudVariableValue.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudVariableValue.Location = new System.Drawing.Point(115, 48);
            this.nudVariableValue.Maximum = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.nudVariableValue.Minimum = new decimal(new int[] {
            -1,
            -1,
            -1,
            -2147483648});
            this.nudVariableValue.Name = "nudVariableValue";
            this.nudVariableValue.Size = new System.Drawing.Size(125, 20);
            this.nudVariableValue.TabIndex = 49;
            this.nudVariableValue.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // lblNumericComparator
            // 
            this.lblNumericComparator.AutoSize = true;
            this.lblNumericComparator.Location = new System.Drawing.Point(9, 23);
            this.lblNumericComparator.Name = "lblNumericComparator";
            this.lblNumericComparator.Size = new System.Drawing.Size(61, 13);
            this.lblNumericComparator.TabIndex = 2;
            this.lblNumericComparator.Text = "Comparator";
            // 
            // cmbCompareGlobalVar
            // 
            this.cmbCompareGlobalVar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbCompareGlobalVar.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbCompareGlobalVar.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbCompareGlobalVar.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbCompareGlobalVar.DrawDropdownHoverOutline = false;
            this.cmbCompareGlobalVar.DrawFocusRectangle = false;
            this.cmbCompareGlobalVar.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbCompareGlobalVar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCompareGlobalVar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbCompareGlobalVar.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbCompareGlobalVar.FormattingEnabled = true;
            this.cmbCompareGlobalVar.Location = new System.Drawing.Point(146, 102);
            this.cmbCompareGlobalVar.Name = "cmbCompareGlobalVar";
            this.cmbCompareGlobalVar.Size = new System.Drawing.Size(94, 21);
            this.cmbCompareGlobalVar.TabIndex = 48;
            this.cmbCompareGlobalVar.Text = null;
            this.cmbCompareGlobalVar.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // rdoVarCompareStaticValue
            // 
            this.rdoVarCompareStaticValue.Location = new System.Drawing.Point(10, 48);
            this.rdoVarCompareStaticValue.Name = "rdoVarCompareStaticValue";
            this.rdoVarCompareStaticValue.Size = new System.Drawing.Size(96, 17);
            this.rdoVarCompareStaticValue.TabIndex = 44;
            this.rdoVarCompareStaticValue.Text = "Static Value:";
            this.rdoVarCompareStaticValue.CheckedChanged += new System.EventHandler(this.rdoVarCompareStaticValue_CheckedChanged);
            // 
            // cmbComparePlayerVar
            // 
            this.cmbComparePlayerVar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbComparePlayerVar.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbComparePlayerVar.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbComparePlayerVar.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbComparePlayerVar.DrawDropdownHoverOutline = false;
            this.cmbComparePlayerVar.DrawFocusRectangle = false;
            this.cmbComparePlayerVar.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbComparePlayerVar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbComparePlayerVar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbComparePlayerVar.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbComparePlayerVar.FormattingEnabled = true;
            this.cmbComparePlayerVar.Location = new System.Drawing.Point(146, 75);
            this.cmbComparePlayerVar.Name = "cmbComparePlayerVar";
            this.cmbComparePlayerVar.Size = new System.Drawing.Size(94, 21);
            this.cmbComparePlayerVar.TabIndex = 47;
            this.cmbComparePlayerVar.Text = null;
            this.cmbComparePlayerVar.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // rdoVarComparePlayerVar
            // 
            this.rdoVarComparePlayerVar.AutoSize = true;
            this.rdoVarComparePlayerVar.Location = new System.Drawing.Point(10, 76);
            this.rdoVarComparePlayerVar.Name = "rdoVarComparePlayerVar";
            this.rdoVarComparePlayerVar.Size = new System.Drawing.Size(128, 17);
            this.rdoVarComparePlayerVar.TabIndex = 45;
            this.rdoVarComparePlayerVar.Text = "Player Variable Value:";
            this.rdoVarComparePlayerVar.CheckedChanged += new System.EventHandler(this.rdoVarComparePlayerVar_CheckedChanged);
            // 
            // rdoVarCompareGlobalVar
            // 
            this.rdoVarCompareGlobalVar.AutoSize = true;
            this.rdoVarCompareGlobalVar.Location = new System.Drawing.Point(10, 103);
            this.rdoVarCompareGlobalVar.Name = "rdoVarCompareGlobalVar";
            this.rdoVarCompareGlobalVar.Size = new System.Drawing.Size(129, 17);
            this.rdoVarCompareGlobalVar.TabIndex = 46;
            this.rdoVarCompareGlobalVar.Text = "Global Variable Value:";
            this.rdoVarCompareGlobalVar.CheckedChanged += new System.EventHandler(this.rdoVarCompareGlobalVar_CheckedChanged);
            // 
            // grpQuestInProgress
            // 
            this.grpQuestInProgress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpQuestInProgress.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpQuestInProgress.Controls.Add(this.lblQuestTask);
            this.grpQuestInProgress.Controls.Add(this.cmbQuestTask);
            this.grpQuestInProgress.Controls.Add(this.cmbTaskModifier);
            this.grpQuestInProgress.Controls.Add(this.lblQuestIs);
            this.grpQuestInProgress.Controls.Add(this.lblQuestProgress);
            this.grpQuestInProgress.Controls.Add(this.cmbQuestInProgress);
            this.grpQuestInProgress.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpQuestInProgress.Location = new System.Drawing.Point(9, 40);
            this.grpQuestInProgress.Name = "grpQuestInProgress";
            this.grpQuestInProgress.Size = new System.Drawing.Size(263, 122);
            this.grpQuestInProgress.TabIndex = 32;
            this.grpQuestInProgress.TabStop = false;
            this.grpQuestInProgress.Text = "Quest In Progress:";
            this.grpQuestInProgress.Visible = false;
            // 
            // lblQuestTask
            // 
            this.lblQuestTask.AutoSize = true;
            this.lblQuestTask.Location = new System.Drawing.Point(6, 86);
            this.lblQuestTask.Name = "lblQuestTask";
            this.lblQuestTask.Size = new System.Drawing.Size(34, 13);
            this.lblQuestTask.TabIndex = 9;
            this.lblQuestTask.Text = "Task:";
            // 
            // cmbQuestTask
            // 
            this.cmbQuestTask.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbQuestTask.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbQuestTask.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbQuestTask.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbQuestTask.DrawDropdownHoverOutline = false;
            this.cmbQuestTask.DrawFocusRectangle = false;
            this.cmbQuestTask.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbQuestTask.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbQuestTask.Enabled = false;
            this.cmbQuestTask.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbQuestTask.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbQuestTask.FormattingEnabled = true;
            this.cmbQuestTask.Location = new System.Drawing.Point(92, 83);
            this.cmbQuestTask.Name = "cmbQuestTask";
            this.cmbQuestTask.Size = new System.Drawing.Size(163, 21);
            this.cmbQuestTask.TabIndex = 8;
            this.cmbQuestTask.Text = null;
            this.cmbQuestTask.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // cmbTaskModifier
            // 
            this.cmbTaskModifier.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbTaskModifier.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbTaskModifier.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbTaskModifier.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbTaskModifier.DrawDropdownHoverOutline = false;
            this.cmbTaskModifier.DrawFocusRectangle = false;
            this.cmbTaskModifier.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbTaskModifier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTaskModifier.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbTaskModifier.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbTaskModifier.FormattingEnabled = true;
            this.cmbTaskModifier.Location = new System.Drawing.Point(92, 50);
            this.cmbTaskModifier.Name = "cmbTaskModifier";
            this.cmbTaskModifier.Size = new System.Drawing.Size(163, 21);
            this.cmbTaskModifier.TabIndex = 7;
            this.cmbTaskModifier.Text = null;
            this.cmbTaskModifier.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbTaskModifier.SelectedIndexChanged += new System.EventHandler(this.cmbTaskModifier_SelectedIndexChanged);
            // 
            // lblQuestIs
            // 
            this.lblQuestIs.AutoSize = true;
            this.lblQuestIs.Location = new System.Drawing.Point(6, 52);
            this.lblQuestIs.Name = "lblQuestIs";
            this.lblQuestIs.Size = new System.Drawing.Size(18, 13);
            this.lblQuestIs.TabIndex = 6;
            this.lblQuestIs.Text = "Is:";
            // 
            // lblQuestProgress
            // 
            this.lblQuestProgress.AutoSize = true;
            this.lblQuestProgress.Location = new System.Drawing.Point(6, 21);
            this.lblQuestProgress.Name = "lblQuestProgress";
            this.lblQuestProgress.Size = new System.Drawing.Size(38, 13);
            this.lblQuestProgress.TabIndex = 5;
            this.lblQuestProgress.Text = "Quest:";
            // 
            // cmbQuestInProgress
            // 
            this.cmbQuestInProgress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbQuestInProgress.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbQuestInProgress.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbQuestInProgress.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbQuestInProgress.DrawDropdownHoverOutline = false;
            this.cmbQuestInProgress.DrawFocusRectangle = false;
            this.cmbQuestInProgress.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbQuestInProgress.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbQuestInProgress.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbQuestInProgress.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbQuestInProgress.FormattingEnabled = true;
            this.cmbQuestInProgress.Location = new System.Drawing.Point(92, 18);
            this.cmbQuestInProgress.Name = "cmbQuestInProgress";
            this.cmbQuestInProgress.Size = new System.Drawing.Size(163, 21);
            this.cmbQuestInProgress.TabIndex = 3;
            this.cmbQuestInProgress.Text = null;
            this.cmbQuestInProgress.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbQuestInProgress.SelectedIndexChanged += new System.EventHandler(this.cmbQuestInProgress_SelectedIndexChanged);
            // 
            // grpStartQuest
            // 
            this.grpStartQuest.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpStartQuest.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpStartQuest.Controls.Add(this.lblStartQuest);
            this.grpStartQuest.Controls.Add(this.cmbStartQuest);
            this.grpStartQuest.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpStartQuest.Location = new System.Drawing.Point(9, 40);
            this.grpStartQuest.Name = "grpStartQuest";
            this.grpStartQuest.Size = new System.Drawing.Size(262, 71);
            this.grpStartQuest.TabIndex = 31;
            this.grpStartQuest.TabStop = false;
            this.grpStartQuest.Text = "Can Start Quest:";
            this.grpStartQuest.Visible = false;
            // 
            // lblStartQuest
            // 
            this.lblStartQuest.AutoSize = true;
            this.lblStartQuest.Location = new System.Drawing.Point(6, 21);
            this.lblStartQuest.Name = "lblStartQuest";
            this.lblStartQuest.Size = new System.Drawing.Size(38, 13);
            this.lblStartQuest.TabIndex = 5;
            this.lblStartQuest.Text = "Quest:";
            // 
            // cmbStartQuest
            // 
            this.cmbStartQuest.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbStartQuest.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbStartQuest.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbStartQuest.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbStartQuest.DrawDropdownHoverOutline = false;
            this.cmbStartQuest.DrawFocusRectangle = false;
            this.cmbStartQuest.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbStartQuest.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStartQuest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbStartQuest.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbStartQuest.FormattingEnabled = true;
            this.cmbStartQuest.Location = new System.Drawing.Point(92, 18);
            this.cmbStartQuest.Name = "cmbStartQuest";
            this.cmbStartQuest.Size = new System.Drawing.Size(162, 21);
            this.cmbStartQuest.TabIndex = 3;
            this.cmbStartQuest.Text = null;
            this.cmbStartQuest.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // grpTime
            // 
            this.grpTime.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpTime.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpTime.Controls.Add(this.lblEndRange);
            this.grpTime.Controls.Add(this.lblStartRange);
            this.grpTime.Controls.Add(this.cmbTime2);
            this.grpTime.Controls.Add(this.cmbTime1);
            this.grpTime.Controls.Add(this.lblAnd);
            this.grpTime.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpTime.Location = new System.Drawing.Point(9, 40);
            this.grpTime.Name = "grpTime";
            this.grpTime.Size = new System.Drawing.Size(261, 105);
            this.grpTime.TabIndex = 30;
            this.grpTime.TabStop = false;
            this.grpTime.Text = "Time is between:";
            this.grpTime.Visible = false;
            // 
            // lblEndRange
            // 
            this.lblEndRange.AutoSize = true;
            this.lblEndRange.Location = new System.Drawing.Point(9, 73);
            this.lblEndRange.Name = "lblEndRange";
            this.lblEndRange.Size = new System.Drawing.Size(64, 13);
            this.lblEndRange.TabIndex = 6;
            this.lblEndRange.Text = "End Range:";
            // 
            // lblStartRange
            // 
            this.lblStartRange.AutoSize = true;
            this.lblStartRange.Location = new System.Drawing.Point(6, 21);
            this.lblStartRange.Name = "lblStartRange";
            this.lblStartRange.Size = new System.Drawing.Size(67, 13);
            this.lblStartRange.TabIndex = 5;
            this.lblStartRange.Text = "Start Range:";
            // 
            // cmbTime2
            // 
            this.cmbTime2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbTime2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbTime2.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbTime2.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbTime2.DrawDropdownHoverOutline = false;
            this.cmbTime2.DrawFocusRectangle = false;
            this.cmbTime2.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbTime2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTime2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbTime2.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbTime2.FormattingEnabled = true;
            this.cmbTime2.Location = new System.Drawing.Point(92, 70);
            this.cmbTime2.Name = "cmbTime2";
            this.cmbTime2.Size = new System.Drawing.Size(161, 21);
            this.cmbTime2.TabIndex = 4;
            this.cmbTime2.Text = null;
            this.cmbTime2.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // cmbTime1
            // 
            this.cmbTime1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbTime1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbTime1.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbTime1.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbTime1.DrawDropdownHoverOutline = false;
            this.cmbTime1.DrawFocusRectangle = false;
            this.cmbTime1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbTime1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTime1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbTime1.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbTime1.FormattingEnabled = true;
            this.cmbTime1.Location = new System.Drawing.Point(92, 18);
            this.cmbTime1.Name = "cmbTime1";
            this.cmbTime1.Size = new System.Drawing.Size(161, 21);
            this.cmbTime1.TabIndex = 3;
            this.cmbTime1.Text = null;
            this.cmbTime1.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // lblAnd
            // 
            this.lblAnd.AutoSize = true;
            this.lblAnd.Location = new System.Drawing.Point(100, 49);
            this.lblAnd.Name = "lblAnd";
            this.lblAnd.Size = new System.Drawing.Size(26, 13);
            this.lblAnd.TabIndex = 2;
            this.lblAnd.Text = "And";
            // 
            // grpPowerIs
            // 
            this.grpPowerIs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpPowerIs.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpPowerIs.Controls.Add(this.cmbPower);
            this.grpPowerIs.Controls.Add(this.lblPower);
            this.grpPowerIs.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpPowerIs.Location = new System.Drawing.Point(9, 40);
            this.grpPowerIs.Name = "grpPowerIs";
            this.grpPowerIs.Size = new System.Drawing.Size(262, 51);
            this.grpPowerIs.TabIndex = 25;
            this.grpPowerIs.TabStop = false;
            this.grpPowerIs.Text = "Power Is...";
            // 
            // cmbPower
            // 
            this.cmbPower.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbPower.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbPower.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbPower.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbPower.DrawDropdownHoverOutline = false;
            this.cmbPower.DrawFocusRectangle = false;
            this.cmbPower.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbPower.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPower.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbPower.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbPower.FormattingEnabled = true;
            this.cmbPower.Location = new System.Drawing.Point(79, 17);
            this.cmbPower.Name = "cmbPower";
            this.cmbPower.Size = new System.Drawing.Size(175, 21);
            this.cmbPower.TabIndex = 1;
            this.cmbPower.Text = null;
            this.cmbPower.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // lblPower
            // 
            this.lblPower.AutoSize = true;
            this.lblPower.Location = new System.Drawing.Point(7, 20);
            this.lblPower.Name = "lblPower";
            this.lblPower.Size = new System.Drawing.Size(40, 13);
            this.lblPower.TabIndex = 0;
            this.lblPower.Text = "Power:";
            // 
            // grpSelfSwitch
            // 
            this.grpSelfSwitch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpSelfSwitch.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpSelfSwitch.Controls.Add(this.cmbSelfSwitchVal);
            this.grpSelfSwitch.Controls.Add(this.lblSelfSwitchIs);
            this.grpSelfSwitch.Controls.Add(this.cmbSelfSwitch);
            this.grpSelfSwitch.Controls.Add(this.lblSelfSwitch);
            this.grpSelfSwitch.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpSelfSwitch.Location = new System.Drawing.Point(9, 40);
            this.grpSelfSwitch.Name = "grpSelfSwitch";
            this.grpSelfSwitch.Size = new System.Drawing.Size(262, 89);
            this.grpSelfSwitch.TabIndex = 29;
            this.grpSelfSwitch.TabStop = false;
            this.grpSelfSwitch.Text = "Self Switch";
            // 
            // cmbSelfSwitchVal
            // 
            this.cmbSelfSwitchVal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbSelfSwitchVal.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbSelfSwitchVal.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbSelfSwitchVal.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbSelfSwitchVal.DrawDropdownHoverOutline = false;
            this.cmbSelfSwitchVal.DrawFocusRectangle = false;
            this.cmbSelfSwitchVal.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbSelfSwitchVal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSelfSwitchVal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbSelfSwitchVal.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbSelfSwitchVal.FormattingEnabled = true;
            this.cmbSelfSwitchVal.Location = new System.Drawing.Point(79, 52);
            this.cmbSelfSwitchVal.Name = "cmbSelfSwitchVal";
            this.cmbSelfSwitchVal.Size = new System.Drawing.Size(177, 21);
            this.cmbSelfSwitchVal.TabIndex = 3;
            this.cmbSelfSwitchVal.Text = null;
            this.cmbSelfSwitchVal.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // lblSelfSwitchIs
            // 
            this.lblSelfSwitchIs.AutoSize = true;
            this.lblSelfSwitchIs.Location = new System.Drawing.Point(10, 55);
            this.lblSelfSwitchIs.Name = "lblSelfSwitchIs";
            this.lblSelfSwitchIs.Size = new System.Drawing.Size(21, 13);
            this.lblSelfSwitchIs.TabIndex = 2;
            this.lblSelfSwitchIs.Text = "Is: ";
            // 
            // cmbSelfSwitch
            // 
            this.cmbSelfSwitch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbSelfSwitch.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbSelfSwitch.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbSelfSwitch.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbSelfSwitch.DrawDropdownHoverOutline = false;
            this.cmbSelfSwitch.DrawFocusRectangle = false;
            this.cmbSelfSwitch.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbSelfSwitch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSelfSwitch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbSelfSwitch.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbSelfSwitch.FormattingEnabled = true;
            this.cmbSelfSwitch.Location = new System.Drawing.Point(79, 17);
            this.cmbSelfSwitch.Name = "cmbSelfSwitch";
            this.cmbSelfSwitch.Size = new System.Drawing.Size(177, 21);
            this.cmbSelfSwitch.TabIndex = 1;
            this.cmbSelfSwitch.Text = null;
            this.cmbSelfSwitch.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // lblSelfSwitch
            // 
            this.lblSelfSwitch.AutoSize = true;
            this.lblSelfSwitch.Location = new System.Drawing.Point(7, 20);
            this.lblSelfSwitch.Name = "lblSelfSwitch";
            this.lblSelfSwitch.Size = new System.Drawing.Size(66, 13);
            this.lblSelfSwitch.TabIndex = 0;
            this.lblSelfSwitch.Text = "Self Switch: ";
            // 
            // grpSpell
            // 
            this.grpSpell.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpSpell.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpSpell.Controls.Add(this.cmbSpell);
            this.grpSpell.Controls.Add(this.lblSpell);
            this.grpSpell.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpSpell.Location = new System.Drawing.Point(9, 40);
            this.grpSpell.Name = "grpSpell";
            this.grpSpell.Size = new System.Drawing.Size(262, 52);
            this.grpSpell.TabIndex = 26;
            this.grpSpell.TabStop = false;
            this.grpSpell.Text = "Knows Spell";
            // 
            // cmbSpell
            // 
            this.cmbSpell.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbSpell.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbSpell.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbSpell.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbSpell.DrawDropdownHoverOutline = false;
            this.cmbSpell.DrawFocusRectangle = false;
            this.cmbSpell.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbSpell.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSpell.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbSpell.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbSpell.FormattingEnabled = true;
            this.cmbSpell.Location = new System.Drawing.Point(79, 18);
            this.cmbSpell.Name = "cmbSpell";
            this.cmbSpell.Size = new System.Drawing.Size(175, 21);
            this.cmbSpell.TabIndex = 3;
            this.cmbSpell.Text = null;
            this.cmbSpell.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // lblSpell
            // 
            this.lblSpell.AutoSize = true;
            this.lblSpell.Location = new System.Drawing.Point(7, 20);
            this.lblSpell.Name = "lblSpell";
            this.lblSpell.Size = new System.Drawing.Size(33, 13);
            this.lblSpell.TabIndex = 2;
            this.lblSpell.Text = "Spell:";
            // 
            // grpClass
            // 
            this.grpClass.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpClass.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpClass.Controls.Add(this.cmbClass);
            this.grpClass.Controls.Add(this.lblClass);
            this.grpClass.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpClass.Location = new System.Drawing.Point(9, 40);
            this.grpClass.Name = "grpClass";
            this.grpClass.Size = new System.Drawing.Size(262, 52);
            this.grpClass.TabIndex = 27;
            this.grpClass.TabStop = false;
            this.grpClass.Text = "Class is";
            // 
            // cmbClass
            // 
            this.cmbClass.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbClass.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbClass.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbClass.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbClass.DrawDropdownHoverOutline = false;
            this.cmbClass.DrawFocusRectangle = false;
            this.cmbClass.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbClass.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbClass.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbClass.FormattingEnabled = true;
            this.cmbClass.Location = new System.Drawing.Point(79, 18);
            this.cmbClass.Name = "cmbClass";
            this.cmbClass.Size = new System.Drawing.Size(175, 21);
            this.cmbClass.TabIndex = 3;
            this.cmbClass.Text = null;
            this.cmbClass.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // lblClass
            // 
            this.lblClass.AutoSize = true;
            this.lblClass.Location = new System.Drawing.Point(7, 20);
            this.lblClass.Name = "lblClass";
            this.lblClass.Size = new System.Drawing.Size(35, 13);
            this.lblClass.TabIndex = 2;
            this.lblClass.Text = "Class:";
            // 
            // grpLevelStat
            // 
            this.grpLevelStat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpLevelStat.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpLevelStat.Controls.Add(this.chkStatIgnoreBuffs);
            this.grpLevelStat.Controls.Add(this.nudLevelStatValue);
            this.grpLevelStat.Controls.Add(this.cmbLevelStat);
            this.grpLevelStat.Controls.Add(this.lblLevelOrStat);
            this.grpLevelStat.Controls.Add(this.lblLvlStatValue);
            this.grpLevelStat.Controls.Add(this.cmbLevelComparator);
            this.grpLevelStat.Controls.Add(this.lblLevelComparator);
            this.grpLevelStat.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpLevelStat.Location = new System.Drawing.Point(9, 40);
            this.grpLevelStat.Name = "grpLevelStat";
            this.grpLevelStat.Size = new System.Drawing.Size(262, 140);
            this.grpLevelStat.TabIndex = 28;
            this.grpLevelStat.TabStop = false;
            this.grpLevelStat.Text = "Level or Stat is...";
            // 
            // chkStatIgnoreBuffs
            // 
            this.chkStatIgnoreBuffs.Location = new System.Drawing.Point(13, 115);
            this.chkStatIgnoreBuffs.Name = "chkStatIgnoreBuffs";
            this.chkStatIgnoreBuffs.Size = new System.Drawing.Size(211, 17);
            this.chkStatIgnoreBuffs.TabIndex = 32;
            this.chkStatIgnoreBuffs.Text = "Ignore equipment & spell buffs.";
            // 
            // nudLevelStatValue
            // 
            this.nudLevelStatValue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudLevelStatValue.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudLevelStatValue.Location = new System.Drawing.Point(79, 87);
            this.nudLevelStatValue.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudLevelStatValue.Name = "nudLevelStatValue";
            this.nudLevelStatValue.Size = new System.Drawing.Size(178, 20);
            this.nudLevelStatValue.TabIndex = 8;
            this.nudLevelStatValue.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // cmbLevelStat
            // 
            this.cmbLevelStat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbLevelStat.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbLevelStat.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbLevelStat.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbLevelStat.DrawDropdownHoverOutline = false;
            this.cmbLevelStat.DrawFocusRectangle = false;
            this.cmbLevelStat.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbLevelStat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLevelStat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbLevelStat.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbLevelStat.FormattingEnabled = true;
            this.cmbLevelStat.Items.AddRange(new object[] {
            "Level",
            "Attack",
            "Defense",
            "Speed",
            "Ability Power",
            "Magic Resist"});
            this.cmbLevelStat.Location = new System.Drawing.Point(79, 23);
            this.cmbLevelStat.Name = "cmbLevelStat";
            this.cmbLevelStat.Size = new System.Drawing.Size(177, 21);
            this.cmbLevelStat.TabIndex = 7;
            this.cmbLevelStat.Text = "Level";
            this.cmbLevelStat.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // lblLevelOrStat
            // 
            this.lblLevelOrStat.AutoSize = true;
            this.lblLevelOrStat.Location = new System.Drawing.Point(7, 25);
            this.lblLevelOrStat.Name = "lblLevelOrStat";
            this.lblLevelOrStat.Size = new System.Drawing.Size(70, 13);
            this.lblLevelOrStat.TabIndex = 6;
            this.lblLevelOrStat.Text = "Level or Stat:";
            // 
            // lblLvlStatValue
            // 
            this.lblLvlStatValue.AutoSize = true;
            this.lblLvlStatValue.Location = new System.Drawing.Point(10, 89);
            this.lblLvlStatValue.Name = "lblLvlStatValue";
            this.lblLvlStatValue.Size = new System.Drawing.Size(37, 13);
            this.lblLvlStatValue.TabIndex = 4;
            this.lblLvlStatValue.Text = "Value:";
            // 
            // cmbLevelComparator
            // 
            this.cmbLevelComparator.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbLevelComparator.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbLevelComparator.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbLevelComparator.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbLevelComparator.DrawDropdownHoverOutline = false;
            this.cmbLevelComparator.DrawFocusRectangle = false;
            this.cmbLevelComparator.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbLevelComparator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLevelComparator.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbLevelComparator.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbLevelComparator.FormattingEnabled = true;
            this.cmbLevelComparator.Location = new System.Drawing.Point(79, 53);
            this.cmbLevelComparator.Name = "cmbLevelComparator";
            this.cmbLevelComparator.Size = new System.Drawing.Size(177, 21);
            this.cmbLevelComparator.TabIndex = 3;
            this.cmbLevelComparator.Text = null;
            this.cmbLevelComparator.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // lblLevelComparator
            // 
            this.lblLevelComparator.AutoSize = true;
            this.lblLevelComparator.Location = new System.Drawing.Point(7, 55);
            this.lblLevelComparator.Name = "lblLevelComparator";
            this.lblLevelComparator.Size = new System.Drawing.Size(64, 13);
            this.lblLevelComparator.TabIndex = 2;
            this.lblLevelComparator.Text = "Comparator:";
            // 
            // grpMapIs
            // 
            this.grpMapIs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpMapIs.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpMapIs.Controls.Add(this.btnSelectMap);
            this.grpMapIs.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpMapIs.Location = new System.Drawing.Point(9, 40);
            this.grpMapIs.Name = "grpMapIs";
            this.grpMapIs.Size = new System.Drawing.Size(261, 61);
            this.grpMapIs.TabIndex = 35;
            this.grpMapIs.TabStop = false;
            this.grpMapIs.Text = "Map Is...";
            // 
            // btnSelectMap
            // 
            this.btnSelectMap.Location = new System.Drawing.Point(9, 21);
            this.btnSelectMap.Name = "btnSelectMap";
            this.btnSelectMap.Padding = new System.Windows.Forms.Padding(5);
            this.btnSelectMap.Size = new System.Drawing.Size(244, 23);
            this.btnSelectMap.TabIndex = 21;
            this.btnSelectMap.Text = "Select Map";
            this.btnSelectMap.Click += new System.EventHandler(this.btnSelectMap_Click);
            // 
            // grpGender
            // 
            this.grpGender.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpGender.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpGender.Controls.Add(this.cmbGender);
            this.grpGender.Controls.Add(this.lblGender);
            this.grpGender.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpGender.Location = new System.Drawing.Point(9, 40);
            this.grpGender.Name = "grpGender";
            this.grpGender.Size = new System.Drawing.Size(261, 51);
            this.grpGender.TabIndex = 33;
            this.grpGender.TabStop = false;
            this.grpGender.Text = "Gender Is...";
            // 
            // cmbGender
            // 
            this.cmbGender.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbGender.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbGender.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbGender.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbGender.DrawDropdownHoverOutline = false;
            this.cmbGender.DrawFocusRectangle = false;
            this.cmbGender.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbGender.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGender.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbGender.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbGender.FormattingEnabled = true;
            this.cmbGender.Location = new System.Drawing.Point(79, 17);
            this.cmbGender.Name = "cmbGender";
            this.cmbGender.Size = new System.Drawing.Size(174, 21);
            this.cmbGender.TabIndex = 1;
            this.cmbGender.Text = null;
            this.cmbGender.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // lblGender
            // 
            this.lblGender.AutoSize = true;
            this.lblGender.Location = new System.Drawing.Point(7, 20);
            this.lblGender.Name = "lblGender";
            this.lblGender.Size = new System.Drawing.Size(45, 13);
            this.lblGender.TabIndex = 0;
            this.lblGender.Text = "Gender:";
            // 
            // grpEquippedItem
            // 
            this.grpEquippedItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpEquippedItem.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpEquippedItem.Controls.Add(this.cmbEquippedItem);
            this.grpEquippedItem.Controls.Add(this.lblEquippedItem);
            this.grpEquippedItem.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpEquippedItem.Location = new System.Drawing.Point(9, 40);
            this.grpEquippedItem.Name = "grpEquippedItem";
            this.grpEquippedItem.Size = new System.Drawing.Size(262, 58);
            this.grpEquippedItem.TabIndex = 26;
            this.grpEquippedItem.TabStop = false;
            this.grpEquippedItem.Text = "Has Equipped Item";
            // 
            // cmbEquippedItem
            // 
            this.cmbEquippedItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbEquippedItem.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbEquippedItem.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbEquippedItem.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbEquippedItem.DrawDropdownHoverOutline = false;
            this.cmbEquippedItem.DrawFocusRectangle = false;
            this.cmbEquippedItem.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbEquippedItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEquippedItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbEquippedItem.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbEquippedItem.FormattingEnabled = true;
            this.cmbEquippedItem.Location = new System.Drawing.Point(105, 27);
            this.cmbEquippedItem.Name = "cmbEquippedItem";
            this.cmbEquippedItem.Size = new System.Drawing.Size(150, 21);
            this.cmbEquippedItem.TabIndex = 3;
            this.cmbEquippedItem.Text = null;
            this.cmbEquippedItem.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // lblEquippedItem
            // 
            this.lblEquippedItem.AutoSize = true;
            this.lblEquippedItem.Location = new System.Drawing.Point(6, 24);
            this.lblEquippedItem.Name = "lblEquippedItem";
            this.lblEquippedItem.Size = new System.Drawing.Size(30, 13);
            this.lblEquippedItem.TabIndex = 2;
            this.lblEquippedItem.Text = "Item:";
            // 
            // grpInventoryConditions
            // 
            this.grpInventoryConditions.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpInventoryConditions.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpInventoryConditions.Controls.Add(this.grpVariableAmount);
            this.grpInventoryConditions.Controls.Add(this.grpManualAmount);
            this.grpInventoryConditions.Controls.Add(this.grpAmountType);
            this.grpInventoryConditions.Controls.Add(this.cmbItem);
            this.grpInventoryConditions.Controls.Add(this.lblItem);
            this.grpInventoryConditions.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpInventoryConditions.Location = new System.Drawing.Point(9, 40);
            this.grpInventoryConditions.Name = "grpInventoryConditions";
            this.grpInventoryConditions.Size = new System.Drawing.Size(262, 179);
            this.grpInventoryConditions.TabIndex = 25;
            this.grpInventoryConditions.TabStop = false;
            this.grpInventoryConditions.Text = "Has Item";
            // 
            // grpVariableAmount
            // 
            this.grpVariableAmount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpVariableAmount.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpVariableAmount.Controls.Add(this.cmbInvVariable);
            this.grpVariableAmount.Controls.Add(this.lblInvVariable);
            this.grpVariableAmount.Controls.Add(this.rdoInvGlobalVariable);
            this.grpVariableAmount.Controls.Add(this.rdoInvPlayerVariable);
            this.grpVariableAmount.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpVariableAmount.Location = new System.Drawing.Point(6, 68);
            this.grpVariableAmount.Name = "grpVariableAmount";
            this.grpVariableAmount.Size = new System.Drawing.Size(250, 71);
            this.grpVariableAmount.TabIndex = 39;
            this.grpVariableAmount.TabStop = false;
            this.grpVariableAmount.Text = "Variable Amount:";
            this.grpVariableAmount.Visible = false;
            // 
            // cmbInvVariable
            // 
            this.cmbInvVariable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbInvVariable.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbInvVariable.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbInvVariable.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbInvVariable.DrawDropdownHoverOutline = false;
            this.cmbInvVariable.DrawFocusRectangle = false;
            this.cmbInvVariable.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbInvVariable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInvVariable.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbInvVariable.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbInvVariable.FormattingEnabled = true;
            this.cmbInvVariable.Location = new System.Drawing.Point(67, 44);
            this.cmbInvVariable.Name = "cmbInvVariable";
            this.cmbInvVariable.Size = new System.Drawing.Size(177, 21);
            this.cmbInvVariable.TabIndex = 39;
            this.cmbInvVariable.Text = null;
            this.cmbInvVariable.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // lblInvVariable
            // 
            this.lblInvVariable.AutoSize = true;
            this.lblInvVariable.Location = new System.Drawing.Point(8, 46);
            this.lblInvVariable.Name = "lblInvVariable";
            this.lblInvVariable.Size = new System.Drawing.Size(45, 13);
            this.lblInvVariable.TabIndex = 38;
            this.lblInvVariable.Text = "Variable";
            // 
            // rdoInvGlobalVariable
            // 
            this.rdoInvGlobalVariable.AutoSize = true;
            this.rdoInvGlobalVariable.Location = new System.Drawing.Point(148, 19);
            this.rdoInvGlobalVariable.Name = "rdoInvGlobalVariable";
            this.rdoInvGlobalVariable.Size = new System.Drawing.Size(96, 17);
            this.rdoInvGlobalVariable.TabIndex = 37;
            this.rdoInvGlobalVariable.Text = "Global Variable";
            this.rdoInvGlobalVariable.CheckedChanged += new System.EventHandler(this.rdoInvGlobalVariable_CheckedChanged);
            // 
            // rdoInvPlayerVariable
            // 
            this.rdoInvPlayerVariable.AutoSize = true;
            this.rdoInvPlayerVariable.Checked = true;
            this.rdoInvPlayerVariable.Location = new System.Drawing.Point(6, 19);
            this.rdoInvPlayerVariable.Name = "rdoInvPlayerVariable";
            this.rdoInvPlayerVariable.Size = new System.Drawing.Size(95, 17);
            this.rdoInvPlayerVariable.TabIndex = 36;
            this.rdoInvPlayerVariable.TabStop = true;
            this.rdoInvPlayerVariable.Text = "Player Variable";
            this.rdoInvPlayerVariable.CheckedChanged += new System.EventHandler(this.rdoInvPlayerVariable_CheckedChanged);
            // 
            // grpManualAmount
            // 
            this.grpManualAmount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpManualAmount.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpManualAmount.Controls.Add(this.nudItemAmount);
            this.grpManualAmount.Controls.Add(this.lblItemQuantity);
            this.grpManualAmount.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpManualAmount.Location = new System.Drawing.Point(6, 68);
            this.grpManualAmount.Name = "grpManualAmount";
            this.grpManualAmount.Size = new System.Drawing.Size(250, 71);
            this.grpManualAmount.TabIndex = 38;
            this.grpManualAmount.TabStop = false;
            this.grpManualAmount.Text = "Manual Amount:";
            // 
            // nudItemAmount
            // 
            this.nudItemAmount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudItemAmount.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudItemAmount.Location = new System.Drawing.Point(86, 25);
            this.nudItemAmount.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudItemAmount.Name = "nudItemAmount";
            this.nudItemAmount.Size = new System.Drawing.Size(150, 20);
            this.nudItemAmount.TabIndex = 6;
            this.nudItemAmount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudItemAmount.ValueChanged += new System.EventHandler(this.NudItemAmount_ValueChanged);
            // 
            // lblItemQuantity
            // 
            this.lblItemQuantity.AutoSize = true;
            this.lblItemQuantity.Location = new System.Drawing.Point(14, 27);
            this.lblItemQuantity.Name = "lblItemQuantity";
            this.lblItemQuantity.Size = new System.Drawing.Size(66, 13);
            this.lblItemQuantity.TabIndex = 5;
            this.lblItemQuantity.Text = "Has at least:";
            // 
            // grpAmountType
            // 
            this.grpAmountType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpAmountType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpAmountType.Controls.Add(this.rdoVariable);
            this.grpAmountType.Controls.Add(this.rdoManual);
            this.grpAmountType.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpAmountType.Location = new System.Drawing.Point(6, 14);
            this.grpAmountType.Name = "grpAmountType";
            this.grpAmountType.Size = new System.Drawing.Size(250, 48);
            this.grpAmountType.TabIndex = 37;
            this.grpAmountType.TabStop = false;
            this.grpAmountType.Text = "Amount Type:";
            // 
            // rdoVariable
            // 
            this.rdoVariable.AutoSize = true;
            this.rdoVariable.Location = new System.Drawing.Point(181, 19);
            this.rdoVariable.Name = "rdoVariable";
            this.rdoVariable.Size = new System.Drawing.Size(63, 17);
            this.rdoVariable.TabIndex = 36;
            this.rdoVariable.Text = "Variable";
            this.rdoVariable.CheckedChanged += new System.EventHandler(this.rdoVariable_CheckedChanged);
            // 
            // rdoManual
            // 
            this.rdoManual.AutoSize = true;
            this.rdoManual.Checked = true;
            this.rdoManual.Location = new System.Drawing.Point(9, 19);
            this.rdoManual.Name = "rdoManual";
            this.rdoManual.Size = new System.Drawing.Size(60, 17);
            this.rdoManual.TabIndex = 35;
            this.rdoManual.TabStop = true;
            this.rdoManual.Text = "Manual";
            this.rdoManual.CheckedChanged += new System.EventHandler(this.rdoManual_CheckedChanged);
            // 
            // cmbItem
            // 
            this.cmbItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbItem.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbItem.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbItem.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbItem.DrawDropdownHoverOutline = false;
            this.cmbItem.DrawFocusRectangle = false;
            this.cmbItem.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbItem.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbItem.FormattingEnabled = true;
            this.cmbItem.Location = new System.Drawing.Point(42, 145);
            this.cmbItem.Name = "cmbItem";
            this.cmbItem.Size = new System.Drawing.Size(212, 21);
            this.cmbItem.TabIndex = 3;
            this.cmbItem.Text = null;
            this.cmbItem.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // lblItem
            // 
            this.lblItem.AutoSize = true;
            this.lblItem.Location = new System.Drawing.Point(6, 147);
            this.lblItem.Name = "lblItem";
            this.lblItem.Size = new System.Drawing.Size(30, 13);
            this.lblItem.TabIndex = 2;
            this.lblItem.Text = "Item:";
            // 
            // EventCommandConditionalBranch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpConditional);
            this.Name = "EventCommandConditionalBranch";
            this.Size = new System.Drawing.Size(285, 345);
            this.grpConditional.ResumeLayout(false);
            this.grpConditional.PerformLayout();
            this.grpMapZoneType.ResumeLayout(false);
            this.grpMapZoneType.PerformLayout();
            this.grpInGuild.ResumeLayout(false);
            this.grpInGuild.PerformLayout();
            this.grpQuestCompleted.ResumeLayout(false);
            this.grpQuestCompleted.PerformLayout();
            this.grpVariable.ResumeLayout(false);
            this.grpSelectVariable.ResumeLayout(false);
            this.grpSelectVariable.PerformLayout();
            this.grpStringVariable.ResumeLayout(false);
            this.grpStringVariable.PerformLayout();
            this.grpBooleanVariable.ResumeLayout(false);
            this.grpBooleanVariable.PerformLayout();
            this.grpNumericVariable.ResumeLayout(false);
            this.grpNumericVariable.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudVariableValue)).EndInit();
            this.grpQuestInProgress.ResumeLayout(false);
            this.grpQuestInProgress.PerformLayout();
            this.grpStartQuest.ResumeLayout(false);
            this.grpStartQuest.PerformLayout();
            this.grpTime.ResumeLayout(false);
            this.grpTime.PerformLayout();
            this.grpPowerIs.ResumeLayout(false);
            this.grpPowerIs.PerformLayout();
            this.grpSelfSwitch.ResumeLayout(false);
            this.grpSelfSwitch.PerformLayout();
            this.grpSpell.ResumeLayout(false);
            this.grpSpell.PerformLayout();
            this.grpClass.ResumeLayout(false);
            this.grpClass.PerformLayout();
            this.grpLevelStat.ResumeLayout(false);
            this.grpLevelStat.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLevelStatValue)).EndInit();
            this.grpMapIs.ResumeLayout(false);
            this.grpGender.ResumeLayout(false);
            this.grpGender.PerformLayout();
            this.grpEquippedItem.ResumeLayout(false);
            this.grpEquippedItem.PerformLayout();
            this.grpInventoryConditions.ResumeLayout(false);
            this.grpInventoryConditions.PerformLayout();
            this.grpVariableAmount.ResumeLayout(false);
            this.grpVariableAmount.PerformLayout();
            this.grpManualAmount.ResumeLayout(false);
            this.grpManualAmount.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudItemAmount)).EndInit();
            this.grpAmountType.ResumeLayout(false);
            this.grpAmountType.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpConditional;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private DarkComboBox cmbConditionType;
        private System.Windows.Forms.Label lblType;
        private DarkGroupBox grpVariable;
        private DarkComboBox cmbNumericComparitor;
        private System.Windows.Forms.Label lblNumericComparator;
        private DarkGroupBox grpInventoryConditions;
        private DarkComboBox cmbItem;
        private System.Windows.Forms.Label lblItem;
        private DarkGroupBox grpSpell;
        private DarkComboBox cmbSpell;
        private System.Windows.Forms.Label lblSpell;
        private DarkGroupBox grpClass;
        private DarkComboBox cmbClass;
        private System.Windows.Forms.Label lblClass;
        private DarkGroupBox grpLevelStat;
        private System.Windows.Forms.Label lblLvlStatValue;
        private DarkComboBox cmbLevelComparator;
        private System.Windows.Forms.Label lblLevelComparator;
        private DarkGroupBox grpSelfSwitch;
        private DarkComboBox cmbSelfSwitchVal;
        private System.Windows.Forms.Label lblSelfSwitchIs;
        private DarkComboBox cmbSelfSwitch;
        private System.Windows.Forms.Label lblSelfSwitch;
        private DarkGroupBox grpPowerIs;
        private DarkComboBox cmbPower;
        private System.Windows.Forms.Label lblPower;
        private DarkGroupBox grpTime;
        private DarkComboBox cmbTime2;
        private DarkComboBox cmbTime1;
        private System.Windows.Forms.Label lblAnd;
        private System.Windows.Forms.Label lblEndRange;
        private System.Windows.Forms.Label lblStartRange;
        private DarkGroupBox grpQuestInProgress;
        private System.Windows.Forms.Label lblQuestTask;
        private DarkComboBox cmbQuestTask;
        private DarkComboBox cmbTaskModifier;
        private System.Windows.Forms.Label lblQuestIs;
        private System.Windows.Forms.Label lblQuestProgress;
        private DarkComboBox cmbQuestInProgress;
        private DarkGroupBox grpStartQuest;
        private System.Windows.Forms.Label lblStartQuest;
        private DarkComboBox cmbStartQuest;
        private DarkGroupBox grpQuestCompleted;
        private System.Windows.Forms.Label lblQuestCompleted;
        private DarkComboBox cmbCompletedQuest;
        private DarkComboBox cmbLevelStat;
        private System.Windows.Forms.Label lblLevelOrStat;
        private DarkGroupBox grpGender;
        private DarkComboBox cmbGender;
        private System.Windows.Forms.Label lblGender;
        private DarkNumericUpDown nudLevelStatValue;
        private DarkCheckBox chkStatIgnoreBuffs;
        private DarkCheckBox chkNegated;
        private DarkGroupBox grpMapIs;
        private DarkButton btnSelectMap;
        internal DarkComboBox cmbCompareGlobalVar;
        internal DarkComboBox cmbComparePlayerVar;
        internal DarkRadioButton rdoVarCompareGlobalVar;
        internal DarkRadioButton rdoVarComparePlayerVar;
        internal DarkRadioButton rdoVarCompareStaticValue;
        private DarkNumericUpDown nudVariableValue;
        private DarkGroupBox grpEquippedItem;
        private DarkComboBox cmbEquippedItem;
        private System.Windows.Forms.Label lblEquippedItem;
        private DarkGroupBox grpBooleanVariable;
        private DarkComboBox cmbBooleanComparator;
        private System.Windows.Forms.Label lblBooleanComparator;
        internal DarkComboBox cmbBooleanGlobalVariable;
        internal DarkComboBox cmbBooleanPlayerVariable;
        internal DarkRadioButton optBooleanPlayerVariable;
        internal DarkRadioButton optBooleanGlobalVariable;
        private DarkGroupBox grpNumericVariable;
        private DarkGroupBox grpSelectVariable;
        private DarkRadioButton rdoPlayerVariable;
        internal DarkComboBox cmbVariable;
        private DarkRadioButton rdoGlobalVariable;
        internal DarkRadioButton optBooleanTrue;
        internal DarkRadioButton optBooleanFalse;
        private DarkGroupBox grpStringVariable;
        private DarkComboBox cmbStringComparitor;
        private System.Windows.Forms.Label lblStringComparator;
        private DarkTextBox txtStringValue;
        private System.Windows.Forms.Label lblStringComparatorValue;
        private System.Windows.Forms.Label lblStringTextVariables;
        private DarkGroupBox grpAmountType;
        private DarkRadioButton rdoVariable;
        private DarkRadioButton rdoManual;
        private DarkGroupBox grpManualAmount;
        private DarkNumericUpDown nudItemAmount;
        private System.Windows.Forms.Label lblItemQuantity;
        private DarkGroupBox grpVariableAmount;
        private DarkComboBox cmbInvVariable;
        private System.Windows.Forms.Label lblInvVariable;
        private DarkRadioButton rdoInvGlobalVariable;
        private DarkRadioButton rdoInvPlayerVariable;
        private DarkCheckBox chkHasElse;
        private DarkGroupBox grpInGuild;
        private System.Windows.Forms.Label lblRank;
        private DarkComboBox cmbRank;
        private DarkGroupBox grpMapZoneType;
        private System.Windows.Forms.Label lblMapZoneType;
        private DarkComboBox cmbMapZoneType;
    }
}
