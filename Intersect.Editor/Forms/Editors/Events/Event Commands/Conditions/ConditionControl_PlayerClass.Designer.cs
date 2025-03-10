namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

partial class ConditionControl_PlayerClass
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
        grpClass = new DarkUI.Controls.DarkGroupBox();
        cmbClass = new DarkUI.Controls.DarkComboBox();
        lblClass = new Label();
        grpClass.SuspendLayout();
        SuspendLayout();
        // 
        // grpClass
        // 
        grpClass.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        grpClass.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        grpClass.Controls.Add(cmbClass);
        grpClass.Controls.Add(lblClass);
        grpClass.ForeColor = System.Drawing.Color.Gainsboro;
        grpClass.Location = new System.Drawing.Point(0, 0);
        grpClass.Margin = new Padding(4, 3, 4, 3);
        grpClass.Name = "grpClass";
        grpClass.Padding = new Padding(4, 3, 4, 3);
        grpClass.Size = new Size(306, 60);
        grpClass.TabIndex = 28;
        grpClass.TabStop = false;
        grpClass.Text = "Class is";
        // 
        // cmbClass
        // 
        cmbClass.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbClass.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbClass.BorderStyle = ButtonBorderStyle.Solid;
        cmbClass.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbClass.DrawDropdownHoverOutline = false;
        cmbClass.DrawFocusRectangle = false;
        cmbClass.DrawMode = DrawMode.OwnerDrawFixed;
        cmbClass.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbClass.FlatStyle = FlatStyle.Flat;
        cmbClass.ForeColor = System.Drawing.Color.Gainsboro;
        cmbClass.FormattingEnabled = true;
        cmbClass.Location = new System.Drawing.Point(92, 21);
        cmbClass.Margin = new Padding(4, 3, 4, 3);
        cmbClass.Name = "cmbClass";
        cmbClass.Size = new Size(204, 24);
        cmbClass.TabIndex = 3;
        cmbClass.Text = null;
        cmbClass.TextPadding = new Padding(2);
        // 
        // lblClass
        // 
        lblClass.AutoSize = true;
        lblClass.Location = new System.Drawing.Point(9, 23);
        lblClass.Margin = new Padding(4, 0, 4, 0);
        lblClass.Name = "lblClass";
        lblClass.Size = new Size(37, 15);
        lblClass.TabIndex = 2;
        lblClass.Text = "Class:";
        // 
        // ConditionControl_PlayerClass
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        Controls.Add(grpClass);
        Name = "ConditionControl_PlayerClass";
        Size = new Size(308, 62);
        grpClass.ResumeLayout(false);
        grpClass.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private DarkUI.Controls.DarkGroupBox grpClass;
    private DarkUI.Controls.DarkComboBox cmbClass;
    private Label lblClass;
}
