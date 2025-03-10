namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

partial class ConditionControl_PlayerStat
{
    /// <summary> 
    /// Variável de designer necessária.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Limpar os recursos que estão sendo usados.
    /// </summary>
    /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Código gerado pelo Designer de Componentes

    /// <summary> 
    /// Método necessário para suporte ao Designer - não modifique 
    /// o conteúdo deste método com o editor de código.
    /// </summary>
    private void InitializeComponent()
    {
        grpLevelStat = new DarkUI.Controls.DarkGroupBox();
        chkStatIgnoreBuffs = new DarkUI.Controls.DarkCheckBox();
        nudLevelStatValue = new DarkUI.Controls.DarkNumericUpDown();
        cmbLevelStat = new DarkUI.Controls.DarkComboBox();
        lblLevelOrStat = new Label();
        lblLvlStatValue = new Label();
        cmbLevelComparator = new DarkUI.Controls.DarkComboBox();
        lblLevelComparator = new Label();
        grpLevelStat.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)nudLevelStatValue).BeginInit();
        SuspendLayout();
        // 
        // grpLevelStat
        // 
        grpLevelStat.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        grpLevelStat.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        grpLevelStat.Controls.Add(chkStatIgnoreBuffs);
        grpLevelStat.Controls.Add(nudLevelStatValue);
        grpLevelStat.Controls.Add(cmbLevelStat);
        grpLevelStat.Controls.Add(lblLevelOrStat);
        grpLevelStat.Controls.Add(lblLvlStatValue);
        grpLevelStat.Controls.Add(cmbLevelComparator);
        grpLevelStat.Controls.Add(lblLevelComparator);
        grpLevelStat.ForeColor = System.Drawing.Color.Gainsboro;
        grpLevelStat.Location = new System.Drawing.Point(0, 0);
        grpLevelStat.Margin = new Padding(4, 3, 4, 3);
        grpLevelStat.Name = "grpLevelStat";
        grpLevelStat.Padding = new Padding(4, 3, 4, 3);
        grpLevelStat.Size = new Size(306, 162);
        grpLevelStat.TabIndex = 29;
        grpLevelStat.TabStop = false;
        grpLevelStat.Text = "Level or Stat is...";
        // 
        // chkStatIgnoreBuffs
        // 
        chkStatIgnoreBuffs.Location = new System.Drawing.Point(15, 133);
        chkStatIgnoreBuffs.Margin = new Padding(4, 3, 4, 3);
        chkStatIgnoreBuffs.Name = "chkStatIgnoreBuffs";
        chkStatIgnoreBuffs.Size = new Size(246, 20);
        chkStatIgnoreBuffs.TabIndex = 32;
        chkStatIgnoreBuffs.Text = "Ignore equipment & spell buffs.";
        // 
        // nudLevelStatValue
        // 
        nudLevelStatValue.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        nudLevelStatValue.ForeColor = System.Drawing.Color.Gainsboro;
        nudLevelStatValue.Location = new System.Drawing.Point(92, 100);
        nudLevelStatValue.Margin = new Padding(4, 3, 4, 3);
        nudLevelStatValue.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
        nudLevelStatValue.Name = "nudLevelStatValue";
        nudLevelStatValue.Size = new Size(208, 23);
        nudLevelStatValue.TabIndex = 8;
        nudLevelStatValue.Value = new decimal(new int[] { 0, 0, 0, 0 });
        // 
        // cmbLevelStat
        // 
        cmbLevelStat.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbLevelStat.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbLevelStat.BorderStyle = ButtonBorderStyle.Solid;
        cmbLevelStat.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbLevelStat.DrawDropdownHoverOutline = false;
        cmbLevelStat.DrawFocusRectangle = false;
        cmbLevelStat.DrawMode = DrawMode.OwnerDrawFixed;
        cmbLevelStat.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbLevelStat.FlatStyle = FlatStyle.Flat;
        cmbLevelStat.ForeColor = System.Drawing.Color.Gainsboro;
        cmbLevelStat.FormattingEnabled = true;
        cmbLevelStat.Items.AddRange(new object[] { "Level", "Attack", "Defense", "Speed", "Ability Power", "Magic Resist" });
        cmbLevelStat.Location = new System.Drawing.Point(92, 27);
        cmbLevelStat.Margin = new Padding(4, 3, 4, 3);
        cmbLevelStat.Name = "cmbLevelStat";
        cmbLevelStat.Size = new Size(206, 24);
        cmbLevelStat.TabIndex = 7;
        cmbLevelStat.Text = "Level";
        cmbLevelStat.TextPadding = new Padding(2);
        // 
        // lblLevelOrStat
        // 
        lblLevelOrStat.AutoSize = true;
        lblLevelOrStat.Location = new System.Drawing.Point(9, 29);
        lblLevelOrStat.Margin = new Padding(4, 0, 4, 0);
        lblLevelOrStat.Name = "lblLevelOrStat";
        lblLevelOrStat.Size = new Size(74, 15);
        lblLevelOrStat.TabIndex = 6;
        lblLevelOrStat.Text = "Level or Stat:";
        // 
        // lblLvlStatValue
        // 
        lblLvlStatValue.AutoSize = true;
        lblLvlStatValue.Location = new System.Drawing.Point(13, 103);
        lblLvlStatValue.Margin = new Padding(4, 0, 4, 0);
        lblLvlStatValue.Name = "lblLvlStatValue";
        lblLvlStatValue.Size = new Size(38, 15);
        lblLvlStatValue.TabIndex = 4;
        lblLvlStatValue.Text = "Value:";
        // 
        // cmbLevelComparator
        // 
        cmbLevelComparator.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbLevelComparator.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbLevelComparator.BorderStyle = ButtonBorderStyle.Solid;
        cmbLevelComparator.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbLevelComparator.DrawDropdownHoverOutline = false;
        cmbLevelComparator.DrawFocusRectangle = false;
        cmbLevelComparator.DrawMode = DrawMode.OwnerDrawFixed;
        cmbLevelComparator.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbLevelComparator.FlatStyle = FlatStyle.Flat;
        cmbLevelComparator.ForeColor = System.Drawing.Color.Gainsboro;
        cmbLevelComparator.FormattingEnabled = true;
        cmbLevelComparator.Location = new System.Drawing.Point(92, 61);
        cmbLevelComparator.Margin = new Padding(4, 3, 4, 3);
        cmbLevelComparator.Name = "cmbLevelComparator";
        cmbLevelComparator.Size = new Size(206, 24);
        cmbLevelComparator.TabIndex = 3;
        cmbLevelComparator.Text = null;
        cmbLevelComparator.TextPadding = new Padding(2);
        // 
        // lblLevelComparator
        // 
        lblLevelComparator.AutoSize = true;
        lblLevelComparator.Location = new System.Drawing.Point(9, 63);
        lblLevelComparator.Margin = new Padding(4, 0, 4, 0);
        lblLevelComparator.Name = "lblLevelComparator";
        lblLevelComparator.Size = new Size(74, 15);
        lblLevelComparator.TabIndex = 2;
        lblLevelComparator.Text = "Comparator:";
        // 
        // ConditionControl_PlayerStat
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        Controls.Add(grpLevelStat);
        Name = "ConditionControl_PlayerStat";
        Size = new Size(310, 167);
        grpLevelStat.ResumeLayout(false);
        grpLevelStat.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)nudLevelStatValue).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private DarkUI.Controls.DarkGroupBox grpLevelStat;
    private DarkUI.Controls.DarkCheckBox chkStatIgnoreBuffs;
    private DarkUI.Controls.DarkNumericUpDown nudLevelStatValue;
    private DarkUI.Controls.DarkComboBox cmbLevelStat;
    private Label lblLevelOrStat;
    private Label lblLvlStatValue;
    private DarkUI.Controls.DarkComboBox cmbLevelComparator;
    private Label lblLevelComparator;
}
