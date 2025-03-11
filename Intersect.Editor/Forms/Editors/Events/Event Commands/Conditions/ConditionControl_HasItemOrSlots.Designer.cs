namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

partial class ConditionControl_HasItemOrSlots
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
        grpInventoryConditions = new DarkUI.Controls.DarkGroupBox();
        chkBank = new DarkUI.Controls.DarkCheckBox();
        grpVariableAmount = new DarkUI.Controls.DarkGroupBox();
        cmbInvVariable = new DarkUI.Controls.DarkComboBox();
        lblInvVariable = new Label();
        rdoInvGuildVariable = new DarkUI.Controls.DarkRadioButton();
        rdoInvGlobalVariable = new DarkUI.Controls.DarkRadioButton();
        rdoInvPlayerVariable = new DarkUI.Controls.DarkRadioButton();
        grpManualAmount = new DarkUI.Controls.DarkGroupBox();
        nudItemAmount = new DarkUI.Controls.DarkNumericUpDown();
        lblItemQuantity = new Label();
        grpAmountType = new DarkUI.Controls.DarkGroupBox();
        rdoVariable = new DarkUI.Controls.DarkRadioButton();
        rdoManual = new DarkUI.Controls.DarkRadioButton();
        cmbItem = new DarkUI.Controls.DarkComboBox();
        lblItem = new Label();
        grpInventoryConditions.SuspendLayout();
        grpVariableAmount.SuspendLayout();
        grpManualAmount.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)nudItemAmount).BeginInit();
        grpAmountType.SuspendLayout();
        SuspendLayout();
        // 
        // grpInventoryConditions
        // 
        grpInventoryConditions.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        grpInventoryConditions.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        grpInventoryConditions.Controls.Add(chkBank);
        grpInventoryConditions.Controls.Add(grpVariableAmount);
        grpInventoryConditions.Controls.Add(grpManualAmount);
        grpInventoryConditions.Controls.Add(grpAmountType);
        grpInventoryConditions.Controls.Add(cmbItem);
        grpInventoryConditions.Controls.Add(lblItem);
        grpInventoryConditions.ForeColor = System.Drawing.Color.Gainsboro;
        grpInventoryConditions.Location = new System.Drawing.Point(0, 0);
        grpInventoryConditions.Margin = new Padding(4, 3, 4, 3);
        grpInventoryConditions.Name = "grpInventoryConditions";
        grpInventoryConditions.Padding = new Padding(4, 3, 4, 3);
        grpInventoryConditions.Size = new Size(308, 251);
        grpInventoryConditions.TabIndex = 26;
        grpInventoryConditions.TabStop = false;
        grpInventoryConditions.Text = "Has Item";
        // 
        // chkBank
        // 
        chkBank.Location = new System.Drawing.Point(7, 227);
        chkBank.Margin = new Padding(4, 3, 4, 3);
        chkBank.Name = "chkBank";
        chkBank.Size = new Size(116, 20);
        chkBank.TabIndex = 59;
        chkBank.Text = "Check Bank?";
        // 
        // grpVariableAmount
        // 
        grpVariableAmount.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        grpVariableAmount.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        grpVariableAmount.Controls.Add(cmbInvVariable);
        grpVariableAmount.Controls.Add(lblInvVariable);
        grpVariableAmount.Controls.Add(rdoInvGuildVariable);
        grpVariableAmount.Controls.Add(rdoInvGlobalVariable);
        grpVariableAmount.Controls.Add(rdoInvPlayerVariable);
        grpVariableAmount.ForeColor = System.Drawing.Color.Gainsboro;
        grpVariableAmount.Location = new System.Drawing.Point(7, 78);
        grpVariableAmount.Margin = new Padding(4, 3, 4, 3);
        grpVariableAmount.Name = "grpVariableAmount";
        grpVariableAmount.Padding = new Padding(4, 3, 4, 3);
        grpVariableAmount.Size = new Size(292, 113);
        grpVariableAmount.TabIndex = 39;
        grpVariableAmount.TabStop = false;
        grpVariableAmount.Text = "Variable Amount:";
        grpVariableAmount.Visible = false;
        // 
        // cmbInvVariable
        // 
        cmbInvVariable.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbInvVariable.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbInvVariable.BorderStyle = ButtonBorderStyle.Solid;
        cmbInvVariable.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbInvVariable.DrawDropdownHoverOutline = false;
        cmbInvVariable.DrawFocusRectangle = false;
        cmbInvVariable.DrawMode = DrawMode.OwnerDrawFixed;
        cmbInvVariable.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbInvVariable.FlatStyle = FlatStyle.Flat;
        cmbInvVariable.ForeColor = System.Drawing.Color.Gainsboro;
        cmbInvVariable.FormattingEnabled = true;
        cmbInvVariable.Location = new System.Drawing.Point(78, 80);
        cmbInvVariable.Margin = new Padding(4, 3, 4, 3);
        cmbInvVariable.Name = "cmbInvVariable";
        cmbInvVariable.Size = new Size(206, 24);
        cmbInvVariable.TabIndex = 39;
        cmbInvVariable.Text = null;
        cmbInvVariable.TextPadding = new Padding(2);
        // 
        // lblInvVariable
        // 
        lblInvVariable.AutoSize = true;
        lblInvVariable.Location = new System.Drawing.Point(10, 82);
        lblInvVariable.Margin = new Padding(4, 0, 4, 0);
        lblInvVariable.Name = "lblInvVariable";
        lblInvVariable.Size = new Size(48, 15);
        lblInvVariable.TabIndex = 38;
        lblInvVariable.Text = "Variable";
        // 
        // rdoInvGuildVariable
        // 
        rdoInvGuildVariable.AutoSize = true;
        rdoInvGuildVariable.Location = new System.Drawing.Point(8, 47);
        rdoInvGuildVariable.Margin = new Padding(4, 3, 4, 3);
        rdoInvGuildVariable.Name = "rdoInvGuildVariable";
        rdoInvGuildVariable.Size = new Size(97, 19);
        rdoInvGuildVariable.TabIndex = 37;
        rdoInvGuildVariable.Text = "Guild Variable";
        rdoInvGuildVariable.CheckedChanged += rdoInvGuildVariable_CheckedChanged;
        // 
        // rdoInvGlobalVariable
        // 
        rdoInvGlobalVariable.AutoSize = true;
        rdoInvGlobalVariable.Location = new System.Drawing.Point(174, 22);
        rdoInvGlobalVariable.Margin = new Padding(4, 3, 4, 3);
        rdoInvGlobalVariable.Name = "rdoInvGlobalVariable";
        rdoInvGlobalVariable.Size = new Size(103, 19);
        rdoInvGlobalVariable.TabIndex = 37;
        rdoInvGlobalVariable.Text = "Global Variable";
        rdoInvGlobalVariable.CheckedChanged += rdoInvGlobalVariable_CheckedChanged;
        // 
        // rdoInvPlayerVariable
        // 
        rdoInvPlayerVariable.AutoSize = true;
        rdoInvPlayerVariable.Checked = true;
        rdoInvPlayerVariable.Location = new System.Drawing.Point(8, 22);
        rdoInvPlayerVariable.Margin = new Padding(4, 3, 4, 3);
        rdoInvPlayerVariable.Name = "rdoInvPlayerVariable";
        rdoInvPlayerVariable.Size = new Size(101, 19);
        rdoInvPlayerVariable.TabIndex = 36;
        rdoInvPlayerVariable.TabStop = true;
        rdoInvPlayerVariable.Text = "Player Variable";
        rdoInvPlayerVariable.CheckedChanged += rdoInvPlayerVariable_CheckedChanged;
        // 
        // grpManualAmount
        // 
        grpManualAmount.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        grpManualAmount.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        grpManualAmount.Controls.Add(nudItemAmount);
        grpManualAmount.Controls.Add(lblItemQuantity);
        grpManualAmount.ForeColor = System.Drawing.Color.Gainsboro;
        grpManualAmount.Location = new System.Drawing.Point(7, 78);
        grpManualAmount.Margin = new Padding(4, 3, 4, 3);
        grpManualAmount.Name = "grpManualAmount";
        grpManualAmount.Padding = new Padding(4, 3, 4, 3);
        grpManualAmount.Size = new Size(292, 82);
        grpManualAmount.TabIndex = 38;
        grpManualAmount.TabStop = false;
        grpManualAmount.Text = "Manual Amount:";
        // 
        // nudItemAmount
        // 
        nudItemAmount.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        nudItemAmount.ForeColor = System.Drawing.Color.Gainsboro;
        nudItemAmount.Location = new System.Drawing.Point(100, 29);
        nudItemAmount.Margin = new Padding(4, 3, 4, 3);
        nudItemAmount.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
        nudItemAmount.Name = "nudItemAmount";
        nudItemAmount.Size = new Size(175, 23);
        nudItemAmount.TabIndex = 6;
        nudItemAmount.Value = new decimal(new int[] { 1, 0, 0, 0 });
        nudItemAmount.ValueChanged += NudItemAmount_ValueChanged;
        // 
        // lblItemQuantity
        // 
        lblItemQuantity.AutoSize = true;
        lblItemQuantity.Location = new System.Drawing.Point(17, 31);
        lblItemQuantity.Margin = new Padding(4, 0, 4, 0);
        lblItemQuantity.Name = "lblItemQuantity";
        lblItemQuantity.Size = new Size(70, 15);
        lblItemQuantity.TabIndex = 5;
        lblItemQuantity.Text = "Has at least:";
        // 
        // grpAmountType
        // 
        grpAmountType.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        grpAmountType.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        grpAmountType.Controls.Add(rdoVariable);
        grpAmountType.Controls.Add(rdoManual);
        grpAmountType.ForeColor = System.Drawing.Color.Gainsboro;
        grpAmountType.Location = new System.Drawing.Point(7, 16);
        grpAmountType.Margin = new Padding(4, 3, 4, 3);
        grpAmountType.Name = "grpAmountType";
        grpAmountType.Padding = new Padding(4, 3, 4, 3);
        grpAmountType.Size = new Size(292, 55);
        grpAmountType.TabIndex = 37;
        grpAmountType.TabStop = false;
        grpAmountType.Text = "Amount Type:";
        // 
        // rdoVariable
        // 
        rdoVariable.AutoSize = true;
        rdoVariable.Location = new System.Drawing.Point(212, 22);
        rdoVariable.Margin = new Padding(4, 3, 4, 3);
        rdoVariable.Name = "rdoVariable";
        rdoVariable.Size = new Size(66, 19);
        rdoVariable.TabIndex = 36;
        rdoVariable.Text = "Variable";
        rdoVariable.CheckedChanged += rdoVariable_CheckedChanged;
        // 
        // rdoManual
        // 
        rdoManual.AutoSize = true;
        rdoManual.Checked = true;
        rdoManual.Location = new System.Drawing.Point(11, 22);
        rdoManual.Margin = new Padding(4, 3, 4, 3);
        rdoManual.Name = "rdoManual";
        rdoManual.Size = new Size(65, 19);
        rdoManual.TabIndex = 35;
        rdoManual.TabStop = true;
        rdoManual.Text = "Manual";
        rdoManual.CheckedChanged += rdoManual_CheckedChanged;
        // 
        // cmbItem
        // 
        cmbItem.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbItem.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbItem.BorderStyle = ButtonBorderStyle.Solid;
        cmbItem.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbItem.DrawDropdownHoverOutline = false;
        cmbItem.DrawFocusRectangle = false;
        cmbItem.DrawMode = DrawMode.OwnerDrawFixed;
        cmbItem.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbItem.FlatStyle = FlatStyle.Flat;
        cmbItem.ForeColor = System.Drawing.Color.Gainsboro;
        cmbItem.FormattingEnabled = true;
        cmbItem.Location = new System.Drawing.Point(51, 197);
        cmbItem.Margin = new Padding(4, 3, 4, 3);
        cmbItem.Name = "cmbItem";
        cmbItem.Size = new Size(247, 24);
        cmbItem.TabIndex = 3;
        cmbItem.Text = null;
        cmbItem.TextPadding = new Padding(2);
        // 
        // lblItem
        // 
        lblItem.AutoSize = true;
        lblItem.Location = new System.Drawing.Point(10, 199);
        lblItem.Margin = new Padding(4, 0, 4, 0);
        lblItem.Name = "lblItem";
        lblItem.Size = new Size(34, 15);
        lblItem.TabIndex = 2;
        lblItem.Text = "Item:";
        // 
        // ConditionControl_HasItemOrSlots
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        Controls.Add(grpInventoryConditions);
        Name = "ConditionControl_HasItemOrSlots";
        Size = new Size(310, 256);
        grpInventoryConditions.ResumeLayout(false);
        grpInventoryConditions.PerformLayout();
        grpVariableAmount.ResumeLayout(false);
        grpVariableAmount.PerformLayout();
        grpManualAmount.ResumeLayout(false);
        grpManualAmount.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)nudItemAmount).EndInit();
        grpAmountType.ResumeLayout(false);
        grpAmountType.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private DarkUI.Controls.DarkGroupBox grpInventoryConditions;
    private DarkUI.Controls.DarkCheckBox chkBank;
    private DarkUI.Controls.DarkGroupBox grpVariableAmount;
    private DarkUI.Controls.DarkComboBox cmbInvVariable;
    private Label lblInvVariable;
    private DarkUI.Controls.DarkRadioButton rdoInvGuildVariable;
    private DarkUI.Controls.DarkRadioButton rdoInvGlobalVariable;
    private DarkUI.Controls.DarkRadioButton rdoInvPlayerVariable;
    private DarkUI.Controls.DarkGroupBox grpManualAmount;
    private DarkUI.Controls.DarkNumericUpDown nudItemAmount;
    private Label lblItemQuantity;
    private DarkUI.Controls.DarkGroupBox grpAmountType;
    private DarkUI.Controls.DarkRadioButton rdoVariable;
    private DarkUI.Controls.DarkRadioButton rdoManual;
    private DarkUI.Controls.DarkComboBox cmbItem;
    private Label lblItem;
}
