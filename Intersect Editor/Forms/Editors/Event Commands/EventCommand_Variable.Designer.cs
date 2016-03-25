namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    partial class EventCommand_Variable
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdoGlobalVariable = new System.Windows.Forms.RadioButton();
            this.rdoPlayerVariable = new System.Windows.Forms.RadioButton();
            this.lblRandomLabel37 = new System.Windows.Forms.Label();
            this.lblRandomLabel13 = new System.Windows.Forms.Label();
            this.txtRandomHigh = new System.Windows.Forms.TextBox();
            this.txtRandomLow = new System.Windows.Forms.TextBox();
            this.optRandom = new System.Windows.Forms.RadioButton();
            this.txtSubtract = new System.Windows.Forms.TextBox();
            this.optSubtract = new System.Windows.Forms.RadioButton();
            this.txtAdd = new System.Windows.Forms.TextBox();
            this.optAdd = new System.Windows.Forms.RadioButton();
            this.txtSet = new System.Windows.Forms.TextBox();
            this.optSet = new System.Windows.Forms.RadioButton();
            this.cmbVariable = new System.Windows.Forms.ComboBox();
            this.lblRandomLabel = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdoGlobalVariable);
            this.groupBox1.Controls.Add(this.rdoPlayerVariable);
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(189, 253);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Set Variable";
            // 
            // rdoGlobalVariable
            // 
            this.rdoGlobalVariable.AutoSize = true;
            this.rdoGlobalVariable.Location = new System.Drawing.Point(98, 19);
            this.rdoGlobalVariable.Name = "rdoGlobalVariable";
            this.rdoGlobalVariable.Size = new System.Drawing.Size(96, 17);
            this.rdoGlobalVariable.TabIndex = 35;
            this.rdoGlobalVariable.Text = "Global Variable";
            this.rdoGlobalVariable.UseVisualStyleBackColor = true;
            this.rdoGlobalVariable.CheckedChanged += new System.EventHandler(this.rdoGlobalVariable_CheckedChanged);
            // 
            // rdoPlayerVariable
            // 
            this.rdoPlayerVariable.AutoSize = true;
            this.rdoPlayerVariable.Checked = true;
            this.rdoPlayerVariable.Location = new System.Drawing.Point(3, 19);
            this.rdoPlayerVariable.Name = "rdoPlayerVariable";
            this.rdoPlayerVariable.Size = new System.Drawing.Size(95, 17);
            this.rdoPlayerVariable.TabIndex = 34;
            this.rdoPlayerVariable.TabStop = true;
            this.rdoPlayerVariable.Text = "Player Variable";
            this.rdoPlayerVariable.UseVisualStyleBackColor = true;
            this.rdoPlayerVariable.CheckedChanged += new System.EventHandler(this.rdoPlayerVariable_CheckedChanged);
            // 
            // lblRandomLabel37
            // 
            this.lblRandomLabel37.AutoSize = true;
            this.lblRandomLabel37.Location = new System.Drawing.Point(78, 155);
            this.lblRandomLabel37.Name = "lblRandomLabel37";
            this.lblRandomLabel37.Size = new System.Drawing.Size(29, 13);
            this.lblRandomLabel37.TabIndex = 32;
            this.lblRandomLabel37.Text = "High";
            // 
            // lblRandomLabel13
            // 
            this.lblRandomLabel13.AutoSize = true;
            this.lblRandomLabel13.Location = new System.Drawing.Point(80, 126);
            this.lblRandomLabel13.Name = "lblRandomLabel13";
            this.lblRandomLabel13.Size = new System.Drawing.Size(27, 13);
            this.lblRandomLabel13.TabIndex = 33;
            this.lblRandomLabel13.Text = "Low";
            // 
            // txtRandomHigh
            // 
            this.txtRandomHigh.Enabled = false;
            this.txtRandomHigh.Location = new System.Drawing.Point(109, 148);
            this.txtRandomHigh.Name = "txtRandomHigh";
            this.txtRandomHigh.Size = new System.Drawing.Size(63, 20);
            this.txtRandomHigh.TabIndex = 27;
            this.txtRandomHigh.Text = "0";
            // 
            // txtRandomLow
            // 
            this.txtRandomLow.Enabled = false;
            this.txtRandomLow.Location = new System.Drawing.Point(109, 121);
            this.txtRandomLow.Name = "txtRandomLow";
            this.txtRandomLow.Size = new System.Drawing.Size(63, 20);
            this.txtRandomLow.TabIndex = 28;
            this.txtRandomLow.Text = "0";
            // 
            // optRandom
            // 
            this.optRandom.AutoSize = true;
            this.optRandom.Location = new System.Drawing.Point(9, 126);
            this.optRandom.Name = "optRandom";
            this.optRandom.Size = new System.Drawing.Size(65, 17);
            this.optRandom.TabIndex = 23;
            this.optRandom.Text = "Random";
            this.optRandom.UseVisualStyleBackColor = true;
            this.optRandom.CheckedChanged += new System.EventHandler(this.optRandom_CheckedChanged);
            // 
            // txtSubtract
            // 
            this.txtSubtract.Enabled = false;
            this.txtSubtract.Location = new System.Drawing.Point(109, 95);
            this.txtSubtract.Name = "txtSubtract";
            this.txtSubtract.Size = new System.Drawing.Size(63, 20);
            this.txtSubtract.TabIndex = 29;
            this.txtSubtract.Text = "0";
            // 
            // optSubtract
            // 
            this.optSubtract.AutoSize = true;
            this.optSubtract.Location = new System.Drawing.Point(9, 98);
            this.optSubtract.Name = "optSubtract";
            this.optSubtract.Size = new System.Drawing.Size(65, 17);
            this.optSubtract.TabIndex = 24;
            this.optSubtract.Text = "Subtract";
            this.optSubtract.UseVisualStyleBackColor = true;
            this.optSubtract.CheckedChanged += new System.EventHandler(this.optSubtract_CheckedChanged);
            // 
            // txtAdd
            // 
            this.txtAdd.Enabled = false;
            this.txtAdd.Location = new System.Drawing.Point(109, 69);
            this.txtAdd.Name = "txtAdd";
            this.txtAdd.Size = new System.Drawing.Size(63, 20);
            this.txtAdd.TabIndex = 30;
            this.txtAdd.Text = "0";
            // 
            // optAdd
            // 
            this.optAdd.AutoSize = true;
            this.optAdd.Location = new System.Drawing.Point(9, 70);
            this.optAdd.Name = "optAdd";
            this.optAdd.Size = new System.Drawing.Size(44, 17);
            this.optAdd.TabIndex = 25;
            this.optAdd.Text = "Add";
            this.optAdd.UseVisualStyleBackColor = true;
            this.optAdd.CheckedChanged += new System.EventHandler(this.optAdd_CheckedChanged);
            // 
            // txtSet
            // 
            this.txtSet.Location = new System.Drawing.Point(109, 43);
            this.txtSet.Name = "txtSet";
            this.txtSet.Size = new System.Drawing.Size(63, 20);
            this.txtSet.TabIndex = 31;
            this.txtSet.Text = "0";
            // 
            // optSet
            // 
            this.optSet.AutoSize = true;
            this.optSet.Location = new System.Drawing.Point(9, 44);
            this.optSet.Name = "optSet";
            this.optSet.Size = new System.Drawing.Size(41, 17);
            this.optSet.TabIndex = 26;
            this.optSet.Text = "Set";
            this.optSet.UseVisualStyleBackColor = true;
            this.optSet.CheckedChanged += new System.EventHandler(this.optSet_CheckedChanged);
            // 
            // cmbVariable
            // 
            this.cmbVariable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVariable.FormattingEnabled = true;
            this.cmbVariable.Location = new System.Drawing.Point(63, 16);
            this.cmbVariable.Name = "cmbVariable";
            this.cmbVariable.Size = new System.Drawing.Size(109, 21);
            this.cmbVariable.TabIndex = 22;
            // 
            // lblRandomLabel
            // 
            this.lblRandomLabel.AutoSize = true;
            this.lblRandomLabel.Location = new System.Drawing.Point(9, 19);
            this.lblRandomLabel.Name = "lblRandomLabel";
            this.lblRandomLabel.Size = new System.Drawing.Size(48, 13);
            this.lblRandomLabel.TabIndex = 21;
            this.lblRandomLabel.Text = "Variable:";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(95, 224);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(14, 224);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Ok";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cmbVariable);
            this.groupBox2.Controls.Add(this.lblRandomLabel);
            this.groupBox2.Controls.Add(this.lblRandomLabel37);
            this.groupBox2.Controls.Add(this.optSet);
            this.groupBox2.Controls.Add(this.lblRandomLabel13);
            this.groupBox2.Controls.Add(this.txtSet);
            this.groupBox2.Controls.Add(this.txtRandomHigh);
            this.groupBox2.Controls.Add(this.optAdd);
            this.groupBox2.Controls.Add(this.txtRandomLow);
            this.groupBox2.Controls.Add(this.txtAdd);
            this.groupBox2.Controls.Add(this.optRandom);
            this.groupBox2.Controls.Add(this.optSubtract);
            this.groupBox2.Controls.Add(this.txtSubtract);
            this.groupBox2.Location = new System.Drawing.Point(3, 43);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(180, 175);
            this.groupBox2.TabIndex = 36;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Variable:";
            // 
            // EventCommand_Variable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.groupBox1);
            this.Name = "EventCommand_Variable";
            this.Size = new System.Drawing.Size(195, 260);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        internal System.Windows.Forms.Label lblRandomLabel37;
        internal System.Windows.Forms.Label lblRandomLabel13;
        internal System.Windows.Forms.TextBox txtRandomHigh;
        internal System.Windows.Forms.TextBox txtRandomLow;
        internal System.Windows.Forms.RadioButton optRandom;
        internal System.Windows.Forms.TextBox txtSubtract;
        internal System.Windows.Forms.RadioButton optSubtract;
        internal System.Windows.Forms.TextBox txtAdd;
        internal System.Windows.Forms.RadioButton optAdd;
        internal System.Windows.Forms.TextBox txtSet;
        internal System.Windows.Forms.RadioButton optSet;
        internal System.Windows.Forms.ComboBox cmbVariable;
        internal System.Windows.Forms.Label lblRandomLabel;
        private System.Windows.Forms.RadioButton rdoGlobalVariable;
        private System.Windows.Forms.RadioButton rdoPlayerVariable;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}
