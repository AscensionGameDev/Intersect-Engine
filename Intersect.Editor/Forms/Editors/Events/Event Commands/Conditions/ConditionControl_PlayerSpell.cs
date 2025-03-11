using Intersect.Editor.Localization;
using Intersect.Framework.Core.GameObjects.Conditions.ConditionMetadata;
using Intersect.GameObjects;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

public partial class ConditionControl_PlayerSpell : UserControl
{
    public ConditionControl_PlayerSpell()
    {
        InitializeComponent();
        InitLocalization();
    }

    public void InitLocalization()
    {
        grpSpell.Text = Strings.EventConditional.knowsspell;
        lblSpell.Text = Strings.EventConditional.spell;
    }

    public void SetupFormValues(KnowsSpellCondition condition)
    {
        cmbSpell.SelectedIndex = SpellDescriptor.ListIndex(condition.SpellId);

        if (cmbSpell.SelectedIndex == -1 && cmbSpell.Items.Count > 0)
        {
            cmbSpell.SelectedIndex = 0;
        }
    }

    public void SaveFormValues(KnowsSpellCondition condition)
    {
        condition.SpellId = SpellDescriptor.IdFromList(cmbSpell.SelectedIndex);
    }

    public new void Show()
    {
        cmbSpell.Items.Clear();
        cmbSpell.Items.AddRange(SpellDescriptor.Names);
        base.Show();
    }
}
