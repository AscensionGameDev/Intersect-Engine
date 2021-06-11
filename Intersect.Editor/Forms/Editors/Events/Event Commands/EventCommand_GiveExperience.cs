using System;
using System.Windows.Forms;

using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events.Commands;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{

    public partial class EventCommandGiveExperience : UserControl
    {

        private readonly FrmEvent mEventEditor;

        private GiveExperienceCommand mMyCommand;

        public EventCommandGiveExperience(GiveExperienceCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            InitLocalization();

            rdoVariable.Checked = mMyCommand.UseVariable;
            rdoGlobalVariable.Checked = mMyCommand.VariableType == VariableTypes.ServerVariable;
            rdoGuildVariable.Checked = mMyCommand.VariableType == VariableTypes.GuildVariable;

            SetupAmountInput();
        }

        private void InitLocalization()
        {
            grpGiveExperience.Text = Strings.EventGiveExperience.title;
            lblExperience.Text = Strings.EventGiveExperience.label;

            lblVariable.Text = Strings.EventGiveExperience.Variable;

            grpAmountType.Text = Strings.EventGiveExperience.AmountType;
            rdoManual.Text = Strings.EventGiveExperience.Manual;
            rdoVariable.Text = Strings.EventGiveExperience.Variable;

            grpManualAmount.Text = Strings.EventGiveExperience.Manual;
            grpVariableAmount.Text = Strings.EventGiveExperience.Variable;

            rdoPlayerVariable.Text = Strings.EventGiveExperience.PlayerVariable;
            rdoGlobalVariable.Text = Strings.EventGiveExperience.ServerVariable;
            rdoGuildVariable.Text = Strings.EventGiveExperience.GuildVariable;

            btnSave.Text = Strings.EventGiveExperience.okay;
            btnCancel.Text = Strings.EventGiveExperience.cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.Exp = (long) nudExperience.Value;
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

            nudExperience.Value = Math.Max(1, mMyCommand.Exp);
        }
    }
}
