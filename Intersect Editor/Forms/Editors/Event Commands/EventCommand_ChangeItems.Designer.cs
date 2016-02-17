namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    partial class EventCommand_ChangeItems
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
            this.cmbItem = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbAction = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblAmount = new System.Windows.Forms.Label();
            this.scrlAmount = new System.Windows.Forms.HScrollBar();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.scrlAmount);
            this.groupBox1.Controls.Add(this.lblAmount);
            this.groupBox1.Controls.Add(this.cmbItem);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cmbAction);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(193, 133);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Change Player Items:";
            // 
            // cmbItem
            // 
            this.cmbItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbItem.FormattingEnabled = true;
            this.cmbItem.Items.AddRange(new object[] {
            "Add",
            "Remove"});
            this.cmbItem.Location = new System.Drawing.Point(64, 46);
            this.cmbItem.Name = "cmbItem";
            this.cmbItem.Size = new System.Drawing.Size(115, 21);
            this.cmbItem.TabIndex = 24;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "Item:";
            // 
            // cmbAction
            // 
            this.cmbAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAction.FormattingEnabled = true;
            this.cmbAction.Items.AddRange(new object[] {
            "Give",
            "Take"});
            this.cmbAction.Location = new System.Drawing.Point(64, 19);
            this.cmbAction.Name = "cmbAction";
            this.cmbAction.Size = new System.Drawing.Size(115, 21);
            this.cmbAction.TabIndex = 22;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(5, 21);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(40, 13);
            this.label16.TabIndex = 21;
            this.label16.Text = "Action:";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(89, 104);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(8, 104);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Ok";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblAmount
            // 
            this.lblAmount.AutoSize = true;
            this.lblAmount.Location = new System.Drawing.Point(5, 73);
            this.lblAmount.Name = "lblAmount";
            this.lblAmount.Size = new System.Drawing.Size(55, 13);
            this.lblAmount.TabIndex = 25;
            this.lblAmount.Text = "Amount: 1";
            // 
            // scrlAmount
            // 
            this.scrlAmount.Location = new System.Drawing.Point(82, 73);
            this.scrlAmount.Maximum = 32000;
            this.scrlAmount.Minimum = 1;
            this.scrlAmount.Name = "scrlAmount";
            this.scrlAmount.Size = new System.Drawing.Size(97, 17);
            this.scrlAmount.TabIndex = 26;
            this.scrlAmount.Value = 1;
            this.scrlAmount.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlAmount_Scroll);
            // 
            // EventCommand_ChangeItems
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.groupBox1);
            this.Name = "EventCommand_ChangeItems";
            this.Size = new System.Drawing.Size(205, 139);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ComboBox cmbAction;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ComboBox cmbItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.HScrollBar scrlAmount;
        private System.Windows.Forms.Label lblAmount;
    }
}
