namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

partial class ConditionControl_Variable
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
        grpVariable = new DarkUI.Controls.DarkGroupBox();
        grpSelectVariable = new DarkUI.Controls.DarkGroupBox();
        rdoUserVariable = new DarkUI.Controls.DarkRadioButton();
        rdoPlayerVariable = new DarkUI.Controls.DarkRadioButton();
        cmbVariable = new DarkUI.Controls.DarkComboBox();
        rdoGlobalVariable = new DarkUI.Controls.DarkRadioButton();
        rdoGuildVariable = new DarkUI.Controls.DarkRadioButton();
        grpNumericVariable = new DarkUI.Controls.DarkGroupBox();
        rdoTimeSystem = new DarkUI.Controls.DarkRadioButton();
        cmbCompareUserVar = new DarkUI.Controls.DarkComboBox();
        rdoVarCompareUserVar = new DarkUI.Controls.DarkRadioButton();
        cmbNumericComparitor = new DarkUI.Controls.DarkComboBox();
        nudVariableValue = new DarkUI.Controls.DarkNumericUpDown();
        lblNumericComparator = new Label();
        cmbCompareGuildVar = new DarkUI.Controls.DarkComboBox();
        rdoVarCompareStaticValue = new DarkUI.Controls.DarkRadioButton();
        cmbComparePlayerVar = new DarkUI.Controls.DarkComboBox();
        rdoVarComparePlayerVar = new DarkUI.Controls.DarkRadioButton();
        rdoVarCompareGuildVar = new DarkUI.Controls.DarkRadioButton();
        cmbCompareGlobalVar = new DarkUI.Controls.DarkComboBox();
        rdoVarCompareGlobalVar = new DarkUI.Controls.DarkRadioButton();
        grpStringVariable = new DarkUI.Controls.DarkGroupBox();
        lblStringTextVariables = new Label();
        lblStringComparatorValue = new Label();
        txtStringValue = new DarkUI.Controls.DarkTextBox();
        cmbStringComparitor = new DarkUI.Controls.DarkComboBox();
        lblStringComparator = new Label();
        grpBooleanVariable = new DarkUI.Controls.DarkGroupBox();
        cmbBooleanUserVariable = new DarkUI.Controls.DarkComboBox();
        optBooleanUserVariable = new DarkUI.Controls.DarkRadioButton();
        optBooleanTrue = new DarkUI.Controls.DarkRadioButton();
        optBooleanFalse = new DarkUI.Controls.DarkRadioButton();
        cmbBooleanComparator = new DarkUI.Controls.DarkComboBox();
        lblBooleanComparator = new Label();
        cmbBooleanGuildVariable = new DarkUI.Controls.DarkComboBox();
        cmbBooleanPlayerVariable = new DarkUI.Controls.DarkComboBox();
        optBooleanPlayerVariable = new DarkUI.Controls.DarkRadioButton();
        optBooleanGuildVariable = new DarkUI.Controls.DarkRadioButton();
        cmbBooleanGlobalVariable = new DarkUI.Controls.DarkComboBox();
        optBooleanGlobalVariable = new DarkUI.Controls.DarkRadioButton();
        grpVariable.SuspendLayout();
        grpSelectVariable.SuspendLayout();
        grpNumericVariable.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)nudVariableValue).BeginInit();
        grpStringVariable.SuspendLayout();
        grpBooleanVariable.SuspendLayout();
        SuspendLayout();
        // 
        // grpVariable
        // 
        grpVariable.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        grpVariable.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        grpVariable.Controls.Add(grpSelectVariable);
        grpVariable.Controls.Add(grpNumericVariable);
        grpVariable.Controls.Add(grpStringVariable);
        grpVariable.Controls.Add(grpBooleanVariable);
        grpVariable.ForeColor = System.Drawing.Color.Gainsboro;
        grpVariable.Location = new System.Drawing.Point(0, 0);
        grpVariable.Margin = new Padding(4, 3, 4, 3);
        grpVariable.Name = "grpVariable";
        grpVariable.Padding = new Padding(4, 3, 4, 3);
        grpVariable.Size = new Size(306, 423);
        grpVariable.TabIndex = 25;
        grpVariable.TabStop = false;
        grpVariable.Text = "Variable is...";
        // 
        // grpSelectVariable
        // 
        grpSelectVariable.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        grpSelectVariable.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        grpSelectVariable.Controls.Add(rdoUserVariable);
        grpSelectVariable.Controls.Add(rdoPlayerVariable);
        grpSelectVariable.Controls.Add(cmbVariable);
        grpSelectVariable.Controls.Add(rdoGlobalVariable);
        grpSelectVariable.Controls.Add(rdoGuildVariable);
        grpSelectVariable.ForeColor = System.Drawing.Color.Gainsboro;
        grpSelectVariable.Location = new System.Drawing.Point(8, 18);
        grpSelectVariable.Margin = new Padding(4, 3, 4, 3);
        grpSelectVariable.Name = "grpSelectVariable";
        grpSelectVariable.Padding = new Padding(4, 3, 4, 3);
        grpSelectVariable.Size = new Size(288, 134);
        grpSelectVariable.TabIndex = 50;
        grpSelectVariable.TabStop = false;
        grpSelectVariable.Text = "Select Variable";
        // 
        // rdoUserVariable
        // 
        rdoUserVariable.AutoSize = true;
        rdoUserVariable.Location = new System.Drawing.Point(136, 47);
        rdoUserVariable.Margin = new Padding(4, 3, 4, 3);
        rdoUserVariable.Name = "rdoUserVariable";
        rdoUserVariable.Size = new Size(114, 19);
        rdoUserVariable.TabIndex = 36;
        rdoUserVariable.Text = "Account Variable";
        // 
        // rdoPlayerVariable
        // 
        rdoPlayerVariable.AutoSize = true;
        rdoPlayerVariable.Checked = true;
        rdoPlayerVariable.Location = new System.Drawing.Point(8, 22);
        rdoPlayerVariable.Margin = new Padding(4, 3, 4, 3);
        rdoPlayerVariable.Name = "rdoPlayerVariable";
        rdoPlayerVariable.Size = new Size(101, 19);
        rdoPlayerVariable.TabIndex = 34;
        rdoPlayerVariable.TabStop = true;
        rdoPlayerVariable.Text = "Player Variable";
        // 
        // cmbVariable
        // 
        cmbVariable.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbVariable.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbVariable.BorderStyle = ButtonBorderStyle.Solid;
        cmbVariable.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbVariable.DrawDropdownHoverOutline = false;
        cmbVariable.DrawFocusRectangle = false;
        cmbVariable.DrawMode = DrawMode.OwnerDrawFixed;
        cmbVariable.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbVariable.FlatStyle = FlatStyle.Flat;
        cmbVariable.ForeColor = System.Drawing.Color.Gainsboro;
        cmbVariable.FormattingEnabled = true;
        cmbVariable.Location = new System.Drawing.Point(7, 89);
        cmbVariable.Margin = new Padding(4, 3, 4, 3);
        cmbVariable.Name = "cmbVariable";
        cmbVariable.Size = new Size(274, 24);
        cmbVariable.TabIndex = 22;
        cmbVariable.Text = null;
        cmbVariable.TextPadding = new Padding(2);
        // 
        // rdoGlobalVariable
        // 
        rdoGlobalVariable.AutoSize = true;
        rdoGlobalVariable.Location = new System.Drawing.Point(136, 22);
        rdoGlobalVariable.Margin = new Padding(4, 3, 4, 3);
        rdoGlobalVariable.Name = "rdoGlobalVariable";
        rdoGlobalVariable.Size = new Size(103, 19);
        rdoGlobalVariable.TabIndex = 35;
        rdoGlobalVariable.Text = "Global Variable";
        // 
        // rdoGuildVariable
        // 
        rdoGuildVariable.AutoSize = true;
        rdoGuildVariable.Location = new System.Drawing.Point(8, 47);
        rdoGuildVariable.Margin = new Padding(4, 3, 4, 3);
        rdoGuildVariable.Name = "rdoGuildVariable";
        rdoGuildVariable.Size = new Size(97, 19);
        rdoGuildVariable.TabIndex = 35;
        rdoGuildVariable.Text = "Guild Variable";
        // 
        // grpNumericVariable
        // 
        grpNumericVariable.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        grpNumericVariable.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        grpNumericVariable.Controls.Add(rdoTimeSystem);
        grpNumericVariable.Controls.Add(cmbCompareUserVar);
        grpNumericVariable.Controls.Add(rdoVarCompareUserVar);
        grpNumericVariable.Controls.Add(cmbNumericComparitor);
        grpNumericVariable.Controls.Add(nudVariableValue);
        grpNumericVariable.Controls.Add(lblNumericComparator);
        grpNumericVariable.Controls.Add(cmbCompareGuildVar);
        grpNumericVariable.Controls.Add(rdoVarCompareStaticValue);
        grpNumericVariable.Controls.Add(cmbComparePlayerVar);
        grpNumericVariable.Controls.Add(rdoVarComparePlayerVar);
        grpNumericVariable.Controls.Add(rdoVarCompareGuildVar);
        grpNumericVariable.Controls.Add(cmbCompareGlobalVar);
        grpNumericVariable.Controls.Add(rdoVarCompareGlobalVar);
        grpNumericVariable.ForeColor = System.Drawing.Color.Gainsboro;
        grpNumericVariable.Location = new System.Drawing.Point(9, 160);
        grpNumericVariable.Margin = new Padding(4, 3, 4, 3);
        grpNumericVariable.Name = "grpNumericVariable";
        grpNumericVariable.Padding = new Padding(4, 3, 4, 3);
        grpNumericVariable.Size = new Size(288, 256);
        grpNumericVariable.TabIndex = 51;
        grpNumericVariable.TabStop = false;
        grpNumericVariable.Text = "Numeric Variable:";
        // 
        // rdoTimeSystem
        // 
        rdoTimeSystem.AutoSize = true;
        rdoTimeSystem.Location = new System.Drawing.Point(13, 215);
        rdoTimeSystem.Margin = new Padding(4, 3, 4, 3);
        rdoTimeSystem.Name = "rdoTimeSystem";
        rdoTimeSystem.Size = new Size(92, 19);
        rdoTimeSystem.TabIndex = 52;
        rdoTimeSystem.Text = "Time System";
        // 
        // cmbCompareUserVar
        // 
        cmbCompareUserVar.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbCompareUserVar.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbCompareUserVar.BorderStyle = ButtonBorderStyle.Solid;
        cmbCompareUserVar.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbCompareUserVar.DrawDropdownHoverOutline = false;
        cmbCompareUserVar.DrawFocusRectangle = false;
        cmbCompareUserVar.DrawMode = DrawMode.OwnerDrawFixed;
        cmbCompareUserVar.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbCompareUserVar.FlatStyle = FlatStyle.Flat;
        cmbCompareUserVar.ForeColor = System.Drawing.Color.Gainsboro;
        cmbCompareUserVar.FormattingEnabled = true;
        cmbCompareUserVar.Location = new System.Drawing.Point(170, 180);
        cmbCompareUserVar.Margin = new Padding(4, 3, 4, 3);
        cmbCompareUserVar.Name = "cmbCompareUserVar";
        cmbCompareUserVar.Size = new Size(109, 24);
        cmbCompareUserVar.TabIndex = 51;
        cmbCompareUserVar.Text = null;
        cmbCompareUserVar.TextPadding = new Padding(2);
        // 
        // rdoVarCompareUserVar
        // 
        rdoVarCompareUserVar.AutoSize = true;
        rdoVarCompareUserVar.Location = new System.Drawing.Point(13, 181);
        rdoVarCompareUserVar.Margin = new Padding(4, 3, 4, 3);
        rdoVarCompareUserVar.Name = "rdoVarCompareUserVar";
        rdoVarCompareUserVar.Size = new Size(148, 19);
        rdoVarCompareUserVar.TabIndex = 50;
        rdoVarCompareUserVar.Text = "Account Variable Value:";
        // 
        // cmbNumericComparitor
        // 
        cmbNumericComparitor.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbNumericComparitor.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbNumericComparitor.BorderStyle = ButtonBorderStyle.Solid;
        cmbNumericComparitor.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbNumericComparitor.DrawDropdownHoverOutline = false;
        cmbNumericComparitor.DrawFocusRectangle = false;
        cmbNumericComparitor.DrawMode = DrawMode.OwnerDrawFixed;
        cmbNumericComparitor.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbNumericComparitor.FlatStyle = FlatStyle.Flat;
        cmbNumericComparitor.ForeColor = System.Drawing.Color.Gainsboro;
        cmbNumericComparitor.FormattingEnabled = true;
        cmbNumericComparitor.Items.AddRange(new object[] { "Equal To", "Greater Than or Equal To", "Less Than or Equal To", "Greater Than", "Less Than", "Does Not Equal" });
        cmbNumericComparitor.Location = new System.Drawing.Point(134, 23);
        cmbNumericComparitor.Margin = new Padding(4, 3, 4, 3);
        cmbNumericComparitor.Name = "cmbNumericComparitor";
        cmbNumericComparitor.Size = new Size(145, 24);
        cmbNumericComparitor.TabIndex = 3;
        cmbNumericComparitor.Text = "Equal To";
        cmbNumericComparitor.TextPadding = new Padding(2);
        // 
        // nudVariableValue
        // 
        nudVariableValue.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        nudVariableValue.ForeColor = System.Drawing.Color.Gainsboro;
        nudVariableValue.Location = new System.Drawing.Point(134, 55);
        nudVariableValue.Margin = new Padding(4, 3, 4, 3);
        nudVariableValue.Maximum = new decimal(new int[] { -1, -1, -1, 0 });
        nudVariableValue.Minimum = new decimal(new int[] { -1, -1, -1, int.MinValue });
        nudVariableValue.Name = "nudVariableValue";
        nudVariableValue.Size = new Size(146, 23);
        nudVariableValue.TabIndex = 49;
        nudVariableValue.Value = new decimal(new int[] { 0, 0, 0, 0 });
        // 
        // lblNumericComparator
        // 
        lblNumericComparator.AutoSize = true;
        lblNumericComparator.Location = new System.Drawing.Point(11, 27);
        lblNumericComparator.Margin = new Padding(4, 0, 4, 0);
        lblNumericComparator.Name = "lblNumericComparator";
        lblNumericComparator.Size = new Size(71, 15);
        lblNumericComparator.TabIndex = 2;
        lblNumericComparator.Text = "Comparator";
        // 
        // cmbCompareGuildVar
        // 
        cmbCompareGuildVar.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbCompareGuildVar.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbCompareGuildVar.BorderStyle = ButtonBorderStyle.Solid;
        cmbCompareGuildVar.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbCompareGuildVar.DrawDropdownHoverOutline = false;
        cmbCompareGuildVar.DrawFocusRectangle = false;
        cmbCompareGuildVar.DrawMode = DrawMode.OwnerDrawFixed;
        cmbCompareGuildVar.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbCompareGuildVar.FlatStyle = FlatStyle.Flat;
        cmbCompareGuildVar.ForeColor = System.Drawing.Color.Gainsboro;
        cmbCompareGuildVar.FormattingEnabled = true;
        cmbCompareGuildVar.Location = new System.Drawing.Point(170, 149);
        cmbCompareGuildVar.Margin = new Padding(4, 3, 4, 3);
        cmbCompareGuildVar.Name = "cmbCompareGuildVar";
        cmbCompareGuildVar.Size = new Size(109, 24);
        cmbCompareGuildVar.TabIndex = 48;
        cmbCompareGuildVar.Text = null;
        cmbCompareGuildVar.TextPadding = new Padding(2);
        // 
        // rdoVarCompareStaticValue
        // 
        rdoVarCompareStaticValue.Location = new System.Drawing.Point(12, 55);
        rdoVarCompareStaticValue.Margin = new Padding(4, 3, 4, 3);
        rdoVarCompareStaticValue.Name = "rdoVarCompareStaticValue";
        rdoVarCompareStaticValue.Size = new Size(112, 20);
        rdoVarCompareStaticValue.TabIndex = 44;
        rdoVarCompareStaticValue.Text = "Static Value:";
        // 
        // cmbComparePlayerVar
        // 
        cmbComparePlayerVar.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbComparePlayerVar.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbComparePlayerVar.BorderStyle = ButtonBorderStyle.Solid;
        cmbComparePlayerVar.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbComparePlayerVar.DrawDropdownHoverOutline = false;
        cmbComparePlayerVar.DrawFocusRectangle = false;
        cmbComparePlayerVar.DrawMode = DrawMode.OwnerDrawFixed;
        cmbComparePlayerVar.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbComparePlayerVar.FlatStyle = FlatStyle.Flat;
        cmbComparePlayerVar.ForeColor = System.Drawing.Color.Gainsboro;
        cmbComparePlayerVar.FormattingEnabled = true;
        cmbComparePlayerVar.Location = new System.Drawing.Point(170, 87);
        cmbComparePlayerVar.Margin = new Padding(4, 3, 4, 3);
        cmbComparePlayerVar.Name = "cmbComparePlayerVar";
        cmbComparePlayerVar.Size = new Size(109, 24);
        cmbComparePlayerVar.TabIndex = 47;
        cmbComparePlayerVar.Text = null;
        cmbComparePlayerVar.TextPadding = new Padding(2);
        // 
        // rdoVarComparePlayerVar
        // 
        rdoVarComparePlayerVar.AutoSize = true;
        rdoVarComparePlayerVar.Location = new System.Drawing.Point(13, 88);
        rdoVarComparePlayerVar.Margin = new Padding(4, 3, 4, 3);
        rdoVarComparePlayerVar.Name = "rdoVarComparePlayerVar";
        rdoVarComparePlayerVar.Size = new Size(135, 19);
        rdoVarComparePlayerVar.TabIndex = 45;
        rdoVarComparePlayerVar.Text = "Player Variable Value:";
        // 
        // rdoVarCompareGuildVar
        // 
        rdoVarCompareGuildVar.AutoSize = true;
        rdoVarCompareGuildVar.Location = new System.Drawing.Point(13, 150);
        rdoVarCompareGuildVar.Margin = new Padding(4, 3, 4, 3);
        rdoVarCompareGuildVar.Name = "rdoVarCompareGuildVar";
        rdoVarCompareGuildVar.Size = new Size(131, 19);
        rdoVarCompareGuildVar.TabIndex = 46;
        rdoVarCompareGuildVar.Text = "Guild Variable Value:";
        // 
        // cmbCompareGlobalVar
        // 
        cmbCompareGlobalVar.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbCompareGlobalVar.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbCompareGlobalVar.BorderStyle = ButtonBorderStyle.Solid;
        cmbCompareGlobalVar.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbCompareGlobalVar.DrawDropdownHoverOutline = false;
        cmbCompareGlobalVar.DrawFocusRectangle = false;
        cmbCompareGlobalVar.DrawMode = DrawMode.OwnerDrawFixed;
        cmbCompareGlobalVar.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbCompareGlobalVar.FlatStyle = FlatStyle.Flat;
        cmbCompareGlobalVar.ForeColor = System.Drawing.Color.Gainsboro;
        cmbCompareGlobalVar.FormattingEnabled = true;
        cmbCompareGlobalVar.Location = new System.Drawing.Point(170, 118);
        cmbCompareGlobalVar.Margin = new Padding(4, 3, 4, 3);
        cmbCompareGlobalVar.Name = "cmbCompareGlobalVar";
        cmbCompareGlobalVar.Size = new Size(109, 24);
        cmbCompareGlobalVar.TabIndex = 48;
        cmbCompareGlobalVar.Text = null;
        cmbCompareGlobalVar.TextPadding = new Padding(2);
        // 
        // rdoVarCompareGlobalVar
        // 
        rdoVarCompareGlobalVar.AutoSize = true;
        rdoVarCompareGlobalVar.Location = new System.Drawing.Point(13, 119);
        rdoVarCompareGlobalVar.Margin = new Padding(4, 3, 4, 3);
        rdoVarCompareGlobalVar.Name = "rdoVarCompareGlobalVar";
        rdoVarCompareGlobalVar.Size = new Size(137, 19);
        rdoVarCompareGlobalVar.TabIndex = 46;
        rdoVarCompareGlobalVar.Text = "Global Variable Value:";
        // 
        // grpStringVariable
        // 
        grpStringVariable.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        grpStringVariable.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        grpStringVariable.Controls.Add(lblStringTextVariables);
        grpStringVariable.Controls.Add(lblStringComparatorValue);
        grpStringVariable.Controls.Add(txtStringValue);
        grpStringVariable.Controls.Add(cmbStringComparitor);
        grpStringVariable.Controls.Add(lblStringComparator);
        grpStringVariable.ForeColor = System.Drawing.Color.Gainsboro;
        grpStringVariable.Location = new System.Drawing.Point(7, 168);
        grpStringVariable.Margin = new Padding(4, 3, 4, 3);
        grpStringVariable.Name = "grpStringVariable";
        grpStringVariable.Padding = new Padding(4, 3, 4, 3);
        grpStringVariable.Size = new Size(288, 202);
        grpStringVariable.TabIndex = 53;
        grpStringVariable.TabStop = false;
        grpStringVariable.Text = "String Variable:";
        // 
        // lblStringTextVariables
        // 
        lblStringTextVariables.AutoSize = true;
        lblStringTextVariables.BackColor = System.Drawing.Color.Transparent;
        lblStringTextVariables.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Underline, GraphicsUnit.Point, 0);
        lblStringTextVariables.ForeColor = SystemColors.MenuHighlight;
        lblStringTextVariables.Location = new System.Drawing.Point(10, 166);
        lblStringTextVariables.Margin = new Padding(4, 0, 4, 0);
        lblStringTextVariables.Name = "lblStringTextVariables";
        lblStringTextVariables.Size = new Size(218, 13);
        lblStringTextVariables.TabIndex = 69;
        lblStringTextVariables.Text = "Text variables work here. Click here for a list!";
        // 
        // lblStringComparatorValue
        // 
        lblStringComparatorValue.AutoSize = true;
        lblStringComparatorValue.Location = new System.Drawing.Point(11, 60);
        lblStringComparatorValue.Margin = new Padding(4, 0, 4, 0);
        lblStringComparatorValue.Name = "lblStringComparatorValue";
        lblStringComparatorValue.Size = new Size(38, 15);
        lblStringComparatorValue.TabIndex = 63;
        lblStringComparatorValue.Text = "Value:";
        // 
        // txtStringValue
        // 
        txtStringValue.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        txtStringValue.BorderStyle = BorderStyle.FixedSingle;
        txtStringValue.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
        txtStringValue.Location = new System.Drawing.Point(102, 58);
        txtStringValue.Margin = new Padding(4, 3, 4, 3);
        txtStringValue.Name = "txtStringValue";
        txtStringValue.Size = new Size(178, 23);
        txtStringValue.TabIndex = 62;
        // 
        // cmbStringComparitor
        // 
        cmbStringComparitor.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbStringComparitor.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbStringComparitor.BorderStyle = ButtonBorderStyle.Solid;
        cmbStringComparitor.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbStringComparitor.DrawDropdownHoverOutline = false;
        cmbStringComparitor.DrawFocusRectangle = false;
        cmbStringComparitor.DrawMode = DrawMode.OwnerDrawFixed;
        cmbStringComparitor.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbStringComparitor.FlatStyle = FlatStyle.Flat;
        cmbStringComparitor.ForeColor = System.Drawing.Color.Gainsboro;
        cmbStringComparitor.FormattingEnabled = true;
        cmbStringComparitor.Items.AddRange(new object[] { "Equal To", "Contains" });
        cmbStringComparitor.Location = new System.Drawing.Point(102, 23);
        cmbStringComparitor.Margin = new Padding(4, 3, 4, 3);
        cmbStringComparitor.Name = "cmbStringComparitor";
        cmbStringComparitor.Size = new Size(178, 24);
        cmbStringComparitor.TabIndex = 3;
        cmbStringComparitor.Text = "Equal To";
        cmbStringComparitor.TextPadding = new Padding(2);
        // 
        // lblStringComparator
        // 
        lblStringComparator.AutoSize = true;
        lblStringComparator.Location = new System.Drawing.Point(11, 27);
        lblStringComparator.Margin = new Padding(4, 0, 4, 0);
        lblStringComparator.Name = "lblStringComparator";
        lblStringComparator.Size = new Size(74, 15);
        lblStringComparator.TabIndex = 2;
        lblStringComparator.Text = "Comparator:";
        // 
        // grpBooleanVariable
        // 
        grpBooleanVariable.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        grpBooleanVariable.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        grpBooleanVariable.Controls.Add(cmbBooleanUserVariable);
        grpBooleanVariable.Controls.Add(optBooleanUserVariable);
        grpBooleanVariable.Controls.Add(optBooleanTrue);
        grpBooleanVariable.Controls.Add(optBooleanFalse);
        grpBooleanVariable.Controls.Add(cmbBooleanComparator);
        grpBooleanVariable.Controls.Add(lblBooleanComparator);
        grpBooleanVariable.Controls.Add(cmbBooleanGuildVariable);
        grpBooleanVariable.Controls.Add(cmbBooleanPlayerVariable);
        grpBooleanVariable.Controls.Add(optBooleanPlayerVariable);
        grpBooleanVariable.Controls.Add(optBooleanGuildVariable);
        grpBooleanVariable.Controls.Add(cmbBooleanGlobalVariable);
        grpBooleanVariable.Controls.Add(optBooleanGlobalVariable);
        grpBooleanVariable.ForeColor = System.Drawing.Color.Gainsboro;
        grpBooleanVariable.Location = new System.Drawing.Point(9, 160);
        grpBooleanVariable.Margin = new Padding(4, 3, 4, 3);
        grpBooleanVariable.Name = "grpBooleanVariable";
        grpBooleanVariable.Padding = new Padding(4, 3, 4, 3);
        grpBooleanVariable.Size = new Size(288, 210);
        grpBooleanVariable.TabIndex = 52;
        grpBooleanVariable.TabStop = false;
        grpBooleanVariable.Text = "Boolean Variable:";
        // 
        // cmbBooleanUserVariable
        // 
        cmbBooleanUserVariable.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbBooleanUserVariable.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbBooleanUserVariable.BorderStyle = ButtonBorderStyle.Solid;
        cmbBooleanUserVariable.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbBooleanUserVariable.DrawDropdownHoverOutline = false;
        cmbBooleanUserVariable.DrawFocusRectangle = false;
        cmbBooleanUserVariable.DrawMode = DrawMode.OwnerDrawFixed;
        cmbBooleanUserVariable.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbBooleanUserVariable.FlatStyle = FlatStyle.Flat;
        cmbBooleanUserVariable.ForeColor = System.Drawing.Color.Gainsboro;
        cmbBooleanUserVariable.FormattingEnabled = true;
        cmbBooleanUserVariable.Location = new System.Drawing.Point(170, 174);
        cmbBooleanUserVariable.Margin = new Padding(4, 3, 4, 3);
        cmbBooleanUserVariable.Name = "cmbBooleanUserVariable";
        cmbBooleanUserVariable.Size = new Size(109, 24);
        cmbBooleanUserVariable.TabIndex = 52;
        cmbBooleanUserVariable.Text = null;
        cmbBooleanUserVariable.TextPadding = new Padding(2);
        // 
        // optBooleanUserVariable
        // 
        optBooleanUserVariable.AutoSize = true;
        optBooleanUserVariable.Location = new System.Drawing.Point(13, 175);
        optBooleanUserVariable.Margin = new Padding(4, 3, 4, 3);
        optBooleanUserVariable.Name = "optBooleanUserVariable";
        optBooleanUserVariable.Size = new Size(148, 19);
        optBooleanUserVariable.TabIndex = 51;
        optBooleanUserVariable.Text = "Account Variable Value:";
        // 
        // optBooleanTrue
        // 
        optBooleanTrue.AutoSize = true;
        optBooleanTrue.Location = new System.Drawing.Point(13, 55);
        optBooleanTrue.Margin = new Padding(4, 3, 4, 3);
        optBooleanTrue.Name = "optBooleanTrue";
        optBooleanTrue.Size = new Size(47, 19);
        optBooleanTrue.TabIndex = 50;
        optBooleanTrue.Text = "True";
        // 
        // optBooleanFalse
        // 
        optBooleanFalse.AutoSize = true;
        optBooleanFalse.Location = new System.Drawing.Point(85, 55);
        optBooleanFalse.Margin = new Padding(4, 3, 4, 3);
        optBooleanFalse.Name = "optBooleanFalse";
        optBooleanFalse.Size = new Size(51, 19);
        optBooleanFalse.TabIndex = 49;
        optBooleanFalse.Text = "False";
        // 
        // cmbBooleanComparator
        // 
        cmbBooleanComparator.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbBooleanComparator.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbBooleanComparator.BorderStyle = ButtonBorderStyle.Solid;
        cmbBooleanComparator.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbBooleanComparator.DrawDropdownHoverOutline = false;
        cmbBooleanComparator.DrawFocusRectangle = false;
        cmbBooleanComparator.DrawMode = DrawMode.OwnerDrawFixed;
        cmbBooleanComparator.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbBooleanComparator.FlatStyle = FlatStyle.Flat;
        cmbBooleanComparator.ForeColor = System.Drawing.Color.Gainsboro;
        cmbBooleanComparator.FormattingEnabled = true;
        cmbBooleanComparator.Items.AddRange(new object[] { "Equal To", "Not Equal To" });
        cmbBooleanComparator.Location = new System.Drawing.Point(134, 23);
        cmbBooleanComparator.Margin = new Padding(4, 3, 4, 3);
        cmbBooleanComparator.Name = "cmbBooleanComparator";
        cmbBooleanComparator.Size = new Size(145, 24);
        cmbBooleanComparator.TabIndex = 3;
        cmbBooleanComparator.Text = "Equal To";
        cmbBooleanComparator.TextPadding = new Padding(2);
        // 
        // lblBooleanComparator
        // 
        lblBooleanComparator.AutoSize = true;
        lblBooleanComparator.Location = new System.Drawing.Point(11, 27);
        lblBooleanComparator.Margin = new Padding(4, 0, 4, 0);
        lblBooleanComparator.Name = "lblBooleanComparator";
        lblBooleanComparator.Size = new Size(71, 15);
        lblBooleanComparator.TabIndex = 2;
        lblBooleanComparator.Text = "Comparator";
        // 
        // cmbBooleanGuildVariable
        // 
        cmbBooleanGuildVariable.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbBooleanGuildVariable.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbBooleanGuildVariable.BorderStyle = ButtonBorderStyle.Solid;
        cmbBooleanGuildVariable.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbBooleanGuildVariable.DrawDropdownHoverOutline = false;
        cmbBooleanGuildVariable.DrawFocusRectangle = false;
        cmbBooleanGuildVariable.DrawMode = DrawMode.OwnerDrawFixed;
        cmbBooleanGuildVariable.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbBooleanGuildVariable.FlatStyle = FlatStyle.Flat;
        cmbBooleanGuildVariable.ForeColor = System.Drawing.Color.Gainsboro;
        cmbBooleanGuildVariable.FormattingEnabled = true;
        cmbBooleanGuildVariable.Location = new System.Drawing.Point(170, 143);
        cmbBooleanGuildVariable.Margin = new Padding(4, 3, 4, 3);
        cmbBooleanGuildVariable.Name = "cmbBooleanGuildVariable";
        cmbBooleanGuildVariable.Size = new Size(109, 24);
        cmbBooleanGuildVariable.TabIndex = 48;
        cmbBooleanGuildVariable.Text = null;
        cmbBooleanGuildVariable.TextPadding = new Padding(2);
        // 
        // cmbBooleanPlayerVariable
        // 
        cmbBooleanPlayerVariable.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbBooleanPlayerVariable.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbBooleanPlayerVariable.BorderStyle = ButtonBorderStyle.Solid;
        cmbBooleanPlayerVariable.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbBooleanPlayerVariable.DrawDropdownHoverOutline = false;
        cmbBooleanPlayerVariable.DrawFocusRectangle = false;
        cmbBooleanPlayerVariable.DrawMode = DrawMode.OwnerDrawFixed;
        cmbBooleanPlayerVariable.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbBooleanPlayerVariable.FlatStyle = FlatStyle.Flat;
        cmbBooleanPlayerVariable.ForeColor = System.Drawing.Color.Gainsboro;
        cmbBooleanPlayerVariable.FormattingEnabled = true;
        cmbBooleanPlayerVariable.Location = new System.Drawing.Point(170, 81);
        cmbBooleanPlayerVariable.Margin = new Padding(4, 3, 4, 3);
        cmbBooleanPlayerVariable.Name = "cmbBooleanPlayerVariable";
        cmbBooleanPlayerVariable.Size = new Size(109, 24);
        cmbBooleanPlayerVariable.TabIndex = 47;
        cmbBooleanPlayerVariable.Text = null;
        cmbBooleanPlayerVariable.TextPadding = new Padding(2);
        // 
        // optBooleanPlayerVariable
        // 
        optBooleanPlayerVariable.AutoSize = true;
        optBooleanPlayerVariable.Location = new System.Drawing.Point(13, 82);
        optBooleanPlayerVariable.Margin = new Padding(4, 3, 4, 3);
        optBooleanPlayerVariable.Name = "optBooleanPlayerVariable";
        optBooleanPlayerVariable.Size = new Size(135, 19);
        optBooleanPlayerVariable.TabIndex = 45;
        optBooleanPlayerVariable.Text = "Player Variable Value:";
        // 
        // optBooleanGuildVariable
        // 
        optBooleanGuildVariable.AutoSize = true;
        optBooleanGuildVariable.Location = new System.Drawing.Point(13, 144);
        optBooleanGuildVariable.Margin = new Padding(4, 3, 4, 3);
        optBooleanGuildVariable.Name = "optBooleanGuildVariable";
        optBooleanGuildVariable.Size = new Size(131, 19);
        optBooleanGuildVariable.TabIndex = 46;
        optBooleanGuildVariable.Text = "Guild Variable Value:";
        // 
        // cmbBooleanGlobalVariable
        // 
        cmbBooleanGlobalVariable.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbBooleanGlobalVariable.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbBooleanGlobalVariable.BorderStyle = ButtonBorderStyle.Solid;
        cmbBooleanGlobalVariable.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbBooleanGlobalVariable.DrawDropdownHoverOutline = false;
        cmbBooleanGlobalVariable.DrawFocusRectangle = false;
        cmbBooleanGlobalVariable.DrawMode = DrawMode.OwnerDrawFixed;
        cmbBooleanGlobalVariable.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbBooleanGlobalVariable.FlatStyle = FlatStyle.Flat;
        cmbBooleanGlobalVariable.ForeColor = System.Drawing.Color.Gainsboro;
        cmbBooleanGlobalVariable.FormattingEnabled = true;
        cmbBooleanGlobalVariable.Location = new System.Drawing.Point(170, 112);
        cmbBooleanGlobalVariable.Margin = new Padding(4, 3, 4, 3);
        cmbBooleanGlobalVariable.Name = "cmbBooleanGlobalVariable";
        cmbBooleanGlobalVariable.Size = new Size(109, 24);
        cmbBooleanGlobalVariable.TabIndex = 48;
        cmbBooleanGlobalVariable.Text = null;
        cmbBooleanGlobalVariable.TextPadding = new Padding(2);
        // 
        // optBooleanGlobalVariable
        // 
        optBooleanGlobalVariable.AutoSize = true;
        optBooleanGlobalVariable.Location = new System.Drawing.Point(13, 113);
        optBooleanGlobalVariable.Margin = new Padding(4, 3, 4, 3);
        optBooleanGlobalVariable.Name = "optBooleanGlobalVariable";
        optBooleanGlobalVariable.Size = new Size(137, 19);
        optBooleanGlobalVariable.TabIndex = 46;
        optBooleanGlobalVariable.Text = "Global Variable Value:";
        // 
        // ConditionControl_VariableIs
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        Controls.Add(grpVariable);
        Name = "ConditionControl_VariableIs";
        Size = new Size(310, 430);
        grpVariable.ResumeLayout(false);
        grpSelectVariable.ResumeLayout(false);
        grpSelectVariable.PerformLayout();
        grpNumericVariable.ResumeLayout(false);
        grpNumericVariable.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)nudVariableValue).EndInit();
        grpStringVariable.ResumeLayout(false);
        grpStringVariable.PerformLayout();
        grpBooleanVariable.ResumeLayout(false);
        grpBooleanVariable.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private DarkUI.Controls.DarkGroupBox grpVariable;
    private DarkUI.Controls.DarkGroupBox grpSelectVariable;
    private DarkUI.Controls.DarkRadioButton rdoUserVariable;
    private DarkUI.Controls.DarkRadioButton rdoPlayerVariable;
    internal DarkUI.Controls.DarkComboBox cmbVariable;
    private DarkUI.Controls.DarkRadioButton rdoGlobalVariable;
    private DarkUI.Controls.DarkRadioButton rdoGuildVariable;
    private DarkUI.Controls.DarkGroupBox grpNumericVariable;
    internal DarkUI.Controls.DarkRadioButton rdoTimeSystem;
    internal DarkUI.Controls.DarkComboBox cmbCompareUserVar;
    internal DarkUI.Controls.DarkRadioButton rdoVarCompareUserVar;
    private DarkUI.Controls.DarkComboBox cmbNumericComparitor;
    private DarkUI.Controls.DarkNumericUpDown nudVariableValue;
    private Label lblNumericComparator;
    internal DarkUI.Controls.DarkComboBox cmbCompareGuildVar;
    internal DarkUI.Controls.DarkRadioButton rdoVarCompareStaticValue;
    internal DarkUI.Controls.DarkComboBox cmbComparePlayerVar;
    internal DarkUI.Controls.DarkRadioButton rdoVarComparePlayerVar;
    internal DarkUI.Controls.DarkRadioButton rdoVarCompareGuildVar;
    internal DarkUI.Controls.DarkComboBox cmbCompareGlobalVar;
    internal DarkUI.Controls.DarkRadioButton rdoVarCompareGlobalVar;
    private DarkUI.Controls.DarkGroupBox grpStringVariable;
    private Label lblStringTextVariables;
    private Label lblStringComparatorValue;
    private DarkUI.Controls.DarkTextBox txtStringValue;
    private DarkUI.Controls.DarkComboBox cmbStringComparitor;
    private Label lblStringComparator;
    private DarkUI.Controls.DarkGroupBox grpBooleanVariable;
    internal DarkUI.Controls.DarkComboBox cmbBooleanUserVariable;
    internal DarkUI.Controls.DarkRadioButton optBooleanUserVariable;
    internal DarkUI.Controls.DarkRadioButton optBooleanTrue;
    internal DarkUI.Controls.DarkRadioButton optBooleanFalse;
    private DarkUI.Controls.DarkComboBox cmbBooleanComparator;
    private Label lblBooleanComparator;
    internal DarkUI.Controls.DarkComboBox cmbBooleanGuildVariable;
    internal DarkUI.Controls.DarkComboBox cmbBooleanPlayerVariable;
    internal DarkUI.Controls.DarkRadioButton optBooleanPlayerVariable;
    internal DarkUI.Controls.DarkRadioButton optBooleanGuildVariable;
    internal DarkUI.Controls.DarkComboBox cmbBooleanGlobalVariable;
    internal DarkUI.Controls.DarkRadioButton optBooleanGlobalVariable;
}
