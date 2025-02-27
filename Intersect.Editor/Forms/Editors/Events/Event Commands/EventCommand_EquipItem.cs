using Intersect.Editor.Localization;
using Intersect.Framework.Core.GameObjects.Events.Commands;
using Intersect.Framework.Core.GameObjects.Items;
using Intersect.GameObjects;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands;


public partial class EventCommandEquipItems : UserControl
{

    private readonly FrmEvent mEventEditor;

    private EquipItemCommand mMyCommand;

    public EventCommandEquipItems(EquipItemCommand refCommand, FrmEvent editor)
    {
        InitializeComponent();
        mMyCommand = refCommand;
        mEventEditor = editor;

        InitLocalization();
        cmbItem.Items.Clear();
        cmbItem.Items.AddRange(ItemDescriptor.Names);
        cmbItem.SelectedIndex = ItemDescriptor.ListIndex(mMyCommand.ItemId);
        chkUnequip.Checked = mMyCommand.Unequip;
        optUnequipItem.Enabled = mMyCommand.Unequip;
        optUnequipItem.Checked = mMyCommand.Unequip && mMyCommand.IsItem;
        optUnequipSlot.Enabled = mMyCommand.Unequip;
        optUnequipSlot.Checked = mMyCommand.Unequip && !mMyCommand.IsItem;
        chkTriggerCooldown.Checked = mMyCommand.TriggerCooldown;
    }

    private void InitLocalization()
    {
        grpEquipItem.Text = Strings.EventEquipItems.title;
        chkTriggerCooldown.Text = Strings.EventEquipItems.TriggerCooldown;
        chkUnequip.Text = Strings.EventEquipItems.unequip;
        optUnequipItem.Text = Strings.EventEquipItems.item;
        optUnequipSlot.Text = Strings.EventEquipItems.slot;
        btnSave.Text = Strings.EventEquipItems.okay;
        btnCancel.Text = Strings.EventEquipItems.cancel;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
        mMyCommand.ItemId = optUnequipItem.Checked || !chkUnequip.Checked ? ItemDescriptor.IdFromList(cmbItem.SelectedIndex) : Guid.Empty;
        mMyCommand.Slot = optUnequipSlot.Checked ? cmbItem.SelectedIndex : -1;
        mMyCommand.Unequip = chkUnequip.Checked;
        mMyCommand.IsItem = optUnequipItem.Checked;
        mMyCommand.TriggerCooldown = chkTriggerCooldown.Checked;
        mEventEditor.FinishCommandEdit();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        mEventEditor.CancelCommandEdit();
    }

    private void chkUnequip_CheckedChanged(object sender, EventArgs e)
    {
        if (chkUnequip.Checked)
        {
            chkTriggerCooldown.Checked = false;
            chkTriggerCooldown.Enabled = false;
            optUnequipItem.Checked = mMyCommand.IsItem;
            optUnequipItem.Enabled = true;
            optUnequipSlot.Checked = !mMyCommand.IsItem;
            optUnequipSlot.Enabled = true;
        }
        else
        {
            chkTriggerCooldown.Enabled = true;
            optUnequipItem.Checked = false;
            optUnequipItem.Enabled = false;
            optUnequipSlot.Checked = false;
            optUnequipSlot.Enabled = false;
            UpdateUnequipOptions();
        }
    }

    private void optUnequipSlot_CheckedChanged(object sender, EventArgs e)
    {
        UpdateUnequipOptions();
    }

    private void UpdateUnequipOptions()
    {
        cmbItem.Items.Clear();
        cmbItem.Items.AddRange(optUnequipItem.Checked || !chkUnequip.Checked ? ItemDescriptor.Names : Options.Instance.Equipment.Slots.ToArray());
        cmbItem.SelectedIndex = cmbItem.Items.Count > 0 ? 0 : -1;
        lblItem.Text = optUnequipItem.Checked || !chkUnequip.Checked ? Strings.EventEquipItems.item : Strings.EventEquipItems.slot;
    }
}
