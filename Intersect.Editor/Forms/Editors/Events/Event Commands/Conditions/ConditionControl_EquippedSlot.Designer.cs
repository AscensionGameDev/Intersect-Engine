namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

partial class ConditionControl_EquippedSlot
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
        grpCheckEquippedSlot = new DarkUI.Controls.DarkGroupBox();
        cmbCheckEquippedSlot = new DarkUI.Controls.DarkComboBox();
        lblCheckEquippedSlot = new Label();
        grpCheckEquippedSlot.SuspendLayout();
        SuspendLayout();
        // 
        // grpCheckEquippedSlot
        // 
        grpCheckEquippedSlot.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        grpCheckEquippedSlot.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        grpCheckEquippedSlot.Controls.Add(cmbCheckEquippedSlot);
        grpCheckEquippedSlot.Controls.Add(lblCheckEquippedSlot);
        grpCheckEquippedSlot.ForeColor = System.Drawing.Color.Gainsboro;
        grpCheckEquippedSlot.Location = new System.Drawing.Point(0, 0);
        grpCheckEquippedSlot.Margin = new Padding(4, 3, 4, 3);
        grpCheckEquippedSlot.Name = "grpCheckEquippedSlot";
        grpCheckEquippedSlot.Padding = new Padding(4, 3, 4, 3);
        grpCheckEquippedSlot.Size = new Size(308, 58);
        grpCheckEquippedSlot.TabIndex = 28;
        grpCheckEquippedSlot.TabStop = false;
        grpCheckEquippedSlot.Text = "Check Equipped Slot:";
        // 
        // cmbCheckEquippedSlot
        // 
        cmbCheckEquippedSlot.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbCheckEquippedSlot.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbCheckEquippedSlot.BorderStyle = ButtonBorderStyle.Solid;
        cmbCheckEquippedSlot.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbCheckEquippedSlot.DrawDropdownHoverOutline = false;
        cmbCheckEquippedSlot.DrawFocusRectangle = false;
        cmbCheckEquippedSlot.DrawMode = DrawMode.OwnerDrawFixed;
        cmbCheckEquippedSlot.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbCheckEquippedSlot.FlatStyle = FlatStyle.Flat;
        cmbCheckEquippedSlot.ForeColor = System.Drawing.Color.Gainsboro;
        cmbCheckEquippedSlot.FormattingEnabled = true;
        cmbCheckEquippedSlot.Location = new System.Drawing.Point(48, 22);
        cmbCheckEquippedSlot.Margin = new Padding(4, 3, 4, 3);
        cmbCheckEquippedSlot.Name = "cmbCheckEquippedSlot";
        cmbCheckEquippedSlot.Size = new Size(252, 24);
        cmbCheckEquippedSlot.TabIndex = 3;
        cmbCheckEquippedSlot.Text = null;
        cmbCheckEquippedSlot.TextPadding = new Padding(2);
        // 
        // lblCheckEquippedSlot
        // 
        lblCheckEquippedSlot.AutoSize = true;
        lblCheckEquippedSlot.Location = new System.Drawing.Point(8, 25);
        lblCheckEquippedSlot.Margin = new Padding(4, 0, 4, 0);
        lblCheckEquippedSlot.Name = "lblCheckEquippedSlot";
        lblCheckEquippedSlot.Size = new Size(30, 15);
        lblCheckEquippedSlot.TabIndex = 2;
        lblCheckEquippedSlot.Text = "Slot:";
        // 
        // ConditionControl_EquippedSlot
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        Controls.Add(grpCheckEquippedSlot);
        Name = "ConditionControl_EquippedSlot";
        Size = new Size(310, 60);
        grpCheckEquippedSlot.ResumeLayout(false);
        grpCheckEquippedSlot.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private DarkUI.Controls.DarkGroupBox grpCheckEquippedSlot;
    private DarkUI.Controls.DarkComboBox cmbCheckEquippedSlot;
    private Label lblCheckEquippedSlot;
}
