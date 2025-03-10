namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

partial class ConditionControl_QuestInProgress
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
        grpQuestInProgress = new DarkUI.Controls.DarkGroupBox();
        lblQuestTask = new Label();
        cmbQuestTask = new DarkUI.Controls.DarkComboBox();
        cmbTaskModifier = new DarkUI.Controls.DarkComboBox();
        lblQuestIs = new Label();
        lblQuestProgress = new Label();
        cmbQuestInProgress = new DarkUI.Controls.DarkComboBox();
        grpQuestInProgress.SuspendLayout();
        SuspendLayout();
        // 
        // grpQuestInProgress
        // 
        grpQuestInProgress.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        grpQuestInProgress.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        grpQuestInProgress.Controls.Add(lblQuestTask);
        grpQuestInProgress.Controls.Add(cmbQuestTask);
        grpQuestInProgress.Controls.Add(cmbTaskModifier);
        grpQuestInProgress.Controls.Add(lblQuestIs);
        grpQuestInProgress.Controls.Add(lblQuestProgress);
        grpQuestInProgress.Controls.Add(cmbQuestInProgress);
        grpQuestInProgress.ForeColor = System.Drawing.Color.Gainsboro;
        grpQuestInProgress.Location = new System.Drawing.Point(0, 0);
        grpQuestInProgress.Margin = new Padding(4, 3, 4, 3);
        grpQuestInProgress.Name = "grpQuestInProgress";
        grpQuestInProgress.Padding = new Padding(4, 3, 4, 3);
        grpQuestInProgress.Size = new Size(307, 141);
        grpQuestInProgress.TabIndex = 33;
        grpQuestInProgress.TabStop = false;
        grpQuestInProgress.Text = "Quest In Progress:";
        grpQuestInProgress.Visible = false;
        // 
        // lblQuestTask
        // 
        lblQuestTask.AutoSize = true;
        lblQuestTask.Location = new System.Drawing.Point(8, 99);
        lblQuestTask.Margin = new Padding(4, 0, 4, 0);
        lblQuestTask.Name = "lblQuestTask";
        lblQuestTask.Size = new Size(32, 15);
        lblQuestTask.TabIndex = 9;
        lblQuestTask.Text = "Task:";
        // 
        // cmbQuestTask
        // 
        cmbQuestTask.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbQuestTask.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbQuestTask.BorderStyle = ButtonBorderStyle.Solid;
        cmbQuestTask.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbQuestTask.DrawDropdownHoverOutline = false;
        cmbQuestTask.DrawFocusRectangle = false;
        cmbQuestTask.DrawMode = DrawMode.OwnerDrawFixed;
        cmbQuestTask.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbQuestTask.Enabled = false;
        cmbQuestTask.FlatStyle = FlatStyle.Flat;
        cmbQuestTask.ForeColor = System.Drawing.Color.Gainsboro;
        cmbQuestTask.FormattingEnabled = true;
        cmbQuestTask.Location = new System.Drawing.Point(107, 96);
        cmbQuestTask.Margin = new Padding(4, 3, 4, 3);
        cmbQuestTask.Name = "cmbQuestTask";
        cmbQuestTask.Size = new Size(190, 24);
        cmbQuestTask.TabIndex = 8;
        cmbQuestTask.Text = null;
        cmbQuestTask.TextPadding = new Padding(2);
        // 
        // cmbTaskModifier
        // 
        cmbTaskModifier.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbTaskModifier.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbTaskModifier.BorderStyle = ButtonBorderStyle.Solid;
        cmbTaskModifier.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbTaskModifier.DrawDropdownHoverOutline = false;
        cmbTaskModifier.DrawFocusRectangle = false;
        cmbTaskModifier.DrawMode = DrawMode.OwnerDrawFixed;
        cmbTaskModifier.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbTaskModifier.FlatStyle = FlatStyle.Flat;
        cmbTaskModifier.ForeColor = System.Drawing.Color.Gainsboro;
        cmbTaskModifier.FormattingEnabled = true;
        cmbTaskModifier.Location = new System.Drawing.Point(107, 58);
        cmbTaskModifier.Margin = new Padding(4, 3, 4, 3);
        cmbTaskModifier.Name = "cmbTaskModifier";
        cmbTaskModifier.Size = new Size(190, 24);
        cmbTaskModifier.TabIndex = 7;
        cmbTaskModifier.Text = null;
        cmbTaskModifier.TextPadding = new Padding(2);
        // 
        // lblQuestIs
        // 
        lblQuestIs.AutoSize = true;
        lblQuestIs.Location = new System.Drawing.Point(8, 60);
        lblQuestIs.Margin = new Padding(4, 0, 4, 0);
        lblQuestIs.Name = "lblQuestIs";
        lblQuestIs.Size = new Size(18, 15);
        lblQuestIs.TabIndex = 6;
        lblQuestIs.Text = "Is:";
        // 
        // lblQuestProgress
        // 
        lblQuestProgress.AutoSize = true;
        lblQuestProgress.Location = new System.Drawing.Point(8, 24);
        lblQuestProgress.Margin = new Padding(4, 0, 4, 0);
        lblQuestProgress.Name = "lblQuestProgress";
        lblQuestProgress.Size = new Size(41, 15);
        lblQuestProgress.TabIndex = 5;
        lblQuestProgress.Text = "Quest:";
        // 
        // cmbQuestInProgress
        // 
        cmbQuestInProgress.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbQuestInProgress.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbQuestInProgress.BorderStyle = ButtonBorderStyle.Solid;
        cmbQuestInProgress.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbQuestInProgress.DrawDropdownHoverOutline = false;
        cmbQuestInProgress.DrawFocusRectangle = false;
        cmbQuestInProgress.DrawMode = DrawMode.OwnerDrawFixed;
        cmbQuestInProgress.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbQuestInProgress.FlatStyle = FlatStyle.Flat;
        cmbQuestInProgress.ForeColor = System.Drawing.Color.Gainsboro;
        cmbQuestInProgress.FormattingEnabled = true;
        cmbQuestInProgress.Location = new System.Drawing.Point(107, 21);
        cmbQuestInProgress.Margin = new Padding(4, 3, 4, 3);
        cmbQuestInProgress.Name = "cmbQuestInProgress";
        cmbQuestInProgress.Size = new Size(190, 24);
        cmbQuestInProgress.TabIndex = 3;
        cmbQuestInProgress.Text = null;
        cmbQuestInProgress.TextPadding = new Padding(2);
        // 
        // ConditionControl_QuestInProgress
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        Controls.Add(grpQuestInProgress);
        Name = "ConditionControl_QuestInProgress";
        Size = new Size(310, 144);
        grpQuestInProgress.ResumeLayout(false);
        grpQuestInProgress.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private DarkUI.Controls.DarkGroupBox grpQuestInProgress;
    private Label lblQuestTask;
    private DarkUI.Controls.DarkComboBox cmbQuestTask;
    private DarkUI.Controls.DarkComboBox cmbTaskModifier;
    private Label lblQuestIs;
    private Label lblQuestProgress;
    private DarkUI.Controls.DarkComboBox cmbQuestInProgress;
}
