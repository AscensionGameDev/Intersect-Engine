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
            grpConditional = new DarkGroupBox();
            chkHasElse = new DarkCheckBox();
            chkNegated = new DarkCheckBox();
            btnSave = new DarkButton();
            cmbConditionType = new DarkComboBox();
            lblType = new Label();
            btnCancel = new DarkButton();
            pnlConditionControl = new Panel();
            grpConditional.SuspendLayout();
            SuspendLayout();
            // 
            // grpConditional
            // 
            grpConditional.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            grpConditional.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpConditional.Controls.Add(pnlConditionControl);
            grpConditional.Controls.Add(chkHasElse);
            grpConditional.Controls.Add(chkNegated);
            grpConditional.Controls.Add(btnSave);
            grpConditional.Controls.Add(cmbConditionType);
            grpConditional.Controls.Add(lblType);
            grpConditional.Controls.Add(btnCancel);
            grpConditional.ForeColor = System.Drawing.Color.Gainsboro;
            grpConditional.Location = new System.Drawing.Point(4, 3);
            grpConditional.Margin = new Padding(4, 3, 4, 3);
            grpConditional.Name = "grpConditional";
            grpConditional.Padding = new Padding(4, 3, 4, 3);
            grpConditional.Size = new Size(324, 542);
            grpConditional.TabIndex = 17;
            grpConditional.TabStop = false;
            grpConditional.Text = "Conditional";
            // 
            // chkHasElse
            // 
            chkHasElse.Location = new System.Drawing.Point(127, 477);
            chkHasElse.Margin = new Padding(4, 3, 4, 3);
            chkHasElse.Name = "chkHasElse";
            chkHasElse.Size = new Size(84, 20);
            chkHasElse.TabIndex = 56;
            chkHasElse.Text = "Has Else";
            // 
            // chkNegated
            // 
            chkNegated.Location = new System.Drawing.Point(229, 477);
            chkNegated.Margin = new Padding(4, 3, 4, 3);
            chkNegated.Name = "chkNegated";
            chkNegated.Size = new Size(84, 20);
            chkNegated.TabIndex = 34;
            chkNegated.Text = "Negated";
            // 
            // btnSave
            // 
            btnSave.Location = new System.Drawing.Point(10, 509);
            btnSave.Margin = new Padding(4, 3, 4, 3);
            btnSave.Name = "btnSave";
            btnSave.Padding = new Padding(6);
            btnSave.Size = new Size(88, 27);
            btnSave.TabIndex = 19;
            btnSave.Text = "Ok";
            btnSave.Click += btnSave_Click;
            // 
            // cmbConditionType
            // 
            cmbConditionType.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbConditionType.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbConditionType.BorderStyle = ButtonBorderStyle.Solid;
            cmbConditionType.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbConditionType.DrawDropdownHoverOutline = false;
            cmbConditionType.DrawFocusRectangle = false;
            cmbConditionType.DrawMode = DrawMode.OwnerDrawFixed;
            cmbConditionType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbConditionType.FlatStyle = FlatStyle.Flat;
            cmbConditionType.ForeColor = System.Drawing.Color.Gainsboro;
            cmbConditionType.FormattingEnabled = true;
            cmbConditionType.Items.AddRange(new object[] { "Variable is...", "Has item...", "Class is...", "Knows spell...", "Level is....", "Self Switch is....", "Power level is....", "Time is between....", "Can Start Quest....", "Quest In Progress....", "Quest Completed....", "Player death...", "No NPCs on the map...", "Gender is...", "Item Equipped Is...", "Has X free Inventory slots...", "In Guild With At Least Rank...", "Check Equipped Slot..." });
            cmbConditionType.Location = new System.Drawing.Point(103, 15);
            cmbConditionType.Margin = new Padding(4, 3, 4, 3);
            cmbConditionType.Name = "cmbConditionType";
            cmbConditionType.Size = new Size(213, 24);
            cmbConditionType.TabIndex = 22;
            cmbConditionType.Text = "Variable is...";
            cmbConditionType.TextPadding = new Padding(2);
            cmbConditionType.SelectedIndexChanged += cmbConditionType_SelectedIndexChanged;
            // 
            // lblType
            // 
            lblType.AutoSize = true;
            lblType.Location = new System.Drawing.Point(7, 18);
            lblType.Margin = new Padding(4, 0, 4, 0);
            lblType.Name = "lblType";
            lblType.Size = new Size(90, 15);
            lblType.TabIndex = 21;
            lblType.Text = "Condition Type:";
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(128, 509);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Padding = new Padding(6);
            btnCancel.Size = new Size(88, 27);
            btnCancel.TabIndex = 20;
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // pnlConditionControl
            // 
            pnlConditionControl.Location = new System.Drawing.Point(7, 45);
            pnlConditionControl.Name = "pnlConditionControl";
            pnlConditionControl.Size = new Size(310, 426);
            pnlConditionControl.TabIndex = 57;
            // 
            // EventCommandConditionalBranch
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            Controls.Add(grpConditional);
            Margin = new Padding(4, 3, 4, 3);
            Name = "EventCommandConditionalBranch";
            Size = new Size(332, 553);
            grpConditional.ResumeLayout(false);
            grpConditional.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private DarkGroupBox grpConditional;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private DarkComboBox cmbConditionType;
        private System.Windows.Forms.Label lblType;
        private DarkCheckBox chkNegated;
        private DarkCheckBox chkHasElse;
        private Panel pnlConditionControl;
    }
}
