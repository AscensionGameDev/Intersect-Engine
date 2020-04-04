using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    partial class EventCommandChangeHair
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EventCommandChangeHair));
            this.grpChangeHair = new DarkUI.Controls.DarkGroupBox();
            this.pnlPreview = new System.Windows.Forms.Panel();
            this.cmbHair = new DarkUI.Controls.DarkComboBox();
            this.lblHair = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.grpChangeHair.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpChangeHair
            // 
            this.grpChangeHair.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpChangeHair.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpChangeHair.Controls.Add(this.pnlPreview);
            this.grpChangeHair.Controls.Add(this.cmbHair);
            this.grpChangeHair.Controls.Add(this.lblHair);
            this.grpChangeHair.Controls.Add(this.btnCancel);
            this.grpChangeHair.Controls.Add(this.btnSave);
            this.grpChangeHair.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpChangeHair.Location = new System.Drawing.Point(3, 3);
            this.grpChangeHair.Name = "grpChangeHair";
            this.grpChangeHair.Size = new System.Drawing.Size(259, 126);
            this.grpChangeHair.TabIndex = 17;
            this.grpChangeHair.TabStop = false;
            this.grpChangeHair.Text = "Change Hair:";
            // 
            // pnlPreview
            // 
            this.pnlPreview.Location = new System.Drawing.Point(170, 19);
            this.pnlPreview.Name = "pnlPreview";
            this.pnlPreview.Size = new System.Drawing.Size(83, 101);
            this.pnlPreview.TabIndex = 23;
            // 
            // cmbHair
            // 
            this.cmbHair.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbHair.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbHair.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbHair.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbHair.ButtonIcon = ((System.Drawing.Bitmap)(resources.GetObject("cmbHair.ButtonIcon")));
            this.cmbHair.DrawDropdownHoverOutline = false;
            this.cmbHair.DrawFocusRectangle = false;
            this.cmbHair.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbHair.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbHair.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbHair.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbHair.FormattingEnabled = true;
            this.cmbHair.Location = new System.Drawing.Point(47, 19);
            this.cmbHair.Name = "cmbHair";
            this.cmbHair.Size = new System.Drawing.Size(117, 21);
            this.cmbHair.TabIndex = 22;
            this.cmbHair.Text = null;
            this.cmbHair.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbHair.SelectedIndexChanged += new System.EventHandler(this.cmbSprite_SelectedIndexChanged);
            // 
            // lblHair
            // 
            this.lblHair.AutoSize = true;
            this.lblHair.Location = new System.Drawing.Point(4, 22);
            this.lblHair.Name = "lblHair";
            this.lblHair.Size = new System.Drawing.Size(29, 13);
            this.lblHair.TabIndex = 21;
            this.lblHair.Text = "Hair:";
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
            // EventCommandChangeHair
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpChangeHair);
            this.Name = "EventCommandChangeHair";
            this.Size = new System.Drawing.Size(268, 132);
            this.grpChangeHair.ResumeLayout(false);
            this.grpChangeHair.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpChangeHair;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private System.Windows.Forms.Label lblHair;
        private DarkComboBox cmbHair;
        private System.Windows.Forms.Panel pnlPreview;
    }
}
