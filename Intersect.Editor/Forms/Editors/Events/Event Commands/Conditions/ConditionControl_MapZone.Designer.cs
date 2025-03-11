namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

partial class ConditionControl_MapZone
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
        grpMapZoneType = new DarkUI.Controls.DarkGroupBox();
        lblMapZoneType = new Label();
        cmbMapZoneType = new DarkUI.Controls.DarkComboBox();
        grpMapZoneType.SuspendLayout();
        SuspendLayout();
        // 
        // grpMapZoneType
        // 
        grpMapZoneType.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        grpMapZoneType.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        grpMapZoneType.Controls.Add(lblMapZoneType);
        grpMapZoneType.Controls.Add(cmbMapZoneType);
        grpMapZoneType.ForeColor = System.Drawing.Color.Gainsboro;
        grpMapZoneType.Location = new System.Drawing.Point(0, 0);
        grpMapZoneType.Margin = new Padding(4, 3, 4, 3);
        grpMapZoneType.Name = "grpMapZoneType";
        grpMapZoneType.Padding = new Padding(4, 3, 4, 3);
        grpMapZoneType.Size = new Size(308, 58);
        grpMapZoneType.TabIndex = 59;
        grpMapZoneType.TabStop = false;
        grpMapZoneType.Text = "Map Zone Type Is:";
        // 
        // lblMapZoneType
        // 
        lblMapZoneType.AutoSize = true;
        lblMapZoneType.Location = new System.Drawing.Point(8, 24);
        lblMapZoneType.Margin = new Padding(4, 0, 4, 0);
        lblMapZoneType.Name = "lblMapZoneType";
        lblMapZoneType.Size = new Size(91, 15);
        lblMapZoneType.TabIndex = 5;
        lblMapZoneType.Text = "Map Zone Type:";
        // 
        // cmbMapZoneType
        // 
        cmbMapZoneType.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbMapZoneType.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbMapZoneType.BorderStyle = ButtonBorderStyle.Solid;
        cmbMapZoneType.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbMapZoneType.DrawDropdownHoverOutline = false;
        cmbMapZoneType.DrawFocusRectangle = false;
        cmbMapZoneType.DrawMode = DrawMode.OwnerDrawFixed;
        cmbMapZoneType.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbMapZoneType.FlatStyle = FlatStyle.Flat;
        cmbMapZoneType.ForeColor = System.Drawing.Color.Gainsboro;
        cmbMapZoneType.FormattingEnabled = true;
        cmbMapZoneType.Location = new System.Drawing.Point(107, 21);
        cmbMapZoneType.Margin = new Padding(4, 3, 4, 3);
        cmbMapZoneType.Name = "cmbMapZoneType";
        cmbMapZoneType.Size = new Size(188, 24);
        cmbMapZoneType.TabIndex = 3;
        cmbMapZoneType.Text = null;
        cmbMapZoneType.TextPadding = new Padding(2);
        // 
        // ConditionControl_MapZone
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        Controls.Add(grpMapZoneType);
        Name = "ConditionControl_MapZone";
        Size = new Size(310, 60);
        grpMapZoneType.ResumeLayout(false);
        grpMapZoneType.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private DarkUI.Controls.DarkGroupBox grpMapZoneType;
    private Label lblMapZoneType;
    private DarkUI.Controls.DarkComboBox cmbMapZoneType;
}
