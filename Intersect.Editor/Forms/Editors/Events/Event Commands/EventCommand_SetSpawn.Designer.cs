namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
	partial class EventCommand_SetSpawn
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EventCommand_SetSpawn));
			this.grpWarp = new DarkUI.Controls.DarkGroupBox();
			this.btnVisual = new DarkUI.Controls.DarkButton();
			this.btnCancel = new DarkUI.Controls.DarkButton();
			this.btnSave = new DarkUI.Controls.DarkButton();
			this.scrlY = new DarkUI.Controls.DarkScrollBar();
			this.scrlX = new DarkUI.Controls.DarkScrollBar();
			this.cmbMap = new DarkUI.Controls.DarkComboBox();
			this.lblY = new System.Windows.Forms.Label();
			this.lblMap = new System.Windows.Forms.Label();
			this.lblX = new System.Windows.Forms.Label();
			this.grpWarp.SuspendLayout();
			this.SuspendLayout();
			// 
			// grpWarp
			// 
			this.grpWarp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
			this.grpWarp.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
			this.grpWarp.Controls.Add(this.btnVisual);
			this.grpWarp.Controls.Add(this.btnCancel);
			this.grpWarp.Controls.Add(this.btnSave);
			this.grpWarp.Controls.Add(this.scrlY);
			this.grpWarp.Controls.Add(this.scrlX);
			this.grpWarp.Controls.Add(this.cmbMap);
			this.grpWarp.Controls.Add(this.lblY);
			this.grpWarp.Controls.Add(this.lblMap);
			this.grpWarp.Controls.Add(this.lblX);
			this.grpWarp.ForeColor = System.Drawing.Color.Gainsboro;
			this.grpWarp.Location = new System.Drawing.Point(3, 3);
			this.grpWarp.Name = "grpWarp";
			this.grpWarp.Size = new System.Drawing.Size(182, 156);
			this.grpWarp.TabIndex = 18;
			this.grpWarp.TabStop = false;
			this.grpWarp.Text = "Set Spawn";
			// 
			// btnVisual
			// 
			this.btnVisual.Location = new System.Drawing.Point(12, 96);
			this.btnVisual.Name = "btnVisual";
			this.btnVisual.Padding = new System.Windows.Forms.Padding(5);
			this.btnVisual.Size = new System.Drawing.Size(155, 23);
			this.btnVisual.TabIndex = 21;
			this.btnVisual.Text = "Open Visual Interface";
			this.btnVisual.Click += new System.EventHandler(this.btnVisual_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(92, 125);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 20;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnSave
			// 
			this.btnSave.Location = new System.Drawing.Point(12, 125);
			this.btnSave.Name = "btnSave";
			this.btnSave.Padding = new System.Windows.Forms.Padding(5);
			this.btnSave.Size = new System.Drawing.Size(75, 23);
			this.btnSave.TabIndex = 19;
			this.btnSave.Text = "Ok";
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// scrlY
			// 
			this.scrlY.Location = new System.Drawing.Point(46, 73);
			this.scrlY.Name = "scrlY";
			this.scrlY.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
			this.scrlY.Size = new System.Drawing.Size(121, 17);
			this.scrlY.TabIndex = 18;
			this.scrlY.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlY_ValueChanged);
			// 
			// scrlX
			// 
			this.scrlX.Location = new System.Drawing.Point(46, 46);
			this.scrlX.Name = "scrlX";
			this.scrlX.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
			this.scrlX.Size = new System.Drawing.Size(121, 17);
			this.scrlX.TabIndex = 17;
			this.scrlX.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlX_ValueChanged);
			// 
			// cmbMap
			// 
			this.cmbMap.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
			this.cmbMap.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
			this.cmbMap.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
			this.cmbMap.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
			this.cmbMap.ButtonIcon = ((System.Drawing.Bitmap)(resources.GetObject("cmbMap.ButtonIcon")));
			this.cmbMap.DrawDropdownHoverOutline = false;
			this.cmbMap.DrawFocusRectangle = false;
			this.cmbMap.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.cmbMap.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbMap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.cmbMap.ForeColor = System.Drawing.Color.Gainsboro;
			this.cmbMap.FormattingEnabled = true;
			this.cmbMap.Location = new System.Drawing.Point(46, 16);
			this.cmbMap.Name = "cmbMap";
			this.cmbMap.Size = new System.Drawing.Size(121, 21);
			this.cmbMap.TabIndex = 16;
			this.cmbMap.Text = null;
			this.cmbMap.TextPadding = new System.Windows.Forms.Padding(2);
			this.cmbMap.SelectedIndexChanged += new System.EventHandler(this.cmbMap_SelectedIndexChanged);
			// 
			// lblY
			// 
			this.lblY.AutoSize = true;
			this.lblY.Location = new System.Drawing.Point(9, 73);
			this.lblY.Name = "lblY";
			this.lblY.Size = new System.Drawing.Size(26, 13);
			this.lblY.TabIndex = 13;
			this.lblY.Text = "Y: 0";
			// 
			// lblMap
			// 
			this.lblMap.AutoSize = true;
			this.lblMap.Location = new System.Drawing.Point(9, 19);
			this.lblMap.Name = "lblMap";
			this.lblMap.Size = new System.Drawing.Size(31, 13);
			this.lblMap.TabIndex = 8;
			this.lblMap.Text = "Map:";
			// 
			// lblX
			// 
			this.lblX.AutoSize = true;
			this.lblX.Location = new System.Drawing.Point(9, 46);
			this.lblX.Name = "lblX";
			this.lblX.Size = new System.Drawing.Size(26, 13);
			this.lblX.TabIndex = 12;
			this.lblX.Text = "X: 0";
			// 
			// EventCommand_SetSpawn
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
			this.Controls.Add(this.grpWarp);
			this.Name = "EventCommand_SetSpawn";
			this.Size = new System.Drawing.Size(193, 165);
			this.grpWarp.ResumeLayout(false);
			this.grpWarp.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private DarkUI.Controls.DarkGroupBox grpWarp;
		private DarkUI.Controls.DarkButton btnVisual;
		private DarkUI.Controls.DarkButton btnCancel;
		private DarkUI.Controls.DarkButton btnSave;
		private DarkUI.Controls.DarkScrollBar scrlY;
		private DarkUI.Controls.DarkScrollBar scrlX;
		private DarkUI.Controls.DarkComboBox cmbMap;
		private System.Windows.Forms.Label lblY;
		private System.Windows.Forms.Label lblMap;
		private System.Windows.Forms.Label lblX;
	}
}
