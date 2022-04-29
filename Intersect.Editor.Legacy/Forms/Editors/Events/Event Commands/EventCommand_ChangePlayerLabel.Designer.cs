using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
  partial class EventCommandChangePlayerLabel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EventCommandChangePlayerLabel));
            this.grpChangeLabel = new DarkUI.Controls.DarkGroupBox();
            this.lblStringTextVariables = new System.Windows.Forms.Label();
            this.txtLabel = new DarkUI.Controls.DarkTextBox();
            this.lblValue = new System.Windows.Forms.Label();
            this.lblPosition = new System.Windows.Forms.Label();
            this.cmbPosition = new DarkUI.Controls.DarkComboBox();
            this.chkPlayerNameColor = new System.Windows.Forms.CheckBox();
            this.pnlColor = new System.Windows.Forms.Panel();
            this.btnSelectLightColor = new DarkUI.Controls.DarkButton();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.grpChangeLabel.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpChangeLabel
            // 
            this.grpChangeLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpChangeLabel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpChangeLabel.Controls.Add(this.lblStringTextVariables);
            this.grpChangeLabel.Controls.Add(this.txtLabel);
            this.grpChangeLabel.Controls.Add(this.lblValue);
            this.grpChangeLabel.Controls.Add(this.lblPosition);
            this.grpChangeLabel.Controls.Add(this.cmbPosition);
            this.grpChangeLabel.Controls.Add(this.chkPlayerNameColor);
            this.grpChangeLabel.Controls.Add(this.pnlColor);
            this.grpChangeLabel.Controls.Add(this.btnSelectLightColor);
            this.grpChangeLabel.Controls.Add(this.btnCancel);
            this.grpChangeLabel.Controls.Add(this.btnSave);
            this.grpChangeLabel.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpChangeLabel.Location = new System.Drawing.Point(3, 3);
            this.grpChangeLabel.Name = "grpChangeLabel";
            this.grpChangeLabel.Size = new System.Drawing.Size(246, 206);
            this.grpChangeLabel.TabIndex = 17;
            this.grpChangeLabel.TabStop = false;
            this.grpChangeLabel.Text = "Change Player Label:";
            // 
            // lblStringTextVariables
            // 
            this.lblStringTextVariables.AutoSize = true;
            this.lblStringTextVariables.BackColor = System.Drawing.Color.Transparent;
            this.lblStringTextVariables.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStringTextVariables.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.lblStringTextVariables.Location = new System.Drawing.Point(2, 185);
            this.lblStringTextVariables.Name = "lblStringTextVariables";
            this.lblStringTextVariables.Size = new System.Drawing.Size(245, 13);
            this.lblStringTextVariables.TabIndex = 69;
            this.lblStringTextVariables.Text = "Text variables work with strings. Click here for info!";
            this.lblStringTextVariables.Click += new System.EventHandler(this.lblStringTextVariables_Click);
            // 
            // txtLabel
            // 
            this.txtLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtLabel.Location = new System.Drawing.Point(88, 57);
            this.txtLabel.Name = "txtLabel";
            this.txtLabel.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.txtLabel.Size = new System.Drawing.Size(152, 20);
            this.txtLabel.TabIndex = 56;
            // 
            // lblValue
            // 
            this.lblValue.AutoSize = true;
            this.lblValue.Location = new System.Drawing.Point(6, 59);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(34, 13);
            this.lblValue.TabIndex = 55;
            this.lblValue.Text = "Value";
            // 
            // lblPosition
            // 
            this.lblPosition.AutoSize = true;
            this.lblPosition.Location = new System.Drawing.Point(6, 22);
            this.lblPosition.Name = "lblPosition";
            this.lblPosition.Size = new System.Drawing.Size(76, 13);
            this.lblPosition.TabIndex = 54;
            this.lblPosition.Text = "Label Position:";
            // 
            // cmbPosition
            // 
            this.cmbPosition.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbPosition.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbPosition.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbPosition.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbPosition.DrawDropdownHoverOutline = false;
            this.cmbPosition.DrawFocusRectangle = false;
            this.cmbPosition.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbPosition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPosition.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbPosition.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbPosition.FormattingEnabled = true;
            this.cmbPosition.Items.AddRange(new object[] {
            "Above Character Name",
            "Below Character Name"});
            this.cmbPosition.Location = new System.Drawing.Point(88, 19);
            this.cmbPosition.Name = "cmbPosition";
            this.cmbPosition.Size = new System.Drawing.Size(152, 21);
            this.cmbPosition.TabIndex = 53;
            this.cmbPosition.Text = "Above Character Name";
            this.cmbPosition.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // chkPlayerNameColor
            // 
            this.chkPlayerNameColor.AutoSize = true;
            this.chkPlayerNameColor.Location = new System.Drawing.Point(8, 127);
            this.chkPlayerNameColor.Name = "chkPlayerNameColor";
            this.chkPlayerNameColor.Size = new System.Drawing.Size(146, 17);
            this.chkPlayerNameColor.TabIndex = 42;
            this.chkPlayerNameColor.Text = "Copy Player Name Color?";
            this.chkPlayerNameColor.UseVisualStyleBackColor = true;
            // 
            // pnlColor
            // 
            this.pnlColor.BackColor = System.Drawing.Color.White;
            this.pnlColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlColor.Location = new System.Drawing.Point(8, 92);
            this.pnlColor.Name = "pnlColor";
            this.pnlColor.Size = new System.Drawing.Size(31, 29);
            this.pnlColor.TabIndex = 41;
            // 
            // btnSelectLightColor
            // 
            this.btnSelectLightColor.Location = new System.Drawing.Point(55, 98);
            this.btnSelectLightColor.Name = "btnSelectLightColor";
            this.btnSelectLightColor.Padding = new System.Windows.Forms.Padding(5);
            this.btnSelectLightColor.Size = new System.Drawing.Size(90, 23);
            this.btnSelectLightColor.TabIndex = 39;
            this.btnSelectLightColor.Text = "Select Color";
            this.btnSelectLightColor.Click += new System.EventHandler(this.btnSelectLightColor_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(165, 150);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(6, 150);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(5);
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Ok";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // EventCommandChangePlayerLabel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpChangeLabel);
            this.Name = "EventCommandChangePlayerLabel";
            this.Size = new System.Drawing.Size(254, 212);
            this.grpChangeLabel.ResumeLayout(false);
            this.grpChangeLabel.PerformLayout();
            this.ResumeLayout(false);

    }

    #endregion

    private DarkGroupBox grpChangeLabel;
    private DarkButton btnCancel;
    private DarkButton btnSave;
    private System.Windows.Forms.CheckBox chkPlayerNameColor;
    public System.Windows.Forms.Panel pnlColor;
    private DarkButton btnSelectLightColor;
    private System.Windows.Forms.ColorDialog colorDialog;
    internal DarkComboBox cmbPosition;
    private System.Windows.Forms.Label lblPosition;
        private System.Windows.Forms.Label lblValue;
        private DarkTextBox txtLabel;
        private System.Windows.Forms.Label lblStringTextVariables;
    }
}
