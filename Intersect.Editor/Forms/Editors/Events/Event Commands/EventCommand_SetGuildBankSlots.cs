using System;
using System.Windows.Forms;

using Intersect.Enums;
using Intersect.Editor.Localization;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Events.Commands;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{

    public partial class EventCommandSetGuildBankSlots : UserControl
    {

        private readonly FrmEvent mEventEditor;

        private EventPage mCurrentPage;

        private SetGuildBankSlotsCommand mMyCommand;

        public EventCommandSetGuildBankSlots(SetGuildBankSlotsCommand refCommand, EventPage refPage, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            mCurrentPage = refPage;
            InitLocalization();

            rdoGlobalVariable.Checked = mMyCommand.VariableType == VariableTypes.ServerVariable;
            rdoGuildVariable.Checked = mMyCommand.VariableType == VariableTypes.GuildVariable;

            SetupAmountInput();
        }

        private void InitLocalization()
        {
            grpGuildSlots.Text = Strings.EventGuildSetBankSlotsCount.title;
            lblVariable.Text = Strings.EventGuildSetBankSlotsCount.Variable;

            rdoPlayerVariable.Text = Strings.EventGuildSetBankSlotsCount.PlayerVariable;
            rdoGlobalVariable.Text = Strings.EventGuildSetBankSlotsCount.ServerVariable;
            rdoGuildVariable.Text = Strings.EventGuildSetBankSlotsCount.GuildVariable;

            btnSave.Text = Strings.EventGuildSetBankSlotsCount.okay;
            btnCancel.Text = Strings.EventGuildSetBankSlotsCount.cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (rdoPlayerVariable.Checked)
            {
                mMyCommand.VariableType = VariableTypes.PlayerVariable;
                mMyCommand.VariableId = PlayerVariableBase.IdFromList(cmbVariable.SelectedIndex, VariableDataTypes.Integer);
            }
            else if (rdoGlobalVariable.Checked)
            {
                mMyCommand.VariableType = VariableTypes.ServerVariable;
                mMyCommand.VariableId = ServerVariableBase.IdFromList(cmbVariable.SelectedIndex, VariableDataTypes.Integer);
            }
            else if (rdoGuildVariable.Checked)
            {
                mMyCommand.VariableType = VariableTypes.GuildVariable;
                mMyCommand.VariableId = GuildVariableBase.IdFromList(cmbVariable.SelectedIndex, VariableDataTypes.Integer);
            }

            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
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

        private void SetupAmountInput()
        {

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
            if (rdoGuildVariable.Checked)
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
        }
    }

}
