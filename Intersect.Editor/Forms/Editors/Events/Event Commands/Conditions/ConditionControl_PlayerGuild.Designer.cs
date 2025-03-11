namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

partial class ConditionControl_PlayerGuild
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
        grpInGuild = new DarkUI.Controls.DarkGroupBox();
        lblRank = new Label();
        cmbRank = new DarkUI.Controls.DarkComboBox();
        grpInGuild.SuspendLayout();
        SuspendLayout();
        // 
        // grpInGuild
        // 
        grpInGuild.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        grpInGuild.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        grpInGuild.Controls.Add(lblRank);
        grpInGuild.Controls.Add(cmbRank);
        grpInGuild.ForeColor = System.Drawing.Color.Gainsboro;
        grpInGuild.Location = new System.Drawing.Point(0, 0);
        grpInGuild.Margin = new Padding(4, 3, 4, 3);
        grpInGuild.Name = "grpInGuild";
        grpInGuild.Padding = new Padding(4, 3, 4, 3);
        grpInGuild.Size = new Size(308, 58);
        grpInGuild.TabIndex = 34;
        grpInGuild.TabStop = false;
        grpInGuild.Text = "In Guild With At Least Rank:";
        // 
        // lblRank
        // 
        lblRank.AutoSize = true;
        lblRank.Location = new System.Drawing.Point(8, 24);
        lblRank.Margin = new Padding(4, 0, 4, 0);
        lblRank.Name = "lblRank";
        lblRank.Size = new Size(36, 15);
        lblRank.TabIndex = 5;
        lblRank.Text = "Rank:";
        // 
        // cmbRank
        // 
        cmbRank.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbRank.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbRank.BorderStyle = ButtonBorderStyle.Solid;
        cmbRank.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbRank.DrawDropdownHoverOutline = false;
        cmbRank.DrawFocusRectangle = false;
        cmbRank.DrawMode = DrawMode.OwnerDrawFixed;
        cmbRank.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbRank.FlatStyle = FlatStyle.Flat;
        cmbRank.ForeColor = System.Drawing.Color.Gainsboro;
        cmbRank.FormattingEnabled = true;
        cmbRank.Location = new System.Drawing.Point(107, 21);
        cmbRank.Margin = new Padding(4, 3, 4, 3);
        cmbRank.Name = "cmbRank";
        cmbRank.Size = new Size(188, 24);
        cmbRank.TabIndex = 3;
        cmbRank.Text = null;
        cmbRank.TextPadding = new Padding(2);
        // 
        // ConditionControl_PlayerGuild
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        Controls.Add(grpInGuild);
        Name = "ConditionControl_PlayerGuild";
        Size = new Size(310, 60);
        grpInGuild.ResumeLayout(false);
        grpInGuild.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private DarkUI.Controls.DarkGroupBox grpInGuild;
    private Label lblRank;
    private DarkUI.Controls.DarkComboBox cmbRank;
}
