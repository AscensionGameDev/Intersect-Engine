namespace Intersect.Editor.Forms.Editors.Events.Event_Commands;

partial class EventCommand_ScreenFade
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
        grpFade = new DarkUI.Controls.DarkGroupBox();
        chkWaitForCompletion = new DarkUI.Controls.DarkCheckBox();
        cmbFadeTypes = new DarkUI.Controls.DarkComboBox();
        lblAction = new Label();
        btnCancel = new DarkUI.Controls.DarkButton();
        btnSave = new DarkUI.Controls.DarkButton();
        lblSpeed = new Label();
        nudFadeSpeed = new DarkUI.Controls.DarkNumericUpDown();
        grpFade.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)nudFadeSpeed).BeginInit();
        SuspendLayout();
        // 
        // grpFade
        // 
        grpFade.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        grpFade.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        grpFade.Controls.Add(nudFadeSpeed);
        grpFade.Controls.Add(lblSpeed);
        grpFade.Controls.Add(chkWaitForCompletion);
        grpFade.Controls.Add(cmbFadeTypes);
        grpFade.Controls.Add(lblAction);
        grpFade.Controls.Add(btnCancel);
        grpFade.Controls.Add(btnSave);
        grpFade.ForeColor = System.Drawing.Color.Gainsboro;
        grpFade.Location = new System.Drawing.Point(4, 3);
        grpFade.Margin = new Padding(4, 3, 4, 3);
        grpFade.Name = "grpFade";
        grpFade.Padding = new Padding(4, 3, 4, 3);
        grpFade.Size = new Size(257, 155);
        grpFade.TabIndex = 18;
        grpFade.TabStop = false;
        grpFade.Text = "Screen Fade";
        // 
        // chkWaitForCompletion
        // 
        chkWaitForCompletion.AutoSize = true;
        chkWaitForCompletion.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        chkWaitForCompletion.Checked = true;
        chkWaitForCompletion.CheckState = CheckState.Checked;
        chkWaitForCompletion.Location = new System.Drawing.Point(6, 97);
        chkWaitForCompletion.Margin = new Padding(4, 3, 4, 3);
        chkWaitForCompletion.Name = "chkWaitForCompletion";
        chkWaitForCompletion.Size = new Size(137, 19);
        chkWaitForCompletion.TabIndex = 23;
        chkWaitForCompletion.Text = "Wait for completion?";
        // 
        // cmbFadeTypes
        // 
        cmbFadeTypes.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbFadeTypes.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbFadeTypes.BorderStyle = ButtonBorderStyle.Solid;
        cmbFadeTypes.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbFadeTypes.DrawDropdownHoverOutline = false;
        cmbFadeTypes.DrawFocusRectangle = false;
        cmbFadeTypes.DrawMode = DrawMode.OwnerDrawFixed;
        cmbFadeTypes.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbFadeTypes.FlatStyle = FlatStyle.Flat;
        cmbFadeTypes.ForeColor = System.Drawing.Color.Gainsboro;
        cmbFadeTypes.FormattingEnabled = true;
        cmbFadeTypes.Location = new System.Drawing.Point(55, 22);
        cmbFadeTypes.Margin = new Padding(4, 3, 4, 3);
        cmbFadeTypes.Name = "cmbFadeTypes";
        cmbFadeTypes.Size = new Size(194, 24);
        cmbFadeTypes.TabIndex = 22;
        cmbFadeTypes.Text = null;
        cmbFadeTypes.TextPadding = new Padding(2);
        // 
        // lblAction
        // 
        lblAction.AutoSize = true;
        lblAction.Location = new System.Drawing.Point(6, 25);
        lblAction.Margin = new Padding(4, 0, 4, 0);
        lblAction.Name = "lblAction";
        lblAction.Size = new Size(42, 15);
        lblAction.TabIndex = 21;
        lblAction.Text = "Action";
        // 
        // btnCancel
        // 
        btnCancel.Location = new System.Drawing.Point(161, 122);
        btnCancel.Margin = new Padding(4, 3, 4, 3);
        btnCancel.Name = "btnCancel";
        btnCancel.Padding = new Padding(6);
        btnCancel.Size = new Size(88, 27);
        btnCancel.TabIndex = 20;
        btnCancel.Text = "Cancel";
        btnCancel.Click += btnCancel_Click;
        // 
        // btnSave
        // 
        btnSave.Location = new System.Drawing.Point(65, 122);
        btnSave.Margin = new Padding(4, 3, 4, 3);
        btnSave.Name = "btnSave";
        btnSave.Padding = new Padding(6);
        btnSave.Size = new Size(88, 27);
        btnSave.TabIndex = 19;
        btnSave.Text = "Ok";
        btnSave.Click += btnSave_Click;
        // 
        // lblSpeed
        // 
        lblSpeed.AutoSize = true;
        lblSpeed.Location = new System.Drawing.Point(6, 63);
        lblSpeed.Margin = new Padding(4, 0, 4, 0);
        lblSpeed.Name = "lblSpeed";
        lblSpeed.Size = new Size(66, 15);
        lblSpeed.TabIndex = 24;
        lblSpeed.Text = "Speed (ms)";
        // 
        // nudFadeSpeed
        // 
        nudFadeSpeed.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        nudFadeSpeed.ForeColor = System.Drawing.Color.Gainsboro;
        nudFadeSpeed.Location = new System.Drawing.Point(80, 61);
        nudFadeSpeed.Margin = new Padding(4, 3, 4, 3);
        nudFadeSpeed.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
        nudFadeSpeed.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        nudFadeSpeed.Name = "nudFadeSpeed";
        nudFadeSpeed.Size = new Size(169, 23);
        nudFadeSpeed.TabIndex = 29;
        nudFadeSpeed.Value = new decimal(new int[] { 1, 0, 0, 0 });
        // 
        // EventCommand_ScreenFade
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
        Controls.Add(grpFade);
        Name = "EventCommand_ScreenFade";
        Size = new Size(266, 161);
        grpFade.ResumeLayout(false);
        grpFade.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)nudFadeSpeed).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private DarkUI.Controls.DarkGroupBox grpFade;
    private DarkUI.Controls.DarkCheckBox chkWaitForCompletion;
    private DarkUI.Controls.DarkComboBox cmbFadeTypes;
    private Label lblAction;
    private DarkUI.Controls.DarkButton btnCancel;
    private DarkUI.Controls.DarkButton btnSave;
    private Label lblSpeed;
    private DarkUI.Controls.DarkNumericUpDown nudFadeSpeed;
}
