namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
	partial class EventCommand_HDV
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EventCommand_HDV));
			this.grpSelectHDV = new DarkUI.Controls.DarkGroupBox();
			this.btnCancel = new DarkUI.Controls.DarkButton();
			this.btnSave = new DarkUI.Controls.DarkButton();
			this.cmbHDV = new DarkUI.Controls.DarkComboBox();
			this.grpSelectHDV.SuspendLayout();
			this.SuspendLayout();
			// 
			// grpSelectHDV
			// 
			this.grpSelectHDV.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
			this.grpSelectHDV.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
			this.grpSelectHDV.Controls.Add(this.cmbHDV);
			this.grpSelectHDV.ForeColor = System.Drawing.Color.Gainsboro;
			this.grpSelectHDV.Location = new System.Drawing.Point(6, 6);
			this.grpSelectHDV.Name = "grpSelectHDV";
			this.grpSelectHDV.Size = new System.Drawing.Size(259, 50);
			this.grpSelectHDV.TabIndex = 26;
			this.grpSelectHDV.TabStop = false;
			this.grpSelectHDV.Text = "Select HDV";
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(184, 62);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 25;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnSave
			// 
			this.btnSave.Location = new System.Drawing.Point(6, 62);
			this.btnSave.Name = "btnSave";
			this.btnSave.Padding = new System.Windows.Forms.Padding(5);
			this.btnSave.Size = new System.Drawing.Size(75, 23);
			this.btnSave.TabIndex = 24;
			this.btnSave.Text = "Ok";
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// cmbHDV
			// 
			this.cmbHDV.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
			this.cmbHDV.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
			this.cmbHDV.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
			this.cmbHDV.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
			this.cmbHDV.ButtonIcon = ((System.Drawing.Bitmap)(resources.GetObject("cmbHDV.ButtonIcon")));
			this.cmbHDV.DrawDropdownHoverOutline = false;
			this.cmbHDV.DrawFocusRectangle = false;
			this.cmbHDV.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.cmbHDV.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbHDV.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.cmbHDV.ForeColor = System.Drawing.Color.Gainsboro;
			this.cmbHDV.FormattingEnabled = true;
			this.cmbHDV.Items.AddRange(new object[] {
            "None",
            "Bucheron",
            "Mineur"});
			this.cmbHDV.Location = new System.Drawing.Point(5, 20);
			this.cmbHDV.Name = "cmbHDV";
			this.cmbHDV.Size = new System.Drawing.Size(248, 21);
			this.cmbHDV.TabIndex = 24;
			this.cmbHDV.Text = "None";
			this.cmbHDV.TextPadding = new System.Windows.Forms.Padding(2);
			// 
			// EventCommand_HDV
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
			this.Controls.Add(this.grpSelectHDV);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnSave);
			this.Name = "EventCommand_HDV";
			this.Size = new System.Drawing.Size(271, 90);
			this.grpSelectHDV.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private DarkUI.Controls.DarkGroupBox grpSelectHDV;
		private DarkUI.Controls.DarkComboBox cmbHDV;
		private DarkUI.Controls.DarkButton btnCancel;
		private DarkUI.Controls.DarkButton btnSave;
	}
}
