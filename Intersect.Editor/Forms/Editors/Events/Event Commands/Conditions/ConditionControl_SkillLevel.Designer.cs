namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

partial class ConditionControl_SkillLevel
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
        grpSkillLevel = new DarkUI.Controls.DarkGroupBox();
        nudSkillValue = new DarkUI.Controls.DarkNumericUpDown();
        cmbSkill = new DarkUI.Controls.DarkComboBox();
        lblSkill = new Label();
        lblSkillValue = new Label();
        cmbSkillComparator = new DarkUI.Controls.DarkComboBox();
        lblSkillComparator = new Label();
        grpSkillLevel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)nudSkillValue).BeginInit();
        SuspendLayout();
        // 
        // grpSkillLevel
        // 
        grpSkillLevel.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        grpSkillLevel.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        grpSkillLevel.Controls.Add(nudSkillValue);
        grpSkillLevel.Controls.Add(cmbSkill);
        grpSkillLevel.Controls.Add(lblSkill);
        grpSkillLevel.Controls.Add(lblSkillValue);
        grpSkillLevel.Controls.Add(cmbSkillComparator);
        grpSkillLevel.Controls.Add(lblSkillComparator);
        grpSkillLevel.ForeColor = System.Drawing.Color.Gainsboro;
        grpSkillLevel.Location = new System.Drawing.Point(0, 0);
        grpSkillLevel.Margin = new Padding(4, 3, 4, 3);
        grpSkillLevel.Name = "grpSkillLevel";
        grpSkillLevel.Padding = new Padding(4, 3, 4, 3);
        grpSkillLevel.Size = new Size(308, 112);
        grpSkillLevel.TabIndex = 29;
        grpSkillLevel.TabStop = false;
        grpSkillLevel.Text = "Skill Level is...";
        // 
        // nudSkillValue
        // 
        nudSkillValue.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        nudSkillValue.ForeColor = System.Drawing.Color.Gainsboro;
        nudSkillValue.Location = new System.Drawing.Point(92, 87);
        nudSkillValue.Margin = new Padding(4, 3, 4, 3);
        nudSkillValue.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
        nudSkillValue.Name = "nudSkillValue";
        nudSkillValue.Size = new Size(208, 23);
        nudSkillValue.TabIndex = 8;
        nudSkillValue.Value = new decimal(new int[] { 0, 0, 0, 0 });
        // 
        // cmbSkill
        // 
        cmbSkill.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbSkill.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbSkill.BorderStyle = ButtonBorderStyle.Solid;
        cmbSkill.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbSkill.DrawDropdownHoverOutline = false;
        cmbSkill.DrawFocusRectangle = false;
        cmbSkill.DrawMode = DrawMode.OwnerDrawFixed;
        cmbSkill.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbSkill.FlatStyle = FlatStyle.Flat;
        cmbSkill.ForeColor = System.Drawing.Color.Gainsboro;
        cmbSkill.FormattingEnabled = true;
        cmbSkill.Location = new System.Drawing.Point(92, 27);
        cmbSkill.Margin = new Padding(4, 3, 4, 3);
        cmbSkill.Name = "cmbSkill";
        cmbSkill.Size = new Size(206, 24);
        cmbSkill.TabIndex = 7;
        cmbSkill.Text = null;
        cmbSkill.TextPadding = new Padding(2);
        // 
        // lblSkill
        // 
        lblSkill.AutoSize = true;
        lblSkill.Location = new System.Drawing.Point(9, 29);
        lblSkill.Margin = new Padding(4, 0, 4, 0);
        lblSkill.Name = "lblSkill";
        lblSkill.Size = new Size(35, 15);
        lblSkill.TabIndex = 6;
        lblSkill.Text = "Skill:";
        // 
        // lblSkillValue
        // 
        lblSkillValue.AutoSize = true;
        lblSkillValue.Location = new System.Drawing.Point(13, 90);
        lblSkillValue.Margin = new Padding(4, 0, 4, 0);
        lblSkillValue.Name = "lblSkillValue";
        lblSkillValue.Size = new Size(38, 15);
        lblSkillValue.TabIndex = 4;
        lblSkillValue.Text = "Value:";
        // 
        // cmbSkillComparator
        // 
        cmbSkillComparator.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbSkillComparator.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbSkillComparator.BorderStyle = ButtonBorderStyle.Solid;
        cmbSkillComparator.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbSkillComparator.DrawDropdownHoverOutline = false;
        cmbSkillComparator.DrawFocusRectangle = false;
        cmbSkillComparator.DrawMode = DrawMode.OwnerDrawFixed;
        cmbSkillComparator.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbSkillComparator.FlatStyle = FlatStyle.Flat;
        cmbSkillComparator.ForeColor = System.Drawing.Color.Gainsboro;
        cmbSkillComparator.FormattingEnabled = true;
        cmbSkillComparator.Location = new System.Drawing.Point(92, 57);
        cmbSkillComparator.Margin = new Padding(4, 3, 4, 3);
        cmbSkillComparator.Name = "cmbSkillComparator";
        cmbSkillComparator.Size = new Size(206, 24);
        cmbSkillComparator.TabIndex = 3;
        cmbSkillComparator.Text = null;
        cmbSkillComparator.TextPadding = new Padding(2);
        // 
        // lblSkillComparator
        // 
        lblSkillComparator.AutoSize = true;
        lblSkillComparator.Location = new System.Drawing.Point(9, 59);
        lblSkillComparator.Margin = new Padding(4, 0, 4, 0);
        lblSkillComparator.Name = "lblSkillComparator";
        lblSkillComparator.Size = new Size(74, 15);
        lblSkillComparator.TabIndex = 2;
        lblSkillComparator.Text = "Comparator:";
        // 
        // ConditionControl_SkillLevel
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        Controls.Add(grpSkillLevel);
        Name = "ConditionControl_SkillLevel";
        Size = new Size(310, 116);
        grpSkillLevel.ResumeLayout(false);
        grpSkillLevel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)nudSkillValue).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private DarkUI.Controls.DarkGroupBox grpSkillLevel;
    private DarkUI.Controls.DarkNumericUpDown nudSkillValue;
    private DarkUI.Controls.DarkComboBox cmbSkill;
    private Label lblSkill;
    private Label lblSkillValue;
    private DarkUI.Controls.DarkComboBox cmbSkillComparator;
    private Label lblSkillComparator;
}

