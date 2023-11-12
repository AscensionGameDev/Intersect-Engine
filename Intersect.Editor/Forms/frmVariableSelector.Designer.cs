namespace Intersect.Editor.Forms;

partial class FrmVariableSelector
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
        btnOk = new DarkUI.Controls.DarkButton();
        btnCancel = new DarkUI.Controls.DarkButton();
        darkGroupBox1 = new DarkUI.Controls.DarkGroupBox();
        grpVariable = new DarkUI.Controls.DarkGroupBox();
        lblBariable = new Label();
        cmbVariables = new DarkUI.Controls.DarkComboBox();
        grpVariablelType = new DarkUI.Controls.DarkGroupBox();
        lblDir = new Label();
        cmbVariableType = new DarkUI.Controls.DarkComboBox();
        darkGroupBox1.SuspendLayout();
        grpVariable.SuspendLayout();
        grpVariablelType.SuspendLayout();
        SuspendLayout();
        // 
        // btnOk
        // 
        btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnOk.Location = new System.Drawing.Point(90, 232);
        btnOk.Margin = new Padding(4, 3, 4, 3);
        btnOk.Name = "btnOk";
        btnOk.Padding = new Padding(6);
        btnOk.Size = new Size(96, 27);
        btnOk.TabIndex = 19;
        btnOk.Text = "Ok";
        btnOk.Click += btnOk_Click;
        // 
        // btnCancel
        // 
        btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnCancel.Location = new System.Drawing.Point(194, 232);
        btnCancel.Margin = new Padding(4, 3, 4, 3);
        btnCancel.Name = "btnCancel";
        btnCancel.Padding = new Padding(6);
        btnCancel.Size = new Size(96, 27);
        btnCancel.TabIndex = 20;
        btnCancel.Text = "Cancel";
        btnCancel.Click += btnCancel_Click;
        // 
        // darkGroupBox1
        // 
        darkGroupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        darkGroupBox1.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
        darkGroupBox1.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        darkGroupBox1.Controls.Add(grpVariable);
        darkGroupBox1.Controls.Add(grpVariablelType);
        darkGroupBox1.ForeColor = System.Drawing.Color.Gainsboro;
        darkGroupBox1.Location = new System.Drawing.Point(13, 12);
        darkGroupBox1.Margin = new Padding(4, 3, 4, 3);
        darkGroupBox1.Name = "darkGroupBox1";
        darkGroupBox1.Padding = new Padding(4, 3, 4, 3);
        darkGroupBox1.Size = new Size(290, 209);
        darkGroupBox1.TabIndex = 21;
        darkGroupBox1.TabStop = false;
        darkGroupBox1.Text = "Variable Type";
        // 
        // grpVariable
        // 
        grpVariable.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        grpVariable.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
        grpVariable.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        grpVariable.Controls.Add(lblBariable);
        grpVariable.Controls.Add(cmbVariables);
        grpVariable.ForeColor = System.Drawing.Color.Gainsboro;
        grpVariable.Location = new System.Drawing.Point(10, 105);
        grpVariable.Margin = new Padding(4, 3, 4, 3);
        grpVariable.Name = "grpVariable";
        grpVariable.Padding = new Padding(4, 3, 4, 3);
        grpVariable.Size = new Size(268, 81);
        grpVariable.TabIndex = 18;
        grpVariable.TabStop = false;
        grpVariable.Text = "Variable";
        // 
        // lblBariable
        // 
        lblBariable.AutoSize = true;
        lblBariable.Location = new System.Drawing.Point(12, 25);
        lblBariable.Margin = new Padding(4, 0, 4, 0);
        lblBariable.Name = "lblBariable";
        lblBariable.Size = new Size(48, 15);
        lblBariable.TabIndex = 17;
        lblBariable.Text = "Variable";
        // 
        // cmbVariables
        // 
        cmbVariables.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbVariables.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbVariables.BorderStyle = ButtonBorderStyle.Solid;
        cmbVariables.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbVariables.DrawDropdownHoverOutline = false;
        cmbVariables.DrawFocusRectangle = false;
        cmbVariables.DrawMode = DrawMode.OwnerDrawFixed;
        cmbVariables.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbVariables.FlatStyle = FlatStyle.Flat;
        cmbVariables.ForeColor = System.Drawing.Color.Gainsboro;
        cmbVariables.FormattingEnabled = true;
        cmbVariables.Location = new System.Drawing.Point(9, 43);
        cmbVariables.Margin = new Padding(4, 3, 4, 3);
        cmbVariables.Name = "cmbVariables";
        cmbVariables.Size = new Size(253, 24);
        cmbVariables.TabIndex = 16;
        cmbVariables.Text = null;
        cmbVariables.TextPadding = new Padding(2);
        cmbVariables.SelectedIndexChanged += cmbVariables_SelectedIndexChanged;
        // 
        // grpVariablelType
        // 
        grpVariablelType.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        grpVariablelType.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
        grpVariablelType.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        grpVariablelType.Controls.Add(lblDir);
        grpVariablelType.Controls.Add(cmbVariableType);
        grpVariablelType.ForeColor = System.Drawing.Color.Gainsboro;
        grpVariablelType.Location = new System.Drawing.Point(10, 22);
        grpVariablelType.Margin = new Padding(4, 3, 4, 3);
        grpVariablelType.Name = "grpVariablelType";
        grpVariablelType.Padding = new Padding(4, 3, 4, 3);
        grpVariablelType.Size = new Size(268, 77);
        grpVariablelType.TabIndex = 9;
        grpVariablelType.TabStop = false;
        grpVariablelType.Text = "Variable Type";
        // 
        // lblDir
        // 
        lblDir.AutoSize = true;
        lblDir.Location = new System.Drawing.Point(11, 25);
        lblDir.Margin = new Padding(4, 0, 4, 0);
        lblDir.Name = "lblDir";
        lblDir.Size = new Size(75, 15);
        lblDir.TabIndex = 17;
        lblDir.Text = "Variable Type";
        // 
        // cmbVariableType
        // 
        cmbVariableType.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbVariableType.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbVariableType.BorderStyle = ButtonBorderStyle.Solid;
        cmbVariableType.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbVariableType.DrawDropdownHoverOutline = false;
        cmbVariableType.DrawFocusRectangle = false;
        cmbVariableType.DrawMode = DrawMode.OwnerDrawFixed;
        cmbVariableType.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbVariableType.FlatStyle = FlatStyle.Flat;
        cmbVariableType.ForeColor = System.Drawing.Color.Gainsboro;
        cmbVariableType.FormattingEnabled = true;
        cmbVariableType.Location = new System.Drawing.Point(9, 43);
        cmbVariableType.Margin = new Padding(4, 3, 4, 3);
        cmbVariableType.Name = "cmbVariableType";
        cmbVariableType.Size = new Size(253, 24);
        cmbVariableType.TabIndex = 16;
        cmbVariableType.Text = null;
        cmbVariableType.TextPadding = new Padding(2);
        cmbVariableType.SelectedIndexChanged += cmbVariableType_SelectedIndexChanged;
        // 
        // FrmVariableSelector
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
        ClientSize = new Size(316, 271);
        Controls.Add(darkGroupBox1);
        Controls.Add(btnCancel);
        Controls.Add(btnOk);
        Name = "FrmVariableSelector";
        Text = "Variable Selector";
        darkGroupBox1.ResumeLayout(false);
        grpVariable.ResumeLayout(false);
        grpVariable.PerformLayout();
        grpVariablelType.ResumeLayout(false);
        grpVariablelType.PerformLayout();
        ResumeLayout(false);
    }

    #endregion
    private DarkUI.Controls.DarkButton btnOk;
    private DarkUI.Controls.DarkButton btnCancel;
    private DarkUI.Controls.DarkGroupBox darkGroupBox1;
    private DarkUI.Controls.DarkGroupBox grpVariable;
    private Label lblBariable;
    private DarkUI.Controls.DarkComboBox cmbVariables;
    private DarkUI.Controls.DarkGroupBox grpVariablelType;
    private Label lblDir;
    private DarkUI.Controls.DarkComboBox cmbVariableType;
}