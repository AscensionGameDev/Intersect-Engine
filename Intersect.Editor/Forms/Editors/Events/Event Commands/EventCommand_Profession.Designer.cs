using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    partial class EventCommandProfession
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.grpProfession = new DarkUI.Controls.DarkGroupBox();
            this.nudAmount = new DarkUI.Controls.DarkNumericUpDown();
            this.lblAmount = new System.Windows.Forms.Label();
            this.cmbRecipe = new DarkUI.Controls.DarkComboBox();
            this.lblRecipe = new System.Windows.Forms.Label();
            this.cmbProfession = new DarkUI.Controls.DarkComboBox();
            this.lblProfession = new System.Windows.Forms.Label();
            this.cmbAction = new DarkUI.Controls.DarkComboBox();
            this.lblAction = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.grpProfession.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudAmount)).BeginInit();
            this.SuspendLayout();
            //
            // grpProfession
            //
            this.grpProfession.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpProfession.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpProfession.Controls.Add(this.nudAmount);
            this.grpProfession.Controls.Add(this.lblAmount);
            this.grpProfession.Controls.Add(this.cmbRecipe);
            this.grpProfession.Controls.Add(this.lblRecipe);
            this.grpProfession.Controls.Add(this.cmbProfession);
            this.grpProfession.Controls.Add(this.lblProfession);
            this.grpProfession.Controls.Add(this.cmbAction);
            this.grpProfession.Controls.Add(this.lblAction);
            this.grpProfession.Controls.Add(this.btnCancel);
            this.grpProfession.Controls.Add(this.btnSave);
            this.grpProfession.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpProfession.Location = new System.Drawing.Point(3, 3);
            this.grpProfession.Name = "grpProfession";
            this.grpProfession.Size = new System.Drawing.Size(260, 190);
            this.grpProfession.TabIndex = 17;
            this.grpProfession.TabStop = false;
            this.grpProfession.Text = "Profession";
            //
            // lblAction
            //
            this.lblAction.AutoSize = true;
            this.lblAction.Location = new System.Drawing.Point(6, 24);
            this.lblAction.Name = "lblAction";
            this.lblAction.Size = new System.Drawing.Size(40, 13);
            this.lblAction.TabIndex = 21;
            this.lblAction.Text = "Action :";
            //
            // cmbAction
            //
            this.cmbAction.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbAction.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbAction.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbAction.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAction.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbAction.FormattingEnabled = true;
            this.cmbAction.Location = new System.Drawing.Point(70, 21);
            this.cmbAction.Name = "cmbAction";
            this.cmbAction.Size = new System.Drawing.Size(180, 21);
            this.cmbAction.TabIndex = 22;
            this.cmbAction.SelectedIndexChanged += new System.EventHandler(this.cmbAction_SelectedIndexChanged);
            //
            // lblProfession
            //
            this.lblProfession.AutoSize = true;
            this.lblProfession.Location = new System.Drawing.Point(6, 54);
            this.lblProfession.Name = "lblProfession";
            this.lblProfession.Size = new System.Drawing.Size(40, 13);
            this.lblProfession.TabIndex = 23;
            this.lblProfession.Text = "Métier :";
            //
            // cmbProfession
            //
            this.cmbProfession.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbProfession.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbProfession.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbProfession.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbProfession.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProfession.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbProfession.FormattingEnabled = true;
            this.cmbProfession.Location = new System.Drawing.Point(70, 51);
            this.cmbProfession.Name = "cmbProfession";
            this.cmbProfession.Size = new System.Drawing.Size(180, 21);
            this.cmbProfession.TabIndex = 24;
            //
            // lblRecipe
            //
            this.lblRecipe.AutoSize = true;
            this.lblRecipe.Location = new System.Drawing.Point(6, 84);
            this.lblRecipe.Name = "lblRecipe";
            this.lblRecipe.Size = new System.Drawing.Size(48, 13);
            this.lblRecipe.TabIndex = 25;
            this.lblRecipe.Text = "Recette :";
            //
            // cmbRecipe
            //
            this.cmbRecipe.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbRecipe.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbRecipe.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbRecipe.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbRecipe.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRecipe.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbRecipe.FormattingEnabled = true;
            this.cmbRecipe.Location = new System.Drawing.Point(70, 81);
            this.cmbRecipe.Name = "cmbRecipe";
            this.cmbRecipe.Size = new System.Drawing.Size(180, 21);
            this.cmbRecipe.TabIndex = 26;
            //
            // lblAmount
            //
            this.lblAmount.AutoSize = true;
            this.lblAmount.Location = new System.Drawing.Point(6, 114);
            this.lblAmount.Name = "lblAmount";
            this.lblAmount.Size = new System.Drawing.Size(40, 13);
            this.lblAmount.TabIndex = 27;
            this.lblAmount.Text = "Valeur :";
            //
            // nudAmount
            //
            this.nudAmount.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudAmount.Location = new System.Drawing.Point(70, 111);
            this.nudAmount.Maximum = new decimal(new int[] { 300, 0, 0, 0 });
            this.nudAmount.Name = "nudAmount";
            this.nudAmount.Size = new System.Drawing.Size(80, 20);
            this.nudAmount.TabIndex = 28;
            this.nudAmount.Value = new decimal(new int[] { 0, 0, 0, 0 });
            //
            // btnCancel
            //
            this.btnCancel.Location = new System.Drawing.Point(92, 155);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // btnSave
            //
            this.btnSave.Location = new System.Drawing.Point(10, 155);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(5);
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Ok";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // EventCommandProfession
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpProfession);
            this.Name = "EventCommandProfession";
            this.Size = new System.Drawing.Size(266, 196);
            this.grpProfession.ResumeLayout(false);
            this.grpProfession.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudAmount)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpProfession;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private System.Windows.Forms.Label lblAction;
        private System.Windows.Forms.Label lblProfession;
        private System.Windows.Forms.Label lblRecipe;
        private System.Windows.Forms.Label lblAmount;
        private DarkComboBox cmbAction;
        private DarkComboBox cmbProfession;
        private DarkComboBox cmbRecipe;
        private DarkNumericUpDown nudAmount;
    }
}
