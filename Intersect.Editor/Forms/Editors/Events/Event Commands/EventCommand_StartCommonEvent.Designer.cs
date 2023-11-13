using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    partial class EventCommandStartCommonEvent
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
            grpCommonEvent = new DarkGroupBox();
            chkOverworldOverride = new CheckBox();
            chkAllInInstance = new CheckBox();
            cmbEvent = new DarkComboBox();
            lblCommonEvent = new Label();
            btnCancel = new DarkButton();
            btnSave = new DarkButton();
            grpCommonEvent.SuspendLayout();
            SuspendLayout();
            // 
            // grpCommonEvent
            // 
            grpCommonEvent.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            grpCommonEvent.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpCommonEvent.Controls.Add(chkOverworldOverride);
            grpCommonEvent.Controls.Add(chkAllInInstance);
            grpCommonEvent.Controls.Add(cmbEvent);
            grpCommonEvent.Controls.Add(lblCommonEvent);
            grpCommonEvent.Controls.Add(btnCancel);
            grpCommonEvent.Controls.Add(btnSave);
            grpCommonEvent.ForeColor = System.Drawing.Color.Gainsboro;
            grpCommonEvent.Location = new System.Drawing.Point(4, 3);
            grpCommonEvent.Margin = new Padding(4, 3, 4, 3);
            grpCommonEvent.Name = "grpCommonEvent";
            grpCommonEvent.Padding = new Padding(4, 3, 4, 3);
            grpCommonEvent.Size = new Size(290, 167);
            grpCommonEvent.TabIndex = 17;
            grpCommonEvent.TabStop = false;
            grpCommonEvent.Text = "Start Common Event";
            // 
            // chkOverworldOverride
            // 
            chkOverworldOverride.AutoSize = true;
            chkOverworldOverride.Location = new System.Drawing.Point(30, 86);
            chkOverworldOverride.Margin = new Padding(4, 3, 4, 3);
            chkOverworldOverride.Name = "chkOverworldOverride";
            chkOverworldOverride.Size = new Size(131, 19);
            chkOverworldOverride.TabIndex = 66;
            chkOverworldOverride.Text = "Even on Overworld?";
            chkOverworldOverride.UseVisualStyleBackColor = true;
            // 
            // chkAllInInstance
            // 
            chkAllInInstance.AutoSize = true;
            chkAllInInstance.Location = new System.Drawing.Point(5, 61);
            chkAllInInstance.Margin = new Padding(4, 3, 4, 3);
            chkAllInInstance.Name = "chkAllInInstance";
            chkAllInInstance.Size = new Size(185, 19);
            chkAllInInstance.TabIndex = 65;
            chkAllInInstance.Text = "Run for all players in instance?";
            chkAllInInstance.UseVisualStyleBackColor = true;
            chkAllInInstance.CheckedChanged += chkAllInInstance_CheckedChanged;
            // 
            // cmbEvent
            // 
            cmbEvent.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbEvent.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbEvent.BorderStyle = ButtonBorderStyle.Solid;
            cmbEvent.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbEvent.DrawDropdownHoverOutline = false;
            cmbEvent.DrawFocusRectangle = false;
            cmbEvent.DrawMode = DrawMode.OwnerDrawFixed;
            cmbEvent.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEvent.FlatStyle = FlatStyle.Flat;
            cmbEvent.ForeColor = System.Drawing.Color.Gainsboro;
            cmbEvent.FormattingEnabled = true;
            cmbEvent.Location = new System.Drawing.Point(104, 22);
            cmbEvent.Margin = new Padding(4, 3, 4, 3);
            cmbEvent.Name = "cmbEvent";
            cmbEvent.Size = new Size(179, 24);
            cmbEvent.TabIndex = 22;
            cmbEvent.Text = null;
            cmbEvent.TextPadding = new Padding(2);
            // 
            // lblCommonEvent
            // 
            lblCommonEvent.AutoSize = true;
            lblCommonEvent.Location = new System.Drawing.Point(5, 25);
            lblCommonEvent.Margin = new Padding(4, 0, 4, 0);
            lblCommonEvent.Name = "lblCommonEvent";
            lblCommonEvent.Size = new Size(96, 15);
            lblCommonEvent.TabIndex = 21;
            lblCommonEvent.Text = "Common Event: ";
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(195, 129);
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
            btnSave.Location = new System.Drawing.Point(102, 129);
            btnSave.Margin = new Padding(4, 3, 4, 3);
            btnSave.Name = "btnSave";
            btnSave.Padding = new Padding(6);
            btnSave.Size = new Size(88, 27);
            btnSave.TabIndex = 19;
            btnSave.Text = "Ok";
            btnSave.Click += btnSave_Click;
            // 
            // EventCommandStartCommonEvent
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            Controls.Add(grpCommonEvent);
            Margin = new Padding(4, 3, 4, 3);
            Name = "EventCommandStartCommonEvent";
            Size = new Size(300, 175);
            grpCommonEvent.ResumeLayout(false);
            grpCommonEvent.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private DarkGroupBox grpCommonEvent;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private Label lblCommonEvent;
        private DarkComboBox cmbEvent;
        private CheckBox chkAllInInstance;
        private CheckBox chkOverworldOverride;
    }
}
