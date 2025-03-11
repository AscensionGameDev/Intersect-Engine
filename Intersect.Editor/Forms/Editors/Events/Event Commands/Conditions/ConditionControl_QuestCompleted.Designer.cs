namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

partial class ConditionControl_QuestCompleted
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
        grpQuestCompleted = new DarkUI.Controls.DarkGroupBox();
        lblQuestCompleted = new Label();
        cmbCompletedQuest = new DarkUI.Controls.DarkComboBox();
        grpQuestCompleted.SuspendLayout();
        SuspendLayout();
        // 
        // grpQuestCompleted
        // 
        grpQuestCompleted.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        grpQuestCompleted.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        grpQuestCompleted.Controls.Add(lblQuestCompleted);
        grpQuestCompleted.Controls.Add(cmbCompletedQuest);
        grpQuestCompleted.ForeColor = System.Drawing.Color.Gainsboro;
        grpQuestCompleted.Location = new System.Drawing.Point(0, 0);
        grpQuestCompleted.Margin = new Padding(4, 3, 4, 3);
        grpQuestCompleted.Name = "grpQuestCompleted";
        grpQuestCompleted.Padding = new Padding(4, 3, 4, 3);
        grpQuestCompleted.Size = new Size(308, 58);
        grpQuestCompleted.TabIndex = 33;
        grpQuestCompleted.TabStop = false;
        grpQuestCompleted.Text = "Quest Completed:";
        // 
        // lblQuestCompleted
        // 
        lblQuestCompleted.AutoSize = true;
        lblQuestCompleted.Location = new System.Drawing.Point(8, 24);
        lblQuestCompleted.Margin = new Padding(4, 0, 4, 0);
        lblQuestCompleted.Name = "lblQuestCompleted";
        lblQuestCompleted.Size = new Size(41, 15);
        lblQuestCompleted.TabIndex = 5;
        lblQuestCompleted.Text = "Quest:";
        // 
        // cmbCompletedQuest
        // 
        cmbCompletedQuest.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbCompletedQuest.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbCompletedQuest.BorderStyle = ButtonBorderStyle.Solid;
        cmbCompletedQuest.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbCompletedQuest.DrawDropdownHoverOutline = false;
        cmbCompletedQuest.DrawFocusRectangle = false;
        cmbCompletedQuest.DrawMode = DrawMode.OwnerDrawFixed;
        cmbCompletedQuest.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbCompletedQuest.FlatStyle = FlatStyle.Flat;
        cmbCompletedQuest.ForeColor = System.Drawing.Color.Gainsboro;
        cmbCompletedQuest.FormattingEnabled = true;
        cmbCompletedQuest.Location = new System.Drawing.Point(107, 21);
        cmbCompletedQuest.Margin = new Padding(4, 3, 4, 3);
        cmbCompletedQuest.Name = "cmbCompletedQuest";
        cmbCompletedQuest.Size = new Size(188, 24);
        cmbCompletedQuest.TabIndex = 3;
        cmbCompletedQuest.Text = null;
        cmbCompletedQuest.TextPadding = new Padding(2);
        // 
        // ConditionControl_QuestCompleted
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        Controls.Add(grpQuestCompleted);
        Name = "ConditionControl_QuestCompleted";
        Size = new Size(310, 60);
        grpQuestCompleted.ResumeLayout(false);
        grpQuestCompleted.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private DarkUI.Controls.DarkGroupBox grpQuestCompleted;
    private Label lblQuestCompleted;
    private DarkUI.Controls.DarkComboBox cmbCompletedQuest;
}
