using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    partial class EventCommandInput
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
            this.grpInput = new DarkUI.Controls.DarkGroupBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.txtTitle = new DarkUI.Controls.DarkTextBox();
            this.nudMaxVal = new DarkUI.Controls.DarkNumericUpDown();
            this.lblMaxVal = new System.Windows.Forms.Label();
            this.nudMinVal = new DarkUI.Controls.DarkNumericUpDown();
            this.lblMinVal = new System.Windows.Forms.Label();
            this.cmbVariable = new DarkUI.Controls.DarkComboBox();
            this.rdoGlobalVariables = new DarkUI.Controls.DarkRadioButton();
            this.rdoGuildVariables = new DarkUI.Controls.DarkRadioButton();
            this.rdoPlayerVariables = new DarkUI.Controls.DarkRadioButton();
            this.lblCommands = new System.Windows.Forms.Label();
            this.txtText = new DarkUI.Controls.DarkTextBox();
            this.lblText = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.grpInput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxVal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinVal)).BeginInit();
            this.SuspendLayout();
            // 
            // grpInput
            // 
            this.grpInput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpInput.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpInput.Controls.Add(this.lblTitle);
            this.grpInput.Controls.Add(this.txtTitle);
            this.grpInput.Controls.Add(this.nudMaxVal);
            this.grpInput.Controls.Add(this.lblMaxVal);
            this.grpInput.Controls.Add(this.nudMinVal);
            this.grpInput.Controls.Add(this.lblMinVal);
            this.grpInput.Controls.Add(this.cmbVariable);
            this.grpInput.Controls.Add(this.rdoGuildVariables);
            this.grpInput.Controls.Add(this.rdoPlayerVariables);
            this.grpInput.Controls.Add(this.lblCommands);
            this.grpInput.Controls.Add(this.txtText);
            this.grpInput.Controls.Add(this.lblText);
            this.grpInput.Controls.Add(this.btnCancel);
            this.grpInput.Controls.Add(this.btnSave);
            this.grpInput.Controls.Add(this.rdoGlobalVariables);
            this.grpInput.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpInput.Location = new System.Drawing.Point(3, 3);
            this.grpInput.Name = "grpInput";
            this.grpInput.Size = new System.Drawing.Size(259, 305);
            this.grpInput.TabIndex = 17;
            this.grpInput.TabStop = false;
            this.grpInput.Text = "Input Variable";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(4, 21);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(30, 13);
            this.lblTitle.TabIndex = 63;
            this.lblTitle.Text = "Title:";
            // 
            // txtTitle
            // 
            this.txtTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtTitle.Location = new System.Drawing.Point(51, 19);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(190, 20);
            this.txtTitle.TabIndex = 62;
            // 
            // nudMaxVal
            // 
            this.nudMaxVal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudMaxVal.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudMaxVal.Location = new System.Drawing.Point(126, 244);
            this.nudMaxVal.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudMaxVal.Name = "nudMaxVal";
            this.nudMaxVal.Size = new System.Drawing.Size(115, 20);
            this.nudMaxVal.TabIndex = 53;
            this.nudMaxVal.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // lblMaxVal
            // 
            this.lblMaxVal.AutoSize = true;
            this.lblMaxVal.Location = new System.Drawing.Point(11, 246);
            this.lblMaxVal.Name = "lblMaxVal";
            this.lblMaxVal.Size = new System.Drawing.Size(84, 13);
            this.lblMaxVal.TabIndex = 52;
            this.lblMaxVal.Text = "Maximum Value:";
            // 
            // nudMinVal
            // 
            this.nudMinVal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudMinVal.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudMinVal.Location = new System.Drawing.Point(126, 218);
            this.nudMinVal.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudMinVal.Name = "nudMinVal";
            this.nudMinVal.Size = new System.Drawing.Size(115, 20);
            this.nudMinVal.TabIndex = 51;
            this.nudMinVal.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // lblMinVal
            // 
            this.lblMinVal.AutoSize = true;
            this.lblMinVal.Location = new System.Drawing.Point(11, 220);
            this.lblMinVal.Name = "lblMinVal";
            this.lblMinVal.Size = new System.Drawing.Size(81, 13);
            this.lblMinVal.TabIndex = 50;
            this.lblMinVal.Text = "Minimum Value:";
            // 
            // cmbVariable
            // 
            this.cmbVariable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbVariable.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbVariable.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbVariable.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbVariable.DrawDropdownHoverOutline = false;
            this.cmbVariable.DrawFocusRectangle = false;
            this.cmbVariable.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbVariable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVariable.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbVariable.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbVariable.FormattingEnabled = true;
            this.cmbVariable.Location = new System.Drawing.Point(7, 188);
            this.cmbVariable.Name = "cmbVariable";
            this.cmbVariable.Size = new System.Drawing.Size(234, 21);
            this.cmbVariable.TabIndex = 49;
            this.cmbVariable.Text = null;
            this.cmbVariable.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbVariable.SelectedIndexChanged += new System.EventHandler(this.cmbVariable_SelectedIndexChanged);
            // 
            // rdoGlobalVariables
            // 
            this.rdoGlobalVariables.AutoSize = true;
            this.rdoGlobalVariables.Location = new System.Drawing.Point(131, 144);
            this.rdoGlobalVariables.Name = "rdoGlobalVariables";
            this.rdoGlobalVariables.Size = new System.Drawing.Size(101, 17);
            this.rdoGlobalVariables.TabIndex = 28;
            this.rdoGlobalVariables.Text = "Global Variables";
            this.rdoGlobalVariables.CheckedChanged += new System.EventHandler(this.rdoGlobalVariables_CheckedChanged);
            // 
            // rdoGuildVariables
            // 
            this.rdoGuildVariables.AutoSize = true;
            this.rdoGuildVariables.Location = new System.Drawing.Point(14, 165);
            this.rdoGuildVariables.Name = "rdoGuildVariables";
            this.rdoGuildVariables.Size = new System.Drawing.Size(95, 17);
            this.rdoGuildVariables.TabIndex = 28;
            this.rdoGuildVariables.Text = "Guild Variables";
            this.rdoGuildVariables.CheckedChanged += new System.EventHandler(this.rdoGuildVariables_CheckedChanged);
            // 
            // rdoPlayerVariables
            // 
            this.rdoPlayerVariables.AutoSize = true;
            this.rdoPlayerVariables.Checked = true;
            this.rdoPlayerVariables.Location = new System.Drawing.Point(14, 144);
            this.rdoPlayerVariables.Name = "rdoPlayerVariables";
            this.rdoPlayerVariables.Size = new System.Drawing.Size(100, 17);
            this.rdoPlayerVariables.TabIndex = 27;
            this.rdoPlayerVariables.TabStop = true;
            this.rdoPlayerVariables.Text = "Player Variables";
            this.rdoPlayerVariables.CheckedChanged += new System.EventHandler(this.rdoPlayerVariables_CheckedChanged);
            // 
            // lblCommands
            // 
            this.lblCommands.AutoSize = true;
            this.lblCommands.BackColor = System.Drawing.Color.Transparent;
            this.lblCommands.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCommands.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.lblCommands.Location = new System.Drawing.Point(157, 49);
            this.lblCommands.Name = "lblCommands";
            this.lblCommands.Size = new System.Drawing.Size(84, 13);
            this.lblCommands.TabIndex = 26;
            this.lblCommands.Text = "Chat Commands";
            this.lblCommands.Click += new System.EventHandler(this.lblCommands_Click);
            // 
            // txtText
            // 
            this.txtText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtText.Location = new System.Drawing.Point(7, 65);
            this.txtText.Multiline = true;
            this.txtText.Name = "txtText";
            this.txtText.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.txtText.Size = new System.Drawing.Size(234, 73);
            this.txtText.TabIndex = 22;
            // 
            // lblText
            // 
            this.lblText.AutoSize = true;
            this.lblText.Location = new System.Drawing.Point(4, 49);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(31, 13);
            this.lblText.TabIndex = 21;
            this.lblText.Text = "Text:";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(89, 276);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(7, 276);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(5);
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Ok";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // EventCommandInput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpInput);
            this.Name = "EventCommandInput";
            this.Size = new System.Drawing.Size(268, 318);
            this.grpInput.ResumeLayout(false);
            this.grpInput.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxVal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinVal)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpInput;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private DarkTextBox txtText;
        private System.Windows.Forms.Label lblText;
        private System.Windows.Forms.Label lblCommands;
        private DarkRadioButton rdoGlobalVariables;
        private DarkRadioButton rdoGuildVariables;
        private DarkRadioButton rdoPlayerVariables;
        internal DarkComboBox cmbVariable;
        private DarkNumericUpDown nudMaxVal;
        private System.Windows.Forms.Label lblMaxVal;
        private DarkNumericUpDown nudMinVal;
        private System.Windows.Forms.Label lblMinVal;
        private System.Windows.Forms.Label lblTitle;
        private DarkTextBox txtTitle;
    }
}
