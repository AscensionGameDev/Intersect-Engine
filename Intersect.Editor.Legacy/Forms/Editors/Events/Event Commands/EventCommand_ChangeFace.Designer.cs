using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    partial class EventCommandChangeFace
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
            this.grpChangeFace = new DarkUI.Controls.DarkGroupBox();
            this.pnlPreview = new System.Windows.Forms.Panel();
            this.cmbFace = new DarkUI.Controls.DarkComboBox();
            this.lblFace = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.grpChangeFace.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpChangeFace
            // 
            this.grpChangeFace.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpChangeFace.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpChangeFace.Controls.Add(this.pnlPreview);
            this.grpChangeFace.Controls.Add(this.cmbFace);
            this.grpChangeFace.Controls.Add(this.lblFace);
            this.grpChangeFace.Controls.Add(this.btnCancel);
            this.grpChangeFace.Controls.Add(this.btnSave);
            this.grpChangeFace.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpChangeFace.Location = new System.Drawing.Point(3, 3);
            this.grpChangeFace.Name = "grpChangeFace";
            this.grpChangeFace.Size = new System.Drawing.Size(259, 126);
            this.grpChangeFace.TabIndex = 17;
            this.grpChangeFace.TabStop = false;
            this.grpChangeFace.Text = "Change Face:";
            // 
            // pnlPreview
            // 
            this.pnlPreview.Location = new System.Drawing.Point(170, 19);
            this.pnlPreview.Name = "pnlPreview";
            this.pnlPreview.Size = new System.Drawing.Size(83, 101);
            this.pnlPreview.TabIndex = 23;
            // 
            // cmbFace
            // 
            this.cmbFace.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbFace.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbFace.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbFace.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbFace.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFace.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbFace.FormattingEnabled = true;
            this.cmbFace.Location = new System.Drawing.Point(47, 19);
            this.cmbFace.Name = "cmbFace";
            this.cmbFace.Size = new System.Drawing.Size(117, 21);
            this.cmbFace.TabIndex = 22;
            this.cmbFace.SelectedIndexChanged += new System.EventHandler(this.cmbSprite_SelectedIndexChanged);
            // 
            // lblFace
            // 
            this.lblFace.AutoSize = true;
            this.lblFace.Location = new System.Drawing.Point(4, 22);
            this.lblFace.Name = "lblFace";
            this.lblFace.Size = new System.Drawing.Size(34, 13);
            this.lblFace.TabIndex = 21;
            this.lblFace.Text = "Face:";
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
            // EventCommand_ChangeFace
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpChangeFace);
            this.Name = "EventCommandChangeFace";
            this.Size = new System.Drawing.Size(268, 132);
            this.grpChangeFace.ResumeLayout(false);
            this.grpChangeFace.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpChangeFace;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private System.Windows.Forms.Label lblFace;
        private DarkComboBox cmbFace;
        private System.Windows.Forms.Panel pnlPreview;
    }
}
