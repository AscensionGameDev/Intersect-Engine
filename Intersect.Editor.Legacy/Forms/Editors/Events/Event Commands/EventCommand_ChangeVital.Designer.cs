using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
  partial class EventCommandChangeVital
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
      this.grpChangeVital = new DarkUI.Controls.DarkGroupBox();
      this.nudVital = new DarkUI.Controls.DarkNumericUpDown();
      this.lblVital = new System.Windows.Forms.Label();
      this.btnCancel = new DarkUI.Controls.DarkButton();
      this.btnSave = new DarkUI.Controls.DarkButton();
      this.grpChangeVital.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.nudVital)).BeginInit();
      this.SuspendLayout();
      // 
      // grpChangeVital
      // 
      this.grpChangeVital.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
      this.grpChangeVital.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
      this.grpChangeVital.Controls.Add(this.nudVital);
      this.grpChangeVital.Controls.Add(this.lblVital);
      this.grpChangeVital.Controls.Add(this.btnCancel);
      this.grpChangeVital.Controls.Add(this.btnSave);
      this.grpChangeVital.ForeColor = System.Drawing.Color.Gainsboro;
      this.grpChangeVital.Location = new System.Drawing.Point(3, 3);
      this.grpChangeVital.Name = "grpChangeVital";
      this.grpChangeVital.Size = new System.Drawing.Size(259, 79);
      this.grpChangeVital.TabIndex = 17;
      this.grpChangeVital.TabStop = false;
      this.grpChangeVital.Text = "Change Vital:";
      // 
      // nudVital
      // 
      this.nudVital.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
      this.nudVital.ForeColor = System.Drawing.Color.Gainsboro;
      this.nudVital.Location = new System.Drawing.Point(89, 20);
      this.nudVital.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
      this.nudVital.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
      this.nudVital.Name = "nudVital";
      this.nudVital.Size = new System.Drawing.Size(164, 20);
      this.nudVital.TabIndex = 22;
      this.nudVital.Value = new decimal(new int[] {
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
      this.lblVital.Size = new System.Drawing.Size(52, 13);
      this.lblVital.TabIndex = 21;
      this.lblVital.Text = "Add Vital:";
      // 
      // btnCancel
      // 
      this.btnCancel.Location = new System.Drawing.Point(89, 47);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
      this.btnCancel.Size = new System.Drawing.Size(75, 23);
      this.btnCancel.TabIndex = 20;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // btnSave
      // 
      this.btnSave.Location = new System.Drawing.Point(7, 47);
      this.btnSave.Name = "btnSave";
      this.btnSave.Padding = new System.Windows.Forms.Padding(5);
      this.btnSave.Size = new System.Drawing.Size(75, 23);
      this.btnSave.TabIndex = 19;
      this.btnSave.Text = "Ok";
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // EventCommandChangeVital
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoSize = true;
      this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
      this.Controls.Add(this.grpChangeVital);
      this.Name = "EventCommandChangeVital";
      this.Size = new System.Drawing.Size(268, 88);
      this.grpChangeVital.ResumeLayout(false);
      this.grpChangeVital.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.nudVital)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private DarkGroupBox grpChangeVital;
    private DarkButton btnCancel;
    private DarkButton btnSave;
    private System.Windows.Forms.Label lblVital;
    private DarkNumericUpDown nudVital;
  }
}
