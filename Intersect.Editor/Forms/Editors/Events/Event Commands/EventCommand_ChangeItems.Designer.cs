using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    partial class EventCommandChangeItems
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EventCommandChangeItems));
            this.grpChangeItems = new DarkUI.Controls.DarkGroupBox();
            this.nudGiveTakeAmount = new DarkUI.Controls.DarkNumericUpDown();
            this.lblAmount = new System.Windows.Forms.Label();
            this.cmbItem = new DarkUI.Controls.DarkComboBox();
            this.lblItem = new System.Windows.Forms.Label();
            this.cmbAction = new DarkUI.Controls.DarkComboBox();
            this.lblAction = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.cmbMethod = new DarkUI.Controls.DarkComboBox();
            this.lblMethod = new System.Windows.Forms.Label();
            this.grpChangeItems.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudGiveTakeAmount)).BeginInit();
            this.SuspendLayout();
            // 
            // grpChangeItems
            // 
            this.grpChangeItems.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpChangeItems.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpChangeItems.Controls.Add(this.cmbMethod);
            this.grpChangeItems.Controls.Add(this.lblMethod);
            this.grpChangeItems.Controls.Add(this.nudGiveTakeAmount);
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
            this.grpChangeItems.Size = new System.Drawing.Size(193, 171);
            this.grpChangeItems.TabIndex = 17;
            this.grpChangeItems.TabStop = false;
            this.grpChangeItems.Text = "Change Player Items:";
            // 
            // nudGiveTakeAmount
            // 
            this.nudGiveTakeAmount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudGiveTakeAmount.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudGiveTakeAmount.Location = new System.Drawing.Point(64, 73);
            this.nudGiveTakeAmount.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudGiveTakeAmount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudGiveTakeAmount.Name = "nudGiveTakeAmount";
            this.nudGiveTakeAmount.Size = new System.Drawing.Size(115, 20);
            this.nudGiveTakeAmount.TabIndex = 26;
            this.nudGiveTakeAmount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudGiveTakeAmount.ValueChanged += new System.EventHandler(this.nudGiveTakeAmount_ValueChanged);
            // 
            // lblAmount
            // 
            this.lblAmount.AutoSize = true;
            this.lblAmount.Location = new System.Drawing.Point(5, 73);
            this.lblAmount.Name = "lblAmount";
            this.lblAmount.Size = new System.Drawing.Size(46, 13);
            this.lblAmount.TabIndex = 25;
            this.lblAmount.Text = "Amount:";
            // 
            // cmbItem
            // 
            this.cmbItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbItem.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbItem.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbItem.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbItem.ButtonIcon = ((System.Drawing.Bitmap)(resources.GetObject("cmbItem.ButtonIcon")));
            this.cmbItem.DrawDropdownHoverOutline = false;
            this.cmbItem.DrawFocusRectangle = false;
            this.cmbItem.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbItem.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbItem.FormattingEnabled = true;
            this.cmbItem.Location = new System.Drawing.Point(64, 46);
            this.cmbItem.Name = "cmbItem";
            this.cmbItem.Size = new System.Drawing.Size(115, 21);
            this.cmbItem.TabIndex = 24;
            this.cmbItem.Text = null;
            this.cmbItem.TextPadding = new System.Windows.Forms.Padding(2);
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
            this.cmbAction.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbAction.ButtonIcon = ((System.Drawing.Bitmap)(resources.GetObject("cmbAction.ButtonIcon")));
            this.cmbAction.DrawDropdownHoverOutline = false;
            this.cmbAction.DrawFocusRectangle = false;
            this.cmbAction.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAction.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbAction.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbAction.FormattingEnabled = true;
            this.cmbAction.Items.AddRange(new object[] {
            "Give",
            "Take"});
            this.cmbAction.Location = new System.Drawing.Point(64, 19);
            this.cmbAction.Name = "cmbAction";
            this.cmbAction.Size = new System.Drawing.Size(115, 21);
            this.cmbAction.TabIndex = 22;
            this.cmbAction.Text = "Give";
            this.cmbAction.TextPadding = new System.Windows.Forms.Padding(2);
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
            this.btnCancel.Location = new System.Drawing.Point(112, 140);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(6, 140);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(5);
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Ok";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // cmbMethod
            // 
            this.cmbMethod.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbMethod.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbMethod.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbMethod.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbMethod.ButtonIcon = ((System.Drawing.Bitmap)(resources.GetObject("cmbMethod.ButtonIcon")));
            this.cmbMethod.DrawDropdownHoverOutline = false;
            this.cmbMethod.DrawFocusRectangle = false;
            this.cmbMethod.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMethod.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbMethod.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbMethod.FormattingEnabled = true;
            this.cmbMethod.Items.AddRange(new object[] {
            "Normal",
            "Allow Overflow",
            "Up to Amount"});
            this.cmbMethod.Location = new System.Drawing.Point(64, 99);
            this.cmbMethod.Name = "cmbMethod";
            this.cmbMethod.Size = new System.Drawing.Size(115, 21);
            this.cmbMethod.TabIndex = 28;
            this.cmbMethod.Text = "Normal";
            this.cmbMethod.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // lblMethod
            // 
            this.lblMethod.AutoSize = true;
            this.lblMethod.Location = new System.Drawing.Point(6, 102);
            this.lblMethod.Name = "lblMethod";
            this.lblMethod.Size = new System.Drawing.Size(46, 13);
            this.lblMethod.TabIndex = 27;
            this.lblMethod.Text = "Method:";
            // 
            // EventCommandChangeItems
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpChangeItems);
            this.Name = "EventCommandChangeItems";
            this.Size = new System.Drawing.Size(205, 181);
            this.grpChangeItems.ResumeLayout(false);
            this.grpChangeItems.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudGiveTakeAmount)).EndInit();
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
        private System.Windows.Forms.Label lblAmount;
        private DarkNumericUpDown nudGiveTakeAmount;
        private DarkComboBox cmbMethod;
        private System.Windows.Forms.Label lblMethod;
    }
}
