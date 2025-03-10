using Intersect.Editor.Localization;
using Intersect.Framework.Core.GameObjects.Conditions.ConditionMetadata;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

public partial class ConditionControl_EquippedSlot : UserControl
{
    public ConditionControl_EquippedSlot()
    {
        InitializeComponent();
        InitLocalization();
    }

    public void InitLocalization()
    {
        grpCheckEquippedSlot.Text = Strings.EventConditional.CheckEquipment;
        lblCheckEquippedSlot.Text = Strings.EventConditional.EquipmentSlot;
    }

    public void SetupFormValues(CheckEquippedSlot condition)
    {
        cmbCheckEquippedSlot.SelectedIndex = Options.Instance.Equipment.Slots.IndexOf(condition.Name);

        if (cmbCheckEquippedSlot.SelectedIndex == -1 && cmbCheckEquippedSlot.Items.Count > 0)
        {
            cmbCheckEquippedSlot.SelectedIndex = 0;
        }
    }

    public void SaveFormValues(CheckEquippedSlot condition)
    {
        condition.Name = Options.Instance.Equipment.Slots[cmbCheckEquippedSlot.SelectedIndex];
    }

    public new void Show()
    {
        cmbCheckEquippedSlot.Items.Clear();
        foreach (var slot in Options.Instance.Equipment.Slots)
        {
            cmbCheckEquippedSlot.Items.Add(slot);
        }

        base.Show();
    }
}
