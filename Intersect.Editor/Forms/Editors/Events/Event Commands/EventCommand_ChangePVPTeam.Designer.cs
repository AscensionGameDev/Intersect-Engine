namespace Intersect.Editor.Forms.Editors.Events.Event_Commands;

partial class EventCommandChangePVPTeam
{
    /// <summary> 
    /// Обязательная переменная конструктора.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Освободить все используемые ресурсы.
    /// </summary>
    /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Код, автоматически созданный конструктором компонентов

    /// <summary> 
    /// Требуемый метод для поддержки конструктора — не изменяйте 
    /// содержимое этого метода с помощью редактора кода.
    /// </summary>
    private void InitializeComponent()
    {
        this.grpSetPVPTeam = new DarkUI.Controls.DarkGroupBox();
        this.grpManualAmount = new DarkUI.Controls.DarkGroupBox();
        this.nudTeamID = new DarkUI.Controls.DarkNumericUpDown();
        this.lblPVPTeam = new System.Windows.Forms.Label();
        this.grpVariableAmount = new DarkUI.Controls.DarkGroupBox();
        this.cmbVariable = new DarkUI.Controls.DarkComboBox();
        this.lblVariable = new System.Windows.Forms.Label();
        this.rdoGlobalVariable = new DarkUI.Controls.DarkRadioButton();
        this.rdoGuildVariable = new DarkUI.Controls.DarkRadioButton();
        this.rdoPlayerVariable = new DarkUI.Controls.DarkRadioButton();
        this.grpAmountType = new DarkUI.Controls.DarkGroupBox();
        this.rdoVariable = new DarkUI.Controls.DarkRadioButton();
        this.rdoManual = new DarkUI.Controls.DarkRadioButton();
        this.btnCancel = new DarkUI.Controls.DarkButton();
        this.btnSave = new DarkUI.Controls.DarkButton();
        this.grpSetPVPTeam.SuspendLayout();
        this.grpManualAmount.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.nudTeamID)).BeginInit();
        this.grpVariableAmount.SuspendLayout();
        this.grpAmountType.SuspendLayout();
        this.SuspendLayout();
        // 
        // grpSetPVPTeam
        // 
        this.grpSetPVPTeam.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
        this.grpSetPVPTeam.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
        this.grpSetPVPTeam.Controls.Add(this.grpVariableAmount);
        this.grpSetPVPTeam.Controls.Add(this.grpAmountType);
        this.grpSetPVPTeam.Controls.Add(this.btnCancel);
        this.grpSetPVPTeam.Controls.Add(this.btnSave);
        this.grpSetPVPTeam.Controls.Add(this.grpManualAmount);
        this.grpSetPVPTeam.ForeColor = System.Drawing.Color.Gainsboro;
        this.grpSetPVPTeam.Location = new System.Drawing.Point(3, 3);
        this.grpSetPVPTeam.Name = "grpSetPVPTeam";
        this.grpSetPVPTeam.Size = new System.Drawing.Size(427, 157);
        this.grpSetPVPTeam.TabIndex = 17;
        this.grpSetPVPTeam.TabStop = false;
        this.grpSetPVPTeam.Text = "Set PVP Team:";
        // 
        // grpManualAmount
        // 
        this.grpManualAmount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
        this.grpManualAmount.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
        this.grpManualAmount.Controls.Add(this.nudTeamID);
        this.grpManualAmount.Controls.Add(this.lblPVPTeam);
        this.grpManualAmount.ForeColor = System.Drawing.Color.Gainsboro;
        this.grpManualAmount.Location = new System.Drawing.Point(6, 19);
        this.grpManualAmount.Name = "grpManualAmount";
        this.grpManualAmount.Size = new System.Drawing.Size(292, 71);
        this.grpManualAmount.TabIndex = 40;
        this.grpManualAmount.TabStop = false;
        this.grpManualAmount.Text = "Manual";
        this.grpManualAmount.Visible = false;
        // 
        // nudTeamID
        // 
        this.nudTeamID.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
        this.nudTeamID.ForeColor = System.Drawing.Color.Gainsboro;
        this.nudTeamID.Location = new System.Drawing.Point(130, 25);
        this.nudTeamID.Minimum = -1;
        this.nudTeamID.Maximum = 1000;
        this.nudTeamID.Name = "nudTeamID";
        this.nudTeamID.Size = new System.Drawing.Size(141, 20);
        this.nudTeamID.TabIndex = 24;
        this.nudTeamID.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
        // 
        // lblPVPTeam
        // 
        this.lblPVPTeam.AutoSize = true;
        this.lblPVPTeam.Location = new System.Drawing.Point(22, 27);
        this.lblPVPTeam.Name = "lblPVPTeam";
        this.lblPVPTeam.Size = new System.Drawing.Size(91, 13);
        this.lblPVPTeam.TabIndex = 23;
        this.lblPVPTeam.Text = "Set PVP Team: ";
        // 
        // grpVariableAmount
        // 
        this.grpVariableAmount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
        this.grpVariableAmount.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
        this.grpVariableAmount.Controls.Add(this.cmbVariable);
        this.grpVariableAmount.Controls.Add(this.lblVariable);
        this.grpVariableAmount.Controls.Add(this.rdoGlobalVariable);
        this.grpVariableAmount.Controls.Add(this.rdoGuildVariable);
        this.grpVariableAmount.Controls.Add(this.rdoPlayerVariable);
        this.grpVariableAmount.ForeColor = System.Drawing.Color.Gainsboro;
        this.grpVariableAmount.Location = new System.Drawing.Point(6, 19);
        this.grpVariableAmount.Name = "grpVariableAmount";
        this.grpVariableAmount.Size = new System.Drawing.Size(292, 103);
        this.grpVariableAmount.TabIndex = 39;
        this.grpVariableAmount.TabStop = false;
        this.grpVariableAmount.Text = "Variable";
        this.grpVariableAmount.Visible = false;
        // 
        // cmbVariable
        // 
        this.cmbVariable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
        this.cmbVariable.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
        this.cmbVariable.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
        this.cmbVariable.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
        this.cmbVariable.DrawDropdownHoverOutline = false;
        this.cmbVariable.DrawFocusRectangle = false;
        this.cmbVariable.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
        this.cmbVariable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.cmbVariable.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.cmbVariable.ForeColor = System.Drawing.Color.Gainsboro;
        this.cmbVariable.FormattingEnabled = true;
        this.cmbVariable.Location = new System.Drawing.Point(67, 72);
        this.cmbVariable.Name = "cmbVariable";
        this.cmbVariable.Size = new System.Drawing.Size(219, 21);
        this.cmbVariable.TabIndex = 39;
        this.cmbVariable.Text = null;
        this.cmbVariable.TextPadding = new System.Windows.Forms.Padding(2);
        // 
        // lblVariable
        // 
        this.lblVariable.AutoSize = true;
        this.lblVariable.Location = new System.Drawing.Point(8, 74);
        this.lblVariable.Name = "lblVariable";
        this.lblVariable.Size = new System.Drawing.Size(45, 13);
        this.lblVariable.TabIndex = 38;
        this.lblVariable.Text = "Variable";
        // 
        // rdoGlobalVariable
        // 
        this.rdoGlobalVariable.AutoSize = true;
        this.rdoGlobalVariable.Location = new System.Drawing.Point(165, 19);
        this.rdoGlobalVariable.Name = "rdoGlobalVariable";
        this.rdoGlobalVariable.Size = new System.Drawing.Size(96, 17);
        this.rdoGlobalVariable.TabIndex = 37;
        this.rdoGlobalVariable.Text = "Global Variable";
        this.rdoGlobalVariable.CheckedChanged += new System.EventHandler(this.rdoGlobalVariable_CheckedChanged);
        // 
        // rdoGuildVariable
        // 
        this.rdoGuildVariable.AutoSize = true;
        this.rdoGuildVariable.Location = new System.Drawing.Point(6, 42);
        this.rdoGuildVariable.Name = "rdoGuildVariable";
        this.rdoGuildVariable.Size = new System.Drawing.Size(90, 17);
        this.rdoGuildVariable.TabIndex = 37;
        this.rdoGuildVariable.Text = "Guild Variable";
        this.rdoGuildVariable.CheckedChanged += new System.EventHandler(this.rdoGuildVariable_CheckedChanged);
        // 
        // rdoPlayerVariable
        // 
        this.rdoPlayerVariable.AutoSize = true;
        this.rdoPlayerVariable.Checked = true;
        this.rdoPlayerVariable.Location = new System.Drawing.Point(6, 19);
        this.rdoPlayerVariable.Name = "rdoPlayerVariable";
        this.rdoPlayerVariable.Size = new System.Drawing.Size(95, 17);
        this.rdoPlayerVariable.TabIndex = 36;
        this.rdoPlayerVariable.TabStop = true;
        this.rdoPlayerVariable.Text = "Player Variable";
        this.rdoPlayerVariable.CheckedChanged += new System.EventHandler(this.rdoPlayerVariable_CheckedChanged);
        // 
        // grpAmountType
        // 
        this.grpAmountType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
        this.grpAmountType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
        this.grpAmountType.Controls.Add(this.rdoVariable);
        this.grpAmountType.Controls.Add(this.rdoManual);
        this.grpAmountType.ForeColor = System.Drawing.Color.Gainsboro;
        this.grpAmountType.Location = new System.Drawing.Point(305, 19);
        this.grpAmountType.Name = "grpAmountType";
        this.grpAmountType.Size = new System.Drawing.Size(115, 71);
        this.grpAmountType.TabIndex = 37;
        this.grpAmountType.TabStop = false;
        this.grpAmountType.Text = "Amount Type:";
        // 
        // rdoVariable
        // 
        this.rdoVariable.AutoSize = true;
        this.rdoVariable.Location = new System.Drawing.Point(9, 42);
        this.rdoVariable.Name = "rdoVariable";
        this.rdoVariable.Size = new System.Drawing.Size(63, 17);
        this.rdoVariable.TabIndex = 36;
        this.rdoVariable.Text = "Variable";
        this.rdoVariable.CheckedChanged += new System.EventHandler(this.rdoVariable_CheckedChanged);
        // 
        // rdoManual
        // 
        this.rdoManual.AutoSize = true;
        this.rdoManual.Checked = true;
        this.rdoManual.Location = new System.Drawing.Point(9, 19);
        this.rdoManual.Name = "rdoManual";
        this.rdoManual.Size = new System.Drawing.Size(60, 17);
        this.rdoManual.TabIndex = 35;
        this.rdoManual.TabStop = true;
        this.rdoManual.Text = "Manual";
        this.rdoManual.CheckedChanged += new System.EventHandler(this.rdoManual_CheckedChanged);
        // 
        // btnCancel
        // 
        this.btnCancel.Location = new System.Drawing.Point(223, 128);
        this.btnCancel.Name = "btnCancel";
        this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
        this.btnCancel.Size = new System.Drawing.Size(75, 23);
        this.btnCancel.TabIndex = 20;
        this.btnCancel.Text = "Cancel";
        this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
        // 
        // btnSave
        // 
        this.btnSave.Location = new System.Drawing.Point(6, 128);
        this.btnSave.Name = "btnSave";
        this.btnSave.Padding = new System.Windows.Forms.Padding(5);
        this.btnSave.Size = new System.Drawing.Size(75, 23);
        this.btnSave.TabIndex = 19;
        this.btnSave.Text = "Ok";
        this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
        // 
        // EventCommandSetPVPTeam
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.AutoSize = true;
        this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
        this.Controls.Add(this.grpSetPVPTeam);
        this.Name = "EventCommandSetPVPTeam";
        this.Size = new System.Drawing.Size(436, 163);
        this.grpSetPVPTeam.ResumeLayout(false);
        this.grpManualAmount.ResumeLayout(false);
        this.grpManualAmount.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)(this.nudTeamID)).EndInit();
        this.grpVariableAmount.ResumeLayout(false);
        this.grpVariableAmount.PerformLayout();
        this.grpAmountType.ResumeLayout(false);
        this.grpAmountType.PerformLayout();
        this.ResumeLayout(false);

    }
    #endregion

    private DarkUI.Controls.DarkGroupBox grpSetPVPTeam;
    private DarkUI.Controls.DarkGroupBox grpVariableAmount;
    private DarkUI.Controls.DarkComboBox cmbVariable;
    private Label lblVariable;
    private DarkUI.Controls.DarkRadioButton rdoGlobalVariable;
    private DarkUI.Controls.DarkRadioButton rdoGuildVariable;
    private DarkUI.Controls.DarkRadioButton rdoPlayerVariable;
    private DarkUI.Controls.DarkGroupBox grpAmountType;
    private DarkUI.Controls.DarkRadioButton rdoVariable;
    private DarkUI.Controls.DarkRadioButton rdoManual;
    private DarkUI.Controls.DarkButton btnCancel;
    private DarkUI.Controls.DarkButton btnSave;
    private DarkUI.Controls.DarkGroupBox grpManualAmount;
    private DarkUI.Controls.DarkNumericUpDown nudTeamID;
    private Label lblPVPTeam;
}
