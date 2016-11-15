namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    partial class EventCommand_EndQuest
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
            this.chkRunCompletionTask = new System.Windows.Forms.CheckBox();
            this.cmbQuests = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkRunCompletionTask);
            this.groupBox1.Controls.Add(this.cmbQuests);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(176, 126);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "End Quest";
            // 
            // chkRunCompletionTask
            // 
            this.chkRunCompletionTask.AutoSize = true;
            this.chkRunCompletionTask.Location = new System.Drawing.Point(7, 46);
            this.chkRunCompletionTask.Name = "chkRunCompletionTask";
            this.chkRunCompletionTask.Size = new System.Drawing.Size(166, 17);
            this.chkRunCompletionTask.TabIndex = 23;
            this.chkRunCompletionTask.Text = "Do not run completion event?";
            this.chkRunCompletionTask.UseVisualStyleBackColor = true;
            // 
            // cmbQuests
            // 
            this.cmbQuests.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbQuests.FormattingEnabled = true;
            this.cmbQuests.Location = new System.Drawing.Point(47, 19);
            this.cmbQuests.Name = "cmbQuests";
            this.cmbQuests.Size = new System.Drawing.Size(117, 21);
            this.cmbQuests.TabIndex = 22;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Quest:";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(89, 97);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(7, 97);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Ok";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // EventCommand_EndQuest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.groupBox1);
            this.Name = "EventCommand_EndQuest";
            this.Size = new System.Drawing.Size(182, 132);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbQuests;
        private System.Windows.Forms.CheckBox chkRunCompletionTask;
    }
}
