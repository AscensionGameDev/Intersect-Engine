using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Conditions.ConditionMetadata;
using Intersect.Framework.Core.GameObjects.Conditions;
using Intersect.Framework.Core.GameObjects.Events;
using Intersect.Framework.Core.GameObjects.Items;
using Intersect.Framework.Core.GameObjects.Variables;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands.Conditions;

public partial class ConditionControl_HasItemOrSlots : UserControl
{
    private readonly EventCommandConditionalBranch _root;

    public ConditionControl_HasItemOrSlots(EventCommandConditionalBranch root)
    {
        InitializeComponent();
        InitLocalization();
        _root = root;
    }

    public void InitLocalization()
    {
        grpInventoryConditions.Text = Strings.EventConditional.hasitem;
        lblItemQuantity.Text = Strings.EventConditional.hasatleast;
        lblItem.Text = Strings.EventConditional.item;
        lblInvVariable.Text = Strings.EventConditional.VariableLabel;
        grpAmountType.Text = Strings.EventConditional.AmountType;
        rdoManual.Text = Strings.EventConditional.Manual;
        rdoVariable.Text = Strings.EventConditional.VariableLabel;
        grpManualAmount.Text = Strings.EventConditional.Manual;
        grpVariableAmount.Text = Strings.EventConditional.VariableLabel;
        rdoInvPlayerVariable.Text = Strings.EventConditional.playervariable;
        rdoInvGlobalVariable.Text = Strings.EventConditional.globalvariable;
        rdoInvGuildVariable.Text = Strings.EventConditional.guildvariable;
        chkBank.Text = Strings.EventConditional.CheckBank;
    }

    public void SetupFormValues(HasItemCondition condition)
    {
        cmbItem.SelectedIndex = ItemDescriptor.ListIndex(condition.ItemId);
        nudItemAmount.Value = Math.Max(1, condition.Quantity);
        rdoManual.Checked = !condition.UseVariable;
        rdoVariable.Checked = condition.UseVariable;
        rdoInvGlobalVariable.Checked = condition.VariableType == VariableType.ServerVariable;
        chkBank.Checked = condition.CheckBank;
        rdoInvGuildVariable.Checked = condition.VariableType == VariableType.GuildVariable;
        SetupAmountInput();
    }

    public void SaveFormValues(HasItemCondition condition)
    {
        condition.ItemId = ItemDescriptor.IdFromList(cmbItem.SelectedIndex);
        condition.Quantity = (int)nudItemAmount.Value;
        if (rdoInvPlayerVariable.Checked)
        {
            condition.VariableType = VariableType.PlayerVariable;
            condition.VariableId = PlayerVariableDescriptor.IdFromList(cmbInvVariable.SelectedIndex, VariableDataType.Integer);
        }
        else if (rdoInvGlobalVariable.Checked)
        {
            condition.VariableType = VariableType.ServerVariable;
            condition.VariableId = ServerVariableDescriptor.IdFromList(cmbInvVariable.SelectedIndex, VariableDataType.Integer);
        }
        else if (rdoInvGuildVariable.Checked)
        {
            condition.VariableType = VariableType.GuildVariable;
            condition.VariableId = GuildVariableDescriptor.IdFromList(cmbInvVariable.SelectedIndex, VariableDataType.Integer);
        }
        condition.UseVariable = !rdoManual.Checked;
        condition.CheckBank = chkBank.Checked;
    }

    public void SetupFormValues(HasFreeInventorySlots condition)
    {
        nudItemAmount.Value = condition.Quantity;
        rdoManual.Checked = !condition.UseVariable;
        rdoVariable.Checked = condition.UseVariable;
        rdoInvGlobalVariable.Checked = condition.VariableType == VariableType.ServerVariable;
        rdoInvGuildVariable.Checked = condition.VariableType == VariableType.GuildVariable;
        SetupAmountInput();
    }

    public void SaveFormValues(HasFreeInventorySlots condition)
    {
        condition.Quantity = (int)nudItemAmount.Value;
        if (rdoInvPlayerVariable.Checked)
        {
            condition.VariableType = VariableType.PlayerVariable;
            condition.VariableId = PlayerVariableDescriptor.IdFromList(cmbInvVariable.SelectedIndex, VariableDataType.Integer);
        }
        else if (rdoInvGlobalVariable.Checked)
        {
            condition.VariableType = VariableType.ServerVariable;
            condition.VariableId = ServerVariableDescriptor.IdFromList(cmbInvVariable.SelectedIndex, VariableDataType.Integer);
        }
        else if (rdoInvGuildVariable.Checked)
        {
            condition.VariableType = VariableType.GuildVariable;
            condition.VariableId = GuildVariableDescriptor.IdFromList(cmbInvVariable.SelectedIndex, VariableDataType.Integer);
        }
        condition.UseVariable = !rdoManual.Checked;
    }

    public void ShowHasItem()
    {
        grpInventoryConditions.Text = Strings.EventConditional.hasitem;
        lblItem.Visible = true;
        cmbItem.Visible = true;
        chkBank.Visible = true;
        cmbItem.Items.Clear();
        cmbItem.Items.AddRange(ItemDescriptor.Names);
        SetupAmountInput();
        Show();
    }

    public void ShowHasFreeInventorySlots()
    {
        grpInventoryConditions.Text = Strings.EventConditional.FreeInventorySlots;
        lblItem.Visible = false;
        cmbItem.Visible = false;
        chkBank.Visible = false;
        cmbItem.Items.Clear();
        SetupAmountInput();
        Show();
    }

    private void SetupAmountInput()
    {
        grpManualAmount.Visible = rdoManual.Checked;
        grpVariableAmount.Visible = !rdoManual.Checked;

        VariableType conditionVariableType;
        Guid conditionVariableId;
        int ConditionQuantity;

        switch (_root.Condition.Type)
        {
            case ConditionType.HasFreeInventorySlots:
                conditionVariableType = ((HasFreeInventorySlots)_root.Condition).VariableType;
                conditionVariableId = ((HasFreeInventorySlots)_root.Condition).VariableId;
                ConditionQuantity = ((HasFreeInventorySlots)_root.Condition).Quantity;
                break;
            case ConditionType.HasItem:
                conditionVariableType = ((HasItemCondition)_root.Condition).VariableType;
                conditionVariableId = ((HasItemCondition)_root.Condition).VariableId;
                ConditionQuantity = ((HasItemCondition)_root.Condition).Quantity;
                break;
            default:
                conditionVariableType = VariableType.PlayerVariable;
                conditionVariableId = Guid.Empty;
                ConditionQuantity = 0;
                return;
        }

        cmbInvVariable.Items.Clear();
        if (rdoInvPlayerVariable.Checked)
        {
            cmbInvVariable.Items.AddRange(PlayerVariableDescriptor.GetNamesByType(VariableDataType.Integer));
            // Do not update if the wrong type of variable is saved
            if (conditionVariableType == VariableType.PlayerVariable)
            {
                var index = PlayerVariableDescriptor.ListIndex(conditionVariableId, VariableDataType.Integer);
                if (index > -1)
                {
                    cmbInvVariable.SelectedIndex = index;
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
        else if (rdoInvGlobalVariable.Checked)
        {
            cmbInvVariable.Items.AddRange(ServerVariableDescriptor.GetNamesByType(VariableDataType.Integer));
            // Do not update if the wrong type of variable is saved
            if (conditionVariableType == VariableType.ServerVariable)
            {
                var index = ServerVariableDescriptor.ListIndex(conditionVariableId, VariableDataType.Integer);
                if (index > -1)
                {
                    cmbInvVariable.SelectedIndex = index;
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
        else if (rdoInvGuildVariable.Checked)
        {
            cmbInvVariable.Items.AddRange(GuildVariableDescriptor.GetNamesByType(VariableDataType.Integer));
            // Do not update if the wrong type of variable is saved
            if (conditionVariableType == VariableType.GuildVariable)
            {
                var index = GuildVariableDescriptor.ListIndex(conditionVariableId, VariableDataType.Integer);
                if (index > -1)
                {
                    cmbInvVariable.SelectedIndex = index;
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

        nudItemAmount.Value = Math.Max(1, ConditionQuantity);
    }

    private void VariableBlank()
    {
        if (cmbInvVariable.Items.Count > 0)
        {
            cmbInvVariable.SelectedIndex = 0;
        }
        else
        {
            cmbInvVariable.SelectedIndex = -1;
            cmbInvVariable.Text = string.Empty;
        }
    }

    private void NudItemAmount_ValueChanged(object sender, System.EventArgs e)
    {
        nudItemAmount.Value = Math.Max(1, nudItemAmount.Value);
    }

    private void rdoManual_CheckedChanged(object sender, EventArgs e)
    {
        SetupAmountInput();
    }

    private void rdoVariable_CheckedChanged(object sender, EventArgs e)
    {
        SetupAmountInput();
    }

    private void rdoInvPlayerVariable_CheckedChanged(object sender, EventArgs e)
    {
        SetupAmountInput();
    }

    private void rdoInvGlobalVariable_CheckedChanged(object sender, EventArgs e)
    {
        SetupAmountInput();
    }

    private void rdoInvGuildVariable_CheckedChanged(object sender, EventArgs e)
    {
        SetupAmountInput();
    }
}
