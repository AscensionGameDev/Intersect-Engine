using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    partial class EventCommandStartQuest
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
            this.grpStartQuest = new DarkUI.Controls.DarkGroupBox();
            this.chkShowOfferWindow = new DarkUI.Controls.DarkCheckBox();
            this.cmbQuests = new DarkUI.Controls.DarkComboBox();
            this.lblQuest = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.grpStartQuest.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpStartQuest
            // 
            this.grpStartQuest.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpStartQuest.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpStartQuest.Controls.Add(this.chkShowOfferWindow);
            this.grpStartQuest.Controls.Add(this.cmbQuests);
            this.grpStartQuest.Controls.Add(this.lblQuest);
            this.grpStartQuest.Controls.Add(this.btnCancel);
            this.grpStartQuest.Controls.Add(this.btnSave);
            this.grpStartQuest.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpStartQuest.Location = new System.Drawing.Point(3, 3);
            this.grpStartQuest.Name = "grpStartQuest";
            this.grpStartQuest.Size = new System.Drawing.Size(176, 126);
            this.grpStartQuest.TabIndex = 17;
            this.grpStartQuest.TabStop = false;
            this.grpStartQuest.Text = "Start Quest";
            // 
            // chkShowOfferWindow
            // 
            this.chkShowOfferWindow.AutoSize = true;
            this.chkShowOfferWindow.Location = new System.Drawing.Point(47, 47);
            this.chkShowOfferWindow.Name = "chkShowOfferWindow";
            this.chkShowOfferWindow.Size = new System.Drawing.Size(127, 17);
            this.chkShowOfferWindow.TabIndex = 23;
            this.chkShowOfferWindow.Text = "Show Offer Window?";
            // 
            // cmbQuests
            // 
            this.cmbQuests.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbQuests.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbQuests.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbQuests.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbQuests.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbQuests.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbQuests.FormattingEnabled = true;
            this.cmbQuests.Location = new System.Drawing.Point(47, 19);
            this.cmbQuests.Name = "cmbQuests";
            this.cmbQuests.Size = new System.Drawing.Size(117, 21);
            this.cmbQuests.TabIndex = 22;
            // 
            // lblQuest
            // 
            this.lblQuest.AutoSize = true;
            this.lblQuest.Location = new System.Drawing.Point(4, 22);
            this.lblQuest.Name = "lblQuest";
            this.lblQuest.Size = new System.Drawing.Size(38, 13);
            this.lblQuest.TabIndex = 21;
            this.lblQuest.Text = "Quest:";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(89, 97);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(7, 97);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(5);
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Ok";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // EventCommand_StartQuest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpStartQuest);
            this.Name = "EventCommandStartQuest";
            this.Size = new System.Drawing.Size(182, 132);
            this.grpStartQuest.ResumeLayout(false);
            this.grpStartQuest.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpStartQuest;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private System.Windows.Forms.Label lblQuest;
        private DarkComboBox cmbQuests;
        private DarkCheckBox chkShowOfferWindow;
    }
}
