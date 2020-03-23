using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    partial class EventCommandChangeNameColor
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
            this.grpChangeLevel = new DarkUI.Controls.DarkGroupBox();
            this.chkRemove = new System.Windows.Forms.CheckBox();
            this.chkOverride = new System.Windows.Forms.CheckBox();
            this.pnlColor = new System.Windows.Forms.Panel();
            this.btnSelectLightColor = new DarkUI.Controls.DarkButton();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.grpChangeLevel.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpChangeLevel
            // 
            this.grpChangeLevel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpChangeLevel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpChangeLevel.Controls.Add(this.chkRemove);
            this.grpChangeLevel.Controls.Add(this.chkOverride);
            this.grpChangeLevel.Controls.Add(this.pnlColor);
            this.grpChangeLevel.Controls.Add(this.btnSelectLightColor);
            this.grpChangeLevel.Controls.Add(this.btnCancel);
            this.grpChangeLevel.Controls.Add(this.btnSave);
            this.grpChangeLevel.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpChangeLevel.Location = new System.Drawing.Point(3, 3);
            this.grpChangeLevel.Name = "grpChangeLevel";
            this.grpChangeLevel.Size = new System.Drawing.Size(188, 133);
            this.grpChangeLevel.TabIndex = 17;
            this.grpChangeLevel.TabStop = false;
            this.grpChangeLevel.Text = "Change Name Color:";
            // 
            // chkRemove
            // 
            this.chkRemove.AutoSize = true;
            this.chkRemove.Location = new System.Drawing.Point(15, 77);
            this.chkRemove.Name = "chkRemove";
            this.chkRemove.Size = new System.Drawing.Size(130, 17);
            this.chkRemove.TabIndex = 43;
            this.chkRemove.Text = "Remove Name Color?";
            this.chkRemove.UseVisualStyleBackColor = true;
            // 
            // chkOverride
            // 
            this.chkOverride.AutoSize = true;
            this.chkOverride.Location = new System.Drawing.Point(15, 54);
            this.chkOverride.Name = "chkOverride";
            this.chkOverride.Size = new System.Drawing.Size(162, 17);
            this.chkOverride.TabIndex = 42;
            this.chkOverride.Text = "Override Admin Name Color?";
            this.chkOverride.UseVisualStyleBackColor = true;
            // 
            // pnlColor
            // 
            this.pnlColor.BackColor = System.Drawing.Color.White;
            this.pnlColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlColor.Location = new System.Drawing.Point(15, 19);
            this.pnlColor.Name = "pnlColor";
            this.pnlColor.Size = new System.Drawing.Size(31, 29);
            this.pnlColor.TabIndex = 41;
            // 
            // btnSelectLightColor
            // 
            this.btnSelectLightColor.Location = new System.Drawing.Point(62, 25);
            this.btnSelectLightColor.Name = "btnSelectLightColor";
            this.btnSelectLightColor.Padding = new System.Windows.Forms.Padding(5);
            this.btnSelectLightColor.Size = new System.Drawing.Size(90, 23);
            this.btnSelectLightColor.TabIndex = 39;
            this.btnSelectLightColor.Text = "Select Color";
            this.btnSelectLightColor.Click += new System.EventHandler(this.btnSelectLightColor_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(102, 100);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(15, 100);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(5);
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Ok";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // EventCommandChangeNameColor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpChangeLevel);
            this.Name = "EventCommandChangeNameColor";
            this.Size = new System.Drawing.Size(197, 144);
            this.grpChangeLevel.ResumeLayout(false);
            this.grpChangeLevel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpChangeLevel;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private System.Windows.Forms.CheckBox chkOverride;
        public System.Windows.Forms.Panel pnlColor;
        private DarkButton btnSelectLightColor;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.CheckBox chkRemove;
    }
}
