using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    partial class EventCommandChangeTag
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EventCommandChangeTag));
            this.grpChangeTag = new DarkUI.Controls.DarkGroupBox();
            this.pnlPreview = new System.Windows.Forms.Panel();
            this.cmbTag = new DarkUI.Controls.DarkComboBox();
            this.lblTag = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.grpChangeTag.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpChangeTag
            // 
            this.grpChangeTag.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpChangeTag.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpChangeTag.Controls.Add(this.pnlPreview);
            this.grpChangeTag.Controls.Add(this.cmbTag);
            this.grpChangeTag.Controls.Add(this.lblTag);
            this.grpChangeTag.Controls.Add(this.btnCancel);
            this.grpChangeTag.Controls.Add(this.btnSave);
            this.grpChangeTag.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpChangeTag.Location = new System.Drawing.Point(3, 3);
            this.grpChangeTag.Name = "grpChangeTag";
            this.grpChangeTag.Size = new System.Drawing.Size(259, 104);
            this.grpChangeTag.TabIndex = 17;
            this.grpChangeTag.TabStop = false;
            this.grpChangeTag.Text = "Change Tag:";
            // 
            // pnlPreview
            // 
            this.pnlPreview.Location = new System.Drawing.Point(180, 19);
            this.pnlPreview.Name = "pnlPreview";
            this.pnlPreview.Size = new System.Drawing.Size(64, 32);
            this.pnlPreview.TabIndex = 23;
            // 
            // cmbTag
            // 
            this.cmbTag.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbTag.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbTag.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbTag.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbTag.ButtonIcon = ((System.Drawing.Bitmap)(resources.GetObject("cmbTag.ButtonIcon")));
            this.cmbTag.DrawDropdownHoverOutline = false;
            this.cmbTag.DrawFocusRectangle = false;
            this.cmbTag.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbTag.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTag.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbTag.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbTag.FormattingEnabled = true;
            this.cmbTag.Location = new System.Drawing.Point(47, 19);
            this.cmbTag.Name = "cmbTag";
            this.cmbTag.Size = new System.Drawing.Size(117, 21);
            this.cmbTag.TabIndex = 22;
            this.cmbTag.Text = null;
            this.cmbTag.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbTag.SelectedIndexChanged += new System.EventHandler(this.cmbTag_SelectedIndexChanged);
            // 
            // lblTag
            // 
            this.lblTag.AutoSize = true;
            this.lblTag.Location = new System.Drawing.Point(4, 22);
            this.lblTag.Name = "lblTag";
            this.lblTag.Size = new System.Drawing.Size(29, 13);
            this.lblTag.TabIndex = 21;
            this.lblTag.Text = "Tag:";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(89, 70);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(7, 70);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(5);
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Ok";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // EventCommandChangeTag
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpChangeTag);
            this.Name = "EventCommandChangeTag";
            this.Size = new System.Drawing.Size(268, 110);
            this.grpChangeTag.ResumeLayout(false);
            this.grpChangeTag.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpChangeTag;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private System.Windows.Forms.Label lblTag;
        private DarkComboBox cmbTag;
        private System.Windows.Forms.Panel pnlPreview;
    }
}
