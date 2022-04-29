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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EventCommandEquipItems));
            this.grpEquipItem = new DarkUI.Controls.DarkGroupBox();
            this.cmbItem = new DarkUI.Controls.DarkComboBox();
            this.lblItem = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.chkUnequip = new DarkUI.Controls.DarkCheckBox();
            this.grpEquipItem.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpEquipItem
            // 
            this.grpEquipItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpEquipItem.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpEquipItem.Controls.Add(this.chkUnequip);
            this.grpEquipItem.Controls.Add(this.cmbItem);
            this.grpEquipItem.Controls.Add(this.lblItem);
            this.grpEquipItem.Controls.Add(this.btnCancel);
            this.grpEquipItem.Controls.Add(this.btnSave);
            this.grpEquipItem.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpEquipItem.Location = new System.Drawing.Point(3, 3);
            this.grpEquipItem.Name = "grpEquipItem";
            this.grpEquipItem.Size = new System.Drawing.Size(193, 111);
            this.grpEquipItem.TabIndex = 17;
            this.grpEquipItem.TabStop = false;
            this.grpEquipItem.Text = "Equip/Unequip Player Items:";
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
            this.cmbItem.Location = new System.Drawing.Point(65, 23);
            this.cmbItem.Name = "cmbItem";
            this.cmbItem.Size = new System.Drawing.Size(115, 21);
            this.cmbItem.TabIndex = 24;
            this.cmbItem.Text = null;
            this.cmbItem.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // lblItem
            // 
            this.lblItem.AutoSize = true;
            this.lblItem.Location = new System.Drawing.Point(6, 25);
            this.lblItem.Name = "lblItem";
            this.lblItem.Size = new System.Drawing.Size(30, 13);
            this.lblItem.TabIndex = 23;
            this.lblItem.Text = "Item:";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(105, 78);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(9, 78);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(5);
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Ok";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // chkUnequip
            // 
            this.chkUnequip.AutoSize = true;
            this.chkUnequip.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkUnequip.Location = new System.Drawing.Point(6, 50);
            this.chkUnequip.Name = "chkUnequip";
            this.chkUnequip.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkUnequip.Size = new System.Drawing.Size(72, 17);
            this.chkUnequip.TabIndex = 57;
            this.chkUnequip.Text = "Unequip?";
            // 
            // EventCommandEquipItems
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpEquipItem);
            this.Name = "EventCommandEquipItems";
            this.Size = new System.Drawing.Size(205, 124);
            this.grpEquipItem.ResumeLayout(false);
            this.grpEquipItem.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpEquipItem;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private DarkComboBox cmbItem;
        private System.Windows.Forms.Label lblItem;
        private DarkCheckBox chkUnequip;
    }
}
