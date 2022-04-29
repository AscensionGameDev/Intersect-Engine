namespace Intersect.Editor.Forms.Editors
{
    partial class FrmDynamicRequirements
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
            this.grpConditionLists = new DarkUI.Controls.DarkGroupBox();
            this.btnRemoveList = new DarkUI.Controls.DarkButton();
            this.btnAddList = new DarkUI.Controls.DarkButton();
            this.lstConditionLists = new System.Windows.Forms.ListBox();
            this.lblInstructions = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.grpConditionList = new DarkUI.Controls.DarkGroupBox();
            this.txtListName = new DarkUI.Controls.DarkTextBox();
            this.lblListName = new System.Windows.Forms.Label();
            this.btnRemoveCondition = new DarkUI.Controls.DarkButton();
            this.btnAddCondition = new DarkUI.Controls.DarkButton();
            this.lstConditions = new System.Windows.Forms.ListBox();
            this.grpConditionLists.SuspendLayout();
            this.grpConditionList.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpConditionLists
            // 
            this.grpConditionLists.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpConditionLists.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpConditionLists.Controls.Add(this.btnRemoveList);
            this.grpConditionLists.Controls.Add(this.btnAddList);
            this.grpConditionLists.Controls.Add(this.lstConditionLists);
            this.grpConditionLists.Controls.Add(this.lblInstructions);
            this.grpConditionLists.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpConditionLists.Location = new System.Drawing.Point(13, 12);
            this.grpConditionLists.Name = "grpConditionLists";
            this.grpConditionLists.Size = new System.Drawing.Size(259, 208);
            this.grpConditionLists.TabIndex = 0;
            this.grpConditionLists.TabStop = false;
            this.grpConditionLists.Text = "Condition Lists";
            // 
            // btnRemoveList
            // 
            this.btnRemoveList.Location = new System.Drawing.Point(141, 177);
            this.btnRemoveList.Name = "btnRemoveList";
            this.btnRemoveList.Padding = new System.Windows.Forms.Padding(5);
            this.btnRemoveList.Size = new System.Drawing.Size(112, 23);
            this.btnRemoveList.TabIndex = 3;
            this.btnRemoveList.Text = "Remove List";
            this.btnRemoveList.Click += new System.EventHandler(this.btnRemoveList_Click);
            // 
            // btnAddList
            // 
            this.btnAddList.Location = new System.Drawing.Point(10, 177);
            this.btnAddList.Name = "btnAddList";
            this.btnAddList.Padding = new System.Windows.Forms.Padding(5);
            this.btnAddList.Size = new System.Drawing.Size(112, 23);
            this.btnAddList.TabIndex = 2;
            this.btnAddList.Text = "Add List";
            this.btnAddList.Click += new System.EventHandler(this.btnAddList_Click);
            // 
            // lstConditionLists
            // 
            this.lstConditionLists.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.lstConditionLists.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstConditionLists.ForeColor = System.Drawing.Color.Gainsboro;
            this.lstConditionLists.FormattingEnabled = true;
            this.lstConditionLists.Location = new System.Drawing.Point(10, 78);
            this.lstConditionLists.Name = "lstConditionLists";
            this.lstConditionLists.Size = new System.Drawing.Size(243, 93);
            this.lstConditionLists.TabIndex = 1;
            this.lstConditionLists.Click += new System.EventHandler(this.lstConditionLists_Click);
            this.lstConditionLists.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lstConditionLists_KeyDown);
            // 
            // lblInstructions
            // 
            this.lblInstructions.Location = new System.Drawing.Point(7, 20);
            this.lblInstructions.Name = "lblInstructions";
            this.lblInstructions.Size = new System.Drawing.Size(246, 55);
            this.lblInstructions.TabIndex = 0;
            this.lblInstructions.Text = "Below are condition lists. If conditions are met on any of the lists then the pla" +
    "yer can do XYZ.";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(470, 230);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(389, 230);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(5);
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // grpConditionList
            // 
            this.grpConditionList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpConditionList.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpConditionList.Controls.Add(this.txtListName);
            this.grpConditionList.Controls.Add(this.lblListName);
            this.grpConditionList.Controls.Add(this.btnRemoveCondition);
            this.grpConditionList.Controls.Add(this.btnAddCondition);
            this.grpConditionList.Controls.Add(this.lstConditions);
            this.grpConditionList.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpConditionList.Location = new System.Drawing.Point(286, 12);
            this.grpConditionList.Name = "grpConditionList";
            this.grpConditionList.Size = new System.Drawing.Size(259, 208);
            this.grpConditionList.TabIndex = 6;
            this.grpConditionList.TabStop = false;
            this.grpConditionList.Text = "Conditions";
            // 
            // txtListName
            // 
            this.txtListName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtListName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtListName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtListName.Location = new System.Drawing.Point(51, 22);
            this.txtListName.Name = "txtListName";
            this.txtListName.Size = new System.Drawing.Size(201, 20);
            this.txtListName.TabIndex = 7;
            this.txtListName.TextChanged += new System.EventHandler(this.txtListName_TextChanged);
            // 
            // lblListName
            // 
            this.lblListName.AutoSize = true;
            this.lblListName.Location = new System.Drawing.Point(7, 24);
            this.lblListName.Name = "lblListName";
            this.lblListName.Size = new System.Drawing.Size(35, 13);
            this.lblListName.TabIndex = 6;
            this.lblListName.Text = "Desc:";
            // 
            // btnRemoveCondition
            // 
            this.btnRemoveCondition.Location = new System.Drawing.Point(140, 178);
            this.btnRemoveCondition.Name = "btnRemoveCondition";
            this.btnRemoveCondition.Padding = new System.Windows.Forms.Padding(5);
            this.btnRemoveCondition.Size = new System.Drawing.Size(112, 23);
            this.btnRemoveCondition.TabIndex = 3;
            this.btnRemoveCondition.Text = "Remove Condition";
            this.btnRemoveCondition.Click += new System.EventHandler(this.btnRemoveCondition_Click);
            // 
            // btnAddCondition
            // 
            this.btnAddCondition.Location = new System.Drawing.Point(9, 178);
            this.btnAddCondition.Name = "btnAddCondition";
            this.btnAddCondition.Padding = new System.Windows.Forms.Padding(5);
            this.btnAddCondition.Size = new System.Drawing.Size(112, 23);
            this.btnAddCondition.TabIndex = 2;
            this.btnAddCondition.Text = "Add Condition";
            this.btnAddCondition.Click += new System.EventHandler(this.btnAddCondition_Click);
            // 
            // lstConditions
            // 
            this.lstConditions.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.lstConditions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstConditions.ForeColor = System.Drawing.Color.Gainsboro;
            this.lstConditions.FormattingEnabled = true;
            this.lstConditions.Location = new System.Drawing.Point(9, 78);
            this.lstConditions.Name = "lstConditions";
            this.lstConditions.Size = new System.Drawing.Size(243, 93);
            this.lstConditions.TabIndex = 1;
            this.lstConditions.DoubleClick += new System.EventHandler(this.lstConditions_DoubleClick);
            this.lstConditions.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lstConditions_KeyDown);
            // 
            // FrmDynamicRequirements
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.ClientSize = new System.Drawing.Size(553, 262);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.grpConditionList);
            this.Controls.Add(this.grpConditionLists);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmDynamicRequirements";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Dynamic Requirements";
            this.grpConditionLists.ResumeLayout(false);
            this.grpConditionList.ResumeLayout(false);
            this.grpConditionList.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkUI.Controls.DarkGroupBox grpConditionLists;
        private DarkUI.Controls.DarkButton btnCancel;
        private DarkUI.Controls.DarkButton btnSave;
        private DarkUI.Controls.DarkButton btnRemoveList;
        private DarkUI.Controls.DarkButton btnAddList;
        private System.Windows.Forms.ListBox lstConditionLists;
        private System.Windows.Forms.Label lblInstructions;
        private DarkUI.Controls.DarkGroupBox grpConditionList;
        private DarkUI.Controls.DarkButton btnRemoveCondition;
        private DarkUI.Controls.DarkButton btnAddCondition;
        private System.Windows.Forms.ListBox lstConditions;
        private DarkUI.Controls.DarkTextBox txtListName;
        private System.Windows.Forms.Label lblListName;
    }
}