using Intersect.Editor.Localization;
using Intersect.Framework.Core.GameObjects.Conditions.ConditionMetadata;
using Intersect.Framework.Core.GameObjects.Items;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

public partial class ConditionControl_EquippedItem : UserControl
{
    public ConditionControl_EquippedItem()
    {
        InitializeComponent();
        InitLocalization();
    }

    public void InitLocalization()
    {
        grpEquippedItem.Text = Strings.EventConditional.hasitemequipped;
        lblEquippedItem.Text = Strings.EventConditional.item;
    }

    public void SetupFormValues(IsItemEquippedCondition condition)
    {
        cmbEquippedItem.SelectedIndex = ItemDescriptor.ListIndex(condition.ItemId);

        if (cmbEquippedItem.SelectedIndex == -1 && cmbEquippedItem.Items.Count > 0)
        {
            cmbEquippedItem.SelectedIndex = 0;
        }
    }

    public void SaveFormValues(IsItemEquippedCondition condition)
    {
        condition.ItemId = ItemDescriptor.IdFromList(cmbEquippedItem.SelectedIndex);
    }

    public new void Show()
    {
        cmbEquippedItem.Items.Clear();
        cmbEquippedItem.Items.AddRange(ItemDescriptor.Names);
        base.Show();
    }
}
