namespace Intersect.Editor.Forms.Editors
{
	partial class FormRandomPlacer
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnGenerate = new DarkUI.Controls.DarkButton();
			this.btnCancel = new DarkUI.Controls.DarkButton();
			this.nudMin = new DarkUI.Controls.DarkNumericUpDown();
			this.lblMin = new System.Windows.Forms.Label();
			this.lblMax = new System.Windows.Forms.Label();
			this.nudMax = new DarkUI.Controls.DarkNumericUpDown();
			this.grpAttribut = new DarkUI.Controls.DarkGroupBox();
			this.rbNone = new DarkUI.Controls.DarkRadioButton();
			this.rbBlocked = new DarkUI.Controls.DarkRadioButton();
			((System.ComponentModel.ISupportInitialize)(this.nudMin)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMax)).BeginInit();
			this.grpAttribut.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnGenerate
			// 
			this.btnGenerate.Location = new System.Drawing.Point(12, 140);
			this.btnGenerate.Name = "btnGenerate";
			this.btnGenerate.Padding = new System.Windows.Forms.Padding(5);
			this.btnGenerate.Size = new System.Drawing.Size(132, 27);
			this.btnGenerate.TabIndex = 52;
			this.btnGenerate.Text = "Generate";
			this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(150, 140);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
			this.btnCancel.Size = new System.Drawing.Size(132, 27);
			this.btnCancel.TabIndex = 53;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// nudMin
			// 
			this.nudMin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
			this.nudMin.ForeColor = System.Drawing.Color.Gainsboro;
			this.nudMin.Location = new System.Drawing.Point(43, 7);
			this.nudMin.Maximum = new decimal(new int[] {
			100000,
			0,
			0,
			0});
			this.nudMin.Minimum = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.nudMin.Name = "nudMin";
			this.nudMin.Size = new System.Drawing.Size(239, 20);
			this.nudMin.TabIndex = 55;
			this.nudMin.Value = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.nudMin.ValueChanged += new System.EventHandler(this.nudMin_ValueChanged);
			// 
			// lblMin
			// 
			this.lblMin.AutoSize = true;
			this.lblMin.ForeColor = System.Drawing.Color.White;
			this.lblMin.Location = new System.Drawing.Point(11, 9);
			this.lblMin.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.lblMin.Name = "lblMin";
			this.lblMin.Size = new System.Drawing.Size(27, 13);
			this.lblMin.TabIndex = 54;
			this.lblMin.Text = "Min:";
			// 
			// lblMax
			// 
			this.lblMax.AutoSize = true;
			this.lblMax.ForeColor = System.Drawing.Color.White;
			this.lblMax.Location = new System.Drawing.Point(11, 35);
			this.lblMax.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.lblMax.Name = "lblMax";
			this.lblMax.Size = new System.Drawing.Size(30, 13);
			this.lblMax.TabIndex = 56;
			this.lblMax.Text = "Max:";
			// 
			// nudMax
			// 
			this.nudMax.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
			this.nudMax.ForeColor = System.Drawing.Color.Gainsboro;
			this.nudMax.Location = new System.Drawing.Point(43, 33);
			this.nudMax.Maximum = new decimal(new int[] {
			100000,
			0,
			0,
			0});
			this.nudMax.Minimum = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.nudMax.Name = "nudMax";
			this.nudMax.Size = new System.Drawing.Size(239, 20);
			this.nudMax.TabIndex = 57;
			this.nudMax.Value = new decimal(new int[] {
			6,
			0,
			0,
			0});
			this.nudMax.ValueChanged += new System.EventHandler(this.nudMax_ValueChanged);
			// 
			// grpAttribut
			// 
			this.grpAttribut.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
			this.grpAttribut.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
			this.grpAttribut.Controls.Add(this.rbNone);
			this.grpAttribut.Controls.Add(this.rbBlocked);
			this.grpAttribut.ForeColor = System.Drawing.Color.Gainsboro;
			this.grpAttribut.Location = new System.Drawing.Point(14, 59);
			this.grpAttribut.Name = "grpAttribut";
			this.grpAttribut.Size = new System.Drawing.Size(268, 75);
			this.grpAttribut.TabIndex = 58;
			this.grpAttribut.TabStop = false;
			this.grpAttribut.Text = "Attribute";
			// 
			// rbNone
			// 
			this.rbNone.AutoSize = true;
			this.rbNone.Checked = true;
			this.rbNone.Location = new System.Drawing.Point(5, 18);
			this.rbNone.Margin = new System.Windows.Forms.Padding(2);
			this.rbNone.Name = "rbNone";
			this.rbNone.Size = new System.Drawing.Size(51, 17);
			this.rbNone.TabIndex = 18;
			this.rbNone.TabStop = true;
			this.rbNone.Text = "None";
			// 
			// rbBlocked
			// 
			this.rbBlocked.AutoSize = true;
			this.rbBlocked.Location = new System.Drawing.Point(5, 38);
			this.rbBlocked.Margin = new System.Windows.Forms.Padding(2);
			this.rbBlocked.Name = "rbBlocked";
			this.rbBlocked.Size = new System.Drawing.Size(64, 17);
			this.rbBlocked.TabIndex = 19;
			this.rbBlocked.Text = "Blocked";
			// 
			// FormRandomPlacer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
			this.ClientSize = new System.Drawing.Size(293, 174);
			this.ControlBox = false;
			this.Controls.Add(this.grpAttribut);
			this.Controls.Add(this.nudMax);
			this.Controls.Add(this.lblMax);
			this.Controls.Add(this.nudMin);
			this.Controls.Add(this.lblMin);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnGenerate);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "FormRandomPlacer";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Mimus Random Placer";
			this.Load += new System.EventHandler(this.FormRandomPlacer_Load);
			((System.ComponentModel.ISupportInitialize)(this.nudMin)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMax)).EndInit();
			this.grpAttribut.ResumeLayout(false);
			this.grpAttribut.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private DarkUI.Controls.DarkButton btnGenerate;
		private DarkUI.Controls.DarkButton btnCancel;
		private DarkUI.Controls.DarkNumericUpDown nudMin;
		private System.Windows.Forms.Label lblMin;
		private System.Windows.Forms.Label lblMax;
		private DarkUI.Controls.DarkNumericUpDown nudMax;
		private DarkUI.Controls.DarkGroupBox grpAttribut;
		private DarkUI.Controls.DarkRadioButton rbNone;
		private DarkUI.Controls.DarkRadioButton rbBlocked;
	}
}