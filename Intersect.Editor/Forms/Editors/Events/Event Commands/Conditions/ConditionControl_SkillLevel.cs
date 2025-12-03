using System;
using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Conditions.ConditionMetadata;
using Intersect.Framework.Core.GameObjects.Events;
using Intersect.Framework.Core.GameObjects.Skills;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

public partial class ConditionControl_SkillLevel : UserControl
{
    public ConditionControl_SkillLevel()
    {
        InitializeComponent();
        InitLocalization();
    }

    public void InitLocalization()
    {
        grpSkillLevel.Text = Strings.EventConditional.skilllevel;
        lblSkill.Text = Strings.EventConditional.skill;
        lblSkillValue.Text = Strings.EventConditional.skilllevelvalue;
        lblSkillComparator.Text = Strings.EventConditional.comparator;
        
        cmbSkill.Items.Clear();
        cmbSkill.Items.Add(Strings.General.None);
        cmbSkill.Items.AddRange(SkillDescriptor.Names);
        
        cmbSkillComparator.Items.Clear();
        cmbSkillComparator.Items.AddRange(Strings.EventConditional.comparators.Values.ToArray());
    }

    public void SetupFormValues(SkillLevelCondition condition)
    {
        cmbSkillComparator.SelectedIndex = (int)condition.Comparator;
        nudSkillValue.Value = condition.Value;
        
        var skillIndex = SkillDescriptor.ListIndex(condition.SkillId);
        if (skillIndex > -1)
        {
            cmbSkill.SelectedIndex = skillIndex + 1;
        }
        else
        {
            cmbSkill.SelectedIndex = 0;
        }
    }

    public void SaveFormValues(SkillLevelCondition condition)
    {
        condition.Comparator = (VariableComparator)cmbSkillComparator.SelectedIndex;
        condition.Value = (int)nudSkillValue.Value;
        
        if (cmbSkill.SelectedIndex > 0)
        {
            condition.SkillId = SkillDescriptor.IdFromList(cmbSkill.SelectedIndex - 1);
        }
        else
        {
            condition.SkillId = Guid.Empty;
        }
    }
}

