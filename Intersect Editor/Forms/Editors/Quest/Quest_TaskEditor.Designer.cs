using DarkUI.Controls;

namespace Intersect_Editor.Forms.Editors.Quest
{
    partial class Quest_TaskEditor
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
            this.groupBox1 = new DarkUI.Controls.DarkGroupBox();
            this.btnEditTaskEvent = new DarkUI.Controls.DarkButton();
            this.txtStartDesc = new DarkUI.Controls.DarkTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.cmbConditionType = new DarkUI.Controls.DarkComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.grpGatherItems = new DarkUI.Controls.DarkGroupBox();
            this.cmbItem = new DarkUI.Controls.DarkComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.scrlItemQuantity = new System.Windows.Forms.HScrollBar();
            this.lblItemQuantity = new System.Windows.Forms.Label();
            this.grpKillNpcs = new DarkUI.Controls.DarkGroupBox();
            this.cmbNpc = new DarkUI.Controls.DarkComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.scrlNpcQuantity = new System.Windows.Forms.HScrollBar();
            this.lblNpcQuantity = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.grpGatherItems.SuspendLayout();
            this.grpKillNpcs.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.groupBox1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.groupBox1.Controls.Add(this.btnEditTaskEvent);
            this.groupBox1.Controls.Add(this.txtStartDesc);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Controls.Add(this.cmbConditionType);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.grpGatherItems);
            this.groupBox1.Controls.Add(this.grpKillNpcs);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.ForeColor = System.Drawing.Color.Gainsboro;
            this.groupBox1.Location = new System.Drawing.Point(-1, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(256, 261);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Task Editor";
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
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 26;
            this.label2.Text = "Desc:";
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
            // cmbConditionType
            // 
            this.cmbConditionType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbConditionType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbConditionType.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbConditionType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbConditionType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbConditionType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbConditionType.FormattingEnabled = true;
            this.cmbConditionType.Items.AddRange(new object[] {
            "Event Driven",
            "Gather Item(s)",
            "Kill NPC(s)"});
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
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Task Type:";
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
            // grpGatherItems
            // 
            this.grpGatherItems.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpGatherItems.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpGatherItems.Controls.Add(this.cmbItem);
            this.grpGatherItems.Controls.Add(this.label8);
            this.grpGatherItems.Controls.Add(this.scrlItemQuantity);
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
            // cmbItem
            // 
            this.cmbItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbItem.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbItem.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbItem.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbItem.FormattingEnabled = true;
            this.cmbItem.Items.AddRange(new object[] {
            "Equal To",
            "Greater Than or Equal To",
            "Less Than or Equal To",
            "Greater Than",
            "Less Than",
            "Does Not Equal"});
            this.cmbItem.Location = new System.Drawing.Point(104, 18);
            this.cmbItem.Name = "cmbItem";
            this.cmbItem.Size = new System.Drawing.Size(116, 21);
            this.cmbItem.TabIndex = 3;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 21);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(30, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "Item:";
            // 
            // scrlItemQuantity
            // 
            this.scrlItemQuantity.Location = new System.Drawing.Point(104, 54);
            this.scrlItemQuantity.Maximum = 32000;
            this.scrlItemQuantity.Minimum = 1;
            this.scrlItemQuantity.Name = "scrlItemQuantity";
            this.scrlItemQuantity.Size = new System.Drawing.Size(116, 17);
            this.scrlItemQuantity.TabIndex = 4;
            this.scrlItemQuantity.Value = 1;
            this.scrlItemQuantity.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlItemQuantity_Scroll);
            // 
            // lblItemQuantity
            // 
            this.lblItemQuantity.AutoSize = true;
            this.lblItemQuantity.Location = new System.Drawing.Point(7, 56);
            this.lblItemQuantity.Name = "lblItemQuantity";
            this.lblItemQuantity.Size = new System.Drawing.Size(55, 13);
            this.lblItemQuantity.TabIndex = 0;
            this.lblItemQuantity.Text = "Amount: 1";
            // 
            // grpKillNpcs
            // 
            this.grpKillNpcs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpKillNpcs.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpKillNpcs.Controls.Add(this.cmbNpc);
            this.grpKillNpcs.Controls.Add(this.label3);
            this.grpKillNpcs.Controls.Add(this.scrlNpcQuantity);
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
            // cmbNpc
            // 
            this.cmbNpc.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbNpc.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbNpc.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbNpc.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbNpc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbNpc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbNpc.FormattingEnabled = true;
            this.cmbNpc.Items.AddRange(new object[] {
            "Equal To",
            "Greater Than or Equal To",
            "Less Than or Equal To",
            "Greater Than",
            "Less Than",
            "Does Not Equal"});
            this.cmbNpc.Location = new System.Drawing.Point(104, 21);
            this.cmbNpc.Name = "cmbNpc";
            this.cmbNpc.Size = new System.Drawing.Size(116, 21);
            this.cmbNpc.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "NPC";
            // 
            // scrlNpcQuantity
            // 
            this.scrlNpcQuantity.Location = new System.Drawing.Point(104, 54);
            this.scrlNpcQuantity.Maximum = 32000;
            this.scrlNpcQuantity.Minimum = 1;
            this.scrlNpcQuantity.Name = "scrlNpcQuantity";
            this.scrlNpcQuantity.Size = new System.Drawing.Size(116, 17);
            this.scrlNpcQuantity.TabIndex = 4;
            this.scrlNpcQuantity.Value = 1;
            this.scrlNpcQuantity.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlNpcQuantity_Scroll);
            // 
            // lblNpcQuantity
            // 
            this.lblNpcQuantity.AutoSize = true;
            this.lblNpcQuantity.Location = new System.Drawing.Point(7, 56);
            this.lblNpcQuantity.Name = "lblNpcQuantity";
            this.lblNpcQuantity.Size = new System.Drawing.Size(55, 13);
            this.lblNpcQuantity.TabIndex = 0;
            this.lblNpcQuantity.Text = "Amount: 1";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(13, 126);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(226, 56);
            this.label5.TabIndex = 29;
            this.label5.Text = "Event Driven: The description should lead the player to an event. The event will " +
    "then complete the task using the complete quest task command.";
            // 
            // Quest_TaskEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.groupBox1);
            this.Name = "Quest_TaskEditor";
            this.Size = new System.Drawing.Size(255, 267);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpGatherItems.ResumeLayout(false);
            this.grpGatherItems.PerformLayout();
            this.grpKillNpcs.ResumeLayout(false);
            this.grpKillNpcs.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox groupBox1;
        private DarkButton btnSave;
        private DarkComboBox cmbConditionType;
        private System.Windows.Forms.Label label1;
        private DarkButton btnCancel;
        private DarkGroupBox grpGatherItems;
        private System.Windows.Forms.HScrollBar scrlItemQuantity;
        private DarkComboBox cmbItem;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblItemQuantity;
        private System.Windows.Forms.Label label2;
        private DarkTextBox txtStartDesc;
        private DarkGroupBox grpKillNpcs;
        private DarkComboBox cmbNpc;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.HScrollBar scrlNpcQuantity;
        private System.Windows.Forms.Label lblNpcQuantity;
        private System.Windows.Forms.Label label5;
        private DarkButton btnEditTaskEvent;
    }
}
