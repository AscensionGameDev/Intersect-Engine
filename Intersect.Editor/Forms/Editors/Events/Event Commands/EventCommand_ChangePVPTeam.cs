using System;
using System.Windows.Forms;

using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events.Commands;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    public partial class EventCommandChangePVPTeam : UserControl
    {

        private readonly FrmEvent mEventEditor;

        private SetPVPTeamCommand mMyCommand;


        public EventCommandChangePVPTeam(SetPVPTeamCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            InitLocalization();

            rdoVariable.Checked = mMyCommand.UseVariable;
            rdoGlobalVariable.Checked = mMyCommand.VariableType == VariableType.ServerVariable;
            rdoGuildVariable.Checked = mMyCommand.VariableType == VariableType.GuildVariable;

            SetupAmountInput();
        }

        private void InitLocalization()
        {
            grpSetPVPTeam.Text = Strings.EventSetPVPTeam.title;
            lblPVPTeam.Text = Strings.EventSetPVPTeam.label;

            lblVariable.Text = Strings.EventSetPVPTeam.Variable;

            grpAmountType.Text = Strings.EventSetPVPTeam.AmountType;
            rdoManual.Text = Strings.EventSetPVPTeam.Manual;
            rdoVariable.Text = Strings.EventSetPVPTeam.Variable;

            grpManualAmount.Text = Strings.EventSetPVPTeam.Manual;
            grpVariableAmount.Text = Strings.EventSetPVPTeam.Variable;

            rdoPlayerVariable.Text = Strings.EventSetPVPTeam.PlayerVariable;
            rdoGlobalVariable.Text = Strings.EventSetPVPTeam.ServerVariable;
            rdoGuildVariable.Text = Strings.EventSetPVPTeam.GuildVariable;

            btnSave.Text = Strings.EventSetPVPTeam.okay;
            btnCancel.Text = Strings.EventSetPVPTeam.cancel;
        }

        private void GrpSetPvpTeamEnter(object sender, EventArgs e)
        {

        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.PVPTeamID = (int)nudTeamID.Value;
            if (rdoPlayerVariable.Checked)
            {
                mMyCommand.VariableType = VariableType.PlayerVariable;
                mMyCommand.VariableId = PlayerVariableBase.IdFromList(cmbVariable.SelectedIndex, VariableDataType.Integer);
            }
            else if (rdoGlobalVariable.Checked)
            {
                mMyCommand.VariableType = VariableType.ServerVariable;
                mMyCommand.VariableId = ServerVariableBase.IdFromList(cmbVariable.SelectedIndex, VariableDataType.Integer);
            }
            else if (rdoGuildVariable.Checked)
            {
                mMyCommand.VariableType = VariableType.GuildVariable;
                mMyCommand.VariableId = GuildVariableBase.IdFromList(cmbVariable.SelectedIndex, VariableDataType.Integer);
            }
            mMyCommand.UseVariable = !rdoManual.Checked;
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }

        private void rdoManual_CheckedChanged(object sender, EventArgs e)
        {
            SetupAmountInput();
        }

        private void rdoVariable_CheckedChanged(object sender, EventArgs e)
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
        private void SetupAmountInput()
        {
            grpManualAmount.Visible = rdoManual.Checked;
            grpVariableAmount.Visible = !rdoManual.Checked;

            cmbVariable.Items.Clear();
            if (rdoPlayerVariable.Checked)
            {
                cmbVariable.Items.AddRange(PlayerVariableBase.GetNamesByType(VariableDataType.Integer));
                // Do not update if the wrong type of variable is saved
                if (mMyCommand.VariableType == VariableType.PlayerVariable)
                {
                    var index = PlayerVariableBase.ListIndex(mMyCommand.VariableId, VariableDataType.Integer);
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
                cmbVariable.Items.AddRange(ServerVariableBase.GetNamesByType(VariableDataType.Integer));
                // Do not update if the wrong type of variable is saved
                if (mMyCommand.VariableType == VariableType.ServerVariable)
                {
                    var index = ServerVariableBase.ListIndex(mMyCommand.VariableId, VariableDataType.Integer);
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
                cmbVariable.Items.AddRange(GuildVariableBase.GetNamesByType(VariableDataType.Integer));
                // Do not update if the wrong type of variable is saved
                if (mMyCommand.VariableType == VariableType.GuildVariable)
                {
                    var index = GuildVariableBase.ListIndex(mMyCommand.VariableId, VariableDataType.Integer);
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

            nudTeamID.Value = Math.Clamp(mMyCommand.PVPTeamID, - 1, 1000);
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
    }
}