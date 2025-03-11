namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

partial class ConditionControl_TimeBetween
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
        grpTime = new DarkUI.Controls.DarkGroupBox();
        lblEndRange = new Label();
        lblStartRange = new Label();
        cmbTime2 = new DarkUI.Controls.DarkComboBox();
        cmbTime1 = new DarkUI.Controls.DarkComboBox();
        lblAnd = new Label();
        grpTime.SuspendLayout();
        SuspendLayout();
        // 
        // grpTime
        // 
        grpTime.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        grpTime.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        grpTime.Controls.Add(lblEndRange);
        grpTime.Controls.Add(lblStartRange);
        grpTime.Controls.Add(cmbTime2);
        grpTime.Controls.Add(cmbTime1);
        grpTime.Controls.Add(lblAnd);
        grpTime.ForeColor = System.Drawing.Color.Gainsboro;
        grpTime.Location = new System.Drawing.Point(0, 0);
        grpTime.Margin = new Padding(4, 3, 4, 3);
        grpTime.Name = "grpTime";
        grpTime.Padding = new Padding(4, 3, 4, 3);
        grpTime.Size = new Size(308, 121);
        grpTime.TabIndex = 31;
        grpTime.TabStop = false;
        grpTime.Text = "Time is between:";
        // 
        // lblEndRange
        // 
        lblEndRange.AutoSize = true;
        lblEndRange.Location = new System.Drawing.Point(11, 84);
        lblEndRange.Margin = new Padding(4, 0, 4, 0);
        lblEndRange.Name = "lblEndRange";
        lblEndRange.Size = new Size(66, 15);
        lblEndRange.TabIndex = 6;
        lblEndRange.Text = "End Range:";
        // 
        // lblStartRange
        // 
        lblStartRange.AutoSize = true;
        lblStartRange.Location = new System.Drawing.Point(8, 24);
        lblStartRange.Margin = new Padding(4, 0, 4, 0);
        lblStartRange.Name = "lblStartRange";
        lblStartRange.Size = new Size(70, 15);
        lblStartRange.TabIndex = 5;
        lblStartRange.Text = "Start Range:";
        // 
        // cmbTime2
        // 
        cmbTime2.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbTime2.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbTime2.BorderStyle = ButtonBorderStyle.Solid;
        cmbTime2.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbTime2.DrawDropdownHoverOutline = false;
        cmbTime2.DrawFocusRectangle = false;
        cmbTime2.DrawMode = DrawMode.OwnerDrawFixed;
        cmbTime2.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbTime2.FlatStyle = FlatStyle.Flat;
        cmbTime2.ForeColor = System.Drawing.Color.Gainsboro;
        cmbTime2.FormattingEnabled = true;
        cmbTime2.Location = new System.Drawing.Point(107, 81);
        cmbTime2.Margin = new Padding(4, 3, 4, 3);
        cmbTime2.Name = "cmbTime2";
        cmbTime2.Size = new Size(187, 24);
        cmbTime2.TabIndex = 4;
        cmbTime2.Text = null;
        cmbTime2.TextPadding = new Padding(2);
        // 
        // cmbTime1
        // 
        cmbTime1.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbTime1.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbTime1.BorderStyle = ButtonBorderStyle.Solid;
        cmbTime1.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbTime1.DrawDropdownHoverOutline = false;
        cmbTime1.DrawFocusRectangle = false;
        cmbTime1.DrawMode = DrawMode.OwnerDrawFixed;
        cmbTime1.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbTime1.FlatStyle = FlatStyle.Flat;
        cmbTime1.ForeColor = System.Drawing.Color.Gainsboro;
        cmbTime1.FormattingEnabled = true;
        cmbTime1.Location = new System.Drawing.Point(107, 21);
        cmbTime1.Margin = new Padding(4, 3, 4, 3);
        cmbTime1.Name = "cmbTime1";
        cmbTime1.Size = new Size(187, 24);
        cmbTime1.TabIndex = 3;
        cmbTime1.Text = null;
        cmbTime1.TextPadding = new Padding(2);
        // 
        // lblAnd
        // 
        lblAnd.AutoSize = true;
        lblAnd.Location = new System.Drawing.Point(118, 57);
        lblAnd.Margin = new Padding(4, 0, 4, 0);
        lblAnd.Name = "lblAnd";
        lblAnd.Size = new Size(29, 15);
        lblAnd.TabIndex = 2;
        lblAnd.Text = "And";
        // 
        // ConditionControl_TimeBetween
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        Controls.Add(grpTime);
        Name = "ConditionControl_TimeBetween";
        Size = new Size(308, 125);
        grpTime.ResumeLayout(false);
        grpTime.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private DarkUI.Controls.DarkGroupBox grpTime;
    private Label lblEndRange;
    private Label lblStartRange;
    private DarkUI.Controls.DarkComboBox cmbTime2;
    private DarkUI.Controls.DarkComboBox cmbTime1;
    private Label lblAnd;
}
