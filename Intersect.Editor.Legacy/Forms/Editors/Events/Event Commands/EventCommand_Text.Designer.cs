using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    partial class EventCommandText
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
            this.grpShowText = new DarkUI.Controls.DarkGroupBox();
            this.lblCommands = new System.Windows.Forms.Label();
            this.pnlFace = new System.Windows.Forms.Panel();
            this.cmbFace = new DarkUI.Controls.DarkComboBox();
            this.lblFace = new System.Windows.Forms.Label();
            this.txtShowText = new DarkUI.Controls.DarkTextBox();
            this.lblText = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.grpShowText.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpShowText
            // 
            this.grpShowText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpShowText.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpShowText.Controls.Add(this.lblCommands);
            this.grpShowText.Controls.Add(this.pnlFace);
            this.grpShowText.Controls.Add(this.cmbFace);
            this.grpShowText.Controls.Add(this.lblFace);
            this.grpShowText.Controls.Add(this.txtShowText);
            this.grpShowText.Controls.Add(this.lblText);
            this.grpShowText.Controls.Add(this.btnCancel);
            this.grpShowText.Controls.Add(this.btnSave);
            this.grpShowText.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpShowText.Location = new System.Drawing.Point(3, 3);
            this.grpShowText.Name = "grpShowText";
            this.grpShowText.Size = new System.Drawing.Size(259, 281);
            this.grpShowText.TabIndex = 17;
            this.grpShowText.TabStop = false;
            this.grpShowText.Text = "Show Text";
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
            this.lblCommands.TabIndex = 26;
            this.lblCommands.Text = "Chat Commands";
            this.lblCommands.Click += new System.EventHandler(this.lblCommands_Click);
            // 
            // pnlFace
            // 
            this.pnlFace.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnlFace.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlFace.Location = new System.Drawing.Point(145, 144);
            this.pnlFace.Name = "pnlFace";
            this.pnlFace.Size = new System.Drawing.Size(96, 96);
            this.pnlFace.TabIndex = 25;
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
            this.cmbFace.Location = new System.Drawing.Point(9, 157);
            this.cmbFace.Name = "cmbFace";
            this.cmbFace.Size = new System.Drawing.Size(121, 21);
            this.cmbFace.TabIndex = 24;
            this.cmbFace.SelectedIndexChanged += new System.EventHandler(this.cmbFace_SelectedIndexChanged);
            // 
            // lblFace
            // 
            this.lblFace.AutoSize = true;
            this.lblFace.Location = new System.Drawing.Point(6, 141);
            this.lblFace.Name = "lblFace";
            this.lblFace.Size = new System.Drawing.Size(34, 13);
            this.lblFace.TabIndex = 23;
            this.lblFace.Text = "Face:";
            // 
            // txtShowText
            // 
            this.txtShowText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtShowText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtShowText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtShowText.Location = new System.Drawing.Point(7, 38);
            this.txtShowText.Multiline = true;
            this.txtShowText.Name = "txtShowText";
            this.txtShowText.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.txtShowText.Size = new System.Drawing.Size(234, 100);
            this.txtShowText.TabIndex = 22;
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
            // EventCommand_Text
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpShowText);
            this.Name = "EventCommandText";
            this.Size = new System.Drawing.Size(268, 287);
            this.grpShowText.ResumeLayout(false);
            this.grpShowText.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpShowText;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private DarkTextBox txtShowText;
        private System.Windows.Forms.Label lblText;
        private System.Windows.Forms.Panel pnlFace;
        private DarkComboBox cmbFace;
        private System.Windows.Forms.Label lblFace;
        private System.Windows.Forms.Label lblCommands;
    }
}
