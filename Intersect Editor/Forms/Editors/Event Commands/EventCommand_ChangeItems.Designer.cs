using DarkUI.Controls;

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
            this.grpChangeItems = new DarkUI.Controls.DarkGroupBox();
            this.scrlAmount = new DarkScrollBar();
            this.lblAmount = new System.Windows.Forms.Label();
            this.cmbItem = new DarkUI.Controls.DarkComboBox();
            this.lblItem = new System.Windows.Forms.Label();
            this.cmbAction = new DarkUI.Controls.DarkComboBox();
            this.lblAction = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.grpChangeItems.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpChangeItems
            // 
            this.grpChangeItems.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpChangeItems.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpChangeItems.Controls.Add(this.scrlAmount);
            this.grpChangeItems.Controls.Add(this.lblAmount);
            this.grpChangeItems.Controls.Add(this.cmbItem);
            this.grpChangeItems.Controls.Add(this.lblItem);
            this.grpChangeItems.Controls.Add(this.cmbAction);
            this.grpChangeItems.Controls.Add(this.lblAction);
            this.grpChangeItems.Controls.Add(this.btnCancel);
            this.grpChangeItems.Controls.Add(this.btnSave);
            this.grpChangeItems.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpChangeItems.Location = new System.Drawing.Point(3, 3);
            this.grpChangeItems.Name = "grpChangeItems";
            this.grpChangeItems.Size = new System.Drawing.Size(193, 133);
            this.grpChangeItems.TabIndex = 17;
            this.grpChangeItems.TabStop = false;
            this.grpChangeItems.Text = "Change Player Items:";
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
            this.scrlAmount.ScrollOrientation = DarkScrollOrientation.Horizontal;
            this.scrlAmount.ValueChanged += this.scrlAmount_Scroll;
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
            // cmbItem
            // 
            this.cmbItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbItem.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbItem.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbItem.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbItem.FormattingEnabled = true;
            this.cmbItem.Location = new System.Drawing.Point(64, 46);
            this.cmbItem.Name = "cmbItem";
            this.cmbItem.Size = new System.Drawing.Size(115, 21);
            this.cmbItem.TabIndex = 24;
            // 
            // lblItem
            // 
            this.lblItem.AutoSize = true;
            this.lblItem.Location = new System.Drawing.Point(5, 48);
            this.lblItem.Name = "lblItem";
            this.lblItem.Size = new System.Drawing.Size(30, 13);
            this.lblItem.TabIndex = 23;
            this.lblItem.Text = "Item:";
            // 
            // cmbAction
            // 
            this.cmbAction.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbAction.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbAction.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbAction.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAction.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbAction.FormattingEnabled = true;
            this.cmbAction.Items.AddRange(new object[] {
            "Give",
            "Take"});
            this.cmbAction.Location = new System.Drawing.Point(64, 19);
            this.cmbAction.Name = "cmbAction";
            this.cmbAction.Size = new System.Drawing.Size(115, 21);
            this.cmbAction.TabIndex = 22;
            // 
            // lblAction
            // 
            this.lblAction.AutoSize = true;
            this.lblAction.Location = new System.Drawing.Point(5, 21);
            this.lblAction.Name = "lblAction";
            this.lblAction.Size = new System.Drawing.Size(40, 13);
            this.lblAction.TabIndex = 21;
            this.lblAction.Text = "Action:";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(89, 104);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(8, 104);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(5);
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Ok";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // EventCommand_ChangeItems
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpChangeItems);
            this.Name = "EventCommand_ChangeItems";
            this.Size = new System.Drawing.Size(205, 139);
            this.grpChangeItems.ResumeLayout(false);
            this.grpChangeItems.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpChangeItems;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private DarkComboBox cmbAction;
        private System.Windows.Forms.Label lblAction;
        private DarkComboBox cmbItem;
        private System.Windows.Forms.Label lblItem;
        private DarkScrollBar scrlAmount;
        private System.Windows.Forms.Label lblAmount;
    }
}
