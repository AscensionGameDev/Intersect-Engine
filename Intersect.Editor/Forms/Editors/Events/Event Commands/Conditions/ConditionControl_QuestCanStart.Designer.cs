namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

partial class ConditionControl_QuestCanStart
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
        grpStartQuest = new DarkUI.Controls.DarkGroupBox();
        lblStartQuest = new Label();
        cmbStartQuest = new DarkUI.Controls.DarkComboBox();
        grpStartQuest.SuspendLayout();
        SuspendLayout();
        // 
        // grpStartQuest
        // 
        grpStartQuest.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        grpStartQuest.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        grpStartQuest.Controls.Add(lblStartQuest);
        grpStartQuest.Controls.Add(cmbStartQuest);
        grpStartQuest.ForeColor = System.Drawing.Color.Gainsboro;
        grpStartQuest.Location = new System.Drawing.Point(0, 0);
        grpStartQuest.Margin = new Padding(4, 3, 4, 3);
        grpStartQuest.Name = "grpStartQuest";
        grpStartQuest.Padding = new Padding(4, 3, 4, 3);
        grpStartQuest.Size = new Size(308, 58);
        grpStartQuest.TabIndex = 32;
        grpStartQuest.TabStop = false;
        grpStartQuest.Text = "Can Start Quest:";
        // 
        // lblStartQuest
        // 
        lblStartQuest.AutoSize = true;
        lblStartQuest.Location = new System.Drawing.Point(8, 24);
        lblStartQuest.Margin = new Padding(4, 0, 4, 0);
        lblStartQuest.Name = "lblStartQuest";
        lblStartQuest.Size = new Size(41, 15);
        lblStartQuest.TabIndex = 5;
        lblStartQuest.Text = "Quest:";
        // 
        // cmbStartQuest
        // 
        cmbStartQuest.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbStartQuest.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbStartQuest.BorderStyle = ButtonBorderStyle.Solid;
        cmbStartQuest.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbStartQuest.DrawDropdownHoverOutline = false;
        cmbStartQuest.DrawFocusRectangle = false;
        cmbStartQuest.DrawMode = DrawMode.OwnerDrawFixed;
        cmbStartQuest.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbStartQuest.FlatStyle = FlatStyle.Flat;
        cmbStartQuest.ForeColor = System.Drawing.Color.Gainsboro;
        cmbStartQuest.FormattingEnabled = true;
        cmbStartQuest.Location = new System.Drawing.Point(107, 21);
        cmbStartQuest.Margin = new Padding(4, 3, 4, 3);
        cmbStartQuest.Name = "cmbStartQuest";
        cmbStartQuest.Size = new Size(188, 24);
        cmbStartQuest.TabIndex = 3;
        cmbStartQuest.Text = null;
        cmbStartQuest.TextPadding = new Padding(2);
        // 
        // ConditionControl_QuestCanStart
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        Controls.Add(grpStartQuest);
        Name = "ConditionControl_QuestCanStart";
        Size = new Size(309, 58);
        grpStartQuest.ResumeLayout(false);
        grpStartQuest.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private DarkUI.Controls.DarkGroupBox grpStartQuest;
    private Label lblStartQuest;
    private DarkUI.Controls.DarkComboBox cmbStartQuest;
}
