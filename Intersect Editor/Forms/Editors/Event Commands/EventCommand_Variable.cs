using System;
using System.Windows.Forms;
using Intersect.Editor.Core;
using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.GameObjects.Events;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommandVariable : UserControl
    {
        private readonly FrmEvent mEventEditor;
        private bool mLoading;
        private EventCommand mMyCommand;

        public EventCommandVariable(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            mLoading = true;
            InitLocalization();
            if (mMyCommand.Ints[0] == (int) SwitchVariableTypes.ServerVariable)
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
                cmbVariable.Items.AddRange(Database.GetGameObjectList(GameObjectType.PlayerVariable));
                cmbVariable.SelectedIndex =
                    Database.GameObjectListIndex(GameObjectType.PlayerVariable, mMyCommand.Ints[1]);
            }
            else
            {
                cmbVariable.Items.AddRange(Database.GetGameObjectList(GameObjectType.ServerVariable));
                cmbVariable.SelectedIndex =
                    Database.GameObjectListIndex(GameObjectType.ServerVariable, mMyCommand.Ints[1]);
            }
            switch (mMyCommand.Ints[2])
            {
                case 0:
                    optSet.Checked = true;
                    nudSet.Value = mMyCommand.Ints[3];
                    break;
                case 1:
                    optAdd.Checked = true;
                    nudAdd.Value = mMyCommand.Ints[3];
                    break;
                case 2:
                    optSubtract.Checked = true;
                    nudSubtract.Value = mMyCommand.Ints[3];
                    break;
                case 3:
                    optRandom.Checked = true;
                    nudLow.Value = mMyCommand.Ints[3];
                    nudHigh.Value = mMyCommand.Ints[4];
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
                mMyCommand.Ints[0] = (int) SwitchVariableTypes.PlayerVariable;
                mMyCommand.Ints[1] = Database.GameObjectIdFromList(GameObjectType.PlayerVariable,
                    cmbVariable.SelectedIndex);
            }
            if (rdoGlobalVariable.Checked)
            {
                mMyCommand.Ints[0] = (int) SwitchVariableTypes.ServerVariable;
                mMyCommand.Ints[1] = Database.GameObjectIdFromList(GameObjectType.ServerVariable,
                    cmbVariable.SelectedIndex);
            }
            if (optSet.Checked)
            {
                mMyCommand.Ints[2] = 0;
                mMyCommand.Ints[3] = (int) nudSet.Value;
            }
            else if (optAdd.Checked)
            {
                mMyCommand.Ints[2] = 1;
                mMyCommand.Ints[3] = (int) nudAdd.Value;
            }
            else if (optSubtract.Checked)
            {
                mMyCommand.Ints[2] = 2;
                mMyCommand.Ints[3] = (int) nudSubtract.Value;
            }
            else if (optRandom.Checked)
            {
                mMyCommand.Ints[2] = 3;
                mMyCommand.Ints[3] = (int) nudLow.Value;
                mMyCommand.Ints[4] = (int) nudHigh.Value;
                if (mMyCommand.Ints[4] < mMyCommand.Ints[3])
                {
                    n = mMyCommand.Ints[3];
                    mMyCommand.Ints[3] = mMyCommand.Ints[4];
                    mMyCommand.Ints[4] = n;
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