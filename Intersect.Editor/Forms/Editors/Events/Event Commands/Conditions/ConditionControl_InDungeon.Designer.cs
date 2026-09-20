namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

partial class ConditionControl_InDungeon
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
        grpDungeon = new DarkUI.Controls.DarkGroupBox();
        lblDungeon = new Label();
        cmbDungeon = new DarkUI.Controls.DarkComboBox();
        grpDungeon.SuspendLayout();
        SuspendLayout();
        // 
        // grpDungeon
        // 
        grpDungeon.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        grpDungeon.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        grpDungeon.Controls.Add(lblDungeon);
        grpDungeon.Controls.Add(cmbDungeon);
        grpDungeon.ForeColor = System.Drawing.Color.Gainsboro;
        grpDungeon.Location = new System.Drawing.Point(0, 0);
        grpDungeon.Margin = new Padding(4, 3, 4, 3);
        grpDungeon.Name = "grpDungeon";
        grpDungeon.Padding = new Padding(4, 3, 4, 3);
        grpDungeon.Size = new Size(308, 58);
        grpDungeon.TabIndex = 59;
        grpDungeon.TabStop = false;
        grpDungeon.Text = "In Dungeon:";
        // 
        // lblDungeon
        // 
        lblDungeon.AutoSize = true;
        lblDungeon.Location = new System.Drawing.Point(8, 24);
        lblDungeon.Margin = new Padding(4, 0, 4, 0);
        lblDungeon.Name = "lblDungeon";
        lblDungeon.Size = new Size(91, 15);
        lblDungeon.TabIndex = 5;
        lblDungeon.Text = "Dungeon:";
        // 
        // cmbDungeon
        // 
        cmbDungeon.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbDungeon.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbDungeon.BorderStyle = ButtonBorderStyle.Solid;
        cmbDungeon.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbDungeon.DrawDropdownHoverOutline = false;
        cmbDungeon.DrawFocusRectangle = false;
        cmbDungeon.DrawMode = DrawMode.OwnerDrawFixed;
        cmbDungeon.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbDungeon.FlatStyle = FlatStyle.Flat;
        cmbDungeon.ForeColor = System.Drawing.Color.Gainsboro;
        cmbDungeon.FormattingEnabled = true;
        cmbDungeon.Location = new System.Drawing.Point(107, 21);
        cmbDungeon.Margin = new Padding(4, 3, 4, 3);
        cmbDungeon.Name = "cmbDungeon";
        cmbDungeon.Size = new Size(188, 24);
        cmbDungeon.TabIndex = 3;
        cmbDungeon.Text = null;
        cmbDungeon.TextPadding = new Padding(2);
        // 
        // ConditionControl_InDungeon
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        Controls.Add(grpDungeon);
        Name = "ConditionControl_InDungeon";
        Size = new Size(310, 60);
        grpDungeon.ResumeLayout(false);
        grpDungeon.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private DarkUI.Controls.DarkGroupBox grpDungeon;
    private Label lblDungeon;
    private DarkUI.Controls.DarkComboBox cmbDungeon;
}
