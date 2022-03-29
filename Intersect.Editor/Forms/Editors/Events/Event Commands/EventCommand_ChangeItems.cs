using System;
using System.Windows.Forms;

using Intersect.Enums;
using Intersect.Editor.Localization;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Events.Commands;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{

    public partial class EventCommandChangeItems : UserControl
    {

        private readonly FrmEvent mEventEditor;

        private EventPage mCurrentPage;

        private ChangeItemsCommand mMyCommand;

        public EventCommandChangeItems(ChangeItemsCommand refCommand, EventPage refPage, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            mCurrentPage = refPage;
            InitLocalization();
            cmbItem.Items.Clear();
            cmbItem.Items.AddRange(ItemBase.Names);
            cmbAction.SelectedIndex = mMyCommand.Add ? 0 : 1;
            cmbItem.SelectedIndex = ItemBase.ListIndex(mMyCommand.ItemId);
            cmbMethod.SelectedIndex = (int)mMyCommand.ItemHandling;

            rdoVariable.Checked = mMyCommand.UseVariable;
            rdoGlobalVariable.Checked = mMyCommand.VariableType == VariableTypes.ServerVariable;
            rdoGuildVariable.Checked = mMyCommand.VariableType == VariableTypes.GuildVariable;

            SetupAmountInput();
        }

        private void InitLocalization()
        {
            grpChangeItems.Text = Strings.EventChangeItems.title;
            lblAction.Text = Strings.EventChangeItems.action;
            cmbAction.Items.Clear();
            for (var i = 0; i < Strings.EventChangeItems.actions.Count; i++)
            {
                cmbAction.Items.Add(Strings.EventChangeItems.actions[i]);
            }

            lblMethod.Text = Strings.EventChangeItems.Method;
            cmbMethod.Items.Clear();
            for (var i = 0; i < Strings.EventChangeItems.Methods.Count; i++)
            {
                cmbMethod.Items.Add(Strings.EventChangeItems.Methods[i]);
            }

            lblAmount.Text = Strings.EventChangeItems.amount;
            lblVariable.Text = Strings.EventChangeItems.Variable;

            grpAmountType.Text = Strings.EventChangeItems.AmountType;
            rdoManual.Text = Strings.EventChangeItems.Manual;
            rdoVariable.Text = Strings.EventChangeItems.Variable;

            grpManualAmount.Text = Strings.EventChangeItems.Manual;
            grpVariableAmount.Text = Strings.EventChangeItems.Variable;

            rdoPlayerVariable.Text = Strings.EventChangeItems.PlayerVariable;
            rdoGlobalVariable.Text = Strings.EventChangeItems.ServerVariable;
            rdoGuildVariable.Text = Strings.EventChangeItems.GuildVariable;

            btnSave.Text = Strings.EventChangeItems.okay;
            btnCancel.Text = Strings.EventChangeItems.cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.Add = !Convert.ToBoolean(cmbAction.SelectedIndex);
            mMyCommand.ItemId = ItemBase.IdFromList(cmbItem.SelectedIndex);
            if (rdoPlayerVariable.Checked)
            {
                mMyCommand.VariableType = VariableTypes.PlayerVariable;
                mMyCommand.VariableId = PlayerVariableBase.IdFromList(cmbVariable.SelectedIndex, VariableDataTypes.Integer);
            }
            else if (rdoGlobalVariable.Checked)
            {
                mMyCommand.VariableType = VariableTypes.PlayerVariable;
                mMyCommand.VariableId = ServerVariableBase.IdFromList(cmbVariable.SelectedIndex, VariableDataTypes.Integer);
            }
            else if (rdoGuildVariable.Checked)
            {
                mMyCommand.VariableType = VariableTypes.GuildVariable;
                mMyCommand.VariableId = GuildVariableBase.IdFromList(cmbVariable.SelectedIndex, VariableDataTypes.Integer);
            }
            mMyCommand.UseVariable = !rdoManual.Checked;

            mMyCommand.Quantity = (int) nudGiveTakeAmount.Value;
            mMyCommand.ItemHandling = (ItemHandling) cmbMethod.SelectedIndex;
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }

        private void nudGiveTakeAmount_ValueChanged(object sender, EventArgs e)
        {
            // This should never be below 1. We shouldn't accept giving or taking away 0 items!
            nudGiveTakeAmount.Value = Math.Max(1, nudGiveTakeAmount.Value);
        }

        private void rdoManual_CheckedChanged(object sender, EventArgs e)
        {
            SetupAmountInput();
        }

        private void rdoPlayerVariable_CheckedChanged(object sender, EventArgs e)
        {
            SetupAmountInput();
        }

        private void rdoGlobalVariable_CheckedChanged(object sender, EventArgs e)
        {
            SetupAmountInput();
        }

        private void rdoGuildVariable_CheckedChanged(object sender, EventArgs e)
        {
            SetupAmountInput();
        }

        private void rdoVariable_CheckedChanged(object sender, EventArgs e)
        {
            SetupAmountInput();
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
                cmbVariable.Text = "";
            }
        }

        private void SetupAmountInput()
        {
            grpManualAmount.Visible = rdoManual.Checked;
            grpVariableAmount.Visible = !rdoManual.Checked;

            cmbVariable.Items.Clear();
            if (rdoPlayerVariable.Checked)
            {
                cmbVariable.Items.AddRange(PlayerVariableBase.GetNamesByType(VariableDataTypes.Integer));
                // Do not update if the wrong type of variable is saved
                if (mMyCommand.VariableType == VariableTypes.PlayerVariable)
                {
                    var index = PlayerVariableBase.ListIndex(mMyCommand.VariableId, VariableDataTypes.Integer);
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
                cmbVariable.Items.AddRange(ServerVariableBase.GetNamesByType(VariableDataTypes.Integer));
                // Do not update if the wrong type of variable is saved
                if (mMyCommand.VariableType == VariableTypes.ServerVariable)
                {
                    var index = ServerVariableBase.ListIndex(mMyCommand.VariableId, VariableDataTypes.Integer);
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
                cmbVariable.Items.AddRange(GuildVariableBase.GetNamesByType(VariableDataTypes.Integer));
                // Do not update if the wrong type of variable is saved
                if (mMyCommand.VariableType == VariableTypes.GuildVariable)
                {
                    var index = GuildVariableBase.ListIndex(mMyCommand.VariableId, VariableDataTypes.Integer);
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

            nudGiveTakeAmount.Value = Math.Max(1, mMyCommand.Quantity);
        }
    }

}
