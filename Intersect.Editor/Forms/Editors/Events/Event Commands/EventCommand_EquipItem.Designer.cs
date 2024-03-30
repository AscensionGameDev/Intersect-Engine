using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    partial class EventCommandEquipItems
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
            grpEquipItem = new DarkGroupBox();
            optUnequipItem = new DarkRadioButton();
            chkUnequip = new DarkCheckBox();
            chkTriggerCooldown = new DarkCheckBox();
            btnCancel = new DarkButton();
            optUnequipSlot = new DarkRadioButton();
            btnSave = new DarkButton();
            cmbItem = new DarkComboBox();
            lblItem = new Label();
            grpEquipItem.SuspendLayout();
            SuspendLayout();
            // 
            // grpEquipItem
            // 
            grpEquipItem.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            grpEquipItem.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpEquipItem.Controls.Add(optUnequipItem);
            grpEquipItem.Controls.Add(chkUnequip);
            grpEquipItem.Controls.Add(chkTriggerCooldown);
            grpEquipItem.Controls.Add(btnCancel);
            grpEquipItem.Controls.Add(optUnequipSlot);
            grpEquipItem.Controls.Add(btnSave);
            grpEquipItem.Controls.Add(cmbItem);
            grpEquipItem.Controls.Add(lblItem);
            grpEquipItem.ForeColor = System.Drawing.Color.Gainsboro;
            grpEquipItem.Location = new System.Drawing.Point(4, 3);
            grpEquipItem.Margin = new Padding(4, 3, 4, 3);
            grpEquipItem.Name = "grpEquipItem";
            grpEquipItem.Padding = new Padding(4, 3, 4, 3);
            grpEquipItem.Size = new Size(259, 161);
            grpEquipItem.TabIndex = 17;
            grpEquipItem.TabStop = false;
            grpEquipItem.Text = "Equip/Unequip Player Items:";
            // 
            // optUnequipItem
            // 
            optUnequipItem.AutoSize = true;
            optUnequipItem.Location = new System.Drawing.Point(95, 81);
            optUnequipItem.Margin = new Padding(4, 3, 4, 3);
            optUnequipItem.Name = "optUnequipItem";
            optUnequipItem.Size = new Size(49, 19);
            optUnequipItem.TabIndex = 26;
            optUnequipItem.Text = "Item";
            // 
            // chkUnequip
            // 
            chkUnequip.AutoSize = true;
            chkUnequip.CheckAlign = ContentAlignment.MiddleRight;
            chkUnequip.Location = new System.Drawing.Point(11, 81);
            chkUnequip.Margin = new Padding(4, 3, 4, 3);
            chkUnequip.Name = "chkUnequip";
            chkUnequip.RightToLeft = RightToLeft.Yes;
            chkUnequip.Size = new Size(76, 19);
            chkUnequip.TabIndex = 57;
            chkUnequip.Text = "Unequip?";
            chkUnequip.CheckedChanged += chkUnequip_CheckedChanged;
            // 
            // chkTriggerCooldown
            // 
            chkTriggerCooldown.AutoSize = true;
            chkTriggerCooldown.CheckAlign = ContentAlignment.MiddleRight;
            chkTriggerCooldown.Location = new System.Drawing.Point(11, 51);
            chkTriggerCooldown.Margin = new Padding(4, 3, 4, 3);
            chkTriggerCooldown.Name = "chkTriggerCooldown";
            chkTriggerCooldown.RightToLeft = RightToLeft.Yes;
            chkTriggerCooldown.Size = new Size(125, 19);
            chkTriggerCooldown.TabIndex = 58;
            chkTriggerCooldown.Text = "Trigger Cooldown?";
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(161, 120);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Padding = new Padding(6);
            btnCancel.Size = new Size(88, 27);
            btnCancel.TabIndex = 20;
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // optUnequipSlot
            // 
            optUnequipSlot.AutoSize = true;
            optUnequipSlot.Location = new System.Drawing.Point(152, 81);
            optUnequipSlot.Margin = new Padding(4, 3, 4, 3);
            optUnequipSlot.Name = "optUnequipSlot";
            optUnequipSlot.Size = new Size(45, 19);
            optUnequipSlot.TabIndex = 25;
            optUnequipSlot.Text = "Slot";
            optUnequipSlot.CheckedChanged += optUnequipSlot_CheckedChanged;
            // 
            // btnSave
            // 
            btnSave.Location = new System.Drawing.Point(11, 120);
            btnSave.Margin = new Padding(4, 3, 4, 3);
            btnSave.Name = "btnSave";
            btnSave.Padding = new Padding(6);
            btnSave.Size = new Size(88, 27);
            btnSave.TabIndex = 19;
            btnSave.Text = "Ok";
            btnSave.Click += btnSave_Click;
            // 
            // cmbItem
            // 
            cmbItem.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbItem.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbItem.BorderStyle = ButtonBorderStyle.Solid;
            cmbItem.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbItem.DrawDropdownHoverOutline = false;
            cmbItem.DrawFocusRectangle = false;
            cmbItem.DrawMode = DrawMode.OwnerDrawFixed;
            cmbItem.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbItem.FlatStyle = FlatStyle.Flat;
            cmbItem.ForeColor = System.Drawing.Color.Gainsboro;
            cmbItem.FormattingEnabled = true;
            cmbItem.Location = new System.Drawing.Point(56, 21);
            cmbItem.Margin = new Padding(4, 3, 4, 3);
            cmbItem.Name = "cmbItem";
            cmbItem.Size = new Size(193, 24);
            cmbItem.TabIndex = 24;
            cmbItem.Text = null;
            cmbItem.TextPadding = new Padding(2);
            // 
            // lblItem
            // 
            lblItem.AutoSize = true;
            lblItem.Location = new System.Drawing.Point(11, 24);
            lblItem.Margin = new Padding(4, 0, 4, 0);
            lblItem.Name = "lblItem";
            lblItem.Size = new Size(34, 15);
            lblItem.TabIndex = 23;
            lblItem.Text = "Item:";
            // 
            // EventCommandEquipItems
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            Controls.Add(grpEquipItem);
            Margin = new Padding(4, 3, 4, 3);
            Name = "EventCommandEquipItems";
            Size = new Size(268, 170);
            grpEquipItem.ResumeLayout(false);
            grpEquipItem.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private DarkGroupBox grpEquipItem;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private DarkComboBox cmbItem;
        private System.Windows.Forms.Label lblItem;
        private DarkCheckBox chkUnequip;
        private DarkCheckBox chkTriggerCooldown;
        internal DarkRadioButton optUnequipItem;
        internal DarkRadioButton optUnequipSlot;
    }
}
