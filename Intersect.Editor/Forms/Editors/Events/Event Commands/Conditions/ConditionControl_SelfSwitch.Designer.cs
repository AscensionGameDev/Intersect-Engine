namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

partial class ConditionControl_SelfSwitch
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
        grpSelfSwitch = new DarkUI.Controls.DarkGroupBox();
        cmbSelfSwitchVal = new DarkUI.Controls.DarkComboBox();
        lblSelfSwitchIs = new Label();
        cmbSelfSwitch = new DarkUI.Controls.DarkComboBox();
        lblSelfSwitch = new Label();
        grpSelfSwitch.SuspendLayout();
        SuspendLayout();
        // 
        // grpSelfSwitch
        // 
        grpSelfSwitch.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        grpSelfSwitch.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        grpSelfSwitch.Controls.Add(cmbSelfSwitchVal);
        grpSelfSwitch.Controls.Add(lblSelfSwitchIs);
        grpSelfSwitch.Controls.Add(cmbSelfSwitch);
        grpSelfSwitch.Controls.Add(lblSelfSwitch);
        grpSelfSwitch.ForeColor = System.Drawing.Color.Gainsboro;
        grpSelfSwitch.Location = new System.Drawing.Point(0, 0);
        grpSelfSwitch.Margin = new Padding(4, 3, 4, 3);
        grpSelfSwitch.Name = "grpSelfSwitch";
        grpSelfSwitch.Padding = new Padding(4, 3, 4, 3);
        grpSelfSwitch.Size = new Size(306, 103);
        grpSelfSwitch.TabIndex = 30;
        grpSelfSwitch.TabStop = false;
        grpSelfSwitch.Text = "Self Switch";
        // 
        // cmbSelfSwitchVal
        // 
        cmbSelfSwitchVal.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbSelfSwitchVal.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbSelfSwitchVal.BorderStyle = ButtonBorderStyle.Solid;
        cmbSelfSwitchVal.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbSelfSwitchVal.DrawDropdownHoverOutline = false;
        cmbSelfSwitchVal.DrawFocusRectangle = false;
        cmbSelfSwitchVal.DrawMode = DrawMode.OwnerDrawFixed;
        cmbSelfSwitchVal.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbSelfSwitchVal.FlatStyle = FlatStyle.Flat;
        cmbSelfSwitchVal.ForeColor = System.Drawing.Color.Gainsboro;
        cmbSelfSwitchVal.FormattingEnabled = true;
        cmbSelfSwitchVal.Location = new System.Drawing.Point(92, 60);
        cmbSelfSwitchVal.Margin = new Padding(4, 3, 4, 3);
        cmbSelfSwitchVal.Name = "cmbSelfSwitchVal";
        cmbSelfSwitchVal.Size = new Size(206, 24);
        cmbSelfSwitchVal.TabIndex = 3;
        cmbSelfSwitchVal.Text = null;
        cmbSelfSwitchVal.TextPadding = new Padding(2);
        // 
        // lblSelfSwitchIs
        // 
        lblSelfSwitchIs.AutoSize = true;
        lblSelfSwitchIs.Location = new System.Drawing.Point(13, 63);
        lblSelfSwitchIs.Margin = new Padding(4, 0, 4, 0);
        lblSelfSwitchIs.Name = "lblSelfSwitchIs";
        lblSelfSwitchIs.Size = new Size(21, 15);
        lblSelfSwitchIs.TabIndex = 2;
        lblSelfSwitchIs.Text = "Is: ";
        // 
        // cmbSelfSwitch
        // 
        cmbSelfSwitch.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbSelfSwitch.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbSelfSwitch.BorderStyle = ButtonBorderStyle.Solid;
        cmbSelfSwitch.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbSelfSwitch.DrawDropdownHoverOutline = false;
        cmbSelfSwitch.DrawFocusRectangle = false;
        cmbSelfSwitch.DrawMode = DrawMode.OwnerDrawFixed;
        cmbSelfSwitch.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbSelfSwitch.FlatStyle = FlatStyle.Flat;
        cmbSelfSwitch.ForeColor = System.Drawing.Color.Gainsboro;
        cmbSelfSwitch.FormattingEnabled = true;
        cmbSelfSwitch.Location = new System.Drawing.Point(92, 20);
        cmbSelfSwitch.Margin = new Padding(4, 3, 4, 3);
        cmbSelfSwitch.Name = "cmbSelfSwitch";
        cmbSelfSwitch.Size = new Size(206, 24);
        cmbSelfSwitch.TabIndex = 1;
        cmbSelfSwitch.Text = null;
        cmbSelfSwitch.TextPadding = new Padding(2);
        // 
        // lblSelfSwitch
        // 
        lblSelfSwitch.AutoSize = true;
        lblSelfSwitch.Location = new System.Drawing.Point(9, 23);
        lblSelfSwitch.Margin = new Padding(4, 0, 4, 0);
        lblSelfSwitch.Name = "lblSelfSwitch";
        lblSelfSwitch.Size = new Size(70, 15);
        lblSelfSwitch.TabIndex = 0;
        lblSelfSwitch.Text = "Self Switch: ";
        // 
        // ConditionControl_SelfSwitch
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        Controls.Add(grpSelfSwitch);
        Name = "ConditionControl_SelfSwitch";
        Size = new Size(310, 106);
        grpSelfSwitch.ResumeLayout(false);
        grpSelfSwitch.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private DarkUI.Controls.DarkGroupBox grpSelfSwitch;
    private DarkUI.Controls.DarkComboBox cmbSelfSwitchVal;
    private Label lblSelfSwitchIs;
    private DarkUI.Controls.DarkComboBox cmbSelfSwitch;
    private Label lblSelfSwitch;
}
