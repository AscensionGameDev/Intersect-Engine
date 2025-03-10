namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

partial class ConditionControl_PlayerGender
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
        grpGender = new DarkUI.Controls.DarkGroupBox();
        cmbGender = new DarkUI.Controls.DarkComboBox();
        lblGender = new Label();
        grpGender.SuspendLayout();
        SuspendLayout();
        // 
        // grpGender
        // 
        grpGender.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        grpGender.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        grpGender.Controls.Add(cmbGender);
        grpGender.Controls.Add(lblGender);
        grpGender.ForeColor = System.Drawing.Color.Gainsboro;
        grpGender.Location = new System.Drawing.Point(0, 0);
        grpGender.Margin = new Padding(4, 3, 4, 3);
        grpGender.Name = "grpGender";
        grpGender.Padding = new Padding(4, 3, 4, 3);
        grpGender.Size = new Size(304, 60);
        grpGender.TabIndex = 34;
        grpGender.TabStop = false;
        grpGender.Text = "Gender Is...";
        // 
        // cmbGender
        // 
        cmbGender.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbGender.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbGender.BorderStyle = ButtonBorderStyle.Solid;
        cmbGender.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbGender.DrawDropdownHoverOutline = false;
        cmbGender.DrawFocusRectangle = false;
        cmbGender.DrawMode = DrawMode.OwnerDrawFixed;
        cmbGender.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbGender.FlatStyle = FlatStyle.Flat;
        cmbGender.ForeColor = System.Drawing.Color.Gainsboro;
        cmbGender.FormattingEnabled = true;
        cmbGender.Location = new System.Drawing.Point(92, 20);
        cmbGender.Margin = new Padding(4, 3, 4, 3);
        cmbGender.Name = "cmbGender";
        cmbGender.Size = new Size(202, 24);
        cmbGender.TabIndex = 1;
        cmbGender.Text = null;
        cmbGender.TextPadding = new Padding(2);
        // 
        // lblGender
        // 
        lblGender.AutoSize = true;
        lblGender.Location = new System.Drawing.Point(9, 23);
        lblGender.Margin = new Padding(4, 0, 4, 0);
        lblGender.Name = "lblGender";
        lblGender.Size = new Size(48, 15);
        lblGender.TabIndex = 0;
        lblGender.Text = "Gender:";
        // 
        // ConditionControl_PlayerGender
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        Controls.Add(grpGender);
        Name = "ConditionControl_PlayerGender";
        Size = new Size(307, 63);
        grpGender.ResumeLayout(false);
        grpGender.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private DarkUI.Controls.DarkGroupBox grpGender;
    private DarkUI.Controls.DarkComboBox cmbGender;
    private Label lblGender;
}
