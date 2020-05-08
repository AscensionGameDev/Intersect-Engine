namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
	partial class EventCommand_Job
	{
		/// <summary> 
		/// Variable nécessaire au concepteur.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Nettoyage des ressources utilisées.
		/// </summary>
		/// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Code généré par le Concepteur de composants

		/// <summary> 
		/// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
		/// le contenu de cette méthode avec l'éditeur de code.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EventCommand_Job));
			this.grpChangeValue = new DarkUI.Controls.DarkGroupBox();
			this.nudValue = new DarkUI.Controls.DarkNumericUpDown();
			this.lblVital = new System.Windows.Forms.Label();
			this.btnCancel = new DarkUI.Controls.DarkButton();
			this.btnSave = new DarkUI.Controls.DarkButton();
			this.grpSelectJob = new DarkUI.Controls.DarkGroupBox();
			this.cmbJob = new DarkUI.Controls.DarkComboBox();
			this.grpChangeValue.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudValue)).BeginInit();
			this.grpSelectJob.SuspendLayout();
			this.SuspendLayout();
			// 
			// grpChangeValue
			// 
			this.grpChangeValue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
			this.grpChangeValue.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
			this.grpChangeValue.Controls.Add(this.nudValue);
			this.grpChangeValue.Controls.Add(this.lblVital);
			this.grpChangeValue.ForeColor = System.Drawing.Color.Gainsboro;
			this.grpChangeValue.Location = new System.Drawing.Point(3, 3);
			this.grpChangeValue.Name = "grpChangeValue";
			this.grpChangeValue.Size = new System.Drawing.Size(259, 50);
			this.grpChangeValue.TabIndex = 18;
			this.grpChangeValue.TabStop = false;
			this.grpChangeValue.Text = "Change Value:";
			// 
			// nudValue
			// 
			this.nudValue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
			this.nudValue.ForeColor = System.Drawing.Color.Gainsboro;
			this.nudValue.Location = new System.Drawing.Point(89, 20);
			this.nudValue.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
			this.nudValue.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
			this.nudValue.Name = "nudValue";
			this.nudValue.Size = new System.Drawing.Size(164, 20);
			this.nudValue.TabIndex = 22;
			this.nudValue.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
			// 
			// lblVital
			// 
			this.lblVital.AutoSize = true;
			this.lblVital.Location = new System.Drawing.Point(4, 22);
			this.lblVital.Name = "lblVital";
			this.lblVital.Size = new System.Drawing.Size(83, 13);
			this.lblVital.TabIndex = 21;
			this.lblVital.Text = "Set/Add Value :";
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(181, 59);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 20;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnSave
			// 
			this.btnSave.Location = new System.Drawing.Point(3, 59);
			this.btnSave.Name = "btnSave";
			this.btnSave.Padding = new System.Windows.Forms.Padding(5);
			this.btnSave.Size = new System.Drawing.Size(75, 23);
			this.btnSave.TabIndex = 19;
			this.btnSave.Text = "Ok";
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// grpSelectJob
			// 
			this.grpSelectJob.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
			this.grpSelectJob.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
			this.grpSelectJob.Controls.Add(this.cmbJob);
			this.grpSelectJob.ForeColor = System.Drawing.Color.Gainsboro;
			this.grpSelectJob.Location = new System.Drawing.Point(3, 3);
			this.grpSelectJob.Name = "grpSelectJob";
			this.grpSelectJob.Size = new System.Drawing.Size(259, 50);
			this.grpSelectJob.TabIndex = 23;
			this.grpSelectJob.TabStop = false;
			this.grpSelectJob.Text = "Select Job:";
			// 
			// cmbJob
			// 
			this.cmbJob.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
			this.cmbJob.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
			this.cmbJob.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
			this.cmbJob.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
			this.cmbJob.ButtonIcon = ((System.Drawing.Bitmap)(resources.GetObject("cmbJob.ButtonIcon")));
			this.cmbJob.DrawDropdownHoverOutline = false;
			this.cmbJob.DrawFocusRectangle = false;
			this.cmbJob.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.cmbJob.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbJob.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.cmbJob.ForeColor = System.Drawing.Color.Gainsboro;
			this.cmbJob.FormattingEnabled = true;
			this.cmbJob.Items.AddRange(new object[] {
            "None",
            "Bucheron",
            "Mineur"});
			this.cmbJob.Location = new System.Drawing.Point(5, 20);
			this.cmbJob.Name = "cmbJob";
			this.cmbJob.Size = new System.Drawing.Size(248, 21);
			this.cmbJob.TabIndex = 24;
			this.cmbJob.Text = "None";
			this.cmbJob.TextPadding = new System.Windows.Forms.Padding(2);
			// 
			// EventCommand_Job
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
			this.Controls.Add(this.grpSelectJob);
			this.Controls.Add(this.grpChangeValue);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnSave);
			this.Name = "EventCommand_Job";
			this.Size = new System.Drawing.Size(271, 90);
			this.grpChangeValue.ResumeLayout(false);
			this.grpChangeValue.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudValue)).EndInit();
			this.grpSelectJob.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private DarkUI.Controls.DarkGroupBox grpChangeValue;
		private DarkUI.Controls.DarkNumericUpDown nudValue;
		private System.Windows.Forms.Label lblVital;
		private DarkUI.Controls.DarkButton btnCancel;
		private DarkUI.Controls.DarkButton btnSave;
		private DarkUI.Controls.DarkGroupBox grpSelectJob;
		private DarkUI.Controls.DarkComboBox cmbJob;
	}
}
