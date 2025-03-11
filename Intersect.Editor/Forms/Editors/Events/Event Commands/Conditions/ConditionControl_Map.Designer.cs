namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

partial class ConditionControl_Map
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
        grpMapIs = new DarkUI.Controls.DarkGroupBox();
        btnSelectMap = new DarkUI.Controls.DarkButton();
        grpMapIs.SuspendLayout();
        SuspendLayout();
        // 
        // grpMapIs
        // 
        grpMapIs.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        grpMapIs.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        grpMapIs.Controls.Add(btnSelectMap);
        grpMapIs.ForeColor = System.Drawing.Color.Gainsboro;
        grpMapIs.Location = new System.Drawing.Point(0, 0);
        grpMapIs.Margin = new Padding(4, 3, 4, 3);
        grpMapIs.Name = "grpMapIs";
        grpMapIs.Padding = new Padding(4, 3, 4, 3);
        grpMapIs.Size = new Size(308, 58);
        grpMapIs.TabIndex = 36;
        grpMapIs.TabStop = false;
        grpMapIs.Text = "Map Is...";
        // 
        // btnSelectMap
        // 
        btnSelectMap.Location = new System.Drawing.Point(15, 22);
        btnSelectMap.Margin = new Padding(4, 3, 4, 3);
        btnSelectMap.Name = "btnSelectMap";
        btnSelectMap.Padding = new Padding(6);
        btnSelectMap.Size = new Size(285, 27);
        btnSelectMap.TabIndex = 21;
        btnSelectMap.Text = "Select Map";
        btnSelectMap.Click += btnSelectMap_Click;
        // 
        // ConditionControl_Map
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        Controls.Add(grpMapIs);
        Name = "ConditionControl_Map";
        Size = new Size(310, 60);
        grpMapIs.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private DarkUI.Controls.DarkGroupBox grpMapIs;
    private DarkUI.Controls.DarkButton btnSelectMap;
}
