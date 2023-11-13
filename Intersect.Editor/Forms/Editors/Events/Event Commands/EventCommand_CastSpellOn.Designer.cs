namespace Intersect.Editor.Forms.Editors.Events.Event_Commands;

partial class EventCommand_CastSpellOn
{
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        grpCastSpellOn = new DarkUI.Controls.DarkGroupBox();
        grpTargets = new DarkUI.Controls.DarkGroupBox();
        chkApplyToSelf = new DarkUI.Controls.DarkCheckBox();
        darkButton1 = new DarkUI.Controls.DarkButton();
        chkApplyToGuildies = new DarkUI.Controls.DarkCheckBox();
        darkButton2 = new DarkUI.Controls.DarkButton();
        chkApplyToParty = new DarkUI.Controls.DarkCheckBox();
        cmbSpell = new DarkUI.Controls.DarkComboBox();
        lblSpell = new Label();
        btnCancel = new DarkUI.Controls.DarkButton();
        btnSave = new DarkUI.Controls.DarkButton();
        grpCastSpellOn.SuspendLayout();
        grpTargets.SuspendLayout();
        SuspendLayout();
        // 
        // grpCastSpellOn
        // 
        grpCastSpellOn.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        grpCastSpellOn.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        grpCastSpellOn.Controls.Add(grpTargets);
        grpCastSpellOn.Controls.Add(cmbSpell);
        grpCastSpellOn.Controls.Add(lblSpell);
        grpCastSpellOn.Controls.Add(btnCancel);
        grpCastSpellOn.Controls.Add(btnSave);
        grpCastSpellOn.ForeColor = System.Drawing.Color.Gainsboro;
        grpCastSpellOn.Location = new System.Drawing.Point(4, 3);
        grpCastSpellOn.Margin = new Padding(4, 3, 4, 3);
        grpCastSpellOn.Name = "grpCastSpellOn";
        grpCastSpellOn.Padding = new Padding(4, 3, 4, 3);
        grpCastSpellOn.Size = new Size(291, 217);
        grpCastSpellOn.TabIndex = 18;
        grpCastSpellOn.TabStop = false;
        grpCastSpellOn.Text = "Cast Spell On";
        // 
        // grpTargets
        // 
        grpTargets.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        grpTargets.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        grpTargets.Controls.Add(chkApplyToSelf);
        grpTargets.Controls.Add(darkButton1);
        grpTargets.Controls.Add(chkApplyToGuildies);
        grpTargets.Controls.Add(darkButton2);
        grpTargets.Controls.Add(chkApplyToParty);
        grpTargets.ForeColor = System.Drawing.Color.Gainsboro;
        grpTargets.Location = new System.Drawing.Point(10, 67);
        grpTargets.Margin = new Padding(4, 3, 4, 3);
        grpTargets.Name = "grpTargets";
        grpTargets.Padding = new Padding(4, 3, 4, 3);
        grpTargets.Size = new Size(273, 104);
        grpTargets.TabIndex = 43;
        grpTargets.TabStop = false;
        grpTargets.Text = "Targets";
        // 
        // chkApplyToSelf
        // 
        chkApplyToSelf.AutoSize = true;
        chkApplyToSelf.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        chkApplyToSelf.Location = new System.Drawing.Point(8, 22);
        chkApplyToSelf.Margin = new Padding(4, 3, 4, 3);
        chkApplyToSelf.Name = "chkApplyToSelf";
        chkApplyToSelf.Size = new Size(45, 19);
        chkApplyToSelf.TabIndex = 43;
        chkApplyToSelf.Text = "Self";
        // 
        // darkButton1
        // 
        darkButton1.Location = new System.Drawing.Point(195, 156);
        darkButton1.Margin = new Padding(4, 3, 4, 3);
        darkButton1.Name = "darkButton1";
        darkButton1.Padding = new Padding(6);
        darkButton1.Size = new Size(88, 27);
        darkButton1.TabIndex = 20;
        darkButton1.Text = "Cancel";
        // 
        // chkApplyToGuildies
        // 
        chkApplyToGuildies.AutoSize = true;
        chkApplyToGuildies.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        chkApplyToGuildies.Location = new System.Drawing.Point(8, 72);
        chkApplyToGuildies.Margin = new Padding(4, 3, 4, 3);
        chkApplyToGuildies.Name = "chkApplyToGuildies";
        chkApplyToGuildies.Size = new Size(145, 19);
        chkApplyToGuildies.TabIndex = 42;
        chkApplyToGuildies.Text = "Online Guild Members";
        // 
        // darkButton2
        // 
        darkButton2.Location = new System.Drawing.Point(98, 156);
        darkButton2.Margin = new Padding(4, 3, 4, 3);
        darkButton2.Name = "darkButton2";
        darkButton2.Padding = new Padding(6);
        darkButton2.Size = new Size(88, 27);
        darkButton2.TabIndex = 19;
        darkButton2.Text = "Ok";
        // 
        // chkApplyToParty
        // 
        chkApplyToParty.AutoSize = true;
        chkApplyToParty.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
        chkApplyToParty.Location = new System.Drawing.Point(8, 47);
        chkApplyToParty.Margin = new Padding(4, 3, 4, 3);
        chkApplyToParty.Name = "chkApplyToParty";
        chkApplyToParty.Size = new Size(106, 19);
        chkApplyToParty.TabIndex = 41;
        chkApplyToParty.Text = "Party Members";
        // 
        // cmbSpell
        // 
        cmbSpell.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
        cmbSpell.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
        cmbSpell.BorderStyle = ButtonBorderStyle.Solid;
        cmbSpell.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
        cmbSpell.DrawDropdownHoverOutline = false;
        cmbSpell.DrawFocusRectangle = false;
        cmbSpell.DrawMode = DrawMode.OwnerDrawFixed;
        cmbSpell.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbSpell.FlatStyle = FlatStyle.Flat;
        cmbSpell.ForeColor = System.Drawing.Color.Gainsboro;
        cmbSpell.FormattingEnabled = true;
        cmbSpell.Location = new System.Drawing.Point(10, 37);
        cmbSpell.Margin = new Padding(4, 3, 4, 3);
        cmbSpell.Name = "cmbSpell";
        cmbSpell.Size = new Size(273, 24);
        cmbSpell.TabIndex = 24;
        cmbSpell.Text = null;
        cmbSpell.TextPadding = new Padding(2);
        // 
        // lblSpell
        // 
        lblSpell.AutoSize = true;
        lblSpell.Location = new System.Drawing.Point(8, 19);
        lblSpell.Margin = new Padding(4, 0, 4, 0);
        lblSpell.Name = "lblSpell";
        lblSpell.Size = new Size(32, 15);
        lblSpell.TabIndex = 23;
        lblSpell.Text = "Spell";
        // 
        // btnCancel
        // 
        btnCancel.Location = new System.Drawing.Point(195, 177);
        btnCancel.Margin = new Padding(4, 3, 4, 3);
        btnCancel.Name = "btnCancel";
        btnCancel.Padding = new Padding(6);
        btnCancel.Size = new Size(88, 27);
        btnCancel.TabIndex = 20;
        btnCancel.Text = "Cancel";
        btnCancel.Click += btnCancel_Click;
        // 
        // btnSave
        // 
        btnSave.Location = new System.Drawing.Point(99, 177);
        btnSave.Margin = new Padding(4, 3, 4, 3);
        btnSave.Name = "btnSave";
        btnSave.Padding = new Padding(6);
        btnSave.Size = new Size(88, 27);
        btnSave.TabIndex = 19;
        btnSave.Text = "Ok";
        btnSave.Click += btnSave_Click;
        // 
        // EventCommand_CastSpellOn
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
        Controls.Add(grpCastSpellOn);
        Name = "EventCommand_CastSpellOn";
        Size = new Size(303, 223);
        grpCastSpellOn.ResumeLayout(false);
        grpCastSpellOn.PerformLayout();
        grpTargets.ResumeLayout(false);
        grpTargets.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private DarkUI.Controls.DarkGroupBox grpCastSpellOn;
    private DarkUI.Controls.DarkCheckBox chkApplyToGuildies;
    private DarkUI.Controls.DarkCheckBox chkApplyToParty;
    private DarkUI.Controls.DarkComboBox cmbSpell;
    private Label lblSpell;
    private DarkUI.Controls.DarkButton btnCancel;
    private DarkUI.Controls.DarkButton btnSave;
    private DarkUI.Controls.DarkGroupBox grpTargets;
    private DarkUI.Controls.DarkCheckBox chkApplyToSelf;
    private DarkUI.Controls.DarkButton darkButton1;
    private DarkUI.Controls.DarkButton darkButton2;
}
