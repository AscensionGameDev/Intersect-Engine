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
        grpSelection = new DarkUI.Controls.DarkGroupBox();
        grpVariable = new DarkUI.Controls.DarkGroupBox();
        cmbVariables = new DarkUI.Controls.DarkComboBox();
        grpVariableType = new DarkUI.Controls.DarkGroupBox();
        cmbVariableType = new DarkUI.Controls.DarkComboBox();
        grpSelection.SuspendLayout();
        grpVariable.SuspendLayout();
        grpVariableType.SuspendLayout();
        SuspendLayout();
        // 
        // btnOk
        // 
        btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnOk.Location = new System.Drawing.Point(90, 203);
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
        btnCancel.Location = new System.Drawing.Point(194, 203);
        btnCancel.Margin = new Padding(4, 3, 4, 3);
        btnCancel.Name = "btnCancel";
        btnCancel.Padding = new Padding(6);
        btnCancel.Size = new Size(96, 27);
        btnCancel.TabIndex = 20;
        btnCancel.Text = "Cancel";
        btnCancel.Click += btnCancel_Click;
        // 
        // grpSelection
        // 
        grpSelection.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        grpSelection.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
        grpSelection.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        grpSelection.Controls.Add(grpVariable);
        grpSelection.Controls.Add(grpVariableType);
        grpSelection.ForeColor = System.Drawing.Color.Gainsboro;
        grpSelection.Location = new System.Drawing.Point(13, 12);
        grpSelection.Margin = new Padding(4, 3, 4, 3);
        grpSelection.Name = "grpSelection";
        grpSelection.Padding = new Padding(4, 3, 4, 3);
        grpSelection.Size = new Size(290, 172);
        grpSelection.TabIndex = 21;
        grpSelection.TabStop = false;
        grpSelection.Text = "Select a Variable";
        // 
        // grpVariable
        // 
        grpVariable.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        grpVariable.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
        grpVariable.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        grpVariable.Controls.Add(cmbVariables);
        grpVariable.ForeColor = System.Drawing.Color.Gainsboro;
        grpVariable.Location = new System.Drawing.Point(10, 91);
        grpVariable.Margin = new Padding(4, 3, 4, 3);
        grpVariable.Name = "grpVariable";
        grpVariable.Padding = new Padding(4, 3, 4, 3);
        grpVariable.Size = new Size(268, 59);
        grpVariable.TabIndex = 18;
        grpVariable.TabStop = false;
        grpVariable.Text = "Variable";
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
        cmbVariables.Location = new System.Drawing.Point(9, 22);
        cmbVariables.Margin = new Padding(4, 3, 4, 3);
        cmbVariables.Name = "cmbVariables";
        cmbVariables.Size = new Size(253, 24);
        cmbVariables.TabIndex = 16;
        cmbVariables.Text = null;
        cmbVariables.TextPadding = new Padding(2);
        cmbVariables.SelectedIndexChanged += cmbVariables_SelectedIndexChanged;
        // 
        // grpVariableType
        // 
        grpVariableType.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        grpVariableType.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
        grpVariableType.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        grpVariableType.Controls.Add(cmbVariableType);
        grpVariableType.ForeColor = System.Drawing.Color.Gainsboro;
        grpVariableType.Location = new System.Drawing.Point(10, 22);
        grpVariableType.Margin = new Padding(4, 3, 4, 3);
        grpVariableType.Name = "grpVariableType";
        grpVariableType.Padding = new Padding(4, 3, 4, 3);
        grpVariableType.Size = new Size(268, 63);
        grpVariableType.TabIndex = 9;
        grpVariableType.TabStop = false;
        grpVariableType.Text = "Variable Type";
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
        cmbVariableType.Location = new System.Drawing.Point(9, 22);
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
        ClientSize = new Size(316, 242);
        Controls.Add(grpSelection);
        Controls.Add(btnCancel);
        Controls.Add(btnOk);
        Name = "FrmVariableSelector";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Variable Selector";
        grpSelection.ResumeLayout(false);
        grpVariable.ResumeLayout(false);
        grpVariableType.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion
    private DarkUI.Controls.DarkButton btnOk;
    private DarkUI.Controls.DarkButton btnCancel;
    private DarkUI.Controls.DarkGroupBox grpSelection;
    private DarkUI.Controls.DarkGroupBox grpVariable;
    private DarkUI.Controls.DarkComboBox cmbVariables;
    private DarkUI.Controls.DarkGroupBox grpVariableType;
    private DarkUI.Controls.DarkComboBox cmbVariableType;
}