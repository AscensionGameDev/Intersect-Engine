using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    partial class EventMoveRouteAnimationSelector
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
            this.grpSetAnimation = new DarkUI.Controls.DarkGroupBox();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnOkay = new DarkUI.Controls.DarkButton();
            this.lblAnimation = new System.Windows.Forms.Label();
            this.cmbAnimation = new DarkUI.Controls.DarkComboBox();
            this.grpSetAnimation.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpSetAnimation
            // 
            this.grpSetAnimation.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.grpSetAnimation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpSetAnimation.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpSetAnimation.Controls.Add(this.btnCancel);
            this.grpSetAnimation.Controls.Add(this.btnOkay);
            this.grpSetAnimation.Controls.Add(this.lblAnimation);
            this.grpSetAnimation.Controls.Add(this.cmbAnimation);
            this.grpSetAnimation.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpSetAnimation.Location = new System.Drawing.Point(4, 4);
            this.grpSetAnimation.Name = "grpSetAnimation";
            this.grpSetAnimation.Size = new System.Drawing.Size(197, 96);
            this.grpSetAnimation.TabIndex = 0;
            this.grpSetAnimation.TabStop = false;
            this.grpSetAnimation.Text = "Set Animation";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(116, 55);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOkay
            // 
            this.btnOkay.Location = new System.Drawing.Point(9, 55);
            this.btnOkay.Name = "btnOkay";
            this.btnOkay.Padding = new System.Windows.Forms.Padding(5);
            this.btnOkay.Size = new System.Drawing.Size(75, 23);
            this.btnOkay.TabIndex = 2;
            this.btnOkay.Text = "Ok";
            this.btnOkay.Click += new System.EventHandler(this.btnOkay_Click);
            // 
            // lblAnimation
            // 
            this.lblAnimation.AutoSize = true;
            this.lblAnimation.Location = new System.Drawing.Point(6, 22);
            this.lblAnimation.Name = "lblAnimation";
            this.lblAnimation.Size = new System.Drawing.Size(56, 13);
            this.lblAnimation.TabIndex = 1;
            this.lblAnimation.Text = "Animation:";
            // 
            // cmbAnimation
            // 
            this.cmbAnimation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbAnimation.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbAnimation.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbAnimation.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbAnimation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAnimation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbAnimation.FormattingEnabled = true;
            this.cmbAnimation.Location = new System.Drawing.Point(68, 19);
            this.cmbAnimation.Name = "cmbAnimation";
            this.cmbAnimation.Size = new System.Drawing.Size(123, 21);
            this.cmbAnimation.TabIndex = 0;
            // 
            // Event_MoveRouteAnimationSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpSetAnimation);
            this.Name = "EventMoveRouteAnimationSelector";
            this.Size = new System.Drawing.Size(204, 105);
            this.grpSetAnimation.ResumeLayout(false);
            this.grpSetAnimation.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpSetAnimation;
        private DarkButton btnCancel;
        private DarkButton btnOkay;
        private System.Windows.Forms.Label lblAnimation;
        private DarkComboBox cmbAnimation;
    }
}
