using System;
using System.Windows.Forms;
using Intersect.Editor.Core;
using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Events.Commands;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommandVariable : UserControl
    {
        private readonly FrmEvent mEventEditor;
        private bool mLoading;
        private SetVariableCommand mMyCommand;

        public EventCommandVariable(SetVariableCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            mLoading = true;
            InitLocalization();
            if (mMyCommand.VariableType == VariableTypes.ServerVariable)
            {
                rdoGlobalVariable.Checked = true;
            }
            else
            {
                rdoPlayerVariable.Checked = true;
            }
            mLoading = false;
            InitEditor();
        }

        private void InitLocalization()
        {
            grpSetVariable.Text = Strings.EventSetVariable.title;
            grpVariableSelection.Text = Strings.EventSetVariable.label;
            lblVariable.Text = Strings.EventSetVariable.label;
            rdoGlobalVariable.Text = Strings.EventSetVariable.global;
            rdoPlayerVariable.Text = Strings.EventSetVariable.player;
            optSet.Text = Strings.EventSetVariable.set;
            optAdd.Text = Strings.EventSetVariable.add;
            optSubtract.Text = Strings.EventSetVariable.subtract;
            optRandom.Text = Strings.EventSetVariable.random;
            lblRandomLow.Text = Strings.EventSetVariable.randomlow;
            lblRandomHigh.Text = Strings.EventSetVariable.randomhigh;
            btnSave.Text = Strings.EventSetVariable.okay;
            btnCancel.Text = Strings.EventSetVariable.cancel;
        }

        private void InitEditor()
        {
            cmbVariable.Items.Clear();
            int varCount = 0;
            if (rdoPlayerVariable.Checked)
            {
                cmbVariable.Items.AddRange(PlayerVariableBase.Names);
                cmbVariable.SelectedIndex = PlayerVariableBase.ListIndex(mMyCommand.VariableId);
            }
            else
            {
                cmbVariable.Items.AddRange(ServerVariableBase.Names);
                cmbVariable.SelectedIndex =  ServerVariableBase.ListIndex(mMyCommand.VariableId);
            }
            switch (mMyCommand.ModType)
            {
                case VariableMods.Set:
                    optSet.Checked = true;
                    nudSet.Value = mMyCommand.Value;
                    break;
                case VariableMods.Add:
                    optAdd.Checked = true;
                    nudAdd.Value = mMyCommand.Value;
                    break;
                case VariableMods.Subtract:
                    optSubtract.Checked = true;
                    nudSubtract.Value = mMyCommand.Value;
                    break;
                case VariableMods.Random:
                    optRandom.Checked = true;
                    nudLow.Value = mMyCommand.Value;
                    nudHigh.Value = mMyCommand.HighValue;
                    break;
            }
            UpdateFormElements();
        }

        private void UpdateFormElements()
        {
            nudSet.Enabled = optSet.Checked;
            nudAdd.Enabled = optAdd.Checked;
            nudSubtract.Enabled = optSubtract.Checked;
            nudLow.Enabled = optRandom.Checked;
            nudHigh.Enabled = optRandom.Checked;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int n;
            if (rdoPlayerVariable.Checked)
            {
                mMyCommand.VariableType = VariableTypes.PlayerVariable;
                mMyCommand.VariableId =PlayerVariableBase.IdFromList(cmbVariable.SelectedIndex);
            }
            if (rdoGlobalVariable.Checked)
            {
                mMyCommand.VariableType = VariableTypes.ServerVariable;
                mMyCommand.VariableId = ServerVariableBase.IdFromList(cmbVariable.SelectedIndex);
            }
            if (optSet.Checked)
            {
                mMyCommand.ModType = VariableMods.Set;
                mMyCommand.Value = (int) nudSet.Value;
            }
            else if (optAdd.Checked)
            {
                mMyCommand.ModType = VariableMods.Add;
                mMyCommand.Value = (int) nudAdd.Value;
            }
            else if (optSubtract.Checked)
            {
                mMyCommand.ModType = VariableMods.Subtract;
                mMyCommand.Value = (int) nudSubtract.Value;
            }
            else if (optRandom.Checked)
            {
                mMyCommand.ModType = VariableMods.Random;
                mMyCommand.Value = (int) nudLow.Value;
                mMyCommand.HighValue = (int) nudHigh.Value;
                if (mMyCommand.HighValue < mMyCommand.Value)
                {
                    n = mMyCommand.Value;
                    mMyCommand.Value = mMyCommand.HighValue;
                    mMyCommand.HighValue = n;
                }
            }
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }

        private void optSet_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFormElements();
        }

        private void optAdd_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFormElements();
        }

        private void optSubtract_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFormElements();
        }

        private void optRandom_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFormElements();
        }

        private void rdoPlayerVariable_CheckedChanged(object sender, EventArgs e)
        {
            InitEditor();
            if (!mLoading && cmbVariable.Items.Count > 0) cmbVariable.SelectedIndex = 0;
            if (!mLoading) optSet.Checked = true;
            if (!mLoading) nudSet.Value = 0;
        }

        private void rdoGlobalVariable_CheckedChanged(object sender, EventArgs e)
        {
            InitEditor();
            if (!mLoading && cmbVariable.Items.Count > 0) cmbVariable.SelectedIndex = 0;
            if (!mLoading) optSet.Checked = true;
            if (!mLoading) nudSet.Value = 0;
        }
    }
}