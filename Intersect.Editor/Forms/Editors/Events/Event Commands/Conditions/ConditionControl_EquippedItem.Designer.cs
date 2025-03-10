namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

partial class ConditionControl_EquippedItem
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
        grpEquippedItem = new DarkUI.Controls.DarkGroupBox();
        cmbEquippedItem = new DarkUI.Controls.DarkComboBox();
        lblEquippedItem = new Label();
        grpEquippedItem.SuspendLayout();
        SuspendLayout();
        // 
        // grpEquippedItem
        // 
        grpEquippedItem.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        grpEquippedItem.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        grpEquippedItem.Controls.Add(cmbEquippedItem);
        grpEquippedItem.Controls.Add(lblEquippedItem);
        grpEquippedItem.ForeColor = System.Drawing.Color.Gainsboro;
        grpEquippedItem.Location = new System.Drawing.Point(0, 0);
        grpEquippedItem.Margin = new Padding(4, 3, 4, 3);
        grpEquippedItem.Name = "grpEquippedItem";
        grpEquippedItem.Padding = new Padding(4, 3, 4, 3);
        grpEquippedItem.Size = new Size(306, 53);
        grpEquippedItem.TabIndex = 27;
        grpEquippedItem.TabStop = false;
        grpEquippedItem.Text = "Has Equipped Item";
        // 
        // cmbEquippedItem
        // 
        cmbEquippedItem.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbEquippedItem.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbEquippedItem.BorderStyle = ButtonBorderStyle.Solid;
        cmbEquippedItem.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbEquippedItem.DrawDropdownHoverOutline = false;
        cmbEquippedItem.DrawFocusRectangle = false;
        cmbEquippedItem.DrawMode = DrawMode.OwnerDrawFixed;
        cmbEquippedItem.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbEquippedItem.FlatStyle = FlatStyle.Flat;
        cmbEquippedItem.ForeColor = System.Drawing.Color.Gainsboro;
        cmbEquippedItem.FormattingEnabled = true;
        cmbEquippedItem.Location = new System.Drawing.Point(50, 19);
        cmbEquippedItem.Margin = new Padding(4, 3, 4, 3);
        cmbEquippedItem.Name = "cmbEquippedItem";
        cmbEquippedItem.Size = new Size(248, 24);
        cmbEquippedItem.TabIndex = 3;
        cmbEquippedItem.Text = null;
        cmbEquippedItem.TextPadding = new Padding(2);
        // 
        // lblEquippedItem
        // 
        lblEquippedItem.AutoSize = true;
        lblEquippedItem.Location = new System.Drawing.Point(8, 22);
        lblEquippedItem.Margin = new Padding(4, 0, 4, 0);
        lblEquippedItem.Name = "lblEquippedItem";
        lblEquippedItem.Size = new Size(34, 15);
        lblEquippedItem.TabIndex = 2;
        lblEquippedItem.Text = "Item:";
        // 
        // ConditionControl_EquippedItem
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        Controls.Add(grpEquippedItem);
        Name = "ConditionControl_EquippedItem";
        Size = new Size(307, 55);
        grpEquippedItem.ResumeLayout(false);
        grpEquippedItem.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private DarkUI.Controls.DarkGroupBox grpEquippedItem;
    private DarkUI.Controls.DarkComboBox cmbEquippedItem;
    private Label lblEquippedItem;
}
