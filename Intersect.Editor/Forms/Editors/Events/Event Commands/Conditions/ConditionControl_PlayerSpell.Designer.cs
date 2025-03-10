namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

partial class ConditionControl_PlayerSpell
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
        grpSpell = new DarkUI.Controls.DarkGroupBox();
        cmbSpell = new DarkUI.Controls.DarkComboBox();
        lblSpell = new Label();
        grpSpell.SuspendLayout();
        SuspendLayout();
        // 
        // grpSpell
        // 
        grpSpell.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        grpSpell.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        grpSpell.Controls.Add(cmbSpell);
        grpSpell.Controls.Add(lblSpell);
        grpSpell.ForeColor = System.Drawing.Color.Gainsboro;
        grpSpell.Location = new System.Drawing.Point(0, 0);
        grpSpell.Margin = new Padding(4, 3, 4, 3);
        grpSpell.Name = "grpSpell";
        grpSpell.Padding = new Padding(4, 3, 4, 3);
        grpSpell.Size = new Size(306, 60);
        grpSpell.TabIndex = 27;
        grpSpell.TabStop = false;
        grpSpell.Text = "Knows Spell";
        // 
        // cmbSpell
        // 
        cmbSpell.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbSpell.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbSpell.BorderStyle = ButtonBorderStyle.Solid;
        cmbSpell.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbSpell.DrawDropdownHoverOutline = false;
        cmbSpell.DrawFocusRectangle = false;
        cmbSpell.DrawMode = DrawMode.OwnerDrawFixed;
        cmbSpell.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbSpell.FlatStyle = FlatStyle.Flat;
        cmbSpell.ForeColor = System.Drawing.Color.Gainsboro;
        cmbSpell.FormattingEnabled = true;
        cmbSpell.Location = new System.Drawing.Point(92, 21);
        cmbSpell.Margin = new Padding(4, 3, 4, 3);
        cmbSpell.Name = "cmbSpell";
        cmbSpell.Size = new Size(204, 24);
        cmbSpell.TabIndex = 3;
        cmbSpell.Text = null;
        cmbSpell.TextPadding = new Padding(2);
        // 
        // lblSpell
        // 
        lblSpell.AutoSize = true;
        lblSpell.Location = new System.Drawing.Point(9, 23);
        lblSpell.Margin = new Padding(4, 0, 4, 0);
        lblSpell.Name = "lblSpell";
        lblSpell.Size = new Size(35, 15);
        lblSpell.TabIndex = 2;
        lblSpell.Text = "Spell:";
        // 
        // KnownSpell
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        Controls.Add(grpSpell);
        Name = "KnownSpell";
        Size = new Size(311, 64);
        grpSpell.ResumeLayout(false);
        grpSpell.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private DarkUI.Controls.DarkGroupBox grpSpell;
    private DarkUI.Controls.DarkComboBox cmbSpell;
    private Label lblSpell;
}
