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
            grpChatboxText = new DarkGroupBox();
            chkShowChatBubble = new DarkCheckBox();
            chkShowChatBubbleInProximity = new DarkCheckBox();
            lblCommands = new Label();
            cmbChannel = new DarkComboBox();
            lblChannel = new Label();
            cmbColor = new DarkComboBox();
            lblColor = new Label();
            txtAddText = new DarkTextBox();
            lblText = new Label();
            btnCancel = new DarkButton();
            btnSave = new DarkButton();
            grpChatboxText.SuspendLayout();
            SuspendLayout();
            // 
            // grpChatboxText
            // 
            grpChatboxText.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            grpChatboxText.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpChatboxText.Controls.Add(chkShowChatBubble);
            grpChatboxText.Controls.Add(chkShowChatBubbleInProximity);
            grpChatboxText.Controls.Add(lblCommands);
            grpChatboxText.Controls.Add(cmbChannel);
            grpChatboxText.Controls.Add(lblChannel);
            grpChatboxText.Controls.Add(cmbColor);
            grpChatboxText.Controls.Add(lblColor);
            grpChatboxText.Controls.Add(txtAddText);
            grpChatboxText.Controls.Add(lblText);
            grpChatboxText.Controls.Add(btnCancel);
            grpChatboxText.Controls.Add(btnSave);
            grpChatboxText.ForeColor = System.Drawing.Color.Gainsboro;
            grpChatboxText.Location = new System.Drawing.Point(4, 3);
            grpChatboxText.Margin = new Padding(4, 3, 4, 3);
            grpChatboxText.Name = "grpChatboxText";
            grpChatboxText.Padding = new Padding(4, 3, 4, 3);
            grpChatboxText.Size = new Size(302, 368);
            grpChatboxText.TabIndex = 17;
            grpChatboxText.TabStop = false;
            grpChatboxText.Text = "Add Chatbox Text";
            // 
            // chkShowChatBubble
            // 
            chkShowChatBubble.Location = new System.Drawing.Point(8, 272);
            chkShowChatBubble.Margin = new Padding(4, 3, 4, 3);
            chkShowChatBubble.Name = "chkShowChatBubble";
            chkShowChatBubble.Size = new Size(130, 20);
            chkShowChatBubble.TabIndex = 57;
            chkShowChatBubble.Text = "Show Chat Bubble";
            // 
            // chkShowChatBubbleInProximity
            // 
            chkShowChatBubbleInProximity.Location = new System.Drawing.Point(8, 298);
            chkShowChatBubbleInProximity.Margin = new Padding(4, 3, 4, 3);
            chkShowChatBubbleInProximity.Name = "chkShowChatBubbleInProximity";
            chkShowChatBubbleInProximity.Size = new Size(184, 20);
            chkShowChatBubbleInProximity.TabIndex = 58;
            chkShowChatBubbleInProximity.Text = "Show Chat Bubble in Proximity";
            // 
            // lblCommands
            // 
            lblCommands.AutoSize = true;
            lblCommands.BackColor = System.Drawing.Color.Transparent;
            lblCommands.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Underline, GraphicsUnit.Point, 0);
            lblCommands.ForeColor = SystemColors.MenuHighlight;
            lblCommands.Location = new System.Drawing.Point(183, 25);
            lblCommands.Margin = new Padding(4, 0, 4, 0);
            lblCommands.Name = "lblCommands";
            lblCommands.Size = new Size(84, 13);
            lblCommands.TabIndex = 35;
            lblCommands.Text = "Chat Commands";
            lblCommands.Click += lblCommands_Click;
            // 
            // cmbChannel
            // 
            cmbChannel.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbChannel.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbChannel.BorderStyle = ButtonBorderStyle.Solid;
            cmbChannel.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbChannel.DrawDropdownHoverOutline = false;
            cmbChannel.DrawFocusRectangle = false;
            cmbChannel.DrawMode = DrawMode.OwnerDrawFixed;
            cmbChannel.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbChannel.FlatStyle = FlatStyle.Flat;
            cmbChannel.ForeColor = System.Drawing.Color.Gainsboro;
            cmbChannel.FormattingEnabled = true;
            cmbChannel.Items.AddRange(new object[] { "Player", "Local", "Global" });
            cmbChannel.Location = new System.Drawing.Point(8, 238);
            cmbChannel.Margin = new Padding(4, 3, 4, 3);
            cmbChannel.Name = "cmbChannel";
            cmbChannel.Size = new Size(286, 24);
            cmbChannel.TabIndex = 26;
            cmbChannel.Text = "Player";
            cmbChannel.TextPadding = new Padding(2);
            // 
            // lblChannel
            // 
            lblChannel.AutoSize = true;
            lblChannel.Location = new System.Drawing.Point(7, 219);
            lblChannel.Margin = new Padding(4, 0, 4, 0);
            lblChannel.Name = "lblChannel";
            lblChannel.Size = new Size(54, 15);
            lblChannel.TabIndex = 25;
            lblChannel.Text = "Channel:";
            // 
            // cmbColor
            // 
            cmbColor.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbColor.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbColor.BorderStyle = ButtonBorderStyle.Solid;
            cmbColor.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbColor.DrawDropdownHoverOutline = false;
            cmbColor.DrawFocusRectangle = false;
            cmbColor.DrawMode = DrawMode.OwnerDrawFixed;
            cmbColor.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbColor.FlatStyle = FlatStyle.Flat;
            cmbColor.ForeColor = System.Drawing.Color.Gainsboro;
            cmbColor.FormattingEnabled = true;
            cmbColor.Location = new System.Drawing.Point(8, 187);
            cmbColor.Margin = new Padding(4, 3, 4, 3);
            cmbColor.Name = "cmbColor";
            cmbColor.Size = new Size(286, 24);
            cmbColor.TabIndex = 24;
            cmbColor.Text = null;
            cmbColor.TextPadding = new Padding(2);
            // 
            // lblColor
            // 
            lblColor.AutoSize = true;
            lblColor.Location = new System.Drawing.Point(7, 168);
            lblColor.Margin = new Padding(4, 0, 4, 0);
            lblColor.Name = "lblColor";
            lblColor.Size = new Size(39, 15);
            lblColor.TabIndex = 23;
            lblColor.Text = "Color:";
            // 
            // txtAddText
            // 
            txtAddText.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            txtAddText.BorderStyle = BorderStyle.FixedSingle;
            txtAddText.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            txtAddText.Location = new System.Drawing.Point(8, 44);
            txtAddText.Margin = new Padding(4, 3, 4, 3);
            txtAddText.Multiline = true;
            txtAddText.Name = "txtAddText";
            txtAddText.ScrollBars = ScrollBars.Horizontal;
            txtAddText.Size = new Size(287, 115);
            txtAddText.TabIndex = 22;
            // 
            // lblText
            // 
            lblText.AutoSize = true;
            lblText.Location = new System.Drawing.Point(5, 25);
            lblText.Margin = new Padding(4, 0, 4, 0);
            lblText.Name = "lblText";
            lblText.Size = new Size(31, 15);
            lblText.TabIndex = 21;
            lblText.Text = "Text:";
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(104, 332);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Padding = new Padding(6, 6, 6, 6);
            btnCancel.Size = new Size(88, 27);
            btnCancel.TabIndex = 20;
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // btnSave
            // 
            btnSave.Location = new System.Drawing.Point(8, 332);
            btnSave.Margin = new Padding(4, 3, 4, 3);
            btnSave.Name = "btnSave";
            btnSave.Padding = new Padding(6, 6, 6, 6);
            btnSave.Size = new Size(88, 27);
            btnSave.TabIndex = 19;
            btnSave.Text = "Ok";
            btnSave.Click += btnSave_Click;
            // 
            // EventCommandChatboxText
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            Controls.Add(grpChatboxText);
            Margin = new Padding(4, 3, 4, 3);
            Name = "EventCommandChatboxText";
            Size = new Size(313, 374);
            grpChatboxText.ResumeLayout(false);
            grpChatboxText.PerformLayout();
            ResumeLayout(false);
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
        private DarkCheckBox chkShowChatBubble;
        private DarkCheckBox chkShowChatBubbleInProximity;
    }
}
