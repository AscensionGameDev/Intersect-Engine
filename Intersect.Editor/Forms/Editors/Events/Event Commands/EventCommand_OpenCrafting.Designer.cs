using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    partial class EventCommandOpenCraftingTable
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
            grpTable = new DarkGroupBox();
            chkJournalMode = new DarkCheckBox();
            cmbTable = new DarkComboBox();
            lblTable = new Label();
            btnCancel = new DarkButton();
            btnSave = new DarkButton();
            grpTable.SuspendLayout();
            SuspendLayout();
            // 
            // grpTable
            // 
            grpTable.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            grpTable.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpTable.Controls.Add(chkJournalMode);
            grpTable.Controls.Add(cmbTable);
            grpTable.Controls.Add(lblTable);
            grpTable.Controls.Add(btnCancel);
            grpTable.Controls.Add(btnSave);
            grpTable.ForeColor = System.Drawing.Color.Gainsboro;
            grpTable.Location = new System.Drawing.Point(4, -4);
            grpTable.Margin = new Padding(4, 3, 4, 3);
            grpTable.Name = "grpTable";
            grpTable.Padding = new Padding(4, 3, 4, 3);
            grpTable.Size = new Size(272, 124);
            grpTable.TabIndex = 17;
            grpTable.TabStop = false;
            grpTable.Text = "Open Crafting";
            // 
            // chkJournalMode
            // 
            chkJournalMode.AutoSize = true;
            chkJournalMode.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            chkJournalMode.Location = new System.Drawing.Point(8, 52);
            chkJournalMode.Margin = new Padding(4, 3, 4, 3);
            chkJournalMode.Name = "chkJournalMode";
            chkJournalMode.Size = new Size(158, 19);
            chkJournalMode.TabIndex = 24;
            chkJournalMode.Text = "Open as crafting journal?";
            // 
            // cmbTable
            // 
            cmbTable.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbTable.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbTable.BorderStyle = ButtonBorderStyle.Solid;
            cmbTable.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbTable.DrawDropdownHoverOutline = false;
            cmbTable.DrawFocusRectangle = false;
            cmbTable.DrawMode = DrawMode.OwnerDrawFixed;
            cmbTable.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTable.FlatStyle = FlatStyle.Flat;
            cmbTable.ForeColor = System.Drawing.Color.Gainsboro;
            cmbTable.FormattingEnabled = true;
            cmbTable.Location = new System.Drawing.Point(55, 22);
            cmbTable.Margin = new Padding(4, 3, 4, 3);
            cmbTable.Name = "cmbTable";
            cmbTable.Size = new Size(209, 24);
            cmbTable.TabIndex = 22;
            cmbTable.Text = null;
            cmbTable.TextPadding = new Padding(2);
            // 
            // lblTable
            // 
            lblTable.AutoSize = true;
            lblTable.Location = new System.Drawing.Point(5, 25);
            lblTable.Margin = new Padding(4, 0, 4, 0);
            lblTable.Name = "lblTable";
            lblTable.Size = new Size(37, 15);
            lblTable.TabIndex = 21;
            lblTable.Text = "Table:";
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(176, 82);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Padding = new Padding(6);
            btnCancel.Size = new Size(88, 27);
            btnCancel.TabIndex = 20;
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // btnSave
            // 
            btnSave.Location = new System.Drawing.Point(80, 82);
            btnSave.Margin = new Padding(4, 3, 4, 3);
            btnSave.Name = "btnSave";
            btnSave.Padding = new Padding(6);
            btnSave.Size = new Size(88, 27);
            btnSave.TabIndex = 19;
            btnSave.Text = "Ok";
            btnSave.Click += btnSave_Click;
            // 
            // EventCommandOpenCraftingTable
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            Controls.Add(grpTable);
            Margin = new Padding(4, 3, 4, 3);
            Name = "EventCommandOpenCraftingTable";
            Size = new Size(283, 125);
            grpTable.ResumeLayout(false);
            grpTable.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private DarkGroupBox grpTable;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private Label lblTable;
        private DarkComboBox cmbTable;
        private DarkCheckBox chkJournalMode;
    }
}
