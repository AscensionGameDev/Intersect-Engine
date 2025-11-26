using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Events;
using Intersect.Framework.Core.GameObjects.Events.Commands;
using Intersect.Framework.Core.GameObjects.Skills;
using Intersect.Framework.Core.GameObjects.Variables;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands;

public partial class EventCommandGiveSkillExperience : UserControl
{
    private readonly FrmEvent _eventEditor;

    private readonly GiveSkillExperienceCommand _command;

    public EventCommandGiveSkillExperience(GiveSkillExperienceCommand refCommand, FrmEvent editor)
    {
        InitializeComponent();
        _command = refCommand;
        _eventEditor = editor;
        InitLocalization();

        rdoVariable.Checked = _command.UseVariable;
        rdoGlobalVariable.Checked = _command.VariableType == VariableType.ServerVariable;
        rdoGuildVariable.Checked = _command.VariableType == VariableType.GuildVariable;

        nudExperience.Minimum = -long.MaxValue;
        nudExperience.Maximum = long.MaxValue;

        chkEnableLevelDown.Checked = _command.EnableLosingLevels;

        // Populate skill combo box
        cmbSkill.Items.Clear();
        cmbSkill.Items.Add(Strings.General.None);
        cmbSkill.Items.AddRange(SkillDescriptor.Names);
        
        var skillIndex = SkillDescriptor.ListIndex(_command.SkillId);
        if (skillIndex > -1)
        {
            cmbSkill.SelectedIndex = skillIndex + 1;
        }
        else
        {
            cmbSkill.SelectedIndex = 0;
        }

        SetupAmountInput(default, default);
    }

    private void InitLocalization()
    {
        grpGiveSkillExperience.Text = "Give Skill Experience";
        lblSkill.Text = "Skill:";
        lblExperience.Text = "Experience:";
        lblVariable.Text = "Variable:";

        grpAmountType.Text = "Amount Type";
        rdoManual.Text = "Manual";
        rdoVariable.Text = "Variable";

        grpManualAmount.Text = "Manual";
        grpVariableAmount.Text = "Variable";

        rdoPlayerVariable.Text = "Player Variable";
        rdoGlobalVariable.Text = "Server Variable";
        rdoGuildVariable.Text = "Guild Variable";

        chkEnableLevelDown.Text = "Enable Losing Levels";

        btnSave.Text = Strings.General.Okay;
        btnCancel.Text = Strings.General.Cancel;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
        if (cmbSkill.SelectedIndex > 0)
        {
            _command.SkillId = SkillDescriptor.IdFromList(cmbSkill.SelectedIndex - 1);
        }
        else
        {
            _command.SkillId = Guid.Empty;
        }

        _command.Exp = (long)nudExperience.Value;

        if (rdoPlayerVariable.Checked)
        {
            _command.VariableType = VariableType.PlayerVariable;
            _command.VariableId = PlayerVariableDescriptor.IdFromList(cmbVariable.SelectedIndex, VariableDataType.Integer);
        }
        else if (rdoGlobalVariable.Checked)
        {
            _command.VariableType = VariableType.ServerVariable;
            _command.VariableId = ServerVariableDescriptor.IdFromList(cmbVariable.SelectedIndex, VariableDataType.Integer);
        }
        else if (rdoGuildVariable.Checked)
        {
            _command.VariableType = VariableType.GuildVariable;
            _command.VariableId = GuildVariableDescriptor.IdFromList(cmbVariable.SelectedIndex, VariableDataType.Integer);
        }

        _command.UseVariable = !rdoManual.Checked;
        _command.EnableLosingLevels = chkEnableLevelDown.Checked;
        _eventEditor.FinishCommandEdit();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        _eventEditor.CancelCommandEdit();
    }

    private void VariableBlank()
    {
        if (cmbVariable.Items.Count > 0)
        {
            cmbVariable.SelectedIndex = 0;
        }
        else
        {
            cmbVariable.SelectedIndex = -1;
            cmbVariable.Text = string.Empty;
        }
    }

    private void SetupAmountInput(object? sender, EventArgs? e)
    {
        grpManualAmount.Visible = rdoManual.Checked;
        grpVariableAmount.Visible = !rdoManual.Checked;

        cmbVariable.Items.Clear();
        if (rdoPlayerVariable.Checked)
        {
            cmbVariable.Items.AddRange(PlayerVariableDescriptor.GetNamesByType(VariableDataType.Integer));
            if (_command.VariableType == VariableType.PlayerVariable)
            {
                var index = PlayerVariableDescriptor.ListIndex(_command.VariableId, VariableDataType.Integer);
                if (index > -1)
                {
                    cmbVariable.SelectedIndex = index;
                }
                else
                {
                    VariableBlank();
                }
            }
            else
            {
                VariableBlank();
            }
        }
        else if (rdoGlobalVariable.Checked)
        {
            cmbVariable.Items.AddRange(ServerVariableDescriptor.GetNamesByType(VariableDataType.Integer));
            if (_command.VariableType == VariableType.ServerVariable)
            {
                var index = ServerVariableDescriptor.ListIndex(_command.VariableId, VariableDataType.Integer);
                if (index > -1)
                {
                    cmbVariable.SelectedIndex = index;
                }
                else
                {
                    VariableBlank();
                }
            }
            else
            {
                VariableBlank();
            }
        }
        else if (rdoGuildVariable.Checked)
        {
            cmbVariable.Items.AddRange(GuildVariableDescriptor.GetNamesByType(VariableDataType.Integer));
            if (_command.VariableType == VariableType.GuildVariable)
            {
                var index = GuildVariableDescriptor.ListIndex(_command.VariableId, VariableDataType.Integer);
                if (index > -1)
                {
                    cmbVariable.SelectedIndex = index;
                }
                else
                {
                    VariableBlank();
                }
            }
            else
            {
                VariableBlank();
            }
        }

        nudExperience.Value = _command.Exp;
    }
}

