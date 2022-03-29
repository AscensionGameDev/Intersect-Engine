using System;
using System.Windows.Forms;

using Intersect.Editor.Localization;
using Intersect.GameObjects;
using Intersect.GameObjects.Events.Commands;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{

    public partial class EventCommandInput : UserControl
    {

        private readonly FrmEvent mEventEditor;

        bool mLoading = false;

        private InputVariableCommand mMyCommand;

        public EventCommandInput(InputVariableCommand refCommand, FrmEvent editor)
        {
            mLoading = true;
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;

            txtText.Text = mMyCommand.Text;
            txtTitle.Text = mMyCommand.Title;
            nudMaxVal.Value = mMyCommand.Maximum;
            nudMinVal.Value = mMyCommand.Minimum;

            if (mMyCommand.VariableType == Enums.VariableTypes.PlayerVariable)
            {
                rdoPlayerVariables.Checked = true;
            }
            else if (mMyCommand.VariableType == Enums.VariableTypes.ServerVariable)
            {
                rdoGlobalVariables.Checked = true;
            }
            else if (mMyCommand.VariableType == Enums.VariableTypes.GuildVariable)
            {
                rdoGuildVariables.Checked = true;
            }

            LoadVariableList();
            InitLocalization();
            mLoading = false;
        }

        private void InitLocalization()
        {
            grpInput.Text = Strings.EventInput.title;
            lblText.Text = Strings.EventInput.text;
            lblTitle.Text = Strings.EventInput.titlestr;
            lblCommands.Text = Strings.EventInput.commands;
            btnSave.Text = Strings.EventInput.okay;
            btnCancel.Text = Strings.EventInput.cancel;
            rdoPlayerVariables.Text = Strings.EventInput.playervariable;
            rdoGlobalVariables.Text = Strings.EventInput.globalvariable;
            rdoGuildVariables.Text = Strings.EventInput.guildvariable;
        }

        private void LoadVariableList()
        {
            cmbVariable.Items.Clear();
            if (rdoPlayerVariables.Checked)
            {
                cmbVariable.Items.AddRange(PlayerVariableBase.Names);
                cmbVariable.SelectedIndex = PlayerVariableBase.ListIndex(mMyCommand.VariableId);

                if (cmbVariable.SelectedIndex != -1)
                {
                    UpdateMinMaxValues(
                        PlayerVariableBase.Get(PlayerVariableBase.IdFromList(cmbVariable.SelectedIndex)).Type
                    );
                }
            }
            else if (rdoGlobalVariables.Checked)
            {
                cmbVariable.Items.AddRange(ServerVariableBase.Names);
                cmbVariable.SelectedIndex = ServerVariableBase.ListIndex(mMyCommand.VariableId);

                if (cmbVariable.SelectedIndex != -1)
                {
                    UpdateMinMaxValues(
                        ServerVariableBase.Get(ServerVariableBase.IdFromList(cmbVariable.SelectedIndex)).Type
                    );
                }
            }
            else if (rdoGuildVariables.Checked)
            {
                cmbVariable.Items.AddRange(GuildVariableBase.Names);
                cmbVariable.SelectedIndex = GuildVariableBase.ListIndex(mMyCommand.VariableId);
                if (cmbVariable.SelectedIndex != -1)
                {
                    UpdateMinMaxValues(
                        GuildVariableBase.Get(GuildVariableBase.IdFromList(cmbVariable.SelectedIndex)).Type
                    );
                }
            }
        }

        private void UpdateMinMaxValues(Enums.VariableDataTypes type)
        {
            lblMaxVal.Show();
            lblMinVal.Show();
            nudMaxVal.Show();
            nudMinVal.Show();

            switch (type)
            {
                case Enums.VariableDataTypes.Integer:
                case Enums.VariableDataTypes.Number:
                    lblMinVal.Text = Strings.EventInput.minval;
                    lblMaxVal.Text = Strings.EventInput.maxval;

                    break;
                case Enums.VariableDataTypes.String:
                    lblMinVal.Text = Strings.EventInput.minlength;
                    lblMaxVal.Text = Strings.EventInput.maxlength;

                    break;
                case Enums.VariableDataTypes.Boolean:
                    lblMaxVal.Hide();
                    lblMinVal.Hide();
                    nudMaxVal.Hide();
                    nudMinVal.Hide();

                    break;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.Text = txtText.Text;
            mMyCommand.Title = txtTitle.Text;
            mMyCommand.Maximum = (int) nudMaxVal.Value;
            mMyCommand.Minimum = (int) nudMinVal.Value;

            if (rdoPlayerVariables.Checked)
            {
                mMyCommand.VariableType = Enums.VariableTypes.PlayerVariable;
                mMyCommand.VariableId = PlayerVariableBase.IdFromList(cmbVariable.SelectedIndex);
            }
            else if (rdoGlobalVariables.Checked)
            {
                mMyCommand.VariableType = Enums.VariableTypes.ServerVariable;
                mMyCommand.VariableId = ServerVariableBase.IdFromList(cmbVariable.SelectedIndex);
            }
            else if (rdoGuildVariables.Checked)
            {
                mMyCommand.VariableType = Enums.VariableTypes.GuildVariable;
                mMyCommand.VariableId = GuildVariableBase.IdFromList(cmbVariable.SelectedIndex);
            }

            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }

        private void lblCommands_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(
                "http://www.ascensiongamedev.com/community/topic/749-event-text-variables/"
            );
        }

        private void rdoGlobalVariables_CheckedChanged(object sender, EventArgs e)
        {
            LoadVariableList();
            if (!mLoading && cmbVariable.Items.Count > 0)
            {
                cmbVariable.SelectedIndex = 0;
            }
        }

        private void rdoGuildVariables_CheckedChanged(object sender, EventArgs e)
        {
            LoadVariableList();
            if (!mLoading && cmbVariable.Items.Count > 0)
            {
                cmbVariable.SelectedIndex = 0;
            }
        }

        private void rdoPlayerVariables_CheckedChanged(object sender, EventArgs e)
        {
            LoadVariableList();
            if (!mLoading && cmbVariable.Items.Count > 0)
            {
                cmbVariable.SelectedIndex = 0;
            }
        }

        private void cmbVariable_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rdoPlayerVariables.Checked)
            {
                UpdateMinMaxValues(
                    PlayerVariableBase.Get(PlayerVariableBase.IdFromList(cmbVariable.SelectedIndex)).Type
                );
            }
            else
            {
                UpdateMinMaxValues(
                    ServerVariableBase.Get(ServerVariableBase.IdFromList(cmbVariable.SelectedIndex)).Type
                );
            }
        }

    }

}
