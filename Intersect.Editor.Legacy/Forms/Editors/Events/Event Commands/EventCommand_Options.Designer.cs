using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    partial class EventCommandOptions
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
            this.grpOptions = new DarkUI.Controls.DarkGroupBox();
            this.lblCommands = new System.Windows.Forms.Label();
            this.pnlFace = new System.Windows.Forms.Panel();
            this.cmbFace = new DarkUI.Controls.DarkComboBox();
            this.lblFace = new System.Windows.Forms.Label();
            this.txtShowOptionsOpt4 = new DarkUI.Controls.DarkTextBox();
            this.txtShowOptionsOpt3 = new DarkUI.Controls.DarkTextBox();
            this.txtShowOptionsOpt2 = new DarkUI.Controls.DarkTextBox();
            this.txtShowOptionsOpt1 = new DarkUI.Controls.DarkTextBox();
            this.lblOpt4 = new System.Windows.Forms.Label();
            this.lblOpt3 = new System.Windows.Forms.Label();
            this.lblOpt2 = new System.Windows.Forms.Label();
            this.lblOpt1 = new System.Windows.Forms.Label();
            this.txtShowOptions = new DarkUI.Controls.DarkTextBox();
            this.lblText = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.grpOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpOptions
            // 
            this.grpOptions.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpOptions.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpOptions.Controls.Add(this.lblCommands);
            this.grpOptions.Controls.Add(this.pnlFace);
            this.grpOptions.Controls.Add(this.cmbFace);
            this.grpOptions.Controls.Add(this.lblFace);
            this.grpOptions.Controls.Add(this.txtShowOptionsOpt4);
            this.grpOptions.Controls.Add(this.txtShowOptionsOpt3);
            this.grpOptions.Controls.Add(this.txtShowOptionsOpt2);
            this.grpOptions.Controls.Add(this.txtShowOptionsOpt1);
            this.grpOptions.Controls.Add(this.lblOpt4);
            this.grpOptions.Controls.Add(this.lblOpt3);
            this.grpOptions.Controls.Add(this.lblOpt2);
            this.grpOptions.Controls.Add(this.lblOpt1);
            this.grpOptions.Controls.Add(this.txtShowOptions);
            this.grpOptions.Controls.Add(this.lblText);
            this.grpOptions.Controls.Add(this.btnCancel);
            this.grpOptions.Controls.Add(this.btnSave);
            this.grpOptions.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpOptions.Location = new System.Drawing.Point(3, 3);
            this.grpOptions.Name = "grpOptions";
            this.grpOptions.Size = new System.Drawing.Size(295, 332);
            this.grpOptions.TabIndex = 17;
            this.grpOptions.TabStop = false;
            this.grpOptions.Text = "Show Options";
            // 
            // lblCommands
            // 
            this.lblCommands.AutoSize = true;
            this.lblCommands.BackColor = System.Drawing.Color.Transparent;
            this.lblCommands.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCommands.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.lblCommands.Location = new System.Drawing.Point(198, 104);
            this.lblCommands.Name = "lblCommands";
            this.lblCommands.Size = new System.Drawing.Size(84, 13);
            this.lblCommands.TabIndex = 34;
            this.lblCommands.Text = "Chat Commands";
            this.lblCommands.Click += new System.EventHandler(this.lblCommands_Click);
            // 
            // pnlFace
            // 
            this.pnlFace.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnlFace.Location = new System.Drawing.Point(182, 201);
            this.pnlFace.Name = "pnlFace";
            this.pnlFace.Size = new System.Drawing.Size(96, 96);
            this.pnlFace.TabIndex = 33;
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
            this.cmbFace.Location = new System.Drawing.Point(47, 201);
            this.cmbFace.Name = "cmbFace";
            this.cmbFace.Size = new System.Drawing.Size(100, 21);
            this.cmbFace.TabIndex = 32;
            this.cmbFace.SelectedIndexChanged += new System.EventHandler(this.cmbFace_SelectedIndexChanged);
            // 
            // lblFace
            // 
            this.lblFace.AutoSize = true;
            this.lblFace.Location = new System.Drawing.Point(10, 185);
            this.lblFace.Name = "lblFace";
            this.lblFace.Size = new System.Drawing.Size(34, 13);
            this.lblFace.TabIndex = 31;
            this.lblFace.Text = "Face:";
            // 
            // txtShowOptionsOpt4
            // 
            this.txtShowOptionsOpt4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtShowOptionsOpt4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtShowOptionsOpt4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtShowOptionsOpt4.Location = new System.Drawing.Point(182, 173);
            this.txtShowOptionsOpt4.Name = "txtShowOptionsOpt4";
            this.txtShowOptionsOpt4.Size = new System.Drawing.Size(100, 20);
            this.txtShowOptionsOpt4.TabIndex = 30;
            // 
            // txtShowOptionsOpt3
            // 
            this.txtShowOptionsOpt3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtShowOptionsOpt3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtShowOptionsOpt3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtShowOptionsOpt3.Location = new System.Drawing.Point(47, 170);
            this.txtShowOptionsOpt3.Name = "txtShowOptionsOpt3";
            this.txtShowOptionsOpt3.Size = new System.Drawing.Size(100, 20);
            this.txtShowOptionsOpt3.TabIndex = 29;
            // 
            // txtShowOptionsOpt2
            // 
            this.txtShowOptionsOpt2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtShowOptionsOpt2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtShowOptionsOpt2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtShowOptionsOpt2.Location = new System.Drawing.Point(183, 134);
            this.txtShowOptionsOpt2.Name = "txtShowOptionsOpt2";
            this.txtShowOptionsOpt2.Size = new System.Drawing.Size(100, 20);
            this.txtShowOptionsOpt2.TabIndex = 28;
            // 
            // txtShowOptionsOpt1
            // 
            this.txtShowOptionsOpt1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtShowOptionsOpt1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtShowOptionsOpt1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtShowOptionsOpt1.Location = new System.Drawing.Point(47, 131);
            this.txtShowOptionsOpt1.Name = "txtShowOptionsOpt1";
            this.txtShowOptionsOpt1.Size = new System.Drawing.Size(100, 20);
            this.txtShowOptionsOpt1.TabIndex = 27;
            // 
            // lblOpt4
            // 
            this.lblOpt4.AutoSize = true;
            this.lblOpt4.Location = new System.Drawing.Point(153, 157);
            this.lblOpt4.Name = "lblOpt4";
            this.lblOpt4.Size = new System.Drawing.Size(50, 13);
            this.lblOpt4.TabIndex = 26;
            this.lblOpt4.Text = "Option 4:";
            // 
            // lblOpt3
            // 
            this.lblOpt3.AutoSize = true;
            this.lblOpt3.Location = new System.Drawing.Point(10, 154);
            this.lblOpt3.Name = "lblOpt3";
            this.lblOpt3.Size = new System.Drawing.Size(50, 13);
            this.lblOpt3.TabIndex = 25;
            this.lblOpt3.Text = "Option 3:";
            // 
            // lblOpt2
            // 
            this.lblOpt2.AutoSize = true;
            this.lblOpt2.Location = new System.Drawing.Point(153, 118);
            this.lblOpt2.Name = "lblOpt2";
            this.lblOpt2.Size = new System.Drawing.Size(50, 13);
            this.lblOpt2.TabIndex = 24;
            this.lblOpt2.Text = "Option 2:";
            // 
            // lblOpt1
            // 
            this.lblOpt1.AutoSize = true;
            this.lblOpt1.Location = new System.Drawing.Point(10, 115);
            this.lblOpt1.Name = "lblOpt1";
            this.lblOpt1.Size = new System.Drawing.Size(50, 13);
            this.lblOpt1.TabIndex = 23;
            this.lblOpt1.Text = "Option 1:";
            // 
            // txtShowOptions
            // 
            this.txtShowOptions.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtShowOptions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtShowOptions.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtShowOptions.Location = new System.Drawing.Point(48, 19);
            this.txtShowOptions.Multiline = true;
            this.txtShowOptions.Name = "txtShowOptions";
            this.txtShowOptions.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.txtShowOptions.Size = new System.Drawing.Size(234, 82);
            this.txtShowOptions.TabIndex = 22;
            // 
            // lblText
            // 
            this.lblText.AutoSize = true;
            this.lblText.Location = new System.Drawing.Point(10, 22);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(31, 13);
            this.lblText.TabIndex = 21;
            this.lblText.Text = "Text:";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(87, 303);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(6, 303);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(5);
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Ok";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // EventCommand_Options
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpOptions);
            this.Name = "EventCommandOptions";
            this.Size = new System.Drawing.Size(301, 340);
            this.grpOptions.ResumeLayout(false);
            this.grpOptions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpOptions;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private DarkTextBox txtShowOptionsOpt4;
        private DarkTextBox txtShowOptionsOpt3;
        private DarkTextBox txtShowOptionsOpt2;
        private DarkTextBox txtShowOptionsOpt1;
        private System.Windows.Forms.Label lblOpt4;
        private System.Windows.Forms.Label lblOpt3;
        private System.Windows.Forms.Label lblOpt2;
        private System.Windows.Forms.Label lblOpt1;
        private DarkTextBox txtShowOptions;
        private System.Windows.Forms.Label lblText;
        private System.Windows.Forms.Panel pnlFace;
        private DarkComboBox cmbFace;
        private System.Windows.Forms.Label lblFace;
        private System.Windows.Forms.Label lblCommands;
    }
}
