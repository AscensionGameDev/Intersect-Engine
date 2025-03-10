using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Conditions.ConditionMetadata;
using Intersect.Framework.Core.GameObjects.Events;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

public partial class ConditionControl_PlayerStat : UserControl
{
    public ConditionControl_PlayerStat()
    {
        InitializeComponent();
        InitLocalization();
    }

    public void InitLocalization()
    {
        grpLevelStat.Text = Strings.EventConditional.levelorstat;
        lblLvlStatValue.Text = Strings.EventConditional.levelstatvalue;
        lblLevelComparator.Text = Strings.EventConditional.comparator;
        lblLevelOrStat.Text = Strings.EventConditional.levelstatitem;
        cmbLevelStat.Items.Clear();
        cmbLevelStat.Items.Add(Strings.EventConditional.level);
        for (var i = 0; i < Enum.GetValues<Stat>().Length; i++)
        {
            cmbLevelStat.Items.Add(Strings.Combat.stats[i]);
        }

        cmbLevelComparator.Items.Clear();
        for (var i = 0; i < Strings.EventConditional.comparators.Count; i++)
        {
            cmbLevelComparator.Items.Add(Strings.EventConditional.comparators[i]);
        }

        chkStatIgnoreBuffs.Text = Strings.EventConditional.ignorestatbuffs;
    }

    public void SetupFormValues(LevelOrStatCondition condition)
    {
        cmbLevelComparator.SelectedIndex = (int)condition.Comparator;
        nudLevelStatValue.Value = condition.Value;
        cmbLevelStat.SelectedIndex = condition.ComparingLevel ? 0 : (int)condition.Stat + 1;
        chkStatIgnoreBuffs.Checked = condition.IgnoreBuffs;
    }

    public void SaveFormValues(LevelOrStatCondition condition)
    {
        condition.Comparator = (VariableComparator)cmbLevelComparator.SelectedIndex;
        condition.Value = (int)nudLevelStatValue.Value;
        condition.ComparingLevel = cmbLevelStat.SelectedIndex == 0;
        condition.IgnoreBuffs = chkStatIgnoreBuffs.Checked;

        if (!condition.ComparingLevel)
        {
            condition.Stat = (Stat)(cmbLevelStat.SelectedIndex - 1);
        }
    }
}
