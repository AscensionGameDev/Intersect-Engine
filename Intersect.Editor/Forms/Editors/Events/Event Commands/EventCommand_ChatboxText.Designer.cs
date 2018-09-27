using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    partial class EventCommandChatboxText
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
            this.grpChatboxText = new DarkUI.Controls.DarkGroupBox();
            this.lblCommands = new System.Windows.Forms.Label();
            this.cmbChannel = new DarkUI.Controls.DarkComboBox();
            this.lblChannel = new System.Windows.Forms.Label();
            this.cmbColor = new DarkUI.Controls.DarkComboBox();
            this.lblColor = new System.Windows.Forms.Label();
            this.txtAddText = new DarkUI.Controls.DarkTextBox();
            this.lblText = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.grpChatboxText.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpChatboxText
            // 
            this.grpChatboxText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpChatboxText.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpChatboxText.Controls.Add(this.lblCommands);
            this.grpChatboxText.Controls.Add(this.cmbChannel);
            this.grpChatboxText.Controls.Add(this.lblChannel);
            this.grpChatboxText.Controls.Add(this.cmbColor);
            this.grpChatboxText.Controls.Add(this.lblColor);
            this.grpChatboxText.Controls.Add(this.txtAddText);
            this.grpChatboxText.Controls.Add(this.lblText);
            this.grpChatboxText.Controls.Add(this.btnCancel);
            this.grpChatboxText.Controls.Add(this.btnSave);
            this.grpChatboxText.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpChatboxText.Location = new System.Drawing.Point(3, 3);
            this.grpChatboxText.Name = "grpChatboxText";
            this.grpChatboxText.Size = new System.Drawing.Size(259, 281);
            this.grpChatboxText.TabIndex = 17;
            this.grpChatboxText.TabStop = false;
            this.grpChatboxText.Text = "Add Chatbox Text";
            // 
            // lblCommands
            // 
            this.lblCommands.AutoSize = true;
            this.lblCommands.BackColor = System.Drawing.Color.Transparent;
            this.lblCommands.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCommands.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.lblCommands.Location = new System.Drawing.Point(157, 22);
            this.lblCommands.Name = "lblCommands";
            this.lblCommands.Size = new System.Drawing.Size(84, 13);
            this.lblCommands.TabIndex = 35;
            this.lblCommands.Text = "Chat Commands";
            this.lblCommands.Click += new System.EventHandler(this.lblCommands_Click);
            // 
            // cmbChannel
            // 
            this.cmbChannel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbChannel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbChannel.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbChannel.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbChannel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbChannel.FormattingEnabled = true;
            this.cmbChannel.Items.AddRange(new object[] {
            "Player",
            "Local",
            "Global"});
            this.cmbChannel.Location = new System.Drawing.Point(7, 206);
            this.cmbChannel.Name = "cmbChannel";
            this.cmbChannel.Size = new System.Drawing.Size(232, 21);
            this.cmbChannel.TabIndex = 26;
            // 
            // lblChannel
            // 
            this.lblChannel.AutoSize = true;
            this.lblChannel.Location = new System.Drawing.Point(6, 190);
            this.lblChannel.Name = "lblChannel";
            this.lblChannel.Size = new System.Drawing.Size(49, 13);
            this.lblChannel.TabIndex = 25;
            this.lblChannel.Text = "Channel:";
            // 
            // cmbColor
            // 
            this.cmbColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbColor.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbColor.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbColor.FormattingEnabled = true;
            this.cmbColor.Location = new System.Drawing.Point(9, 157);
            this.cmbColor.Name = "cmbColor";
            this.cmbColor.Size = new System.Drawing.Size(232, 21);
            this.cmbColor.TabIndex = 24;
            // 
            // lblColor
            // 
            this.lblColor.AutoSize = true;
            this.lblColor.Location = new System.Drawing.Point(6, 141);
            this.lblColor.Name = "lblColor";
            this.lblColor.Size = new System.Drawing.Size(34, 13);
            this.lblColor.TabIndex = 23;
            this.lblColor.Text = "Color:";
            // 
            // txtAddText
            // 
            this.txtAddText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtAddText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAddText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtAddText.Location = new System.Drawing.Point(7, 38);
            this.txtAddText.Multiline = true;
            this.txtAddText.Name = "txtAddText";
            this.txtAddText.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.txtAddText.Size = new System.Drawing.Size(234, 100);
            this.txtAddText.TabIndex = 22;
            // 
            // lblText
            // 
            this.lblText.AutoSize = true;
            this.lblText.Location = new System.Drawing.Point(4, 22);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(31, 13);
            this.lblText.TabIndex = 21;
            this.lblText.Text = "Text:";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(89, 252);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(7, 252);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(5);
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Ok";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // EventCommand_ChatboxText
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpChatboxText);
            this.Name = "EventCommandChatboxText";
            this.Size = new System.Drawing.Size(268, 287);
            this.grpChatboxText.ResumeLayout(false);
            this.grpChatboxText.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpChatboxText;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private DarkTextBox txtAddText;
        private System.Windows.Forms.Label lblText;
        private DarkComboBox cmbColor;
        private System.Windows.Forms.Label lblColor;
        private DarkComboBox cmbChannel;
        private System.Windows.Forms.Label lblChannel;
        private System.Windows.Forms.Label lblCommands;
    }
}
