using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Quest
{
    partial class QuestTaskEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QuestTaskEditor));
            this.grpEditor = new DarkUI.Controls.DarkGroupBox();
            this.btnEditTaskEvent = new DarkUI.Controls.DarkButton();
            this.txtStartDesc = new DarkUI.Controls.DarkTextBox();
            this.lblDesc = new System.Windows.Forms.Label();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.cmbTaskType = new DarkUI.Controls.DarkComboBox();
            this.lblType = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.grpKillNpcs = new DarkUI.Controls.DarkGroupBox();
            this.nudNpcQuantity = new DarkUI.Controls.DarkNumericUpDown();
            this.cmbNpc = new DarkUI.Controls.DarkComboBox();
            this.lblNpc = new System.Windows.Forms.Label();
            this.lblNpcQuantity = new System.Windows.Forms.Label();
            this.grpGatherItems = new DarkUI.Controls.DarkGroupBox();
            this.nudItemAmount = new DarkUI.Controls.DarkNumericUpDown();
            this.cmbItem = new DarkUI.Controls.DarkComboBox();
            this.lblItem = new System.Windows.Forms.Label();
            this.lblItemQuantity = new System.Windows.Forms.Label();
            this.lblEventDriven = new System.Windows.Forms.Label();
            this.grpEditor.SuspendLayout();
            this.grpKillNpcs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudNpcQuantity)).BeginInit();
            this.grpGatherItems.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudItemAmount)).BeginInit();
            this.SuspendLayout();
            // 
            // grpEditor
            // 
            this.grpEditor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpEditor.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpEditor.Controls.Add(this.btnEditTaskEvent);
            this.grpEditor.Controls.Add(this.txtStartDesc);
            this.grpEditor.Controls.Add(this.lblDesc);
            this.grpEditor.Controls.Add(this.btnSave);
            this.grpEditor.Controls.Add(this.cmbTaskType);
            this.grpEditor.Controls.Add(this.lblType);
            this.grpEditor.Controls.Add(this.btnCancel);
            this.grpEditor.Controls.Add(this.grpKillNpcs);
            this.grpEditor.Controls.Add(this.grpGatherItems);
            this.grpEditor.Controls.Add(this.lblEventDriven);
            this.grpEditor.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpEditor.Location = new System.Drawing.Point(-1, 2);
            this.grpEditor.Name = "grpEditor";
            this.grpEditor.Size = new System.Drawing.Size(256, 261);
            this.grpEditor.TabIndex = 18;
            this.grpEditor.TabStop = false;
            this.grpEditor.Text = "Task Editor";
            // 
            // btnEditTaskEvent
            // 
            this.btnEditTaskEvent.Location = new System.Drawing.Point(10, 202);
            this.btnEditTaskEvent.Name = "btnEditTaskEvent";
            this.btnEditTaskEvent.Padding = new System.Windows.Forms.Padding(5);
            this.btnEditTaskEvent.Size = new System.Drawing.Size(236, 23);
            this.btnEditTaskEvent.TabIndex = 30;
            this.btnEditTaskEvent.Text = "Edit Task Completion Event";
            this.btnEditTaskEvent.Click += new System.EventHandler(this.btnEditTaskEvent_Click);
            // 
            // txtStartDesc
            // 
            this.txtStartDesc.AcceptsReturn = true;
            this.txtStartDesc.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtStartDesc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtStartDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtStartDesc.Location = new System.Drawing.Point(88, 40);
            this.txtStartDesc.Multiline = true;
            this.txtStartDesc.Name = "txtStartDesc";
            this.txtStartDesc.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtStartDesc.Size = new System.Drawing.Size(157, 64);
            this.txtStartDesc.TabIndex = 27;
            // 
            // lblDesc
            // 
            this.lblDesc.AutoSize = true;
            this.lblDesc.Location = new System.Drawing.Point(9, 42);
            this.lblDesc.Name = "lblDesc";
            this.lblDesc.Size = new System.Drawing.Size(35, 13);
            this.lblDesc.TabIndex = 26;
            this.lblDesc.Text = "Desc:";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(10, 231);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(5);
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Ok";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // cmbTaskType
            // 
            this.cmbTaskType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbTaskType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbTaskType.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbTaskType.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbTaskType.DrawDropdownHoverOutline = false;
            this.cmbTaskType.DrawFocusRectangle = false;
            this.cmbTaskType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbTaskType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTaskType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbTaskType.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbTaskType.FormattingEnabled = true;
            this.cmbTaskType.Items.AddRange(new object[] {
            "Event Driven",
            "Gather Item(s)",
            "Kill NPC(s)"});
            this.cmbTaskType.Location = new System.Drawing.Point(88, 13);
            this.cmbTaskType.Name = "cmbTaskType";
            this.cmbTaskType.Size = new System.Drawing.Size(157, 21);
            this.cmbTaskType.TabIndex = 22;
            this.cmbTaskType.Text = "Event Driven";
            this.cmbTaskType.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbTaskType.SelectedIndexChanged += new System.EventHandler(this.cmbConditionType_SelectedIndexChanged);
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(6, 16);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(61, 13);
            this.lblType.TabIndex = 21;
            this.lblType.Text = "Task Type:";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(91, 231);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // grpKillNpcs
            // 
            this.grpKillNpcs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpKillNpcs.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpKillNpcs.Controls.Add(this.nudNpcQuantity);
            this.grpKillNpcs.Controls.Add(this.cmbNpc);
            this.grpKillNpcs.Controls.Add(this.lblNpc);
            this.grpKillNpcs.Controls.Add(this.lblNpcQuantity);
            this.grpKillNpcs.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpKillNpcs.Location = new System.Drawing.Point(10, 110);
            this.grpKillNpcs.Name = "grpKillNpcs";
            this.grpKillNpcs.Size = new System.Drawing.Size(236, 83);
            this.grpKillNpcs.TabIndex = 28;
            this.grpKillNpcs.TabStop = false;
            this.grpKillNpcs.Text = "Kill NPC(s)";
            this.grpKillNpcs.Visible = false;
            // 
            // nudNpcQuantity
            // 
            this.nudNpcQuantity.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudNpcQuantity.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudNpcQuantity.Location = new System.Drawing.Point(103, 54);
            this.nudNpcQuantity.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudNpcQuantity.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudNpcQuantity.Name = "nudNpcQuantity";
            this.nudNpcQuantity.Size = new System.Drawing.Size(116, 20);
            this.nudNpcQuantity.TabIndex = 64;
            this.nudNpcQuantity.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // cmbNpc
            // 
            this.cmbNpc.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbNpc.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbNpc.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbNpc.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbNpc.DrawDropdownHoverOutline = false;
            this.cmbNpc.DrawFocusRectangle = false;
            this.cmbNpc.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbNpc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbNpc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbNpc.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbNpc.FormattingEnabled = true;
            this.cmbNpc.Location = new System.Drawing.Point(104, 21);
            this.cmbNpc.Name = "cmbNpc";
            this.cmbNpc.Size = new System.Drawing.Size(116, 21);
            this.cmbNpc.TabIndex = 3;
            this.cmbNpc.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // lblNpc
            // 
            this.lblNpc.AutoSize = true;
            this.lblNpc.Location = new System.Drawing.Point(7, 24);
            this.lblNpc.Name = "lblNpc";
            this.lblNpc.Size = new System.Drawing.Size(29, 13);
            this.lblNpc.TabIndex = 2;
            this.lblNpc.Text = "NPC";
            // 
            // lblNpcQuantity
            // 
            this.lblNpcQuantity.AutoSize = true;
            this.lblNpcQuantity.Location = new System.Drawing.Point(7, 56);
            this.lblNpcQuantity.Name = "lblNpcQuantity";
            this.lblNpcQuantity.Size = new System.Drawing.Size(46, 13);
            this.lblNpcQuantity.TabIndex = 0;
            this.lblNpcQuantity.Text = "Amount:";
            // 
            // grpGatherItems
            // 
            this.grpGatherItems.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpGatherItems.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpGatherItems.Controls.Add(this.nudItemAmount);
            this.grpGatherItems.Controls.Add(this.cmbItem);
            this.grpGatherItems.Controls.Add(this.lblItem);
            this.grpGatherItems.Controls.Add(this.lblItemQuantity);
            this.grpGatherItems.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpGatherItems.Location = new System.Drawing.Point(9, 112);
            this.grpGatherItems.Name = "grpGatherItems";
            this.grpGatherItems.Size = new System.Drawing.Size(236, 83);
            this.grpGatherItems.TabIndex = 25;
            this.grpGatherItems.TabStop = false;
            this.grpGatherItems.Text = "Gather Item(s)";
            this.grpGatherItems.Visible = false;
            // 
            // nudItemAmount
            // 
            this.nudItemAmount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudItemAmount.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudItemAmount.Location = new System.Drawing.Point(104, 52);
            this.nudItemAmount.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudItemAmount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudItemAmount.Name = "nudItemAmount";
            this.nudItemAmount.Size = new System.Drawing.Size(116, 20);
            this.nudItemAmount.TabIndex = 63;
            this.nudItemAmount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
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
            this.cmbItem.Location = new System.Drawing.Point(104, 18);
            this.cmbItem.Name = "cmbItem";
            this.cmbItem.Size = new System.Drawing.Size(116, 21);
            this.cmbItem.TabIndex = 3;
            this.cmbItem.Text = "Equal To";
            this.cmbItem.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // lblItem
            // 
            this.lblItem.AutoSize = true;
            this.lblItem.Location = new System.Drawing.Point(7, 21);
            this.lblItem.Name = "lblItem";
            this.lblItem.Size = new System.Drawing.Size(30, 13);
            this.lblItem.TabIndex = 2;
            this.lblItem.Text = "Item:";
            // 
            // lblItemQuantity
            // 
            this.lblItemQuantity.AutoSize = true;
            this.lblItemQuantity.Location = new System.Drawing.Point(7, 56);
            this.lblItemQuantity.Name = "lblItemQuantity";
            this.lblItemQuantity.Size = new System.Drawing.Size(46, 13);
            this.lblItemQuantity.TabIndex = 0;
            this.lblItemQuantity.Text = "Amount:";
            // 
            // lblEventDriven
            // 
            this.lblEventDriven.Location = new System.Drawing.Point(13, 126);
            this.lblEventDriven.Name = "lblEventDriven";
            this.lblEventDriven.Size = new System.Drawing.Size(226, 56);
            this.lblEventDriven.TabIndex = 29;
            this.lblEventDriven.Text = "Event Driven: The description should lead the player to an event. The event will " +
    "then complete the task using the complete quest task command.";
            // 
            // QuestTaskEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpEditor);
            this.Name = "QuestTaskEditor";
            this.Size = new System.Drawing.Size(255, 267);
            this.grpEditor.ResumeLayout(false);
            this.grpEditor.PerformLayout();
            this.grpKillNpcs.ResumeLayout(false);
            this.grpKillNpcs.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudNpcQuantity)).EndInit();
            this.grpGatherItems.ResumeLayout(false);
            this.grpGatherItems.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudItemAmount)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpEditor;
        private DarkButton btnSave;
        private DarkComboBox cmbTaskType;
        private System.Windows.Forms.Label lblType;
        private DarkButton btnCancel;
        private DarkGroupBox grpGatherItems;
        private DarkComboBox cmbItem;
        private System.Windows.Forms.Label lblItem;
        private System.Windows.Forms.Label lblItemQuantity;
        private System.Windows.Forms.Label lblDesc;
        private DarkTextBox txtStartDesc;
        private DarkGroupBox grpKillNpcs;
        private DarkComboBox cmbNpc;
        private System.Windows.Forms.Label lblNpc;
        private System.Windows.Forms.Label lblNpcQuantity;
        private System.Windows.Forms.Label lblEventDriven;
        private DarkButton btnEditTaskEvent;
        private DarkNumericUpDown nudItemAmount;
        private DarkNumericUpDown nudNpcQuantity;
    }
}
