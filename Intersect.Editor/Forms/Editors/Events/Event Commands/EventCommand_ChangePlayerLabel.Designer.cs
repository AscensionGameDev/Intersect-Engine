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
      this.chkPlayerNameColor = new System.Windows.Forms.CheckBox();
      this.pnlLightColor = new System.Windows.Forms.Panel();
      this.btnSelectLightColor = new DarkUI.Controls.DarkButton();
      this.btnCancel = new DarkUI.Controls.DarkButton();
      this.btnSave = new DarkUI.Controls.DarkButton();
      this.colorDialog = new System.Windows.Forms.ColorDialog();
      this.rdoGlobalVariables = new DarkUI.Controls.DarkRadioButton();
      this.rdoPlayerVariables = new DarkUI.Controls.DarkRadioButton();
      this.cmbVariable = new DarkUI.Controls.DarkComboBox();
      this.cmbPosition = new DarkUI.Controls.DarkComboBox();
      this.lblPosition = new System.Windows.Forms.Label();
      this.grpChangeLabel.SuspendLayout();
      this.SuspendLayout();
      // 
      // grpChangeLabel
      // 
      this.grpChangeLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
      this.grpChangeLabel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
      this.grpChangeLabel.Controls.Add(this.lblPosition);
      this.grpChangeLabel.Controls.Add(this.cmbPosition);
      this.grpChangeLabel.Controls.Add(this.cmbVariable);
      this.grpChangeLabel.Controls.Add(this.rdoGlobalVariables);
      this.grpChangeLabel.Controls.Add(this.rdoPlayerVariables);
      this.grpChangeLabel.Controls.Add(this.chkPlayerNameColor);
      this.grpChangeLabel.Controls.Add(this.pnlLightColor);
      this.grpChangeLabel.Controls.Add(this.btnSelectLightColor);
      this.grpChangeLabel.Controls.Add(this.btnCancel);
      this.grpChangeLabel.Controls.Add(this.btnSave);
      this.grpChangeLabel.ForeColor = System.Drawing.Color.Gainsboro;
      this.grpChangeLabel.Location = new System.Drawing.Point(3, 3);
      this.grpChangeLabel.Name = "grpChangeLabel";
      this.grpChangeLabel.Size = new System.Drawing.Size(246, 182);
      this.grpChangeLabel.TabIndex = 17;
      this.grpChangeLabel.TabStop = false;
      this.grpChangeLabel.Text = "Change Player Label:";
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
      // pnlLightColor
      // 
      this.pnlLightColor.BackColor = System.Drawing.Color.White;
      this.pnlLightColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.pnlLightColor.Location = new System.Drawing.Point(8, 92);
      this.pnlLightColor.Name = "pnlLightColor";
      this.pnlLightColor.Size = new System.Drawing.Size(31, 29);
      this.pnlLightColor.TabIndex = 41;
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
      // rdoGlobalVariables
      // 
      this.rdoGlobalVariables.AutoSize = true;
      this.rdoGlobalVariables.Location = new System.Drawing.Point(123, 45);
      this.rdoGlobalVariables.Name = "rdoGlobalVariables";
      this.rdoGlobalVariables.Size = new System.Drawing.Size(101, 17);
      this.rdoGlobalVariables.TabIndex = 51;
      this.rdoGlobalVariables.Text = "Global Variables";
      // 
      // rdoPlayerVariables
      // 
      this.rdoPlayerVariables.AutoSize = true;
      this.rdoPlayerVariables.Checked = true;
      this.rdoPlayerVariables.Location = new System.Drawing.Point(6, 45);
      this.rdoPlayerVariables.Name = "rdoPlayerVariables";
      this.rdoPlayerVariables.Size = new System.Drawing.Size(100, 17);
      this.rdoPlayerVariables.TabIndex = 50;
      this.rdoPlayerVariables.TabStop = true;
      this.rdoPlayerVariables.Text = "Player Variables";
      // 
      // cmbVariable
      // 
      this.cmbVariable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
      this.cmbVariable.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
      this.cmbVariable.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
      this.cmbVariable.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
      this.cmbVariable.ButtonIcon = ((System.Drawing.Bitmap)(resources.GetObject("cmbVariable.ButtonIcon")));
      this.cmbVariable.DrawDropdownHoverOutline = false;
      this.cmbVariable.DrawFocusRectangle = false;
      this.cmbVariable.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this.cmbVariable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbVariable.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.cmbVariable.ForeColor = System.Drawing.Color.Gainsboro;
      this.cmbVariable.FormattingEnabled = true;
      this.cmbVariable.Location = new System.Drawing.Point(7, 68);
      this.cmbVariable.Name = "cmbVariable";
      this.cmbVariable.Size = new System.Drawing.Size(234, 21);
      this.cmbVariable.TabIndex = 52;
      this.cmbVariable.Text = null;
      this.cmbVariable.TextPadding = new System.Windows.Forms.Padding(2);
      // 
      // cmbPosition
      // 
      this.cmbPosition.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
      this.cmbPosition.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
      this.cmbPosition.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
      this.cmbPosition.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
      this.cmbPosition.ButtonIcon = ((System.Drawing.Bitmap)(resources.GetObject("cmbPosition.ButtonIcon")));
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
      // lblPosition
      // 
      this.lblPosition.AutoSize = true;
      this.lblPosition.Location = new System.Drawing.Point(6, 22);
      this.lblPosition.Name = "lblPosition";
      this.lblPosition.Size = new System.Drawing.Size(76, 13);
      this.lblPosition.TabIndex = 54;
      this.lblPosition.Text = "Label Position:";
      // 
      // EventCommandChangePlayerLabel
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoSize = true;
      this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
      this.Controls.Add(this.grpChangeLabel);
      this.Name = "EventCommandChangePlayerLabel";
      this.Size = new System.Drawing.Size(254, 188);
      this.grpChangeLabel.ResumeLayout(false);
      this.grpChangeLabel.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private DarkGroupBox grpChangeLabel;
    private DarkButton btnCancel;
    private DarkButton btnSave;
    private System.Windows.Forms.CheckBox chkPlayerNameColor;
    public System.Windows.Forms.Panel pnlLightColor;
    private DarkButton btnSelectLightColor;
    private System.Windows.Forms.ColorDialog colorDialog;
    internal DarkComboBox cmbVariable;
    private DarkRadioButton rdoGlobalVariables;
    private DarkRadioButton rdoPlayerVariables;
    internal DarkComboBox cmbPosition;
    private System.Windows.Forms.Label lblPosition;
  }
}
