namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

partial class ConditionControl_NPC
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
        grpNpc = new DarkUI.Controls.DarkGroupBox();
        chkNpc = new DarkUI.Controls.DarkCheckBox();
        cmbNpcs = new DarkUI.Controls.DarkComboBox();
        lblNpc = new Label();
        grpNpc.SuspendLayout();
        SuspendLayout();
        // 
        // grpNpc
        // 
        grpNpc.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        grpNpc.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        grpNpc.Controls.Add(chkNpc);
        grpNpc.Controls.Add(cmbNpcs);
        grpNpc.Controls.Add(lblNpc);
        grpNpc.ForeColor = System.Drawing.Color.Gainsboro;
        grpNpc.Location = new System.Drawing.Point(0, 0);
        grpNpc.Margin = new Padding(4, 3, 4, 3);
        grpNpc.Name = "grpNpc";
        grpNpc.Padding = new Padding(4, 3, 4, 3);
        grpNpc.Size = new Size(308, 85);
        grpNpc.TabIndex = 41;
        grpNpc.TabStop = false;
        grpNpc.Text = "NPCs";
        // 
        // chkNpc
        // 
        chkNpc.Location = new System.Drawing.Point(8, 22);
        chkNpc.Margin = new Padding(4, 3, 4, 3);
        chkNpc.Name = "chkNpc";
        chkNpc.Size = new Size(114, 20);
        chkNpc.TabIndex = 60;
        chkNpc.Text = "Specify NPC?";
        chkNpc.CheckedChanged += chkNpc_CheckedChanged;
        // 
        // cmbNpcs
        // 
        cmbNpcs.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbNpcs.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbNpcs.BorderStyle = ButtonBorderStyle.Solid;
        cmbNpcs.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbNpcs.DrawDropdownHoverOutline = false;
        cmbNpcs.DrawFocusRectangle = false;
        cmbNpcs.DrawMode = DrawMode.OwnerDrawFixed;
        cmbNpcs.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbNpcs.FlatStyle = FlatStyle.Flat;
        cmbNpcs.ForeColor = System.Drawing.Color.Gainsboro;
        cmbNpcs.FormattingEnabled = true;
        cmbNpcs.Location = new System.Drawing.Point(50, 52);
        cmbNpcs.Margin = new Padding(4, 3, 4, 3);
        cmbNpcs.Name = "cmbNpcs";
        cmbNpcs.Size = new Size(245, 24);
        cmbNpcs.TabIndex = 39;
        cmbNpcs.Text = null;
        cmbNpcs.TextPadding = new Padding(2);
        // 
        // lblNpc
        // 
        lblNpc.AutoSize = true;
        lblNpc.Location = new System.Drawing.Point(8, 57);
        lblNpc.Margin = new Padding(4, 0, 4, 0);
        lblNpc.Name = "lblNpc";
        lblNpc.Size = new Size(34, 15);
        lblNpc.TabIndex = 38;
        lblNpc.Text = "NPC:";
        // 
        // ConditionControl_NPC
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        Controls.Add(grpNpc);
        Name = "ConditionControl_NPC";
        Size = new Size(310, 89);
        grpNpc.ResumeLayout(false);
        grpNpc.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private DarkUI.Controls.DarkGroupBox grpNpc;
    private DarkUI.Controls.DarkCheckBox chkNpc;
    private DarkUI.Controls.DarkComboBox cmbNpcs;
    private Label lblNpc;
}
