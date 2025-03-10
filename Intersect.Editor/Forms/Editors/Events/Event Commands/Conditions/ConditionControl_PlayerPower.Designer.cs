namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

partial class ConditionControl_PlayerPower
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
        grpPowerIs = new DarkUI.Controls.DarkGroupBox();
        cmbPower = new DarkUI.Controls.DarkComboBox();
        lblPower = new Label();
        grpPowerIs.SuspendLayout();
        SuspendLayout();
        // 
        // grpPowerIs
        // 
        grpPowerIs.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        grpPowerIs.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        grpPowerIs.Controls.Add(cmbPower);
        grpPowerIs.Controls.Add(lblPower);
        grpPowerIs.ForeColor = System.Drawing.Color.Gainsboro;
        grpPowerIs.Location = new System.Drawing.Point(0, 0);
        grpPowerIs.Margin = new Padding(4, 3, 4, 3);
        grpPowerIs.Name = "grpPowerIs";
        grpPowerIs.Padding = new Padding(4, 3, 4, 3);
        grpPowerIs.Size = new Size(306, 59);
        grpPowerIs.TabIndex = 26;
        grpPowerIs.TabStop = false;
        grpPowerIs.Text = "Power Is...";
        // 
        // cmbPower
        // 
        cmbPower.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbPower.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbPower.BorderStyle = ButtonBorderStyle.Solid;
        cmbPower.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbPower.DrawDropdownHoverOutline = false;
        cmbPower.DrawFocusRectangle = false;
        cmbPower.DrawMode = DrawMode.OwnerDrawFixed;
        cmbPower.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbPower.FlatStyle = FlatStyle.Flat;
        cmbPower.ForeColor = System.Drawing.Color.Gainsboro;
        cmbPower.FormattingEnabled = true;
        cmbPower.Location = new System.Drawing.Point(92, 20);
        cmbPower.Margin = new Padding(4, 3, 4, 3);
        cmbPower.Name = "cmbPower";
        cmbPower.Size = new Size(204, 24);
        cmbPower.TabIndex = 1;
        cmbPower.Text = null;
        cmbPower.TextPadding = new Padding(2);
        // 
        // lblPower
        // 
        lblPower.AutoSize = true;
        lblPower.Location = new System.Drawing.Point(9, 23);
        lblPower.Margin = new Padding(4, 0, 4, 0);
        lblPower.Name = "lblPower";
        lblPower.Size = new Size(43, 15);
        lblPower.TabIndex = 0;
        lblPower.Text = "Power:";
        // 
        // ConditionControl_PowerIs
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        Controls.Add(grpPowerIs);
        Name = "ConditionControl_PowerIs";
        Size = new Size(309, 63);
        grpPowerIs.ResumeLayout(false);
        grpPowerIs.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private DarkUI.Controls.DarkGroupBox grpPowerIs;
    private DarkUI.Controls.DarkComboBox cmbPower;
    private Label lblPower;
}
