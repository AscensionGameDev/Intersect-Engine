namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    partial class EventCommand_ShowPicture
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grpShowPicture = new DarkUI.Controls.DarkGroupBox();
            this.chkClick = new DarkUI.Controls.DarkCheckBox();
            this.lblSize = new System.Windows.Forms.Label();
            this.lblPicture = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.lblHide = new System.Windows.Forms.Label();
            this.cmbSize = new DarkUI.Controls.DarkComboBox();
            this.cmbPicture = new DarkUI.Controls.DarkComboBox();
            this.nudHideTime = new DarkUI.Controls.DarkNumericUpDown();
            this.chkWaitUntilClosed = new DarkUI.Controls.DarkCheckBox();
            this.grpShowPicture.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudHideTime)).BeginInit();
            this.SuspendLayout();
            // 
            // grpShowPicture
            // 
            this.grpShowPicture.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpShowPicture.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpShowPicture.Controls.Add(this.chkWaitUntilClosed);
            this.grpShowPicture.Controls.Add(this.nudHideTime);
            this.grpShowPicture.Controls.Add(this.lblHide);
            this.grpShowPicture.Controls.Add(this.chkClick);
            this.grpShowPicture.Controls.Add(this.cmbSize);
            this.grpShowPicture.Controls.Add(this.lblSize);
            this.grpShowPicture.Controls.Add(this.cmbPicture);
            this.grpShowPicture.Controls.Add(this.lblPicture);
            this.grpShowPicture.Controls.Add(this.btnCancel);
            this.grpShowPicture.Controls.Add(this.btnSave);
            this.grpShowPicture.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpShowPicture.Location = new System.Drawing.Point(3, 3);
            this.grpShowPicture.Name = "grpShowPicture";
            this.grpShowPicture.Size = new System.Drawing.Size(248, 195);
            this.grpShowPicture.TabIndex = 18;
            this.grpShowPicture.TabStop = false;
            this.grpShowPicture.Text = "Show Picture:";
            // 
            // chkClick
            // 
            this.chkClick.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkClick.Location = new System.Drawing.Point(93, 110);
            this.chkClick.Name = "chkClick";
            this.chkClick.Size = new System.Drawing.Size(149, 17);
            this.chkClick.TabIndex = 25;
            this.chkClick.Text = "Click To Close?";
            // 
            // lblSize
            // 
            this.lblSize.AutoSize = true;
            this.lblSize.Location = new System.Drawing.Point(5, 53);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new System.Drawing.Size(30, 13);
            this.lblSize.TabIndex = 23;
            this.lblSize.Text = "Size:";
            // 
            // lblPicture
            // 
            this.lblPicture.AutoSize = true;
            this.lblPicture.Location = new System.Drawing.Point(4, 22);
            this.lblPicture.Name = "lblPicture";
            this.lblPicture.Size = new System.Drawing.Size(43, 13);
            this.lblPicture.TabIndex = 21;
            this.lblPicture.Text = "Picture:";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(167, 165);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(6, 165);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(5);
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Ok";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblHide
            // 
            this.lblHide.AutoSize = true;
            this.lblHide.Location = new System.Drawing.Point(5, 84);
            this.lblHide.Name = "lblHide";
            this.lblHide.Size = new System.Drawing.Size(79, 13);
            this.lblHide.TabIndex = 26;
            this.lblHide.Text = "Hide After (ms):";
            // 
            // cmbSize
            // 
            this.cmbSize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbSize.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbSize.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbSize.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbSize.DrawDropdownHoverOutline = false;
            this.cmbSize.DrawFocusRectangle = false;
            this.cmbSize.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbSize.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbSize.FormattingEnabled = true;
            this.cmbSize.Items.AddRange(new object[] {
            "Original",
            "Full Screen",
            "Half Screen",
            "Stretch To Fit"});
            this.cmbSize.Location = new System.Drawing.Point(93, 53);
            this.cmbSize.Name = "cmbSize";
            this.cmbSize.Size = new System.Drawing.Size(149, 21);
            this.cmbSize.TabIndex = 24;
            this.cmbSize.Text = "Original";
            this.cmbSize.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // cmbPicture
            // 
            this.cmbPicture.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbPicture.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbPicture.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbPicture.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbPicture.DrawDropdownHoverOutline = false;
            this.cmbPicture.DrawFocusRectangle = false;
            this.cmbPicture.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbPicture.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPicture.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbPicture.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbPicture.FormattingEnabled = true;
            this.cmbPicture.Location = new System.Drawing.Point(93, 22);
            this.cmbPicture.Name = "cmbPicture";
            this.cmbPicture.Size = new System.Drawing.Size(149, 21);
            this.cmbPicture.TabIndex = 22;
            this.cmbPicture.Text = null;
            this.cmbPicture.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // nudHideTime
            // 
            this.nudHideTime.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudHideTime.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudHideTime.Location = new System.Drawing.Point(93, 84);
            this.nudHideTime.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.nudHideTime.Name = "nudHideTime";
            this.nudHideTime.Size = new System.Drawing.Size(149, 20);
            this.nudHideTime.TabIndex = 27;
            this.nudHideTime.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // chkWaitUntilClosed
            // 
            this.chkWaitUntilClosed.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkWaitUntilClosed.Location = new System.Drawing.Point(93, 133);
            this.chkWaitUntilClosed.Name = "chkWaitUntilClosed";
            this.chkWaitUntilClosed.Size = new System.Drawing.Size(149, 17);
            this.chkWaitUntilClosed.TabIndex = 28;
            this.chkWaitUntilClosed.Text = "Wait Until Closed?";
            // 
            // EventCommand_ShowPicture
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpShowPicture);
            this.Name = "EventCommand_ShowPicture";
            this.Size = new System.Drawing.Size(254, 201);
            this.grpShowPicture.ResumeLayout(false);
            this.grpShowPicture.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudHideTime)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkUI.Controls.DarkGroupBox grpShowPicture;
        private DarkUI.Controls.DarkComboBox cmbPicture;
        private System.Windows.Forms.Label lblPicture;
        private DarkUI.Controls.DarkButton btnCancel;
        private DarkUI.Controls.DarkButton btnSave;
        private DarkUI.Controls.DarkComboBox cmbSize;
        private System.Windows.Forms.Label lblSize;
        private DarkUI.Controls.DarkCheckBox chkClick;
        private System.Windows.Forms.Label lblHide;
        private DarkUI.Controls.DarkCheckBox chkWaitUntilClosed;
        private DarkUI.Controls.DarkNumericUpDown nudHideTime;
    }
}